using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lithnet.GoogleApps.MA.UnitTests
{
    using System.Net;
    using Google.GData.Client;
    using Google.GData.Contacts;
    using Google.GData.Extensions;
    using Microsoft.MetadirectoryServices;

    [TestClass]
    public class ContactTests
    {
        [TestMethod]
        public void ContactAdd()
        {
            CSEntryChange cs = CSEntryChange.Create();
            cs.ObjectModificationType = ObjectModificationType.Add;
            cs.DN = Guid.NewGuid().ToString();
            cs.ObjectType = SchemaConstants.Contact;

            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("billingInformation", "billingInformation"));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("birthday", "2000-01-01"));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("directoryServer", "directoryServer"));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("initials", "initials"));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("location", "location"));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("maidenName", "maidenName"));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("mileage", "mileage"));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("nickname", "nickname"));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("occupation", "occupation"));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("sensitivity", "private"));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("shortName", "shortName"));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("subject", "subject"));

            string id = null;

            try
            {
                CSEntryChangeResult result = ExportProcessor.PutCSEntryChange(cs, UnitTestControl.Schema.GetSchema().Types[SchemaConstants.Contact]);
                id = result.AnchorAttributes["id"].GetStringValueAddOrNullPlaceholder();

                if (result.ErrorCode != MAExportError.Success)
                {
                    Assert.Fail(result.ErrorName);
                }

                ContactEntry e = ContactRequestFactory.GetContact(id);
                Assert.AreEqual("billingInformation", e.BillingInformation);
                Assert.AreEqual("2000-01-01", e.Birthday);
                Assert.AreEqual("directoryServer", e.DirectoryServer);
                Assert.AreEqual("initials", e.Initials);
                Assert.AreEqual("location", e.Location);
                Assert.AreEqual("maidenName", e.MaidenName);
                Assert.AreEqual("mileage", e.Mileage);
                Assert.AreEqual("nickname", e.Nickname);
                Assert.AreEqual("occupation", e.Occupation);
                Assert.AreEqual("private", e.Sensitivity);
                Assert.AreEqual("shortName", e.ShortName);
                Assert.AreEqual("subject", e.Subject);

            }
            finally
            {
                if (id != null)
                {
                    ContactRequestFactory.DeleteContact(id);
                }
            }

        }

        [TestMethod]
        public void ContactDelete()
        {
            string id = null;

            try
            {
                string dn = Guid.NewGuid().ToString();
                ContactEntry e = new ContactEntry
                {
                    BillingInformation = "test"
                };

                e.ExtendedProperties.Add(new ExtendedProperty(dn, ApiInterfaceContact.DNAttributeName));

                e = ContactRequestFactory.CreateContact(e, UnitTestControl.TestParameters.Domain);
                id = e.SelfUri.Content;

                CSEntryChange cs = CSEntryChange.Create();
                cs.ObjectModificationType = ObjectModificationType.Delete;
                cs.DN = dn;
                
                cs.AnchorAttributes.Add(AnchorAttribute.Create("id", id));
                CSEntryChangeResult result = ExportProcessor.PutCSEntryChange(cs, UnitTestControl.Schema.GetSchema().Types[SchemaConstants.Contact]);

                if (result.ErrorCode != MAExportError.Success)
                {
                    Assert.Fail(result.ErrorName);
                }

                try
                {
                    System.Threading.Thread.Sleep(5000);
                    e = ContactRequestFactory.GetContact(id);
                    Assert.Fail("The object did not get deleted");
                }
                catch (GDataRequestException ex)
                {
                    if (((HttpWebResponse)ex.Response).StatusCode == HttpStatusCode.NotFound)
                    {
                        id = null;
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            finally
            {
                if (id != null)
                {
                    ContactRequestFactory.DeleteContact(id);
                }
            }

        }
    }
}
