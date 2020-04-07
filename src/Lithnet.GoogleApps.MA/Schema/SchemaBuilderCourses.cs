using System;
using System.Collections.Generic;
using Microsoft.MetadirectoryServices;

namespace Lithnet.GoogleApps.MA
{
    internal class SchemaBuilderCourses : ISchemaTypeBuilder
    {
        public string TypeName => "course";

        public IEnumerable<MASchemaType> GetSchemaTypes(IManagementAgentParameters config)
        {
            MASchemaType type = new MASchemaType
            {
                AttributeAdapters = new List<IAttributeAdapter>(),
                Name = this.TypeName,
                AnchorAttributeNames = new[] { "id" },
                SupportsPatch = true
            };

            type.ApiInterface = new ApiInterfaceCourse(type, config);

            AdapterPropertyValue alternateLink = new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                GoogleApiFieldName = "alternateLink",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportOnly,
                MmsAttributeName = "alternateLink",
                ManagedObjectPropertyName = "AlternateLink",
                Api = "classroom",
                SupportsPatch = false
            };

            type.AttributeAdapters.Add(alternateLink);

            AdapterPropertyValue courseGroupEmail = new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                GoogleApiFieldName = "courseGroupEmail",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportOnly,
                MmsAttributeName = "courseGroupEmail",
                ManagedObjectPropertyName = "CourseGroupEmail",
                Api = "classroom",
                SupportsPatch = false
            };

            type.AttributeAdapters.Add(courseGroupEmail);

            AdapterPropertyValue courseState = new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                GoogleApiFieldName = "courseState",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                MmsAttributeName = "courseState",
                ManagedObjectPropertyName = "CourseState",
                Api = "classroom",
                SupportsPatch = true
            };

            type.AttributeAdapters.Add(courseState);

            AdapterPropertyValue calendarId = new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                GoogleApiFieldName = "calendarId",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportOnly,
                MmsAttributeName = "calendarId",
                ManagedObjectPropertyName = "CalendarId",
                Api = "classroom",
                SupportsPatch = false
            };

            type.AttributeAdapters.Add(calendarId);

            AdapterPropertyValue creationTime = new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                GoogleApiFieldName = "creationTime",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportOnly,
                MmsAttributeName = "creationTime",
                ManagedObjectPropertyName = "CreationTime",
                Api = "classroom",
                SupportsPatch = false
            };

            type.AttributeAdapters.Add(creationTime);

            AdapterPropertyValue descriptionHeading = new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                GoogleApiFieldName = "descriptionHeading",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                MmsAttributeName = "descriptionHeading",
                ManagedObjectPropertyName = "DescriptionHeading",
                Api = "classroom",
                SupportsPatch = true
            };

            type.AttributeAdapters.Add(descriptionHeading);

            AdapterPropertyValue description = new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                GoogleApiFieldName = "description",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                MmsAttributeName = "description",
                ManagedObjectPropertyName = "Description",
                Api = "classroom",
                SupportsPatch = true
            };

            type.AttributeAdapters.Add(description);

            AdapterPropertyValue enrollmentCode = new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                GoogleApiFieldName = "enrollmentCode",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportOnly,
                MmsAttributeName = "enrollmentCode",
                ManagedObjectPropertyName = "EnrollmentCode",
                Api = "classroom",
                SupportsPatch = false
            };

            type.AttributeAdapters.Add(enrollmentCode);

            AdapterPropertyValue eTag = new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                GoogleApiFieldName = "eTag",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                MmsAttributeName = "eTag",
                ManagedObjectPropertyName = "ETag",
                Api = "classroom",
                SupportsPatch = false
            };

            type.AttributeAdapters.Add(eTag);

            AdapterPropertyValue guardiansEnabled = new AdapterPropertyValue
            {
                AttributeType = AttributeType.Boolean,
                GoogleApiFieldName = "guardiansEnabled",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportOnly,
                MmsAttributeName = "guardiansEnabled",
                ManagedObjectPropertyName = "GuardiansEnabled",
                Api = "classroom",
                SupportsPatch = false
            };

            type.AttributeAdapters.Add(guardiansEnabled);

            AdapterPropertyValue ownerId = new AdapterPropertyValue
            {
                AttributeType = AttributeType.Reference,
                GoogleApiFieldName = "ownerId",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                MmsAttributeName = "ownerId",
                ManagedObjectPropertyName = "OwnerId",
                Api = "classroom",
                SupportsPatch = true
            };

            type.AttributeAdapters.Add(ownerId);

            AdapterPropertyValue room = new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                GoogleApiFieldName = "room",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                MmsAttributeName = "room",
                ManagedObjectPropertyName = "Room",
                Api = "classroom",
                SupportsPatch = true
            };

            type.AttributeAdapters.Add(room);

            AdapterPropertyValue section = new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                GoogleApiFieldName = "section",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                MmsAttributeName = "section",
                ManagedObjectPropertyName = "Section",
                Api = "classroom",
                SupportsPatch = true
            };

            type.AttributeAdapters.Add(section);

            AdapterPropertyValue teacherGroupEmail = new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                GoogleApiFieldName = "teacherGroupEmail",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportOnly,
                MmsAttributeName = "teacherGroupEmail",
                ManagedObjectPropertyName = "TeacherGroupEmail",
                Api = "classroom",
                SupportsPatch = false
            };

            type.AttributeAdapters.Add(teacherGroupEmail);

            AdapterPropertyValue updateTime = new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                GoogleApiFieldName = "updateTime",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportOnly,
                MmsAttributeName = "updateTime",
                ManagedObjectPropertyName = "UpdateTime",
                Api = "classroom",
                SupportsPatch = false
            };

            type.AttributeAdapters.Add(updateTime);

            AdapterPropertyValue name = new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                GoogleApiFieldName = "name",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                MmsAttributeName = "name",
                ManagedObjectPropertyName = "Name",
                Api = "classroom",
                SupportsPatch = true
            };

            type.AttributeAdapters.Add(name);

            AdapterPropertyValue id = new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                GoogleApiFieldName = "id",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportOnly,
                MmsAttributeName = "id",
                ManagedObjectPropertyName = "Id",
                Api = "classroom",
                SupportsPatch = true,
                IsAnchor = true
            };

            type.AttributeAdapters.Add(id);

            SchemaBuilderCourses.AddStudents(type);
            SchemaBuilderCourses.AddTeachers(type);

            yield return type;
        }

        private static void AddStudents(MASchemaType type)
        {
            AdapterCollection<string> students = new AdapterCollection<string>
            {
                AttributeType = AttributeType.Reference,
                GoogleApiFieldName = "userId",
                Operation = AttributeOperation.ImportExport,
                MmsAttributeName = "students",
                ManagedObjectPropertyName = "Students",
                Api = "coursestudents",
                SupportsPatch = true
            };

            type.AttributeAdapters.Add(students);
        }

        private static void AddTeachers(MASchemaType type)
        {
            AdapterCollection<string> teachers = new AdapterCollection<string>
            {
                AttributeType = AttributeType.Reference,
                GoogleApiFieldName = "userId",
                Operation = AttributeOperation.ImportExport,
                MmsAttributeName = "teachers",
                ManagedObjectPropertyName = "Teachers",
                Api = "courseteachers",
                SupportsPatch = true
            };

            type.AttributeAdapters.Add(teachers);
        }
    }
}