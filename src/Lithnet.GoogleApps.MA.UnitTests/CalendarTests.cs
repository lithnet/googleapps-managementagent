using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Google;
using Google.Apis.Admin.Directory.directory_v1.Data;
using Lithnet.GoogleApps.ManagedObjects;
using Lithnet.MetadirectoryServices;
using Microsoft.MetadirectoryServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace Lithnet.GoogleApps.MA.UnitTests
{
    [TestClass]
    public class CalendarTests
    {
        [TestMethod]
        public void GetCalendarAclList()
        {
            foreach (var calendar in ResourceRequestFactory.GetCalendars("my_customer"))
            {
                try
                {
                    foreach (var acl in ResourceRequestFactory.GetCalendarAclRules("my_customer", calendar.ResourceEmail))
                    {
                        Trace.WriteLine($"Calendar {calendar.ResourceName} ACL {acl.Id}/{acl.Role}/{acl.Scope}");
                    }
                }
                catch (GoogleApiException ex)
                {
                    if (ex.HttpStatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        Trace.WriteLine($"Calendar {calendar.ResourceName} ACL not found");
                    }
                    else
                    {
                        throw;
                    }
                }
            }
        }

        [TestMethod]
        public void GetCalendarsViaApiInterface()
        {
            MASchemaType maSchemaType = UnitTestControl.Schema[SchemaConstants.Calendar];

            ApiInterfaceCalendar u = new ApiInterfaceCalendar("my_customer", maSchemaType);

            BlockingCollection<object> items = new BlockingCollection<object>();

            u.GetItems(UnitTestControl.TestParameters, UnitTestControl.MmsSchema, items).Wait();
            HashSet<string> dns = new HashSet<string>();

            foreach (CSEntryChange item in items.OfType<CSEntryChange>())
            {
                Assert.AreEqual(MAImportError.Success, item.ErrorCodeImport);
                Assert.IsTrue(dns.Add(item.DN));
            }

            Assert.AreNotEqual(0, items.Count);
        }

        [TestMethod]
        public void CreateCalendar()
        {
            CSEntryChange cs = CSEntryChange.Create();
            cs.ObjectModificationType = ObjectModificationType.Add;
            cs.DN = "test-name@calendar.resource";
            cs.ObjectType = SchemaConstants.Calendar;

            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("buildingId", "AU203"));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("capacity", 33L));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("floorName", "G"));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("floorSection", "33B"));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("resourceCategory", "CONFERENCE_ROOM"));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("resourceDescription", "internal description"));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("userVisibleDescription", "user description"));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("features", new List<object> { "Test1", "Test2" }));

            string id = null;

            try
            {
                CSEntryChangeResult result = ExportProcessor.PutCSEntryChange(cs, UnitTestControl.Schema.GetSchema().Types[SchemaConstants.Calendar], UnitTestControl.TestParameters);

                if (result.ErrorCode != MAExportError.Success)
                {
                    Assert.Fail(result.ErrorName);
                }

                id = result.AnchorAttributes["id"].GetValueAdd<string>();

                Thread.Sleep(UnitTestControl.PostGoogleOperationSleepInterval);

                CalendarResource c = ResourceRequestFactory.GetCalendar(UnitTestControl.TestParameters.CustomerID, id);
                Assert.AreEqual(cs.DN, "test-name@calendar.resource");
                Assert.AreEqual("test-name", c.ResourceName);
                Assert.AreEqual("AU203", c.BuildingId);
                Assert.AreEqual(33, c.Capacity);
                Assert.AreEqual("G", c.FloorName);
                Assert.AreEqual("33B", c.FloorSection);
                Assert.AreEqual("CONFERENCE_ROOM", c.ResourceCategory);
                Assert.AreEqual("internal description", c.ResourceDescription);
                Assert.AreEqual("user description", c.UserVisibleDescription);
                CollectionAssert.AreEquivalent(new string[] { "Test1", "Test2" }, ApiInterfaceCalendar.GetFeatureNames(c).ToList());
            }
            finally
            {
                if (id != null)
                {
                    ResourceRequestFactory.DeleteCalendar(UnitTestControl.TestParameters.CustomerID, id);
                }
            }
        }

        [TestMethod]
        public void UpdateCalendar()
        {
            CalendarResource calendar = new CalendarResource();
            calendar.ResourceId = Guid.NewGuid().ToString("n");
            calendar.ResourceName = "test-name";
            calendar.BuildingId = "AU205";
            calendar.Capacity = 9;
            calendar.FloorName = "G";
            calendar.FloorSection = "39b";
            calendar.ResourceCategory = "OTHER";
            calendar.ResourceDescription = "internal description 1";
            calendar.UserVisibleDescription = "my description 2";
            calendar.FeatureInstances = new List<FeatureInstance>()
            {
                new FeatureInstance() {Feature = new Feature() {Name = "Test1"}},
                new FeatureInstance() {Feature = new Feature() {Name = "Test2"}},
            };


            ResourceRequestFactory.AddCalendar(UnitTestControl.TestParameters.CustomerID, calendar);

            CSEntryChange cs = CSEntryChange.Create();
            cs.ObjectModificationType = ObjectModificationType.Update;
            cs.DN = "test-name@calendar.resource";
            cs.ObjectType = SchemaConstants.Calendar;
            cs.AnchorAttributes.Add(AnchorAttribute.Create("id", calendar.ResourceId));

            cs.AttributeChanges.Add(AttributeChange.CreateAttributeUpdate("buildingId", new List<ValueChange>() { ValueChange.CreateValueAdd("AU203") }));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeReplace("capacity", 33L));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeReplace("floorName", "G"));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeReplace("floorSection", "33B"));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeReplace("resourceCategory", "CONFERENCE_ROOM"));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeReplace("resourceDescription", "internal description"));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeReplace("userVisibleDescription", "user description"));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeUpdate("features", new List<ValueChange>()
            {
                ValueChange.CreateValueAdd("Test3"),
                ValueChange.CreateValueDelete("Test1"),
            }));

            string id = calendar.ResourceId;

            try
            {
                CSEntryChangeResult result = ExportProcessor.PutCSEntryChange(cs, UnitTestControl.Schema.GetSchema().Types[SchemaConstants.Calendar], UnitTestControl.TestParameters);

                if (result.ErrorCode != MAExportError.Success)
                {
                    Assert.Fail(result.ErrorName);
                }

                Thread.Sleep(UnitTestControl.PostGoogleOperationSleepInterval);

                CalendarResource c = ResourceRequestFactory.GetCalendar(UnitTestControl.TestParameters.CustomerID, id);
                Assert.AreEqual(cs.DN, "test-name@calendar.resource");
                Assert.AreEqual("test-name", c.ResourceName);
                Assert.AreEqual("AU203", c.BuildingId);
                Assert.AreEqual(33, c.Capacity);
                Assert.AreEqual("G", c.FloorName);
                Assert.AreEqual("33B", c.FloorSection);
                Assert.AreEqual("CONFERENCE_ROOM", c.ResourceCategory);
                Assert.AreEqual("internal description", c.ResourceDescription);
                Assert.AreEqual("user description", c.UserVisibleDescription);
                CollectionAssert.AreEquivalent(new string[] { "Test2", "Test3" }, ApiInterfaceCalendar.GetFeatureNames(c).ToList());
            }
            finally
            {
                if (id != null)
                {
                    ResourceRequestFactory.DeleteCalendar(UnitTestControl.TestParameters.CustomerID, id);
                }
            }
        }

        [TestMethod]
        public void UpdateCalendarClearValues()
        {
            CalendarResource calendar = new CalendarResource();
            calendar.ResourceId = Guid.NewGuid().ToString("n");
            calendar.ResourceName = "test-name";
            calendar.BuildingId = "AU205";
            calendar.Capacity = 9;
            calendar.FloorName = "G";
            calendar.FloorSection = "39b";
            calendar.ResourceCategory = "OTHER";
            calendar.ResourceDescription = "internal description 1";
            calendar.UserVisibleDescription = "my description 2";
            calendar.FeatureInstances = new List<FeatureInstance>()
            {
                new FeatureInstance() {Feature = new Feature() {Name = "Test1"}},
                new FeatureInstance() {Feature = new Feature() {Name = "Test2"}},
            };

            ResourceRequestFactory.AddCalendar(UnitTestControl.TestParameters.CustomerID, calendar);

            CSEntryChange cs = CSEntryChange.Create();
            cs.ObjectModificationType = ObjectModificationType.Update;
            cs.DN = "test-name@calendar.resource";
            cs.ObjectType = SchemaConstants.Calendar;
            cs.AnchorAttributes.Add(AnchorAttribute.Create("id", calendar.ResourceId));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeDelete("buildingId"));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeDelete("capacity"));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeDelete("floorName"));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeDelete("floorSection"));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeDelete("resourceDescription"));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeDelete("userVisibleDescription"));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeDelete("features"));

            string id = calendar.ResourceId;

            try
            {
                CSEntryChangeResult result = ExportProcessor.PutCSEntryChange(cs, UnitTestControl.Schema.GetSchema().Types[SchemaConstants.Calendar], UnitTestControl.TestParameters);

                if (result.ErrorCode != MAExportError.Success)
                {
                    Assert.Fail(result.ErrorName);
                }

                Thread.Sleep(UnitTestControl.PostGoogleOperationSleepInterval);

                CalendarResource c = ResourceRequestFactory.GetCalendar(UnitTestControl.TestParameters.CustomerID, id);
                Assert.AreEqual(cs.DN, "test-name@calendar.resource");
                Assert.IsNull(c.BuildingId);
                Assert.IsNull(c.Capacity);
                Assert.IsNull(c.FloorName);
                Assert.IsNull(c.FloorSection);
                Assert.IsNull(c.ResourceDescription);
                Assert.IsNull(c.UserVisibleDescription);
                Assert.IsNull(c.FeatureInstances);
            }
            finally
            {
                if (id != null)
                {
                    ResourceRequestFactory.DeleteCalendar(UnitTestControl.TestParameters.CustomerID, id);
                }
            }
        }
    }
}
