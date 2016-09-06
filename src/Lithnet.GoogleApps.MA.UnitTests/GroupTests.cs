using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.MetadirectoryServices;
using System.Linq;
using System.Net;
using Google;
using Google.Apis.Admin.Directory.directory_v1.Data;
using Lithnet.GoogleApps.ManagedObjects;
using Lithnet.MetadirectoryServices;

namespace Lithnet.GoogleApps.MA.UnitTests
{
    [TestClass]
    public class GroupTests
    {
        [TestMethod]
        public void Add()
        {
            CSEntryChange cs = CSEntryChange.Create();
            cs.ObjectModificationType = ObjectModificationType.Add;
            cs.DN = $"{Guid.NewGuid()}@{UnitTestControl.TestParameters.Domain}";
            cs.ObjectType = SchemaConstants.Group;

            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("description", "description"));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("name", "name"));

            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("allowExternalMembers", true));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("allowGoogleCommunication", true));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("allowWebPosting", true));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("archiveOnly", false));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("customReplyTo", "test@test.com"));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("customFooterText", "custom footer"));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("defaultMessageDenyNotificationText", "occupation"));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("includeInGlobalAddressList", true));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("isArchived", false));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("includeCustomFooter", true));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("maxMessageBytes", 5000000));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("membersCanPostAsTheGroup", true));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("messageDisplayFont", "DEFAULT_FONT"));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("messageModerationLevel", "MODERATE_NEW_MEMBERS"));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("primaryLanguage", "en-GB"));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("sendMessageDenyNotification", true));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("showInGroupDirectory", true));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("spamModerationLevel", "SILENTLY_MODERATE"));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("whoCanAdd", "ALL_MANAGERS_CAN_ADD"));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("whoCanContactOwner", "ANYONE_CAN_CONTACT"));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("whoCanInvite", "NONE_CAN_INVITE"));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("whoCanJoin", "CAN_REQUEST_TO_JOIN"));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("replyTo", "REPLY_TO_CUSTOM"));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("whoCanPostMessage", "ALL_MANAGERS_CAN_POST"));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("whoCanViewGroup", "ALL_MANAGERS_CAN_VIEW"));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("whoCanViewMembership", "ALL_MEMBERS_CAN_VIEW"));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("whoCanLeaveGroup", "ALL_MANAGERS_CAN_LEAVE"));

            string alias1 = $"{Guid.NewGuid()}@{UnitTestControl.TestParameters.Domain}";
            string alias2 = $"{Guid.NewGuid()}@{UnitTestControl.TestParameters.Domain}";

            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("aliases", new List<object>() { alias1, alias2 }));

            string id = null;

            try
            {
                CSEntryChangeResult result =
                    ExportProcessor.PutCSEntryChange(cs, UnitTestControl.Schema.GetSchema().Types[SchemaConstants.Group]);

                if (result.ErrorCode != MAExportError.Success)
                {
                    Assert.Fail($"{result.ErrorName}\n{result.ErrorDetail}");
                }

                id = result.AnchorAttributes["id"].GetValueAdd<string>();

                Group e = GroupRequestFactory.Get(id);
                Assert.AreEqual(cs.DN, e.Email);

                Assert.AreEqual(true, e.AdminCreated);
                Assert.AreEqual("description", e.Description);
                Assert.AreEqual("name", e.Name);

                GroupSettings s = GroupSettingsRequestFactory.Get(cs.DN);
                Assert.AreEqual(true, s.AllowExternalMembers);
                Assert.AreEqual(true, s.AllowGoogleCommunication);
                Assert.AreEqual(true, s.AllowWebPosting);
                Assert.AreEqual(false, s.ArchiveOnly);
                Assert.AreEqual("test@test.com", s.CustomReplyTo);
                Assert.AreEqual("custom footer", s.CustomFooterText);
                Assert.AreEqual("occupation", s.DefaultMessageDenyNotificationText);
                Assert.AreEqual(true, s.IncludeInGlobalAddressList);
                Assert.AreEqual(true, s.IncludeCustomFooter);
                Assert.AreEqual(false, s.IsArchived);
                Assert.AreEqual(5000000, s.MaxMessageBytes);
                Assert.AreEqual(true, s.MembersCanPostAsTheGroup);
                Assert.AreEqual("DEFAULT_FONT", s.MessageDisplayFont);
                Assert.AreEqual("MODERATE_NEW_MEMBERS", s.MessageModerationLevel);
                Assert.AreEqual("en-GB", s.PrimaryLanguage);
                Assert.AreEqual(true, s.SendMessageDenyNotification);
                Assert.AreEqual(true, s.ShowInGroupDirectory);
                Assert.AreEqual("SILENTLY_MODERATE", s.SpamModerationLevel);
                Assert.AreEqual(true, s.ShowInGroupDirectory);
                Assert.AreEqual("ALL_MANAGERS_CAN_ADD", s.WhoCanAdd);
                Assert.AreEqual("ANYONE_CAN_CONTACT", s.WhoCanContactOwner);
                Assert.AreEqual("NONE_CAN_INVITE", s.WhoCanInvite);
                Assert.AreEqual("CAN_REQUEST_TO_JOIN", s.WhoCanJoin);
                Assert.AreEqual("ALL_MANAGERS_CAN_LEAVE", s.WhoCanLeaveGroup);
                Assert.AreEqual("REPLY_TO_CUSTOM", s.ReplyTo);
                Assert.AreEqual("ALL_MANAGERS_CAN_POST", s.WhoCanPostMessage);
                Assert.AreEqual("ALL_MANAGERS_CAN_VIEW", s.WhoCanViewGroup);
                Assert.AreEqual("ALL_MEMBERS_CAN_VIEW", s.WhoCanViewMembership);

                CollectionAssert.AreEquivalent(new string[] { alias1, alias2 }, e.Aliases.ToArray());
            }
            finally
            {
                if (id != null)
                {
                    GroupRequestFactory.Delete(id);
                    CSEntryChangeQueue.SaveQueue("D:\\temp\\group-add.xml", UnitTestControl.MmsSchema);
                }
            }

        }

        [TestMethod]
        public void Update()
        {
            string id = null;
            string dn = $"{Guid.NewGuid()}@{UnitTestControl.TestParameters.Domain}";
            Group e = new Group
            {
                Email = dn,
                Name = "name"
            };

            e = GroupRequestFactory.Add(e);
            id = e.Id;

            CSEntryChange cs = CSEntryChange.Create();
            cs.ObjectModificationType = ObjectModificationType.Update;
            cs.DN = dn;
            cs.ObjectType = SchemaConstants.Group;
            cs.AnchorAttributes.Add(AnchorAttribute.Create("id", id));

            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("description", "description"));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("allowExternalMembers", true));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("allowGoogleCommunication", true));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("allowWebPosting", true));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("archiveOnly", false));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("customReplyTo", "test@test.com"));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("customFooterText", "custom footer"));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("defaultMessageDenyNotificationText", "occupation"));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("includeInGlobalAddressList", true));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("isArchived", false));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("includeCustomFooter", true));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("maxMessageBytes", 5000000));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("membersCanPostAsTheGroup", true));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("messageDisplayFont", "DEFAULT_FONT"));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("messageModerationLevel", "MODERATE_NEW_MEMBERS"));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("primaryLanguage", "en-GB"));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("sendMessageDenyNotification", true));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("showInGroupDirectory", true));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("spamModerationLevel", "SILENTLY_MODERATE"));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("whoCanAdd", "ALL_MANAGERS_CAN_ADD"));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("whoCanContactOwner", "ANYONE_CAN_CONTACT"));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("whoCanInvite", "NONE_CAN_INVITE"));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("whoCanJoin", "CAN_REQUEST_TO_JOIN"));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("replyTo", "REPLY_TO_CUSTOM"));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("whoCanPostMessage", "ALL_MANAGERS_CAN_POST"));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("whoCanViewGroup", "ALL_MANAGERS_CAN_VIEW"));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("whoCanViewMembership", "ALL_MEMBERS_CAN_VIEW"));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("whoCanLeaveGroup", "ALL_MANAGERS_CAN_LEAVE"));

            string alias1 = $"{Guid.NewGuid()}@{UnitTestControl.TestParameters.Domain}";
            string alias2 = $"{Guid.NewGuid()}@{UnitTestControl.TestParameters.Domain}";

            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("aliases", new List<object>() { alias1, alias2 }));

            try
            {
                CSEntryChangeResult result =
                    ExportProcessor.PutCSEntryChange(cs, UnitTestControl.Schema.GetSchema().Types[SchemaConstants.Group]);

                if (result.ErrorCode != MAExportError.Success)
                {
                    Assert.Fail($"{result.ErrorName}\n{result.ErrorDetail}");
                }

                e = GroupRequestFactory.Get(id);
                Assert.AreEqual(cs.DN, e.Email);

                Assert.AreEqual(true, e.AdminCreated);
                Assert.AreEqual("description", e.Description);
                Assert.AreEqual("name", e.Name);

                GroupSettings s = GroupSettingsRequestFactory.Get(cs.DN);
                Assert.AreEqual(true, s.AllowExternalMembers);
                Assert.AreEqual(true, s.AllowGoogleCommunication);
                Assert.AreEqual(true, s.AllowWebPosting);
                Assert.AreEqual(false, s.ArchiveOnly);
                Assert.AreEqual("test@test.com", s.CustomReplyTo);
                Assert.AreEqual("custom footer", s.CustomFooterText);
                Assert.AreEqual("occupation", s.DefaultMessageDenyNotificationText);
                Assert.AreEqual(true, s.IncludeInGlobalAddressList);
                Assert.AreEqual(true, s.IncludeCustomFooter);
                Assert.AreEqual(false, s.IsArchived);
                Assert.AreEqual(5000000, s.MaxMessageBytes);
                Assert.AreEqual(true, s.MembersCanPostAsTheGroup);
                Assert.AreEqual("DEFAULT_FONT", s.MessageDisplayFont);
                Assert.AreEqual("MODERATE_NEW_MEMBERS", s.MessageModerationLevel);
                Assert.AreEqual("en-GB", s.PrimaryLanguage);
                Assert.AreEqual(true, s.SendMessageDenyNotification);
                Assert.AreEqual(true, s.ShowInGroupDirectory);
                Assert.AreEqual("SILENTLY_MODERATE", s.SpamModerationLevel);
                Assert.AreEqual(true, s.ShowInGroupDirectory);
                Assert.AreEqual("ALL_MANAGERS_CAN_ADD", s.WhoCanAdd);
                Assert.AreEqual("ANYONE_CAN_CONTACT", s.WhoCanContactOwner);
                Assert.AreEqual("NONE_CAN_INVITE", s.WhoCanInvite);
                Assert.AreEqual("CAN_REQUEST_TO_JOIN", s.WhoCanJoin);
                Assert.AreEqual("ALL_MANAGERS_CAN_LEAVE", s.WhoCanLeaveGroup);
                Assert.AreEqual("REPLY_TO_CUSTOM", s.ReplyTo);
                Assert.AreEqual("ALL_MANAGERS_CAN_POST", s.WhoCanPostMessage);
                Assert.AreEqual("ALL_MANAGERS_CAN_VIEW", s.WhoCanViewGroup);
                Assert.AreEqual("ALL_MEMBERS_CAN_VIEW", s.WhoCanViewMembership);

                CollectionAssert.AreEquivalent(new string[] { alias1, alias2 }, e.Aliases.ToArray());

            }
            finally
            {
                if (id != null)
                {
                    GroupRequestFactory.Delete(id);
                    CSEntryChangeQueue.SaveQueue("D:\\temp\\group-update.xml", UnitTestControl.MmsSchema);
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
                Group e = new Group
                {
                    Email = dn,
                    Name = Guid.NewGuid().ToString()
                };

                e = GroupRequestFactory.Add(e);
                id = e.Id;

                CSEntryChange cs = CSEntryChange.Create();
                cs.ObjectModificationType = ObjectModificationType.Delete;
                cs.DN = dn;
                cs.ObjectType = SchemaConstants.Group;
                cs.AnchorAttributes.Add(AnchorAttribute.Create("id", id));
                CSEntryChangeResult result =
                    ExportProcessor.PutCSEntryChange(cs, UnitTestControl.Schema.GetSchema().Types[SchemaConstants.Group]);

                if (result.ErrorCode != MAExportError.Success)
                {
                    Assert.Fail(result.ErrorName);
                }

                try
                {
                    System.Threading.Thread.Sleep(5000);
                    e = GroupRequestFactory.Get(id);
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
                    GroupRequestFactory.Delete(id);
                    CSEntryChangeQueue.SaveQueue("D:\\temp\\group-delete.xml", UnitTestControl.MmsSchema);
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
                Group e = new Group
                {
                    Email = dn,
                    Name = Guid.NewGuid().ToString()
                };

                e = GroupRequestFactory.Add(e);
                id = e.Id;
                System.Threading.Thread.Sleep(5000);

                CSEntryChange cs = CSEntryChange.Create();
                cs.ObjectModificationType = ObjectModificationType.Update;
                cs.ObjectType = SchemaConstants.Group;
                cs.DN = dn;

                string newDN = $"{Guid.NewGuid()}@{UnitTestControl.TestParameters.Domain}";

                cs.AttributeChanges.Add(AttributeChange.CreateAttributeUpdate("DN", new List<ValueChange>() { ValueChange.CreateValueAdd(newDN), ValueChange.CreateValueDelete(dn) }));

                cs.AnchorAttributes.Add(AnchorAttribute.Create("id", id));
                CSEntryChangeResult result =
                    ExportProcessor.PutCSEntryChange(cs, UnitTestControl.Schema.GetSchema().Types[SchemaConstants.Group]);

                if (result.ErrorCode != MAExportError.Success)
                {
                    Assert.Fail(result.ErrorName);
                }

                System.Threading.Thread.Sleep(5000);
                e = GroupRequestFactory.Get(id);
                Assert.AreEqual(newDN, e.Email);
            }
            finally
            {
                if (id != null)
                {
                    GroupRequestFactory.Delete(id);
                    CSEntryChangeQueue.SaveQueue("D:\\temp\\group-rename.xml", UnitTestControl.MmsSchema);
                }
            }

        }

        [TestMethod]
        public void AddAliases()
        {
            string id = null;
            string dn = $"{Guid.NewGuid()}@{UnitTestControl.TestParameters.Domain}";
            Group e = new Group
            {
                Email = dn,
                Name = Guid.NewGuid().ToString()
            };

            e = GroupRequestFactory.Add(e);
            id = e.Id;

            CSEntryChange cs = CSEntryChange.Create();
            cs.ObjectModificationType = ObjectModificationType.Update;
            cs.DN = dn;
            cs.ObjectType = SchemaConstants.Group;
            cs.AnchorAttributes.Add(AnchorAttribute.Create("id", id));

            string alias1 = $"{Guid.NewGuid()}@{UnitTestControl.TestParameters.Domain}";
            string alias2 = $"{Guid.NewGuid()}@{UnitTestControl.TestParameters.Domain}";

            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("aliases", new List<object>() { alias1, alias2 }));

            try
            {
                CSEntryChangeResult result =
                    ExportProcessor.PutCSEntryChange(cs, UnitTestControl.Schema.GetSchema().Types[SchemaConstants.Group]);

                if (result.ErrorCode != MAExportError.Success)
                {
                    Assert.Fail(result.ErrorName);
                }

                System.Threading.Thread.Sleep(5000);

                e = GroupRequestFactory.Get(id);

                CollectionAssert.AreEquivalent(new string[] { alias1, alias2 }, e.Aliases.ToArray());
            }
            finally
            {
                if (id != null)
                {
                    GroupRequestFactory.Delete(id);
                    CSEntryChangeQueue.SaveQueue("D:\\temp\\group-add-aliases.xml", UnitTestControl.MmsSchema);
                }
            }

        }

        [TestMethod]
        public void RemoveAliases()
        {
            string id = null;
            string dn = $"{Guid.NewGuid()}@{UnitTestControl.TestParameters.Domain}";
            Group e = new Group
            {
                Email = dn,
                Name = Guid.NewGuid().ToString()
            };

            e = GroupRequestFactory.Add(e);
            id = e.Id;

            string alias1 = $"{Guid.NewGuid()}@{UnitTestControl.TestParameters.Domain}";
            string alias2 = $"{Guid.NewGuid()}@{UnitTestControl.TestParameters.Domain}";

            GroupRequestFactory.AddAlias(id, alias1);
            GroupRequestFactory.AddAlias(id, alias2);

            CSEntryChange cs = CSEntryChange.Create();
            cs.ObjectModificationType = ObjectModificationType.Update;
            cs.DN = dn;
            cs.ObjectType = SchemaConstants.Group;
            cs.AnchorAttributes.Add(AnchorAttribute.Create("id", id));

            cs.AttributeChanges.Add(AttributeChange.CreateAttributeDelete("aliases"));

            try
            {
                CSEntryChangeResult result =
                    ExportProcessor.PutCSEntryChange(cs, UnitTestControl.Schema.GetSchema().Types[SchemaConstants.Group]);

                if (result.ErrorCode != MAExportError.Success)
                {
                    Assert.Fail(result.ErrorName);
                }

                System.Threading.Thread.Sleep(10000);

                e = GroupRequestFactory.Get(id);

                Assert.IsNull(e.Aliases);
            }
            finally
            {
                if (id != null)
                {
                    GroupRequestFactory.Delete(id);
                }
            }

        }

        [TestMethod]
        public void AddAlias()
        {
            string id = null;
            string dn = $"{Guid.NewGuid()}@{UnitTestControl.TestParameters.Domain}";
            Group e = new Group
            {
                Email = dn,
                Name = Guid.NewGuid().ToString()
            };

            e = GroupRequestFactory.Add(e);
            id = e.Id;

            string alias1 = $"{Guid.NewGuid()}@{UnitTestControl.TestParameters.Domain}";
            string alias2 = $"{Guid.NewGuid()}@{UnitTestControl.TestParameters.Domain}";
            string alias3 = $"{Guid.NewGuid()}@{UnitTestControl.TestParameters.Domain}";

            GroupRequestFactory.AddAlias(id, alias1);
            GroupRequestFactory.AddAlias(id, alias2);

            CSEntryChange cs = CSEntryChange.Create();
            cs.ObjectModificationType = ObjectModificationType.Update;
            cs.DN = dn;
            cs.ObjectType = SchemaConstants.Group;
            cs.AnchorAttributes.Add(AnchorAttribute.Create("id", id));

            cs.AttributeChanges.Add(AttributeChange.CreateAttributeUpdate("aliases", new List<ValueChange>
            {
                new ValueChange(alias3, ValueModificationType.Add )
            }));

            try
            {
                CSEntryChangeResult result =
                    ExportProcessor.PutCSEntryChange(cs, UnitTestControl.Schema.GetSchema().Types[SchemaConstants.Group]);

                if (result.ErrorCode != MAExportError.Success)
                {
                    Assert.Fail(result.ErrorName);
                }

                System.Threading.Thread.Sleep(10000);

                e = GroupRequestFactory.Get(id);

                CollectionAssert.AreEquivalent(new string[] { alias1, alias2, alias3 }, e.Aliases.ToArray());

            }
            finally
            {
                if (id != null)
                {
                    GroupRequestFactory.Delete(id);
                }
            }

        }

        [TestMethod]
        public void RemoveAlias()
        {
            string id = null;
            string dn = $"{Guid.NewGuid()}@{UnitTestControl.TestParameters.Domain}";
            Group e = new Group
            {
                Email = dn,
                Name = Guid.NewGuid().ToString()
            };

            e = GroupRequestFactory.Add(e);
            id = e.Id;

            string alias1 = $"{Guid.NewGuid()}@{UnitTestControl.TestParameters.Domain}";
            string alias2 = $"{Guid.NewGuid()}@{UnitTestControl.TestParameters.Domain}";

            GroupRequestFactory.AddAlias(id, alias1);
            GroupRequestFactory.AddAlias(id, alias2);

            CSEntryChange cs = CSEntryChange.Create();
            cs.ObjectModificationType = ObjectModificationType.Update;
            cs.DN = dn;
            cs.ObjectType = SchemaConstants.Group;
            cs.AnchorAttributes.Add(AnchorAttribute.Create("id", id));

            cs.AttributeChanges.Add(AttributeChange.CreateAttributeUpdate("aliases", new List<ValueChange>
            {
                new ValueChange(alias2, ValueModificationType.Delete )
            }));

            try
            {
                CSEntryChangeResult result =
                    ExportProcessor.PutCSEntryChange(cs, UnitTestControl.Schema.GetSchema().Types[SchemaConstants.Group]);

                if (result.ErrorCode != MAExportError.Success)
                {
                    Assert.Fail(result.ErrorName);
                }

                System.Threading.Thread.Sleep(5000);

                e = GroupRequestFactory.Get(id);

                CollectionAssert.AreEquivalent(new string[] { alias1 }, e.Aliases.ToArray());

            }
            finally
            {
                if (id != null)
                {
                    GroupRequestFactory.Delete(id);
                }
            }

        }

        [TestMethod]
        public void ReplaceAliases()
        {
            string id = null;
            string dn = $"{Guid.NewGuid()}@{UnitTestControl.TestParameters.Domain}";
            Group e = new Group
            {
                Email = dn,
                Name = Guid.NewGuid().ToString()
            };

            e = GroupRequestFactory.Add(e);
            id = e.Id;

            string alias1 = $"{Guid.NewGuid()}@{UnitTestControl.TestParameters.Domain}";
            string alias2 = $"{Guid.NewGuid()}@{UnitTestControl.TestParameters.Domain}";
            string alias3 = $"{Guid.NewGuid()}@{UnitTestControl.TestParameters.Domain}";
            string alias4 = $"{Guid.NewGuid()}@{UnitTestControl.TestParameters.Domain}";

            GroupRequestFactory.AddAlias(id, alias1);
            GroupRequestFactory.AddAlias(id, alias2);

            CSEntryChange cs = CSEntryChange.Create();
            cs.ObjectModificationType = ObjectModificationType.Update;
            cs.DN = dn;
            cs.ObjectType = SchemaConstants.Group;
            cs.AnchorAttributes.Add(AnchorAttribute.Create("id", id));

            cs.AttributeChanges.Add(AttributeChange.CreateAttributeReplace("aliases", new List<object>
            {
               alias3, alias4
            }));

            try
            {
                CSEntryChangeResult result =
                    ExportProcessor.PutCSEntryChange(cs, UnitTestControl.Schema.GetSchema().Types[SchemaConstants.Group]);

                if (result.ErrorCode != MAExportError.Success)
                {
                    Assert.Fail(result.ErrorName);
                }

                System.Threading.Thread.Sleep(10000);

                e = GroupRequestFactory.Get(id);

                CollectionAssert.AreEquivalent(new string[] { alias3, alias4 }, e.Aliases.ToArray());
            }
            finally
            {
                if (id != null)
                {
                    GroupRequestFactory.Delete(id);
                }
            }

        }

        [TestMethod]
        public void AddGroupWithMembers()
        {
            string dn = $"{Guid.NewGuid()}@{UnitTestControl.TestParameters.Domain}";

            CSEntryChange cs = CSEntryChange.Create();
            cs.ObjectModificationType = ObjectModificationType.Add;
            cs.DN = dn;
            cs.ObjectType = SchemaConstants.Group;
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("name", Guid.NewGuid().ToString()));

            Group group1;
            Group group2;
            string member1 = this.CreateGroup(out group1);
            string member2 = this.CreateGroup(out group2);
            string member3 = $"{Guid.NewGuid()}@{UnitTestControl.TestParameters.Domain}";

            ManagedObjects.User e = new ManagedObjects.User
            {
                PrimaryEmail = member3,
                Password = Guid.NewGuid().ToString(),
                Name =
                    {
                        GivenName = "test",
                        FamilyName = "test"
                    }
            };

            e = UserRequestFactory.Add(e);
            
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("member", new List<object>() { member1, member2 }));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("owner", new List<object>() { member3 }));

            CSEntryChangeResult result = null;

            try
            {
                result = ExportProcessor.PutCSEntryChange(cs, UnitTestControl.Schema.GetSchema().Types[SchemaConstants.Group]);

                if (result.ErrorCode != MAExportError.Success)
                {
                    Assert.Fail(result.ErrorName);
                }

                System.Threading.Thread.Sleep(10000);

                CollectionAssert.AreEquivalent(new string[] { member1, member2 }, GroupMemberRequestFactory.GetMembership(cs.DN).Members.ToArray());
            }
            finally
            {
                string id = result?.AnchorAttributes.FirstOrDefault()?.GetValueAdd<string>();

                if (id != null)
                {
                    GroupRequestFactory.Delete(id);
                }

                if (group1?.Id != null)
                {
                    GroupRequestFactory.Delete(group1.Id);
                }

                if (group2?.Id != null)
                {
                    GroupRequestFactory.Delete(group2.Id);
                }

                if (e?.Id != null)
                {
                    UserRequestFactory.Delete(e.Id);
                }
            }
        }

        [TestMethod]
        public void AddMembers()
        {
            string id = null;
            Group e;
            string dn = this.CreateGroup(out e);
            id = e.Id;

            CSEntryChange cs = CSEntryChange.Create();
            cs.ObjectModificationType = ObjectModificationType.Update;
            cs.DN = dn;
            cs.ObjectType = SchemaConstants.Group;
            cs.AnchorAttributes.Add(AnchorAttribute.Create("id", id));

            Group x;
            string member1 = this.CreateGroup(out x);
            string member2 = this.CreateGroup(out x);

            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("member", new List<object>() { member1, member2 }));

            try
            {
                CSEntryChangeResult result =
                    ExportProcessor.PutCSEntryChange(cs, UnitTestControl.Schema.GetSchema().Types[SchemaConstants.Group]);

                if (result.ErrorCode != MAExportError.Success)
                {
                    Assert.Fail(result.ErrorName);
                }

                System.Threading.Thread.Sleep(10000);

                CollectionAssert.AreEquivalent(new string[] { member1, member2 }, GroupMemberRequestFactory.GetMembership(cs.DN).Members.ToArray());
            }
            finally
            {
                if (id != null)
                {
                    GroupRequestFactory.Delete(id);
                }

                GroupRequestFactory.Delete(member1);
                GroupRequestFactory.Delete(member2);
            }

        }

        [TestMethod]
        public void AddMember()
        {
            string id = null;
            Group e;
            string dn = this.CreateGroup(out e);
            id = e.Id;
            Group x;
            string member1 = this.CreateGroup(out x);
            GroupMemberRequestFactory.AddMember(dn, new Member() { Email = member1 });

            System.Threading.Thread.Sleep(10000);

            CSEntryChange cs = CSEntryChange.Create();
            cs.ObjectModificationType = ObjectModificationType.Update;
            cs.DN = dn;
            cs.ObjectType = SchemaConstants.Group;
            cs.AnchorAttributes.Add(AnchorAttribute.Create("id", id));

            string member2 = this.CreateGroup(out x);

            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("member", new List<object>() { member2 }));

            try
            {
                CSEntryChangeResult result =
                    ExportProcessor.PutCSEntryChange(cs, UnitTestControl.Schema.GetSchema().Types[SchemaConstants.Group]);

                if (result.ErrorCode != MAExportError.Success)
                {
                    Assert.Fail(result.ErrorName);
                }

                System.Threading.Thread.Sleep(10000);

                CollectionAssert.AreEquivalent(new string[] { member1, member2 }, GroupMemberRequestFactory.GetMembership(cs.DN).Members.ToArray());
            }
            finally
            {
                if (id != null)
                {
                    GroupRequestFactory.Delete(id);
                }
            }

        }

        [TestMethod]
        public void RemoveMember()
        {
            string id = null;
            Group e;
            string dn = this.CreateGroup(out e);
            id = e.Id;
            Group x;
            string member1 = this.CreateGroup(out x);
            string member2 = this.CreateGroup(out x);
            GroupMemberRequestFactory.AddMember(dn, new Member() { Email = member1 });
            GroupMemberRequestFactory.AddMember(dn, new Member() { Email = member2 });

            System.Threading.Thread.Sleep(10000);

            CSEntryChange cs = CSEntryChange.Create();
            cs.ObjectModificationType = ObjectModificationType.Update;
            cs.DN = dn;
            cs.ObjectType = SchemaConstants.Group;
            cs.AnchorAttributes.Add(AnchorAttribute.Create("id", id));

            cs.AttributeChanges.Add(AttributeChange.CreateAttributeUpdate("member", new List<ValueChange>() { new ValueChange(member2, ValueModificationType.Delete) }));

            try
            {
                CSEntryChangeResult result =
                    ExportProcessor.PutCSEntryChange(cs, UnitTestControl.Schema.GetSchema().Types[SchemaConstants.Group]);

                if (result.ErrorCode != MAExportError.Success)
                {
                    Assert.Fail(result.ErrorName);
                }

                System.Threading.Thread.Sleep(10000);

                CollectionAssert.AreEquivalent(new string[] { member1 }, GroupMemberRequestFactory.GetMembership(cs.DN).Members.ToArray());
            }
            finally
            {
                if (id != null)
                {
                    GroupRequestFactory.Delete(id);
                }
            }

        }

        [TestMethod]
        public void RemoveMembers()
        {
            string id = null;
            Group e;
            string dn = this.CreateGroup(out e);
            id = e.Id;
            Group x;
            string member1 = this.CreateGroup(out x);
            string member2 = this.CreateGroup(out x);
            GroupMemberRequestFactory.AddMember(dn, new Member() { Email = member1 });
            GroupMemberRequestFactory.AddMember(dn, new Member() { Email = member2 });

            System.Threading.Thread.Sleep(10000);

            CSEntryChange cs = CSEntryChange.Create();
            cs.ObjectModificationType = ObjectModificationType.Update;
            cs.DN = dn;
            cs.ObjectType = SchemaConstants.Group;
            cs.AnchorAttributes.Add(AnchorAttribute.Create("id", id));

            cs.AttributeChanges.Add(AttributeChange.CreateAttributeDelete("member"));

            try
            {
                CSEntryChangeResult result =
                    ExportProcessor.PutCSEntryChange(cs, UnitTestControl.Schema.GetSchema().Types[SchemaConstants.Group]);

                if (result.ErrorCode != MAExportError.Success)
                {
                    Assert.Fail(result.ErrorName);
                }

                System.Threading.Thread.Sleep(10000);

                CollectionAssert.AreEquivalent(new string[] { }, GroupMemberRequestFactory.GetMembership(cs.DN).Members.ToArray());
            }
            finally
            {
                if (id != null)
                {
                    GroupRequestFactory.Delete(id);
                }
            }

        }

        [TestMethod]
        public void ReplaceMembers()
        {
            string id = null;
            Group e;
            string dn = this.CreateGroup(out e);
            id = e.Id;

            CSEntryChange cs = CSEntryChange.Create();
            cs.ObjectModificationType = ObjectModificationType.Update;
            cs.DN = dn;
            cs.ObjectType = SchemaConstants.Group;
            cs.AnchorAttributes.Add(AnchorAttribute.Create("id", id));

            Group x;
            string member1 = this.CreateGroup(out x);
            string member2 = this.CreateGroup(out x);
            string member3 = this.CreateGroup(out x);
            string member4 = this.CreateGroup(out x);
            GroupMemberRequestFactory.AddMember(dn, new Member() { Email = member1 });
            GroupMemberRequestFactory.AddMember(dn, new Member() { Email = member2 });

            System.Threading.Thread.Sleep(10000);

            cs.AttributeChanges.Add(AttributeChange.CreateAttributeReplace("member", new List<object>() { member3, member4 }));

            try
            {
                CSEntryChangeResult result =
                    ExportProcessor.PutCSEntryChange(cs, UnitTestControl.Schema.GetSchema().Types[SchemaConstants.Group]);

                if (result.ErrorCode != MAExportError.Success)
                {
                    Assert.Fail(result.ErrorName);
                }

                System.Threading.Thread.Sleep(10000);

                CollectionAssert.AreEquivalent(new string[] { member3, member4 }, GroupMemberRequestFactory.GetMembership(cs.DN).Members.ToArray());
            }
            finally
            {
                if (id != null)
                {
                    GroupRequestFactory.Delete(id);
                }
            }

        }

        private string CreateGroup(out Group e)
        {
            string dn = $"{Guid.NewGuid()}@{UnitTestControl.TestParameters.Domain}";
            e = new Group
            {
                Email = dn,
                Name = Guid.NewGuid().ToString()
            };

            GroupRequestFactory.Add(e);

            return dn;
        }
    }
}
