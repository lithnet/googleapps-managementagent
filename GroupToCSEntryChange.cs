using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.MetadirectoryServices;
using G = Google.Apis.Admin.Directory.directory_v1.Data;
using Lithnet.GoogleApps.ManagedObjects;
using Lithnet.MetadirectoryServices;

namespace Lithnet.GoogleApps.MA
{
    internal static class GroupToCSEntryChange
    {
        public static void GroupCoreToCSEntryChange(G.Group group, SchemaType type, CSEntryChange csentry)
        {
            AttributeModificationType modificationType = csentry.GetSVAttributeModificationType();

            csentry.CreateAttributeChangeIfInSchema(type, "name", modificationType, group.Name);
            csentry.CreateAttributeChangeIfInSchema(type, "primaryEmail", modificationType, group.Email);
            csentry.CreateAttributeChangeIfInSchema(type, "description", modificationType, group.Description);
            csentry.CreateAttributeChangeIfInSchema(type, "adminCreated", modificationType, group.AdminCreated);
        }

        public static void GroupAlisaesToCSEntryChange(G.Group group, SchemaType type, CSEntryChange csentry)
        {
            AttributeModificationType modificationType = csentry.GetMVAttributeModificationType();

            csentry.CreateAttributeChangeIfInSchema(type, "aliases", modificationType, group.Aliases?.ToList<object>());
        }

        public static void GroupSettingsToCSEntryChange(GroupSettings settings, SchemaType type, CSEntryChange csentry)
        {
            AttributeModificationType modificationType = csentry.GetSVAttributeModificationType();

            csentry.CreateAttributeChangeIfInSchema(type, "maxMessageBytes", modificationType, settings.MaxMessageBytes);
            csentry.CreateAttributeChangeIfInSchema(type, "messageDisplayFont", modificationType, settings.MessageDisplayFont);
            csentry.CreateAttributeChangeIfInSchema(type, "messageModerationLevel", modificationType, settings.MessageModerationLevel);
            csentry.CreateAttributeChangeIfInSchema(type, "primaryLanguage", modificationType, settings.PrimaryLanguage);
            csentry.CreateAttributeChangeIfInSchema(type, "replyTo", modificationType, settings.ReplyTo);
            csentry.CreateAttributeChangeIfInSchema(type, "spamModerationLevel", modificationType, settings.SpamModerationLevel);
            csentry.CreateAttributeChangeIfInSchema(type, "whoCanContactOwner", modificationType, settings.WhoCanContactOwner);
            csentry.CreateAttributeChangeIfInSchema(type, "whoCanInvite", modificationType, settings.WhoCanInvite);
            csentry.CreateAttributeChangeIfInSchema(type, "whoCanJoin", modificationType, settings.WhoCanJoin);
            csentry.CreateAttributeChangeIfInSchema(type, "whoCanLeaveGroup", modificationType, settings.WhoCanLeaveGroup);
            csentry.CreateAttributeChangeIfInSchema(type, "whoCanPostMessage", modificationType, settings.WhoCanPostMessage);
            csentry.CreateAttributeChangeIfInSchema(type, "whoCanViewGroup", modificationType, settings.WhoCanViewGroup);
            csentry.CreateAttributeChangeIfInSchema(type, "whoCanViewMembership", modificationType, settings.WhoCanViewMembership);
            csentry.CreateAttributeChangeIfInSchema(type, "includeInGlobalAddressList", modificationType, settings.IncludeInGlobalAddressList);
            csentry.CreateAttributeChangeIfInSchema(type, "allowExternalMembers", modificationType, settings.AllowExternalMembers);
            csentry.CreateAttributeChangeIfInSchema(type, "allowGoogleCommunication", modificationType, settings.AllowGoogleCommunication);
            csentry.CreateAttributeChangeIfInSchema(type, "allowWebPosting", modificationType, settings.AllowWebPosting);
            csentry.CreateAttributeChangeIfInSchema(type, "archiveOnly", modificationType, settings.ArchiveOnly);
            csentry.CreateAttributeChangeIfInSchema(type, "isArchived", modificationType, settings.IsArchived);
            csentry.CreateAttributeChangeIfInSchema(type, "membersCanPostAsTheGroup", modificationType, settings.MembersCanPostAsTheGroup);
            csentry.CreateAttributeChangeIfInSchema(type, "sendMessageDenyNotification", modificationType, settings.SendMessageDenyNotification);
            csentry.CreateAttributeChangeIfInSchema(type, "showInGroupDirectory", modificationType, settings.ShowInGroupDirectory);
            csentry.CreateAttributeChangeIfInSchema(type, "customReplyTo", modificationType, settings.CustomReplyTo);
            csentry.CreateAttributeChangeIfInSchema(type, "defaultMessageDenyNotificationText", modificationType, settings.DefaultMessageDenyNotificationText);
        }

        public static void GroupMembersToCSEntryChange(GroupMembership membership, SchemaType type, CSEntryChange csentry)
        {
            AttributeModificationType modificationType = csentry.GetSVAttributeModificationType();

            csentry.CreateAttributeChangeIfInSchema(type, "member", modificationType, membership.Members.ToList<object>());
            csentry.CreateAttributeChangeIfInSchema(type, "externalMember", modificationType, membership.ExternalMembers.ToList<object>());
            csentry.CreateAttributeChangeIfInSchema(type, "manager", modificationType, membership.Managers.ToList<object>());
            csentry.CreateAttributeChangeIfInSchema(type, "externalManager", modificationType, membership.ExternalManagers.ToList<object>());
            csentry.CreateAttributeChangeIfInSchema(type, "owner", modificationType, membership.Owners.ToList<object>());
            csentry.CreateAttributeChangeIfInSchema(type, "externalOwner", modificationType, membership.ExternalOwners.ToList<object>());
        }
    }
}
