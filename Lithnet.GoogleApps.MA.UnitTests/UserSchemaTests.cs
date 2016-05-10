using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lithnet.GoogleApps.MA.UnitTests
{
    using Microsoft.MetadirectoryServices;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using ManagedObjects;
    using MetadirectoryServices;

    [TestClass]
    public class UserSchemaTests
    {
        internal static MASchemaType Type;

        static UserSchemaTests()
        {
            UserSchemaTests.Type = new MASchemaType
            {
                Attributes = new List<IMASchemaAttribute>(),
                Name = "user"
            };

            MASchemaArrayField givenName = new MASchemaArrayField
            {
                AttributeType = AttributeType.String,
                FieldName = "givenName",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                PropertyName = "GivenName",
                AttributeNamePart = "givenName"
            };

            MASchemaArrayField familyName = new MASchemaArrayField
            {
                AttributeType = AttributeType.String,
                FieldName = "familyName",
                IsMultivalued = false,
                PropertyName = "FamilyName",
                Operation = AttributeOperation.ImportExport,
                AttributeNamePart = "familyName"
            };

            MASchemaNestedType schemaItem = new MASchemaNestedType
            {
                Api = "user",
                AttributeName = "name",
                Fields = new List<MASchemaArrayField>() { givenName, familyName },
                FieldName = "name",
                PropertyName = "Name",
                CanPatch = false
            };

             UserSchemaTests.Type.Attributes.Add(schemaItem);


            MASchemaArrayField notesValue = new MASchemaArrayField
            {
                AttributeType = AttributeType.String,
                FieldName = "value",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                PropertyName = "Value",
                AttributeNamePart = "value"
            };

            MASchemaArrayField notesContentType = new MASchemaArrayField
            {
                AttributeType = AttributeType.String,
                FieldName = "contentType",
                IsMultivalued = false,
                PropertyName = "ContentType",
                Operation = AttributeOperation.ImportExport,
                AttributeNamePart = "contentType"
            };

            MASchemaNestedType notesType = new MASchemaNestedType
            {
                Api = "user",
                AttributeName = "notes",
                Fields = new List<MASchemaArrayField>() { notesContentType, notesValue },
                FieldName = "notes",
                PropertyName = "Notes",
                CanPatch = false
            };

             UserSchemaTests.Type.Attributes.Add(notesType);

            MASchemaAttribute orgUnitPath = new MASchemaAttribute
            {
                AttributeType = AttributeType.String,
                FieldName = "orgUnitPath",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                AttributeName = "orgUnitPath",
                PropertyName = "OrgUnitPath",
                Api = "user",
                CanPatch = true,
                IsArrayAttribute = false
            };

             UserSchemaTests.Type.Attributes.Add(orgUnitPath);

            MASchemaAttribute includeInGal = new MASchemaAttribute
            {
                AttributeType = AttributeType.Boolean,
                FieldName = "includeInGlobalAddressList",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                AttributeName = "includeInGlobalAddressList",
                PropertyName = "IncludeInGlobalAddressList",
                Api = "user",
                CanPatch = true,
                IsArrayAttribute = false
            };

             UserSchemaTests.Type.Attributes.Add(includeInGal);

            MASchemaAttribute suspended = new MASchemaAttribute
            {
                AttributeType = AttributeType.Boolean,
                FieldName = "suspended",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                AttributeName = "suspended",
                PropertyName = "Suspended",
                Api = "user",
                CanPatch = true,
                IsArrayAttribute = false
            };

             UserSchemaTests.Type.Attributes.Add(suspended);

            MASchemaAttribute changePasswordAtNextLogin = new MASchemaAttribute
            {
                AttributeType = AttributeType.Boolean,
                FieldName = "changePasswordAtNextLogin",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                AttributeName = "changePasswordAtNextLogin",
                PropertyName = "ChangePasswordAtNextLogin",
                Api = "user",
                CanPatch = true,
                IsArrayAttribute = false
            };

             UserSchemaTests.Type.Attributes.Add(changePasswordAtNextLogin);

            MASchemaAttribute ipWhitelisted = new MASchemaAttribute
            {
                AttributeType = AttributeType.Boolean,
                FieldName = "ipWhitelisted",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                AttributeName = "ipWhitelisted",
                PropertyName = "IpWhitelisted",
                Api = "user",
                CanPatch = true,
                IsArrayAttribute = false
            };

             UserSchemaTests.Type.Attributes.Add(ipWhitelisted);

            MASchemaAttribute id = new MASchemaAttribute
            {
                AttributeType = AttributeType.String,
                FieldName = "id",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                AttributeName = "id",
                PropertyName = "Id",
                Api = "user",
                CanPatch = true,
                IsArrayAttribute = false
            };

             UserSchemaTests.Type.Attributes.Add(id);

            MASchemaAttribute primaryEmail = new MASchemaAttribute
            {
                AttributeType = AttributeType.String,
                FieldName = "primaryEmail",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                AttributeName = "primaryEmail",
                PropertyName = "PrimaryEmail",
                Api = "user",
                CanPatch = true,
                IsArrayAttribute = false
            };

             UserSchemaTests.Type.Attributes.Add(primaryEmail);

            MASchemaAttribute isAdmin = new MASchemaAttribute
            {
                AttributeType = AttributeType.Boolean,
                FieldName = "isAdmin",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                AttributeName = "isAdmin",
                PropertyName = "IsAdmin",
                Api = "user",
                CanPatch = true,
                IsArrayAttribute = false
            };

             UserSchemaTests.Type.Attributes.Add(isAdmin);

            MASchemaAttribute isDelegatedAdmin = new MASchemaAttribute
            {
                AttributeType = AttributeType.Boolean,
                FieldName = "isDelegatedAdmin",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                AttributeName = "isDelegatedAdmin",
                PropertyName = "IsDelegatedAdmin",
                Api = "user",
                CanPatch = true,
                IsArrayAttribute = false
            };

             UserSchemaTests.Type.Attributes.Add(isDelegatedAdmin);

            MASchemaAttribute lastLoginTime = new MASchemaAttribute
            {
                AttributeType = AttributeType.String,
                FieldName = "lastLoginTime",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                AttributeName = "lastLoginTime",
                PropertyName = "LastLoginTime",
                Api = "user",
                CanPatch = true,
                IsArrayAttribute = false
            };

             UserSchemaTests.Type.Attributes.Add(lastLoginTime);

            MASchemaAttribute creationTime = new MASchemaAttribute
            {
                AttributeType = AttributeType.String,
                FieldName = "creationTime",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                AttributeName = "creationTime",
                PropertyName = "CreationTime",
                Api = "user",
                CanPatch = true,
                IsArrayAttribute = false
            };

             UserSchemaTests.Type.Attributes.Add(creationTime);

            MASchemaAttribute deletionTime = new MASchemaAttribute
            {
                AttributeType = AttributeType.String,
                FieldName = "deletionTime",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                AttributeName = "deletionTime",
                PropertyName = "DeletionTime",
                Api = "user",
                CanPatch = true,
                IsArrayAttribute = false
            };

             UserSchemaTests.Type.Attributes.Add(deletionTime);

            MASchemaAttribute agreedToTerms = new MASchemaAttribute
            {
                AttributeType = AttributeType.Boolean,
                FieldName = "agreedToTerms",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                AttributeName = "agreedToTerms",
                PropertyName = "AgreedToTerms",
                Api = "user",
                CanPatch = true,
                IsArrayAttribute = false
            };

             UserSchemaTests.Type.Attributes.Add(agreedToTerms);

            MASchemaAttribute suspensionReason = new MASchemaAttribute
            {
                AttributeType = AttributeType.String,
                FieldName = "suspensionReason",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                AttributeName = "suspensionReason",
                PropertyName = "SuspensionReason",
                Api = "user",
                CanPatch = true,
                IsArrayAttribute = false
            };

             UserSchemaTests.Type.Attributes.Add(suspensionReason);

            MASchemaAttribute isMailboxSetup = new MASchemaAttribute
            {
                AttributeType = AttributeType.String,
                FieldName = "isMailboxSetup",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                AttributeName = "isMailboxSetup",
                PropertyName = "IsMailboxSetup",
                Api = "user",
                CanPatch = true,
                IsArrayAttribute = false
            };

             UserSchemaTests.Type.Attributes.Add(isMailboxSetup);

            MASchemaAttribute thumbnailPhotoUrl = new MASchemaAttribute
            {
                AttributeType = AttributeType.String,
                FieldName = "thumbnailPhotoUrl",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                AttributeName = "thumbnailPhotoUrl",
                PropertyName = "ThumbnailPhotoUrl",
                Api = "user",
                CanPatch = true,
                IsArrayAttribute = false
            };

             UserSchemaTests.Type.Attributes.Add(thumbnailPhotoUrl);

            MASchemaAttribute thumbnailPhotoEtag = new MASchemaAttribute
            {
                AttributeType = AttributeType.String,
                FieldName = "thumbnailPhotoEtag",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                AttributeName = "thumbnailPhotoEtag",
                PropertyName = "ThumbnailPhotoEtag",
                Api = "user",
                CanPatch = true,
                IsArrayAttribute = false
            };

             UserSchemaTests.Type.Attributes.Add(thumbnailPhotoEtag);

            MASchemaAttribute customerId = new MASchemaAttribute
            {
                AttributeType = AttributeType.String,
                FieldName = "customerId",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                AttributeName = "customerId",
                PropertyName = "CustomerId",
                Api = "user",
                CanPatch = true,
                IsArrayAttribute = false
            };

             UserSchemaTests.Type.Attributes.Add(customerId);

            MASchemaArrayField webSiteValue = new MASchemaArrayField
            {
                AttributeType = AttributeType.String,
                FieldName = "value",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                PropertyName = "Value",
                AttributeNamePart = "value"
            };

            MASchemaArrayField webSitePrimary = new MASchemaArrayField
            {
                AttributeType = AttributeType.Boolean,
                FieldName = "primary",
                IsMultivalued = false,
                PropertyName = "Primary",
                Operation = AttributeOperation.ImportExport,
                AttributeNamePart = "primary"
            };

            MASchemaCustomTypeArray webSiteType = new MASchemaCustomTypeArray
            {
                Api = "user",
                AttributeName = "websites",
                Fields = new List<MASchemaArrayField>() { webSitePrimary, webSiteValue },
                FieldName = "websites",
                PropertyName = "Websites",
                Type = typeof(Website),
                KnownTypes = new List<string>() { "work", "home", "other" },
                CanPatch = false
            };

             UserSchemaTests.Type.Attributes.Add(webSiteType);

            MASchemaSimpleList aliasesList = new MASchemaSimpleList
            {
                Api = "useraliases",
                AttributeName = "aliases",
                FieldName = "aliases",
                PropertyName = "Aliases",
                CanPatch = false,
                IsReadOnly = false
            };

            UserSchemaTests.Type.Attributes.Add(aliasesList);

        }

        [TestMethod]
        public void TestWebSites()
        {
            IMASchemaAttribute schemaItem =  UserSchemaTests.Type.Attributes.First(t => t.FieldName == "websites");

            User u = new User
            {
                Websites = new List<Website>()
               {
                   new Website()
                   {
                       Primary=true,
                       Type="work",
                       Value="http://work.com"
                   },

                    new Website()
                   {
                       Primary=false,
                       Type="home",
                       Value="http://home.com"
                   }
               }
            };

            CSEntryChange x = CSEntryChange.Create();
            x.ObjectModificationType = ObjectModificationType.Add;
            IList<AttributeChange> result = schemaItem.CreateAttributeChanges(ObjectModificationType.Add, u).ToList();

            AttributeChange change = result.FirstOrDefault(t => t.Name == "websites_work_primary");
            Assert.IsNotNull(change);
            Assert.AreEqual(true, change.GetValueAdd<bool>());
            x.AttributeChanges.Add(change);

            change = result.FirstOrDefault(t => t.Name == "websites_work_value");
            Assert.IsNotNull(change);
            Assert.AreEqual("http://work.com", change.GetValueAdd<string>());
            x.AttributeChanges.Add(change);

            change = result.FirstOrDefault(t => t.Name == "websites_home_primary");
            Assert.IsNotNull(change);
            Assert.AreEqual(false, change.GetValueAdd<bool>());
            x.AttributeChanges.Add(change);

            change = result.FirstOrDefault(t => t.Name == "websites_home_value");
            Assert.IsNotNull(change);
            Assert.AreEqual("http://home.com", change.GetValueAdd<string>());
            x.AttributeChanges.Add(change);

            User ux = new User();
            schemaItem.UpdateField(x, ux);
            Assert.AreEqual("http://work.com", ux.Websites.First(t => t.Type == "work").Value);
            Assert.AreEqual(true, ux.Websites.First(t => t.Type == "work").IsPrimary);
            Assert.AreEqual("http://home.com", ux.Websites.First(t => t.Type == "home").Value);
            Assert.AreEqual(false, ux.Websites.First(t => t.Type == "home").IsPrimary);


            x = CSEntryChange.Create();
            x.ObjectModificationType = ObjectModificationType.Update;

            change = result.FirstOrDefault(t => t.Name == "websites_home_value");
            x.AttributeChanges.Remove(change);
            x.AttributeChanges.Add(AttributeChange.CreateAttributeDelete("websites_home_value"));
            schemaItem.UpdateField(x, ux);

            x = CSEntryChange.Create();
            x.ObjectModificationType = ObjectModificationType.Update;

            change = result.FirstOrDefault(t => t.Name == "websites_other_value");
            x.AttributeChanges.Remove(change);
            x.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("websites_other_value", "http://other.com"));
            schemaItem.UpdateField(x, ux);

        }

        [TestMethod]
        public void TestNames()
        {
            IMASchemaAttribute schemaItem =  UserSchemaTests.Type.Attributes.First(t => t.PropertyName == "Name");

            User u = new User
            {
                Name = new UserName
                {
                    GivenName = "Bob",
                    FamilyName = "Smith"
                }
            };

            IList<AttributeChange> result = schemaItem.CreateAttributeChanges(ObjectModificationType.Add, u).ToList();

            AttributeChange firstNameChange = result.FirstOrDefault(t => t.Name == "name_givenName");
            Assert.IsNotNull(firstNameChange);
            Assert.AreEqual("Bob", firstNameChange.GetValueAdd<string>());

            AttributeChange familyNameChange = result.FirstOrDefault(t => t.Name == "name_familyName");
            Assert.IsNotNull(familyNameChange);
            Assert.AreEqual("Smith", familyNameChange.GetValueAdd<string>());

            CSEntryChange x = CSEntryChange.Create();
            x.ObjectModificationType = ObjectModificationType.Add;
            x.AttributeChanges.Add(result.First());
            x.AttributeChanges.Add(result.Last());

            User ux = new User();
            schemaItem.UpdateField(x, ux);
            Assert.AreEqual("Bob", ux.Name.GivenName);
            Assert.AreEqual("Smith", ux.Name.FamilyName);
        }

        [TestMethod]
        public void TestNotes()
        {
            IMASchemaAttribute schemaItem =  UserSchemaTests.Type.Attributes.First(t => t.PropertyName == "Notes");

            User u = new User
            {
                Notes = new Notes
                {
                    ContentType = "text",
                    Value = "something"
                }
            };

            IList<AttributeChange> result = schemaItem.CreateAttributeChanges(ObjectModificationType.Add, u).ToList();

            AttributeChange notesValue = result.FirstOrDefault(t => t.Name == "notes_value");
            Assert.IsNotNull(notesValue);
            Assert.AreEqual("something", notesValue.GetValueAdd<string>());

            AttributeChange notesType = result.FirstOrDefault(t => t.Name == "notes_contentType");
            Assert.IsNotNull(notesType);
            Assert.AreEqual("text", notesType.GetValueAdd<string>());

            CSEntryChange x = CSEntryChange.Create();
            x.ObjectModificationType = ObjectModificationType.Add;
            x.AttributeChanges.Add(result.First());
            x.AttributeChanges.Add(result.Last());

            User ux = new User();
            schemaItem.UpdateField(x, ux);
            Assert.AreEqual("something", ux.Notes.Value);
            Assert.AreEqual("text", ux.Notes.ContentType);
        }

        [TestMethod]
        public void TestAliases()
        {
            IMASchemaAttribute schemaItem = UserSchemaTests.Type.Attributes.First(t => t.FieldName == "aliases");

            UserUpdateTemplate u = new UserUpdateTemplate
            {
                Aliases = new List<string>()
                {
                    "alias1@test.com",
                    "alias2@test.com"
                }
            };

            IList<AttributeChange> result = schemaItem.CreateAttributeChanges(ObjectModificationType.Add, u).ToList();

            AttributeChange change = result.FirstOrDefault(t => t.Name == "aliases");
            Assert.IsNotNull(change);
            CollectionAssert.AreEqual(new string[] {   "alias1@test.com",
                    "alias2@test.com" }, change.GetValueAdds<string>().ToArray());
            

            CSEntryChange x = CSEntryChange.Create();
            x.ObjectModificationType = ObjectModificationType.Add;
            x.AttributeChanges.Add(change);

            UserUpdateTemplate ux = new UserUpdateTemplate();

            schemaItem.UpdateField(x, ux);
            CollectionAssert.AreEqual(new string[]
            {
                "alias1@test.com",
                "alias2@test.com"
            }, ux.Aliases.ToArray());
        }


        [TestMethod]
        public void TestStandaloneAttributes()
        {
            IMASchemaAttribute orgUnitPath =  UserSchemaTests.Type.Attributes.First(t => t.FieldName == "orgUnitPath");
            IMASchemaAttribute includeInGlobalAddressList =  UserSchemaTests.Type.Attributes.First(t => t.FieldName == "includeInGlobalAddressList");
            IMASchemaAttribute suspended =  UserSchemaTests.Type.Attributes.First(t => t.FieldName == "suspended");
            IMASchemaAttribute changePasswordAtNextLogin =  UserSchemaTests.Type.Attributes.First(t => t.FieldName == "changePasswordAtNextLogin");
            IMASchemaAttribute ipWhitelisted =  UserSchemaTests.Type.Attributes.First(t => t.FieldName == "ipWhitelisted");
            IMASchemaAttribute customerId =  UserSchemaTests.Type.Attributes.First(t => t.FieldName == "customerId");
            IMASchemaAttribute primaryEmail =  UserSchemaTests.Type.Attributes.First(t => t.FieldName == "primaryEmail");
            IMASchemaAttribute id =  UserSchemaTests.Type.Attributes.First(t => t.FieldName == "id");

            User u = new User
            {
                OrgUnitPath = "/Test",
                IncludeInGlobalAddressList = true,
                Suspended = true,
                ChangePasswordAtNextLogin = true,
                IpWhitelisted = true,
                CustomerId = "mytest",
                PrimaryEmail = "test@test.com",
                Id = "testid"
            };

            CSEntryChange x = CSEntryChange.Create();
            x.ObjectModificationType = ObjectModificationType.Add;

            IList<AttributeChange> result = orgUnitPath.CreateAttributeChanges(ObjectModificationType.Add, u).ToList();
            AttributeChange change = result.FirstOrDefault(t => t.Name == "orgUnitPath");
            Assert.IsNotNull(change);
            Assert.AreEqual("/Test", change.GetValueAdd<string>());
            x.AttributeChanges.Add(result.First());

            result = includeInGlobalAddressList.CreateAttributeChanges(ObjectModificationType.Add, u).ToList();
            change = result.FirstOrDefault(t => t.Name == "includeInGlobalAddressList");
            Assert.IsNotNull(change);
            Assert.AreEqual(true, change.GetValueAdd<bool>());
            x.AttributeChanges.Add(result.First());

            result = suspended.CreateAttributeChanges(ObjectModificationType.Add, u).ToList();
            change = result.FirstOrDefault(t => t.Name == "suspended");
            Assert.IsNotNull(change);
            Assert.AreEqual(true, change.GetValueAdd<bool>());
            x.AttributeChanges.Add(result.First());

            result = changePasswordAtNextLogin.CreateAttributeChanges(ObjectModificationType.Add, u).ToList();
            change = result.FirstOrDefault(t => t.Name == "changePasswordAtNextLogin");
            Assert.IsNotNull(change);
            Assert.AreEqual(true, change.GetValueAdd<bool>());
            x.AttributeChanges.Add(result.First());

            result = ipWhitelisted.CreateAttributeChanges(ObjectModificationType.Add, u).ToList();
            change = result.FirstOrDefault(t => t.Name == "ipWhitelisted");
            Assert.IsNotNull(change);
            Assert.AreEqual(true, change.GetValueAdd<bool>());
            x.AttributeChanges.Add(result.First());

            result = customerId.CreateAttributeChanges(ObjectModificationType.Add, u).ToList();
            change = result.FirstOrDefault(t => t.Name == "customerId");
            Assert.IsNotNull(change);
            Assert.AreEqual("mytest", change.GetValueAdd<string>());
            x.AttributeChanges.Add(result.First());

            result = primaryEmail.CreateAttributeChanges(ObjectModificationType.Add, u).ToList();
            change = result.FirstOrDefault(t => t.Name == "primaryEmail");
            Assert.IsNotNull(change);
            Assert.AreEqual("test@test.com", change.GetValueAdd<string>());
            x.AttributeChanges.Add(result.First());

            result = id.CreateAttributeChanges(ObjectModificationType.Add, u).ToList();
            change = result.FirstOrDefault(t => t.Name == "id");
            Assert.IsNotNull(change);
            Assert.AreEqual("testid", change.GetValueAdd<string>());
            x.AttributeChanges.Add(result.First());



            User ux = new User();
            id.UpdateField(x, ux);
            primaryEmail.UpdateField(x, ux);
            customerId.UpdateField(x, ux);
            ipWhitelisted.UpdateField(x, ux);
            changePasswordAtNextLogin.UpdateField(x, ux);
            suspended.UpdateField(x, ux);
            includeInGlobalAddressList.UpdateField(x, ux);
            orgUnitPath.UpdateField(x, ux);

            Assert.AreEqual("testid", ux.Id);
            Assert.AreEqual("test@test.com", ux.PrimaryEmail);
            Assert.AreEqual("mytest", ux.CustomerId);
            Assert.AreEqual(true, ux.IpWhitelisted);
            Assert.AreEqual(true, ux.ChangePasswordAtNextLogin);
            Assert.AreEqual(true, ux.Suspended);
            Assert.AreEqual(true, ux.IncludeInGlobalAddressList);
            Assert.AreEqual("/Test", ux.OrgUnitPath);
        }
    }
}
