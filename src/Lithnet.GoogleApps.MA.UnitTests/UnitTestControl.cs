using System;
using Lithnet.GoogleApps.ManagedObjects;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using Lithnet.MetadirectoryServices;
using Microsoft.MetadirectoryServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lithnet.GoogleApps.MA.UnitTests
{
    
    [TestClass]
    internal static class UnitTestControl
    {
        [AssemblyCleanup()]
        public static void AssemblyCleanup()
        {
            CSEntryChangeQueue.SaveQueue("D:\\temp\\test-run.xml", UnitTestControl.MmsSchema);
        }

        public static MASchemaTypes Schema { get; private set; }
       
        public static Schema MmsSchema { get; private set; }

        public static TestParameters TestParameters { get; private set; }

        [AssemblyInitialize]
        public static void Initialize(TestContext c)
        {
            BuildSchema();
            GroupMembership.GetInternalDomains(TestParameters.CustomerID);
        }

        private static void BuildSchema()
        {
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            ConnectionPools.DisableGzip = true;

            UnitTestControl.TestParameters = new TestParameters();
            ConnectionPools.InitializePools(TestParameters.Credentials, 1, 1, 1, 1);
            UnitTestControl.Schema = SchemaBuilder.GetSchema(UnitTestControl.TestParameters);
            ManagementAgent.Schema = UnitTestControl.Schema;
            UnitTestControl.MmsSchema = UnitTestControl.Schema.GetSchema();
        }
    }
}
