using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using Lithnet.GoogleApps.ManagedObjects;
using Lithnet.MetadirectoryServices;
using Microsoft.MetadirectoryServices;

namespace Lithnet.GoogleApps.MA.UnitTests
{
    using Google.GData.Contacts;
    using Google.GData.Extensions;
    using Website = global::Lithnet.GoogleApps.ManagedObjects.Website;

    [TestClass]
    public class AdapterGDataSimpleAttributeListTTests
    {
        [TestMethod]
        public void TestToCSEntryChangeAdd()
        {
            IAttributeAdapter schemaItem = UnitTestControl.Schema["contact"].GetAdapterForMmsAttribute("externalIds_work");

            ContactEntry e = new ContactEntry();

            e.ExternalIds.Add(new ExternalId()
            { 
                 Value = "id",
                 Label = "work"
            });

            CSEntryChange x = CSEntryChange.Create();
            x.ObjectModificationType = ObjectModificationType.Add;
            IList<AttributeChange> result = schemaItem.CreateAttributeChanges(x.DN, x.ObjectModificationType, e).ToList();

            AttributeChange change;

            change = result.FirstOrDefault(t => t.Name == "externalIds_work");
            Assert.IsNotNull(change);
            Assert.AreEqual("id", change.GetValueAdd<string>());
            x.AttributeChanges.Add(change);
        }

        [TestMethod]
        public void TestToCSEntryChangeReplace()
        {
            IAttributeAdapter schemaItem = UnitTestControl.Schema["contact"].GetAdapterForMmsAttribute("externalIds_work");

            ContactEntry e = new ContactEntry();

            e.ExternalIds.Add(new ExternalId()
            {
                Value = "id",
                Label = "work"
            });

            CSEntryChange x = CSEntryChange.Create();
            x.ObjectModificationType = ObjectModificationType.Replace;
            IList<AttributeChange> result = schemaItem.CreateAttributeChanges(x.DN, x.ObjectModificationType, e).ToList();

            AttributeChange change;

            change = result.FirstOrDefault(t => t.Name == "externalIds_work");
            Assert.IsNotNull(change);
            Assert.AreEqual("id", change.GetValueAdd<string>());
            Assert.AreEqual(AttributeModificationType.Add, change.ModificationType);
            x.AttributeChanges.Add(change);
        }

        [TestMethod]
        public void TestToCSEntryChangeUpdate()
        {
            IAttributeAdapter schemaItem = UnitTestControl.Schema["contact"].GetAdapterForMmsAttribute("externalIds_work");

            ContactEntry e = new ContactEntry();

            e.ExternalIds.Add(new ExternalId()
            {
                Value = "id",
                Label = "work"
            });

            CSEntryChange x = CSEntryChange.Create();
            x.ObjectModificationType = ObjectModificationType.Update;
            IList<AttributeChange> result = schemaItem.CreateAttributeChanges(x.DN, x.ObjectModificationType, e).ToList();

            AttributeChange change;

            change = result.FirstOrDefault(t => t.Name == "externalIds_work");
            Assert.IsNotNull(change);
            Assert.AreEqual("id", change.GetValueAdd<string>());
            Assert.AreEqual(AttributeModificationType.Replace, change.ModificationType);
            x.AttributeChanges.Add(change);
        }

        [TestMethod]
        public void TestFromCSEntryChangeAdd()
        {
            IAttributeAdapter schemaItem = UnitTestControl.Schema["contact"].GetAdapterForMmsAttribute("externalIds_work");

            ContactEntry e = new ContactEntry();

            CSEntryChange x = CSEntryChange.Create();
            x.ObjectModificationType = ObjectModificationType.Add;
            x.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("externalIds_work", "myorg"));
           
            schemaItem.UpdateField(x, e);
            ExternalId o = e.ExternalIds.First(t => t.Label == "work");

            Assert.AreEqual("myorg", o.Value);
        }

        [TestMethod]
        public void TestFromCSEntryChangeUpdate()
        {
            IAttributeAdapter schemaItem = UnitTestControl.Schema["contact"].GetAdapterForMmsAttribute("externalIds_work");

            ContactEntry e = new ContactEntry();
            e.ExternalIds.Add(new ExternalId()
            {
                Value = "id",
                Label = "work"
            });

            CSEntryChange x = CSEntryChange.Create();
            x.ObjectModificationType = ObjectModificationType.Update;
            x.AttributeChanges.Add(AttributeChange.CreateAttributeReplace("externalIds_work", "myorg"));

            schemaItem.UpdateField(x, e);
            ExternalId o = e.ExternalIds.First(t => t.Label == "work");

            Assert.AreEqual("myorg", o.Value);
        }
    }
}