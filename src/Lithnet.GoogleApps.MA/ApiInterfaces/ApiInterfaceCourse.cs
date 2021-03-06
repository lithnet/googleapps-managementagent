﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Google;
using Google.Apis.Classroom.v1.Data;
using Lithnet.GoogleApps.ManagedObjects;
using Lithnet.Logging;
using Lithnet.MetadirectoryServices;
using Microsoft.MetadirectoryServices;
using MmsSchema = Microsoft.MetadirectoryServices.Schema;

namespace Lithnet.GoogleApps.MA
{
    internal class ApiInterfaceCourse : IApiInterfaceObject
    {
        private const string DomainPrefix = "d:";
        private const string ProjectPrefix = "p";

        private ApiInterfaceKeyedCollection internalInterfaces;

        private IManagementAgentParameters config;

        protected MASchemaType SchemaType { get; set; }

        protected ConcurrentDictionary<string, string> UserMappingCache { get; set; }

        protected readonly string UserCacheFieldNames = string.Join(",", new string[] { "primaryEmail", "id" });

        public ApiInterfaceCourse(MASchemaType type, IManagementAgentParameters config)
        {
            this.SchemaType = type;
            this.config = config;

            this.internalInterfaces = new ApiInterfaceKeyedCollection
            {
                new ApiInterfaceCourseStudents(config, this),
                new ApiInterfaceCourseTeachers(config, this)
            };

            this.UserMappingCache = new ConcurrentDictionary<string, string>();
        }

        public string Api => "classroom";

        public ObjectModificationType DeltaUpdateType => ObjectModificationType.Update;

        public object CreateInstance(CSEntryChange csentry)
        {
            GoogleCourse c = new GoogleCourse();
            c.Course.Id = csentry.DN;
            return c;
        }

        public object GetInstance(CSEntryChange csentry)
        {
            GoogleCourse googleCourse = new GoogleCourse();
            googleCourse.Course = this.config.ClassroomService.GetCourse(csentry.GetAnchorValueOrDefault<string>("id") ?? csentry.DN);
            string[] fieldNames = new string[] { "primaryEmail", "id" };
            string fields = string.Join(",", fieldNames);
            try
            {
                googleCourse.Course.OwnerId = this.config.UsersService.Get(googleCourse.Course.OwnerId, fields).PrimaryEmail;
            }
            catch (GoogleApiException ex)
            {
                Logger.WriteLine($"Error fetching OwnerId {googleCourse.Course.OwnerId} for Course {googleCourse.Course.Id}");
                Logger.WriteException(ex);
            }
            return googleCourse;
        }

        public void DeleteInstance(CSEntryChange csentry)
        {
            this.config.ClassroomService.Delete(csentry.GetAnchorValueOrDefault<string>("id") ?? csentry.DN);
        }

        public void ApplyChanges(CSEntryChange csentry, CSEntryChange committedChanges, SchemaType type, ref object target, bool patch = false)
        {
            bool hasChanged = false;

            if (!(target is GoogleCourse googleCourse))
            {
                throw new InvalidOperationException();
            }

            hasChanged |= this.SetDNValue(csentry, googleCourse);

            List<string> updateMask = new List<string>();
            foreach (IAttributeAdapter typeDef in this.SchemaType.AttributeAdapters.Where(t => t.Api == this.Api))
            {
                if (typeDef.UpdateField(csentry, googleCourse.Course))
                {
                    hasChanged = true;
                    updateMask.Add(typeDef.GoogleApiFieldName);
                }
            }

            if (csentry.ObjectModificationType == ObjectModificationType.Add)
            {
                // Remove Id if invalid
                if (googleCourse.Course.Id != null && !googleCourse.Course.Id.StartsWith(ProjectPrefix) && !googleCourse.Course.Id.StartsWith(DomainPrefix))
                {
                    googleCourse.Course.Id = null;
                }

                Logger.WriteLine($"Adding course {csentry.DN}");
                googleCourse.Course = this.config.ClassroomService.Add(googleCourse.Course);
                Logger.WriteLine($"Added course {csentry.DN} (Id: {googleCourse.Course.Id})");

                // Get Id and result
                csentry.DN = googleCourse.Course.Id;
                committedChanges.ObjectModificationType = ObjectModificationType.Add;
                committedChanges.DN = this.GetDNValue(googleCourse.Course);
            }

            if (csentry.IsUpdateOrReplace() && hasChanged)
            {
                string id = csentry.GetAnchorValueOrDefault<string>("id");

                if (patch)
                {
                    googleCourse.Course = this.config.ClassroomService.Patch(id, googleCourse.Course, string.Join(",", updateMask));
                }
                else
                {
                    googleCourse.Course = this.config.ClassroomService.Update(googleCourse.Course);
                }
            }

            if (csentry.IsUpdateOrReplace())
            {
                committedChanges.ObjectModificationType = this.DeltaUpdateType;
                committedChanges.DN = this.GetDNValue(googleCourse.Course);
            }

            string ownerId = googleCourse.Course.OwnerId;

            // Reset OwnerId for Delta import
            if (updateMask.Contains(SchemaConstants.OwnerId))
            {
                googleCourse.Course.OwnerId = ownerId;
            }

            foreach (AttributeChange change in this.GetLocalChanges(csentry.DN, csentry.ObjectModificationType, type, googleCourse.Course))
            {
                committedChanges.AttributeChanges.Add(change);
            }

            target = googleCourse.Course;

            foreach (IApiInterface i in this.internalInterfaces)
            {
                i.ApplyChanges(csentry, committedChanges, type, ref target, patch);
            }
        }

