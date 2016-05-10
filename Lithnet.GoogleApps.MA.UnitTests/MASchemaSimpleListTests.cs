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
    public class MASchemaSimpleListTests
    {
        [TestMethod]
        public void TestToCSEntryChangeAdd()
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

            CSEntryChange x = CSEntryChange.Create();
            x.ObjectModificationType = ObjectModificationType.Add;
            IList<AttributeChange> result = schemaItem.CreateAttributeChanges(x.ObjectModificationType, u).ToList();

            AttributeChange change = result.FirstOrDefault(t => t.Name == "aliases");
            Assert.IsNotNull(change);
            CollectionAssert.AreEqual(new string[] {   "alias1@test.com",
                    "alias2@test.com" }, change.GetValueAdds<string>().ToArray());
            Assert.AreEqual(AttributeModificationType.Add, change.ModificationType);
        }

        [TestMethod]
        public void TestToCSEntryChangeReplace()
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

            CSEntryChange x = CSEntryChange.Create();
            x.ObjectModificationType = ObjectModificationType.Replace;
            IList<AttributeChange> result = schemaItem.CreateAttributeChanges(x.ObjectModificationType, u).ToList();

            AttributeChange change = result.FirstOrDefault(t => t.Name == "aliases");
            Assert.IsNotNull(change);
            CollectionAssert.AreEqual(new string[] {   "alias1@test.com",
                    "alias2@test.com" }, change.GetValueAdds<string>().ToArray());
            Assert.AreEqual(AttributeModificationType.Add, change.ModificationType);
        }

        [TestMethod]
        public void TestToCSEntryChangeUpdate()
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

            CSEntryChange x = CSEntryChange.Create();
            x.ObjectModificationType = ObjectModificationType.Update;
            IList<AttributeChange> result = schemaItem.CreateAttributeChanges(x.ObjectModificationType, u).ToList();

            AttributeChange change = result.FirstOrDefault(t => t.Name == "aliases");
            Assert.IsNotNull(change);
            CollectionAssert.AreEqual(new string[] {   "alias1@test.com",
                    "alias2@test.com" }, change.GetValueAdds<string>().ToArray());
            Assert.AreEqual(AttributeModificationType.Replace, change.ModificationType);
        }

        [TestMethod]
        public void TestFromCSEntryChangeAdd()
        {
            IMASchemaAttribute schemaItem = UserSchemaTests.Type.Attributes.First(t => t.FieldName == "aliases");

            CSEntryChange x = CSEntryChange.Create();
            x.ObjectModificationType = ObjectModificationType.Add;

            x.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("aliases", new List<object>()
                {
                    "alias1@test.com",
                    "alias2@test.com"
                }));

            UserUpdateTemplate ux = new UserUpdateTemplate();
            schemaItem.UpdateField(x, ux);

            CollectionAssert.AreEqual(new string[] {   "alias1@test.com",
                    "alias2@test.com" }, ux.Aliases.ToArray());
        }

        [TestMethod]
        public void TestFromCSEntryChangeReplace()
        {
            IMASchemaAttribute schemaItem = UserSchemaTests.Type.Attributes.First(t => t.FieldName == "aliases");

            CSEntryChange x = CSEntryChange.Create();
            x.ObjectModificationType = ObjectModificationType.Update;

            x.AttributeChanges.Add(AttributeChange.CreateAttributeReplace("aliases", new List<object>()
                {
                    "alias1@test.com",
                    "alias2@test.com"
                }));

            UserUpdateTemplate ux = new UserUpdateTemplate();
            schemaItem.UpdateField(x, ux);

            CollectionAssert.AreEqual(new string[] {   "alias1@test.com",
                    "alias2@test.com" }, ux.Aliases.ToArray());
        }

        [TestMethod]
        public void TestFromCSEntryChangeUpdate()
        {
            IMASchemaAttribute schemaItem = UserSchemaTests.Type.Attributes.First(t => t.FieldName == "aliases");

            CSEntryChange x = CSEntryChange.Create();
            x.ObjectModificationType = ObjectModificationType.Update;

            List<ValueChange> changes = new List<ValueChange>();
            changes.Add(ValueChange.CreateValueAdd("alias3@test.com"));
            changes.Add(ValueChange.CreateValueDelete("alias2@test.com"));

            x.AttributeChanges.Add(AttributeChange.CreateAttributeUpdate("aliases", changes));

            UserUpdateTemplate ux = new UserUpdateTemplate();
            ux.Aliases = new List<string>() {"alias1@test.com", "alias2@test.com"};
            schemaItem.UpdateField(x, ux);

            CollectionAssert.AreEqual(new string[] {   "alias1@test.com",
                    "alias3@test.com" }, ux.Aliases.ToArray());
        }

        [TestMethod]
        public void TestFromCSEntryChangeDelete()
        {
            IMASchemaAttribute schemaItem = UserSchemaTests.Type.Attributes.First(t => t.FieldName == "aliases");

            CSEntryChange x = CSEntryChange.Create();
            x.ObjectModificationType = ObjectModificationType.Update;

            x.AttributeChanges.Add(AttributeChange.CreateAttributeDelete("aliases"));

            UserUpdateTemplate ux = new UserUpdateTemplate();
            ux.Aliases = new List<string>() { "alias1@test.com", "alias2@test.com" };
            schemaItem.UpdateField(x, ux);

            Assert.AreEqual(0, ux.Aliases.Count);
        }
    }
}