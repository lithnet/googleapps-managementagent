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
        static UserSchemaTests()
        {
        }
        
        [TestMethod]
        public void TestWebSites()
        {
            IAttributeAdapter schemaItem =  UnitTestControl.Schema["user"].AttributeAdapters.First(t => t.FieldName == "websites");

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
            IList<AttributeChange> result = schemaItem.CreateAttributeChanges(x.DN, ObjectModificationType.Add, u).ToList();

            AttributeChange change = result.FirstOrDefault(t => t.Name == "websites_work");
            Assert.IsNotNull(change);
            Assert.AreEqual("http://work.com", change.GetValueAdd<string>());
            x.AttributeChanges.Add(change);

            change = result.FirstOrDefault(t => t.Name == "websites_home");
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

            change = result.FirstOrDefault(t => t.Name == "websites_home");
            x.AttributeChanges.Remove(change);
            x.AttributeChanges.Add(AttributeChange.CreateAttributeDelete("websites_home"));
            schemaItem.UpdateField(x, ux);

            x = CSEntryChange.Create();
            x.ObjectModificationType = ObjectModificationType.Update;

            change = result.FirstOrDefault(t => t.Name == "websites_other");
            x.AttributeChanges.Remove(change);
            x.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("websites_other", "http://other.com"));
            schemaItem.UpdateField(x, ux);

        }

        [TestMethod]
        public void TestNames()
        {
            IAttributeAdapter schemaItem =  UnitTestControl.Schema["user"].AttributeAdapters.First(t => t.FieldName == "name");

            User u = new User
            {
                Name = new UserName
                {
                    GivenName = "Bob",
                    FamilyName = "Smith"
                }
            };

            IList<AttributeChange> result = schemaItem.CreateAttributeChanges("test", ObjectModificationType.Add, u).ToList();

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
            IAttributeAdapter schemaItem =  UnitTestControl.Schema["user"].AttributeAdapters.First(t => t.FieldName == "notes");

            User u = new User
            {
                Notes = new Notes
                {
                    ContentType = "text",
                    Value = "something"
                }
            };

            IList<AttributeChange> result = schemaItem.CreateAttributeChanges(null, ObjectModificationType.Add, u).ToList();

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
            IAttributeAdapter schemaItem = UnitTestControl.Schema["user"].AttributeAdapters.First(t => t.FieldName == "aliases");

            UserUpdateTemplate u = new UserUpdateTemplate
            {
                Aliases = new List<string>()
                {
                    "alias1@test.com",
                    "alias2@test.com"
                }
            };

            IList<AttributeChange> result = schemaItem.CreateAttributeChanges(null, ObjectModificationType.Add, u).ToList();

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
            IAttributeAdapter orgUnitPath =  UnitTestControl.Schema["user"].AttributeAdapters.First(t => t.FieldName == "orgUnitPath");
            IAttributeAdapter includeInGlobalAddressList =  UnitTestControl.Schema["user"].AttributeAdapters.First(t => t.FieldName == "includeInGlobalAddressList");
            IAttributeAdapter suspended =  UnitTestControl.Schema["user"].AttributeAdapters.First(t => t.FieldName == "suspended");
            IAttributeAdapter changePasswordAtNextLogin =  UnitTestControl.Schema["user"].AttributeAdapters.First(t => t.FieldName == "changePasswordAtNextLogin");
            IAttributeAdapter ipWhitelisted =  UnitTestControl.Schema["user"].AttributeAdapters.First(t => t.FieldName == "ipWhitelisted");
            IAttributeAdapter customerId =  UnitTestControl.Schema["user"].AttributeAdapters.First(t => t.FieldName == "customerId");
            IAttributeAdapter primaryEmail =  UnitTestControl.Schema["user"].AttributeAdapters.First(t => t.FieldName == "primaryEmail");
            IAttributeAdapter id =  UnitTestControl.Schema["user"].AttributeAdapters.First(t => t.FieldName == "id");

            User u = new User
            {
                OrgUnitPath = "/Test",
                IncludeInGlobalAddressList = true,
                Suspended = true,
                ChangePasswordAtNextLogin = true,
                IpWhitelisted = true,
                CustomerId = "mytest",
                PrimaryEmail = "test@lithnet.io",
                Id = "testid"
            };

            CSEntryChange x = CSEntryChange.Create();
            x.ObjectModificationType = ObjectModificationType.Add;

            IList<AttributeChange> result = orgUnitPath.CreateAttributeChanges(x.DN, ObjectModificationType.Add, u).ToList();
            AttributeChange change = result.FirstOrDefault(t => t.Name == "orgUnitPath");
            Assert.IsNotNull(change);
            Assert.AreEqual("/Test", change.GetValueAdd<string>());
            x.AttributeChanges.Add(result.First());

            result = includeInGlobalAddressList.CreateAttributeChanges(x.DN, ObjectModificationType.Add, u).ToList();
            change = result.FirstOrDefault(t => t.Name == "includeInGlobalAddressList");
            Assert.IsNotNull(change);
            Assert.AreEqual(true, change.GetValueAdd<bool>());
            x.AttributeChanges.Add(result.First());

            result = suspended.CreateAttributeChanges(x.DN, ObjectModificationType.Add, u).ToList();
            change = result.FirstOrDefault(t => t.Name == "suspended");
            Assert.IsNotNull(change);
            Assert.AreEqual(true, change.GetValueAdd<bool>());
            x.AttributeChanges.Add(result.First());

            result = changePasswordAtNextLogin.CreateAttributeChanges(x.DN, ObjectModificationType.Add, u).ToList();
            change = result.FirstOrDefault(t => t.Name == "changePasswordAtNextLogin");
            Assert.IsNotNull(change);
            Assert.AreEqual(true, change.GetValueAdd<bool>());
            x.AttributeChanges.Add(result.First());

            result = ipWhitelisted.CreateAttributeChanges(x.DN, ObjectModificationType.Add, u).ToList();
            change = result.FirstOrDefault(t => t.Name == "ipWhitelisted");
            Assert.IsNotNull(change);
            Assert.AreEqual(true, change.GetValueAdd<bool>());
            x.AttributeChanges.Add(result.First());

            result = customerId.CreateAttributeChanges(x.DN, ObjectModificationType.Add, u).ToList();
            change = result.FirstOrDefault(t => t.Name == "customerId");
            Assert.IsNotNull(change);
            Assert.AreEqual("mytest", change.GetValueAdd<string>());
            x.AttributeChanges.Add(result.First());

            result = primaryEmail.CreateAttributeChanges(x.DN, ObjectModificationType.Add, u).ToList();
            change = result.FirstOrDefault(t => t.Name == "primaryEmail");
            Assert.IsNotNull(change);
            Assert.AreEqual("test@lithnet.io", change.GetValueAdd<string>());
            x.AttributeChanges.Add(result.First());

            result = id.CreateAttributeChanges(x.DN, ObjectModificationType.Add, u).ToList();
            change = result.FirstOrDefault(t => t.Name == "id");
            Assert.IsNotNull(change);
            Assert.AreEqual("testid", change.GetValueAdd<string>());
            x.AttributeChanges.Add(result.First());



            User ux = new User();
            changePasswordAtNextLogin.UpdateField(x, ux);
            suspended.UpdateField(x, ux);
            includeInGlobalAddressList.UpdateField(x, ux);
            orgUnitPath.UpdateField(x, ux);

            Assert.AreEqual(true, ux.ChangePasswordAtNextLogin);
            Assert.AreEqual(true, ux.Suspended);
            Assert.AreEqual(true, ux.IncludeInGlobalAddressList);
            Assert.AreEqual("/Test", ux.OrgUnitPath);
        }
    }
}
