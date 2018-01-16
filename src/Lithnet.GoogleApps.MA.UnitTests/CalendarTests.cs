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
using Google.Apis.Calendar.v3.Data;
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
        //[TestMethod]
        //public void GetCalendarAclList()
        //{
        //    foreach (var calendar in ResourceRequestFactory.GetCalendars("my_customer"))
        //    {
        //        try
        //        {
        //            foreach (var acl in ResourceRequestFactory.GetCalendarAclRules("my_customer", calendar.ResourceEmail))
        //            {
        //                Trace.WriteLine($"Calendar {calendar.ResourceName} ACL {acl.Id}/{acl.Role}/{acl.Scope}");
        //            }
        //        }
        //        catch (GoogleApiException ex)
        //        {
        //            if (ex.HttpStatusCode == System.Net.HttpStatusCode.NotFound)
        //            {
        //                Trace.WriteLine($"Calendar {calendar.ResourceName} ACL not found");
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //    }
        //}

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
                //Assert.AreEqual(MAImportError.Success, item.ErrorCodeImport);
                //Assert.IsTrue(dns.Add(item.DN));
                foreach (AttributeChange c in item.AttributeChanges)
                {
                    foreach (ValueChange v in c.ValueChanges)
                    {
                        Type t = v.Value.GetType();

                        if (t != typeof(string) && t != typeof(long))
                        {
                            Debug.WriteLine($"{c.Name} - {v.Value.GetType()}");
                        }
                    }
                }
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

            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("id", "test-calendar-id"));
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
                Assert.IsNotNull(c.ResourceEmail);
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

        private string CreateAddress(string prefix)
        {
            return $"{prefix}@{UnitTestControl.TestParameters.Domain}";
        }

        [TestMethod]
        public void CreateCalendarWithAcls()
        {
            CSEntryChange cs = CSEntryChange.Create();
            cs.ObjectModificationType = ObjectModificationType.Add;
            cs.DN = "test-name@calendar.resource";
            cs.ObjectType = SchemaConstants.Calendar;

            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("id", "test-calendar-id"));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("buildingId", "AU203"));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("capacity", 33L));

            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("owners", new List<object> {this.CreateAddress("owner1"), this.CreateAddress("owner2") }));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("readers", this.CreateAddress("reader")));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("writers", this.CreateAddress("writer")));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("freeBusyReaders", this.CreateAddress("freebusyreader")));

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

                List<AclRule> acls = ResourceRequestFactory.GetCalendarAclRules(UnitTestControl.TestParameters.CustomerID, c.ResourceEmail).ToList();

                Assert.IsNotNull(acls.FirstOrDefault(t => t.Role == "owner" && t.Scope.Value == this.CreateAddress("owner1")));
                Assert.IsNotNull(acls.FirstOrDefault(t => t.Role == "owner" && t.Scope.Value == this.CreateAddress("owner2")));
                Assert.IsNotNull(acls.FirstOrDefault(t => t.Role == "reader" && t.Scope.Value == this.CreateAddress("reader")));
                Assert.IsNotNull(acls.FirstOrDefault(t => t.Role == "writer" && t.Scope.Value == this.CreateAddress("writer")));
                Assert.IsNotNull(acls.FirstOrDefault(t => t.Role == "freeBusyReader" && t.Scope.Value == this.CreateAddress("freebusyreader")));
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
        public void UpdateCalendarWithAcls()
        {
            CSEntryChange cs = CSEntryChange.Create();
            cs.ObjectModificationType = ObjectModificationType.Add;
            cs.DN = "test-name@calendar.resource";
            cs.ObjectType = SchemaConstants.Calendar;

            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("id", "test-calendar-id"));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("buildingId", "AU203"));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("capacity", 33L));

            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("owners", new List<object> {this.CreateAddress("owner1"), this.CreateAddress("owner2") }));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("readers", new List<object> {this.CreateAddress("reader1"), this.CreateAddress("reader2") }));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("writers", new List<object> {this.CreateAddress("writer1"), this.CreateAddress("writer2") }));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("freeBusyReaders", this.CreateAddress("freebusyreader")));

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

                List<AclRule> acls = ResourceRequestFactory.GetCalendarAclRules(UnitTestControl.TestParameters.CustomerID, c.ResourceEmail).ToList();

                Assert.IsNotNull(acls.FirstOrDefault(t => t.Role == "owner" && t.Scope.Value == this.CreateAddress("owner1")));
                Assert.IsNotNull(acls.FirstOrDefault(t => t.Role == "owner" && t.Scope.Value == this.CreateAddress("owner2")));
                Assert.IsNotNull(acls.FirstOrDefault(t => t.Role == "reader" && t.Scope.Value == this.CreateAddress("reader1")));
                Assert.IsNotNull(acls.FirstOrDefault(t => t.Role == "reader" && t.Scope.Value == this.CreateAddress("reader2")));
                Assert.IsNotNull(acls.FirstOrDefault(t => t.Role == "writer" && t.Scope.Value == this.CreateAddress("writer1")));
                Assert.IsNotNull(acls.FirstOrDefault(t => t.Role == "writer" && t.Scope.Value == this.CreateAddress("writer2")));
                Assert.IsNotNull(acls.FirstOrDefault(t => t.Role == "freeBusyReader" && t.Scope.Value == this.CreateAddress("freebusyreader")));


                cs = CSEntryChange.Create();
                cs.ObjectModificationType = ObjectModificationType.Update;
                cs.DN = "test-name@calendar.resource";
                cs.ObjectType = SchemaConstants.Calendar;

                cs.AnchorAttributes.Add(AnchorAttribute.Create("id", id));
                cs.AnchorAttributes.Add(AnchorAttribute.Create("resourceEmail", result.AnchorAttributes["resourceEmail"].GetValueAdd<string>()));

                cs.AttributeChanges.Add(AttributeChange.CreateAttributeReplace("owners", new List<object> {this.CreateAddress("owner3"), this.CreateAddress("owner4") }));
                cs.AttributeChanges.Add(AttributeChange.CreateAttributeUpdate("readers", new List<ValueChange> { ValueChange.CreateValueAdd(this.CreateAddress("reader3")) }));
                cs.AttributeChanges.Add(AttributeChange.CreateAttributeUpdate("writers", new List<ValueChange> { ValueChange.CreateValueDelete(this.CreateAddress("writer1")) }));
                cs.AttributeChanges.Add(AttributeChange.CreateAttributeDelete("freeBusyReaders"));

                result = ExportProcessor.PutCSEntryChange(cs, UnitTestControl.Schema.GetSchema().Types[SchemaConstants.Calendar], UnitTestControl.TestParameters);

                if (result.ErrorCode != MAExportError.Success)
                {
                    Assert.Fail(result.ErrorName);
                }

                Thread.Sleep(UnitTestControl.PostGoogleOperationSleepInterval);

                acls = ResourceRequestFactory.GetCalendarAclRules(UnitTestControl.TestParameters.CustomerID, c.ResourceEmail).ToList();
                
                Assert.IsNull(acls.FirstOrDefault(t => t.Role == "owner" && t.Scope.Value == this.CreateAddress("owner1")));
                Assert.IsNull(acls.FirstOrDefault(t => t.Role == "owner" && t.Scope.Value == this.CreateAddress("owner2")));
                Assert.IsNotNull(acls.FirstOrDefault(t => t.Role == "owner" && t.Scope.Value == this.CreateAddress("owner3")));
                Assert.IsNotNull(acls.FirstOrDefault(t => t.Role == "owner" && t.Scope.Value == this.CreateAddress("owner4")));
                Assert.IsNotNull(acls.FirstOrDefault(t => t.Role == "reader" && t.Scope.Value == this.CreateAddress("reader1")));
                Assert.IsNotNull(acls.FirstOrDefault(t => t.Role == "reader" && t.Scope.Value == this.CreateAddress("reader2")));
                Assert.IsNotNull(acls.FirstOrDefault(t => t.Role == "reader" && t.Scope.Value == this.CreateAddress("reader3")));
                Assert.IsNull(acls.FirstOrDefault(t => t.Role == "owner" && t.Scope.Value == this.CreateAddress("writer1")));
                Assert.IsNotNull(acls.FirstOrDefault(t => t.Role == "writer" && t.Scope.Value == this.CreateAddress("writer2")));
                Assert.IsNull(acls.FirstOrDefault(t => t.Role == "freeBusyReader" && t.Scope.Value == this.CreateAddress("freebusyreader")));

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
        public void UpdateCalendarClearAcls()
        {
            CSEntryChange cs = CSEntryChange.Create();
            cs.ObjectModificationType = ObjectModificationType.Add;
            cs.DN = "test-name@calendar.resource";
            cs.ObjectType = SchemaConstants.Calendar;

            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("id", "test-calendar-id"));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("buildingId", "AU203"));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("capacity", 33L));

            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("owners", new List<object> { this.CreateAddress("owner1"), this.CreateAddress("owner2") }));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("readers", new List<object> { this.CreateAddress("reader1"), this.CreateAddress("reader2") }));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("writers", new List<object> { this.CreateAddress("writer1"), this.CreateAddress("writer2") }));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("freeBusyReaders", this.CreateAddress("freebusyreader")));

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

                List<AclRule> acls = ResourceRequestFactory.GetCalendarAclRules(UnitTestControl.TestParameters.CustomerID, c.ResourceEmail).ToList();

                Assert.IsNotNull(acls.FirstOrDefault(t => t.Role == "owner" && t.Scope.Value == this.CreateAddress("owner1")));
                Assert.IsNotNull(acls.FirstOrDefault(t => t.Role == "owner" && t.Scope.Value == this.CreateAddress("owner2")));
                Assert.IsNotNull(acls.FirstOrDefault(t => t.Role == "reader" && t.Scope.Value == this.CreateAddress("reader1")));
                Assert.IsNotNull(acls.FirstOrDefault(t => t.Role == "reader" && t.Scope.Value == this.CreateAddress("reader2")));
                Assert.IsNotNull(acls.FirstOrDefault(t => t.Role == "writer" && t.Scope.Value == this.CreateAddress("writer1")));
                Assert.IsNotNull(acls.FirstOrDefault(t => t.Role == "writer" && t.Scope.Value == this.CreateAddress("writer2")));
                Assert.IsNotNull(acls.FirstOrDefault(t => t.Role == "freeBusyReader" && t.Scope.Value == this.CreateAddress("freebusyreader")));

                cs = CSEntryChange.Create();
                cs.ObjectModificationType = ObjectModificationType.Update;
                cs.DN = "test-name@calendar.resource";
                cs.ObjectType = SchemaConstants.Calendar;

                cs.AnchorAttributes.Add(AnchorAttribute.Create("id", id));
                cs.AnchorAttributes.Add(AnchorAttribute.Create("resourceEmail", result.AnchorAttributes["resourceEmail"].GetValueAdd<string>()));

                cs.AttributeChanges.Add(AttributeChange.CreateAttributeDelete("owners"));
                cs.AttributeChanges.Add(AttributeChange.CreateAttributeDelete("readers"));
                cs.AttributeChanges.Add(AttributeChange.CreateAttributeDelete("writers"));
                cs.AttributeChanges.Add(AttributeChange.CreateAttributeDelete("freeBusyReaders"));

                result = ExportProcessor.PutCSEntryChange(cs, UnitTestControl.Schema.GetSchema().Types[SchemaConstants.Calendar], UnitTestControl.TestParameters);

                if (result.ErrorCode != MAExportError.Success)
                {
                    Assert.Fail(result.ErrorName);
                }

                Thread.Sleep(UnitTestControl.PostGoogleOperationSleepInterval);

                acls = ResourceRequestFactory.GetCalendarAclRules(UnitTestControl.TestParameters.CustomerID, c.ResourceEmail).ToList();

                Assert.IsNull(acls.FirstOrDefault(t => t.Role == "owner" && t.Scope.Value != c.ResourceEmail));
                Assert.IsNull(acls.FirstOrDefault(t => t.Role == "freeBusyReader" && t.Scope.Type != "domain"));
                Assert.IsNull(acls.FirstOrDefault(t => t.Role == "reader"));
                Assert.IsNull(acls.FirstOrDefault(t => t.Role == "writer"));
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
