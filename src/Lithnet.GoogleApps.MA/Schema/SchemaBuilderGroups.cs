using System;
using System.Collections.Generic;
using Microsoft.MetadirectoryServices;

namespace Lithnet.GoogleApps.MA
{
    internal class SchemaBuilderGroups : ISchemaTypeBuilder
    {
        public string TypeName => "group";

        public IEnumerable<MASchemaType> GetSchemaTypes(IManagementAgentParameters config)
        {
            MASchemaType type = new MASchemaType
            {
                AttributeAdapters = new List<IAttributeAdapter>(),
                Name = "group",
                AnchorAttributeNames = new[] { "id" },
                SupportsPatch = true,
            };

            type.ApiInterface = new ApiInterfaceGroup(type, config);

            AdapterPropertyValue adminCreated = new AdapterPropertyValue
            {
                AttributeType = AttributeType.Boolean,
                GoogleApiFieldName = "adminCreated",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportOnly,
                MmsAttributeName = "adminCreated",
                ManagedObjectPropertyName = "AdminCreated",
                Api = "group",
                SupportsPatch = true,
                NullValueRepresentation = NullValueRepresentation.NullPlaceHolder,
                CastForImport = (i) => i ?? false
            };

            type.AttributeAdapters.Add(adminCreated);

            AdapterPropertyValue description = new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                GoogleApiFieldName = "description",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                MmsAttributeName = "description",
                ManagedObjectPropertyName = "Description",
                Api = "group",
                SupportsPatch = true,
                NullValueRepresentation = NullValueRepresentation.EmptyString,
            };

            type.AttributeAdapters.Add(description);

            AdapterPropertyValue email = new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                GoogleApiFieldName = "email",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportOnly,
                MmsAttributeName = "primaryEmail",
                ManagedObjectPropertyName = "Email",
                Api = "group",
                SupportsPatch = true,
            };

            type.AttributeAdapters.Add(email);

