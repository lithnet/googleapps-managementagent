namespace Lithnet.GoogleApps.MA.UnitTests
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Net;
    using Google;
    using Lithnet.GoogleApps;
    using ManagedObjects;
    using MetadirectoryServices;
    using Microsoft.MetadirectoryServices;

    [TestClass]
    public class UserTests
    {
        [TestMethod]
        public void Add()
        {
            CSEntryChange cs = CSEntryChange.Create();
            cs.ObjectModificationType = ObjectModificationType.Add;
            cs.DN = $"{Guid.NewGuid()}@{UnitTestControl.TestParameters.Domain}";
            cs.ObjectType = SchemaConstants.User;

            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("orgUnitPath", "/"));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("name_givenName", "gn"));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("name_familyName", "sn"));

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
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("organizations_work_costCenter", "costCenter"));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("organizations_work_location", "location"));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("organizations_work_description", "description"));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("organizations_work_domain", "domain"));

            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("phones_work", "phwork"));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("phones_home", "phhome"));

            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("ims_work_address", "work@ims.com"));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("ims_work_protocol", "proto"));

            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("isAdmin", true));

            string alias1 = $"{Guid.NewGuid()}@{UnitTestControl.TestParameters.Domain}";
            string alias2 = $"{Guid.NewGuid()}@{UnitTestControl.TestParameters.Domain}";

            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("aliases", new List<object>() { alias1, alias2 }));

            string id = null;

            try
            {
                CSEntryChangeResult result = ExportProcessor.PutCSEntryChange(cs, UnitTestControl.Schema.GetSchema().Types[SchemaConstants.User]);
                id = result.AnchorAttributes["id"].GetStringValueAddOrNullPlaceholder();

                if (result.ErrorCode != MAExportError.Success)
                {
                    Assert.Fail(result.ErrorName);
                }

                System.Threading.Thread.Sleep(5000);

                User e = UserRequestFactory.Get(id);
                Assert.AreEqual(cs.DN, e.PrimaryEmail);
                Assert.AreEqual("/", e.OrgUnitPath);
                Assert.AreEqual("gn", e.Name.GivenName);
                Assert.AreEqual("sn", e.Name.FamilyName);

                Assert.AreEqual(true, e.IsAdmin);

                Assert.AreEqual(2, e.ExternalIds.Count);
                Assert.AreEqual("eidwork", e.ExternalIds[0].Value);
                Assert.AreEqual("eidhome", e.ExternalIds[1].Value);

                Assert.AreEqual(1, e.Organizations.Count);
                Assert.AreEqual("name", e.Organizations[0].Name);
                Assert.AreEqual("title", e.Organizations[0].Title);
                Assert.AreEqual("department", e.Organizations[0].Department);
                Assert.AreEqual("symbol", e.Organizations[0].Symbol);
                Assert.AreEqual("costCenter", e.Organizations[0].CostCenter);
                Assert.AreEqual("location", e.Organizations[0].Location);
                Assert.AreEqual("description", e.Organizations[0].Description);
                Assert.AreEqual("domain", e.Organizations[0].Domain);

                Assert.AreEqual(2, e.Phones.Count);
                Assert.AreEqual("phwork", e.Phones[0].Value);
                Assert.AreEqual("phhome", e.Phones[1].Value);

                Assert.AreEqual(1, e.Ims.Count);
                Assert.AreEqual("work@ims.com", e.Ims[0].IMAddress);
                Assert.AreEqual("proto", e.Ims[0].Protocol);

                CollectionAssert.AreEquivalent(new string[] { alias1, alias2 }, e.Aliases);
            }
            finally
            {
                if (id != null)
                {
                    UserRequestFactory.Delete(id);
                }
            }

        }

        [TestMethod]
        public void Delete()
        {
            string id = null;

            try
            {
                string dn = $"{Guid.NewGuid()}@{UnitTestControl.TestParameters.Domain}";
                User e = new User
                {
                    PrimaryEmail = dn,
                    Password = Guid.NewGuid().ToString(),
                    Name =
                    {
                        GivenName = "test",
                        FamilyName = "test"
                    }
                };

                e = UserRequestFactory.Add(e);
                id = e.Id;

                CSEntryChange cs = CSEntryChange.Create();
                cs.ObjectModificationType = ObjectModificationType.Delete;
                cs.DN = dn;
                cs.ObjectType = SchemaConstants.User;
                cs.AnchorAttributes.Add(AnchorAttribute.Create("id", id));
                CSEntryChangeResult result = ExportProcessor.PutCSEntryChange(cs, UnitTestControl.Schema.GetSchema().Types[SchemaConstants.User]);

                if (result.ErrorCode != MAExportError.Success)
                {
                    Assert.Fail(result.ErrorName);
                }

                try
                {
                    System.Threading.Thread.Sleep(5000);
                    e = UserRequestFactory.Get(id);
                    Assert.Fail("The object did not get deleted");
                }
                catch (GoogleApiException ex)
                {
                    if (ex.HttpStatusCode == HttpStatusCode.NotFound)
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
                    UserRequestFactory.Delete(id);
                }
            }

        }

        [TestMethod]
        public void Rename()
        {
            string id = null;

            try
            {
                string dn = $"{Guid.NewGuid()}@{UnitTestControl.TestParameters.Domain}";
                User e = new User
                {
                    PrimaryEmail = dn,
                    Password = Guid.NewGuid().ToString(),
                    Name =
                    {
                        GivenName = "test",
                        FamilyName = "test"
                    }
                };

                e = UserRequestFactory.Add(e);
                id = e.Id;
                System.Threading.Thread.Sleep(5000);

                CSEntryChange cs = CSEntryChange.Create();
                cs.ObjectModificationType = ObjectModificationType.Update;
                cs.DN = dn;

                string newDN = $"{Guid.NewGuid()}@{UnitTestControl.TestParameters.Domain}";

                cs.AttributeChanges.Add(AttributeChange.CreateAttributeUpdate("DN", new List<ValueChange>() { ValueChange.CreateValueAdd(newDN), ValueChange.CreateValueDelete(dn) }));

                cs.AnchorAttributes.Add(AnchorAttribute.Create("id", id));
                CSEntryChangeResult result =
                    ExportProcessor.PutCSEntryChange(cs, UnitTestControl.Schema.GetSchema().Types[SchemaConstants.User]);

                if (result.ErrorCode != MAExportError.Success)
                {
                    Assert.Fail(result.ErrorName);
                }

                System.Threading.Thread.Sleep(5000);
                e = UserRequestFactory.Get(id);
                Assert.AreEqual(newDN, e.PrimaryEmail);
            }
            finally
            {
                if (id != null)
                {
                    UserRequestFactory.Delete(id);
                }
            }

        }

        [TestMethod]
        public void Update()
        {
            string id = null;
            string dn = $"{Guid.NewGuid()}@{UnitTestControl.TestParameters.Domain}";
            User e = new User
            {
                PrimaryEmail = dn,
                Password = Guid.NewGuid().ToString(),
                Name = new UserName
                {
                    GivenName = "gn",
                    FamilyName = "sn"
                }
            };

            e.ExternalIds = new List<ExternalID>();
            e.ExternalIds.Add(new ExternalID() { Type = "work", Value = "test" });
            e.Organizations = new List<Organization>();
            e.Organizations.Add(new Organization()
            {
                Name = "test",
                Title = "test",
                Department = "test",
                Symbol = "test",
                Location = "test",
            });

            e.Phones = new List<Phone>();
            e.Phones.Add(new Phone() { Type = "work", Value = "phwork" });
            e.Phones.Add(new Phone() { Type = "home", Value = "phhome" });


            e = UserRequestFactory.Add(e);
            id = e.Id;

            CSEntryChange cs = CSEntryChange.Create();
            cs.ObjectModificationType = ObjectModificationType.Update;
            cs.DN = dn;
            cs.ObjectType = SchemaConstants.Contact;
            cs.AnchorAttributes.Add(AnchorAttribute.Create("id", id));

            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("orgUnitPath", "/"));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("name_givenName", "gn"));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("name_familyName", "sn"));

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
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("organizations_work_costCenter", "costCenter"));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("organizations_work_location", "location"));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("organizations_work_description", "description"));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("organizations_work_domain", "domain"));

            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("phones_work", "phwork"));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("phones_home", "phhome"));

            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("ims_work_address", "work@ims.com"));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("ims_work_protocol", "proto"));

            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("isAdmin", true));

            string alias1 = $"{Guid.NewGuid()}@{UnitTestControl.TestParameters.Domain}";
            string alias2 = $"{Guid.NewGuid()}@{UnitTestControl.TestParameters.Domain}";

            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("aliases", new List<object>() { alias1, alias2 }));

            try
            {
                CSEntryChangeResult result =
                    ExportProcessor.PutCSEntryChange(cs, UnitTestControl.Schema.GetSchema().Types[SchemaConstants.User]);

                if (result.ErrorCode != MAExportError.Success)
                {
                    Assert.Fail(result.ErrorName);
                }

                System.Threading.Thread.Sleep(5000);

                e = UserRequestFactory.Get(id);
                Assert.AreEqual(cs.DN, e.PrimaryEmail);
                Assert.AreEqual("/", e.OrgUnitPath);
                Assert.AreEqual("gn", e.Name.GivenName);
                Assert.AreEqual("sn", e.Name.FamilyName);

                Assert.AreEqual(true, e.IsAdmin);

                Assert.AreEqual(2, e.ExternalIds.Count);
                Assert.AreEqual("eidwork", e.ExternalIds[0].Value);
                Assert.AreEqual("eidhome", e.ExternalIds[1].Value);

                Assert.AreEqual(1, e.Organizations.Count);
                Assert.AreEqual("name", e.Organizations[0].Name);
                Assert.AreEqual("title", e.Organizations[0].Title);
                Assert.AreEqual("department", e.Organizations[0].Department);
                Assert.AreEqual("symbol", e.Organizations[0].Symbol);
                Assert.AreEqual("costCenter", e.Organizations[0].CostCenter);
                Assert.AreEqual("location", e.Organizations[0].Location);
                Assert.AreEqual("description", e.Organizations[0].Description);
                Assert.AreEqual("domain", e.Organizations[0].Domain);

                Assert.AreEqual(2, e.Phones.Count);
                Assert.AreEqual("phwork", e.Phones[0].Value);
                Assert.AreEqual("phhome", e.Phones[1].Value);

                Assert.AreEqual(1, e.Ims.Count);
                Assert.AreEqual("work@ims.com", e.Ims[0].IMAddress);
                Assert.AreEqual("proto", e.Ims[0].Protocol);

                CollectionAssert.AreEquivalent(new string[] { alias1, alias2 }, e.Aliases);
            }
            finally
            {
                if (id != null)
                {
                    UserRequestFactory.Delete(id);
                }
            }

        }

        [TestMethod]
        public void AddAliases()
        {
            string id = null;
            string dn = $"{Guid.NewGuid()}@{UnitTestControl.TestParameters.Domain}";
            User e = new User
            {
                PrimaryEmail = dn,
                Password = Guid.NewGuid().ToString(),
                Name = new UserName
                {
                    GivenName = "gn",
                    FamilyName = "sn"
                }
            };

            e = UserRequestFactory.Add(e);
            id = e.Id;

            CSEntryChange cs = CSEntryChange.Create();
            cs.ObjectModificationType = ObjectModificationType.Update;
            cs.DN = dn;
            cs.ObjectType = SchemaConstants.Contact;
            cs.AnchorAttributes.Add(AnchorAttribute.Create("id", id));

            string alias1 = $"{Guid.NewGuid()}@{UnitTestControl.TestParameters.Domain}";
            string alias2 = $"{Guid.NewGuid()}@{UnitTestControl.TestParameters.Domain}";

            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("aliases", new List<object>() { alias1, alias2 }));

            try
            {
                CSEntryChangeResult result =
                    ExportProcessor.PutCSEntryChange(cs, UnitTestControl.Schema.GetSchema().Types[SchemaConstants.User]);

                if (result.ErrorCode != MAExportError.Success)
                {
                    Assert.Fail(result.ErrorName);
                }

                System.Threading.Thread.Sleep(5000);

                e = UserRequestFactory.Get(id);

                CollectionAssert.AreEquivalent(new string[] { alias1, alias2 }, e.Aliases);
            }
            finally
            {
                if (id != null)
                {
                    UserRequestFactory.Delete(id);
                }
            }

        }

        [TestMethod]
        public void RemoveAliases()
        {
            string id = null;
            string dn = $"{Guid.NewGuid()}@{UnitTestControl.TestParameters.Domain}";
            User e = new User
            {
                PrimaryEmail = dn,
                Password = Guid.NewGuid().ToString(),
                Name = new UserName
                {
                    GivenName = "gn",
                    FamilyName = "sn"
                }
            };

            e = UserRequestFactory.Add(e);
            id = e.Id;

            string alias1 = $"{Guid.NewGuid()}@{UnitTestControl.TestParameters.Domain}";
            string alias2 = $"{Guid.NewGuid()}@{UnitTestControl.TestParameters.Domain}";

            UserRequestFactory.AddAlias(id, alias1);
            UserRequestFactory.AddAlias(id, alias2);

            CSEntryChange cs = CSEntryChange.Create();
            cs.ObjectModificationType = ObjectModificationType.Update;
            cs.DN = dn;
            cs.ObjectType = SchemaConstants.Contact;
            cs.AnchorAttributes.Add(AnchorAttribute.Create("id", id));

            cs.AttributeChanges.Add(AttributeChange.CreateAttributeDelete("aliases"));

            try
            {
                CSEntryChangeResult result =
                    ExportProcessor.PutCSEntryChange(cs, UnitTestControl.Schema.GetSchema().Types[SchemaConstants.User]);

                if (result.ErrorCode != MAExportError.Success)
                {
                    Assert.Fail(result.ErrorName);
                }

                System.Threading.Thread.Sleep(10000);

                e = UserRequestFactory.Get(id);

                Assert.IsNull(e.Aliases);
            }
            finally
            {
                if (id != null)
                {
                    UserRequestFactory.Delete(id);
                }
            }

        }

        [TestMethod]
        public void AddAlias()
        {
            string id = null;
            string dn = $"{Guid.NewGuid()}@{UnitTestControl.TestParameters.Domain}";
            User e = new User
            {
                PrimaryEmail = dn,
                Password = Guid.NewGuid().ToString(),
                Name = new UserName
                {
                    GivenName = "gn",
                    FamilyName = "sn"
                }
            };

            e = UserRequestFactory.Add(e);
            id = e.Id;

            string alias1 = $"{Guid.NewGuid()}@{UnitTestControl.TestParameters.Domain}";
            string alias2 = $"{Guid.NewGuid()}@{UnitTestControl.TestParameters.Domain}";
            string alias3 = $"{Guid.NewGuid()}@{UnitTestControl.TestParameters.Domain}";

            UserRequestFactory.AddAlias(id, alias1);
            UserRequestFactory.AddAlias(id, alias2);

            CSEntryChange cs = CSEntryChange.Create();
            cs.ObjectModificationType = ObjectModificationType.Update;
            cs.DN = dn;
            cs.ObjectType = SchemaConstants.Contact;
            cs.AnchorAttributes.Add(AnchorAttribute.Create("id", id));

            cs.AttributeChanges.Add(AttributeChange.CreateAttributeUpdate("aliases", new List<ValueChange>
            {
                new ValueChange(alias3, ValueModificationType.Add )
            }));

            try
            {
                CSEntryChangeResult result =
                    ExportProcessor.PutCSEntryChange(cs, UnitTestControl.Schema.GetSchema().Types[SchemaConstants.User]);

                if (result.ErrorCode != MAExportError.Success)
                {
                    Assert.Fail(result.ErrorName);
                }

                System.Threading.Thread.Sleep(5000);

                e = UserRequestFactory.Get(id);

                CollectionAssert.AreEquivalent(new string[] { alias1, alias2, alias3 }, e.Aliases);

            }
            finally
            {
                if (id != null)
                {
                    UserRequestFactory.Delete(id);
                }
            }

        }

        [TestMethod]
        public void RemoveAlias()
        {
            string id = null;
            string dn = $"{Guid.NewGuid()}@{UnitTestControl.TestParameters.Domain}";
            User e = new User
            {
                PrimaryEmail = dn,
                Password = Guid.NewGuid().ToString(),
                Name = new UserName
                {
                    GivenName = "gn",
                    FamilyName = "sn"
                }
            };

            e = UserRequestFactory.Add(e);
            id = e.Id;

            string alias1 = $"{Guid.NewGuid()}@{UnitTestControl.TestParameters.Domain}";
            string alias2 = $"{Guid.NewGuid()}@{UnitTestControl.TestParameters.Domain}";

            UserRequestFactory.AddAlias(id, alias1);
            UserRequestFactory.AddAlias(id, alias2);

            CSEntryChange cs = CSEntryChange.Create();
            cs.ObjectModificationType = ObjectModificationType.Update;
            cs.DN = dn;
            cs.ObjectType = SchemaConstants.Contact;
            cs.AnchorAttributes.Add(AnchorAttribute.Create("id", id));

            cs.AttributeChanges.Add(AttributeChange.CreateAttributeUpdate("aliases", new List<ValueChange>
            {
                new ValueChange(alias2, ValueModificationType.Delete )
            }));

            try
            {
                CSEntryChangeResult result =
                    ExportProcessor.PutCSEntryChange(cs, UnitTestControl.Schema.GetSchema().Types[SchemaConstants.User]);

                if (result.ErrorCode != MAExportError.Success)
                {
                    Assert.Fail(result.ErrorName);
                }

                System.Threading.Thread.Sleep(5000);

                e = UserRequestFactory.Get(id);

                CollectionAssert.AreEquivalent(new string[] { alias1  }, e.Aliases);

            }
            finally
            {
                if (id != null)
                {
                    UserRequestFactory.Delete(id);
                }
            }

        }

        [TestMethod]
        public void ReplaceAliases()
        {
            string id = null;
            string dn = $"{Guid.NewGuid()}@{UnitTestControl.TestParameters.Domain}";
            User e = new User
            {
                PrimaryEmail = dn,
                Password = Guid.NewGuid().ToString(),
                Name = new UserName
                {
                    GivenName = "gn",
                    FamilyName = "sn"
                }
            };

            e = UserRequestFactory.Add(e);
            id = e.Id;

            string alias1 = $"{Guid.NewGuid()}@{UnitTestControl.TestParameters.Domain}";
            string alias2 = $"{Guid.NewGuid()}@{UnitTestControl.TestParameters.Domain}";
            string alias3 = $"{Guid.NewGuid()}@{UnitTestControl.TestParameters.Domain}";
            string alias4 = $"{Guid.NewGuid()}@{UnitTestControl.TestParameters.Domain}";

            UserRequestFactory.AddAlias(id, alias1);
            UserRequestFactory.AddAlias(id, alias2);

            CSEntryChange cs = CSEntryChange.Create();
            cs.ObjectModificationType = ObjectModificationType.Update;
            cs.DN = dn;
            cs.ObjectType = SchemaConstants.Contact;
            cs.AnchorAttributes.Add(AnchorAttribute.Create("id", id));

            cs.AttributeChanges.Add(AttributeChange.CreateAttributeReplace("aliases", new List<object>
            {
               alias3, alias4
            }));

            try
            {
                CSEntryChangeResult result =
                    ExportProcessor.PutCSEntryChange(cs, UnitTestControl.Schema.GetSchema().Types[SchemaConstants.User]);

                if (result.ErrorCode != MAExportError.Success)
                {
                    Assert.Fail(result.ErrorName);
                }

                System.Threading.Thread.Sleep(10000);

                e = UserRequestFactory.Get(id);

                CollectionAssert.AreEquivalent(new string[] { alias3, alias4 }, e.Aliases);
            }
            finally
            {
                if (id != null)
                {
                    UserRequestFactory.Delete(id);
                }
            }

        }
        [TestMethod]
        public void MakeAdmin()
        {
            string id = null;
            string dn = $"{Guid.NewGuid()}@{UnitTestControl.TestParameters.Domain}";
            User e = new User
            {
                PrimaryEmail = dn,
                Password = Guid.NewGuid().ToString(),
                Name = new UserName
                {
                    GivenName = "gn",
                    FamilyName = "sn"
                }
            };

            e = UserRequestFactory.Add(e);
            id = e.Id;

            CSEntryChange cs = CSEntryChange.Create();
            cs.ObjectModificationType = ObjectModificationType.Update;
            cs.DN = dn;
            cs.ObjectType = SchemaConstants.Contact;
            cs.AnchorAttributes.Add(AnchorAttribute.Create("id", id));

            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("isAdmin", true));

            try
            {
                CSEntryChangeResult result =
                    ExportProcessor.PutCSEntryChange(cs, UnitTestControl.Schema.GetSchema().Types[SchemaConstants.User]);

                if (result.ErrorCode != MAExportError.Success)
                {
                    Assert.Fail(result.ErrorName);
                }

                System.Threading.Thread.Sleep(5000);

                e = UserRequestFactory.Get(id);

                Assert.AreEqual(true, e.IsAdmin);
            }
            finally
            {
                if (id != null)
                {
                    UserRequestFactory.Delete(id);
                }
            }

        }

        [TestMethod]
        public void TakeAdmin()
        {
            string id = null;
            string dn = $"{Guid.NewGuid()}@{UnitTestControl.TestParameters.Domain}";
            User e = new User
            {
                PrimaryEmail = dn,
                Password = Guid.NewGuid().ToString(),
                Name = new UserName
                {
                    GivenName = "gn",
                    FamilyName = "sn"
                }
            };

            e = UserRequestFactory.Add(e);
            id = e.Id;
            System.Threading.Thread.Sleep(5000);
            UserRequestFactory.MakeAdmin(true, id);

            CSEntryChange cs = CSEntryChange.Create();
            cs.ObjectModificationType = ObjectModificationType.Update;
            cs.DN = dn;
            cs.ObjectType = SchemaConstants.Contact;
            cs.AnchorAttributes.Add(AnchorAttribute.Create("id", id));

            cs.AttributeChanges.Add(AttributeChange.CreateAttributeReplace("isAdmin", false));

            try
            {
                CSEntryChangeResult result =
                    ExportProcessor.PutCSEntryChange(cs, UnitTestControl.Schema.GetSchema().Types[SchemaConstants.User]);

                if (result.ErrorCode != MAExportError.Success)
                {
                    Assert.Fail(result.ErrorName);
                }

                System.Threading.Thread.Sleep(5000);

                e = UserRequestFactory.Get(id);

                Assert.AreEqual(false, e.IsAdmin);
            }
            finally
            {
                if (id != null)
                {
                    UserRequestFactory.Delete(id);
                }
            }

        }
    }
}
