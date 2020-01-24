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
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("allowWebPosting", true));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("archiveOnly", false));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("customReplyTo", "test@lithnet.io"));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("customFooterText", "custom footer"));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("defaultMessageDenyNotificationText", "occupation"));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("includeInGlobalAddressList", true));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("isArchived", false));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("includeCustomFooter", true));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("membersCanPostAsTheGroup", true));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("messageModerationLevel", "MODERATE_NEW_MEMBERS"));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("primaryLanguage", "en-GB"));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("sendMessageDenyNotification", true));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("showInGroupDirectory", true));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("spamModerationLevel", "SILENTLY_MODERATE"));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("whoCanModerateMembers", "OWNERS_ONLY"));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("whoCanContactOwner", "ANYONE_CAN_CONTACT"));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("whoCanJoin", "CAN_REQUEST_TO_JOIN"));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("replyTo", "REPLY_TO_CUSTOM"));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("whoCanPostMessage", "ALL_MANAGERS_CAN_POST"));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("whoCanViewGroup", "ALL_MANAGERS_CAN_VIEW"));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("whoCanViewMembership", "ALL_MEMBERS_CAN_VIEW"));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("whoCanLeaveGroup", "ALL_MANAGERS_CAN_LEAVE"));

            string alias1 = $"{Guid.NewGuid()}@{UnitTestControl.TestParameters.Domain}";
            string alias2 = $"{Guid.NewGuid()}@{UnitTestControl.TestParameters.Domain}";

            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("aliases", new List<object>() {alias1, alias2}));

            string id = null;

            try
            {
                CSEntryChangeResult result =
                    ExportProcessor.PutCSEntryChange(cs, UnitTestControl.Schema.GetSchema().Types[SchemaConstants.Group], UnitTestControl.TestParameters);

                if (result.ErrorCode != MAExportError.Success)
                {
                    Assert.Fail($"{result.ErrorName}\n{result.ErrorDetail}");
                }

                id = result.AnchorAttributes["id"].GetValueAdd<string>();

                Group e = UnitTestControl.TestParameters.GroupsService.Get(id);
                Assert.AreEqual(cs.DN, e.Email);

                Assert.AreEqual(true, e.AdminCreated);
                Assert.AreEqual("description", e.Description);
                Assert.AreEqual("name", e.Name);

                GroupSettings s = UnitTestControl.TestParameters.GroupsService.SettingsFactory.Get(cs.DN);
                Assert.AreEqual(true, s.AllowExternalMembers);
                Assert.AreEqual(true, s.AllowWebPosting);
                Assert.AreEqual(false, s.ArchiveOnly);
                Assert.AreEqual("test@lithnet.io", s.CustomReplyTo);
                Assert.AreEqual("custom footer", s.CustomFooterText);
                Assert.AreEqual("occupation", s.DefaultMessageDenyNotificationText);
                Assert.AreEqual(true, s.IncludeInGlobalAddressList);
                Assert.AreEqual(true, s.IncludeCustomFooter);
                Assert.AreEqual(false, s.IsArchived);
                Assert.AreEqual(true, s.MembersCanPostAsTheGroup);
                Assert.AreEqual("MODERATE_NEW_MEMBERS", s.MessageModerationLevel);
                Assert.AreEqual("en-GB", s.PrimaryLanguage);
                Assert.AreEqual(true, s.SendMessageDenyNotification);
                Assert.AreEqual(true, s.ShowInGroupDirectory);
                Assert.AreEqual("SILENTLY_MODERATE", s.SpamModerationLevel);
                Assert.AreEqual(true, s.ShowInGroupDirectory);
                Assert.AreEqual("OWNERS_ONLY", s.WhoCanModerateMembers);
                Assert.AreEqual("ANYONE_CAN_CONTACT", s.WhoCanContactOwner);
                Assert.AreEqual("CAN_REQUEST_TO_JOIN", s.WhoCanJoin);
                Assert.AreEqual("ALL_MANAGERS_CAN_LEAVE", s.WhoCanLeaveGroup);
                Assert.AreEqual("REPLY_TO_CUSTOM", s.ReplyTo);
                Assert.AreEqual("ALL_MANAGERS_CAN_POST", s.WhoCanPostMessage);
                Assert.AreEqual("ALL_MANAGERS_CAN_VIEW", s.WhoCanViewGroup);
                Assert.AreEqual("ALL_MEMBERS_CAN_VIEW", s.WhoCanViewMembership);

                CollectionAssert.AreEquivalent(new string[] {alias1, alias2}, e.Aliases.ToArray());
            }
            finally
            {
                if (id != null)
                {
                    UnitTestControl.TestParameters.GroupsService.Delete(id);
                }
            }
        }

        [TestMethod]
        public void AddWithErrorOnMember()
        {
            CSEntryChange cs = CSEntryChange.Create();
            cs.ObjectModificationType = ObjectModificationType.Add;
            cs.DN = $"{Guid.NewGuid()}@{UnitTestControl.TestParameters.Domain}";
            cs.ObjectType = SchemaConstants.Group;

            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("description", "description"));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("name", "name"));

            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("allowExternalMembers", true));

            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("member", "notanemail"));

            string id = null;

            try
            {
                CSEntryChangeResult result =
                    ExportProcessor.PutCSEntryChange(cs, UnitTestControl.Schema.GetSchema().Types[SchemaConstants.Group], UnitTestControl.TestParameters);

                if (result.ErrorCode != MAExportError.Success)
                {
                    Assert.Fail($"{result.ErrorName}\n{result.ErrorDetail}");
                }

                id = result.AnchorAttributes["id"].GetValueAdd<string>();

                Group e = UnitTestControl.TestParameters.GroupsService.Get(id);
                Assert.AreEqual(cs.DN, e.Email);

                Assert.AreEqual(true, e.AdminCreated);
                Assert.AreEqual("description", e.Description);
                Assert.AreEqual("name", e.Name);

                GroupSettings s = UnitTestControl.TestParameters.GroupsService.SettingsFactory.Get(cs.DN);
                Assert.AreEqual(true, s.AllowExternalMembers);
                Assert.AreEqual(true, s.AllowWebPosting);
                Assert.AreEqual(false, s.ArchiveOnly);
                Assert.AreEqual("test@lithnet.io", s.CustomReplyTo);
                Assert.AreEqual("custom footer", s.CustomFooterText);
                Assert.AreEqual("occupation", s.DefaultMessageDenyNotificationText);
                Assert.AreEqual(true, s.IncludeInGlobalAddressList);
                Assert.AreEqual(true, s.IncludeCustomFooter);
                Assert.AreEqual(false, s.IsArchived);
                Assert.AreEqual(true, s.MembersCanPostAsTheGroup);
                Assert.AreEqual("MODERATE_NEW_MEMBERS", s.MessageModerationLevel);
                Assert.AreEqual("en-GB", s.PrimaryLanguage);
                Assert.AreEqual(true, s.SendMessageDenyNotification);
                Assert.AreEqual(true, s.ShowInGroupDirectory);
                Assert.AreEqual("SILENTLY_MODERATE", s.SpamModerationLevel);
                Assert.AreEqual(true, s.ShowInGroupDirectory);
                Assert.AreEqual("OWNERS_ONLY", s.WhoCanModerateMembers);
                Assert.AreEqual("ANYONE_CAN_CONTACT", s.WhoCanContactOwner);
                Assert.AreEqual("CAN_REQUEST_TO_JOIN", s.WhoCanJoin);
                Assert.AreEqual("ALL_MANAGERS_CAN_LEAVE", s.WhoCanLeaveGroup);
                Assert.AreEqual("REPLY_TO_CUSTOM", s.ReplyTo);
                Assert.AreEqual("ALL_MANAGERS_CAN_POST", s.WhoCanPostMessage);
                Assert.AreEqual("ALL_MANAGERS_CAN_VIEW", s.WhoCanViewGroup);
                Assert.AreEqual("ALL_MEMBERS_CAN_VIEW", s.WhoCanViewMembership);
            }
            finally
            {
                if (id != null)
                {
                    UnitTestControl.TestParameters.GroupsService.Delete(id);
                }
            }
        }

        [TestMethod]
        public void Update()
        {
            Group e = null;

            try
            {
                e = UnitTestControl.CreateGroup();

                CSEntryChange cs = CSEntryChange.Create();
                cs.ObjectModificationType = ObjectModificationType.Update;
                cs.DN = e.Email;
                cs.ObjectType = SchemaConstants.Group;
                cs.AnchorAttributes.Add(AnchorAttribute.Create("id", e.Id));

                cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("name", "name"));
                cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("description", "description"));
                cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("allowExternalMembers", true));
                cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("allowWebPosting", true));
                cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("archiveOnly", false));
                cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("customReplyTo", "test@lithnet.io"));
                cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("customFooterText", "custom footer"));
                cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("defaultMessageDenyNotificationText", "occupation"));
                cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("includeInGlobalAddressList", true));
                cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("isArchived", false));
                cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("includeCustomFooter", true));
                cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("membersCanPostAsTheGroup", true));
                cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("messageModerationLevel", "MODERATE_NEW_MEMBERS"));
                cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("primaryLanguage", "en-GB"));
                cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("sendMessageDenyNotification", true));
                cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("showInGroupDirectory", true));
                cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("spamModerationLevel", "SILENTLY_MODERATE"));
                cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("whoCanModerateMembers", "OWNERS_ONLY"));
                cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("whoCanContactOwner", "ANYONE_CAN_CONTACT"));
                cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("whoCanJoin", "CAN_REQUEST_TO_JOIN"));
                cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("replyTo", "REPLY_TO_CUSTOM"));
                cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("whoCanPostMessage", "ALL_MANAGERS_CAN_POST"));
                cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("whoCanViewGroup", "ALL_MANAGERS_CAN_VIEW"));
                cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("whoCanViewMembership", "ALL_MEMBERS_CAN_VIEW"));
                cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("whoCanLeaveGroup", "ALL_MANAGERS_CAN_LEAVE"));

                string alias1 = $"{Guid.NewGuid()}@{UnitTestControl.TestParameters.Domain}";
                string alias2 = $"{Guid.NewGuid()}@{UnitTestControl.TestParameters.Domain}";

                cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("aliases", new List<object>() {alias1, alias2}));

                CSEntryChangeResult result =
                    ExportProcessor.PutCSEntryChange(cs, UnitTestControl.Schema.GetSchema().Types[SchemaConstants.Group], UnitTestControl.TestParameters);

                if (result.ErrorCode != MAExportError.Success)
                {
                    Assert.Fail($"{result.ErrorName}\n{result.ErrorDetail}");
                }

                e = UnitTestControl.TestParameters.GroupsService.Get(e.Id);
                Assert.AreEqual(cs.DN, e.Email);

                Assert.AreEqual(true, e.AdminCreated);
                Assert.AreEqual("description", e.Description);
                Assert.AreEqual("name", e.Name);

                GroupSettings s = UnitTestControl.TestParameters.GroupsService.SettingsFactory.Get(cs.DN);
                Assert.AreEqual(true, s.AllowExternalMembers);
                Assert.AreEqual(true, s.AllowWebPosting);
                Assert.AreEqual(false, s.ArchiveOnly);
                Assert.AreEqual("test@lithnet.io", s.CustomReplyTo);
                Assert.AreEqual("custom footer", s.CustomFooterText);
                Assert.AreEqual("occupation", s.DefaultMessageDenyNotificationText);
                Assert.AreEqual(true, s.IncludeInGlobalAddressList);
                Assert.AreEqual(true, s.IncludeCustomFooter);
                Assert.AreEqual(false, s.IsArchived);
                Assert.AreEqual(true, s.MembersCanPostAsTheGroup);
                Assert.AreEqual("MODERATE_NEW_MEMBERS", s.MessageModerationLevel);
                Assert.AreEqual("en-GB", s.PrimaryLanguage);
                Assert.AreEqual(true, s.SendMessageDenyNotification);
                Assert.AreEqual(true, s.ShowInGroupDirectory);
                Assert.AreEqual("SILENTLY_MODERATE", s.SpamModerationLevel);
                Assert.AreEqual(true, s.ShowInGroupDirectory);
                Assert.AreEqual("OWNERS_ONLY", s.WhoCanModerateMembers);
                Assert.AreEqual("ANYONE_CAN_CONTACT", s.WhoCanContactOwner);
                Assert.AreEqual("CAN_REQUEST_TO_JOIN", s.WhoCanJoin);
                Assert.AreEqual("ALL_MANAGERS_CAN_LEAVE", s.WhoCanLeaveGroup);
                Assert.AreEqual("REPLY_TO_CUSTOM", s.ReplyTo);
                Assert.AreEqual("ALL_MANAGERS_CAN_POST", s.WhoCanPostMessage);
                Assert.AreEqual("ALL_MANAGERS_CAN_VIEW", s.WhoCanViewGroup);
                Assert.AreEqual("ALL_MEMBERS_CAN_VIEW", s.WhoCanViewMembership);

                CollectionAssert.AreEquivalent(new string[] {alias1, alias2}, e.Aliases.ToArray());
            }
            finally
            {
                UnitTestControl.Cleanup(e);
            }
        }

        [TestMethod]
        public void UpdateDescription()
        {
            Group e = null;

            try
            {
                e = UnitTestControl.CreateGroup();

                CSEntryChange cs = CSEntryChange.Create();
                cs.ObjectModificationType = ObjectModificationType.Update;
                cs.DN = e.Email;
                cs.ObjectType = SchemaConstants.Group;
                cs.AnchorAttributes.Add(AnchorAttribute.Create("id", e.Id));
                cs.AttributeChanges.Add(AttributeChange.CreateAttributeDelete("description"));

                CSEntryChangeResult result =
                    ExportProcessor.PutCSEntryChange(cs, UnitTestControl.Schema.GetSchema().Types[SchemaConstants.Group], UnitTestControl.TestParameters);

                if (result.ErrorCode != MAExportError.Success)
                {
                    Assert.Fail($"{result.ErrorName}\n{result.ErrorDetail}");
                }

                e = UnitTestControl.TestParameters.GroupsService.Get(e.Id);
                Assert.AreEqual(cs.DN, e.Email);
                Assert.AreEqual(true, e.AdminCreated);
                Assert.AreEqual(string.Empty, e.Description);
            }
            finally
            {
                UnitTestControl.Cleanup(e);
            }
        }

        [TestMethod]
        public void UpdateNoneCanPostOn()
        {
            Group e = null;

            try
            {
                e = UnitTestControl.CreateGroup();

                CSEntryChange cs = CSEntryChange.Create();
                cs.ObjectModificationType = ObjectModificationType.Update;
                cs.DN = e.Email;
                cs.ObjectType = SchemaConstants.Group;
                cs.AnchorAttributes.Add(AnchorAttribute.Create("id", e.Id));

                cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("whoCanPostMessage", "NONE_CAN_POST"));
                cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("primaryLanguage", "en-GB"));

                CSEntryChangeResult result =
                    ExportProcessor.PutCSEntryChange(cs, UnitTestControl.Schema.GetSchema().Types[SchemaConstants.Group], UnitTestControl.TestParameters);

                if (result.ErrorCode != MAExportError.Success)
                {
                    Assert.Fail($"{result.ErrorName}\n{result.ErrorDetail}");
                }

                e = UnitTestControl.TestParameters.GroupsService.Get(e.Id);
                Assert.AreEqual(cs.DN, e.Email);

                GroupSettings s = UnitTestControl.TestParameters.GroupsService.SettingsFactory.Get(cs.DN);

                Assert.AreEqual(true, s.ArchiveOnly);

                Assert.AreEqual("NONE_CAN_POST", s.WhoCanPostMessage);

                cs = CSEntryChange.Create();
                cs.ObjectModificationType = ObjectModificationType.Update;
                cs.DN = e.Email;
                cs.ObjectType = SchemaConstants.Group;
                cs.AnchorAttributes.Add(AnchorAttribute.Create("id", e.Id));

                cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("whoCanPostMessage", "ALL_IN_DOMAIN_CAN_POST"));

                result =
                    ExportProcessor.PutCSEntryChange(cs, UnitTestControl.Schema.GetSchema().Types[SchemaConstants.Group], UnitTestControl.TestParameters);

                if (result.ErrorCode != MAExportError.Success)
                {
                    Assert.Fail($"{result.ErrorName}\n{result.ErrorDetail}");
                }

                e = UnitTestControl.TestParameters.GroupsService.Get(e.Id);
                Assert.AreEqual(cs.DN, e.Email);

                s = UnitTestControl.TestParameters.GroupsService.SettingsFactory.Get(cs.DN);

                Assert.AreEqual(false, s.ArchiveOnly);

                Assert.AreEqual("ALL_IN_DOMAIN_CAN_POST", s.WhoCanPostMessage);
            }
            finally
            {
                UnitTestControl.Cleanup(e);
            }
        }

        [TestMethod]
        public void Delete()
        {
            Group e = null;

            try
            {
                e = UnitTestControl.CreateGroup();

                CSEntryChange cs = CSEntryChange.Create();
                cs.ObjectModificationType = ObjectModificationType.Delete;
                cs.DN = e.Email;
                cs.ObjectType = SchemaConstants.Group;
                cs.AnchorAttributes.Add(AnchorAttribute.Create("id", e.Id));
                CSEntryChangeResult result =
                    ExportProcessor.PutCSEntryChange(cs, UnitTestControl.Schema.GetSchema().Types[SchemaConstants.Group], UnitTestControl.TestParameters);

                if (result.ErrorCode != MAExportError.Success)
                {
                    Assert.Fail(result.ErrorName);
                }

                try
                {
                    System.Threading.Thread.Sleep(UnitTestControl.PostGoogleOperationSleepInterval);
                    e = UnitTestControl.TestParameters.GroupsService.Get(e.Id);
                    Assert.Fail("The object did not get deleted");
                }
                catch (GoogleApiException ex)
                {
                    if (ex.HttpStatusCode == HttpStatusCode.NotFound)
                    {
                        e = null;
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            finally
            {
                UnitTestControl.Cleanup(e);
            }
        }

        [TestMethod]
        public void Rename()
        {
            Group e = null;

            try
            {
                e = UnitTestControl.CreateGroup();

                CSEntryChange cs = CSEntryChange.Create();
                cs.ObjectModificationType = ObjectModificationType.Update;
                cs.ObjectType = SchemaConstants.Group;
                cs.DN = e.Email;

                string newDN = $"{Guid.NewGuid()}@{UnitTestControl.TestParameters.Domain}";

                cs.AttributeChanges.Add(AttributeChange.CreateAttributeUpdate("DN", new List<ValueChange>() {ValueChange.CreateValueAdd(newDN), ValueChange.CreateValueDelete(e.Email)}));

                cs.AnchorAttributes.Add(AnchorAttribute.Create("id", e.Id));
                CSEntryChangeResult result =
                    ExportProcessor.PutCSEntryChange(cs, UnitTestControl.Schema.GetSchema().Types[SchemaConstants.Group], UnitTestControl.TestParameters);

                if (result.ErrorCode != MAExportError.Success)
                {
                    Assert.Fail(result.ErrorName);
                }

                System.Threading.Thread.Sleep(UnitTestControl.PostGoogleOperationSleepInterval);
                e = UnitTestControl.TestParameters.GroupsService.Get(e.Id);
                Assert.AreEqual(newDN, e.Email);
            }
            finally
            {
                UnitTestControl.Cleanup(e);
            }
        }

        [TestMethod]
        public void AddAliases()
        {
            Group e = null;

            try
            {
                e = UnitTestControl.CreateGroup();

                CSEntryChange cs = CSEntryChange.Create();
                cs.ObjectModificationType = ObjectModificationType.Update;
                cs.DN = e.Email;
                cs.ObjectType = SchemaConstants.Group;
                cs.AnchorAttributes.Add(AnchorAttribute.Create("id", e.Id));

                string alias1 = $"{Guid.NewGuid()}@{UnitTestControl.TestParameters.Domain}";
                string alias2 = $"{Guid.NewGuid()}@{UnitTestControl.TestParameters.Domain}";

                cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("aliases", new List<object>() {alias1, alias2}));

                CSEntryChangeResult result =
                    ExportProcessor.PutCSEntryChange(cs, UnitTestControl.Schema.GetSchema().Types[SchemaConstants.Group], UnitTestControl.TestParameters);

                if (result.ErrorCode != MAExportError.Success)
                {
                    Assert.Fail(result.ErrorName);
                }

                System.Threading.Thread.Sleep(UnitTestControl.PostGoogleOperationSleepInterval);

                e = UnitTestControl.TestParameters.GroupsService.Get(e.Id);

                CollectionAssert.AreEquivalent(new string[] {alias1, alias2}, e.Aliases.ToArray());
            }
            finally
            {
                UnitTestControl.Cleanup(e);
            }
        }

        [TestMethod]
        public void RemoveAliases()
        {
            Group e = null;

            try
            {
                e = UnitTestControl.CreateGroup();

                string alias1 = $"{Guid.NewGuid()}@{UnitTestControl.TestParameters.Domain}";
                string alias2 = $"{Guid.NewGuid()}@{UnitTestControl.TestParameters.Domain}";

                UnitTestControl.TestParameters.GroupsService.AddAlias(e.Id, alias1);
                UnitTestControl.TestParameters.GroupsService.AddAlias(e.Id, alias2);

                CSEntryChange cs = CSEntryChange.Create();
                cs.ObjectModificationType = ObjectModificationType.Update;
                cs.DN = e.Email;
                cs.ObjectType = SchemaConstants.Group;
                cs.AnchorAttributes.Add(AnchorAttribute.Create("id", e.Id));

                cs.AttributeChanges.Add(AttributeChange.CreateAttributeDelete("aliases"));

                CSEntryChangeResult result =
                    ExportProcessor.PutCSEntryChange(cs, UnitTestControl.Schema.GetSchema().Types[SchemaConstants.Group], UnitTestControl.TestParameters);

                if (result.ErrorCode != MAExportError.Success)
                {
                    Assert.Fail(result.ErrorName);
                }

                System.Threading.Thread.Sleep(UnitTestControl.PostGoogleOperationSleepInterval);

                e = UnitTestControl.TestParameters.GroupsService.Get(e.Id);

                Assert.IsNull(e.Aliases);
            }
            finally
            {
                UnitTestControl.Cleanup(e);
            }
        }

        [TestMethod]
        public void AddAlias()
        {
            Group e = null;

            try
            {
                e = UnitTestControl.CreateGroup();

                string alias1 = $"{Guid.NewGuid()}@{UnitTestControl.TestParameters.Domain}";
                string alias2 = $"{Guid.NewGuid()}@{UnitTestControl.TestParameters.Domain}";
                string alias3 = $"{Guid.NewGuid()}@{UnitTestControl.TestParameters.Domain}";

                UnitTestControl.TestParameters.GroupsService.AddAlias(e.Id, alias1);
                UnitTestControl.TestParameters.GroupsService.AddAlias(e.Id, alias2);

                CSEntryChange cs = CSEntryChange.Create();
                cs.ObjectModificationType = ObjectModificationType.Update;
                cs.DN = e.Email;
                cs.ObjectType = SchemaConstants.Group;
                cs.AnchorAttributes.Add(AnchorAttribute.Create("id", e.Id));

                cs.AttributeChanges.Add(AttributeChange.CreateAttributeUpdate("aliases", new List<ValueChange>
                {
                    new ValueChange(alias3, ValueModificationType.Add)
                }));

                CSEntryChangeResult result =
                    ExportProcessor.PutCSEntryChange(cs, UnitTestControl.Schema.GetSchema().Types[SchemaConstants.Group], UnitTestControl.TestParameters);

                if (result.ErrorCode != MAExportError.Success)
                {
                    Assert.Fail(result.ErrorName);
                }

                System.Threading.Thread.Sleep(UnitTestControl.PostGoogleOperationSleepInterval);

                e = UnitTestControl.TestParameters.GroupsService.Get(e.Id);

                CollectionAssert.AreEquivalent(new string[] {alias1, alias2, alias3}, e.Aliases.ToArray());
            }
            finally
            {
                UnitTestControl.Cleanup(e);
            }
        }

        [TestMethod]
        public void RemoveAlias()
        {
            Group e = null;

            try
            {
                e = UnitTestControl.CreateGroup();

                string alias1 = $"{Guid.NewGuid()}@{UnitTestControl.TestParameters.Domain}";
                string alias2 = $"{Guid.NewGuid()}@{UnitTestControl.TestParameters.Domain}";

                UnitTestControl.TestParameters.GroupsService.AddAlias(e.Id, alias1);
                UnitTestControl.TestParameters.GroupsService.AddAlias(e.Id, alias2);

                CSEntryChange cs = CSEntryChange.Create();
                cs.ObjectModificationType = ObjectModificationType.Update;
                cs.DN = e.Email;
                cs.ObjectType = SchemaConstants.Group;
                cs.AnchorAttributes.Add(AnchorAttribute.Create("id", e.Id));

                cs.AttributeChanges.Add(AttributeChange.CreateAttributeUpdate("aliases", new List<ValueChange>
                {
                    new ValueChange(alias2, ValueModificationType.Delete)
                }));

                CSEntryChangeResult result =
                    ExportProcessor.PutCSEntryChange(cs, UnitTestControl.Schema.GetSchema().Types[SchemaConstants.Group], UnitTestControl.TestParameters);

                if (result.ErrorCode != MAExportError.Success)
                {
                    Assert.Fail(result.ErrorName);
                }

                System.Threading.Thread.Sleep(UnitTestControl.PostGoogleOperationSleepInterval);

                e = UnitTestControl.TestParameters.GroupsService.Get(e.Id);

                CollectionAssert.AreEquivalent(new string[] {alias1}, e.Aliases.ToArray());
            }
            finally
            {
                UnitTestControl.Cleanup(e);
            }
        }

        [TestMethod]
        public void ReplaceAliases()
        {
            Group e = null;

            try
            {
                e = UnitTestControl.CreateGroup();

                string alias1 = $"{Guid.NewGuid()}@{UnitTestControl.TestParameters.Domain}";
                string alias2 = $"{Guid.NewGuid()}@{UnitTestControl.TestParameters.Domain}";
                string alias3 = $"{Guid.NewGuid()}@{UnitTestControl.TestParameters.Domain}";
                string alias4 = $"{Guid.NewGuid()}@{UnitTestControl.TestParameters.Domain}";

                UnitTestControl.TestParameters.GroupsService.AddAlias(e.Id, alias1);
                UnitTestControl.TestParameters.GroupsService.AddAlias(e.Id, alias2);

                CSEntryChange cs = CSEntryChange.Create();
                cs.ObjectModificationType = ObjectModificationType.Update;
                cs.DN = e.Email;
                cs.ObjectType = SchemaConstants.Group;
                cs.AnchorAttributes.Add(AnchorAttribute.Create("id", e.Id));

                cs.AttributeChanges.Add(AttributeChange.CreateAttributeReplace("aliases", new List<object>
                {
                    alias3, alias4
                }));

                CSEntryChangeResult result =
                    ExportProcessor.PutCSEntryChange(cs, UnitTestControl.Schema.GetSchema().Types[SchemaConstants.Group], UnitTestControl.TestParameters);

                if (result.ErrorCode != MAExportError.Success)
                {
                    Assert.Fail(result.ErrorName);
                }

                System.Threading.Thread.Sleep(UnitTestControl.PostGoogleOperationSleepInterval);

                e = UnitTestControl.TestParameters.GroupsService.Get(e.Id);

                CollectionAssert.AreEquivalent(new string[] {alias3, alias4}, e.Aliases.ToArray());
            }
            finally
            {
                UnitTestControl.Cleanup(e);
            }
        }
    }
}
