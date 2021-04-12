using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Google;
using Google.Apis.Admin.Directory.directory_v1.Data;
using Lithnet.MetadirectoryServices;
using Microsoft.MetadirectoryServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lithnet.GoogleApps.MA.UnitTests
{
    [TestClass]
    public class OrgUnitTests
    {
        [TestMethod]
        public void GetOrgUnits()
        {
            var items = UnitTestControl.TestParameters.OrgUnitsService.List("my_customer").OrganizationUnits;

            foreach (OrgUnit item in items)
            {
                {
                    Trace.WriteLine(item);
                }
            }
        }

        [TestMethod]
        public void GetOrgUnitsViaApiInterface()
        {
            MASchemaType s = UnitTestControl.Schema[SchemaConstants.OrgUnit];

            ApiInterfaceOrgUnit u = new ApiInterfaceOrgUnit("my_customer", s, UnitTestControl.TestParameters);

            BlockingCollection<object> items = new BlockingCollection<object>();

            u.GetObjectImportTask(UnitTestControl.MmsSchema, items, CancellationToken.None).Wait();

            foreach (CSEntryChange item in items.OfType<CSEntryChange>())
            {
                Assert.AreEqual(MAImportError.Success, item.ErrorCodeImport);
            }

            Assert.AreNotEqual(0, items.Count);
        }

        [TestMethod]
        public void CreateOrgUnit()
        {
            string name = Guid.NewGuid().ToString();

            CSEntryChange cs = CSEntryChange.Create();
            cs.ObjectModificationType = ObjectModificationType.Add;
            cs.DN = $"/Unit testing/{name}";
            cs.ObjectType = SchemaConstants.OrgUnit;

            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("description", "my description"));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("blockInheritance", false));

            string id = null;

            try
            {
                CSEntryChangeResult result = ExportProcessor.PutCSEntryChange(cs, UnitTestControl.Schema.GetSchema().Types[SchemaConstants.OrgUnit], UnitTestControl.TestParameters);

                if (result.ErrorCode != MAExportError.Success)
                {
                    Assert.Fail(result.ErrorName);
                }

                id = result.AnchorAttributes["id"].GetValueAdd<string>();

                //Thread.Sleep(UnitTestControl.PostGoogleOperationSleepInterval);

                OrgUnit c = UnitTestControl.TestParameters.OrgUnitsService.Get(UnitTestControl.TestParameters.CustomerID, id);
                Assert.AreEqual("my description", c.Description);
                Assert.AreEqual(name, c.Name);
                Assert.AreEqual("/Unit testing", c.ParentOrgUnitPath);
                Assert.AreEqual($"/Unit testing/{name}", c.OrgUnitPath);
            }
            finally
            {
                if (id != null)
                {
                    UnitTestControl.TestParameters.OrgUnitsService.Delete(UnitTestControl.TestParameters.CustomerID, id);
                }
            }
        }

        [TestMethod]
        public void RenameOrgUnit()
        {
            string name1 = Guid.NewGuid().ToString();
            string name2 = Guid.NewGuid().ToString();
            OrgUnit ou = new OrgUnit();
            ou.ParentOrgUnitPath = "/Unit testing";
            ou.Name = name1;
            ou.Description = "my description";

            ou = UnitTestControl.TestParameters.OrgUnitsService.Insert(UnitTestControl.TestParameters.CustomerID, ou);
            string id = ou.OrgUnitId;

            CSEntryChange cs = CSEntryChange.Create();
            cs.ObjectModificationType = ObjectModificationType.Update;
            cs.DN = $"/Unit testing/{name1}";
            cs.ObjectType = SchemaConstants.OrgUnit;
            cs.AnchorAttributes.Add(AnchorAttribute.Create("id", ou.OrgUnitId));

            cs.AttributeChanges.Add(AttributeChange.CreateNewDN($"/Unit testing/{name2}"));


            try
            {
                CSEntryChangeResult result = ExportProcessor.PutCSEntryChange(cs, UnitTestControl.Schema.GetSchema().Types[SchemaConstants.OrgUnit], UnitTestControl.TestParameters);

                if (result.ErrorCode != MAExportError.Success)
                {
                    Assert.Fail(result.ErrorName);
                }

                //Thread.Sleep(UnitTestControl.PostGoogleOperationSleepInterval);

                OrgUnit c = UnitTestControl.TestParameters.OrgUnitsService.Get(UnitTestControl.TestParameters.CustomerID, id);
                Assert.AreEqual(name2, c.Name);
                Assert.AreEqual("/Unit testing", c.ParentOrgUnitPath);
                Assert.AreEqual($"/Unit testing/{name2}", c.OrgUnitPath);
            }
            finally
            {
                if (id != null)
                {
                    UnitTestControl.TestParameters.OrgUnitsService.Delete(UnitTestControl.TestParameters.CustomerID, id);
                }
            }
        }

        [TestMethod]
        public void DeleteOrgUnit()
        {
            string id = null;
            string name1 = Guid.NewGuid().ToString();

            OrgUnit ou = new OrgUnit();
            ou.ParentOrgUnitPath = "/Unit testing";
            ou.Name = name1;
            ou.Description = "my description";

            ou = UnitTestControl.TestParameters.OrgUnitsService.Insert(UnitTestControl.TestParameters.CustomerID, ou);
            id = ou.OrgUnitId;

            CSEntryChange cs = CSEntryChange.Create();
            cs.ObjectModificationType = ObjectModificationType.Delete;
            cs.DN = $"/Unit testing/{name1}";
            cs.ObjectType = SchemaConstants.OrgUnit;
            cs.AnchorAttributes.Add(AnchorAttribute.Create("id", ou.OrgUnitId));

            try
            {
                CSEntryChangeResult result = ExportProcessor.PutCSEntryChange(cs, UnitTestControl.Schema.GetSchema().Types[SchemaConstants.OrgUnit], UnitTestControl.TestParameters);

                if (result.ErrorCode != MAExportError.Success)
                {
                    Assert.Fail(result.ErrorName);
                }

                try
                {
                    OrgUnit c = UnitTestControl.TestParameters.OrgUnitsService.Get(UnitTestControl.TestParameters.CustomerID, id);

                }
                catch (GoogleApiException ex)
                {
                    if (ex.HttpStatusCode != System.Net.HttpStatusCode.NotFound)
                    {
                        Assert.Fail("The expected 404 error was not returned");
                    }
                }

                id = null;
            }
            finally
            {
                if (id != null)
                {
                    UnitTestControl.TestParameters.OrgUnitsService.Delete(UnitTestControl.TestParameters.CustomerID, id);
                }
            }
        }

        [TestMethod]
        public void MoveOrgUnit()
        {
            string parentId = null;
            string childId = null;

            try
            {
                string parentName = Guid.NewGuid().ToString();
                OrgUnit parentOu = new OrgUnit();
                parentOu.ParentOrgUnitPath = "/Unit testing";
                parentOu.Name = parentName;

                parentId = UnitTestControl.TestParameters.OrgUnitsService.Insert(UnitTestControl.TestParameters.CustomerID, parentOu).OrgUnitId;

                string childName = Guid.NewGuid().ToString();

                OrgUnit childOu = new OrgUnit();
                childOu.ParentOrgUnitPath = $"/Unit testing/{parentName}";
                childOu.Name = childName;

                childId = UnitTestControl.TestParameters.OrgUnitsService.Insert(UnitTestControl.TestParameters.CustomerID, childOu).OrgUnitId;

                CSEntryChange cs = CSEntryChange.Create();
                cs.ObjectModificationType = ObjectModificationType.Update;
                cs.DN = $"/Unit testing/{parentName}/{childName}";
                cs.ObjectType = SchemaConstants.OrgUnit;
                cs.AnchorAttributes.Add(AnchorAttribute.Create("id", childId));

                cs.AttributeChanges.Add(AttributeChange.CreateNewDN($"/Unit testing/{childName}"));

                CSEntryChangeResult result = ExportProcessor.PutCSEntryChange(cs, UnitTestControl.Schema.GetSchema().Types[SchemaConstants.OrgUnit], UnitTestControl.TestParameters);

                if (result.ErrorCode != MAExportError.Success)
                {
                    Assert.Fail(result.ErrorName);
                }

                //Thread.Sleep(UnitTestControl.PostGoogleOperationSleepInterval);

                OrgUnit c = UnitTestControl.TestParameters.OrgUnitsService.Get(UnitTestControl.TestParameters.CustomerID, childId);
                Assert.AreEqual(childName, c.Name);
                Assert.AreEqual("/Unit testing", c.ParentOrgUnitPath);
                Assert.AreEqual($"/Unit testing/{childName}", c.OrgUnitPath);
            }
            finally
            {
                if (childId != null)
                {
                    UnitTestControl.TestParameters.OrgUnitsService.Delete(UnitTestControl.TestParameters.CustomerID, childId);
                }

                if (parentId != null)
                {
                    UnitTestControl.TestParameters.OrgUnitsService.Delete(UnitTestControl.TestParameters.CustomerID, parentId);
                }
            }
        }

        [TestMethod]
        public void UpdateOrgUnitDescription()
        {
            string name = Guid.NewGuid().ToString();
            OrgUnit ou = new OrgUnit();
            ou.ParentOrgUnitPath = "/Unit testing";
            ou.Name = name;
            ou.Description = "my description";

            ou = UnitTestControl.TestParameters.OrgUnitsService.Insert(UnitTestControl.TestParameters.CustomerID, ou);

            CSEntryChange cs = CSEntryChange.Create();
            cs.ObjectModificationType = ObjectModificationType.Update;
            cs.DN = $"/Unit testing/{name}";
            cs.ObjectType = SchemaConstants.OrgUnit;
            cs.AnchorAttributes.Add(AnchorAttribute.Create("id", ou.OrgUnitId));

            cs.AttributeChanges.Add(AttributeChange.CreateAttributeReplace("description", "new description"));

            string id = ou.OrgUnitId;

            try
            {
                CSEntryChangeResult result = ExportProcessor.PutCSEntryChange(cs, UnitTestControl.Schema.GetSchema().Types[SchemaConstants.OrgUnit], UnitTestControl.TestParameters);

                if (result.ErrorCode != MAExportError.Success)
                {
                    Assert.Fail(result.ErrorName);
                }

                //Thread.Sleep(UnitTestControl.PostGoogleOperationSleepInterval);

                OrgUnit c = UnitTestControl.TestParameters.OrgUnitsService.Get(UnitTestControl.TestParameters.CustomerID, id);
                Assert.AreEqual("new description", c.Description);
            }
            finally
            {
                if (id != null)
                {
                    UnitTestControl.TestParameters.OrgUnitsService.Delete(UnitTestControl.TestParameters.CustomerID, id);
                }
            }
        }

        [TestMethod]
        public void UpdateOrgUnitBlockInheritanceOn()
        {
            string name = Guid.NewGuid().ToString();
            OrgUnit ou = new OrgUnit();
            ou.ParentOrgUnitPath = "/Unit testing";
            ou.Name = name;
            ou.BlockInheritance = false;

            ou = UnitTestControl.TestParameters.OrgUnitsService.Insert(UnitTestControl.TestParameters.CustomerID, ou);

            CSEntryChange cs = CSEntryChange.Create();
            cs.ObjectModificationType = ObjectModificationType.Update;
            cs.DN = $"/Unit testing/{name}";
            cs.ObjectType = SchemaConstants.OrgUnit;
            cs.AnchorAttributes.Add(AnchorAttribute.Create("id", ou.OrgUnitId));

            cs.AttributeChanges.Add(AttributeChange.CreateAttributeReplace("blockInheritance", true));

            string id = ou.OrgUnitId;

            try
            {
                CSEntryChangeResult result = ExportProcessor.PutCSEntryChange(cs, UnitTestControl.Schema.GetSchema().Types[SchemaConstants.OrgUnit], UnitTestControl.TestParameters);

                if (result.ErrorCode != MAExportError.Success)
                {
                    Assert.Fail(result.ErrorName);
                }

                // Thread.Sleep(UnitTestControl.PostGoogleOperationSleepInterval);

                OrgUnit c = UnitTestControl.TestParameters.OrgUnitsService.Get(UnitTestControl.TestParameters.CustomerID, id);
                Assert.AreEqual(true, c.BlockInheritance);
            }
            finally
            {
                if (id != null)
                {
                    UnitTestControl.TestParameters.OrgUnitsService.Delete(UnitTestControl.TestParameters.CustomerID, id);
                }
            }
        }

        [TestMethod]
        public void UpdateOrgUnitBlockInheritanceOff()
        {
            string name = Guid.NewGuid().ToString();
            OrgUnit ou = new OrgUnit();
            ou.ParentOrgUnitPath = "/Unit testing";
            ou.Name = name;
            ou.BlockInheritance = true;

            ou = UnitTestControl.TestParameters.OrgUnitsService.Insert(UnitTestControl.TestParameters.CustomerID, ou);

            CSEntryChange cs = CSEntryChange.Create();
            cs.ObjectModificationType = ObjectModificationType.Update;
            cs.DN = $"/Unit testing/{name}";
            cs.ObjectType = SchemaConstants.OrgUnit;
            cs.AnchorAttributes.Add(AnchorAttribute.Create("id", ou.OrgUnitId));

            cs.AttributeChanges.Add(AttributeChange.CreateAttributeReplace("blockInheritance", false));

            string id = ou.OrgUnitId;

            try
            {
                CSEntryChangeResult result = ExportProcessor.PutCSEntryChange(cs, UnitTestControl.Schema.GetSchema().Types[SchemaConstants.OrgUnit], UnitTestControl.TestParameters);

                if (result.ErrorCode != MAExportError.Success)
                {
                    Assert.Fail(result.ErrorName);
                }

                // Thread.Sleep(UnitTestControl.PostGoogleOperationSleepInterval);

                OrgUnit c = UnitTestControl.TestParameters.OrgUnitsService.Get(UnitTestControl.TestParameters.CustomerID, id);
                Assert.AreEqual(false, c.BlockInheritance);
            }
            finally
            {
                if (id != null)
                {
                    UnitTestControl.TestParameters.OrgUnitsService.Delete(UnitTestControl.TestParameters.CustomerID, id);
                }
            }
        }

        [TestMethod]
        public void UpdateOrgUnitClearDescription()
        {
            string name = Guid.NewGuid().ToString();
            OrgUnit ou = new OrgUnit();
            ou.ParentOrgUnitPath = "/Unit testing";
            ou.Name = name;
            ou.Description = "my description";

            ou = UnitTestControl.TestParameters.OrgUnitsService.Insert(UnitTestControl.TestParameters.CustomerID, ou);

            CSEntryChange cs = CSEntryChange.Create();
            cs.ObjectModificationType = ObjectModificationType.Update;
            cs.DN = $"/Unit testing/{name}";
            cs.ObjectType = SchemaConstants.OrgUnit;
            cs.AnchorAttributes.Add(AnchorAttribute.Create("id", ou.OrgUnitId));


            cs.AttributeChanges.Add(AttributeChange.CreateAttributeDelete("description"));

            string id = ou.OrgUnitId;

            try
            {
                CSEntryChangeResult result = ExportProcessor.PutCSEntryChange(cs, UnitTestControl.Schema.GetSchema().Types[SchemaConstants.OrgUnit], UnitTestControl.TestParameters);

                if (result.ErrorCode != MAExportError.Success)
                {
                    Assert.Fail(result.ErrorName);
                }

                //Thread.Sleep(UnitTestControl.PostGoogleOperationSleepInterval);
                OrgUnit c = UnitTestControl.TestParameters.OrgUnitsService.Get(UnitTestControl.TestParameters.CustomerID, id);
                Assert.IsTrue(string.IsNullOrEmpty(c.Description));
            }
            finally
            {
                if (id != null)
                {
                    UnitTestControl.TestParameters.OrgUnitsService.Delete(UnitTestControl.TestParameters.CustomerID, id);
                }
            }
        }
    }
}