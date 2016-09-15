namespace Lithnet.GoogleApps.MA.UnitTests
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using Google.GData.Client;
    using Google.GData.Contacts;
    using Google.GData.Extensions;
    using MetadirectoryServices;
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

            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("externalIds_work", "eidwork"));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("externalIds_home", "eidhome"));

            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("organizations_work_name", "name"));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("organizations_work_title", "title"));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("organizations_work_department", "department"));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("organizations_work_symbol", "symbol"));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("organizations_work_jobDescription", "jobDescription"));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("organizations_work_location", "location"));

            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("phones_work", "phwork"));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("phones_home", "phhome"));

            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("email_work", "work@work.com"));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("email_home", "home@home.com"));

            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("ims_work_address", "work@ims.com"));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("ims_work_protocol", "proto"));

            string id = null;

            try
            {
                CSEntryChangeResult result = ExportProcessor.PutCSEntryChange(cs, UnitTestControl.Schema.GetSchema().Types[SchemaConstants.Contact], UnitTestControl.TestParameters);
                
                id = result.AnchorAttributes["id"].GetValueAdd<string>();


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

                Assert.AreEqual(2, e.ExternalIds.Count);
                Assert.AreEqual("eidwork", e.ExternalIds[0].Value);
                Assert.AreEqual("eidhome", e.ExternalIds[1].Value);

                Assert.AreEqual(1, e.Organizations.Count);
                Assert.AreEqual("name", e.Organizations[0].Name);
                Assert.AreEqual("title", e.Organizations[0].Title);
                Assert.AreEqual("department", e.Organizations[0].Department);
                Assert.AreEqual("symbol", e.Organizations[0].Symbol);
                Assert.AreEqual("jobDescription", e.Organizations[0].JobDescription);
                Assert.AreEqual("location", e.Organizations[0].Location);

                Assert.AreEqual(2, e.Phonenumbers.Count);
                Assert.AreEqual("phwork", e.Phonenumbers[0].Value);
                Assert.AreEqual("phhome", e.Phonenumbers[1].Value);

                Assert.AreEqual(2, e.Emails.Count);
                Assert.AreEqual("work@work.com", e.Emails[0].Address);
                Assert.AreEqual("home@home.com", e.Emails[1].Address);
                Assert.AreEqual("work@work.com", e.PrimaryEmail.Address);

                Assert.AreEqual(1, e.IMs.Count);
                Assert.AreEqual("work@ims.com", e.IMs[0].Address);
                Assert.AreEqual("proto", e.IMs[0].Protocol);
            }
            finally
            {
                if (id != null)
                {
                    ContactRequestFactory.Delete(id);
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

                e = ContactRequestFactory.Add(e, UnitTestControl.TestParameters.Domain);
                id = e.SelfUri.Content;

                CSEntryChange cs = CSEntryChange.Create();
                cs.ObjectModificationType = ObjectModificationType.Delete;
                cs.DN = dn;
                cs.ObjectType = SchemaConstants.Contact;
                cs.AnchorAttributes.Add(AnchorAttribute.Create("id", id));
                CSEntryChangeResult result = ExportProcessor.PutCSEntryChange(cs, UnitTestControl.Schema.GetSchema().Types[SchemaConstants.Contact], UnitTestControl.TestParameters);

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
                    ContactRequestFactory.Delete(id);
                }
            }

        }

        [TestMethod]
        public void ContactRename()
        {
            string id = null;

            try
            {
                string dn = Guid.NewGuid().ToString();
                ContactEntry e = new ContactEntry();

                e.Emails.Add(new EMail() { Address = "test@test.com", Label = "work" });

                e.ExtendedProperties.Add(new ExtendedProperty(dn, ApiInterfaceContact.DNAttributeName));

                e = ContactRequestFactory.Add(e, UnitTestControl.TestParameters.Domain);
                id = e.SelfUri.Content;

                CSEntryChange cs = CSEntryChange.Create();
                cs.ObjectModificationType = ObjectModificationType.Update;
                cs.DN = dn;
                cs.ObjectType = SchemaConstants.Contact;

                string newDN = Guid.NewGuid().ToString();

                cs.AttributeChanges.Add(AttributeChange.CreateAttributeUpdate("DN", new List<ValueChange>() { ValueChange.CreateValueAdd(newDN), ValueChange.CreateValueDelete(dn) }));

                cs.AnchorAttributes.Add(AnchorAttribute.Create("id", id));
                CSEntryChangeResult result = ExportProcessor.PutCSEntryChange(cs, UnitTestControl.Schema.GetSchema().Types[SchemaConstants.Contact], UnitTestControl.TestParameters);

                if (result.ErrorCode != MAExportError.Success)
                {
                    Assert.Fail(result.ErrorName);
                }

                System.Threading.Thread.Sleep(5000);
                e = ContactRequestFactory.GetContact(id);
                Assert.AreEqual(newDN, e.ExtendedProperties.Single(t => t.Name == ApiInterfaceContact.DNAttributeName).Value);
                var x = CSEntryChangeQueue.Take();
            }
            finally
            {
                if (id != null)
                {
                    ContactRequestFactory.Delete(id);
                }
            }

        }

        [TestMethod]
        public void ContactUpdate()
        {
            string id = null;
            string dn = Guid.NewGuid().ToString();
            ContactEntry e = new ContactEntry
            {
                BillingInformation = "test",
                Birthday = "2001-01-01",
                DirectoryServer = "test",
                Initials = "test",
                Location = "test",
                MaidenName = "test",
                Mileage = "test",
                Nickname = "test",
                Occupation = "test",
                Sensitivity = "normal",
                ShortName = "test",
                Subject = "test",
            };

            e.ExternalIds.Add(new ExternalId() {Label = "work", Value = "test"});
            e.Organizations.Add(new  Organization()
            {
                Rel = "http://schemas.google.com/g/2005#work",
                Name = "test",
                Title = "test",
                Department = "test",
                Symbol= "test",
                JobDescription = "test",
                Location = "test",
            });

            e.Phonenumbers.Add(new PhoneNumber() {Primary = true, Rel = "http://schemas.google.com/g/2005#home", Value = "test"});

            e.ExtendedProperties.Add(new ExtendedProperty(dn, ApiInterfaceContact.DNAttributeName));

            e = ContactRequestFactory.Add(e, UnitTestControl.TestParameters.Domain);
            id = e.SelfUri.Content;

            CSEntryChange cs = CSEntryChange.Create();
            cs.ObjectModificationType = ObjectModificationType.Update;
            cs.DN = Guid.NewGuid().ToString();
            cs.ObjectType = SchemaConstants.Contact;
            cs.AnchorAttributes.Add(AnchorAttribute.Create("id", id));

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

            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("externalIds_work", "eidwork"));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("externalIds_home", "eidhome"));

            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("organizations_work_name", "name"));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("organizations_work_title", "title"));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("organizations_work_department", "department"));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("organizations_work_symbol", "symbol"));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("organizations_work_jobDescription", "jobDescription"));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("organizations_work_location", "location"));

            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("phones_work", "phwork"));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("phones_home", "phhome"));

            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("email_work", "work@work.com"));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("email_home", "home@home.com"));

            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("ims_work_address", "work@ims.com"));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("ims_work_protocol", "proto"));


            try
            {
                CSEntryChangeResult result = ExportProcessor.PutCSEntryChange(cs, UnitTestControl.Schema.GetSchema().Types[SchemaConstants.Contact], UnitTestControl.TestParameters);

                if (result.ErrorCode != MAExportError.Success)
                {
                    Assert.Fail(result.ErrorName);
                }

                System.Threading.Thread.Sleep(5000);

                e = ContactRequestFactory.GetContact(id);
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

                Assert.AreEqual(2, e.ExternalIds.Count);
                Assert.AreEqual("eidwork", e.ExternalIds[0].Value);
                Assert.AreEqual("eidhome", e.ExternalIds[1].Value);

                Assert.AreEqual(1, e.Organizations.Count);
                Assert.AreEqual("name", e.Organizations[0].Name);
                Assert.AreEqual("title", e.Organizations[0].Title);
                Assert.AreEqual("department", e.Organizations[0].Department);
                Assert.AreEqual("symbol", e.Organizations[0].Symbol);
                Assert.AreEqual("jobDescription", e.Organizations[0].JobDescription);
                Assert.AreEqual("location", e.Organizations[0].Location);

                Assert.AreEqual(2, e.Phonenumbers.Count);
                Assert.AreEqual("phwork", e.Phonenumbers.First( t=> t.Rel == "http://schemas.google.com/g/2005#work").Value);
                Assert.AreEqual(true, e.Phonenumbers.First( t=> t.Rel == "http://schemas.google.com/g/2005#work").Primary);
                Assert.AreEqual("phhome", e.Phonenumbers.First( t=> t.Rel == "http://schemas.google.com/g/2005#home").Value);

                Assert.AreEqual(2, e.Emails.Count);
                Assert.AreEqual("work@work.com", e.Emails[0].Address);
                Assert.AreEqual("home@home.com", e.Emails[1].Address);
                Assert.AreEqual("work@work.com", e.PrimaryEmail.Address);

                Assert.AreEqual(1, e.IMs.Count);
                Assert.AreEqual("work@ims.com", e.IMs[0].Address);
                Assert.AreEqual("proto", e.IMs[0].Protocol);
            }
            finally
            {
                if (id != null)
                {
                    ContactRequestFactory.Delete(id);
                }
            }

        }
    }
}
