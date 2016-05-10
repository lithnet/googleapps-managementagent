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
    public class MASchemaAttributeTests
    {
        [TestMethod]
        public void TestToCSEntryChangeAdd()
        {
            IMASchemaAttribute schemaItem = UserSchemaTests.Type.Attributes.First(t => t.FieldName == "orgUnitPath");

            User u = new User
            {
                OrgUnitPath = "/Test"
            };

            CSEntryChange x = CSEntryChange.Create();
            x.ObjectModificationType = ObjectModificationType.Add;

            IList<AttributeChange> result = schemaItem.CreateAttributeChanges(x.ObjectModificationType, u).ToList();

            AttributeChange change = result.FirstOrDefault(t => t.Name == "orgUnitPath");
            Assert.IsNotNull(change);
            Assert.AreEqual("/Test", change.GetValueAdd<string>());
            Assert.AreEqual(AttributeModificationType.Add, change.ModificationType);
            x.AttributeChanges.Add(change);
            
            User ux = new User();
            schemaItem.UpdateField(x, ux);
            Assert.AreEqual("/Test", ux.OrgUnitPath);
        }

        [TestMethod]
        public void TestToCSEntryChangeReplace()
        {
            IMASchemaAttribute schemaItem = UserSchemaTests.Type.Attributes.First(t => t.FieldName == "orgUnitPath");

            User u = new User
            {
                OrgUnitPath = "/Test"
            };

            CSEntryChange x = CSEntryChange.Create();
            x.ObjectModificationType = ObjectModificationType.Replace;

            IList<AttributeChange> result = schemaItem.CreateAttributeChanges(x.ObjectModificationType, u).ToList();

            AttributeChange change = result.FirstOrDefault(t => t.Name == "orgUnitPath");
            Assert.IsNotNull(change);
            Assert.AreEqual("/Test", change.GetValueAdd<string>());
            Assert.AreEqual(AttributeModificationType.Add, change.ModificationType);
            x.AttributeChanges.Add(change);

            User ux = new User();
            schemaItem.UpdateField(x, ux);
            Assert.AreEqual("/Test", ux.OrgUnitPath);
        }

        [TestMethod]
        public void TestToCSEntryChangeUpdate()
        {
            IMASchemaAttribute schemaItem = UserSchemaTests.Type.Attributes.First(t => t.FieldName == "orgUnitPath");

            User u = new User
            {
                OrgUnitPath = "/Test"
            };

            CSEntryChange x = CSEntryChange.Create();
            x.ObjectModificationType = ObjectModificationType.Update;

            IList<AttributeChange> result = schemaItem.CreateAttributeChanges(x.ObjectModificationType, u).ToList();

            AttributeChange change = result.FirstOrDefault(t => t.Name == "orgUnitPath");
            Assert.IsNotNull(change);
            Assert.AreEqual("/Test", change.GetValueAdd<string>());
            Assert.AreEqual(AttributeModificationType.Replace, change.ModificationType);
            x.AttributeChanges.Add(change);

            User ux = new User();
            schemaItem.UpdateField(x, ux);
            Assert.AreEqual("/Test", ux.OrgUnitPath);
        }

        [TestMethod]
        public void TestFromCSEntryChangeAdd()
        {
            IMASchemaAttribute schemaItem = UserSchemaTests.Type.Attributes.First(t => t.FieldName == "orgUnitPath");

            CSEntryChange x = CSEntryChange.Create();
            x.ObjectModificationType = ObjectModificationType.Add;

            x.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("orgUnitPath", "/Test"));

            User ux = new User();
            schemaItem.UpdateField(x, ux);

            Assert.AreEqual("/Test", ux.OrgUnitPath);
        }

        [TestMethod]
        public void TestFromCSEntryChangeReplace()
        {
            IMASchemaAttribute schemaItem = UserSchemaTests.Type.Attributes.First(t => t.FieldName == "orgUnitPath");

            CSEntryChange x = CSEntryChange.Create();
            x.ObjectModificationType = ObjectModificationType.Update;

            x.AttributeChanges.Add(AttributeChange.CreateAttributeReplace("orgUnitPath", "/Test"));

            User ux = new User();
            schemaItem.UpdateField(x, ux);

            Assert.AreEqual("/Test", ux.OrgUnitPath);
        }

        [TestMethod]
        public void TestFromCSEntryChangeUpdate()
        {
            IMASchemaAttribute schemaItem = UserSchemaTests.Type.Attributes.First(t => t.FieldName == "orgUnitPath");

            CSEntryChange x = CSEntryChange.Create();
            x.ObjectModificationType = ObjectModificationType.Update;

            x.AttributeChanges.Add(AttributeChange.CreateAttributeReplace("orgUnitPath", "/Test"));

            User ux = new User();
            schemaItem.UpdateField(x, ux);

            Assert.AreEqual("/Test", ux.OrgUnitPath);
        }

        [TestMethod]
        public void TestFromCSEntryChangeDelete()
        {
            IMASchemaAttribute schemaItem = UserSchemaTests.Type.Attributes.First(t => t.FieldName == "orgUnitPath");

            CSEntryChange x = CSEntryChange.Create();
            x.ObjectModificationType = ObjectModificationType.Update;

            x.AttributeChanges.Add(AttributeChange.CreateAttributeDelete("orgUnitPath"));

            User ux = new User();
            schemaItem.UpdateField(x, ux);

            Assert.AreEqual(Constants.NullValuePlaceholder, ux.OrgUnitPath);
        }
    }
}