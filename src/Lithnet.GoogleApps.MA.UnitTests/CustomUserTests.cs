using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using Google;
using Google.Apis.Gmail.v1.Data;
using Lithnet.GoogleApps.ManagedObjects;
using Lithnet.MetadirectoryServices;
using Microsoft.MetadirectoryServices;

namespace Lithnet.GoogleApps.MA.UnitTests
{
    [TestClass]
    public class CustomUserTests
    {

        [TestMethod]
        public void Add()
        {
            CSEntryChange cs = CSEntryChange.Create();
            cs.ObjectModificationType = ObjectModificationType.Add;
            cs.DN = $"{Guid.NewGuid()}@{UnitTestControl.TestParameters.Domain}";
            cs.ObjectType = UnitTestControl.TestUser;

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
                CSEntryChangeResult result = ExportProcessor.PutCSEntryChange(cs, UnitTestControl.Schema.GetSchema().Types[UnitTestControl.TestUser], UnitTestControl.TestParameters);
                id = result.AnchorAttributes["id"].GetValueAdd<string>();


                if (result.ErrorCode != MAExportError.Success)
                {
                    Assert.Fail(result.ErrorName);
                }

                System.Threading.Thread.Sleep(UnitTestControl.PostGoogleOperationSleepInterval);

                User e = UnitTestControl.TestParameters.UsersService.Get(id);
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
                    UnitTestControl.TestParameters.UsersService.Delete(id);
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

                e = UnitTestControl.TestParameters.UsersService.Add(e);
                id = e.Id;

                CSEntryChange cs = CSEntryChange.Create();
                cs.ObjectModificationType = ObjectModificationType.Delete;
                cs.DN = dn;
                cs.ObjectType = UnitTestControl.TestUser;
                cs.AnchorAttributes.Add(AnchorAttribute.Create("id", id));
                CSEntryChangeResult result = ExportProcessor.PutCSEntryChange(cs, UnitTestControl.Schema.GetSchema().Types[UnitTestControl.TestUser], UnitTestControl.TestParameters);

                if (result.ErrorCode != MAExportError.Success)
                {
                    Assert.Fail(result.ErrorName);
                }

                try
                {
                    System.Threading.Thread.Sleep(UnitTestControl.PostGoogleOperationSleepInterval);
                    e = UnitTestControl.TestParameters.UsersService.Get(id);
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
                    UnitTestControl.TestParameters.UsersService.Delete(id);
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

                e = UnitTestControl.TestParameters.UsersService.Add(e);
                id = e.Id;
                System.Threading.Thread.Sleep(2000);

                CSEntryChange cs = CSEntryChange.Create();
                cs.ObjectModificationType = ObjectModificationType.Update;
                cs.ObjectType = UnitTestControl.TestUser;
                cs.DN = dn;

                string newDN = $"{Guid.NewGuid()}@{UnitTestControl.TestParameters.Domain}";

                cs.AttributeChanges.Add(AttributeChange.CreateAttributeUpdate("DN", new List<ValueChange>() { ValueChange.CreateValueAdd(newDN), ValueChange.CreateValueDelete(dn) }));

                cs.AnchorAttributes.Add(AnchorAttribute.Create("id", id));
                CSEntryChangeResult result =
                    ExportProcessor.PutCSEntryChange(cs, UnitTestControl.Schema.GetSchema().Types[UnitTestControl.TestUser], UnitTestControl.TestParameters);

                if (result.ErrorCode != MAExportError.Success)
                {
                    Assert.Fail(result.ErrorName);
                }

                System.Threading.Thread.Sleep(2000);
                e = UnitTestControl.TestParameters.UsersService.Get(id);
                Assert.AreEqual(newDN, e.PrimaryEmail);
            }
            finally
            {
                if (id != null)
                {
                    UnitTestControl.TestParameters.UsersService.Delete(id);
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
                Type = "work"
            });

            e.Phones = new List<Phone>();
            e.Phones.Add(new Phone() { Type = "work", Value = "phwork" });
            e.Phones.Add(new Phone() { Type = "home", Value = "phhome" });


            e = UnitTestControl.TestParameters.UsersService.Add(e);
            id = e.Id;

            CSEntryChange cs = CSEntryChange.Create();
            cs.ObjectModificationType = ObjectModificationType.Update;
            cs.DN = dn;
            cs.ObjectType = UnitTestControl.TestUser;
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
                    ExportProcessor.PutCSEntryChange(cs, UnitTestControl.Schema.GetSchema().Types[UnitTestControl.TestUser], UnitTestControl.TestParameters);

                if (result.ErrorCode != MAExportError.Success)
                {
                    Assert.Fail(result.ErrorName);
                }

                System.Threading.Thread.Sleep(UnitTestControl.PostGoogleOperationSleepInterval);

                e = UnitTestControl.TestParameters.UsersService.Get(id);
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
                    UnitTestControl.TestParameters.UsersService.Delete(id);
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

            e = UnitTestControl.TestParameters.UsersService.Add(e);
            id = e.Id;

            CSEntryChange cs = CSEntryChange.Create();
            cs.ObjectModificationType = ObjectModificationType.Update;
            cs.DN = dn;
            cs.ObjectType = UnitTestControl.TestUser;
            cs.AnchorAttributes.Add(AnchorAttribute.Create("id", id));

            string alias1 = $"{Guid.NewGuid()}@{UnitTestControl.TestParameters.Domain}";
            string alias2 = $"{Guid.NewGuid()}@{UnitTestControl.TestParameters.Domain}";

            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("aliases", new List<object>() { alias1, alias2 }));

