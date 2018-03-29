﻿using System;
using System.Linq;
using Lithnet.GoogleApps.ManagedObjects;
using System.Net;
using System.Threading;
using Google.Apis.Admin.Directory.directory_v1.Data;
using Lithnet.MetadirectoryServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Schema = Microsoft.MetadirectoryServices.Schema;
using User = Lithnet.GoogleApps.ManagedObjects.User;

namespace Lithnet.GoogleApps.MA.UnitTests
{
    
    [TestClass]
    internal static class UnitTestControl
    {
        internal const int PostGoogleOperationSleepInterval = 5000;

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
            var x = MAConfigurationSection.Configuration;
        }

        private static void BuildSchema()
        {
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            ConnectionPools.DisableGzip = true;

            UnitTestControl.TestParameters = new TestParameters();
            ConnectionPools.InitializePools(TestParameters.GetCredentials(TestParameters.TestScopes), 1, 1, 1, 1, 1);
            UnitTestControl.Schema = SchemaBuilder.GetSchema(UnitTestControl.TestParameters);
            ManagementAgent.Schema = UnitTestControl.Schema;
            UnitTestControl.MmsSchema = UnitTestControl.Schema.GetSchema();
        }

        public static void Cleanup(params object[] objects)
        {
            if (objects == null)
            {
                return;
            }

            foreach (Group g in objects.OfType<Group>())
            {
                if (g != null)
                {
                    GroupRequestFactory.Delete(g.Id);
                }
            }

            foreach (User u in objects.OfType<User>())
            {
                if (u != null)
                {
                    UserRequestFactory.Delete(u.Id);
                }
            }
        }
        public static Group CreateGroup()
        {
            string dn = $"{Guid.NewGuid()}@{UnitTestControl.TestParameters.Domain}";
            Group e = new Group
            {
                Email = dn,
                Name = Guid.NewGuid().ToString()
            };

            e = GroupRequestFactory.Add(e);

            Thread.Sleep(1000);
            return e;
        }
    }
}
