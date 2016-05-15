using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lithnet.GoogleApps.MA;

namespace Lithnet.GoogleApps.MA.UnitTests
{
    using System.Net;
    using System.Net.Security;
    using System.Security.Cryptography.X509Certificates;
    using Lithnet.GoogleApps.MA;
    using MetadirectoryServices;
    using Microsoft.MetadirectoryServices;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using ManagementAgent = global::Lithnet.GoogleApps.MA.ManagementAgent;

    [TestClass]
    internal static class UnitTestControl
    {
        static UnitTestControl()
        {
           // UnitTestControl.BuildSchema();
        }

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
        }

        private static void BuildSchema()
        {
            ServicePointManager.ServerCertificateValidationCallback = delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };
            ConnectionPools.DisableGzip = true;

            UnitTestControl.TestParameters = new TestParameters();
            ConnectionPools.InitializePools(TestParameters.Credentials, 1, 1, 1, 1);
            UnitTestControl.Schema = SchemaBuilder.GetSchema(UnitTestControl.TestParameters);
            ManagementAgent.Schema = UnitTestControl.Schema;
            UnitTestControl.MmsSchema = UnitTestControl.Schema.GetSchema();
        }
    }
}