            AdapterPropertyValue id = new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                GoogleApiFieldName = "id",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportOnly,
                MmsAttributeName = "id",
                ManagedObjectPropertyName = "Id",
                Api = "group",
                SupportsPatch = true,
                IsAnchor = true,
            };

            type.AttributeAdapters.Add(id);

            AdapterPropertyValue name = new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                GoogleApiFieldName = "name",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                MmsAttributeName = "name",
                ManagedObjectPropertyName = "Name",
                Api = "group",
                SupportsPatch = true,
            };

            type.AttributeAdapters.Add(name);

            SchemaBuilderGroups.AddGroupAliases(type);
            SchemaBuilderGroups.AddGroupSettings(type);

            if (config.MembersAsNonReference)
            {
                SchemaBuilderGroups.AddGroupMembersRaw(type);
            }
            else
            {
                SchemaBuilderGroups.AddGroupMembers(type);
            }

            yield return type;
        }

        private static void AddGroupAliases(MASchemaType type)
        {
            AdapterCollection<string> aliasesList = new AdapterCollection<string>
            {
                Api = "groupaliases",
                MmsAttributeName = "aliases",
                GoogleApiFieldName = "aliases",
                ManagedObjectPropertyName = "Aliases",
                SupportsPatch = true,
                Operation = AttributeOperation.ImportExport
            };

            type.AttributeAdapters.Add(aliasesList);

            AdapterCollection<string> nonEditableAliasesList = new AdapterCollection<string>
            {
                Api = "group",
                MmsAttributeName = "nonEditableAliases",
                GoogleApiFieldName = "nonEditableAliases",
                ManagedObjectPropertyName = "NonEditableAliases",
                SupportsPatch = false,
                Operation = AttributeOperation.ImportOnly
            };

            type.AttributeAdapters.Add(nonEditableAliasesList);
        }


        private static void AddGroupMembersRaw(MASchemaType type)
        {
            AdapterCollection<string> members = new AdapterCollection<string>
            {
                AttributeType = AttributeType.String,
                GoogleApiFieldName = "email",
                Operation = AttributeOperation.ImportExport,
                MmsAttributeName = "member_raw",
                ManagedObjectPropertyName = "Members",
                Api = "groupmembership",
                SupportsPatch = true,
            };

            type.AttributeAdapters.Add(members);

            AdapterCollection<string> managers = new AdapterCollection<string>
            {
                AttributeType = AttributeType.String,
                GoogleApiFieldName = "email",
                Operation = AttributeOperation.ImportExport,
                MmsAttributeName = "manager_raw",
                ManagedObjectPropertyName = "Managers",
                Api = "groupmembership",
                SupportsPatch = true,
            };

            type.AttributeAdapters.Add(managers);

            AdapterCollection<string> owners = new AdapterCollection<string>
            {
                AttributeType = AttributeType.String,
                GoogleApiFieldName = "email",
                Operation = AttributeOperation.ImportExport,
                MmsAttributeName = "owner_raw",
                ManagedObjectPropertyName = "Owners",
                Api = "groupmembership",
                SupportsPatch = true,
            };

            type.AttributeAdapters.Add(owners);
        }

        private static void AddGroupMembers(MASchemaType type)
        {
            AdapterCollection<string> members = new AdapterCollection<string>
            {
                AttributeType = AttributeType.Reference,
                GoogleApiFieldName = "email",
                Operation = AttributeOperation.ImportExport,
                MmsAttributeName = "member",
                ManagedObjectPropertyName = "Members",
                Api = "groupmembership",
                SupportsPatch = true,
            };

            type.AttributeAdapters.Add(members);

            AdapterCollection<string> managers = new AdapterCollection<string>
            {
                AttributeType = AttributeType.Reference,
                GoogleApiFieldName = "email",
                Operation = AttributeOperation.ImportExport,
                MmsAttributeName = "manager",
                ManagedObjectPropertyName = "Managers",
                Api = "groupmembership",
                SupportsPatch = true,
            };

            type.AttributeAdapters.Add(managers);

            AdapterCollection<string> owners = new AdapterCollection<string>
            {
                AttributeType = AttributeType.Reference,
                GoogleApiFieldName = "email",
                Operation = AttributeOperation.ImportExport,
                MmsAttributeName = "owner",
                ManagedObjectPropertyName = "Owners",
                Api = "groupmembership",
                SupportsPatch = true,
            };

            type.AttributeAdapters.Add(owners);

            AdapterCollection<string> externalMembers = new AdapterCollection<string>
            {
                AttributeType = AttributeType.String,
                GoogleApiFieldName = "email",
                Operation = AttributeOperation.ImportExport,
                MmsAttributeName = "externalMember",
                ManagedObjectPropertyName = "ExternalMembers",
                Api = "groupmembership",
                SupportsPatch = true,
            };

            type.AttributeAdapters.Add(externalMembers);

            AdapterCollection<string> externalManagers = new AdapterCollection<string>
            {
                AttributeType = AttributeType.String,
                GoogleApiFieldName = "email",
                Operation = AttributeOperation.ImportExport,
                MmsAttributeName = "externalManager",
                ManagedObjectPropertyName = "ExternalManagers",
                Api = "groupmembership",
                SupportsPatch = true,
            };

            type.AttributeAdapters.Add(externalManagers);

            AdapterCollection<string> externalOwners = new AdapterCollection<string>
            {
                AttributeType = AttributeType.String,
                GoogleApiFieldName = "email",
                Operation = AttributeOperation.ImportExport,
                MmsAttributeName = "externalOwner",
                ManagedObjectPropertyName = "ExternalOwners",
                Api = "groupmembership",
                SupportsPatch = true,
            };

            type.AttributeAdapters.Add(externalOwners);
        }

        private static void AddGroupSettings(MASchemaType type)
        {
            AdapterPropertyValue includeCustomFooter = new AdapterPropertyValue
            {
                AttributeType = AttributeType.Boolean,
                GoogleApiFieldName = "includeCustomFooter",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                MmsAttributeName = "includeCustomFooter",
                ManagedObjectPropertyName = "IncludeCustomFooter",
                Api = "groupsettings",
                SupportsPatch = true,
                NullValueRepresentation = NullValueRepresentation.NullPlaceHolder,
            };

            type.AttributeAdapters.Add(includeCustomFooter);

            AdapterPropertyValue customFooterText = new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                GoogleApiFieldName = "customFooterText",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                MmsAttributeName = "customFooterText",
                ManagedObjectPropertyName = "CustomFooterText",
                Api = "groupsettings",
                SupportsPatch = true,
                NullValueRepresentation = NullValueRepresentation.NullPlaceHolder,
            };

            type.AttributeAdapters.Add(customFooterText);

            AdapterPropertyValue whoCanJoin = new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                GoogleApiFieldName = "whoCanJoin",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                MmsAttributeName = "whoCanJoin",
                ManagedObjectPropertyName = "WhoCanJoin",
                Api = "groupsettings",
                SupportsPatch = true,
                NullValueRepresentation = NullValueRepresentation.NullPlaceHolder,
            };

            type.AttributeAdapters.Add(whoCanJoin);

            AdapterPropertyValue whoCanViewMembership = new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                GoogleApiFieldName = "whoCanViewMembership",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                MmsAttributeName = "whoCanViewMembership",
                ManagedObjectPropertyName = "WhoCanViewMembership",
                Api = "groupsettings",
                SupportsPatch = true,
                NullValueRepresentation = NullValueRepresentation.NullPlaceHolder,
            };

            type.AttributeAdapters.Add(whoCanViewMembership);

            AdapterPropertyValue whoCanViewGroup = new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                GoogleApiFieldName = "whoCanViewGroup",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                MmsAttributeName = "whoCanViewGroup",
                ManagedObjectPropertyName = "WhoCanViewGroup",
                Api = "groupsettings",
                SupportsPatch = true,
                NullValueRepresentation = NullValueRepresentation.NullPlaceHolder,
            };

            type.AttributeAdapters.Add(whoCanViewGroup);

            AdapterPropertyValue whoCanModerateMembers = new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                GoogleApiFieldName = "whoCanModerateMembers",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                MmsAttributeName = "whoCanModerateMembers",
                ManagedObjectPropertyName = "WhoCanModerateMembers",
                Api = "groupsettings",
                SupportsPatch = true,
                NullValueRepresentation = NullValueRepresentation.NullPlaceHolder,
            };

            type.AttributeAdapters.Add(whoCanModerateMembers);

            AdapterPropertyValue allowExternalMembers = new AdapterPropertyValue
            {
                AttributeType = AttributeType.Boolean,
                GoogleApiFieldName = "allowExternalMembers",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                MmsAttributeName = "allowExternalMembers",
                ManagedObjectPropertyName = "AllowExternalMembers",
                Api = "groupsettings",
                SupportsPatch = true,
                NullValueRepresentation = NullValueRepresentation.NullPlaceHolder,
                CastForImport = (i) => i ?? false
            };

            type.AttributeAdapters.Add(allowExternalMembers);

            AdapterPropertyValue whoCanPostMessage = new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                GoogleApiFieldName = "whoCanPostMessage",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                MmsAttributeName = "whoCanPostMessage",
                ManagedObjectPropertyName = "WhoCanPostMessage",
                Api = "groupsettings",
                SupportsPatch = true,
                NullValueRepresentation = NullValueRepresentation.NullPlaceHolder,
            };

            type.AttributeAdapters.Add(whoCanPostMessage);

            AdapterPropertyValue allowWebPosting = new AdapterPropertyValue
            {
                AttributeType = AttributeType.Boolean,
                GoogleApiFieldName = "allowWebPosting",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                MmsAttributeName = "allowWebPosting",
                ManagedObjectPropertyName = "AllowWebPosting",
                Api = "groupsettings",
                SupportsPatch = true,
                NullValueRepresentation = NullValueRepresentation.NullPlaceHolder,
                CastForImport = (i) => i ?? false
            };

            type.AttributeAdapters.Add(allowWebPosting);

            AdapterPropertyValue primaryLanguage = new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                GoogleApiFieldName = "primaryLanguage",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                MmsAttributeName = "primaryLanguage",
                ManagedObjectPropertyName = "PrimaryLanguage",
                Api = "groupsettings",
                SupportsPatch = true,
                NullValueRepresentation = NullValueRepresentation.NullPlaceHolder,
            };

            type.AttributeAdapters.Add(primaryLanguage);

            AdapterPropertyValue isArchived = new AdapterPropertyValue
            {
                AttributeType = AttributeType.Boolean,
                GoogleApiFieldName = "isArchived",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                MmsAttributeName = "isArchived",
                ManagedObjectPropertyName = "IsArchived",
                Api = "groupsettings",
                SupportsPatch = true,
                NullValueRepresentation = NullValueRepresentation.NullPlaceHolder,
                CastForImport = (i) => i ?? false
            };

            type.AttributeAdapters.Add(isArchived);

            AdapterPropertyValue archiveOnly = new AdapterPropertyValue
            {
                AttributeType = AttributeType.Boolean,
                GoogleApiFieldName = "archiveOnly",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportOnly,
                MmsAttributeName = "archiveOnly",
                ManagedObjectPropertyName = "ArchiveOnly",
                Api = "groupsettings",
                SupportsPatch = true,
                NullValueRepresentation = NullValueRepresentation.NullPlaceHolder,
                CastForImport = (i) => i ?? false
            };

            type.AttributeAdapters.Add(archiveOnly);

            AdapterPropertyValue messageModerationLevel = new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                GoogleApiFieldName = "messageModerationLevel",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                MmsAttributeName = "messageModerationLevel",
                ManagedObjectPropertyName = "MessageModerationLevel",
                Api = "groupsettings",
                SupportsPatch = true,
                NullValueRepresentation = NullValueRepresentation.NullPlaceHolder,
            };

            type.AttributeAdapters.Add(messageModerationLevel);


            AdapterPropertyValue spamModerationLevel = new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                GoogleApiFieldName = "spamModerationLevel",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                MmsAttributeName = "spamModerationLevel",
                ManagedObjectPropertyName = "SpamModerationLevel",
                Api = "groupsettings",
                SupportsPatch = true,
                NullValueRepresentation = NullValueRepresentation.NullPlaceHolder,
            };

            type.AttributeAdapters.Add(spamModerationLevel);

            AdapterPropertyValue replyTo = new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                GoogleApiFieldName = "replyTo",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                MmsAttributeName = "replyTo",
                ManagedObjectPropertyName = "ReplyTo",
                Api = "groupsettings",
                SupportsPatch = true,
                NullValueRepresentation = NullValueRepresentation.NullPlaceHolder,
            };

            type.AttributeAdapters.Add(replyTo);

            AdapterPropertyValue customReplyTo = new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                GoogleApiFieldName = "customReplyTo",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                MmsAttributeName = "customReplyTo",
                ManagedObjectPropertyName = "CustomReplyTo",
                Api = "groupsettings",
                SupportsPatch = true,
                NullValueRepresentation = NullValueRepresentation.NullPlaceHolder,
                CastForImport = (i) => string.IsNullOrEmpty((string)i) ? null : i,
            };

            type.AttributeAdapters.Add(customReplyTo);

            AdapterPropertyValue sendMessageDenyNotification = new AdapterPropertyValue
            {
                AttributeType = AttributeType.Boolean,
                GoogleApiFieldName = "sendMessageDenyNotification",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                MmsAttributeName = "sendMessageDenyNotification",
                ManagedObjectPropertyName = "SendMessageDenyNotification",
                Api = "groupsettings",
                SupportsPatch = true,
                NullValueRepresentation = NullValueRepresentation.NullPlaceHolder,
                CastForImport = (i) => i ?? false
            };

            type.AttributeAdapters.Add(sendMessageDenyNotification);
            
            AdapterPropertyValue defaultMessageDenyNotificationText = new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                GoogleApiFieldName = "defaultMessageDenyNotificationText",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                MmsAttributeName = "defaultMessageDenyNotificationText",
                ManagedObjectPropertyName = "DefaultMessageDenyNotificationText",
                Api = "groupsettings",
                SupportsPatch = true,
                NullValueRepresentation = NullValueRepresentation.NullPlaceHolder,
                CastForImport = (i) => string.IsNullOrEmpty((string)i) ? null : i,
            };

            type.AttributeAdapters.Add(defaultMessageDenyNotificationText);

            AdapterPropertyValue membersCanPostAsTheGroup = new AdapterPropertyValue
            {
                AttributeType = AttributeType.Boolean,
                GoogleApiFieldName = "membersCanPostAsTheGroup",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                MmsAttributeName = "membersCanPostAsTheGroup",
                ManagedObjectPropertyName = "MembersCanPostAsTheGroup",
                Api = "groupsettings",
                SupportsPatch = true,
                NullValueRepresentation = NullValueRepresentation.NullPlaceHolder,
            };

            type.AttributeAdapters.Add(membersCanPostAsTheGroup);

            AdapterPropertyValue includeInGlobalAddressList = new AdapterPropertyValue
            {
                AttributeType = AttributeType.Boolean,
                GoogleApiFieldName = "includeInGlobalAddressList",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                MmsAttributeName = "includeInGlobalAddressList",
                ManagedObjectPropertyName = "IncludeInGlobalAddressList",
                Api = "groupsettings",
                SupportsPatch = true,
                NullValueRepresentation = NullValueRepresentation.NullPlaceHolder,
                CastForImport = (i) => i ?? false
            };

            type.AttributeAdapters.Add(includeInGlobalAddressList);

            AdapterPropertyValue whoCanLeaveGroup = new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                GoogleApiFieldName = "whoCanLeaveGroup",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                MmsAttributeName = "whoCanLeaveGroup",
                ManagedObjectPropertyName = "WhoCanLeaveGroup",
                Api = "groupsettings",
                SupportsPatch = true,
                NullValueRepresentation = NullValueRepresentation.NullPlaceHolder,
            };

            type.AttributeAdapters.Add(whoCanLeaveGroup);

            AdapterPropertyValue whoCanContactOwner = new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                GoogleApiFieldName = "whoCanContactOwner",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                MmsAttributeName = "whoCanContactOwner",
                ManagedObjectPropertyName = "WhoCanContactOwner",
                Api = "groupsettings",
                SupportsPatch = true,
                NullValueRepresentation = NullValueRepresentation.NullPlaceHolder,
            };

            type.AttributeAdapters.Add(whoCanContactOwner);

            AdapterPropertyValue whoCanDiscoverGroup = new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                GoogleApiFieldName = "whoCanDiscoverGroup",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                MmsAttributeName = "whoCanDiscoverGroup",
                ManagedObjectPropertyName = "WhoCanDiscoverGroup",
                Api = "groupsettings",
                SupportsPatch = true,
                NullValueRepresentation = NullValueRepresentation.NullPlaceHolder,
            };

            type.AttributeAdapters.Add(whoCanDiscoverGroup);

            AdapterPropertyValue whoCanModerateContent = new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                GoogleApiFieldName = "whoCanModerateContent",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                MmsAttributeName = "whoCanModerateContent",
                ManagedObjectPropertyName = "WhoCanModerateContent",
                Api = "groupsettings",
                SupportsPatch = true,
                NullValueRepresentation = NullValueRepresentation.NullPlaceHolder,
            };

            type.AttributeAdapters.Add(whoCanModerateContent);

            AdapterPropertyValue whoCanAssistContent = new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                GoogleApiFieldName = "whoCanAssistContent",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                MmsAttributeName = "whoCanAssistContent",
                ManagedObjectPropertyName = "WhoCanAssistContent",
                Api = "groupsettings",
                SupportsPatch = true,
                NullValueRepresentation = NullValueRepresentation.NullPlaceHolder,
            };

            type.AttributeAdapters.Add(whoCanAssistContent);

            AdapterPropertyValue enableCollaborativeInbox = new AdapterPropertyValue
            {
                AttributeType = AttributeType.Boolean,
                GoogleApiFieldName = "enableCollaborativeInbox",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                MmsAttributeName = "enableCollaborativeInbox",
                ManagedObjectPropertyName = "EnableCollaborativeInbox",
                Api = "groupsettings",
                SupportsPatch = true,
                NullValueRepresentation = NullValueRepresentation.NullPlaceHolder,
            };

            type.AttributeAdapters.Add(enableCollaborativeInbox);
        }
    }
}
