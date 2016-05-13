using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using G = Google.Apis.Admin.Directory.directory_v1.Data;

namespace Lithnet.GoogleApps.MA
{
    using System.IO;
    using System.Runtime.Serialization;
    using System.Xml;
    using Google.Apis.Admin.Directory.directory_v1.Data;
    using Google.GData.Client;
    using Google.GData.Contacts;
    using Google.GData.Extensions;
    using ManagedObjects;
    using Microsoft.MetadirectoryServices;
    using Relation = ManagedObjects.Relation;
    using Website = ManagedObjects.Website;
    using GDataOrganization = Google.GData.Extensions.Organization;
    using GDataEmail = Google.GData.Extensions.EMail;
    using GDataIM = Google.GData.Extensions.IMAddress;
    using Organization = ManagedObjects.Organization;

    internal static class SchemaBuilder
    {
        public static MASchemaTypes GetSchema(IManagementAgentParameters config)
        {
            MASchemaTypes types = new MASchemaTypes
            {
                SchemaBuilder.GetSchema(SchemaConstants.User, config),
                SchemaBuilder.GetSchema(SchemaConstants.Group, config),
                SchemaBuilder.GetSchema(SchemaConstants.Contact, config)
            };

            if (SchemaRequestFactory.HasSchema(config.CustomerID, SchemaConstants.CustomGoogleAppsSchemaName))
            {
                types.Add(SchemaBuilder.GetSchema(SchemaConstants.AdvancedUser, config));
            }

            return types;
        }

        public static MASchemaType GetSchema(string type, IManagementAgentParameters config)
        {
            switch (type)
            {
                case SchemaConstants.User:
                    return SchemaBuilder.GetUserSchema(config);

                case SchemaConstants.AdvancedUser:
                    return SchemaBuilder.GetAdvancedUserSchema(config);

                case SchemaConstants.Group:
                    return SchemaBuilder.GetGroupSchema();

                case SchemaConstants.Contact:
                    return SchemaBuilder.GetContactSchema(config);
            }

            throw new InvalidOperationException();
        }