        public IEnumerable<AttributeChange> GetChanges(string dn, ObjectModificationType modType, SchemaType type, object source)
        {
            foreach (AttributeChange change in this.GetLocalChanges(dn, modType, type, source))
            {
                yield return change;
            }

            foreach (IApiInterface i in this.internalInterfaces)
            {
                foreach (AttributeChange change in i.GetChanges(dn, modType, type, source))
                {
                    yield return change;
                }
            }
        }

        private List<AttributeChange> GetLocalChanges(string dn, ObjectModificationType modType, SchemaType type, object source)
        {
            List<AttributeChange> attributeChanges = new List<AttributeChange>();

            GoogleCourse googleCourse = source as GoogleCourse;
            Course course = null;

            if (googleCourse != null)
            {
                course = googleCourse.Course;
            }

            if (course == null)
            {
                course = source as Course;
            }

            if (course == null)
            {
                throw new InvalidOperationException();
            }

            foreach (IAttributeAdapter typeDef in this.SchemaType.AttributeAdapters.Where(t => t.Api == this.Api))
            {
                if (typeDef.IsAnchor)
                {
                    continue;
                }

                foreach (AttributeChange change in typeDef.CreateAttributeChanges(dn, modType, course))
                {
                    if (type.HasAttribute(change.Name))
                    {
                        attributeChanges.Add(change);
                    }
                }
            }

            return attributeChanges;
        }

        public string GetAnchorValue(string name, object target)
        {
            Course course;

            if (target is GoogleCourse googleCourse)
            {
                course = googleCourse.Course;
            }
            else
            {
                course = target as Course;
            }

            if (course == null)
            {
                throw new InvalidOperationException();
            }

            return course.Id;
        }

        public string GetDNValue(object target)
        {
            Course course;

            if (target is GoogleCourse googleCourse)
            {
                course = googleCourse.Course;
            }
            else
            {
                course = target as Course;
            }

            if (course == null)
            {
                throw new InvalidOperationException();
            }

            return course.Id;
        }

