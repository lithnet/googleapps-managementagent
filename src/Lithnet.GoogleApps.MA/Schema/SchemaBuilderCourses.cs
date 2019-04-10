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
                FieldName = "alternateLink",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportOnly,
                AttributeName = "alternateLink",
                PropertyName = "AlternateLink",
                Api = "classroom",
                SupportsPatch = false
            };

            type.AttributeAdapters.Add(alternateLink);

            AdapterPropertyValue courseGroupEmail = new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                FieldName = "courseGroupEmail",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportOnly,
                AttributeName = "courseGroupEmail",
                PropertyName = "CourseGroupEmail",
                Api = "classroom",
                SupportsPatch = false
            };

            type.AttributeAdapters.Add(courseGroupEmail);

            AdapterPropertyValue courseState = new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                FieldName = "courseState",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                AttributeName = "courseState",
                PropertyName = "CourseState",
                Api = "classroom",
                SupportsPatch = true
            };

            type.AttributeAdapters.Add(courseState);

            AdapterPropertyValue calendarId = new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                FieldName = "calendarId",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportOnly,
                AttributeName = "calendarId",
                PropertyName = "CalendarId",
                Api = "classroom",
                SupportsPatch = false
            };

            type.AttributeAdapters.Add(calendarId);

            AdapterPropertyValue creationTime = new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                FieldName = "creationTime",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportOnly,
                AttributeName = "creationTime",
                PropertyName = "CreationTime",
                Api = "classroom",
                SupportsPatch = false
            };

            type.AttributeAdapters.Add(creationTime);

            AdapterPropertyValue descriptionHeading = new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                FieldName = "descriptionHeading",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                AttributeName = "descriptionHeading",
                PropertyName = "DescriptionHeading",
                Api = "classroom",
                SupportsPatch = true
            };

            type.AttributeAdapters.Add(descriptionHeading);

            AdapterPropertyValue description = new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                FieldName = "description",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                AttributeName = "description",
                PropertyName = "Description",
                Api = "classroom",
                SupportsPatch = true
            };

            type.AttributeAdapters.Add(description);

            AdapterPropertyValue enrollmentCode = new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                FieldName = "enrollmentCode",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportOnly,
                AttributeName = "enrollmentCode",
                PropertyName = "EnrollmentCode",
                Api = "classroom",
                SupportsPatch = false
            };

            type.AttributeAdapters.Add(enrollmentCode);

            AdapterPropertyValue eTag = new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                FieldName = "eTag",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                AttributeName = "eTag",
                PropertyName = "ETag",
                Api = "classroom",
                SupportsPatch = false
            };

            type.AttributeAdapters.Add(eTag);

            AdapterPropertyValue guardiansEnabled = new AdapterPropertyValue
            {
                AttributeType = AttributeType.Boolean,
                FieldName = "guardiansEnabled",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportOnly,
                AttributeName = "guardiansEnabled",
                PropertyName = "GuardiansEnabled",
                Api = "classroom",
                SupportsPatch = false
            };

            type.AttributeAdapters.Add(guardiansEnabled);

            AdapterPropertyValue ownerId = new AdapterPropertyValue
            {
                AttributeType = AttributeType.Reference,
                FieldName = "ownerId",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                AttributeName = "ownerId",
                PropertyName = "OwnerId",
                Api = "classroom",
                SupportsPatch = true
            };

            type.AttributeAdapters.Add(ownerId);

            AdapterPropertyValue room = new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                FieldName = "room",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                AttributeName = "room",
                PropertyName = "Room",
                Api = "classroom",
                SupportsPatch = true
            };

            type.AttributeAdapters.Add(room);

            AdapterPropertyValue section = new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                FieldName = "section",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                AttributeName = "section",
                PropertyName = "Section",
                Api = "classroom",
                SupportsPatch = true
            };

            type.AttributeAdapters.Add(section);

            AdapterPropertyValue teacherGroupEmail = new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                FieldName = "teacherGroupEmail",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportOnly,
                AttributeName = "teacherGroupEmail",
                PropertyName = "TeacherGroupEmail",
                Api = "classroom",
                SupportsPatch = false
            };

            type.AttributeAdapters.Add(teacherGroupEmail);

            AdapterPropertyValue updateTime = new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                FieldName = "updateTime",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportOnly,
                AttributeName = "updateTime",
                PropertyName = "UpdateTime",
                Api = "classroom",
                SupportsPatch = false
            };

            type.AttributeAdapters.Add(updateTime);

            AdapterPropertyValue name = new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                FieldName = "name",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                AttributeName = "name",
                PropertyName = "Name",
                Api = "classroom",
                SupportsPatch = true
            };

            type.AttributeAdapters.Add(name);

            AdapterPropertyValue id = new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                FieldName = "id",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportOnly,
                AttributeName = "id",
                PropertyName = "Id",
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
                FieldName = "userId",
                Operation = AttributeOperation.ImportExport,
                AttributeName = "students",
                PropertyName = "Students",
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
                FieldName = "userId",
                Operation = AttributeOperation.ImportExport,
                AttributeName = "teachers",
                PropertyName = "Teachers",
                Api = "courseteachers",
                SupportsPatch = true
            };

            type.AttributeAdapters.Add(teachers);
        }
    }
}