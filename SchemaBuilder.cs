using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lithnet.GoogleApps.MA
{
    using System.IO;
    using System.Runtime.Serialization;
    using System.Xml;
    using ManagedObjects;
    using Microsoft.MetadirectoryServices;

    internal static class SchemaBuilder
    {
        public static MASchemaTypes GetSchema(IManagementAgentParameters config)
        {
            MASchemaTypes types = new MASchemaTypes
            {
                SchemaBuilder.GetSchema(SchemaConstants.User, config),
                SchemaBuilder.GetSchema(SchemaConstants.Group, config)
            };

            return types;
        }

        public static MASchemaType GetSchema(string type, IManagementAgentParameters config)
        {
            switch (type)
            {
                case SchemaConstants.User:
                    return SchemaBuilder.GetUserSchema(config);

                case SchemaConstants.Group:
                    return SchemaBuilder.GetGroupSchema();
            }

            throw new InvalidOperationException();
        }

        public static MASchemaType GetUserSchema(IManagementAgentParameters config)
        {
            MASchemaType type = new MASchemaType
            {
                Attributes = new List<IMASchemaAttribute>(),
                Name = "user",
                AnchorAttributeName = "id",
                CanPatch = true,
                ApiInterface = new ApiInterfaceUser()
            };

            MASchemaAttribute orgUnitPath = new MASchemaAttribute
            {
                AttributeType = AttributeType.String,
                FieldName = "orgUnitPath",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                AttributeName = "orgUnitPath",
                PropertyName = "OrgUnitPath",
                Api = "user",
                CanPatch = true,
                IsArrayAttribute = false
            };

            type.Attributes.Add(orgUnitPath);

            MASchemaAttribute includeInGal = new MASchemaAttribute
            {
                AttributeType = AttributeType.Boolean,
                FieldName = "includeInGlobalAddressList",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                AttributeName = "includeInGlobalAddressList",
                PropertyName = "IncludeInGlobalAddressList",
                Api = "user",
                CanPatch = true,
                IsArrayAttribute = false
            };

            type.Attributes.Add(includeInGal);

            MASchemaAttribute suspended = new MASchemaAttribute
            {
                AttributeType = AttributeType.Boolean,
                FieldName = "suspended",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                AttributeName = "suspended",
                PropertyName = "Suspended",
                Api = "user",
                CanPatch = true,
                IsArrayAttribute = false
            };

            type.Attributes.Add(suspended);

            MASchemaAttribute changePasswordAtNextLogin = new MASchemaAttribute
            {
                AttributeType = AttributeType.Boolean,
                FieldName = "changePasswordAtNextLogin",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                AttributeName = "changePasswordAtNextLogin",
                PropertyName = "ChangePasswordAtNextLogin",
                Api = "user",
                CanPatch = true,
                IsArrayAttribute = false
            };

            type.Attributes.Add(changePasswordAtNextLogin);

            MASchemaAttribute ipWhitelisted = new MASchemaAttribute
            {
                AttributeType = AttributeType.Boolean,
                FieldName = "ipWhitelisted",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportOnly,
                AttributeName = "ipWhitelisted",
                PropertyName = "IpWhitelisted",
                Api = "user",
                CanPatch = true,
                IsArrayAttribute = false
            };

            type.Attributes.Add(ipWhitelisted);

            MASchemaAttribute id = new MASchemaAttribute
            {
                AttributeType = AttributeType.String,
                FieldName = "id",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportOnly,
                AttributeName = "id",
                PropertyName = "Id",
                Api = "user",
                CanPatch = true,
                IsAnchor = true,
                IsArrayAttribute = false
            };

            type.Attributes.Add(id);

            MASchemaAttribute primaryEmail = new MASchemaAttribute
            {
                AttributeType = AttributeType.String,
                FieldName = "primaryEmail",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportOnly,
                AttributeName = "primaryEmail",
                PropertyName = "PrimaryEmail",
                Api = "user",
                CanPatch = true,
                IsArrayAttribute = false
            };

            type.Attributes.Add(primaryEmail);

            MASchemaAttribute isAdmin = new MASchemaAttribute
            {
                AttributeType = AttributeType.Boolean,
                FieldName = "isAdmin",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                AttributeName = "isAdmin",
                PropertyName = "IsAdmin",
                Api = "usermakeadmin",
                CanPatch = true,
                IsArrayAttribute = false
            };

            type.Attributes.Add(isAdmin);

            MASchemaAttribute isDelegatedAdmin = new MASchemaAttribute
            {
                AttributeType = AttributeType.Boolean,
                FieldName = "isDelegatedAdmin",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportOnly,
                AttributeName = "isDelegatedAdmin",
                PropertyName = "IsDelegatedAdmin",
                Api = "user",
                CanPatch = true,
                IsArrayAttribute = false
            };

            type.Attributes.Add(isDelegatedAdmin);

            MASchemaAttribute lastLoginTime = new MASchemaAttribute
            {
                AttributeType = AttributeType.String,
                FieldName = "lastLoginTime",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportOnly,
                AttributeName = "lastLoginTime",
                PropertyName = "LastLoginTime",
                Api = "user",
                CanPatch = true,
                IsArrayAttribute = false
            };

            type.Attributes.Add(lastLoginTime);

            MASchemaAttribute creationTime = new MASchemaAttribute
            {
                AttributeType = AttributeType.String,
                FieldName = "creationTime",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportOnly,
                AttributeName = "creationTime",
                PropertyName = "CreationTime",
                Api = "user",
                CanPatch = true,
                IsArrayAttribute = false
            };

            type.Attributes.Add(creationTime);

            MASchemaAttribute deletionTime = new MASchemaAttribute
            {
                AttributeType = AttributeType.String,
                FieldName = "deletionTime",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportOnly,
                AttributeName = "deletionTime",
                PropertyName = "DeletionTime",
                Api = "user",
                CanPatch = true,
                IsArrayAttribute = false
            };

            type.Attributes.Add(deletionTime);

            MASchemaAttribute agreedToTerms = new MASchemaAttribute
            {
                AttributeType = AttributeType.Boolean,
                FieldName = "agreedToTerms",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportOnly,
                AttributeName = "agreedToTerms",
                PropertyName = "AgreedToTerms",
                Api = "user",
                CanPatch = true,
                IsArrayAttribute = false
            };

            type.Attributes.Add(agreedToTerms);

            MASchemaAttribute suspensionReason = new MASchemaAttribute
            {
                AttributeType = AttributeType.String,
                FieldName = "suspensionReason",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportOnly,
                AttributeName = "suspensionReason",
                PropertyName = "SuspensionReason",
                Api = "user",
                CanPatch = true,
                IsArrayAttribute = false
            };

            type.Attributes.Add(suspensionReason);

            MASchemaAttribute isMailboxSetup = new MASchemaAttribute
            {
                AttributeType = AttributeType.String,
                FieldName = "isMailboxSetup",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportOnly,
                AttributeName = "isMailboxSetup",
                PropertyName = "IsMailboxSetup",
                Api = "user",
                CanPatch = true,
                IsArrayAttribute = false
            };

            type.Attributes.Add(isMailboxSetup);

            MASchemaAttribute thumbnailPhotoUrl = new MASchemaAttribute
            {
                AttributeType = AttributeType.String,
                FieldName = "thumbnailPhotoUrl",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportOnly,
                AttributeName = "thumbnailPhotoUrl",
                PropertyName = "ThumbnailPhotoUrl",
                Api = "user",
                CanPatch = true,
                IsArrayAttribute = false
            };

            type.Attributes.Add(thumbnailPhotoUrl);

            MASchemaAttribute thumbnailPhotoEtag = new MASchemaAttribute
            {
                AttributeType = AttributeType.String,
                FieldName = "thumbnailPhotoEtag",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportOnly,
                AttributeName = "thumbnailPhotoEtag",
                PropertyName = "ThumbnailPhotoEtag",
                Api = "user",
                CanPatch = true,
                IsArrayAttribute = false
            };

            type.Attributes.Add(thumbnailPhotoEtag);

            MASchemaAttribute customerId = new MASchemaAttribute
            {
                AttributeType = AttributeType.String,
                FieldName = "customerId",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportOnly,
                AttributeName = "customerId",
                PropertyName = "CustomerId",
                Api = "user",
                CanPatch = true,
                IsArrayAttribute = false
            };

            type.Attributes.Add(customerId);

            SchemaBuilder.AddNames(type);
            SchemaBuilder.AddNotes(type);
            SchemaBuilder.AddWebSites(type, config);
            SchemaBuilder.AddUserAliases(type);
            SchemaBuilder.AddPhonesAttributes(type, config);
            SchemaBuilder.AddOrganizationsAttributes(type, config);
            SchemaBuilder.AddAddresses(type, config);
            SchemaBuilder.AddRelations(type, config);
            SchemaBuilder.AddExternalIds(type, config);
            SchemaBuilder.AddIms(type, config);

            return type;
        }

        public static MASchemaType GetGroupSchema()
        {
            MASchemaType type = new MASchemaType
            {
                Attributes = new List<IMASchemaAttribute>(),
                Name = "group",
                AnchorAttributeName = "id",
                CanPatch = true,
                ApiInterface = new ApiInterfaceGroup()
            };

            MASchemaAttribute adminCreated = new MASchemaAttribute
            {
                AttributeType = AttributeType.Boolean,
                FieldName = "adminCreated",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportOnly,
                AttributeName = "adminCreated",
                PropertyName = "AdminCreated",
                Api = "group",
                CanPatch = true,
                IsArrayAttribute = false
            };

            type.Attributes.Add(adminCreated);

            MASchemaAttribute description = new MASchemaAttribute
            {
                AttributeType = AttributeType.String,
                FieldName = "description",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                AttributeName = "description",
                PropertyName = "Description",
                Api = "group",
                CanPatch = true,
                IsArrayAttribute = false
            };

            type.Attributes.Add(description);

            MASchemaAttribute email = new MASchemaAttribute
            {
                AttributeType = AttributeType.String,
                FieldName = "email",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportOnly,
                AttributeName = "primaryEmail",
                PropertyName = "Email",
                Api = "group",
                CanPatch = true,
                IsArrayAttribute = false
            };

            type.Attributes.Add(email);

            MASchemaAttribute id = new MASchemaAttribute
            {
                AttributeType = AttributeType.String,
                FieldName = "id",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportOnly,
                AttributeName = "id",
                PropertyName = "Id",
                Api = "group",
                CanPatch = true,
                IsAnchor =  true,
                IsArrayAttribute = false
            };

            type.Attributes.Add(id);

            MASchemaAttribute name = new MASchemaAttribute
            {
                AttributeType = AttributeType.String,
                FieldName = "name",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                AttributeName = "name",
                PropertyName = "Name",
                Api = "group",
                CanPatch = true,
                IsArrayAttribute = false
            };

            type.Attributes.Add(name);

            SchemaBuilder.AddGroupAliases(type);
            SchemaBuilder.AddGroupSettings(type);
            SchemaBuilder.AddGroupMembers(type);
            return type;
        }

        private static void AddGroupMembers(MASchemaType type)
        {
            MASchemaSimpleList<string> members = new MASchemaSimpleList<string>
            {
                AttributeType = AttributeType.Reference,
                FieldName = "email",
                Operation = AttributeOperation.ImportExport,
                AttributeName = "member",
                PropertyName = "Members",
                Api = "groupmembership",
                CanPatch = true,
            };

            type.Attributes.Add(members);

            MASchemaSimpleList<string> managers = new MASchemaSimpleList<string>
            {
                AttributeType = AttributeType.Reference,
                FieldName = "email",
                Operation = AttributeOperation.ImportExport,
                AttributeName = "manager",
                PropertyName = "Managers",
                Api = "groupmembership",
                CanPatch = true,
            };

            type.Attributes.Add(managers);

            MASchemaSimpleList<string> owners = new MASchemaSimpleList<string>
            {
                AttributeType = AttributeType.Reference,
                FieldName = "email",
                Operation = AttributeOperation.ImportExport,
                AttributeName = "owner",
                PropertyName = "Owners",
                Api = "groupmembership",
                CanPatch = true,
            };

            type.Attributes.Add(owners);

            MASchemaSimpleList<string> externalMembers = new MASchemaSimpleList<string>
            {
                AttributeType = AttributeType.String,
                FieldName = "email",
                Operation = AttributeOperation.ImportExport,
                AttributeName = "externalMember",
                PropertyName = "ExternalMembers",
                Api = "groupmembership",
                CanPatch = true,
            };

            type.Attributes.Add(externalMembers);

            MASchemaSimpleList<string> externalManagers = new MASchemaSimpleList<string>
            {
                AttributeType = AttributeType.String,
                FieldName = "email",
                Operation = AttributeOperation.ImportExport,
                AttributeName = "externalManager",
                PropertyName = "ExternalManagers",
                Api = "groupmembership",
                CanPatch = true,
            };

            type.Attributes.Add(externalManagers);

            MASchemaSimpleList<string> externalOwners = new MASchemaSimpleList<string>
            {
                AttributeType = AttributeType.String,
                FieldName = "email",
                Operation = AttributeOperation.ImportExport,
                AttributeName = "externalOwner",
                PropertyName = "ExternalOwners",
                Api = "groupmembership",
                CanPatch = true,
            };

            type.Attributes.Add(externalOwners);
        }

        private static void AddGroupSettings(MASchemaType type)
        {
            MASchemaAttribute whoCanJoin = new MASchemaAttribute
            {
                AttributeType = AttributeType.String,
                FieldName = "whoCanJoin",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                AttributeName = "whoCanJoin",
                PropertyName = "WhoCanJoin",
                Api = "groupsettings",
                CanPatch = true,
                IsArrayAttribute = false
            };

            type.Attributes.Add(whoCanJoin);

            MASchemaAttribute whoCanViewMembership = new MASchemaAttribute
            {
                AttributeType = AttributeType.String,
                FieldName = "whoCanViewMembership",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                AttributeName = "whoCanViewMembership",
                PropertyName = "WhoCanViewMembership",
                Api = "groupsettings",
                CanPatch = true,
                IsArrayAttribute = false
            };

            type.Attributes.Add(whoCanViewMembership);

            MASchemaAttribute whoCanViewGroup = new MASchemaAttribute
            {
                AttributeType = AttributeType.String,
                FieldName = "whoCanViewGroup",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                AttributeName = "whoCanViewGroup",
                PropertyName = "WhoCanViewGroup",
                Api = "groupsettings",
                CanPatch = true,
                IsArrayAttribute = false
            };

            type.Attributes.Add(whoCanViewGroup);

            MASchemaAttribute whoCanInvite = new MASchemaAttribute
            {
                AttributeType = AttributeType.String,
                FieldName = "whoCanInvite",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                AttributeName = "whoCanInvite",
                PropertyName = "WhoCanInvite",
                Api = "groupsettings",
                CanPatch = true,
                IsArrayAttribute = false
            };

            type.Attributes.Add(whoCanInvite);
            
            //MASchemaAttribute whoCanAdd = new MASchemaAttribute
            //{
            //    AttributeType = AttributeType.String,
            //    FieldName = "whoCanAdd",
            //    IsMultivalued = false,
            //    Operation = AttributeOperation.ImportExport,
            //    AttributeName = "whoCanAdd",
            //    PropertyName = "WhoCanAdd",
            //    Api = "groupsettings",
            //    CanPatch = true,
            //    IsArrayAttribute = false
            //};
            //
            //type.Attributes.Add(whoCanAdd);

            MASchemaAttribute allowExternalMembers = new MASchemaAttribute
            {
                AttributeType = AttributeType.Boolean,
                FieldName = "allowExternalMembers",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                AttributeName = "allowExternalMembers",
                PropertyName = "AllowExternalMembers",
                Api = "groupsettings",
                CanPatch = true,
                IsArrayAttribute = false
            };

            type.Attributes.Add(allowExternalMembers);

            MASchemaAttribute whoCanPostMessage = new MASchemaAttribute
            {
                AttributeType = AttributeType.String,
                FieldName = "whoCanPostMessage",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                AttributeName = "whoCanPostMessage",
                PropertyName = "WhoCanPostMessage",
                Api = "groupsettings",
                CanPatch = true,
                IsArrayAttribute = false
            };

            type.Attributes.Add(whoCanPostMessage);

            MASchemaAttribute allowWebPosting = new MASchemaAttribute
            {
                AttributeType = AttributeType.Boolean,
                FieldName = "allowWebPosting",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                AttributeName = "allowWebPosting",
                PropertyName = "AllowWebPosting",
                Api = "groupsettings",
                CanPatch = true,
                IsArrayAttribute = false
            };

            type.Attributes.Add(allowWebPosting);

            MASchemaAttribute primaryLanguage = new MASchemaAttribute
            {
                AttributeType = AttributeType.String,
                FieldName = "primaryLanguage",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                AttributeName = "primaryLanguage",
                PropertyName = "PrimaryLanguage",
                Api = "groupsettings",
                CanPatch = true,
                IsArrayAttribute = false
            };

            type.Attributes.Add(primaryLanguage);

            MASchemaAttribute maxMessageBytes = new MASchemaAttribute
            {
                AttributeType = AttributeType.Integer,
                FieldName = "maxMessageBytes",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                AttributeName = "maxMessageBytes",
                PropertyName = "MaxMessageBytes",
                Api = "groupsettings",
                CanPatch = true,
                IsArrayAttribute = false
            };

            type.Attributes.Add(maxMessageBytes);

            MASchemaAttribute isArchived = new MASchemaAttribute
            {
                AttributeType = AttributeType.Boolean,
                FieldName = "isArchived",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                AttributeName = "isArchived",
                PropertyName = "IsArchived",
                Api = "groupsettings",
                CanPatch = true,
                IsArrayAttribute = false
            };

            type.Attributes.Add(isArchived);


            MASchemaAttribute archiveOnly = new MASchemaAttribute
            {
                AttributeType = AttributeType.Boolean,
                FieldName = "archiveOnly",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                AttributeName = "archiveOnly",
                PropertyName = "ArchiveOnly",
                Api = "groupsettings",
                CanPatch = true,
                IsArrayAttribute = false
            };

            type.Attributes.Add(archiveOnly);

            MASchemaAttribute messageModerationLevel = new MASchemaAttribute
            {
                AttributeType = AttributeType.String,
                FieldName = "messageModerationLevel",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                AttributeName = "messageModerationLevel",
                PropertyName = "MessageModerationLevel",
                Api = "groupsettings",
                CanPatch = true,
                IsArrayAttribute = false
            };

            type.Attributes.Add(messageModerationLevel);


            MASchemaAttribute spamModerationLevel = new MASchemaAttribute
            {
                AttributeType = AttributeType.String,
                FieldName = "spamModerationLevel",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                AttributeName = "spamModerationLevel",
                PropertyName = "SpamModerationLevel",
                Api = "groupsettings",
                CanPatch = true,
                IsArrayAttribute = false
            };

            type.Attributes.Add(spamModerationLevel);

            MASchemaAttribute replyTo = new MASchemaAttribute
            {
                AttributeType = AttributeType.String,
                FieldName = "replyTo",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                AttributeName = "replyTo",
                PropertyName = "ReplyTo",
                Api = "groupsettings",
                CanPatch = true,
                IsArrayAttribute = false
            };

            type.Attributes.Add(replyTo);

            MASchemaAttribute customReplyTo = new MASchemaAttribute
            {
                AttributeType = AttributeType.String,
                FieldName = "customReplyTo",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                AttributeName = "customReplyTo",
                PropertyName = "CustomReplyTo",
                Api = "groupsettings",
                CanPatch = true,
                IsArrayAttribute = false
            };

            type.Attributes.Add(customReplyTo);

            MASchemaAttribute sendMessageDenyNotification = new MASchemaAttribute
            {
                AttributeType = AttributeType.Boolean,
                FieldName = "sendMessageDenyNotification",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                AttributeName = "sendMessageDenyNotification",
                PropertyName = "SendMessageDenyNotification",
                Api = "groupsettings",
                CanPatch = true,
                IsArrayAttribute = false
            };

            type.Attributes.Add(sendMessageDenyNotification);


            MASchemaAttribute defaultMessageDenyNotificationText = new MASchemaAttribute
            {
                AttributeType = AttributeType.String,
                FieldName = "defaultMessageDenyNotificationText",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                AttributeName = "defaultMessageDenyNotificationText",
                PropertyName = "DefaultMessageDenyNotificationText",
                Api = "groupsettings",
                CanPatch = true,
                IsArrayAttribute = false
            };

            type.Attributes.Add(defaultMessageDenyNotificationText);

            MASchemaAttribute showInGroupDirectory = new MASchemaAttribute
            {
                AttributeType = AttributeType.Boolean,
                FieldName = "showInGroupDirectory",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                AttributeName = "showInGroupDirectory",
                PropertyName = "ShowInGroupDirectory",
                Api = "groupsettings",
                CanPatch = true,
                IsArrayAttribute = false
            };

            type.Attributes.Add(showInGroupDirectory);


            MASchemaAttribute allowGoogleCommunication = new MASchemaAttribute
            {
                AttributeType = AttributeType.Boolean,
                FieldName = "allowGoogleCommunication",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                AttributeName = "allowGoogleCommunication",
                PropertyName = "AllowGoogleCommunication",
                Api = "groupsettings",
                CanPatch = true,
                IsArrayAttribute = false
            };

            type.Attributes.Add(allowGoogleCommunication);

            MASchemaAttribute membersCanPostAsTheGroup = new MASchemaAttribute
            {
                AttributeType = AttributeType.Boolean,
                FieldName = "membersCanPostAsTheGroup",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                AttributeName = "membersCanPostAsTheGroup",
                PropertyName = "MembersCanPostAsTheGroup",
                Api = "groupsettings",
                CanPatch = true,
                IsArrayAttribute = false
            };

            type.Attributes.Add(membersCanPostAsTheGroup);

            MASchemaAttribute messageDisplayFont = new MASchemaAttribute
            {
                AttributeType = AttributeType.String,
                FieldName = "messageDisplayFont",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                AttributeName = "messageDisplayFont",
                PropertyName = "MessageDisplayFont",
                Api = "groupsettings",
                CanPatch = true,
                IsArrayAttribute = false
            };

            type.Attributes.Add(messageDisplayFont);

            MASchemaAttribute includeInGlobalAddressList = new MASchemaAttribute
            {
                AttributeType = AttributeType.Boolean,
                FieldName = "includeInGlobalAddressList",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                AttributeName = "includeInGlobalAddressList",
                PropertyName = "IncludeInGlobalAddressList",
                Api = "groupsettings",
                CanPatch = true,
                IsArrayAttribute = false
            };

            type.Attributes.Add(includeInGlobalAddressList);

            MASchemaAttribute whoCanLeaveGroup = new MASchemaAttribute
            {
                AttributeType = AttributeType.String,
                FieldName = "whoCanLeaveGroup",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                AttributeName = "whoCanLeaveGroup",
                PropertyName = "WhoCanLeaveGroup",
                Api = "groupsettings",
                CanPatch = true,
                IsArrayAttribute = false
            };

            type.Attributes.Add(whoCanLeaveGroup);


            MASchemaAttribute whoCanContactOwner = new MASchemaAttribute
            {
                AttributeType = AttributeType.String,
                FieldName = "whoCanContactOwner",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                AttributeName = "whoCanContactOwner",
                PropertyName = "WhoCanContactOwner",
                Api = "groupsettings",
                CanPatch = true,
                IsArrayAttribute = false
            };

            type.Attributes.Add(whoCanContactOwner);
        }

        private static void AddNames(MASchemaType type)
        {
            MASchemaField givenName = new MASchemaField
            {
                AttributeType = AttributeType.String,
                FieldName = "givenName",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                PropertyName = "GivenName",
                AttributeNamePart = "givenName"
            };

            MASchemaField familyName = new MASchemaField
            {
                AttributeType = AttributeType.String,
                FieldName = "familyName",
                IsMultivalued = false,
                PropertyName = "FamilyName",
                Operation = AttributeOperation.ImportExport,
                AttributeNamePart = "familyName"
            };

            MASchemaNestedType schemaItem = new MASchemaNestedType
            {
                Api = "user",
                AttributeName = "name",
                Fields = new List<MASchemaField>() {givenName, familyName},
                FieldName = "name",
                PropertyName = "Name",
                CanPatch = false
            };

            type.Attributes.Add(schemaItem);
        }

        private static void AddNotes(MASchemaType type)
        {
            MASchemaField notesValue = new MASchemaField
            {
                AttributeType = AttributeType.String,
                FieldName = "value",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                PropertyName = "Value",
                AttributeNamePart = "value"
            };

            MASchemaField notesContentType = new MASchemaField
            {
                AttributeType = AttributeType.String,
                FieldName = "contentType",
                IsMultivalued = false,
                PropertyName = "ContentType",
                Operation = AttributeOperation.ImportExport,
                AttributeNamePart = "contentType"
            };

            MASchemaNestedType notesType = new MASchemaNestedType
            {
                Api = "user",
                AttributeName = "notes",
                Fields = new List<MASchemaField>() {notesContentType, notesValue},
                FieldName = "notes",
                PropertyName = "Notes",
                CanPatch = false
            };

            type.Attributes.Add(notesType);
        }

        private static void AddWebSites(MASchemaType type, IManagementAgentParameters config)
        {
            MASchemaField webSiteValue = new MASchemaField
            {
                AttributeType = AttributeType.String,
                FieldName = "value",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                PropertyName = "Value",
                AttributeNamePart = null
            };

            MASchemaField webSitePrimary = new MASchemaField
            {
                AttributeType = AttributeType.Boolean,
                FieldName = "primary",
                IsMultivalued = false,
                PropertyName = "Primary",
                Operation = AttributeOperation.ImportExport,
                AttributeNamePart = "primary"
            };

            MASchemaCustomTypeList<Website> webSiteType = new MASchemaCustomTypeList<Website>
            {
                Api = "user",
                AttributeName = "websites",
                Fields = new List<MASchemaField>() {webSitePrimary, webSiteValue},
                FieldName = "websites",
                PropertyName = "Websites",
                KnownTypes = config.WebsitesAttributeFixedTypes?.ToList(),
                CanPatch = false
            };
            
            type.Attributes.Add(webSiteType);
        }

        private static void AddGroupAliases(MASchemaType type)
        {
            MASchemaSimpleList<string> aliasesList = new MASchemaSimpleList<string>
            {
                Api = "groupaliases",
                AttributeName = "aliases",
                FieldName = "aliases",
                PropertyName = "Aliases",
                CanPatch = true,
                IsReadOnly = false
            };

            type.Attributes.Add(aliasesList);

            MASchemaSimpleList<string> nonEditableAliasesList = new MASchemaSimpleList<string>
            {
                Api = "group",
                AttributeName = "nonEditableAliases",
                FieldName = "nonEditableAliases",
                PropertyName = "NonEditableAliases",
                CanPatch = false,
                IsReadOnly = true
            };

            type.Attributes.Add(nonEditableAliasesList);
        }

        private static void AddUserAliases(MASchemaType type)
        {
            MASchemaSimpleList<string> aliasesList = new MASchemaSimpleList<string>
            {
                Api = "useraliases",
                AttributeName = "aliases",
                FieldName = "aliases",
                PropertyName = "Aliases",
                CanPatch = true,
                IsReadOnly = false
            };

            type.Attributes.Add(aliasesList);

            MASchemaSimpleList<string> nonEditableAliasesList = new MASchemaSimpleList<string>
            {
                Api = "user",
                AttributeName = "nonEditableAliases",
                FieldName = "nonEditableAliases",
                PropertyName = "NonEditableAliases",
                CanPatch = false,
                IsReadOnly = true
            };

            type.Attributes.Add(nonEditableAliasesList);
        }

        private static void AddPhonesAttributes(MASchemaType type, IManagementAgentParameters config)
        {
            MASchemaField phonesValue = new MASchemaField
            {
                AttributeType = AttributeType.String,
                FieldName = "value",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                PropertyName = "Value",
                AttributeNamePart = null
            };

            MASchemaField phonesPrimary = new MASchemaField
            {
                AttributeType = AttributeType.Boolean,
                FieldName = "primary",
                IsMultivalued = false,
                PropertyName = "Primary",
                Operation = AttributeOperation.ImportExport,
                AttributeNamePart = "primary"
            };

            MASchemaCustomTypeList<Phone> phonesType = new MASchemaCustomTypeList<Phone>
            {
                Api = "user",
                AttributeName = "phones",
                Fields = new List<MASchemaField>() {phonesPrimary, phonesValue},
                FieldName = "phones",
                PropertyName = "Phones",
                KnownTypes = config.PhonesAttributeFixedTypes?.ToList(),
                CanPatch = false
            };

            type.Attributes.Add(phonesType);
        }

        private static void AddOrganizationsAttributes(MASchemaType type, IManagementAgentParameters config)
        {
            MASchemaField name = new MASchemaField
            {
                AttributeType = AttributeType.String,
                FieldName = "name",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                PropertyName = "Name",
                AttributeNamePart = "name"
            };

            MASchemaField title = new MASchemaField
            {
                AttributeType = AttributeType.String,
                FieldName = "title",
                IsMultivalued = false,
                PropertyName = "Title",
                Operation = AttributeOperation.ImportExport,
                AttributeNamePart = "title"
            };

            MASchemaField primary = new MASchemaField
            {
                AttributeType = AttributeType.Boolean,
                FieldName = "primary",
                IsMultivalued = false,
                PropertyName = "Primary",
                Operation = AttributeOperation.ImportExport,
                AttributeNamePart = "primary"
            };

            MASchemaField department = new MASchemaField
            {
                AttributeType = AttributeType.String,
                FieldName = "department",
                IsMultivalued = false,
                PropertyName = "Department",
                Operation = AttributeOperation.ImportExport,
                AttributeNamePart = "department"
            };

            MASchemaField symbol = new MASchemaField
            {
                AttributeType = AttributeType.String,
                FieldName = "symbol",
                IsMultivalued = false,
                PropertyName = "Symbol",
                Operation = AttributeOperation.ImportExport,
                AttributeNamePart = "symbol"
            };
            
            MASchemaField location = new MASchemaField
            {
                AttributeType = AttributeType.String,
                FieldName = "location",
                IsMultivalued = false,
                PropertyName = "Location",
                Operation = AttributeOperation.ImportExport,
                AttributeNamePart = "location"
            };

            MASchemaField description = new MASchemaField
            {
                AttributeType = AttributeType.String,
                FieldName = "description",
                IsMultivalued = false,
                PropertyName = "Description",
                Operation = AttributeOperation.ImportExport,
                AttributeNamePart = "description"
            };

            MASchemaField domain = new MASchemaField
            {
                AttributeType = AttributeType.String,
                FieldName = "domain",
                IsMultivalued = false,
                PropertyName = "Domain",
                Operation = AttributeOperation.ImportExport,
                AttributeNamePart = "domain"
            };

            MASchemaField costCenter = new MASchemaField
            {
                AttributeType = AttributeType.String,
                FieldName = "costCenter",
                IsMultivalued = false,
                PropertyName = "CostCenter",
                Operation = AttributeOperation.ImportExport,
                AttributeNamePart = "costCenter"
            };

            MASchemaCustomTypeList<Organization> customType = new MASchemaCustomTypeList<Organization>
            {
                Api = "user",
                AttributeName = "organizations",
                Fields = new List<MASchemaField>() {name, title, primary, department, symbol, location, description, domain, costCenter},
                FieldName = "organizations",
                PropertyName = "Organizations",
                KnownTypes = config.OrganizationsAttributeFixedTypes?.ToList(),
                CanPatch = false
            };

            type.Attributes.Add(customType);
        }

        private static void AddAddresses(MASchemaType type, IManagementAgentParameters config)
        {
            MASchemaField sourceIsStructured = new MASchemaField
            {
                AttributeType = AttributeType.Boolean,
                FieldName = "sourceIsStructured",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                PropertyName = "SourceIsStructured",
                AttributeNamePart = "sourceIsStructured"
            };

            MASchemaField formatted = new MASchemaField
            {
                AttributeType = AttributeType.String,
                FieldName = "formatted",
                IsMultivalued = false,
                PropertyName = "Formatted",
                Operation = AttributeOperation.ImportExport,
                AttributeNamePart = "formatted"
            };

            MASchemaField primary = new MASchemaField
            {
                AttributeType = AttributeType.Boolean,
                FieldName = "primary",
                IsMultivalued = false,
                PropertyName = "Primary",
                Operation = AttributeOperation.ImportExport,
                AttributeNamePart = "primary"
            };

            MASchemaField poBox = new MASchemaField
            {
                AttributeType = AttributeType.String,
                FieldName = "poBox",
                IsMultivalued = false,
                PropertyName = "POBox",
                Operation = AttributeOperation.ImportExport,
                AttributeNamePart = "poBox"
            };

            MASchemaField extendedAddress = new MASchemaField
            {
                AttributeType = AttributeType.String,
                FieldName = "extendedAddress",
                IsMultivalued = false,
                PropertyName = "ExtendedAddress",
                Operation = AttributeOperation.ImportExport,
                AttributeNamePart = "extendedAddress"
            };


            MASchemaField streetAddress = new MASchemaField
            {
                AttributeType = AttributeType.String,
                FieldName = "streetAddress",
                IsMultivalued = false,
                PropertyName = "StreetAddress",
                Operation = AttributeOperation.ImportExport,
                AttributeNamePart = "streetAddress"
            };

            MASchemaField locality = new MASchemaField
            {
                AttributeType = AttributeType.String,
                FieldName = "locality",
                IsMultivalued = false,
                PropertyName = "Locality",
                Operation = AttributeOperation.ImportExport,
                AttributeNamePart = "locality"
            };

            MASchemaField region = new MASchemaField
            {
                AttributeType = AttributeType.String,
                FieldName = "region",
                IsMultivalued = false,
                PropertyName = "Region",
                Operation = AttributeOperation.ImportExport,
                AttributeNamePart = "region"
            };

            MASchemaField postalCode = new MASchemaField
            {
                AttributeType = AttributeType.String,
                FieldName = "postalCode",
                IsMultivalued = false,
                PropertyName = "PostalCode",
                Operation = AttributeOperation.ImportExport,
                AttributeNamePart = "postalCode"
            };

            MASchemaField country = new MASchemaField
            {
                AttributeType = AttributeType.String,
                FieldName = "country",
                IsMultivalued = false,
                PropertyName = "Country",
                Operation = AttributeOperation.ImportExport,
                AttributeNamePart = "country"
            };

            MASchemaField countryCode = new MASchemaField
            {
                AttributeType = AttributeType.String,
                FieldName = "countryCode",
                IsMultivalued = false,
                PropertyName = "CountryCode",
                Operation = AttributeOperation.ImportExport,
                AttributeNamePart = "countryCode"
            };

            MASchemaCustomTypeList<Address> customType = new MASchemaCustomTypeList<Address>
            {
                Api = "user",
                AttributeName = "addresses",
                Fields = new List<MASchemaField>() {sourceIsStructured, formatted, poBox, extendedAddress, primary, streetAddress, locality, region, postalCode, country, countryCode},
                FieldName = "addresses",
                PropertyName = "Addresses",
                KnownTypes = config.AddressesAttributeFixedTypes?.ToList(),
                CanPatch = false
            };

            type.Attributes.Add(customType);
        }

        private static void AddRelations(MASchemaType type, IManagementAgentParameters config)
        {
            MASchemaField value = new MASchemaField
            {
                AttributeType = AttributeType.String,
                FieldName = "value",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                PropertyName = "Value",
                AttributeNamePart = null
            };

            MASchemaCustomTypeList<Relation> customType = new MASchemaCustomTypeList<Relation>
            {
                Api = "user",
                AttributeName = "relations",
                Fields = new List<MASchemaField>() {value},
                FieldName = "relations",
                PropertyName = "Relations",
                KnownTypes = config.RelationsAttributeFixedTypes?.ToList(),
                CanPatch = false
            };

            type.Attributes.Add(customType);
        }

        private static void AddExternalIds(MASchemaType type, IManagementAgentParameters config)
        {
            MASchemaField value = new MASchemaField
            {
                AttributeType = AttributeType.String,
                FieldName = "value",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                PropertyName = "Value",
                AttributeNamePart = null
            };

            MASchemaCustomTypeList<ExternalID> customType = new MASchemaCustomTypeList<ExternalID>
            {
                Api = "user",
                AttributeName = "externalIds",
                Fields = new List<MASchemaField>() {value},
                FieldName = "externalIds",
                PropertyName = "ExternalIds",
                KnownTypes = config.ExternalIDsAttributeFixedTypes?.ToList(),
                CanPatch = false
            };

            type.Attributes.Add(customType);
        }

        private static void AddIms(MASchemaType type, IManagementAgentParameters config)
        {
            MASchemaField im = new MASchemaField
            {
                AttributeType = AttributeType.String,
                FieldName = "im",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                PropertyName = "IMAddress",
                AttributeNamePart = "im"
            };

            MASchemaField protocol = new MASchemaField
            {
                AttributeType = AttributeType.String,
                FieldName = "protocol",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                PropertyName = "Protocol",
                AttributeNamePart = "protocol"
            };

            MASchemaField primary = new MASchemaField
            {
                AttributeType = AttributeType.Boolean,
                FieldName = "primary",
                IsMultivalued = false,
                PropertyName = "Primary",
                Operation = AttributeOperation.ImportExport,
                AttributeNamePart = "primary"
            };

            MASchemaCustomTypeList<IM> customType = new MASchemaCustomTypeList<IM>
            {
                Api = "user",
                AttributeName = "ims",
                Fields = new List<MASchemaField>() {im, protocol, primary},
                FieldName = "ims",
                PropertyName = "Ims",
                KnownTypes = new List<string>() {"work"},
                CanPatch = false
            };

            type.Attributes.Add(customType);
        }
    }
}