        public static MASchemaType GetContactSchema(IManagementAgentParameters config)
        {
            MASchemaType type = new MASchemaType
            {
                Attributes = new List<IMASchemaAttribute>(),
                Name = "contact",
                AnchorAttributeName = "id",
                CanPatch = false,
                ApiInterface = new ApiInterfaceContact(config.Domain)
            };

            MASchemaAttribute occupation = new MASchemaAttribute
            {
                AttributeType = AttributeType.String,
                FieldName = "occupation",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                AttributeName = "occupation",
                PropertyName = "Occupation",
                Api = "contact",
                CanPatch = false,
                IsArrayAttribute = false
            };

            type.Attributes.Add(occupation);
            
            MASchemaAttribute billingInformation = new MASchemaAttribute
            {
                AttributeType = AttributeType.String,
                FieldName = "billingInformation",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                AttributeName = "billingInformation",
                PropertyName = "BillingInformation",
                Api = "contact",
                CanPatch = false,
                IsArrayAttribute = false
            };

            type.Attributes.Add(billingInformation);
            
            MASchemaAttribute birthday = new MASchemaAttribute
            {
                AttributeType = AttributeType.String,
                FieldName = "birthday",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                AttributeName = "birthday",
                PropertyName = "Birthday",
                Api = "contact",
                CanPatch = false,
                IsArrayAttribute = false
            };

            type.Attributes.Add(birthday);
            
            MASchemaAttribute directoryServer = new MASchemaAttribute
            {
                AttributeType = AttributeType.String,
                FieldName = "directoryServer",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                AttributeName = "directoryServer",
                PropertyName = "DirectoryServer",
                Api = "contact",
                CanPatch = false,
                IsArrayAttribute = false
            };

            type.Attributes.Add(directoryServer);


            MASchemaAttribute initials = new MASchemaAttribute
            {
                AttributeType = AttributeType.String,
                FieldName = "initials",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                AttributeName = "initials",
                PropertyName = "Initials",
                Api = "contact",
                CanPatch = false,
                IsArrayAttribute = false
            };

            type.Attributes.Add(initials);

            MASchemaAttribute maidenName = new MASchemaAttribute
            {
                AttributeType = AttributeType.String,
                FieldName = "maidenName",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                AttributeName = "maidenName",
                PropertyName = "MaidenName",
                Api = "contact",
                CanPatch = false,
                IsArrayAttribute = false
            };

            type.Attributes.Add(maidenName);

            MASchemaAttribute mileage = new MASchemaAttribute
            {
                AttributeType = AttributeType.String,
                FieldName = "mileage",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                AttributeName = "mileage",
                PropertyName = "Mileage",
                Api = "contact",
                CanPatch = false,
                IsArrayAttribute = false
            };

            type.Attributes.Add(mileage);

            MASchemaAttribute nickname = new MASchemaAttribute
            {
                AttributeType = AttributeType.String,
                FieldName = "nickname",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                AttributeName = "nickname",
                PropertyName = "Nickname",
                Api = "contact",
                CanPatch = false,
                IsArrayAttribute = false
            };

            type.Attributes.Add(nickname);

            MASchemaAttribute priority = new MASchemaAttribute
            {
                AttributeType = AttributeType.String,
                FieldName = "priority",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                AttributeName = "priority",
                PropertyName = "Priority",
                Api = "contact",
                CanPatch = false,
                IsArrayAttribute = false
            };

            type.Attributes.Add(priority);

            MASchemaAttribute sensitivity = new MASchemaAttribute
            {
                AttributeType = AttributeType.String,
                FieldName = "sensitivity",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                AttributeName = "sensitivity",
                PropertyName = "Sensitivity",
                Api = "contact",
                CanPatch = false,
                IsArrayAttribute = false
            };

            type.Attributes.Add(sensitivity);

            MASchemaAttribute shortName = new MASchemaAttribute
            {
                AttributeType = AttributeType.String,
                FieldName = "shortName",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                AttributeName = "shortName",
                PropertyName = "ShortName",
                Api = "contact",
                CanPatch = false,
                IsArrayAttribute = false
            };

            type.Attributes.Add(shortName);

            MASchemaAttribute subject = new MASchemaAttribute
            {
                AttributeType = AttributeType.String,
                FieldName = "subject",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                AttributeName = "subject",
                PropertyName = "Subject",
                Api = "contact",
                CanPatch = false,
                IsArrayAttribute = false
            };

            type.Attributes.Add(subject);

            MASchemaAttribute id = new MASchemaAttribute
            {
                AttributeType = AttributeType.String,
                FieldName = "id",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportOnly,
                AttributeName = "id",
                PropertyName = "Id",
                Api = "contact",
                CanPatch = true,
                IsAnchor = true,
                IsArrayAttribute = false,
                CastForImport = (value) => ((AtomId)value).AbsoluteUri,
            };

            type.Attributes.Add(id);

            SchemaBuilder.AddContactNames(type);
            SchemaBuilder.AddContactOrganizationsAttributes(type, config);
            SchemaBuilder.AddContactExternalIds(type, config);
            SchemaBuilder.AddContactEmailAttributes(type, config);
            SchemaBuilder.AddContactIms(type, config);
            SchemaBuilder.AddContactPhones(type, config);

            return type;
        }

        private static void AddContactEmailAttributes(MASchemaType type, IManagementAgentParameters config)
        {
            MASchemaField address = new MASchemaField
            {
                AttributeType = AttributeType.String,
                FieldName = "address",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                PropertyName = "Address",
                AttributeNamePart = null
            };
            
            MASchemaGDataCommonAttributesList<GDataEmail> customType = new MASchemaGDataCommonAttributesList<GDataEmail>
            {
                Api = "contact",
                AttributeName = "email",
                Fields = new List<MASchemaField>() { address },
                FieldName = "email",
                PropertyName = "Emails",
                KnownTypes = config.EmailsAttributeFixedTypes?.ToList(),
                CanPatch = false,
                KnownRels = new Dictionary<string, string>() { { "http://schemas.google.com/g/2005#work", "work" }, { "http://schemas.google.com/g/2005#home", "home" }, { "http://schemas.google.com/g/2005#other", "other" } }
            };

            type.Attributes.Add(customType);
        }


