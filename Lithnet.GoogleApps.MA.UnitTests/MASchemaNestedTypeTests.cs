using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using Lithnet.GoogleApps.ManagedObjects;
using Lithnet.MetadirectoryServices;
using Microsoft.MetadirectoryServices;

namespace Lithnet.GoogleApps.MA.UnitTests
{
    [TestClass]
    public class MASchemaNestedTypeTests
    {
        [TestMethod]
        public void TestToCSEntryChangeAdd()
        {
            IMASchemaAttribute schemaItem = UnitTestControl.Schema["user"].Attributes.First(t => t.PropertyName == "Name");

            User u = new User
            {
                Name = new UserName
                {
                    GivenName = "Bob",
                    FamilyName = "Smith"
                }
            };

            CSEntryChange x = CSEntryChange.Create();
            x.ObjectModificationType = ObjectModificationType.Add;

            IList<AttributeChange> result = schemaItem.CreateAttributeChanges(x.DN, ObjectModificationType.Add, u).ToList();

            AttributeChange change = result.FirstOrDefault(t => t.Name == "name_givenName");
            Assert.IsNotNull(change);
            Assert.AreEqual("Bob", change.GetValueAdd<string>());
            x.AttributeChanges.Add(change);

            change = result.FirstOrDefault(t => t.Name == "name_familyName");
            Assert.IsNotNull(change);
            Assert.AreEqual("Smith", change.GetValueAdd<string>());
            x.AttributeChanges.Add(change);

            User ux = new User();
            schemaItem.UpdateField(x, ux);
            Assert.AreEqual("Bob", ux.Name.GivenName);
            Assert.AreEqual("Smith", ux.Name.FamilyName);
        }

        [TestMethod]
        public void TestToCSEntryChangeReplace()
        {
            IMASchemaAttribute schemaItem = UnitTestControl.Schema["user"].Attributes.First(t => t.PropertyName == "Name");

            User u = new User
            {
                Name = new UserName
                {
                    GivenName = "Bob",
                    FamilyName = "Smith"
                }
            };

            CSEntryChange x = CSEntryChange.Create();
            x.ObjectModificationType = ObjectModificationType.Replace;

            IList<AttributeChange> result = schemaItem.CreateAttributeChanges(x.DN, x.ObjectModificationType, u).ToList();

            AttributeChange change = result.FirstOrDefault(t => t.Name == "name_givenName");
            Assert.IsNotNull(change);
            Assert.AreEqual("Bob", change.GetValueAdd<string>());
            x.AttributeChanges.Add(change);

            change = result.FirstOrDefault(t => t.Name == "name_familyName");
            Assert.IsNotNull(change);
            Assert.AreEqual("Smith", change.GetValueAdd<string>());
            x.AttributeChanges.Add(change);

            User ux = new User();
            schemaItem.UpdateField(x, ux);
            Assert.AreEqual("Bob", ux.Name.GivenName);
            Assert.AreEqual("Smith", ux.Name.FamilyName);
        }

        [TestMethod]
        public void TestToCSEntryChangeUpdate()
        {
            IMASchemaAttribute schemaItem = UnitTestControl.Schema["user"].Attributes.First(t => t.PropertyName == "Name");

            User u = new User
            {
                Name = new UserName
                {
                    GivenName = "Bob",
                    FamilyName = "Smith"
                }
            };

            CSEntryChange x = CSEntryChange.Create();
            x.ObjectModificationType = ObjectModificationType.Update;

            IList<AttributeChange> result = schemaItem.CreateAttributeChanges(x.DN, x.ObjectModificationType, u).ToList();

            AttributeChange change = result.FirstOrDefault(t => t.Name == "name_givenName");
            Assert.IsNotNull(change);
            Assert.AreEqual("Bob", change.GetValueAdd<string>());
            Assert.AreEqual(AttributeModificationType.Replace, change.ModificationType);
            x.AttributeChanges.Add(change);

            change = result.FirstOrDefault(t => t.Name == "name_familyName");
            Assert.IsNotNull(change);
            Assert.AreEqual("Smith", change.GetValueAdd<string>());
            Assert.AreEqual(AttributeModificationType.Replace, change.ModificationType);
            x.AttributeChanges.Add(change);

            User ux = new User();
            schemaItem.UpdateField(x, ux);
            Assert.AreEqual("Bob", ux.Name.GivenName);
            Assert.AreEqual("Smith", ux.Name.FamilyName);
        }

        [TestMethod]
        public void TestFromCSEntryChangeAdd()
        {
            IMASchemaAttribute schemaItem = UnitTestControl.Schema["user"].Attributes.First(t => t.PropertyName == "Name");

            CSEntryChange x = CSEntryChange.Create();
            x.ObjectModificationType = ObjectModificationType.Add;

            x.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("name_givenName", "Bob"));
            x.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("name_familyName", "Smith"));

            User ux = new User();
            schemaItem.UpdateField(x, ux);

            Assert.AreEqual("Bob", ux.Name.GivenName);
            Assert.AreEqual("Smith", ux.Name.FamilyName);
        }

        [TestMethod]
        public void TestFromCSEntryChangeReplace()
        {
            IMASchemaAttribute schemaItem = UnitTestControl.Schema["user"].Attributes.First(t => t.PropertyName == "Name");

            CSEntryChange x = CSEntryChange.Create();
            x.ObjectModificationType = ObjectModificationType.Replace;

            x.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("name_givenName", "Bob"));
            x.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("name_familyName", "Smith"));

            User ux = new User();
            ux.Name.GivenName = "NotBob";
            ux.Name.FamilyName = "NotSmith";

            schemaItem.UpdateField(x, ux);

            Assert.AreEqual("Bob", ux.Name.GivenName);
            Assert.AreEqual("Smith", ux.Name.FamilyName);
        }

        [TestMethod]
        public void TestFromCSEntryChangeUpdate()
        {
            IMASchemaAttribute schemaItem = UnitTestControl.Schema["user"].Attributes.First(t => t.PropertyName == "Name");

            CSEntryChange x = CSEntryChange.Create();
            x.ObjectModificationType = ObjectModificationType.Update;

            x.AttributeChanges.Add(AttributeChange.CreateAttributeReplace("name_givenName", "Bob"));
            x.AttributeChanges.Add(AttributeChange.CreateAttributeReplace("name_familyName", "Smith"));

            User ux = new User();
            ux.Name.GivenName = "NotBob";
            ux.Name.FamilyName = "NotSmith";

            schemaItem.UpdateField(x, ux);

            Assert.AreEqual("Bob", ux.Name.GivenName);
            Assert.AreEqual("Smith", ux.Name.FamilyName);
        }

        [TestMethod]
        public void TestFromCSEntryChangeDelete()
        {
            IMASchemaAttribute schemaItem = UnitTestControl.Schema["user"].Attributes.First(t => t.PropertyName == "Name");

            CSEntryChange x = CSEntryChange.Create();
            x.ObjectModificationType = ObjectModificationType.Update;

            x.AttributeChanges.Add(AttributeChange.CreateAttributeDelete("name_givenName"));

            User ux = new User();
            ux.Name.GivenName = "Bob";
            ux.Name.FamilyName = "Smith";

            schemaItem.UpdateField(x, ux);

            Assert.AreEqual(Constants.NullValuePlaceholder, ux.Name.GivenName);
            Assert.AreEqual("Smith", ux.Name.FamilyName);
        }
    }
}