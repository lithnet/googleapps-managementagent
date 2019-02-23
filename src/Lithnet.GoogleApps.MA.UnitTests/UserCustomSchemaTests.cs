using System;
using System.Collections;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.MetadirectoryServices;
using System.Collections.Generic;
using System.Linq;
using Lithnet.GoogleApps.ManagedObjects;
using Lithnet.MetadirectoryServices;
using Newtonsoft.Json.Linq;
using System.Threading;

namespace Lithnet.GoogleApps.MA.UnitTests
{
    [TestClass]
    public class UserCustomSchemaTests
    {
        private const string TestSchemaName = "TestSchema";

        [TestMethod]
        public void TestMultivalueStringRoundTrip()
        {
            UserCustomSchemaTests.TestMultivalueRoundTrip("TestMVString", new List<string>() { "testmvstring1", "testmvstring2" }, "testmvstring3");
        }

        [TestMethod]
        public void TestMultivalueDateRoundTrip()
        {
            UserCustomSchemaTests.TestMultivalueRoundTrip("TestMVDate", new List<string>() { "2018-01-08", "2018-01-09" }, "2017-01-01");
        }

        [TestMethod]
        public void TestMultivalueEmailRoundTrip()
        {
            UserCustomSchemaTests.TestMultivalueRoundTrip("TestMVEmail", new List<string>() { "test2@test.com", "test3@test.com" }, "test8@test.com");
        }

        [TestMethod]
        public void TestMultivalueDoubleRoundTrip()
        {
            UserCustomSchemaTests.TestMultivalueRoundTrip("TestMVDouble", new List<double>() { 2.1D, 3.1D }, new List<string>() { "2.1", "3.1" }, 1.8D);
        }

        [TestMethod]
        public void TestMultivaluePhoneRoundTrip()
        {
            UserCustomSchemaTests.TestMultivalueRoundTrip("TestMVPhone", new List<string>() { "98765431", "456789456" }, "99994444");
        }

        [TestMethod]
        public void TestMultivalueLongRoundTrip()
        {
            UserCustomSchemaTests.TestMultivalueRoundTrip("TestMVInt", new List<long>() { 2L, 3L }, new List<string>() { "2", "3" }, 4L);
        }

        [TestMethod]
        public void TestSingleValueStringRoundTrip()
        {
            UserCustomSchemaTests.TestSingleValueRoundTrip("TestSVString", "testsvstring", "replacementstring");
        }

        [TestMethod]
        public void TestSingleValueDateRoundTrip()
        {
            UserCustomSchemaTests.TestSingleValueRoundTrip("TestSVDate", "2018-01-01", "2018-01-02");
        }

        [TestMethod]
        public void TestSingleValueEmailRoundTrip()
        {
            UserCustomSchemaTests.TestSingleValueRoundTrip("TestSVEmail", "test1@test.com", "test9@test.com");
        }

        [TestMethod]
        public void TestSingleValueLongRoundTrip()
        {
            UserCustomSchemaTests.TestSingleValueRoundTrip("TestSVInt", 44L, 99L);
        }

        [TestMethod]
        public void TestSingleValuePhoneRoundTrip()
        {
            UserCustomSchemaTests.TestSingleValueRoundTrip("TestSVPhone", "123456789", "987654321");
        }

        [TestMethod]
        public void TestSingleValueBooleanRoundTrip()
        {
            UserCustomSchemaTests.TestSingleValueRoundTrip("TestSVBool", true, false);
        }

        [TestMethod]
        public void TestSingleValueDoubleRoundTrip()
        {
            UserCustomSchemaTests.TestSingleValueRoundTrip("TestSVDouble", 0.6822871999174D, "0.6822871999174", 0.9);
        }