        private static void AddContactOrganizationsAttributes(MASchemaType type, IManagementAgentParameters config)
        {

            MASchemaField name = new MASchemaField
            {
                AttributeType = AttributeType.String,
                FieldName = "orgName",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                PropertyName = "Name",
                AttributeNamePart = "name"
            };

            MASchemaField title = new MASchemaField
            {
                AttributeType = AttributeType.String,
                FieldName = "orgTitle",
                IsMultivalued = false,
                PropertyName = "Title",
                Operation = AttributeOperation.ImportExport,
                AttributeNamePart = "title"
            };

            MASchemaField department = new MASchemaField
            {
                AttributeType = AttributeType.String,
                FieldName = "orgDepartment",
                IsMultivalued = false,
                PropertyName = "Department",
                Operation = AttributeOperation.ImportExport,
                AttributeNamePart = "department"
            };

            MASchemaField symbol = new MASchemaField
            {
                AttributeType = AttributeType.String,
                FieldName = "orgSymbol",
                IsMultivalued = false,
                PropertyName = "Symbol",
                Operation = AttributeOperation.ImportExport,
                AttributeNamePart = "symbol"
            };

            MASchemaField location = new MASchemaField
            {
                AttributeType = AttributeType.String,
                FieldName = "where",
                IsMultivalued = false,
                PropertyName = "Location",
                Operation = AttributeOperation.ImportExport,
                AttributeNamePart = "location"
            };

            MASchemaGDataCommonAttributesList<GDataOrganization> customType = new MASchemaGDataCommonAttributesList<GDataOrganization>
            {
                Api = "contact",
                AttributeName = "organizations",
                Fields = new List<MASchemaField>() { name, title, department, symbol, location },
                FieldName = "organizations",
                PropertyName = "Organizations",
                KnownTypes = config.OrganizationsAttributeFixedTypes?.ToList(),
                CanPatch = false,
                KnownRels = new Dictionary<string, string>() { { "http://schemas.google.com/g/2005#work", "work" } }
            };

            type.Attributes.Add(customType);
        }

        private static void AddContactExternalIds(MASchemaType type, IManagementAgentParameters config)
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

            MASchemaGDataSimpleAttributesList<ExternalId> customType = new MASchemaGDataSimpleAttributesList<ExternalId>
            {
                Api = "contact",
                AttributeName = "externalIds",
                Fields = new List<MASchemaField>() { value },
                FieldName = "externalIds",
                PropertyName = "ExternalIds",
                KnownTypes = config.ExternalIDsAttributeFixedTypes?.ToList(),
                CanPatch = false,
                KnownRels = new HashSet<string>() { "account", "customer", "network", "organization" }
            };

            type.Attributes.Add(customType);
        }

        private static void AddContactPhones(MASchemaType type, IManagementAgentParameters config)
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
            
            MASchemaGDataCommonAttributesList<PhoneNumber> phonesType = new MASchemaGDataCommonAttributesList<PhoneNumber>
            {
                Api = "contact",
                AttributeName = "phones",
                Fields = new List<MASchemaField>() {phonesValue},
                FieldName = "phones",
                PropertyName = "Phonenumbers",
                KnownTypes = config.PhonesAttributeFixedTypes?.ToList(),
                CanPatch = false,
                KnownRels = new Dictionary<string, string>()
                {
                    { "http://schemas.google.com/g/2005#work_pager", "work_pager" } ,
                    { "http://schemas.google.com/g/2005#work_mobile", "work_mobile" },
                    { "http://schemas.google.com/g/2005#work", "work" },
                    { "http://schemas.google.com/g/2005#tty_tdd", "tty_tdd" },
                    { "http://schemas.google.com/g/2005#telex", "telex" },
                    { "http://schemas.google.com/g/2005#radio", "radio" },
                    { "http://schemas.google.com/g/2005#pager", "pager" },
                    { "http://schemas.google.com/g/2005#other_fax", "other_fax" },
                    { "http://schemas.google.com/g/2005#mobile", "mobile" },
                    { "http://schemas.google.com/g/2005#main", "main" },
                    { "http://schemas.google.com/g/2005#isdn", "isdn" },
                    { "http://schemas.google.com/g/2005#home_fax", "home_fax" },
                    { "http://schemas.google.com/g/2005#home", "home" },
                    { "http://schemas.google.com/g/2005#fax", "fax" },
                    { "http://schemas.google.com/g/2005#company_main", "company_main" },
                    { "http://schemas.google.com/g/2005#car", "car" },
                    { "http://schemas.google.com/g/2005#callback", "callback" },
                    { "http://schemas.google.com/g/2005#assistant", "assistant" },
                }
            };

