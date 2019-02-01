using System;
using System.Collections.Generic;
using Lithnet.GoogleApps.MA.AttributeAdapters;
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
                Name = TypeName,
                AnchorAttributeNames = new[] { "id" },
                SupportsPatch = true

            };

            type.ApiInterface = new ApiInterfaceCourse(type, config);

            PatchableAdapterPropertyValue alternateLink = new PatchableAdapterPropertyValue
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


            PatchableAdapterPropertyValue courseGroupEmail = new PatchableAdapterPropertyValue
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

            PatchableAdapterPropertyValue courseState = new PatchableAdapterPropertyValue
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

            PatchableAdapterPropertyValue calendarId = new PatchableAdapterPropertyValue
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

            PatchableAdapterPropertyValue creationTime = new PatchableAdapterPropertyValue
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

            PatchableAdapterPropertyValue descriptionHeading = new PatchableAdapterPropertyValue
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


            PatchableAdapterPropertyValue description = new PatchableAdapterPropertyValue
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


            PatchableAdapterPropertyValue enrollmentCode = new PatchableAdapterPropertyValue
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

            PatchableAdapterPropertyValue eTag = new PatchableAdapterPropertyValue
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

            PatchableAdapterPropertyValue guardiansEnabled = new PatchableAdapterPropertyValue
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

            PatchableAdapterPropertyValue ownerId = new PatchableAdapterPropertyValue
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

            PatchableAdapterPropertyValue room = new PatchableAdapterPropertyValue
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


            PatchableAdapterPropertyValue section = new PatchableAdapterPropertyValue
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

            PatchableAdapterPropertyValue teacherGroupEmail = new PatchableAdapterPropertyValue
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

            PatchableAdapterPropertyValue updateTime = new PatchableAdapterPropertyValue
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


            PatchableAdapterPropertyValue name = new PatchableAdapterPropertyValue
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


            PatchableAdapterPropertyValue id = new PatchableAdapterPropertyValue
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