using System;
using System.Collections.Generic;
using System.Linq;
using Google;
using Google.Apis.Admin.Directory.directory_v1.Data;
using Google.Apis.Classroom.v1.Data;
using Lithnet.GoogleApps.ManagedObjects;
using Lithnet.Logging;
using Lithnet.MetadirectoryServices;
using Microsoft.MetadirectoryServices;

namespace Lithnet.GoogleApps.MA
{
    internal class ApiInterfaceCourseStudents : IApiInterface
    {
        private IManagementAgentParameters config;

        private ApiInterfaceCourse CourseApiInterface;

        public string Api => "coursestudents";

        public ApiInterfaceCourseStudents(IManagementAgentParameters config, ApiInterfaceCourse courseApiInterface)
        {
            this.config = config;
            this.CourseApiInterface = courseApiInterface;
        }

        public IList<AttributeChange> ApplyChanges(CSEntryChange csentry, SchemaType type, ref object target, bool patch = false)
        {
            Logger.WriteLine($"Processing students for course {csentry.DN}");
            List<AttributeChange> changes = new List<AttributeChange>();

            this.GetStudentChangesFromCSEntryChange(csentry, out CourseStudents studentsToAdd, out CourseStudents studentsToDelete, out CourseStudents reportedAdds, out CourseStudents reportedDeletes, csentry.ObjectModificationType == ObjectModificationType.Replace);

            HashSet<string> allStudentsToDelete = studentsToDelete.GetAllStudents();
            List<Student> allStudentsToAdd = studentsToAdd.ToStudentList();

            AttributeModificationType modificationType = csentry.ObjectModificationType == ObjectModificationType.Update ? AttributeModificationType.Update : AttributeModificationType.Add;
            try
            {
                if (csentry.ObjectModificationType != ObjectModificationType.Add && allStudentsToDelete.Count > 0)
                {
                    try
                    {
                        this.config.ClassroomService.StudentFactory.RemoveStudents(csentry.DN, allStudentsToDelete.ToList(), false);

                        foreach (string student in allStudentsToDelete)
                        {
                            Logger.WriteLine($"Deleted student {student} from course {csentry.DN}", LogLevel.Debug);
                        }

                        if (allStudentsToDelete.Count == 1)
                        {
                            Logger.WriteLine($"Deleted {allStudentsToDelete.Count} student from course {csentry.DN}");
                        }
                        else
                        {
                            Logger.WriteLine($"Deleted {allStudentsToDelete.Count} students from course {csentry.DN}");
                        }
                    }
                    catch (AggregateCourseStudentException ex)
                    {
                        Logger.WriteLine("The following students removals failed");
                        foreach (Exception e in ex.Exceptions)
                        {
                            Logger.WriteException(e);
                        }

                        reportedDeletes.RemoveStudents(ex.FailedStudents);
                        throw;
                    }
                }

                if (allStudentsToAdd.Count > 0)
                {
                    try
                    {
                        this.config.ClassroomService.StudentFactory.AddStudents(csentry.DN, allStudentsToAdd, false);

                        foreach (Student student in allStudentsToAdd)
                        {
                            Logger.WriteLine($"Added student {student.UserId} to {csentry.DN}", LogLevel.Debug);
                        }

                        if (allStudentsToAdd.Count == 1)
                        {
                            Logger.WriteLine($"Added {allStudentsToAdd.Count} student to {csentry.DN}");
                        }
                        else
                        {
                            Logger.WriteLine($"Added {allStudentsToAdd.Count} students to {csentry.DN}");
                        }
                    }
                    catch (AggregateCourseStudentException ex)
                    {
                        Logger.WriteLine("The following student additions failed");
                        foreach (Exception e in ex.Exceptions)
                        {
                            Logger.WriteException(e);
                        }

                        reportedAdds.RemoveStudents(ex.FailedStudents);
                        throw;
                    }
                }
            }
            finally
            {
                ApiInterfaceCourseStudents.AddAttributeChange(SchemaConstants.Students, modificationType, reportedDeletes.Students.ToValueChange(ValueModificationType.Delete), changes);
                ApiInterfaceCourseStudents.AddAttributeChange(SchemaConstants.Students, modificationType, reportedAdds.Students.ToValueChange(ValueModificationType.Add), changes);
            }

            Logger.WriteLine($"Processed students for course {csentry.DN}");
            return changes;
        }

        public IList<AttributeChange> GetChanges(string dn, ObjectModificationType modType, SchemaType type, object source)
        {
            List<AttributeChange> attributeChanges = new List<AttributeChange>();

            CourseStudents students = source as CourseStudents;

            if (students == null)
            {
                GoogleCourse course = source as GoogleCourse;

                if (course == null)
                {
                    throw new InvalidOperationException();
                }
                else
                {
                    students = course.Students;
                }
            }

            foreach (IAttributeAdapter typeDef in ManagementAgent.Schema[SchemaConstants.Course].AttributeAdapters.Where(t => t.Api == this.Api))
            {
                foreach (string attributeName in typeDef.MmsAttributeNames)
                {
                    if (type.HasAttribute(attributeName))
                    {
                        attributeChanges.AddRange(typeDef.CreateAttributeChanges(dn, modType, students));
                    }
                }
            }

            return attributeChanges;
        }

