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
    public class AdapterCustomTypeListTTests
    {
        [TestMethod]
        public void TestToCSEntryChangeAdd()
        {
            IAttributeAdapter schemaItem = UnitTestControl.Schema["user"].Attributes.First(t => t.FieldName == "websites");

            User u = new User
            {
                Websites = new List<Website>()
                {
                    new Website()
                    {
                        Primary = true,
                        Type = "work",
                        Value = "http://work.com"
                    },

                    new Website()
                    {
                        Primary = false,
                        Type = "home",
                        Value = "http://home.com"
                    }
                }
            };

            CSEntryChange x = CSEntryChange.Create();
            x.ObjectModificationType = ObjectModificationType.Add;
            IList<AttributeChange> result = schemaItem.CreateAttributeChanges(x.DN, x.ObjectModificationType, u).ToList();

            AttributeChange change = result.FirstOrDefault(t => t.Name == "websites_work");
            Assert.IsNotNull(change);
            Assert.AreEqual("http://work.com", change.GetValueAdd<string>());
            x.AttributeChanges.Add(change);

            change = result.FirstOrDefault(t => t.Name == "websites_home");
            Assert.IsNotNull(change);
            Assert.AreEqual("http://home.com", change.GetValueAdd<string>());
            x.AttributeChanges.Add(change);
        }

        [TestMethod]
        public void TestToCSEntryChangeReplace()
        {
            IAttributeAdapter schemaItem = UnitTestControl.Schema["user"].Attributes.First(t => t.FieldName == "websites");

            User u = new User
            {
                Websites = new List<Website>()
                {
                    new Website()
                    {
                        Primary = true,
                        Type = "work",
                        Value = "http://work.com"
                    },

                    new Website()
                    {
                        Primary = false,
                        Type = "home",
                        Value = "http://home.com"
                    }
                }
            };

            CSEntryChange x = CSEntryChange.Create();
            x.ObjectModificationType = ObjectModificationType.Replace;
            IList<AttributeChange> result = schemaItem.CreateAttributeChanges(x.DN, x.ObjectModificationType, u).ToList();

            AttributeChange change = result.FirstOrDefault(t => t.Name == "websites_work");
            Assert.IsNotNull(change);
            Assert.AreEqual("http://work.com", change.GetValueAdd<string>());
            Assert.AreEqual(AttributeModificationType.Add, change.ModificationType);
            x.AttributeChanges.Add(change);
       
            change = result.FirstOrDefault(t => t.Name == "websites_home");
            Assert.IsNotNull(change);
            Assert.AreEqual("http://home.com", change.GetValueAdd<string>());
            Assert.AreEqual(AttributeModificationType.Add, change.ModificationType);
            x.AttributeChanges.Add(change);
        }

        [TestMethod]
        public void TestToCSEntryChangeUpdate()
        {
            IAttributeAdapter schemaItem = UnitTestControl.Schema["user"].Attributes.First(t => t.FieldName == "websites");

            User u = new User
            {
                Websites = new List<Website>()
                {
                    new Website()
                    {
                        Primary = true,
                        Type = "work",
                        Value = "http://work.com"
                    },

                    new Website()
                    {
                        Primary = false,
                        Type = "home",
                        Value = "http://home.com"
                    }
                }
            };

            CSEntryChange x = CSEntryChange.Create();
            x.ObjectModificationType = ObjectModificationType.Update;
            IList<AttributeChange> result = schemaItem.CreateAttributeChanges(x.DN, x.ObjectModificationType, u).ToList();

            AttributeChange change = result.FirstOrDefault(t => t.Name == "websites_work");
           Assert.IsNotNull(change);
            Assert.AreEqual("http://work.com", change.GetValueAdd<string>());
            Assert.AreEqual(AttributeModificationType.Replace, change.ModificationType);
            x.AttributeChanges.Add(change);


            change = result.FirstOrDefault(t => t.Name == "websites_home");
            Assert.IsNotNull(change);
            Assert.AreEqual("http://home.com", change.GetValueAdd<string>());
            Assert.AreEqual(AttributeModificationType.Replace, change.ModificationType);
            x.AttributeChanges.Add(change);
        }
        
        [TestMethod]
        public void TestFromCSEntryChangeAdd()
        {
            IAttributeAdapter schemaItem = UnitTestControl.Schema["user"].Attributes.First(t => t.FieldName == "websites");
            CSEntryChange x = CSEntryChange.Create();
            x.ObjectModificationType = ObjectModificationType.Add;

            x.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("websites_home", "http://home.com"));
            x.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("websites_work", "http://work.com"));
            
            User ux = new User();
            schemaItem.UpdateField(x, ux);

            Assert.AreEqual("http://work.com", ux.Websites.First(t => t.Type == "work").Value);
            Assert.AreEqual(true, ux.Websites.First(t => t.Type == "work").IsPrimary);

            Assert.AreEqual("http://home.com", ux.Websites.First(t => t.Type == "home").Value);
            Assert.AreEqual(false, ux.Websites.First(t => t.Type == "home").IsPrimary);
        }

        [TestMethod]
        public void TestFromCSEntryChangeReplace()
        {
            IAttributeAdapter schemaItem = UnitTestControl.Schema["user"].Attributes.First(t => t.FieldName == "websites");
            CSEntryChange x = CSEntryChange.Create();
            x.ObjectModificationType = ObjectModificationType.Update;

            x.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("websites_home", "http://home.com"));
            x.AttributeChanges.Add(AttributeChange.CreateAttributeReplace("websites_work", "http://work.com"));

            User ux = new User();
            ux.Websites = new List<Website>() {new Website() {Primary = false, Type = "work", Value = "http://notwork.com"}};
            schemaItem.UpdateField(x, ux);

            Assert.AreEqual("http://work.com", ux.Websites.First(t => t.Type == "work").Value);
            Assert.AreEqual(true, ux.Websites.First(t => t.Type == "work").IsPrimary);

            Assert.AreEqual("http://home.com", ux.Websites.First(t => t.Type == "home").Value);
            Assert.AreEqual(false, ux.Websites.First(t => t.Type == "home").IsPrimary);
        }

        [TestMethod]
        public void TestFromCSEntryChangeUpdate()
        {
            IAttributeAdapter schemaItem = UnitTestControl.Schema["user"].Attributes.First(t => t.FieldName == "websites");
            CSEntryChange x = CSEntryChange.Create();
            x.ObjectModificationType = ObjectModificationType.Update;

            x.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("websites_home", "http://home.com"));
            x.AttributeChanges.Add(AttributeChange.CreateAttributeUpdate("websites_work", "http://work.com"));

            User ux = new User();
            ux.Websites = new List<Website>() { new Website() { Primary = false, Type = "work", Value = "http://notwork.com" } };
            schemaItem.UpdateField(x, ux);

            Assert.AreEqual("http://work.com", ux.Websites.First(t => t.Type == "work").Value);
            Assert.AreEqual(true, ux.Websites.First(t => t.Type == "work").IsPrimary);

            Assert.AreEqual("http://home.com", ux.Websites.First(t => t.Type == "home").Value);
            Assert.AreEqual(false, ux.Websites.First(t => t.Type == "home").IsPrimary);
        }
    }
}
