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
    public class FeatureTests
    {
        [TestMethod]
        public void GetFeatures()
        {
            foreach (Feature item in UnitTestControl.TestParameters.ResourcesService.GetFeatures("my_customer"))
            {
                {
                    Trace.WriteLine(item.Name);
                }
            }
        }

        [TestMethod]
        public void GetFeaturesViaApiInterface()
        {
            MASchemaType s = UnitTestControl.Schema[SchemaConstants.Feature];

            ApiInterfaceFeature u = new ApiInterfaceFeature("my_customer", s, UnitTestControl.TestParameters);

            BlockingCollection<object> items = new BlockingCollection<object>();

            u.GetItems(UnitTestControl.MmsSchema, items).Wait();

            foreach (CSEntryChange item in items.OfType<CSEntryChange>())
            {
                Assert.AreEqual(MAImportError.Success, item.ErrorCodeImport);
            }

            Assert.AreNotEqual(0, items.Count);
        }

        [TestMethod]
        public void CreateFeature()
        {
            Guid g = Guid.NewGuid();

            CSEntryChange cs = CSEntryChange.Create();
            cs.ObjectModificationType = ObjectModificationType.Add;
            cs.DN = $"my-feature{g:N}{ApiInterfaceFeature.DNSuffix}";
            cs.ObjectType = SchemaConstants.Feature;

            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("name", "My feature 2"));

            string id = null;

            try
            {
                CSEntryChangeResult result = ExportProcessor.PutCSEntryChange(cs, UnitTestControl.Schema.GetSchema().Types[SchemaConstants.Feature], UnitTestControl.TestParameters);

                if (result.ErrorCode != MAExportError.Success)
                {
                    Assert.Fail(result.ErrorName);
                }

                id = result.AnchorAttributes["id"].GetValueAdd<string>();

                Thread.Sleep(UnitTestControl.PostGoogleOperationSleepInterval);

                Feature c = UnitTestControl.TestParameters.ResourcesService.GetFeature(UnitTestControl.TestParameters.CustomerID, id);
                Assert.AreEqual($"my-feature{g:N}", c.Name);
            }
            finally
            {
                if (id != null)
                {
                    UnitTestControl.TestParameters.ResourcesService.DeleteFeature(UnitTestControl.TestParameters.CustomerID, id);
                }
            }
        }
    }
}