            try
            {
                CSEntryChangeResult result =
                    ExportProcessor.PutCSEntryChange(cs, UnitTestControl.Schema.GetSchema().Types[UnitTestControl.TestUser], UnitTestControl.TestParameters);

                if (result.ErrorCode != MAExportError.Success)
                {
                    Assert.Fail(result.ErrorName);
                }

                System.Threading.Thread.Sleep(UnitTestControl.PostGoogleOperationSleepInterval);

                e = UnitTestControl.TestParameters.UsersService.Get(id);

                CollectionAssert.AreEquivalent(new string[] { alias1, alias2 }, e.Aliases);
            }
            finally
            {
                if (id != null)
                {
                    UnitTestControl.TestParameters.UsersService.Delete(id);
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

            e = UnitTestControl.TestParameters.UsersService.Add(e);
            id = e.Id;

            string alias1 = $"{Guid.NewGuid()}@{UnitTestControl.TestParameters.Domain}";
            string alias2 = $"{Guid.NewGuid()}@{UnitTestControl.TestParameters.Domain}";

            UnitTestControl.TestParameters.UsersService.AddAlias(id, alias1);
            UnitTestControl.TestParameters.UsersService.AddAlias(id, alias2);

            CSEntryChange cs = CSEntryChange.Create();
            cs.ObjectModificationType = ObjectModificationType.Update;
            cs.DN = dn;
            cs.ObjectType = UnitTestControl.TestUser;
            cs.AnchorAttributes.Add(AnchorAttribute.Create("id", id));

            cs.AttributeChanges.Add(AttributeChange.CreateAttributeDelete("aliases"));

            try
            {
                CSEntryChangeResult result =
                    ExportProcessor.PutCSEntryChange(cs, UnitTestControl.Schema.GetSchema().Types[UnitTestControl.TestUser], UnitTestControl.TestParameters);

                if (result.ErrorCode != MAExportError.Success)
                {
                    Assert.Fail(result.ErrorName);
                }

                System.Threading.Thread.Sleep(UnitTestControl.PostGoogleOperationSleepInterval);

                e = UnitTestControl.TestParameters.UsersService.Get(id);

                Assert.IsNull(e.Aliases);
            }
            finally
            {
                if (id != null)
                {
                    UnitTestControl.TestParameters.UsersService.Delete(id);
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

            e = UnitTestControl.TestParameters.UsersService.Add(e);
            id = e.Id;

            string alias1 = $"{Guid.NewGuid()}@{UnitTestControl.TestParameters.Domain}";
            string alias2 = $"{Guid.NewGuid()}@{UnitTestControl.TestParameters.Domain}";
            string alias3 = $"{Guid.NewGuid()}@{UnitTestControl.TestParameters.Domain}";

            UnitTestControl.TestParameters.UsersService.AddAlias(id, alias1);
            UnitTestControl.TestParameters.UsersService.AddAlias(id, alias2);

            CSEntryChange cs = CSEntryChange.Create();
            cs.ObjectModificationType = ObjectModificationType.Update;
            cs.DN = dn;
            cs.ObjectType = UnitTestControl.TestUser;
            cs.AnchorAttributes.Add(AnchorAttribute.Create("id", id));

            cs.AttributeChanges.Add(AttributeChange.CreateAttributeUpdate("aliases", new List<ValueChange>
            {
                new ValueChange(alias3, ValueModificationType.Add )
            }));

            try
            {
                CSEntryChangeResult result =
                    ExportProcessor.PutCSEntryChange(cs, UnitTestControl.Schema.GetSchema().Types[UnitTestControl.TestUser], UnitTestControl.TestParameters);

                if (result.ErrorCode != MAExportError.Success)
                {
                    Assert.Fail(result.ErrorName);
                }

                System.Threading.Thread.Sleep(UnitTestControl.PostGoogleOperationSleepInterval);

                e = UnitTestControl.TestParameters.UsersService.Get(id);

                CollectionAssert.AreEquivalent(new string[] { alias1, alias2, alias3 }, e.Aliases);

            }
            finally
            {
                if (id != null)
                {
                    UnitTestControl.TestParameters.UsersService.Delete(id);
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

            e = UnitTestControl.TestParameters.UsersService.Add(e);
            id = e.Id;

            string alias1 = $"{Guid.NewGuid()}@{UnitTestControl.TestParameters.Domain}";
            string alias2 = $"{Guid.NewGuid()}@{UnitTestControl.TestParameters.Domain}";

            UnitTestControl.TestParameters.UsersService.AddAlias(id, alias1);
            UnitTestControl.TestParameters.UsersService.AddAlias(id, alias2);

            CSEntryChange cs = CSEntryChange.Create();
            cs.ObjectModificationType = ObjectModificationType.Update;
            cs.DN = dn;
            cs.ObjectType = UnitTestControl.TestUser;
            cs.AnchorAttributes.Add(AnchorAttribute.Create("id", id));

            cs.AttributeChanges.Add(AttributeChange.CreateAttributeUpdate("aliases", new List<ValueChange>
            {
                new ValueChange(alias2, ValueModificationType.Delete )
            }));

            try
            {
                CSEntryChangeResult result =
                    ExportProcessor.PutCSEntryChange(cs, UnitTestControl.Schema.GetSchema().Types[UnitTestControl.TestUser], UnitTestControl.TestParameters);

                if (result.ErrorCode != MAExportError.Success)
                {
                    Assert.Fail(result.ErrorName);
                }

                System.Threading.Thread.Sleep(2000);

                e = UnitTestControl.TestParameters.UsersService.Get(id);

                CollectionAssert.AreEquivalent(new string[] { alias1 }, e.Aliases);

            }
            finally
            {
                if (id != null)
                {
                    UnitTestControl.TestParameters.UsersService.Delete(id);
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

            e = UnitTestControl.TestParameters.UsersService.Add(e);
            id = e.Id;

            string alias1 = $"{Guid.NewGuid()}@{UnitTestControl.TestParameters.Domain}";
            string alias2 = $"{Guid.NewGuid()}@{UnitTestControl.TestParameters.Domain}";
            string alias3 = $"{Guid.NewGuid()}@{UnitTestControl.TestParameters.Domain}";
            string alias4 = $"{Guid.NewGuid()}@{UnitTestControl.TestParameters.Domain}";

            UnitTestControl.TestParameters.UsersService.AddAlias(id, alias1);
            UnitTestControl.TestParameters.UsersService.AddAlias(id, alias2);

            CSEntryChange cs = CSEntryChange.Create();
            cs.ObjectModificationType = ObjectModificationType.Update;
            cs.DN = dn;
            cs.ObjectType = UnitTestControl.TestUser;
            cs.AnchorAttributes.Add(AnchorAttribute.Create("id", id));

            cs.AttributeChanges.Add(AttributeChange.CreateAttributeReplace("aliases", new List<object>
            {
               alias3, alias4
            }));

            try
            {
                CSEntryChangeResult result =
                    ExportProcessor.PutCSEntryChange(cs, UnitTestControl.Schema.GetSchema().Types[UnitTestControl.TestUser], UnitTestControl.TestParameters);

                if (result.ErrorCode != MAExportError.Success)
                {
                    Assert.Fail(result.ErrorName);
                }

                System.Threading.Thread.Sleep(UnitTestControl.PostGoogleOperationSleepInterval);

                e = UnitTestControl.TestParameters.UsersService.Get(id);

                CollectionAssert.AreEquivalent(new string[] { alias3, alias4 }, e.Aliases);
            }
            finally
            {
                if (id != null)
                {
                    UnitTestControl.TestParameters.UsersService.Delete(id);
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

            e = UnitTestControl.TestParameters.UsersService.Add(e);
            id = e.Id;

            CSEntryChange cs = CSEntryChange.Create();
            cs.ObjectModificationType = ObjectModificationType.Update;
            cs.DN = dn;
            cs.ObjectType = UnitTestControl.TestUser;
            cs.AnchorAttributes.Add(AnchorAttribute.Create("id", id));

            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("isAdmin", true));

            try
            {
                CSEntryChangeResult result =
                    ExportProcessor.PutCSEntryChange(cs, UnitTestControl.Schema.GetSchema().Types[UnitTestControl.TestUser], UnitTestControl.TestParameters);

                if (result.ErrorCode != MAExportError.Success)
                {
                    Assert.Fail(result.ErrorName);
                }

                System.Threading.Thread.Sleep(UnitTestControl.PostGoogleOperationSleepInterval);

                e = UnitTestControl.TestParameters.UsersService.Get(id);

                Assert.AreEqual(true, e.IsAdmin);
            }
            finally
            {
                if (id != null)
                {
                    UnitTestControl.TestParameters.UsersService.Delete(id);
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

            e = UnitTestControl.TestParameters.UsersService.Add(e);
            id = e.Id;
            System.Threading.Thread.Sleep(UnitTestControl.PostGoogleOperationSleepInterval);
            UnitTestControl.TestParameters.UsersService.MakeAdmin(true, id);

            CSEntryChange cs = CSEntryChange.Create();
            cs.ObjectModificationType = ObjectModificationType.Update;
            cs.DN = dn;
            cs.ObjectType = UnitTestControl.TestUser;
            cs.AnchorAttributes.Add(AnchorAttribute.Create("id", id));

            cs.AttributeChanges.Add(AttributeChange.CreateAttributeReplace("isAdmin", false));

            try
            {
                CSEntryChangeResult result =
                    ExportProcessor.PutCSEntryChange(cs, UnitTestControl.Schema.GetSchema().Types[UnitTestControl.TestUser], UnitTestControl.TestParameters);

                if (result.ErrorCode != MAExportError.Success)
                {
                    Assert.Fail(result.ErrorName);
                }

                System.Threading.Thread.Sleep(UnitTestControl.PostGoogleOperationSleepInterval);

                e = UnitTestControl.TestParameters.UsersService.Get(id);

                Assert.AreEqual(false, e.IsAdmin);
            }
            finally
            {
                if (id != null)
                {
                    UnitTestControl.TestParameters.UsersService.Delete(id);
                }
            }

        }

        [TestMethod]
        public void AddDelegates()
        {
            string id = null;
            string dn = this.CreateAdvUser(out User e);
            id = e.Id;

            CSEntryChange cs = CSEntryChange.Create();
            cs.ObjectModificationType = ObjectModificationType.Update;
            cs.DN = dn;
            cs.ObjectType = UnitTestControl.TestUser;
            cs.AnchorAttributes.Add(AnchorAttribute.Create("id", id));

            string delegate1 = this.CreateUser(out User x);
            string delegate2 = this.CreateUser(out x);

            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd($"{UnitTestControl.TestUser}_Delegate", new List<object>() { delegate1, delegate2 }));

            try
            {
                CSEntryChangeResult result =
                    ExportProcessor.PutCSEntryChange(cs, UnitTestControl.Schema.GetSchema().Types[UnitTestControl.TestUser], UnitTestControl.TestParameters);

                if (result.ErrorCode != MAExportError.Success)
                {
                    Assert.Fail(result.ErrorName);
                }

                System.Threading.Thread.Sleep(10000);

                CollectionAssert.AreEquivalent(new string[] { delegate1, delegate2 }, UnitTestControl.TestParameters.GmailService.GetDelegates(cs.DN).ToArray());
            }
            finally
            {
                if (id != null)
                {
                    UnitTestControl.TestParameters.UsersService.Delete(id);
                }
            }

        }

        [TestMethod]
        public void AddDelegate()
        {
            string id = null;
            string dn = this.CreateAdvUser(out User e);
            id = e.Id;
            string delegate1 = this.CreateUser(out User x);

            UnitTestControl.TestParameters.GmailService.AddDelegate(dn, delegate1);

            CSEntryChange cs = CSEntryChange.Create();
            cs.ObjectModificationType = ObjectModificationType.Update;
            cs.DN = dn;
            cs.ObjectType = UnitTestControl.TestUser;
            cs.AnchorAttributes.Add(AnchorAttribute.Create("id", id));

            string delegate2 = this.CreateUser(out x);

            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd($"{UnitTestControl.TestUser}_Delegate", new List<object>() { delegate2 }));

            try
            {
                CSEntryChangeResult result =
                    ExportProcessor.PutCSEntryChange(cs, UnitTestControl.Schema.GetSchema().Types[UnitTestControl.TestUser], UnitTestControl.TestParameters);

                if (result.ErrorCode != MAExportError.Success)
                {
                    Assert.Fail(result.ErrorName);
                }

                System.Threading.Thread.Sleep(10000);

                CollectionAssert.AreEquivalent(new string[] { delegate1, delegate2 }, UnitTestControl.TestParameters.GmailService.GetDelegates(cs.DN).ToArray());
            }
            finally
            {
                if (id != null)
                {
                    UnitTestControl.TestParameters.UsersService.Delete(id);
                }
            }

        }

        [TestMethod]
        public void RemoveDelegate()
        {
            string id = null;
            string dn = this.CreateAdvUser(out User e);
            id = e.Id;
            string delegate1 = this.CreateUser(out User x);
            string delegate2 = this.CreateUser(out x);

            UnitTestControl.TestParameters.GmailService.AddDelegate(dn, delegate1);
            UnitTestControl.TestParameters.GmailService.AddDelegate(dn, delegate2);

            CSEntryChange cs = CSEntryChange.Create();
            cs.ObjectModificationType = ObjectModificationType.Update;
            cs.DN = dn;
            cs.ObjectType = UnitTestControl.TestUser;
            cs.AnchorAttributes.Add(AnchorAttribute.Create("id", id));

            cs.AttributeChanges.Add(AttributeChange.CreateAttributeUpdate($"{UnitTestControl.TestUser}_Delegate", new List<ValueChange>() { new ValueChange(delegate2, ValueModificationType.Delete) }));

            try
            {
                CSEntryChangeResult result =
                    ExportProcessor.PutCSEntryChange(cs, UnitTestControl.Schema.GetSchema().Types[UnitTestControl.TestUser], UnitTestControl.TestParameters);

                if (result.ErrorCode != MAExportError.Success)
                {
                    Assert.Fail(result.ErrorName);
                }

                System.Threading.Thread.Sleep(UnitTestControl.PostGoogleOperationSleepInterval);

                CollectionAssert.AreEquivalent(new string[] { delegate1 }, UnitTestControl.TestParameters.GmailService.GetDelegates(cs.DN).ToArray());
            }
            finally
            {
                if (id != null)
                {
                    UnitTestControl.TestParameters.UsersService.Delete(id);
                }
            }

        }

        [TestMethod]
        public void RemoveDelegates()
        {
            string id = null;
            string dn = this.CreateAdvUser(out User e);
            id = e.Id;
            string delegate1 = this.CreateUser(out User x);
            string delegate2 = this.CreateUser(out x);

            UnitTestControl.TestParameters.GmailService.AddDelegate(dn, delegate1);
            UnitTestControl.TestParameters.GmailService.AddDelegate(dn, delegate2);

            CSEntryChange cs = CSEntryChange.Create();
            cs.ObjectModificationType = ObjectModificationType.Update;
            cs.DN = dn;
            cs.ObjectType = UnitTestControl.TestUser;
            cs.AnchorAttributes.Add(AnchorAttribute.Create("id", id));

            cs.AttributeChanges.Add(AttributeChange.CreateAttributeDelete($"{UnitTestControl.TestUser}_Delegate"));

            try
            {
                CSEntryChangeResult result =
                    ExportProcessor.PutCSEntryChange(cs, UnitTestControl.Schema.GetSchema().Types[UnitTestControl.TestUser], UnitTestControl.TestParameters);

                if (result.ErrorCode != MAExportError.Success)
                {
                    Assert.Fail(result.ErrorName);
                }

                System.Threading.Thread.Sleep(20000);

                CollectionAssert.AreEquivalent(new string[] { }, UnitTestControl.TestParameters.GmailService.GetDelegates(cs.DN)?.ToArray() ?? new string[] { });
            }
            finally
            {
                if (id != null)
                {
                    UnitTestControl.TestParameters.UsersService.Delete(id);
                }
            }
        }

        [TestMethod]
        public void ReplaceDelegates()
        {
            string id = null;
            string dn = this.CreateAdvUser(out User e);
            id = e.Id;

            CSEntryChange cs = CSEntryChange.Create();
            cs.ObjectModificationType = ObjectModificationType.Update;
            cs.DN = dn;
            cs.ObjectType = UnitTestControl.TestUser;
            cs.AnchorAttributes.Add(AnchorAttribute.Create("id", id));

            string delegate1 = this.CreateUser(out User x);
            string delegate2 = this.CreateUser(out x);
            string delegate3 = this.CreateUser(out x);
            string delegate4 = this.CreateUser(out x);

            UnitTestControl.TestParameters.GmailService.AddDelegate(dn, delegate1);
            UnitTestControl.TestParameters.GmailService.AddDelegate(dn, delegate2);

            cs.AttributeChanges.Add(AttributeChange.CreateAttributeReplace($"{UnitTestControl.TestUser}_Delegate", new List<object>() { delegate3, delegate4 }));

            try
            {
                CSEntryChangeResult result =
                    ExportProcessor.PutCSEntryChange(cs, UnitTestControl.Schema.GetSchema().Types[UnitTestControl.TestUser], UnitTestControl.TestParameters);

                if (result.ErrorCode != MAExportError.Success)
                {
                    Assert.Fail(result.ErrorName);
                }

                System.Threading.Thread.Sleep(10000);

                CollectionAssert.AreEquivalent(new string[] { delegate3, delegate4 }, UnitTestControl.TestParameters.GmailService.GetDelegates(cs.DN).ToArray());
            }
            finally
            {
                if (id != null)
                {
                    UnitTestControl.TestParameters.UsersService.Delete(id);
                }
            }

        }



        [TestMethod]
        public void AddSendAsAddresses()
        {
            string id = null;
            string dn = this.CreateAdvUser(out User e);
            id = e.Id;

            CSEntryChange cs = CSEntryChange.Create();
            cs.ObjectModificationType = ObjectModificationType.Update;
            cs.DN = dn;
            cs.ObjectType = UnitTestControl.TestUser;
            cs.AnchorAttributes.Add(AnchorAttribute.Create("id", id));

            string sendAs1 = this.CreateUser(out User x);
            string sendAs2 = this.CreateUser(out x);
            MailAddress user1MailAddress = new MailAddress(sendAs1, "Test User");
            MailAddress user2MailAddress = new MailAddress(sendAs2, "Test User 2");

            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd($"{UnitTestControl.TestUser}_SendAs", new List<object>() { user1MailAddress.ToString(), user2MailAddress.ToString() }));

            try
            {
                CSEntryChangeResult result =
                    ExportProcessor.PutCSEntryChange(cs, UnitTestControl.Schema.GetSchema().Types[UnitTestControl.TestUser], UnitTestControl.TestParameters);

                if (result.ErrorCode != MAExportError.Success)
                {
                    Assert.Fail(result.ErrorName);
                }

                System.Threading.Thread.Sleep(10000);

                CollectionAssert.AreEquivalent(new string[] { user1MailAddress.ToString(), user2MailAddress.ToString() }, this.GetFormattedSendAsResults(cs.DN));
            }
            finally
            {
                if (id != null)
                {
                    UnitTestControl.TestParameters.UsersService.Delete(id);
                }
            }
        }

        private string[] GetFormattedSendAsResults(string dn)
        {
            return UnitTestControl.TestParameters.GmailService.GetSendAs(dn).Where(t => !(t.IsPrimary ?? false)).Select(t => new MailAddress(t.SendAsEmail, t.DisplayName).ToString()).ToArray();
        }

        [TestMethod]
        public void AddSendAsAddress()
        {
            string id = null;
            string dn = this.CreateAdvUser(out User e);
            id = e.Id;
            string sendAs1 = this.CreateUser(out User _);

            MailAddress user1MailAddress = new MailAddress(sendAs1, "Test User");

            UnitTestControl.TestParameters.GmailService.AddSendAs(dn, new SendAs() { DisplayName = user1MailAddress.DisplayName, SendAsEmail = user1MailAddress.Address });

            CSEntryChange cs = CSEntryChange.Create();
            cs.ObjectModificationType = ObjectModificationType.Update;
            cs.DN = dn;
            cs.ObjectType = UnitTestControl.TestUser;
            cs.AnchorAttributes.Add(AnchorAttribute.Create("id", id));

            string sendAs2 = this.CreateUser(out User _);
            MailAddress user2MailAddress = new MailAddress(sendAs2, "Test User 2");

            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd($"{UnitTestControl.TestUser}_SendAs", new List<object>() { user2MailAddress.ToString() }));

            try
            {
                CSEntryChangeResult result = ExportProcessor.PutCSEntryChange(cs, UnitTestControl.Schema.GetSchema().Types[UnitTestControl.TestUser], UnitTestControl.TestParameters);

                if (result.ErrorCode != MAExportError.Success)
                {
                    Assert.Fail(result.ErrorName);
                }

                System.Threading.Thread.Sleep(10000);

                CollectionAssert.AreEquivalent(new string[] { user1MailAddress.ToString(), user2MailAddress.ToString() }, this.GetFormattedSendAsResults(cs.DN));
            }
            finally
            {
                if (id != null)
                {
                    UnitTestControl.TestParameters.UsersService.Delete(id);
                }
            }
        }

        [TestMethod]
        public void RemoveSendAsAddress()
        {
            string id = null;
            string dn = this.CreateAdvUser(out User e);
            id = e.Id;
            string sendAs1 = this.CreateUser(out User x);
            string sendAs2 = this.CreateUser(out x);

            MailAddress user1MailAddress = new MailAddress(sendAs1, "Test User 1");
            MailAddress user2MailAddress = new MailAddress(sendAs2, "Test User 2");

            UnitTestControl.TestParameters.GmailService.AddSendAs(dn, new SendAs() { SendAsEmail = user1MailAddress.Address, DisplayName = user1MailAddress.DisplayName });
            UnitTestControl.TestParameters.GmailService.AddSendAs(dn, new SendAs() { SendAsEmail = user2MailAddress.Address, DisplayName = user2MailAddress.DisplayName });

            CSEntryChange cs = CSEntryChange.Create();
            cs.ObjectModificationType = ObjectModificationType.Update;
            cs.DN = dn;
            cs.ObjectType = UnitTestControl.TestUser;
            cs.AnchorAttributes.Add(AnchorAttribute.Create("id", id));

            cs.AttributeChanges.Add(AttributeChange.CreateAttributeUpdate($"{UnitTestControl.TestUser}_SendAs", new List<ValueChange>()
            {
                new ValueChange(user2MailAddress.ToString(), ValueModificationType.Delete)
            }));

            try
            {
                CSEntryChangeResult result =
                    ExportProcessor.PutCSEntryChange(cs, UnitTestControl.Schema.GetSchema().Types[UnitTestControl.TestUser], UnitTestControl.TestParameters);

                if (result.ErrorCode != MAExportError.Success)
                {
                    Assert.Fail(result.ErrorName);
                }

                System.Threading.Thread.Sleep(UnitTestControl.PostGoogleOperationSleepInterval);

                CollectionAssert.AreEquivalent(new string[] { user1MailAddress.ToString() }, this.GetFormattedSendAsResults(cs.DN));
            }
            finally
            {
                if (id != null)
                {
                    UnitTestControl.TestParameters.UsersService.Delete(id);
                }
            }

        }

        [TestMethod]
        public void RemoveSendAsAddresses()
        {
            string id = null;
            string dn = this.CreateAdvUser(out User e);
            id = e.Id;
            string sendAs1 = this.CreateUser(out User x);
            string sendAs2 = this.CreateUser(out x);

            MailAddress user1MailAddress = new MailAddress(sendAs1, "Test User 1");
            MailAddress user2MailAddress = new MailAddress(sendAs2, "Test User 2");

            UnitTestControl.TestParameters.GmailService.AddSendAs(dn, new SendAs() { SendAsEmail = user1MailAddress.Address, DisplayName = user1MailAddress.DisplayName });
            UnitTestControl.TestParameters.GmailService.AddSendAs(dn, new SendAs() { SendAsEmail = user2MailAddress.Address, DisplayName = user2MailAddress.DisplayName });

            CSEntryChange cs = CSEntryChange.Create();
            cs.ObjectModificationType = ObjectModificationType.Update;
            cs.DN = dn;
            cs.ObjectType = UnitTestControl.TestUser;
            cs.AnchorAttributes.Add(AnchorAttribute.Create("id", id));

            cs.AttributeChanges.Add(AttributeChange.CreateAttributeDelete($"{UnitTestControl.TestUser}_SendAs"));

            try
            {
                CSEntryChangeResult result =
                    ExportProcessor.PutCSEntryChange(cs, UnitTestControl.Schema.GetSchema().Types[UnitTestControl.TestUser],
                        UnitTestControl.TestParameters);

                if (result.ErrorCode != MAExportError.Success)
                {
                    Assert.Fail(result.ErrorName);
                }

                System.Threading.Thread.Sleep(20000);

                CollectionAssert.AreEquivalent(new string[] { }, this.GetFormattedSendAsResults(cs.DN) ?? new string[] { });
            }
            finally
            {
                if (id != null)
                {
                    UnitTestControl.TestParameters.UsersService.Delete(id);
                }
            }
        }

        [TestMethod]
        public void ReplaceSendAsAddresses()
        {
            string id = null;
            string dn = this.CreateAdvUser(out User e);
            id = e.Id;

            CSEntryChange cs = CSEntryChange.Create();
            cs.ObjectModificationType = ObjectModificationType.Update;
            cs.DN = dn;
            cs.ObjectType = UnitTestControl.TestUser;
            cs.AnchorAttributes.Add(AnchorAttribute.Create("id", id));

            string sendAs1 = this.CreateUser(out User x);
            string sendAs2 = this.CreateUser(out x);
            string sendAs3 = this.CreateUser(out x);
            string sendAs4 = this.CreateUser(out x);

            UnitTestControl.TestParameters.GmailService.AddSendAs(dn, sendAs1);
            UnitTestControl.TestParameters.GmailService.AddSendAs(dn, sendAs2);

            cs.AttributeChanges.Add(AttributeChange.CreateAttributeReplace($"{UnitTestControl.TestUser}_SendAs", new List<object>() { sendAs3, sendAs4 }));

            try
            {
                CSEntryChangeResult result =
                    ExportProcessor.PutCSEntryChange(cs, UnitTestControl.Schema.GetSchema().Types[UnitTestControl.TestUser], UnitTestControl.TestParameters);

                if (result.ErrorCode != MAExportError.Success)
                {
                    Assert.Fail(result.ErrorName);
                }

                System.Threading.Thread.Sleep(10000);

                CollectionAssert.AreEquivalent(new string[] { sendAs3, sendAs4 }, this.GetFormattedSendAsResults(cs.DN));
            }
            finally
            {
                if (id != null)
                {
                    UnitTestControl.TestParameters.UsersService.Delete(id);
                }
            }

        }

        private string CreateAdvUser(out User e)
        {
            string dn = $"{Guid.NewGuid()}@{UnitTestControl.TestParameters.Domain}";
            e = new User
            {
                PrimaryEmail = dn,
                Password = Guid.NewGuid().ToString(),
                CustomSchemas = new Dictionary<string, IDictionary<string, object>>
                {
                    {
                        SchemaConstants.CustomGoogleAppsSchemaName, new Dictionary<string, object>
                        {{SchemaConstants.CustomSchemaObjectType, UnitTestControl.TestUser}}
                    }
                },
                Name = new UserName
                {
                    GivenName = "gn",
                    FamilyName = "sn"
                }
            };

            UnitTestControl.TestParameters.UsersService.Add(e);

            return dn;
        }

        private string CreateUser(out User e)
        {
            string dn = $"{Guid.NewGuid()}@{UnitTestControl.TestParameters.Domain}";
            e = new User
            {
                PrimaryEmail = dn,
                Password = Guid.NewGuid().ToString(),
                Name = new UserName
                {
                    GivenName = "gn",
                    FamilyName = "sn"
                }
            };

            UnitTestControl.TestParameters.UsersService.Add(e);
            System.Threading.Thread.Sleep(1000);
            return dn;
        }
    }
}
