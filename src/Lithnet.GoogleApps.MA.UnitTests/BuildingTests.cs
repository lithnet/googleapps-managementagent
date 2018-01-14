using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Google.Apis.Admin.Directory.directory_v1.Data;
using Lithnet.MetadirectoryServices;
using Microsoft.MetadirectoryServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lithnet.GoogleApps.MA.UnitTests
{
    [TestClass]
    public class BuildingTests
    {
        [TestMethod]
        public void GetBuildings()
        {
            foreach (Building item in ResourceRequestFactory.GetBuildings("my_customer"))
            {
                {
                    Trace.WriteLine(item.BuildingName);
                }
            }
        }

        [TestMethod]
        public void GetBuildingsViaApiInterface()
        {
            MASchemaType s = UnitTestControl.Schema[SchemaConstants.Building];

            ApiInterfaceBuilding u = new ApiInterfaceBuilding("my_customer", s);

            BlockingCollection<object> items = new BlockingCollection<object>();

            u.GetItems(UnitTestControl.TestParameters, UnitTestControl.MmsSchema, items).Wait();

            foreach (CSEntryChange item in items.OfType<CSEntryChange>())
            {
                Assert.AreEqual(MAImportError.Success, item.ErrorCodeImport);
            }

            Assert.AreNotEqual(0, items.Count);
        }

        [TestMethod]
        public void CreateBuilding()
        {
            CSEntryChange cs = CSEntryChange.Create();
            cs.ObjectModificationType = ObjectModificationType.Add;
            cs.DN = "new-building@building.resource";
            cs.ObjectType = SchemaConstants.Building;

            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("buildingName", "My building"));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("description", "my description"));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("coordinates_latitude", "1"));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("coordinates_longitude", "-99"));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("floorNames", "B2,B1,G,1,2,3,4,5"));

            string id = null;

            try
            {
                CSEntryChangeResult result = ExportProcessor.PutCSEntryChange(cs, UnitTestControl.Schema.GetSchema().Types[SchemaConstants.Building], UnitTestControl.TestParameters);

                if (result.ErrorCode != MAExportError.Success)
                {
                    Assert.Fail(result.ErrorName);
                }

                id = result.AnchorAttributes["id"].GetValueAdd<string>();

                Thread.Sleep(UnitTestControl.PostGoogleOperationSleepInterval);

                Building c = ResourceRequestFactory.GetBuilding(UnitTestControl.TestParameters.CustomerID, id);
                Assert.AreEqual("new-building", c.BuildingId);
                Assert.AreEqual("My building", c.BuildingName);
                Assert.AreEqual("my description", c.Description);
                Assert.AreEqual(1D, c.Coordinates?.Latitude);
                Assert.AreEqual(-99D, c.Coordinates?.Longitude);
                CollectionAssert.AreEqual(new string[] { "B2", "B1", "G", "1", "2", "3", "4", "5" }, c.FloorNames.ToArray());
            }
            finally
            {
                if (id != null)
                {
                    ResourceRequestFactory.DeleteBuilding(UnitTestControl.TestParameters.CustomerID, id);
                }
            }
        }

        [TestMethod]
        public void UpdateBuilding()
        {
            Building building = new Building();
            building.BuildingId = "test-building";
            building.BuildingName = "My building";
            building.Description = "some description";
            building.FloorNames = new List<string>() { "B1", "B2", "G" };
            building.Coordinates = new BuildingCoordinates() { Latitude = -66, Longitude = 44 };

            ResourceRequestFactory.AddBuilding(UnitTestControl.TestParameters.CustomerID, building);

            CSEntryChange cs = CSEntryChange.Create();
            cs.ObjectModificationType = ObjectModificationType.Update;
            cs.DN = $"{building.BuildingId}{ApiInterfaceBuilding.DNSuffix}";
            cs.ObjectType = SchemaConstants.Building;
            cs.AnchorAttributes.Add(AnchorAttribute.Create("id", building.BuildingId));

            cs.AttributeChanges.Add(AttributeChange.CreateAttributeReplace("buildingName", "new name"));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeReplace("description", "new description"));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeReplace("floorNames", "G,1,2"));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeReplace("coordinates_latitude", "11"));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeReplace("coordinates_longitude", "-22"));

            string id = building.BuildingId;

            try
            {
                CSEntryChangeResult result = ExportProcessor.PutCSEntryChange(cs, UnitTestControl.Schema.GetSchema().Types[SchemaConstants.Building], UnitTestControl.TestParameters);

                if (result.ErrorCode != MAExportError.Success)
                {
                    Assert.Fail(result.ErrorName);
                }

                Thread.Sleep(UnitTestControl.PostGoogleOperationSleepInterval);

                Building c = ResourceRequestFactory.GetBuilding(UnitTestControl.TestParameters.CustomerID, id);
                Assert.AreEqual("test-building", c.BuildingId);
                Assert.AreEqual("new name", c.BuildingName);
                Assert.AreEqual("new description", c.Description);

                CollectionAssert.AreEqual(new string[] { "G", "1", "2" }, c.FloorNames.ToArray());
                Assert.AreEqual(11D, c.Coordinates?.Latitude);
                Assert.AreEqual(-22D, c.Coordinates?.Longitude);
            }
            finally
            {
                if (id != null)
                {
                    ResourceRequestFactory.DeleteBuilding(UnitTestControl.TestParameters.CustomerID, id);
                }
            }
        }

        [TestMethod]
        public void UpdateBuildingClearValues()
        {
            Building building = new Building();
            building.BuildingId = "test-building";
            building.BuildingName = "My building";
            building.Description = "some description";
            building.FloorNames = new List<string>() { "B1", "B2", "G" };
            building.Coordinates = new BuildingCoordinates() { Latitude = -66, Longitude = 44 };


            ResourceRequestFactory.AddBuilding(UnitTestControl.TestParameters.CustomerID, building);

            CSEntryChange cs = CSEntryChange.Create();
            cs.ObjectModificationType = ObjectModificationType.Update;
            cs.DN = $"{building.BuildingId}{ApiInterfaceBuilding.DNSuffix}";
            cs.ObjectType = SchemaConstants.Building;
            cs.AnchorAttributes.Add(AnchorAttribute.Create("id", building.BuildingId));
            
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeDelete("description"));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeDelete("coordinates_latitude"));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeDelete("coordinates_longitude"));

            string id = building.BuildingId;

            try
            {
                CSEntryChangeResult result = ExportProcessor.PutCSEntryChange(cs, UnitTestControl.Schema.GetSchema().Types[SchemaConstants.Building], UnitTestControl.TestParameters);

                if (result.ErrorCode != MAExportError.Success)
                {
                    Assert.Fail(result.ErrorName);
                }

                Thread.Sleep(UnitTestControl.PostGoogleOperationSleepInterval);

                Building c = ResourceRequestFactory.GetBuilding(UnitTestControl.TestParameters.CustomerID, id);
                Assert.AreEqual("test-building", c.BuildingId);
                Assert.IsTrue(string.IsNullOrEmpty(c.Description));
                Assert.IsNull(c.Coordinates?.Longitude);
                Assert.IsNull(c.Coordinates?.Latitude);
            }
            finally
            {
                if (id != null)
                {
                    ResourceRequestFactory.DeleteBuilding(UnitTestControl.TestParameters.CustomerID, id);
                }
            }
        }
    }
}