        [TestMethod]
        public void AddValuesOnObjectAdd()
        {
            CSEntryChange cs = CSEntryChange.Create();
            cs.ObjectModificationType = ObjectModificationType.Add;
            cs.DN = $"{Guid.NewGuid()}@{UnitTestControl.TestParameters.Domain}";
            cs.ObjectType = SchemaConstants.User;

            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("orgUnitPath", "/"));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("name_givenName", "gn"));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("name_familyName", "sn"));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd($"{TestSchemaName}_TestSVString", "string1"));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd($"{TestSchemaName}_TestSVBool", true));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd($"{TestSchemaName}_TestSVDate", "2018-01-03"));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd($"{TestSchemaName}_TestSVDouble", "1.9999"));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd($"{TestSchemaName}_TestSVEmail", "test99@test.com"));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd($"{TestSchemaName}_TestSVInt", 899L));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd($"{TestSchemaName}_TestSVPhone", "555-1234"));

            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd($"{TestSchemaName}_TestMVString", new List<object> { "test2", "test3" }));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd($"{TestSchemaName}_TestMVDouble", new List<object> { "2.99999", "3.99999" }));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd($"{TestSchemaName}_TestMVDate", new List<object> { "2018-02-02", "2018-03-03" }));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd($"{TestSchemaName}_TestMVEmail", new List<object> { "test88@test.com", "test77@test.com" }));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd($"{TestSchemaName}_TestMVPhone", new List<object> { "555-6789", "555-9512" }));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd($"{TestSchemaName}_TestMVInt", new List<object> { 555L, 444L }));

            string id = null;

            try
            {
                CSEntryChangeResult result = ExportProcessor.PutCSEntryChange(cs, UnitTestControl.Schema.GetSchema().Types[SchemaConstants.User], UnitTestControl.TestParameters);
                id = result.AnchorAttributes["id"].GetValueAdd<string>();

                if (result.ErrorCode != MAExportError.Success)
                {
                    Assert.Fail(result.ErrorName);
                }

                Thread.Sleep(UnitTestControl.PostGoogleOperationSleepInterval);

                User e = UnitTestControl.TestParameters.UsersService.Get(id);
                Assert.AreEqual(cs.DN, e.PrimaryEmail);

                Assert.AreEqual("string1", (string)e.CustomSchemas[TestSchemaName]["TestSVString"]);
                Assert.AreEqual(true, (bool)e.CustomSchemas[TestSchemaName]["TestSVBool"]);
                Assert.AreEqual("2018-01-03", (string)e.CustomSchemas[TestSchemaName]["TestSVDate"]);
                Assert.AreEqual(1.9999D, (double)e.CustomSchemas[TestSchemaName]["TestSVDouble"]);
                Assert.AreEqual("test99@test.com", (string)e.CustomSchemas[TestSchemaName]["TestSVEmail"]);
                Assert.AreEqual("899", e.CustomSchemas[TestSchemaName]["TestSVInt"]);
                Assert.AreEqual("555-1234", (string)e.CustomSchemas[TestSchemaName]["TestSVPhone"]);

                CollectionAssert.AreEquivalent(new List<object> { "test2", "test3" }, GetReturnedValues<string>("TestMVString", e));
                CollectionAssert.AreEquivalent(new List<object> { "2018-02-02", "2018-03-03" }, GetReturnedValues<string>("TestMVDate", e));
                CollectionAssert.AreEquivalent(new List<object> { "test88@test.com", "test77@test.com" }, GetReturnedValues<string>("TestMVEmail", e));
                CollectionAssert.AreEquivalent(new List<object> { "555-6789", "555-9512" }, GetReturnedValues<string>("TestMVPhone", e));
                CollectionAssert.AreEquivalent(new List<object> { "555", "444" }, GetReturnedValues<string>("TestMVInt", e));
                CollectionAssert.AreEquivalent(new List<object> { "2.99999", "3.99999" }, GetReturnedValues<string>("TestMVDouble", e));
            }
            finally
            {
                if (id != null)
                {
                    UnitTestControl.TestParameters.UsersService.Delete(id);
                }
            }
        }

        [TestMethod]
        public void AddValuesOnObjectUpdate()
        {
            User u = UserTests.CreateUser();

            CSEntryChange cs = CSEntryChange.Create();
            cs.ObjectModificationType = ObjectModificationType.Update;
            cs.DN = u.PrimaryEmail;
            cs.ObjectType = SchemaConstants.User;
            cs.AnchorAttributes.Add(AnchorAttribute.Create("id", u.Id));

            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd($"{TestSchemaName}_TestSVString", "string1"));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd($"{TestSchemaName}_TestSVBool", true));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd($"{TestSchemaName}_TestSVDate", "2018-01-03"));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd($"{TestSchemaName}_TestSVDouble", "1.9999"));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd($"{TestSchemaName}_TestSVEmail", "test99@test.com"));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd($"{TestSchemaName}_TestSVInt", 899L));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd($"{TestSchemaName}_TestSVPhone", "555-1234"));

            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd($"{TestSchemaName}_TestMVString", new List<object> { "test2", "test3" }));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd($"{TestSchemaName}_TestMVDouble", new List<object> { "2.99999", "3.99999" }));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd($"{TestSchemaName}_TestMVDate", new List<object> { "2018-02-02", "2018-03-03" }));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd($"{TestSchemaName}_TestMVEmail", new List<object> { "test88@test.com", "test77@test.com" }));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd($"{TestSchemaName}_TestMVPhone", new List<object> { "555-6789", "555-9512" }));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd($"{TestSchemaName}_TestMVInt", new List<object> { 555L, 444L }));

            try
            {
                CSEntryChangeResult result = ExportProcessor.PutCSEntryChange(cs, UnitTestControl.Schema.GetSchema().Types[SchemaConstants.User], UnitTestControl.TestParameters);

                if (result.ErrorCode != MAExportError.Success)
                {
                    Assert.Fail(result.ErrorName);
                }

                Thread.Sleep(UnitTestControl.PostGoogleOperationSleepInterval);

                User e = UnitTestControl.TestParameters.UsersService.Get(u.Id);
                Assert.AreEqual(cs.DN, e.PrimaryEmail);

                Assert.AreEqual("string1", (string)e.CustomSchemas[TestSchemaName]["TestSVString"]);
                Assert.AreEqual(true, (bool)e.CustomSchemas[TestSchemaName]["TestSVBool"]);
                Assert.AreEqual("2018-01-03", (string)e.CustomSchemas[TestSchemaName]["TestSVDate"]);
                Assert.AreEqual(1.9999D, (double)e.CustomSchemas[TestSchemaName]["TestSVDouble"]);
                Assert.AreEqual("test99@test.com", (string)e.CustomSchemas[TestSchemaName]["TestSVEmail"]);
                Assert.AreEqual("899", e.CustomSchemas[TestSchemaName]["TestSVInt"]);
                Assert.AreEqual("555-1234", (string)e.CustomSchemas[TestSchemaName]["TestSVPhone"]);

                CollectionAssert.AreEquivalent(new List<object> { "test2", "test3" }, GetReturnedValues<string>("TestMVString", e));
                CollectionAssert.AreEquivalent(new List<object> { "2018-02-02", "2018-03-03" }, GetReturnedValues<string>("TestMVDate", e));
                CollectionAssert.AreEquivalent(new List<object> { "test88@test.com", "test77@test.com" }, GetReturnedValues<string>("TestMVEmail", e));
                CollectionAssert.AreEquivalent(new List<object> { "555-6789", "555-9512" }, GetReturnedValues<string>("TestMVPhone", e));
                CollectionAssert.AreEquivalent(new List<object> { "555", "444" }, GetReturnedValues<string>("TestMVInt", e));
                CollectionAssert.AreEquivalent(new List<object> { "2.99999", "3.99999" }, GetReturnedValues<string>("TestMVDouble", e));
            }
            finally
            {
                if (u?.Id != null)
                {
                    UnitTestControl.TestParameters.UsersService.Delete(u.Id);
                }
            }
        }

        [TestMethod]
        public void ReplaceStringValuesOnObjectUpdate()
        {
            User u = UserTests.CreateUser();

            // Create the initial object
            CSEntryChange cs = CSEntryChange.Create();
            cs.ObjectModificationType = ObjectModificationType.Update;
            cs.DN = u.PrimaryEmail;
            cs.ObjectType = SchemaConstants.User;
            cs.AnchorAttributes.Add(AnchorAttribute.Create("id", u.Id));

            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd($"{TestSchemaName}_TestSVString", "string1"));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd($"{TestSchemaName}_TestMVString", new List<object> { "test2", "test3" }));

            try
            {
                CSEntryChangeResult result = ExportProcessor.PutCSEntryChange(cs, UnitTestControl.Schema.GetSchema().Types[SchemaConstants.User], UnitTestControl.TestParameters);

                if (result.ErrorCode != MAExportError.Success)
                {
                    Assert.Fail(result.ErrorName);
                }

                Thread.Sleep(UnitTestControl.PostGoogleOperationSleepInterval);

                User e = UnitTestControl.TestParameters.UsersService.Get(u.Id);
                Assert.AreEqual(cs.DN, e.PrimaryEmail);

                Assert.AreEqual("string1", (string)e.CustomSchemas[TestSchemaName]["TestSVString"]);
                CollectionAssert.AreEquivalent(new List<object> { "test2", "test3" }, GetReturnedValues<string>("TestMVString", e));

                // Perform the update

                cs = CSEntryChange.Create();
                cs.ObjectModificationType = ObjectModificationType.Update;
                cs.DN = u.PrimaryEmail;
                cs.ObjectType = SchemaConstants.User;
                cs.AnchorAttributes.Add(AnchorAttribute.Create("id", u.Id));

                cs.AttributeChanges.Add(AttributeChange.CreateAttributeReplace($"{TestSchemaName}_TestSVString", "string9"));
                cs.AttributeChanges.Add(AttributeChange.CreateAttributeReplace($"{TestSchemaName}_TestMVString", new List<object> { "test4", "test5" }));

                result = ExportProcessor.PutCSEntryChange(cs, UnitTestControl.Schema.GetSchema().Types[SchemaConstants.User], UnitTestControl.TestParameters);

                if (result.ErrorCode != MAExportError.Success)
                {
                    Assert.Fail(result.ErrorName);
                }

                Thread.Sleep(UnitTestControl.PostGoogleOperationSleepInterval);

                e = UnitTestControl.TestParameters.UsersService.Get(u.Id);
                Assert.AreEqual(cs.DN, e.PrimaryEmail);

                Assert.AreEqual("string9", (string)e.CustomSchemas[TestSchemaName]["TestSVString"]);
                CollectionAssert.AreEquivalent(new List<object> { "test4", "test5" }, GetReturnedValues<string>("TestMVString", e));
            }
            finally
            {
                if (u?.Id != null)
                {
                    UnitTestControl.TestParameters.UsersService.Delete(u.Id);
                }
            }
        }

        [TestMethod]
        public void UpdateStringValuesOnObjectUpdate()
        {
            User u = UserTests.CreateUser();

            // Create the initial object
            CSEntryChange cs = CSEntryChange.Create();
            cs.ObjectModificationType = ObjectModificationType.Update;
            cs.DN = u.PrimaryEmail;
            cs.ObjectType = SchemaConstants.User;
            cs.AnchorAttributes.Add(AnchorAttribute.Create("id", u.Id));

            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd($"{TestSchemaName}_TestSVString", "string1"));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd($"{TestSchemaName}_TestMVString", new List<object> { "test2", "test3" }));

            try
            {
                CSEntryChangeResult result = ExportProcessor.PutCSEntryChange(cs, UnitTestControl.Schema.GetSchema().Types[SchemaConstants.User], UnitTestControl.TestParameters);

                if (result.ErrorCode != MAExportError.Success)
                {
                    Assert.Fail(result.ErrorName);
                }

                Thread.Sleep(UnitTestControl.PostGoogleOperationSleepInterval);

                User e = UnitTestControl.TestParameters.UsersService.Get(u.Id);
                Assert.AreEqual(cs.DN, e.PrimaryEmail);

                Assert.AreEqual("string1", (string)e.CustomSchemas[TestSchemaName]["TestSVString"]);
                CollectionAssert.AreEquivalent(new List<object> { "test2", "test3" }, GetReturnedValues<string>("TestMVString", e));

                // Perform the update

                cs = CSEntryChange.Create();
                cs.ObjectModificationType = ObjectModificationType.Update;
                cs.DN = u.PrimaryEmail;
                cs.ObjectType = SchemaConstants.User;
                cs.AnchorAttributes.Add(AnchorAttribute.Create("id", u.Id));

                cs.AttributeChanges.Add(AttributeChange.CreateAttributeUpdate($"{TestSchemaName}_TestSVString", "string9"));
                cs.AttributeChanges.Add(AttributeChange.CreateAttributeUpdate($"{TestSchemaName}_TestMVString", new List<ValueChange> { ValueChange.CreateValueAdd("test4"), ValueChange.CreateValueAdd("test5") }));

                result = ExportProcessor.PutCSEntryChange(cs, UnitTestControl.Schema.GetSchema().Types[SchemaConstants.User], UnitTestControl.TestParameters);

                if (result.ErrorCode != MAExportError.Success)
                {
                    Assert.Fail(result.ErrorName);
                }

                Thread.Sleep(UnitTestControl.PostGoogleOperationSleepInterval);

                e = UnitTestControl.TestParameters.UsersService.Get(u.Id);
                Assert.AreEqual(cs.DN, e.PrimaryEmail);

                Assert.AreEqual("string9", (string)e.CustomSchemas[TestSchemaName]["TestSVString"]);
                CollectionAssert.AreEquivalent(new List<object> { "test2", "test3", "test4", "test5" }, GetReturnedValues<string>("TestMVString", e));
            }
            finally
            {
                if (u?.Id != null)
                {
                    UnitTestControl.TestParameters.UsersService.Delete(u.Id);
                }
            }
        }

        [TestMethod]
        public void DeleteStringValueOnObjectUpdate()
        {
            User u = UserTests.CreateUser();

            // Create the initial object
            CSEntryChange cs = CSEntryChange.Create();
            cs.ObjectModificationType = ObjectModificationType.Update;
            cs.DN = u.PrimaryEmail;
            cs.ObjectType = SchemaConstants.User;
            cs.AnchorAttributes.Add(AnchorAttribute.Create("id", u.Id));

            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd($"{TestSchemaName}_TestSVString", "string1"));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd($"{TestSchemaName}_TestMVString", new List<object> { "test2", "test3" }));

            try
            {
                CSEntryChangeResult result = ExportProcessor.PutCSEntryChange(cs, UnitTestControl.Schema.GetSchema().Types[SchemaConstants.User], UnitTestControl.TestParameters);

                if (result.ErrorCode != MAExportError.Success)
                {
                    Assert.Fail(result.ErrorName);
                }

                Thread.Sleep(UnitTestControl.PostGoogleOperationSleepInterval);

                User e = UnitTestControl.TestParameters.UsersService.Get(u.Id);
                Assert.AreEqual(cs.DN, e.PrimaryEmail);

                Assert.AreEqual("string1", (string)e.CustomSchemas[TestSchemaName]["TestSVString"]);
                CollectionAssert.AreEquivalent(new List<object> { "test2", "test3" }, GetReturnedValues<string>("TestMVString", e));

                // Remove a single value

                cs = CSEntryChange.Create();
                cs.ObjectModificationType = ObjectModificationType.Update;
                cs.DN = u.PrimaryEmail;
                cs.ObjectType = SchemaConstants.User;
                cs.AnchorAttributes.Add(AnchorAttribute.Create("id", u.Id));

                cs.AttributeChanges.Add(AttributeChange.CreateAttributeUpdate($"{TestSchemaName}_TestSVString", new List<ValueChange> { ValueChange.CreateValueDelete("string1") }));
                cs.AttributeChanges.Add(AttributeChange.CreateAttributeUpdate($"{TestSchemaName}_TestMVString", new List<ValueChange> { ValueChange.CreateValueDelete("test3") }));

                result = ExportProcessor.PutCSEntryChange(cs, UnitTestControl.Schema.GetSchema().Types[SchemaConstants.User], UnitTestControl.TestParameters);

                if (result.ErrorCode != MAExportError.Success)
                {
                    Assert.Fail(result.ErrorName);
                }

                Thread.Sleep(UnitTestControl.PostGoogleOperationSleepInterval);

                e = UnitTestControl.TestParameters.UsersService.Get(u.Id);
                Assert.AreEqual(cs.DN, e.PrimaryEmail);

                Assert.IsFalse(e.CustomSchemas[TestSchemaName].ContainsKey("TestSVString"));
                CollectionAssert.AreEquivalent(new List<object> { "test2" }, GetReturnedValues<string>("TestMVString", e));
            }
            finally
            {
                if (u?.Id != null)
                {
                    UnitTestControl.TestParameters.UsersService.Delete(u.Id);
                }
            }
        }

        [TestMethod]
        public void DeleteStringValuesAsValueDeleteOnObjectUpdate()
        {
            User u = UserTests.CreateUser();

            // Create the initial object
            CSEntryChange cs = CSEntryChange.Create();
            cs.ObjectModificationType = ObjectModificationType.Update;
            cs.DN = u.PrimaryEmail;
            cs.ObjectType = SchemaConstants.User;
            cs.AnchorAttributes.Add(AnchorAttribute.Create("id", u.Id));

            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd($"{TestSchemaName}_TestSVString", "string1"));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd($"{TestSchemaName}_TestMVString", new List<object> { "test2", "test3" }));

            try
            {
                CSEntryChangeResult result = ExportProcessor.PutCSEntryChange(cs, UnitTestControl.Schema.GetSchema().Types[SchemaConstants.User], UnitTestControl.TestParameters);

                if (result.ErrorCode != MAExportError.Success)
                {
                    Assert.Fail(result.ErrorName);
                }

                Thread.Sleep(UnitTestControl.PostGoogleOperationSleepInterval);

                User e = UnitTestControl.TestParameters.UsersService.Get(u.Id);
                Assert.AreEqual(cs.DN, e.PrimaryEmail);

                Assert.AreEqual("string1", (string)e.CustomSchemas[TestSchemaName]["TestSVString"]);
                CollectionAssert.AreEquivalent(new List<object> { "test2", "test3" }, GetReturnedValues<string>("TestMVString", e));

                // Remove all the values with a 'value delete' 

                cs = CSEntryChange.Create();
                cs.ObjectModificationType = ObjectModificationType.Update;
                cs.DN = u.PrimaryEmail;
                cs.ObjectType = SchemaConstants.User;
                cs.AnchorAttributes.Add(AnchorAttribute.Create("id", u.Id));

                cs.AttributeChanges.Add(AttributeChange.CreateAttributeUpdate($"{TestSchemaName}_TestSVString", new List<ValueChange> { ValueChange.CreateValueDelete("string1") }));
                cs.AttributeChanges.Add(AttributeChange.CreateAttributeUpdate($"{TestSchemaName}_TestMVString", new List<ValueChange> { ValueChange.CreateValueDelete("test2"), ValueChange.CreateValueDelete("test3") }));

                result = ExportProcessor.PutCSEntryChange(cs, UnitTestControl.Schema.GetSchema().Types[SchemaConstants.User], UnitTestControl.TestParameters);

                if (result.ErrorCode != MAExportError.Success)
                {
                    Assert.Fail(result.ErrorName);
                }

                Thread.Sleep(UnitTestControl.PostGoogleOperationSleepInterval);

                e = UnitTestControl.TestParameters.UsersService.Get(u.Id);
                Assert.AreEqual(cs.DN, e.PrimaryEmail);

                if (e.CustomSchemas != null && e.CustomSchemas.ContainsKey(TestSchemaName))
                {
                    Assert.IsFalse(e.CustomSchemas[TestSchemaName].ContainsKey("TestSVString"));
                    Assert.IsFalse(e.CustomSchemas[TestSchemaName].ContainsKey("TestMVString"));
                }
            }
            finally
            {
                if (u?.Id != null)
                {
                    UnitTestControl.TestParameters.UsersService.Delete(u.Id);
                }
            }
        }

        [TestMethod]
        public void DeleteStringValuesAsAttributeDeleteOnObjectUpdate()
        {
            User u = UserTests.CreateUser();

            // Create the initial object
            CSEntryChange cs = CSEntryChange.Create();
            cs.ObjectModificationType = ObjectModificationType.Update;
            cs.DN = u.PrimaryEmail;
            cs.ObjectType = SchemaConstants.User;
            cs.AnchorAttributes.Add(AnchorAttribute.Create("id", u.Id));

            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd($"{TestSchemaName}_TestSVString", "string1"));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd($"{TestSchemaName}_TestMVString", new List<object> { "test2", "test3" }));

            try
            {
                CSEntryChangeResult result = ExportProcessor.PutCSEntryChange(cs, UnitTestControl.Schema.GetSchema().Types[SchemaConstants.User], UnitTestControl.TestParameters);

                if (result.ErrorCode != MAExportError.Success)
                {
                    Assert.Fail(result.ErrorName);
                }

                Thread.Sleep(UnitTestControl.PostGoogleOperationSleepInterval);

                User e = UnitTestControl.TestParameters.UsersService.Get(u.Id);
                Assert.AreEqual(cs.DN, e.PrimaryEmail);

                Assert.AreEqual("string1", (string)e.CustomSchemas[TestSchemaName]["TestSVString"]);
                CollectionAssert.AreEquivalent(new List<object> { "test2", "test3" }, GetReturnedValues<string>("TestMVString", e));

                // Remove all the values with an 'attribute delete' 

                cs = CSEntryChange.Create();
                cs.ObjectModificationType = ObjectModificationType.Update;
                cs.DN = u.PrimaryEmail;
                cs.ObjectType = SchemaConstants.User;
                cs.AnchorAttributes.Add(AnchorAttribute.Create("id", u.Id));

                cs.AttributeChanges.Add(AttributeChange.CreateAttributeDelete($"{TestSchemaName}_TestSVString"));
                cs.AttributeChanges.Add(AttributeChange.CreateAttributeDelete($"{TestSchemaName}_TestMVString"));

                result = ExportProcessor.PutCSEntryChange(cs, UnitTestControl.Schema.GetSchema().Types[SchemaConstants.User], UnitTestControl.TestParameters);

                if (result.ErrorCode != MAExportError.Success)
                {
                    Assert.Fail(result.ErrorName);
                }

                Thread.Sleep(UnitTestControl.PostGoogleOperationSleepInterval);

                e = UnitTestControl.TestParameters.UsersService.Get(u.Id);
                Assert.AreEqual(cs.DN, e.PrimaryEmail);

                if (e.CustomSchemas != null && e.CustomSchemas.ContainsKey(TestSchemaName))
                {
                    Assert.IsFalse(e.CustomSchemas[TestSchemaName].ContainsKey("TestSVString"));
                    Assert.IsFalse(e.CustomSchemas[TestSchemaName].ContainsKey("TestMVString"));
                }
            }
            finally
            {
                if (u?.Id != null)
                {
                    UnitTestControl.TestParameters.UsersService.Delete(u.Id);
                }
            }
        }

        private static void TestSingleValueRoundTrip<T1>(string fieldName, T1 expectedValue, T1 valueForUpdateTest)
        {
            UserCustomSchemaTests.TestSingleValueRoundTrip(fieldName, expectedValue, expectedValue, valueForUpdateTest);
        }

        private static void TestMultivalueRoundTrip<T1>(string fieldName, IList<T1> expectedValue, T1 valueToAdd)
        {
            UserCustomSchemaTests.TestMultivalueRoundTrip(fieldName, expectedValue, expectedValue, valueToAdd);
        }

        private static AdapterCustomSchemaField GetFieldAdapter(string attributeName)
        {
            return UnitTestControl.Schema["user"].AttributeAdapters.OfType<AdapterCustomSchemas>().
                First().
                CustomSchemas.First(t => t.SchemaName == TestSchemaName).
                Fields.First(u => u.MmsAttributeName == attributeName);
        }

        private static void TestSingleValueRoundTrip<T1, T2>(string fieldName, T1 expectedGoogleValue, T2 expectedFimValue, T1 valueForUpdateTest)
        {
            string testAttributeName = $"{UserCustomSchemaTests.TestSchemaName}_{fieldName}";
            AdapterCustomSchemaField schemaItem = GetFieldAdapter(testAttributeName);

            User u = DeserializeTestUser();

            CSEntryChange x = CSEntryChange.Create();
            x.ObjectModificationType = ObjectModificationType.Add;

            // Convert the user object into a series of attribute changes

            IList<AttributeChange> result = schemaItem.CreateAttributeChanges(x.DN, ObjectModificationType.Add, u).ToList();

            AttributeChange change = result.FirstOrDefault(t => t.Name == testAttributeName);
            Assert.IsNotNull(change);
            Assert.AreEqual(expectedFimValue, change.GetValueAdd<T2>());

            // Reverse the process and convert the attribute changes onto a new user

            x.AttributeChanges.Add(change);

            User ux = new User();
            schemaItem.UpdateField(x, ux);
            Assert.AreEqual(expectedGoogleValue, (T1)ux.CustomSchemas[TestSchemaName][fieldName]);

            // Modify the value

            change = AttributeChange.CreateAttributeUpdate(testAttributeName, valueForUpdateTest);
            x = CSEntryChange.Create();
            x.ObjectModificationType = ObjectModificationType.Update;
            x.AttributeChanges.Add(change);

            schemaItem.UpdateField(x, ux);
            Assert.AreEqual(valueForUpdateTest, (T1)ux.CustomSchemas[TestSchemaName][fieldName]);

            // Delete the value

            change = AttributeChange.CreateAttributeDelete(testAttributeName);
            x = CSEntryChange.Create();
            x.ObjectModificationType = ObjectModificationType.Update;
            x.AttributeChanges.Add(change);

            schemaItem.UpdateField(x, ux);
            Assert.AreEqual(Constants.NullValuePlaceholder, (string)ux.CustomSchemas[TestSchemaName][fieldName]);
        }

        private static void TestMultivalueRoundTrip<T1, T2>(string fieldName, IList<T1> expectedGoogleValue, IList<T2> expectedFimValues, T1 valueToAdd)
        {
            string testAttributeName = $"{UserCustomSchemaTests.TestSchemaName}_{fieldName}";
            AdapterCustomSchemaField schemaItem = GetFieldAdapter(testAttributeName);

            User u = DeserializeTestUser();

            CSEntryChange x = CSEntryChange.Create();
            x.ObjectModificationType = ObjectModificationType.Add;

            // Convert the user object into a series of attribute changes

            IList<AttributeChange> result = schemaItem.CreateAttributeChanges(x.DN, ObjectModificationType.Add, u).ToList();

            AttributeChange change = result.FirstOrDefault(t => t.Name == testAttributeName);
            Assert.IsNotNull(change);
            CollectionAssert.AreEquivalent(expectedFimValues.ToArray(), change.GetValueAdds<T2>().ToArray());

            // Reverse the process and convert the attribute changes onto a new user
            x.AttributeChanges.Add(change);

            User ux = new User();
            schemaItem.UpdateField(x, ux);

            List<T1> returnedValues = UserCustomSchemaTests.GetReturnedValues<T1>(fieldName, ux);

            CollectionAssert.AreEquivalent(expectedGoogleValue.ToArray(), returnedValues);

            // Add a value
            ValueChange add1 = ValueChange.CreateValueAdd(valueToAdd);
            change = AttributeChange.CreateAttributeUpdate(testAttributeName, new List<ValueChange>() { add1 });
            x = CSEntryChange.Create();
            x.ObjectModificationType = ObjectModificationType.Update;
            x.AttributeChanges.Add(change);
            schemaItem.UpdateField(x, ux);

            returnedValues = UserCustomSchemaTests.GetReturnedValues<T1>(fieldName, ux);

            IList<object> e = expectedGoogleValue.Cast<object>().ToList();
            e.Add(valueToAdd);

            CollectionAssert.AreEquivalent(e.ToArray(), returnedValues);

            // Remove a value

            ValueChange delete1 = ValueChange.CreateValueDelete(valueToAdd);
            change = AttributeChange.CreateAttributeUpdate(testAttributeName, new List<ValueChange>() { delete1 });
            x = CSEntryChange.Create();
            x.ObjectModificationType = ObjectModificationType.Update;
            x.AttributeChanges.Add(change);
            schemaItem.UpdateField(x, ux);

            returnedValues = UserCustomSchemaTests.GetReturnedValues<T1>(fieldName, ux);

            CollectionAssert.AreEquivalent(expectedGoogleValue.ToArray(), returnedValues);

            // Delete the attribute

            change = AttributeChange.CreateAttributeDelete(testAttributeName);
            x = CSEntryChange.Create();
            x.ObjectModificationType = ObjectModificationType.Update;
            x.AttributeChanges.Add(change);

            schemaItem.UpdateField(x, ux);
            Assert.AreEqual(Constants.NullValuePlaceholder, (string)ux.CustomSchemas[TestSchemaName][fieldName]);
        }

        private static User DeserializeTestUser()
        {
            string json = @"
{
    ""customSchemas"": {
            ""TestSchema"": {
                ""TestSVPhone"": ""123456789"",
                ""TestSVInt"": ""44"",
                ""TestMVInt"": [{
                    ""type"": ""work"",
                    ""value"": ""2""

                }, {
                    ""type"": ""work"",
                    ""value"": ""3""
                }],
                ""TestMVString"": [{
                    ""type"": ""work"",
                    ""value"": ""testmvstring1""
                }, {
                    ""type"": ""work"",
                    ""value"": ""testmvstring2""
                }],
                ""TestMVDouble"": [{
                    ""type"": ""work"",
                    ""value"": 2.1
                }, {
                    ""type"": ""work"",
                    ""value"": 3.1
                }],
                ""TestSVString"": ""testsvstring"",
                ""TestMVEmail"": [{
                    ""type"": ""work"",
                    ""value"": ""test2@test.com""
                }, {
                    ""type"": ""work"",
                    ""value"": ""test3@test.com""
                }],
                ""TestSVDate"": ""2018-01-01"",
                ""TestSVDouble"": 0.6822871999174,
                ""TestSVBool"": true,
                ""TestMVDate"": [{
                    ""type"": ""work"",
                    ""value"": ""2018-01-08""
                }, {
                    ""type"": ""work"",
                    ""value"": ""2018-01-09""
                }],
                ""TestMVBool"": [{
                    ""type"": ""work"",
                    ""value"": false
                }, {
                    ""type"": ""work"",
                    ""value"": true
                }],
                ""TestMVPhone"": [{
                    ""type"": ""work"",
                    ""value"": ""98765431""
                }, {
                    ""type"": ""work"",
                    ""value"": ""456789456""
                }],
                ""TestSVEmail"": ""test1@test.com""
            },
            ""accountDeprovisioning"": {
                ""permissionsRemoved"": true
            }
        }
    }";

            GoogleJsonSerializer s = new GoogleJsonSerializer();
            return (User)s.Deserialize(json, typeof(User));
        }

        private static List<T1> GetReturnedValues<T1>(string fieldName, User u)
        {
            object value = u.CustomSchemas[TestSchemaName][fieldName];

            if (value is JArray jarray)
            {
                return UserCustomSchemaTests.GetValuesFromJArray<T1>(jarray);
            }

            if (value is IList list)
            {
                return UserCustomSchemaTests.GetValuesFromList<T1>(list);
            }

            throw new NotSupportedException("The array type was unknown");
        }

        private static List<T1> GetValuesFromList<T1>(IList list)
        {
            List<T1> newList = new List<T1>();

            if (list is null)
            {
                return newList;
            }

            foreach (object item in list)
            {
                if (item is IDictionary<string, object> d)
                {
                    if (d.ContainsKey("value"))
                    {
                        newList.Add((T1)d["value"]);
                    }
                }
            }

            return newList;
        }

        private static List<T1> GetValuesFromJArray<T1>(JArray jarray)
        {
            List<T1> newList = new List<T1>();

            if (jarray is null)
            {
                return newList;
            }

            foreach (JToken i in jarray.Children())
            {
                JEnumerable<JProperty> itemProperties = i.Children<JProperty>();

                foreach (JProperty myElement in itemProperties.Where(x => x.Name == "value"))
                {
                    newList.Add(TypeConverter.ConvertData<T1>((string)myElement.Value));
                }
            }

            return newList;
        }
    }
}