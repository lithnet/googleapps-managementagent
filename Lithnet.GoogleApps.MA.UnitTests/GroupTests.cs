using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Lithnet.GoogleApps;
using Lithnet.GoogleApps.MA;
using Lithnet.GoogleApps.ManagedObjects;

namespace Lithnet.GoogleApps.MA.UnitTests
{
    using System.Linq;
    using System.Net;
    using System.Net.Security;
    using System.Security.Cryptography.X509Certificates;
    using Lithnet.GoogleApps.MA;

    [TestClass]
    public class GroupTests
    {
        [TestMethod]
        public void TestMethod1()
        {

    //        ServicePointManager.ServerCertificateValidationCallback =
    //delegate (object s, X509Certificate certificate,
    //        X509Chain chain, SslPolicyErrors sslPolicyErrors)
    //{ return true; };

            TestParameters r = new TestParameters();

            ConnectionPools.InitializePools(r.Credentials, 1, 1);
           // SchemaBuilder.CreateGoogleAppsCustomSchema();

            var cont = ContactsRequestFactory.GetContacts("ga-staff-dev.monash.edu").ToList();


            List<string> items = UserSettingsRequestFactory.GetDelegates("amut0001-student.monash.edu-d1@ga-staff-dev.monash.edu").ToList();
         

            var d = SchemaRequestFactory.GetSchema("my_customer", SchemaConstants.CustomGoogleAppsSchemaName);

            User u= UserRequestFactory.Get("amut0001-student.monash.edu-d1@ga-staff-dev.monash.edu");
            u.CustomSchemas = new Dictionary<string, IDictionary<string, object>>();
            u.CustomSchemas.Add(SchemaConstants.CustomGoogleAppsSchemaName, new Dictionary<string, object>());
            u.CustomSchemas[SchemaConstants.CustomGoogleAppsSchemaName].Add(SchemaConstants.CustomSchemaObjectType, SchemaConstants.AdvancedUser);
            u.IncludeInGlobalAddressList = true;
            User x = UserRequestFactory.Update(u, u.PrimaryEmail);

        }
    }
}
