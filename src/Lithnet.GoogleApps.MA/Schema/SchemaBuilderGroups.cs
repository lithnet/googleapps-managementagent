using System;
using System.Collections.Generic;
using Microsoft.MetadirectoryServices;

namespace Lithnet.GoogleApps.MA
{
    internal class SchemaBuilderGroups : ISchemaTypeBuilder
    {
        public string TypeName => "group";

        public MASchemaType GetSchemaType(IManagementAgentParameters config)
        {
            MASchemaType type = new MASchemaType
            {
                AttributeAdapters = new List<IAttributeAdapter>(),
                Name = "group",
                AnchorAttributeNames = new[] { "id" },
                SupportsPatch = true,
            };

            type.ApiInterface = new ApiInterfaceGroup(type);

            AdapterPropertyValue adminCreated = new AdapterPropertyValue
            {
                AttributeType = AttributeType.Boolean,
                FieldName = "adminCreated",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportOnly,
                AttributeName = "adminCreated",
                PropertyName = "AdminCreated",
                Api = "group",
                SupportsPatch = true,
                NullValueRepresentation = NullValueRepresentation.NullPlaceHolder,
                CastForImport = (i) => i ?? false
            };

            type.AttributeAdapters.Add(adminCreated);

            AdapterPropertyValue description = new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                FieldName = "description",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                AttributeName = "description",
                PropertyName = "Description",
                Api = "group",
                SupportsPatch = true,
                NullValueRepresentation = NullValueRepresentation.EmptyString,
            };

            type.AttributeAdapters.Add(description);

            AdapterPropertyValue email = new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                FieldName = "email",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportOnly,
                AttributeName = "primaryEmail",
                PropertyName = "Email",
                Api = "group",
                SupportsPatch = true,
            };

            type.AttributeAdapters.Add(email);

