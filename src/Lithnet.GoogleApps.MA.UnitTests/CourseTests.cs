using System;
using System.Collections.Concurrent;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using Google;
using Lithnet.GoogleApps.ManagedObjects;
using Microsoft.MetadirectoryServices;
using Lithnet.MetadirectoryServices;
using Google.Apis.Classroom.v1.Data;
using System.Linq;

namespace Lithnet.GoogleApps.MA.UnitTests
{
    [TestClass]
    public class CourseTests
    {
        [TestMethod]
        public void GetCourses()
        {
            var courseSchemaType = UnitTestControl.Schema[SchemaConstants.Course];
            var mmsCourseSchemaType = UnitTestControl.MmsSchema.Types[SchemaConstants.Course];

            ApiInterfaceCourse u = new ApiInterfaceCourse(courseSchemaType, UnitTestControl.TestParameters);

            u.GetObjectImportTask(UnitTestControl.MmsSchema, new BlockingCollection<object>(), CancellationToken.None).Wait();
        }

        [TestMethod]
        public void Add()
        {
            CSEntryChange cs = CSEntryChange.Create();
            cs.ObjectModificationType = ObjectModificationType.Add;
            cs.DN = $"{Guid.NewGuid()}";
            cs.ObjectType = SchemaConstants.Course;

            User t1 = UserTests.CreateUser();
            User t2 = UserTests.CreateUser();
            User s1 = UserTests.CreateUser();
            User s2 = UserTests.CreateUser();

            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("name", "name"));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("ownerId", new List<object>() { t1.PrimaryEmail }));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("teachers", new List<object>() { t1.PrimaryEmail, t2.PrimaryEmail }));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("students", new List<object>() { s1.PrimaryEmail, s2.PrimaryEmail }));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("courseState", "ACTIVE"));

            string id = null;

            try
            {
                CSEntryChangeResult result = ExportProcessor.PutCSEntryChange(cs, UnitTestControl.Schema.GetSchema().Types[SchemaConstants.Course], UnitTestControl.TestParameters);
                id = result.AnchorAttributes["id"].GetValueAdd<string>();

                if (result.ErrorCode != MAExportError.Success)
                {
                    Assert.Fail(result.ErrorName);
                }

                System.Threading.Thread.Sleep(UnitTestControl.PostGoogleOperationSleepInterval);

                Course e = UnitTestControl.TestParameters.ClassroomService.GetCourse(id);
                Assert.AreEqual("name", e.Name);
                Assert.AreEqual("ACTIVE", e.CourseState);

                CollectionAssert.AreEquivalent(new string[] { s1.Id, s2.Id },
                    UnitTestControl.TestParameters.ClassroomService.StudentFactory.GetCourseStudents(id).GetAllStudents().ToArray());

                CollectionAssert.AreEquivalent(new string[] { t1.Id, t2.Id },
                    UnitTestControl.TestParameters.ClassroomService.TeacherFactory.GetCourseTeachers(id).GetAllTeachers().ToArray());
            }
            finally
            {
                UnitTestControl.Cleanup(t1, t2, s1, s2);

                if (id != null)
                {
                    UnitTestControl.TestParameters.ClassroomService.Delete(id);
                }
            }
        }

        [TestMethod]
        public void Delete()
        {
            string id = null;

            User owner = null;
            try
            {
                owner = UserTests.CreateUser();

                Course e = new Course
                {
                    Name = "name",
                    OwnerId = owner.Id
                };

                e = UnitTestControl.TestParameters.ClassroomService.Add(e);
                id = e.Id;

                CSEntryChange cs = CSEntryChange.Create();
                cs.ObjectModificationType = ObjectModificationType.Delete;
                cs.DN = id;
                cs.ObjectType = SchemaConstants.Course;
                cs.AnchorAttributes.Add(AnchorAttribute.Create("id", id));
                CSEntryChangeResult result = ExportProcessor.PutCSEntryChange(cs, UnitTestControl.Schema.GetSchema().Types[SchemaConstants.Course], UnitTestControl.TestParameters);

                if (result.ErrorCode != MAExportError.Success)
                {
                    Assert.Fail(result.ErrorName);
                }

                try
                {
                    System.Threading.Thread.Sleep(UnitTestControl.PostGoogleOperationSleepInterval);
                    e = UnitTestControl.TestParameters.ClassroomService.GetCourse(id);
                    Assert.Fail("The object did not get deleted");
                }
                catch (GoogleApiException ex)
                {
                    if (ex.HttpStatusCode == HttpStatusCode.NotFound)
                    {
                        id = null;
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            finally
            {
                UnitTestControl.Cleanup(owner);

                if (id != null)
                {
                    UnitTestControl.TestParameters.ClassroomService.Delete(id);
                }

            }
        }

        [TestMethod]
        public void Update()
        {
            User owner = null;
            string id = null;
            try
            {
                owner = UserTests.CreateUser();

                Course e = new Course
                {
                    Name = "name",
                    OwnerId = owner.Id
                };

                e = UnitTestControl.TestParameters.ClassroomService.Add(e);
                id = e.Id;

                CSEntryChange cs = CSEntryChange.Create();
                cs.ObjectModificationType = ObjectModificationType.Update;
                cs.DN = id;
                cs.ObjectType = SchemaConstants.Course;
                cs.AnchorAttributes.Add(AnchorAttribute.Create("id", id));

                cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("name", "name2"));


                CSEntryChangeResult result =
                    ExportProcessor.PutCSEntryChange(cs, UnitTestControl.Schema.GetSchema().Types[SchemaConstants.Course], UnitTestControl.TestParameters);

                if (result.ErrorCode != MAExportError.Success)
                {
                    Assert.Fail(result.ErrorName);
                }

                System.Threading.Thread.Sleep(UnitTestControl.PostGoogleOperationSleepInterval);

                e = UnitTestControl.TestParameters.ClassroomService.GetCourse(id);
                Assert.AreEqual("name2", e.Name);
            }
            finally
            {
                UnitTestControl.Cleanup(owner);

                if (id != null)
                {
                    UnitTestControl.TestParameters.ClassroomService.Delete(id);
                }
            }
        }
    }
}