            type.Attributes.Add(phonesType);
        }

        private static void AddContactIms(MASchemaType type, IManagementAgentParameters config)
        {
            MASchemaField im = new MASchemaField
            {
                AttributeType = AttributeType.String,
                FieldName = "address",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                PropertyName = "Address",
                AttributeNamePart = "address"
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
            
            MASchemaGDataCommonAttributesList<GDataIM> customType = new MASchemaGDataCommonAttributesList<GDataIM>
            {
                Api = "contact",
                AttributeName = "ims",
                Fields = new List<MASchemaField>() { im, protocol },
                FieldName = "ims",
                PropertyName = "IMs",
                KnownTypes = config.IMsAttributeFixedTypes?.ToList(),
                CanPatch = false,
                KnownRels = new Dictionary<string, string>()
                {
                    { "http://schemas.google.com/g/2005#work", "work" } ,
                    { "http://schemas.google.com/g/2005#netmeeting", "netmeeting" },
                    { "http://schemas.google.com/g/2005#home", "home" }
                }

            };

            type.Attributes.Add(customType);
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
                AttributeType = AttributeType.Boolean,
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

            SchemaBuilder.AddUserNames(type);
            SchemaBuilder.AddUserNotes(type);
            SchemaBuilder.AddUserWebSites(type, config);
            SchemaBuilder.AddUserAliases(type);
            SchemaBuilder.AddUserPhonesAttributes(type, config);
            SchemaBuilder.AddUserOrganizationsAttributes(type, config);
            SchemaBuilder.AddUserAddresses(type, config);
            SchemaBuilder.AddUserRelations(type, config);
            SchemaBuilder.AddUserExternalIds(type, config);
            SchemaBuilder.AddUserIms(type, config);

            return type;
        }

        public static MASchemaType GetAdvancedUserSchema(IManagementAgentParameters config)
        {
            MASchemaType userType = SchemaBuilder.GetUserSchema(config);
            userType.Name = SchemaConstants.AdvancedUser;
            userType.ApiInterface = new ApiInterfaceAdvancedUser();

            MASchemaCollection<string> delegates = new MASchemaCollection<string>
            {
                AttributeType = AttributeType.Reference,
                FieldName = null,
                Operation = AttributeOperation.ImportExport,
                AttributeName = "delegate",
                PropertyName = "Delegates",
                Api = "userdelegates",
                CanPatch = true,
            };

            userType.Attributes.Add(delegates);

            return userType;
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
                IsAnchor = true,
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
            MASchemaCollection<string> members = new MASchemaCollection<string>
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

            MASchemaCollection<string> managers = new MASchemaCollection<string>
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

            MASchemaCollection<string> owners = new MASchemaCollection<string>
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

            MASchemaCollection<string> externalMembers = new MASchemaCollection<string>
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

            MASchemaCollection<string> externalManagers = new MASchemaCollection<string>
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

            MASchemaCollection<string> externalOwners = new MASchemaCollection<string>
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

        private static void AddUserNames(MASchemaType type)
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
                Fields = new List<MASchemaField>() { givenName, familyName },
                FieldName = "name",
                PropertyName = "Name",
                CanPatch = false
            };

            type.Attributes.Add(schemaItem);
        }

        private static void AddContactNames(MASchemaType type)
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

            MASchemaField fullName = new MASchemaField
            {
                AttributeType = AttributeType.String,
                FieldName = "fullName",
                IsMultivalued = false,
                PropertyName = "FullName",
                Operation = AttributeOperation.ImportExport,
                AttributeNamePart = "fullName"
            };

            MASchemaNestedType schemaItem = new MASchemaNestedType
            {
                Api = "contact",
                AttributeName = "name",
                Fields = new List<MASchemaField>() { givenName, familyName, fullName },
                FieldName = "name",
                PropertyName = "Name",
                CanPatch = false
            };

            type.Attributes.Add(schemaItem);
        }

        private static void AddUserNotes(MASchemaType type)
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
                Fields = new List<MASchemaField>() { notesContentType, notesValue },
                FieldName = "notes",
                PropertyName = "Notes",
                CanPatch = false
            };

            type.Attributes.Add(notesType);
        }

        private static void AddUserWebSites(MASchemaType type, IManagementAgentParameters config)
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
                Fields = new List<MASchemaField>() { webSitePrimary, webSiteValue },
                FieldName = "websites",
                PropertyName = "Websites",
                IsPrimaryCandidateType = true,
                KnownTypes = config.WebsitesAttributeFixedTypes?.ToList(),
                CanPatch = false
            };

            type.Attributes.Add(webSiteType);
        }

        private static void AddGroupAliases(MASchemaType type)
        {
            MASchemaCollection<string> aliasesList = new MASchemaCollection<string>
            {
                Api = "groupaliases",
                AttributeName = "aliases",
                FieldName = "aliases",
                PropertyName = "Aliases",
                CanPatch = true,
                IsReadOnly = false
            };

            type.Attributes.Add(aliasesList);

            MASchemaCollection<string> nonEditableAliasesList = new MASchemaCollection<string>
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
            MASchemaCollection<string> aliasesList = new MASchemaCollection<string>
            {
                Api = "useraliases",
                AttributeName = "aliases",
                FieldName = "aliases",
                PropertyName = "Aliases",
                CanPatch = true,
                IsReadOnly = false
            };

            type.Attributes.Add(aliasesList);

            MASchemaCollection<string> nonEditableAliasesList = new MASchemaCollection<string>
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

        private static void AddUserPhonesAttributes(MASchemaType type, IManagementAgentParameters config)
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

            MASchemaCustomTypeList<Phone> phonesType = new MASchemaCustomTypeList<Phone>
            {
                Api = "user",
                AttributeName = "phones",
                Fields = new List<MASchemaField>() { phonesValue },
                FieldName = "phones",
                PropertyName = "Phones",
                IsPrimaryCandidateType = true,
                KnownTypes = config.PhonesAttributeFixedTypes?.ToList(),
                CanPatch = false
            };

            type.Attributes.Add(phonesType);
        }

        private static void AddUserOrganizationsAttributes(MASchemaType type, IManagementAgentParameters config)
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
                Fields = new List<MASchemaField>() { name, title, department, symbol, location, description, domain, costCenter },
                FieldName = "organizations",
                PropertyName = "Organizations",
                IsPrimaryCandidateType = true,
                KnownTypes = config.OrganizationsAttributeFixedTypes?.ToList(),
                CanPatch = false
            };

            type.Attributes.Add(customType);
        }

        private static void AddUserAddresses(MASchemaType type, IManagementAgentParameters config)
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
                Fields = new List<MASchemaField>() { sourceIsStructured, formatted, poBox, extendedAddress, streetAddress, locality, region, postalCode, country, countryCode },
                FieldName = "addresses",
                PropertyName = "Addresses",
                IsPrimaryCandidateType = true,
                KnownTypes = config.AddressesAttributeFixedTypes?.ToList(),
                CanPatch = false
            };

            type.Attributes.Add(customType);
        }

        private static void AddUserRelations(MASchemaType type, IManagementAgentParameters config)
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
                Fields = new List<MASchemaField>() { value },
                FieldName = "relations",
                PropertyName = "Relations",
                KnownTypes = config.RelationsAttributeFixedTypes?.ToList(),
                CanPatch = false
            };

            type.Attributes.Add(customType);
        }

        private static void AddUserExternalIds(MASchemaType type, IManagementAgentParameters config)
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
                Fields = new List<MASchemaField>() { value },
                FieldName = "externalIds",
                PropertyName = "ExternalIds",
                KnownTypes = config.ExternalIDsAttributeFixedTypes?.ToList(),
                CanPatch = false
            };

            type.Attributes.Add(customType);
        }

        private static void AddUserIms(MASchemaType type, IManagementAgentParameters config)
        {
            MASchemaField im = new MASchemaField
            {
                AttributeType = AttributeType.String,
                FieldName = "im",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                PropertyName = "IMAddress",
                AttributeNamePart = "address"
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

            MASchemaCustomTypeList<IM> customType = new MASchemaCustomTypeList<IM>
            {
                Api = "user",
                AttributeName = "ims",
                Fields = new List<MASchemaField>() { im, protocol },
                FieldName = "ims",
                PropertyName = "Ims",
                IsPrimaryCandidateType = true,
                KnownTypes = config.IMsAttributeFixedTypes?.ToList(),
                CanPatch = false
            };

            type.Attributes.Add(customType);
        }

        public static void CreateGoogleAppsCustomSchema()
        {
            G.Schema schema = new G.Schema();

            schema.SchemaName = SchemaConstants.CustomGoogleAppsSchemaName;
            schema.Fields = new List<SchemaFieldSpec>();
            schema.Fields.Add(new SchemaFieldSpec()
            {
                FieldName = SchemaConstants.CustomSchemaObjectType,
                FieldType = "STRING",
                MultiValued = false,
                ReadAccessType = "ADMINS_AND_SELF"
            });

            SchemaRequestFactory.CreateSchema("my_customer", schema);

        }
    }
}
