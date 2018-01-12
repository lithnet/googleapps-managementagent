using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Admin.Directory.directory_v1.Data;
using Lithnet.GoogleApps.ManagedObjects;
using Lithnet.MetadirectoryServices;
using Microsoft.MetadirectoryServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lithnet.GoogleApps.MA.UnitTests
{
    [TestClass]
    public class CalendarTests
    {
        [TestMethod]
        public void GetCalendars()
        {
            foreach (var item in ResourceRequestFactory.GetCalendars("my_customer"))
            {
                Trace.WriteLine(item.ResourceName);
            }
        }

        [TestMethod]
        public void GetCalendarsViaApiInterface()
        {
            var ff = UnitTestControl.Schema[SchemaConstants.Calendar];
            var s = UnitTestControl.MmsSchema.Types[SchemaConstants.Calendar];

            ApiInterfaceCalendar u = new ApiInterfaceCalendar("my_customer", ff);

            u.GetItems(UnitTestControl.TestParameters, UnitTestControl.MmsSchema, new BlockingCollection<object>()).Wait();
        }

        [TestMethod]
        public void CreateCalendar()
        {
            CSEntryChange cs = CSEntryChange.Create();
            cs.ObjectModificationType = ObjectModificationType.Add;
            cs.DN = Guid.NewGuid().ToString("n");
            cs.ObjectType = SchemaConstants.Calendar;

            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("resourceName", "name"));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("buildingId", "AU203"));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("capacity", 33L));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("floorName", "G"));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("floorSection", "33B"));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("resourceCategory", "CONFERENCE_ROOM"));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("resourceDescription", "internal description"));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("userVisibleDescription", "user description"));

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
                Assert.AreEqual(cs.DN, c.ResourceId);
                Assert.AreEqual("name", c.ResourceName);
                Assert.AreEqual("AU203", c.BuildingId);
                Assert.AreEqual(33, c.Capacity);
                Assert.AreEqual("G", c.FloorName);
                Assert.AreEqual("33B", c.FloorSection);
                Assert.AreEqual("CONFERENCE_ROOM", c.ResourceCategory);
                Assert.AreEqual("internal description", c.ResourceDescription);
                Assert.AreEqual("user description", c.UserVisibleDescription);
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
            calendar.ResourceName = calendar.ResourceId;
            calendar.BuildingId = "AU205";
            calendar.Capacity = 9;
            calendar.FloorName = "G";
            calendar.FloorSection = "39b";

            calendar.ResourceCategory = "OTHER";
            calendar.ResourceDescription = "internal description 1";
            calendar.UserVisibleDescription = "my description 2";

            ResourceRequestFactory.AddCalendar(UnitTestControl.TestParameters.CustomerID, calendar);

            CSEntryChange cs = CSEntryChange.Create();
            cs.ObjectModificationType = ObjectModificationType.Update;
            cs.DN = calendar.ResourceId;
            cs.ObjectType = SchemaConstants.Calendar;
            cs.AnchorAttributes.Add(AnchorAttribute.Create("id", calendar.ResourceId));

            cs.AttributeChanges.Add(AttributeChange.CreateAttributeReplace("resourceName", "name"));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeUpdate("buildingId", new List<ValueChange>() { ValueChange.CreateValueAdd("AU203") }));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeReplace("capacity", 33L));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeReplace("floorName", "G"));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeReplace("floorSection", "33B"));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeReplace("resourceCategory", "CONFERENCE_ROOM"));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeReplace("resourceDescription", "internal description"));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeReplace("userVisibleDescription", "user description"));

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
                Assert.AreEqual(cs.DN, c.ResourceId);
                Assert.AreEqual("name", c.ResourceName);
                Assert.AreEqual("AU203", c.BuildingId);
                Assert.AreEqual(33, c.Capacity);
                Assert.AreEqual("G", c.FloorName);
                Assert.AreEqual("33B", c.FloorSection);
                Assert.AreEqual("CONFERENCE_ROOM", c.ResourceCategory);
                Assert.AreEqual("internal description", c.ResourceDescription);
                Assert.AreEqual("user description", c.UserVisibleDescription);
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
            calendar.ResourceName = calendar.ResourceId;
            calendar.BuildingId = "AU205";
            calendar.Capacity = 9;
            calendar.FloorName = "G";
            calendar.FloorSection = "39b";

            calendar.ResourceCategory = "OTHER";
            calendar.ResourceDescription = "internal description 1";
            calendar.UserVisibleDescription = "my description 2";

            ResourceRequestFactory.AddCalendar(UnitTestControl.TestParameters.CustomerID, calendar);

            CSEntryChange cs = CSEntryChange.Create();
            cs.ObjectModificationType = ObjectModificationType.Update;
            cs.DN = calendar.ResourceId;
            cs.ObjectType = SchemaConstants.Calendar;
            cs.AnchorAttributes.Add(AnchorAttribute.Create("id", calendar.ResourceId));

            cs.AttributeChanges.Add(AttributeChange.CreateAttributeDelete("buildingId"));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeDelete("capacity"));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeDelete("floorName"));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeDelete("floorSection"));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeDelete("resourceDescription"));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeDelete("userVisibleDescription"));

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
                Assert.AreEqual(cs.DN, c.ResourceId);
                Assert.IsNull(c.BuildingId);
                Assert.IsNull(c.Capacity);
                Assert.IsNull(c.FloorName);
                Assert.IsNull(c.FloorSection);
                Assert.IsNull(c.ResourceDescription);
                Assert.IsNull(c.UserVisibleDescription);
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
