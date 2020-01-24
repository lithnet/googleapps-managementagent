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
    internal class ApiInterfaceCourseTeachers : IApiInterface
    {
        private IManagementAgentParameters config;

        private ApiInterfaceCourse courseApiInterface;

        public string Api => "courseteachers";

        public ApiInterfaceCourseTeachers(IManagementAgentParameters config, ApiInterfaceCourse courseApiInterface)
        {
            this.config = config;
            this.courseApiInterface = courseApiInterface;
        }

        public void ApplyChanges(CSEntryChange csentry, CSEntryChange committedChanges, SchemaType type, ref object target, bool patch = false)
        {
            Logger.WriteLine($"Processing teachers for course {csentry.DN}");

            this.GetTeacherChangesFromCSEntryChange(csentry, out CourseTeachers teachersToAdd, out CourseTeachers teachersToDelete, out CourseTeachers reportedAdds, out CourseTeachers reportedDeletes, csentry.ObjectModificationType == ObjectModificationType.Replace);

            HashSet<string> allTeachersToDelete = teachersToDelete.GetAllTeachers();
            List<Teacher> allTeachersToAdd = teachersToAdd.ToTeacherList();

            AttributeModificationType modificationType = csentry.ObjectModificationType == ObjectModificationType.Update ? AttributeModificationType.Update : AttributeModificationType.Add;
            try
            {
                if (csentry.ObjectModificationType != ObjectModificationType.Add && allTeachersToDelete.Count > 0)
                {
                    try
                    {
                        this.config.ClassroomService.TeacherFactory.RemoveTeachers(csentry.DN, allTeachersToDelete.ToList(), false);

                        foreach (string teacher in allTeachersToDelete)
                        {
                            Logger.WriteLine($"Deleted teacher {teacher} from course {csentry.DN}", LogLevel.Debug);
                        }

                        if (allTeachersToDelete.Count == 1)
                        {
                            Logger.WriteLine($"Deleted {allTeachersToDelete.Count} teachers from course {csentry.DN}");
                        }
                        else
                        {
                            Logger.WriteLine($"Deleted {allTeachersToDelete.Count} teachers from course {csentry.DN}");
                        }
                    }
                    catch (AggregateCourseStudentException ex)
                    {
                        Logger.WriteLine("The following teacher removals failed");
                        foreach (Exception e in ex.Exceptions)
                        {
                            Logger.WriteException(e);
                        }

                        reportedDeletes.RemoveTeachers(ex.FailedStudents);
                        throw;
                    }
                }

                if (allTeachersToAdd.Count > 0)
                {
                    try
                    {
                        this.config.ClassroomService.TeacherFactory.AddTeachers(csentry.DN, allTeachersToAdd, false);

                        foreach (Teacher teacher in allTeachersToAdd)
                        {
                            Logger.WriteLine($"Added {teacher.UserId} to {csentry.DN}", LogLevel.Debug);
                        }

                        if (allTeachersToAdd.Count == 1)
                        {
                            Logger.WriteLine($"Added {allTeachersToAdd.Count} teacher to {csentry.DN}");
                        }
                        else
                        {
                            Logger.WriteLine($"Added {allTeachersToAdd.Count} teachers to {csentry.DN}");
                        }
                    }
                    catch (AggregateCourseTeacherException ex)
                    {
                        Logger.WriteLine("The following teacher additions failed");
                        foreach (Exception e in ex.Exceptions)
                        {
                            Logger.WriteException(e);
                        }

                        reportedAdds.RemoveTeachers(ex.FailedTeachers);
                        throw;
                    }
                }
            }
            finally
            {
                ApiInterfaceCourseTeachers.AddAttributeChange(SchemaConstants.Teachers, modificationType, reportedDeletes.Teachers.ToValueChange(ValueModificationType.Delete), committedChanges.AttributeChanges);
                ApiInterfaceCourseTeachers.AddAttributeChange(SchemaConstants.Teachers, modificationType, reportedAdds.Teachers.ToValueChange(ValueModificationType.Add), committedChanges.AttributeChanges);
            }

            Logger.WriteLine($"Processed teachers for course {csentry.DN}");
        }

        public IEnumerable<AttributeChange> GetChanges(string dn, ObjectModificationType modType, SchemaType type, object source)
        {
            if (!(source is CourseTeachers teachers))
            {
                if (!(source is GoogleCourse course))
                {
                    throw new InvalidOperationException();
                }
                else
                {
                    teachers = course.Teachers;
                }
            }

            foreach (IAttributeAdapter typeDef in ManagementAgent.Schema[SchemaConstants.Course].AttributeAdapters.Where(t => t.Api == this.Api))
            {
                foreach (string attributeName in typeDef.MmsAttributeNames)
                {
                    if (type.HasAttribute(attributeName))
                    {
                        foreach (AttributeChange change in typeDef.CreateAttributeChanges(dn, modType, teachers))
                        {
                            yield return change;
                        }
                    }
                }
            }
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

        private void GetTeacherChangesFromCSEntryChange(CSEntryChange csentry, out CourseTeachers adds, out CourseTeachers deletes, out CourseTeachers reportedAdds, out CourseTeachers reportedDeletes, bool replacing)
        {
            adds = new CourseTeachers();
            deletes = new CourseTeachers();
            reportedAdds = new CourseTeachers();
            reportedDeletes = new CourseTeachers();

            CourseTeachers existingTeachers;

            if (replacing)
            {
                // Translate existing teachers from Id to PrimaryEmail
                existingTeachers = new CourseTeachers(this.courseApiInterface.TranslateMembers(this.config.ClassroomService.TeacherFactory.GetCourseTeachers(csentry.DN).GetAllTeachers()));
            }
            else
            {
                existingTeachers = new CourseTeachers();
            }

            this.GetTeacherChangesFromCSEntryChange(csentry, adds.Teachers, deletes.Teachers, existingTeachers.Teachers, SchemaConstants.Teachers, replacing);

            reportedAdds.MergeTeachers(adds);
            reportedDeletes.MergeTeachers(deletes);
        }

        private void GetTeacherChangesFromCSEntryChange(CSEntryChange csentry, HashSet<string> adds, HashSet<string> deletes, HashSet<string> existingMembers, string attributeName, bool replacing)
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