        public Task GetObjectImportTask(MmsSchema schema, BlockingCollection<object> collection, CancellationToken cancellationToken)
        {
            bool studentsRequired =
              ManagementAgent.Schema[SchemaConstants.Course].AttributeAdapters.Where(u => u.Api == "coursestudents").Any(v =>
              {
                  return v.MmsAttributeNames.Any(attributeName => schema.Types[SchemaConstants.Course].Attributes.Contains(attributeName));
              });

            bool teachersRequired =
               ManagementAgent.Schema[SchemaConstants.Course].AttributeAdapters.Where(u => u.Api == "courseteachers").Any(v =>
               {
                   return v.MmsAttributeNames.Any(attributeName => schema.Types[SchemaConstants.Course].Attributes.Contains(attributeName));
               });

            Task t = new Task(() =>
            {
                Logger.WriteLine("Starting course import task");
                Logger.WriteLine("Requesting students: " + studentsRequired);
                Logger.WriteLine("Requesting teachers: " + teachersRequired);
                Logger.WriteLine("Skip members for ARCHIVED Courses: " + this.config.SkipMemberImportOnArchivedCourses);

                // Fetch all users and cache for performance translation of id to PrimaryEmail
                this.InitializeUserMappingCache();

                foreach (GoogleCourse course in this.config.ClassroomService.GetCourses(this.config.CustomerID, studentsRequired, teachersRequired, this.config.SkipMemberImportOnArchivedCourses, MAConfigurationSection.Configuration.ClassroomApi.ImportThreadsCourseMember))
                {
                    // Translate OwnerId to Email. Use cache.
                    course.Course.OwnerId = this.GetUserPrimaryEmailForId(course.Course.OwnerId);

                    // Translate students and teachers
                    course.Students = new CourseStudents(this.TranslateMembers(course.Students.GetAllStudents()));
                    course.Teachers = new CourseTeachers(this.TranslateMembers(course.Teachers.GetAllTeachers()));

                    collection.Add(this.GetCSEntryForCourse(course, schema));
                    Debug.WriteLine($"Created CSEntryChange for course: {course.Course.Id}");

                    continue;
                }

                Logger.WriteLine("Courses import task complete");
            }, cancellationToken);

            t.Start();

            return t;
        }

        public string GetUserPrimaryEmailForId(string numericId)
        {
            string primaryEmail;
            if (!this.UserMappingCache.TryGetValue(numericId, out primaryEmail))
            {
                try
                {
                    primaryEmail = this.config.UsersService.Get(numericId, this.UserCacheFieldNames).PrimaryEmail;
                    this.UserMappingCache.TryAdd(numericId, primaryEmail);
                }
                catch (GoogleApiException ex)
                {
                    Logger.WriteLine($"Error fetching primary email for id {numericId} for user translation.");
                    Logger.WriteException(ex);
                    return numericId;
                }
            }

            return primaryEmail;
        }

        protected void InitializeUserMappingCache()
        {
            Logger.WriteLine("Fetching all users for Course member mapping");
            string[] fieldNames = new string[] { "primaryEmail", "id" };
            string fields = $"users({string.Join(",", fieldNames)}),nextPageToken";
            IList<User> users = this.config.UsersService.GetUsers(this.config.CustomerID, fields).ToList();
            this.UserMappingCache = new ConcurrentDictionary<string, string>(users.ToDictionary(u => u.Id, u => u.PrimaryEmail));
            Logger.WriteLine($"Done fetching all users for User Course Membership Mapping (count: {users.Count}). Importing courses.");
        }
        public HashSet<string> TranslateMembers(HashSet<string> members)
        {
            return new HashSet<string>(members.Select(m => this.GetUserPrimaryEmailForId(m)),
                StringComparer.CurrentCultureIgnoreCase);
        }

        private CSEntryChange GetCSEntryForCourse(GoogleCourse course, Microsoft.MetadirectoryServices.Schema schema)
        {
            CSEntryChange csentry;

            if (course.Errors.Count > 0)
            {
                csentry = CSEntryChange.Create();
                csentry.ObjectType = "course";
                csentry.ObjectModificationType = ObjectModificationType.Add;
                csentry.DN = course.Course.Id;
                csentry.ErrorCodeImport = MAImportError.ImportErrorCustomContinueRun;
                csentry.ErrorDetail = course.Errors.FirstOrDefault()?.StackTrace;
                csentry.ErrorName = course.Errors.FirstOrDefault()?.Message;
            }
            else
            {
                csentry = ImportProcessor.GetCSEntryChange(course, schema.Types[SchemaConstants.Course], this.config);
            }

            return csentry;
        }

        private bool SetDNValue(CSEntryChange csentry, GoogleCourse course)
        {
            if (csentry.ObjectModificationType != ObjectModificationType.Replace && csentry.ObjectModificationType != ObjectModificationType.Update)
            {
                return false;
            }

            string newDN = csentry.GetNewDNOrDefault<string>();

            if (newDN == null)
            {
                return false;
            }

            throw new NotSupportedException("Renaming the DN of this object is not supported");
        }
    }
}