        private static void AddAttributeChange(string attributeName, AttributeModificationType modificationType, IList<ValueChange> changes, IList<AttributeChange> attributeChanges)
        {
            AttributeChange existingChange = attributeChanges.FirstOrDefault(t => t.Name == attributeName);

            if (modificationType == AttributeModificationType.Delete)
            {
                if (existingChange != null)
                {
                    attributeChanges.Remove(existingChange);
                }

                attributeChanges.Add(AttributeChange.CreateAttributeDelete(attributeName));
                return;
            }

            if (changes == null || changes.Count == 0)
            {
                return;
            }

            IList<object> adds;
            switch (modificationType)
            {
                case AttributeModificationType.Add:
                    if (existingChange != null)
                    {
                        foreach (ValueChange valueChange in changes.Where(t => t.ModificationType == ValueModificationType.Add))
                        {
                            existingChange.ValueChanges.Add(valueChange);
                        }
                    }
                    else
                    {
                        adds = changes.Where(t => t.ModificationType == ValueModificationType.Add).Select(t => t.Value).ToList();

                        if (adds.Count > 0)
                        {
                            attributeChanges.Add(AttributeChange.CreateAttributeAdd(attributeName, adds));
                        }
                    }
                    break;

                case AttributeModificationType.Replace:
                    if (existingChange != null)
                    {
                        attributeChanges.Remove(existingChange);
                    }

                    adds = changes.Where(t => t.ModificationType == ValueModificationType.Add).Select(t => t.Value).ToList();
                    if (adds.Count > 0)
                    {
                        attributeChanges.Add(AttributeChange.CreateAttributeReplace(attributeName, adds));
                    }

                    break;

                case AttributeModificationType.Update:
                    if (existingChange != null)
                    {
                        if (existingChange.ModificationType != AttributeModificationType.Update)
                        {
                            throw new InvalidOperationException();
                        }

                        foreach (ValueChange valueChange in changes)
                        {
                            existingChange.ValueChanges.Add(valueChange);
                        }
                    }
                    else
                    {
                        if (changes.Count > 0)
                        {
                            attributeChanges.Add(AttributeChange.CreateAttributeUpdate(attributeName, changes));
                        }
                    }

                    break;

                case AttributeModificationType.Delete:
                case AttributeModificationType.Unconfigured:
                default:
                    throw new InvalidOperationException();
            }
        }

        private void GetStudentChangesFromCSEntryChange(CSEntryChange csentry, out CourseStudents adds, out CourseStudents deletes, out CourseStudents reportedAdds, out CourseStudents reportedDeletes, bool replacing)
        {
            adds = new CourseStudents();
            deletes = new CourseStudents();
            reportedAdds = new CourseStudents();
            reportedDeletes = new CourseStudents();

            CourseStudents existingStudents;

            if (replacing)
            {
                // Translate existing students from Id to PrimaryEmail
                existingStudents = new CourseStudents(this.CourseApiInterface.TranslateMembers(this.config.ClassroomService.StudentFactory.GetCourseStudents(csentry.DN).GetAllStudents()));
            }
            else
            {
                existingStudents = new CourseStudents();
            }

            this.GetStudentChangesFromCSEntryChange(csentry, adds.Students, deletes.Students, existingStudents.Students, SchemaConstants.Students, replacing);

            reportedAdds.MergeStudents(adds);
            reportedDeletes.MergeStudents(deletes);
        }

        private void GetStudentChangesFromCSEntryChange(CSEntryChange csentry, HashSet<string> adds, HashSet<string> deletes, HashSet<string> existingMembers, string attributeName, bool replacing)
        {
            if (replacing)
            {
                foreach (string address in csentry.GetValueAdds<string>(attributeName))
                {
                    adds.Add(address);
                }

                foreach (string address in deletes.Except(adds))
                {
                    deletes.Add(address);
                }

                return;
            }
            else
            {
                AttributeChange change = csentry.AttributeChanges.FirstOrDefault(t => t.Name == attributeName);

                if (change == null)
                {
                    return;
                }

                switch (change.ModificationType)
                {
                    case AttributeModificationType.Add:
                        foreach (string address in csentry.GetValueAdds<string>(attributeName))
                        {
                            adds.Add(address);
                        }
                        break;

                    case AttributeModificationType.Delete:
                        foreach (string member in existingMembers)
                        {
                            deletes.Add(member);
                        }

                        break;

                    case AttributeModificationType.Replace:
                        IList<string> newMembers = csentry.GetValueAdds<string>(attributeName);
                        foreach (string address in newMembers)
                        {
                            adds.Add(address);
                        }

                        foreach (string member in existingMembers.Except(newMembers))
                        {
                            deletes.Add(member);
                        }

                        break;

                    case AttributeModificationType.Update:
                        foreach (string address in csentry.GetValueDeletes<string>(attributeName))
                        {
                            deletes.Add(address);
                        }

                        foreach (string address in csentry.GetValueAdds<string>(attributeName))
                        {
                            adds.Add(address);
                        }

                        break;

                    case AttributeModificationType.Unconfigured:
                    default:
                        throw new NotSupportedException("The modification type was unknown or unsupported");
                }
            }
        }
    }
}
