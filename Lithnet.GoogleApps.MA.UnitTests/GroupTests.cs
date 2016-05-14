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
    using Google.GData.Extensions;
    using Lithnet.GoogleApps.MA;

    [TestClass]
    public class GroupTests
    {
        [TestMethod]
        public void TestMethod1()
        {
            return;
            

            TestParameters r = new TestParameters();

            ConnectionPools.InitializePools(r.Credentials, 1, 1, 1, 1);
            // SchemaBuilder.CreateGoogleAppsCustomSchema();

            foreach (var t in ContactRequestFactory.GetContacts("ga-staff-dev.monash.edu"))
            {
                if (t.PrimaryEmail.Address.Contains("d1@"))
                {
                    bool update = false;
                    ContactEntry x = ContactRequestFactory.GetContact(t.SelfUri.Content);

                    
                    ExtendedProperty dn = x.ExtendedProperties.FirstOrDefault(e => e.Name == "lithnet-google-ma-dn");
                    if (dn == null)
                    {
                        dn = new ExtendedProperty();
                        dn.Name = "lithnet-google-ma-dn";
                        dn.Value = "contact::" + x.PrimaryEmail.Address;
                        x.ExtendedProperties.Add(dn);
                        update = true;
                    }

                    if (x.Organizations.Count > 0 && x.Organizations[0].Rel != "http://schemas.google.com/g/2005#work")
                    {
                        if (x.Organizations[0].Label != null)
                        {
                            x.Organizations[0].Label = null;
                        }

                        x.Organizations[0].Rel = "http://schemas.google.com/g/2005#work";
                        update = true;
                    }
                    
                    if (update)
                    {
                        ContactRequestFactory.Update(x);
                        var updated = ContactRequestFactory.GetContact(x.SelfUri.Content);
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