            AdapterPropertyValue id = new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                FieldName = "id",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportOnly,
                AttributeName = "id",
                PropertyName = "Id",
                Api = "group",
                SupportsPatch = true,
                IsAnchor = true,
            };

            type.AttributeAdapters.Add(id);

            AdapterPropertyValue name = new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                FieldName = "name",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                AttributeName = "name",
                PropertyName = "Name",
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

            return type;
        }
        
        private static void AddGroupAliases(MASchemaType type)
        {
            AdapterCollection<string> aliasesList = new AdapterCollection<string>
            {
                Api = "groupaliases",
                AttributeName = "aliases",
                FieldName = "aliases",
                PropertyName = "Aliases",
                SupportsPatch = true,
                Operation = AttributeOperation.ImportExport
            };

            type.AttributeAdapters.Add(aliasesList);

            AdapterCollection<string> nonEditableAliasesList = new AdapterCollection<string>
            {
                Api = "group",
                AttributeName = "nonEditableAliases",
                FieldName = "nonEditableAliases",
                PropertyName = "NonEditableAliases",
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
                FieldName = "email",
                Operation = AttributeOperation.ImportExport,
                AttributeName = "member_raw",
                PropertyName = "Members",
                Api = "groupmembership",
                SupportsPatch = true,
            };

            type.AttributeAdapters.Add(members);

            AdapterCollection<string> managers = new AdapterCollection<string>
            {
                AttributeType = AttributeType.String,
                FieldName = "email",
                Operation = AttributeOperation.ImportExport,
                AttributeName = "manager_raw",
                PropertyName = "Managers",
                Api = "groupmembership",
                SupportsPatch = true,
            };

            type.AttributeAdapters.Add(managers);

            AdapterCollection<string> owners = new AdapterCollection<string>
            {
                AttributeType = AttributeType.String,
                FieldName = "email",
                Operation = AttributeOperation.ImportExport,
                AttributeName = "owner_raw",
                PropertyName = "Owners",
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
                FieldName = "email",
                Operation = AttributeOperation.ImportExport,
                AttributeName = "member",
                PropertyName = "Members",
                Api = "groupmembership",
                SupportsPatch = true,
            };

            type.AttributeAdapters.Add(members);

            AdapterCollection<string> managers = new AdapterCollection<string>
            {
                AttributeType = AttributeType.Reference,
                FieldName = "email",
                Operation = AttributeOperation.ImportExport,
                AttributeName = "manager",
                PropertyName = "Managers",
                Api = "groupmembership",
                SupportsPatch = true,
            };

            type.AttributeAdapters.Add(managers);

            AdapterCollection<string> owners = new AdapterCollection<string>
            {
                AttributeType = AttributeType.Reference,
                FieldName = "email",
                Operation = AttributeOperation.ImportExport,
                AttributeName = "owner",
                PropertyName = "Owners",
                Api = "groupmembership",
                SupportsPatch = true,
            };

            type.AttributeAdapters.Add(owners);

            AdapterCollection<string> externalMembers = new AdapterCollection<string>
            {
                AttributeType = AttributeType.String,
                FieldName = "email",
                Operation = AttributeOperation.ImportExport,
                AttributeName = "externalMember",
                PropertyName = "ExternalMembers",
                Api = "groupmembership",
                SupportsPatch = true,
            };

            type.AttributeAdapters.Add(externalMembers);

            AdapterCollection<string> externalManagers = new AdapterCollection<string>
            {
                AttributeType = AttributeType.String,
                FieldName = "email",
                Operation = AttributeOperation.ImportExport,
                AttributeName = "externalManager",
                PropertyName = "ExternalManagers",
                Api = "groupmembership",
                SupportsPatch = true,
            };

            type.AttributeAdapters.Add(externalManagers);

            AdapterCollection<string> externalOwners = new AdapterCollection<string>
            {
                AttributeType = AttributeType.String,
                FieldName = "email",
                Operation = AttributeOperation.ImportExport,
                AttributeName = "externalOwner",
                PropertyName = "ExternalOwners",
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
                FieldName = "includeCustomFooter",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                AttributeName = "includeCustomFooter",
                PropertyName = "IncludeCustomFooter",
                Api = "groupsettings",
                SupportsPatch = true,
                NullValueRepresentation = NullValueRepresentation.NullPlaceHolder,
            };

            type.AttributeAdapters.Add(includeCustomFooter);

            AdapterPropertyValue customFooterText = new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                FieldName = "customFooterText",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                AttributeName = "customFooterText",
                PropertyName = "CustomFooterText",
                Api = "groupsettings",
                SupportsPatch = true,
                NullValueRepresentation = NullValueRepresentation.NullPlaceHolder,
            };

            type.AttributeAdapters.Add(customFooterText);

            AdapterPropertyValue whoCanJoin = new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                FieldName = "whoCanJoin",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                AttributeName = "whoCanJoin",
                PropertyName = "WhoCanJoin",
                Api = "groupsettings",
                SupportsPatch = true,
                NullValueRepresentation = NullValueRepresentation.NullPlaceHolder,
            };

            type.AttributeAdapters.Add(whoCanJoin);

            AdapterPropertyValue whoCanViewMembership = new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                FieldName = "whoCanViewMembership",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                AttributeName = "whoCanViewMembership",
                PropertyName = "WhoCanViewMembership",
                Api = "groupsettings",
                SupportsPatch = true,
                NullValueRepresentation = NullValueRepresentation.NullPlaceHolder,
            };

            type.AttributeAdapters.Add(whoCanViewMembership);

            AdapterPropertyValue whoCanViewGroup = new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                FieldName = "whoCanViewGroup",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                AttributeName = "whoCanViewGroup",
                PropertyName = "WhoCanViewGroup",
                Api = "groupsettings",
                SupportsPatch = true,
                NullValueRepresentation = NullValueRepresentation.NullPlaceHolder,
            };

            type.AttributeAdapters.Add(whoCanViewGroup);

            AdapterPropertyValue whoCanInvite = new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                FieldName = "whoCanInvite",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                AttributeName = "whoCanInvite",
                PropertyName = "WhoCanInvite",
                Api = "groupsettings",
                SupportsPatch = true,
                NullValueRepresentation = NullValueRepresentation.NullPlaceHolder,
            };

            type.AttributeAdapters.Add(whoCanInvite);

            AdapterPropertyValue whoCanAdd = new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                FieldName = "whoCanAdd",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                AttributeName = "whoCanAdd",
                PropertyName = "WhoCanAdd",
                Api = "groupsettings",
                SupportsPatch = true,
                NullValueRepresentation = NullValueRepresentation.NullPlaceHolder,
            };

            type.AttributeAdapters.Add(whoCanAdd);

            AdapterPropertyValue allowExternalMembers = new AdapterPropertyValue
            {
                AttributeType = AttributeType.Boolean,
                FieldName = "allowExternalMembers",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                AttributeName = "allowExternalMembers",
                PropertyName = "AllowExternalMembers",
                Api = "groupsettings",
                SupportsPatch = true,
                NullValueRepresentation = NullValueRepresentation.NullPlaceHolder,
                CastForImport = (i) => i ?? false
            };

            type.AttributeAdapters.Add(allowExternalMembers);

            AdapterPropertyValue whoCanPostMessage = new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                FieldName = "whoCanPostMessage",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                AttributeName = "whoCanPostMessage",
                PropertyName = "WhoCanPostMessage",
                Api = "groupsettings",
                SupportsPatch = true,
                NullValueRepresentation = NullValueRepresentation.NullPlaceHolder,
            };

            type.AttributeAdapters.Add(whoCanPostMessage);

            AdapterPropertyValue allowWebPosting = new AdapterPropertyValue
            {
                AttributeType = AttributeType.Boolean,
                FieldName = "allowWebPosting",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                AttributeName = "allowWebPosting",
                PropertyName = "AllowWebPosting",
                Api = "groupsettings",
                SupportsPatch = true,
                NullValueRepresentation = NullValueRepresentation.NullPlaceHolder,
                CastForImport = (i) => i ?? false
            };

            type.AttributeAdapters.Add(allowWebPosting);

            AdapterPropertyValue primaryLanguage = new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                FieldName = "primaryLanguage",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                AttributeName = "primaryLanguage",
                PropertyName = "PrimaryLanguage",
                Api = "groupsettings",
                SupportsPatch = true,
                NullValueRepresentation = NullValueRepresentation.NullPlaceHolder,
            };

            type.AttributeAdapters.Add(primaryLanguage);

            AdapterPropertyValue maxMessageBytes = new AdapterPropertyValue
            {
                AttributeType = AttributeType.Integer,
                FieldName = "maxMessageBytes",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                AttributeName = "maxMessageBytes",
                PropertyName = "MaxMessageBytes",
                Api = "groupsettings",
                SupportsPatch = true,
                NullValueRepresentation = NullValueRepresentation.NullPlaceHolder,
                CastForExport = (value) =>
                {
                    if (value == null)
                    {
                        return null;
                    }

                    return Convert.ToInt32((long)value);
                }
            };

            type.AttributeAdapters.Add(maxMessageBytes);

            AdapterPropertyValue isArchived = new AdapterPropertyValue
            {
                AttributeType = AttributeType.Boolean,
                FieldName = "isArchived",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                AttributeName = "isArchived",
                PropertyName = "IsArchived",
                Api = "groupsettings",
                SupportsPatch = true,
                NullValueRepresentation = NullValueRepresentation.NullPlaceHolder,
                CastForImport = (i) => i ?? false
            };

            type.AttributeAdapters.Add(isArchived);


            AdapterPropertyValue archiveOnly = new AdapterPropertyValue
            {
                AttributeType = AttributeType.Boolean,
                FieldName = "archiveOnly",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportOnly,
                AttributeName = "archiveOnly",
                PropertyName = "ArchiveOnly",
                Api = "groupsettings",
                SupportsPatch = true,
                NullValueRepresentation = NullValueRepresentation.NullPlaceHolder,
                CastForImport = (i) => i ?? false
            };

            type.AttributeAdapters.Add(archiveOnly);

            AdapterPropertyValue messageModerationLevel = new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                FieldName = "messageModerationLevel",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                AttributeName = "messageModerationLevel",
                PropertyName = "MessageModerationLevel",
                Api = "groupsettings",
                SupportsPatch = true,
                NullValueRepresentation = NullValueRepresentation.NullPlaceHolder,
            };

            type.AttributeAdapters.Add(messageModerationLevel);


            AdapterPropertyValue spamModerationLevel = new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                FieldName = "spamModerationLevel",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                AttributeName = "spamModerationLevel",
                PropertyName = "SpamModerationLevel",
                Api = "groupsettings",
                SupportsPatch = true,
                NullValueRepresentation = NullValueRepresentation.NullPlaceHolder,
            };

            type.AttributeAdapters.Add(spamModerationLevel);

            AdapterPropertyValue replyTo = new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                FieldName = "replyTo",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                AttributeName = "replyTo",
                PropertyName = "ReplyTo",
                Api = "groupsettings",
                SupportsPatch = true,
                NullValueRepresentation = NullValueRepresentation.NullPlaceHolder,
            };

            type.AttributeAdapters.Add(replyTo);

            AdapterPropertyValue customReplyTo = new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                FieldName = "customReplyTo",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                AttributeName = "customReplyTo",
                PropertyName = "CustomReplyTo",
                Api = "groupsettings",
                SupportsPatch = true,
                NullValueRepresentation = NullValueRepresentation.NullPlaceHolder,
            };

            type.AttributeAdapters.Add(customReplyTo);

            AdapterPropertyValue sendMessageDenyNotification = new AdapterPropertyValue
            {
                AttributeType = AttributeType.Boolean,
                FieldName = "sendMessageDenyNotification",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                AttributeName = "sendMessageDenyNotification",
                PropertyName = "SendMessageDenyNotification",
                Api = "groupsettings",
                SupportsPatch = true,
                NullValueRepresentation = NullValueRepresentation.NullPlaceHolder,
                CastForImport = (i) => i ?? false
            };

            type.AttributeAdapters.Add(sendMessageDenyNotification);


            AdapterPropertyValue defaultMessageDenyNotificationText = new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                FieldName = "defaultMessageDenyNotificationText",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                AttributeName = "defaultMessageDenyNotificationText",
                PropertyName = "DefaultMessageDenyNotificationText",
                Api = "groupsettings",
                SupportsPatch = true,
                NullValueRepresentation = NullValueRepresentation.NullPlaceHolder,
            };

            type.AttributeAdapters.Add(defaultMessageDenyNotificationText);

            AdapterPropertyValue showInGroupDirectory = new AdapterPropertyValue
            {
                AttributeType = AttributeType.Boolean,
                FieldName = "showInGroupDirectory",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                AttributeName = "showInGroupDirectory",
                PropertyName = "ShowInGroupDirectory",
                Api = "groupsettings",
                SupportsPatch = true,
                NullValueRepresentation = NullValueRepresentation.NullPlaceHolder,
                CastForImport = (i) => i ?? false
            };

            type.AttributeAdapters.Add(showInGroupDirectory);


            AdapterPropertyValue allowGoogleCommunication = new AdapterPropertyValue
            {
                AttributeType = AttributeType.Boolean,
                FieldName = "allowGoogleCommunication",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                AttributeName = "allowGoogleCommunication",
                PropertyName = "AllowGoogleCommunication",
                Api = "groupsettings",
                SupportsPatch = true,
                NullValueRepresentation = NullValueRepresentation.NullPlaceHolder,
                CastForImport = (i) => i ?? false
            };

            type.AttributeAdapters.Add(allowGoogleCommunication);

            AdapterPropertyValue membersCanPostAsTheGroup = new AdapterPropertyValue
            {
                AttributeType = AttributeType.Boolean,
                FieldName = "membersCanPostAsTheGroup",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                AttributeName = "membersCanPostAsTheGroup",
                PropertyName = "MembersCanPostAsTheGroup",
                Api = "groupsettings",
                SupportsPatch = true,
                NullValueRepresentation = NullValueRepresentation.NullPlaceHolder,
            };

            type.AttributeAdapters.Add(membersCanPostAsTheGroup);

            AdapterPropertyValue messageDisplayFont = new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                FieldName = "messageDisplayFont",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                AttributeName = "messageDisplayFont",
                PropertyName = "MessageDisplayFont",
                Api = "groupsettings",
                SupportsPatch = true,
                NullValueRepresentation = NullValueRepresentation.NullPlaceHolder,
            };

            type.AttributeAdapters.Add(messageDisplayFont);

            AdapterPropertyValue includeInGlobalAddressList = new AdapterPropertyValue
            {
                AttributeType = AttributeType.Boolean,
                FieldName = "includeInGlobalAddressList",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                AttributeName = "includeInGlobalAddressList",
                PropertyName = "IncludeInGlobalAddressList",
                Api = "groupsettings",
                SupportsPatch = true,
                NullValueRepresentation = NullValueRepresentation.NullPlaceHolder,
                CastForImport = (i) => i ?? false
            };

            type.AttributeAdapters.Add(includeInGlobalAddressList);

            AdapterPropertyValue whoCanLeaveGroup = new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                FieldName = "whoCanLeaveGroup",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                AttributeName = "whoCanLeaveGroup",
                PropertyName = "WhoCanLeaveGroup",
                Api = "groupsettings",
                SupportsPatch = true,
                NullValueRepresentation = NullValueRepresentation.NullPlaceHolder,
            };

            type.AttributeAdapters.Add(whoCanLeaveGroup);


            AdapterPropertyValue whoCanContactOwner = new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                FieldName = "whoCanContactOwner",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                AttributeName = "whoCanContactOwner",
                PropertyName = "WhoCanContactOwner",
                Api = "groupsettings",
                SupportsPatch = true,
                NullValueRepresentation = NullValueRepresentation.NullPlaceHolder,
            };

            type.AttributeAdapters.Add(whoCanContactOwner);
        }
        }
}