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
    using Google.Contacts;
    using Google.GData.Contacts;
    using Lithnet.GoogleApps.MA;

    [TestClass]
    public class GroupTests
    {
        [TestMethod]
        public void TestMethod1()
        {

            ServicePointManager.ServerCertificateValidationCallback = delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };
            ConnectionPools.DisableGzip = true;

            TestParameters r = new TestParameters();

            ConnectionPools.InitializePools(r.Credentials, 1, 1);
            // SchemaBuilder.CreateGoogleAppsCustomSchema();

            foreach (var t in ContactRequestFactory.GetContacts("ga-staff-dev.monash.edu"))
            {
                if (t.PrimaryEmail.Address.Contains("d1@"))
                {
                    bool update = false;
                    Contact x  = ContactRequestFactory.GetContact(t.SelfUri.Content);

                    if (x.Organizations.Count > 0 && x.Organizations[0].Rel != "http://schemas.google.com/g/2005#work")
                    {
                        if (x.Organizations[0].Label != null)
                        {
                            x.Organizations[0].Label = null;
                        }

                        x.Organizations[0].Rel = "http://schemas.google.com/g/2005#work";
                        update = true;
                    }

                    if (x.ContactEntry.ExternalIds.Count == 0)
                    {
                        ExternalId id = new ExternalId();

                        id.Label = "monashPersonID";
                        id.Value = Guid.NewGuid().ToString();

                        x.ContactEntry.ExternalIds.Add(id);
                        update = true;
                    }

                    if (update)
                    {
                        ContactRequestFactory.UpdateContact(x);
                        var updated = ContactRequestFactory.GetContact(x.ContactEntry.SelfUri.Content);
                    }
                }
            }


            List<string> items = UserSettingsRequestFactory.GetDelegates("amut0001-student.monash.edu-d1@ga-staff-dev.monash.edu").ToList();


            var d = SchemaRequestFactory.GetSchema("my_customer", SchemaConstants.CustomGoogleAppsSchemaName);

            User u = UserRequestFactory.Get("amut0001-student.monash.edu-d1@ga-staff-dev.monash.edu");
            u.CustomSchemas = new Dictionary<string, IDictionary<string, object>>();
            u.CustomSchemas.Add(SchemaConstants.CustomGoogleAppsSchemaName, new Dictionary<string, object>());
            u.CustomSchemas[SchemaConstants.CustomGoogleAppsSchemaName].Add(SchemaConstants.CustomSchemaObjectType, SchemaConstants.AdvancedUser);
            u.IncludeInGlobalAddressList = true;
            User zx = UserRequestFactory.Update(u, u.PrimaryEmail);

        }
    }
}
