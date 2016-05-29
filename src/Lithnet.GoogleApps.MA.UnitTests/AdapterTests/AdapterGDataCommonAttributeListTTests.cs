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
    public class AdapterGDataCommonAttributeListTTests
    {
        [TestMethod]
        public void TestToCSEntryChangeAdd()
        {
            IAttributeAdapter schemaItem = UnitTestControl.Schema["contact"].Attributes.First(t => t.AttributeName == "organizations");

            ContactEntry e = new ContactEntry();

            e.Organizations.Add(new Organization()
            {
                Name = "myorg",
                Department = "department",
                JobDescription = "jobdescription",
                Location = "location",
                Symbol = "symbol",
                Title = "title",
                Rel = "http://schemas.google.com/g/2005#work"
            });

            CSEntryChange x = CSEntryChange.Create();
            x.ObjectModificationType = ObjectModificationType.Add;
            IList<AttributeChange> result = schemaItem.CreateAttributeChanges(x.DN, x.ObjectModificationType, e).ToList();

            AttributeChange change;

            change = result.FirstOrDefault(t => t.Name == "organizations_work_name");
            Assert.IsNotNull(change);
            Assert.AreEqual("myorg", change.GetValueAdd<string>());
            x.AttributeChanges.Add(change);

            change = result.FirstOrDefault(t => t.Name == "organizations_work_department");
            Assert.IsNotNull(change);
            Assert.AreEqual("department", change.GetValueAdd<string>());
            x.AttributeChanges.Add(change);

            change = result.FirstOrDefault(t => t.Name == "organizations_work_jobDescription");
            Assert.IsNotNull(change);
            Assert.AreEqual("jobdescription", change.GetValueAdd<string>());
            x.AttributeChanges.Add(change);

            change = result.FirstOrDefault(t => t.Name == "organizations_work_location");
            Assert.IsNotNull(change);
            Assert.AreEqual("location", change.GetValueAdd<string>());
            x.AttributeChanges.Add(change);

            change = result.FirstOrDefault(t => t.Name == "organizations_work_symbol");
            Assert.IsNotNull(change);
            Assert.AreEqual("symbol", change.GetValueAdd<string>());
            x.AttributeChanges.Add(change);

            change = result.FirstOrDefault(t => t.Name == "organizations_work_title");
            Assert.IsNotNull(change);
            Assert.AreEqual("title", change.GetValueAdd<string>());
            x.AttributeChanges.Add(change);
        }

        [TestMethod]
        public void TestToCSEntryChangeReplace()
        {
            IAttributeAdapter schemaItem = UnitTestControl.Schema["contact"].Attributes.First(t => t.AttributeName == "organizations");

            ContactEntry e = new ContactEntry();

            e.Organizations.Add(new Organization()
            {
                Name = "myorg",
                Department = "department",
                JobDescription = "jobdescription",
                Location = "location",
                Symbol = "symbol",
                Title = "title",
                Rel = "http://schemas.google.com/g/2005#work"
            });

            CSEntryChange x = CSEntryChange.Create();
            x.ObjectModificationType = ObjectModificationType.Replace;

            IList<AttributeChange> result = schemaItem.CreateAttributeChanges(x.DN, x.ObjectModificationType, e).ToList();

            AttributeChange change;

            change = result.FirstOrDefault(t => t.Name == "organizations_work_name");
            Assert.IsNotNull(change);
            Assert.AreEqual("myorg", change.GetValueAdd<string>());
            Assert.AreEqual(AttributeModificationType.Add, change.ModificationType);
            x.AttributeChanges.Add(change);

            change = result.FirstOrDefault(t => t.Name == "organizations_work_department");
            Assert.IsNotNull(change);
            Assert.AreEqual("department", change.GetValueAdd<string>());
            Assert.AreEqual(AttributeModificationType.Add, change.ModificationType);
            x.AttributeChanges.Add(change);

            change = result.FirstOrDefault(t => t.Name == "organizations_work_jobDescription");
            Assert.IsNotNull(change);
            Assert.AreEqual("jobdescription", change.GetValueAdd<string>());
            Assert.AreEqual(AttributeModificationType.Add, change.ModificationType);
            x.AttributeChanges.Add(change);

            change = result.FirstOrDefault(t => t.Name == "organizations_work_location");
            Assert.IsNotNull(change);
            Assert.AreEqual("location", change.GetValueAdd<string>());
            Assert.AreEqual(AttributeModificationType.Add, change.ModificationType);
            x.AttributeChanges.Add(change);

            change = result.FirstOrDefault(t => t.Name == "organizations_work_symbol");
            Assert.IsNotNull(change);
            Assert.AreEqual("symbol", change.GetValueAdd<string>());
            Assert.AreEqual(AttributeModificationType.Add, change.ModificationType);
            x.AttributeChanges.Add(change);

            change = result.FirstOrDefault(t => t.Name == "organizations_work_title");
            Assert.IsNotNull(change);
            Assert.AreEqual("title", change.GetValueAdd<string>());
            Assert.AreEqual(AttributeModificationType.Add, change.ModificationType);
            x.AttributeChanges.Add(change);
        }

        [TestMethod]
        public void TestToCSEntryChangeUpdate()
        {
            IAttributeAdapter schemaItem = UnitTestControl.Schema["contact"].Attributes.First(t => t.AttributeName == "organizations");

            ContactEntry e = new ContactEntry();

            e.Organizations.Add(new Organization()
            {
                Name = "myorg",
                Department = "department",
                JobDescription = "jobdescription",
                Location = "location",
                Symbol = "symbol",
                Title = "title",
                Rel = "http://schemas.google.com/g/2005#work"
            });

            CSEntryChange x = CSEntryChange.Create();
            x.ObjectModificationType = ObjectModificationType.Update;

            IList<AttributeChange> result = schemaItem.CreateAttributeChanges(x.DN, x.ObjectModificationType, e).ToList();

            AttributeChange change;

            change = result.FirstOrDefault(t => t.Name == "organizations_work_name");
            Assert.IsNotNull(change);
            Assert.AreEqual("myorg", change.GetValueAdd<string>());
            Assert.AreEqual(AttributeModificationType.Replace, change.ModificationType);
            x.AttributeChanges.Add(change);

            change = result.FirstOrDefault(t => t.Name == "organizations_work_department");
            Assert.IsNotNull(change);
            Assert.AreEqual("department", change.GetValueAdd<string>());
            Assert.AreEqual(AttributeModificationType.Replace, change.ModificationType);
            x.AttributeChanges.Add(change);

            change = result.FirstOrDefault(t => t.Name == "organizations_work_jobDescription");
            Assert.IsNotNull(change);
            Assert.AreEqual("jobdescription", change.GetValueAdd<string>());
            Assert.AreEqual(AttributeModificationType.Replace, change.ModificationType);
            x.AttributeChanges.Add(change);

            change = result.FirstOrDefault(t => t.Name == "organizations_work_location");
            Assert.IsNotNull(change);
            Assert.AreEqual("location", change.GetValueAdd<string>());
            Assert.AreEqual(AttributeModificationType.Replace, change.ModificationType);
            x.AttributeChanges.Add(change);

            change = result.FirstOrDefault(t => t.Name == "organizations_work_symbol");
            Assert.IsNotNull(change);
            Assert.AreEqual("symbol", change.GetValueAdd<string>());
            Assert.AreEqual(AttributeModificationType.Replace, change.ModificationType);
            x.AttributeChanges.Add(change);

            change = result.FirstOrDefault(t => t.Name == "organizations_work_title");
            Assert.IsNotNull(change);
            Assert.AreEqual("title", change.GetValueAdd<string>());
            Assert.AreEqual(AttributeModificationType.Replace, change.ModificationType);
            x.AttributeChanges.Add(change);
        }

        [TestMethod]
        public void TestFromCSEntryChangeAdd()
        {
            IAttributeAdapter schemaItem = UnitTestControl.Schema["contact"].Attributes.First(t => t.AttributeName == "organizations");

            ContactEntry e = new ContactEntry();

            CSEntryChange x = CSEntryChange.Create();
            x.ObjectModificationType = ObjectModificationType.Add;
            x.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("organizations_work_name", "myorg"));
            x.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("organizations_work_department", "department"));
            x.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("organizations_work_jobDescription", "jobdescription"));
            x.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("organizations_work_location", "location"));
            x.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("organizations_work_symbol", "symbol"));
            x.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("organizations_work_title", "title"));

            schemaItem.UpdateField(x, e);
            Organization o = e.Organizations.First(t => t.Rel == "http://schemas.google.com/g/2005#work");

            Assert.AreEqual("myorg", o.Name);
            Assert.AreEqual("department", o.Department);
            Assert.AreEqual("jobdescription", o.JobDescription);
            Assert.AreEqual("location", o.Location);
            Assert.AreEqual("symbol", o.Symbol);
            Assert.AreEqual("title", o.Title);
            Assert.AreEqual(true, o.Primary);
        }

        [TestMethod]
        public void TestFromCSEntryChangeUpdate()
        {
            IAttributeAdapter schemaItem = UnitTestControl.Schema["contact"].Attributes.First(t => t.AttributeName == "organizations");

            ContactEntry e = new ContactEntry();

            e.Organizations.Add(new Organization()
            {
                Name = "myorg-old",
                Department = "department-old",
                JobDescription = "jobdescription-old",
                Location = "location-old",
                Symbol = "symbol-old",
                Title = "title-old",
                Rel = "http://schemas.google.com/g/2005#work"
            });

            CSEntryChange x = CSEntryChange.Create();
            x.ObjectModificationType = ObjectModificationType.Update;
            x.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("organizations_work_name", "myorg"));
            x.AttributeChanges.Add(AttributeChange.CreateAttributeReplace("organizations_work_department", "department"));
            x.AttributeChanges.Add(AttributeChange.CreateAttributeReplace("organizations_work_jobDescription", "jobdescription"));
            x.AttributeChanges.Add(AttributeChange.CreateAttributeReplace("organizations_work_location", "location"));
            x.AttributeChanges.Add(AttributeChange.CreateAttributeReplace("organizations_work_symbol", "symbol"));
            x.AttributeChanges.Add(AttributeChange.CreateAttributeDelete("organizations_work_title"));

            schemaItem.UpdateField(x, e);
            Organization o = e.Organizations.First(t => t.Rel == "http://schemas.google.com/g/2005#work");

            Assert.AreEqual("myorg", o.Name);
            Assert.AreEqual("department", o.Department);
            Assert.AreEqual("jobdescription", o.JobDescription);
            Assert.AreEqual("location", o.Location);
            Assert.AreEqual("symbol", o.Symbol);
            Assert.AreEqual(null, o.Title);
            Assert.AreEqual(true, o.Primary);
        }
    }
}