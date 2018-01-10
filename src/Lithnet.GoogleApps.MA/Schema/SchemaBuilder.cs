using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using G = Google.Apis.Admin.Directory.directory_v1.Data;
using Google.GData.Client;
using Google.GData.Contacts;
using Google.GData.Extensions;
using Lithnet.GoogleApps.ManagedObjects;
using Microsoft.MetadirectoryServices;
using Relation = Lithnet.GoogleApps.ManagedObjects.Relation;
using Website = Lithnet.GoogleApps.ManagedObjects.Website;
using GDataOrganization = Google.GData.Extensions.Organization;
using GDataEmail = Google.GData.Extensions.EMail;
using GDataIM = Google.GData.Extensions.IMAddress;
using Organization = Lithnet.GoogleApps.ManagedObjects.Organization;

namespace Lithnet.GoogleApps.MA
{
    internal static class SchemaBuilder
    {
        public static MASchemaTypes GetSchema(IManagementAgentParameters config)
        {
            MASchemaTypes types = new MASchemaTypes
            {
                SchemaBuilder.GetSchema(SchemaConstants.User, config),
                SchemaBuilder.GetSchema(SchemaConstants.Group, config),
                SchemaBuilder.GetSchema(SchemaConstants.Contact, config),
                SchemaBuilder.GetSchema(SchemaConstants.Domain, config)
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

                case SchemaConstants.Domain:
                    return SchemaBuilder.GetDomainSchema(config);
            }

            throw new InvalidOperationException();
        }

        public static MASchemaType GetDomainSchema(IManagementAgentParameters config)
        {
            MASchemaType type = new MASchemaType
            {
                AttributeAdapters = new List<IAttributeAdapter>(),
                Name = "domain",
                AnchorAttributeName = SchemaConstants.DomainName,
                SupportsPatch = false,
            };

            type.ApiInterface = new ApiInterfaceDomain(config.CustomerID, type);

            AdapterPropertyValue domainName = new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                FieldName = "domainName",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportOnly,
                AttributeName = "domainName",
                PropertyName = "DomainName",
                Api = "domain",
                SupportsPatch = false,
                IsArrayAttribute = false,
                IsAnchor = true
            };

            type.AttributeAdapters.Add(domainName);

            AdapterPropertyValue isPrimary = new AdapterPropertyValue
            {
                AttributeType = AttributeType.Boolean,
                FieldName = "isPrimary",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportOnly,
                AttributeName = "isPrimary",
                PropertyName = "IsPrimary",
                Api = "domain",
                SupportsPatch = false,
                IsArrayAttribute = false
            };

            type.AttributeAdapters.Add(isPrimary);

            AdapterPropertyValue verified = new AdapterPropertyValue
            {
                AttributeType = AttributeType.Boolean,
                FieldName = "verified",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportOnly,
                AttributeName = "verified",
                PropertyName = "Verified",
                Api = "domain",
                SupportsPatch = false,
                IsArrayAttribute = false
            };

            type.AttributeAdapters.Add(verified);

            AdapterCollection<string> domainAliases = new AdapterCollection<string>
            {
                Api = "domain",
                AttributeName = "domainAliases",
                FieldName = "domainAliases",
                PropertyName = "DomainAliasNames",
                SupportsPatch = false,
                AttributeType = AttributeType.String,
                Operation = AttributeOperation.ImportOnly
            };

            type.AttributeAdapters.Add(domainAliases);

            return type;
        }

        public static MASchemaType GetContactSchema(IManagementAgentParameters config)
        {
            MASchemaType type = new MASchemaType
            {
                AttributeAdapters = new List<IAttributeAdapter>(),
                Name = "contact",
                AnchorAttributeName = "id",
                SupportsPatch = false,
            };

            type.ApiInterface = new ApiInterfaceContact(config.Domain, type);

            AdapterPropertyValue occupation = new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                FieldName = "occupation",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                AttributeName = "occupation",
                PropertyName = "Occupation",
                Api = "contact",
                SupportsPatch = false,
                IsArrayAttribute = false
            };

            type.AttributeAdapters.Add(occupation);

            AdapterPropertyValue billingInformation = new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                FieldName = "billingInformation",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                AttributeName = "billingInformation",
                PropertyName = "BillingInformation",
                Api = "contact",
                SupportsPatch = false,
                IsArrayAttribute = false
            };

            type.AttributeAdapters.Add(billingInformation);

            AdapterPropertyValue birthday = new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                FieldName = "birthday",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                AttributeName = "birthday",
                PropertyName = "Birthday",
                Api = "contact",
                SupportsPatch = false,
                IsArrayAttribute = false
            };

            type.AttributeAdapters.Add(birthday);

            AdapterPropertyValue directoryServer = new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                FieldName = "directoryServer",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                AttributeName = "directoryServer",
                PropertyName = "DirectoryServer",
                Api = "contact",
                SupportsPatch = false,
                IsArrayAttribute = false
            };

            type.AttributeAdapters.Add(directoryServer);


            AdapterPropertyValue initials = new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                FieldName = "initials",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                AttributeName = "initials",
                PropertyName = "Initials",
                Api = "contact",
                SupportsPatch = false,
                IsArrayAttribute = false
            };

            type.AttributeAdapters.Add(initials);

            AdapterPropertyValue maidenName = new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                FieldName = "maidenName",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                AttributeName = "maidenName",
                PropertyName = "MaidenName",
                Api = "contact",
                SupportsPatch = false,
                IsArrayAttribute = false
            };

            type.AttributeAdapters.Add(maidenName);

            AdapterPropertyValue mileage = new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                FieldName = "mileage",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                AttributeName = "mileage",
                PropertyName = "Mileage",
                Api = "contact",
                SupportsPatch = false,
                IsArrayAttribute = false
            };

            type.AttributeAdapters.Add(mileage);

            AdapterPropertyValue nickname = new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                FieldName = "nickname",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                AttributeName = "nickname",
                PropertyName = "Nickname",
                Api = "contact",
                SupportsPatch = false,
                IsArrayAttribute = false
            };

            type.AttributeAdapters.Add(nickname);

            // There is a bug in the contact implementation where priority thinks its occupation, causing an error when saving contacts
            // So no support for priority!

            //AdapterPropertyValue priority = new AdapterPropertyValue
            //{
            //    AttributeType = AttributeType.String,
            //    FieldName = "priority",
            //    IsMultivalued = false,
            //    Operation = AttributeOperation.ImportExport,
            //    AttributeName = "priority",
            //    PropertyName = "Priority",
            //    Api = "contact",
            //    SupportsPatch = false,
            //    IsArrayAttribute = false
            //};
            //
            //type.Attributes.Add(priority);

            AdapterPropertyValue sensitivity = new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                FieldName = "sensitivity",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                AttributeName = "sensitivity",
                PropertyName = "Sensitivity",
                Api = "contact",
                SupportsPatch = false,
                IsArrayAttribute = false
            };

            type.AttributeAdapters.Add(sensitivity);

            AdapterPropertyValue shortName = new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                FieldName = "shortName",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                AttributeName = "shortName",
                PropertyName = "ShortName",
                Api = "contact",
                SupportsPatch = false,
                IsArrayAttribute = false
            };

            type.AttributeAdapters.Add(shortName);

            AdapterPropertyValue subject = new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                FieldName = "subject",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                AttributeName = "subject",
                PropertyName = "Subject",
                Api = "contact",
                SupportsPatch = false,
                IsArrayAttribute = false
            };

            type.AttributeAdapters.Add(subject);

            AdapterPropertyValue location = new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                FieldName = "where",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                AttributeName = "location",
                PropertyName = "Location",
                Api = "contact",
                SupportsPatch = false,
                IsArrayAttribute = false
            };

            type.AttributeAdapters.Add(location);

            AdapterPropertyValue id = new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                FieldName = "id",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportOnly,
                AttributeName = "id",
                PropertyName = "Id",
                Api = "contact",
                SupportsPatch = true,
                IsAnchor = true,
                IsArrayAttribute = false,
                CastForImport = (value) => ((AtomId)value).AbsoluteUri,
            };

            type.AttributeAdapters.Add(id);

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
            AdapterSubfield address = new AdapterSubfield
            {
                AttributeType = AttributeType.String,
                FieldName = "address",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                PropertyName = "Address",
                AttributeNamePart = null
            };

            AdapterGDataCommonAttributeList<GDataEmail> customType = new AdapterGDataCommonAttributeList<GDataEmail>
            {
                Api = "contact",
                AttributeName = "email",
                Fields = new List<AdapterSubfield>() { address },
                FieldName = "email",
                PropertyName = "Emails",
                KnownTypes = config.EmailsAttributeFixedTypes?.ToList(),
                SupportsPatch = false,
                IsEmpty = (t) => string.IsNullOrWhiteSpace(t.Address),
                KnownRels = new Dictionary<string, string>() { { "http://schemas.google.com/g/2005#work", "work" }, { "http://schemas.google.com/g/2005#home", "home" }, { "http://schemas.google.com/g/2005#other", "other" } }
            };

            type.AttributeAdapters.Add(customType);
        }

        private static void AddContactOrganizationsAttributes(MASchemaType type, IManagementAgentParameters config)
        {

            AdapterSubfield name = new AdapterSubfield
            {
                AttributeType = AttributeType.String,
                FieldName = "orgName",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                PropertyName = "Name",
                AttributeNamePart = "name"
            };

            AdapterSubfield title = new AdapterSubfield
            {
                AttributeType = AttributeType.String,
                FieldName = "orgTitle",
                IsMultivalued = false,
                PropertyName = "Title",
                Operation = AttributeOperation.ImportExport,
                AttributeNamePart = "title"
            };

            AdapterSubfield department = new AdapterSubfield
            {
                AttributeType = AttributeType.String,
                FieldName = "orgDepartment",
                IsMultivalued = false,
                PropertyName = "Department",
                Operation = AttributeOperation.ImportExport,
                AttributeNamePart = "department"
            };

            AdapterSubfield symbol = new AdapterSubfield
            {
                AttributeType = AttributeType.String,
                FieldName = "orgSymbol",
                IsMultivalued = false,
                PropertyName = "Symbol",
                Operation = AttributeOperation.ImportExport,
                AttributeNamePart = "symbol"
            };

            AdapterSubfield jobDescription = new AdapterSubfield
            {
                AttributeType = AttributeType.String,
                FieldName = "orgJobDescription",
                IsMultivalued = false,
                PropertyName = "JobDescription",
                Operation = AttributeOperation.ImportExport,
                AttributeNamePart = "jobDescription"
            };


            AdapterSubfield location = new AdapterSubfield
            {
                AttributeType = AttributeType.String,
                FieldName = "where",
                IsMultivalued = false,
                PropertyName = "Location",
                Operation = AttributeOperation.ImportExport,
                AttributeNamePart = "location"
            };

            AdapterGDataCommonAttributeList<GDataOrganization> customType = new AdapterGDataCommonAttributeList<GDataOrganization>
            {
                Api = "contact",
                AttributeName = "organizations",
                Fields = new List<AdapterSubfield>() { name, title, department, symbol, location, jobDescription },
                FieldName = "organizations",
                PropertyName = "Organizations",
                KnownTypes = config.OrganizationsAttributeFixedTypes?.ToList(),
                SupportsPatch = false,
                IsEmpty = (t) =>
                {
                    return string.IsNullOrWhiteSpace(t.Department) &&
                           string.IsNullOrWhiteSpace(t.JobDescription) &&
                           string.IsNullOrWhiteSpace(t.Name) &&
                           string.IsNullOrWhiteSpace(t.Title) &&
                           string.IsNullOrWhiteSpace(t.Symbol) &&
                           string.IsNullOrWhiteSpace(t.Location);
                },
                KnownRels = new Dictionary<string, string>() { { "http://schemas.google.com/g/2005#work", "work" } }
            };

            type.AttributeAdapters.Add(customType);
        }

        private static void AddContactExternalIds(MASchemaType type, IManagementAgentParameters config)
        {
            AdapterSubfield value = new AdapterSubfield
            {
                AttributeType = AttributeType.String,
                FieldName = "value",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                PropertyName = "Value",
                AttributeNamePart = null
            };

            AdapterGDataSimpleAttributeList<ExternalId> customType = new AdapterGDataSimpleAttributeList<ExternalId>
            {
                Api = "contact",
                AttributeName = "externalIds",
                Fields = new List<AdapterSubfield>() { value },
                FieldName = "externalIds",
                PropertyName = "ExternalIds",
                KnownTypes = config.ExternalIDsAttributeFixedTypes?.ToList(),
                SupportsPatch = false,
                KnownRels = new HashSet<string>() { "account", "customer", "network", "organization" }
            };

            type.AttributeAdapters.Add(customType);
        }

        private static void AddContactPhones(MASchemaType type, IManagementAgentParameters config)
        {
            AdapterSubfield phonesValue = new AdapterSubfield
            {
                AttributeType = AttributeType.String,
                FieldName = "value",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                PropertyName = "Value",
                AttributeNamePart = null
            };

            AdapterGDataCommonAttributeList<PhoneNumber> phonesType = new AdapterGDataCommonAttributeList<PhoneNumber>
            {
                Api = "contact",
                AttributeName = "phones",
                Fields = new List<AdapterSubfield>() { phonesValue },
                FieldName = "phones",
                PropertyName = "Phonenumbers",
                KnownTypes = config.PhonesAttributeFixedTypes?.ToList(),
                SupportsPatch = false,
                IsEmpty = (t) => string.IsNullOrWhiteSpace(t.Value),
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

            type.AttributeAdapters.Add(phonesType);
        }

        private static void AddContactIms(MASchemaType type, IManagementAgentParameters config)
        {
            AdapterSubfield im = new AdapterSubfield
            {
                AttributeType = AttributeType.String,
                FieldName = "address",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                PropertyName = "Address",
                AttributeNamePart = "address"
            };

            AdapterSubfield protocol = new AdapterSubfield
            {
                AttributeType = AttributeType.String,
                FieldName = "protocol",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                PropertyName = "Protocol",
                AttributeNamePart = "protocol"
            };

            AdapterGDataCommonAttributeList<GDataIM> customType = new AdapterGDataCommonAttributeList<GDataIM>
            {
                Api = "contact",
                AttributeName = "ims",
                Fields = new List<AdapterSubfield>() { im, protocol },
                FieldName = "ims",
                PropertyName = "IMs",
                KnownTypes = config.IMsAttributeFixedTypes?.ToList(),
                SupportsPatch = false,
                IsEmpty = (t) => string.IsNullOrWhiteSpace(t.Address),

                KnownRels = new Dictionary<string, string>()
                {
                    { "http://schemas.google.com/g/2005#work", "work" } ,
                    { "http://schemas.google.com/g/2005#netmeeting", "netmeeting" },
                    { "http://schemas.google.com/g/2005#home", "home" }
                }

            };

            type.AttributeAdapters.Add(customType);
        }

        public static MASchemaType GetUserSchema(IManagementAgentParameters config)
        {
            MASchemaType type = new MASchemaType
            {
                AttributeAdapters = new List<IAttributeAdapter>(),
                Name = "user",
                AnchorAttributeName = "id",
                SupportsPatch = true,
            };

            type.ApiInterface = new ApiInterfaceUser(type);

            AdapterPropertyValue orgUnitPath = new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                FieldName = "orgUnitPath",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                AttributeName = "orgUnitPath",
                PropertyName = "OrgUnitPath",
                Api = "user",
                SupportsPatch = true,
                UseNullPlaceHolder = true,
                IsArrayAttribute = false
            };

            type.AttributeAdapters.Add(orgUnitPath);

            AdapterPropertyValue includeInGal = new AdapterPropertyValue
            {
                AttributeType = AttributeType.Boolean,
                FieldName = "includeInGlobalAddressList",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                AttributeName = "includeInGlobalAddressList",
                PropertyName = "IncludeInGlobalAddressList",
                Api = "user",
                SupportsPatch = true,
                UseNullPlaceHolder = true,
                IsArrayAttribute = false,
                CastForImport = (i) => i ?? false
            };

            type.AttributeAdapters.Add(includeInGal);

            AdapterPropertyValue suspended = new AdapterPropertyValue
            {
                AttributeType = AttributeType.Boolean,
                FieldName = "suspended",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                AttributeName = "suspended",
                PropertyName = "Suspended",
                Api = "user",
                SupportsPatch = true,
                UseNullPlaceHolder = true,
                IsArrayAttribute = false,
                CastForImport = (i) => i ?? false
            };

            type.AttributeAdapters.Add(suspended);

            AdapterPropertyValue changePasswordAtNextLogin = new AdapterPropertyValue
            {
                AttributeType = AttributeType.Boolean,
                FieldName = "changePasswordAtNextLogin",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                AttributeName = "changePasswordAtNextLogin",
                PropertyName = "ChangePasswordAtNextLogin",
                Api = "user",
                SupportsPatch = true,
                UseNullPlaceHolder = true,
                IsArrayAttribute = false,
                CastForImport = (i) => i ?? false
            };

            type.AttributeAdapters.Add(changePasswordAtNextLogin);

            AdapterPropertyValue ipWhitelisted = new AdapterPropertyValue
            {
                AttributeType = AttributeType.Boolean,
                FieldName = "ipWhitelisted",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportOnly,
                AttributeName = "ipWhitelisted",
                PropertyName = "IpWhitelisted",
                Api = "user",
                SupportsPatch = true,
                IsArrayAttribute = false,
                CastForImport = (i) => i ?? false
            };

            type.AttributeAdapters.Add(ipWhitelisted);

            AdapterPropertyValue id = new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                FieldName = "id",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportOnly,
                AttributeName = "id",
                PropertyName = "Id",
                Api = "user",
                SupportsPatch = true,
                IsAnchor = true,
                IsArrayAttribute = false
            };

            type.AttributeAdapters.Add(id);

            AdapterPropertyValue primaryEmail = new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                FieldName = "primaryEmail",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportOnly,
                AttributeName = "primaryEmail",
                PropertyName = "PrimaryEmail",
                Api = "user",
                SupportsPatch = true,
                IsArrayAttribute = false
            };

            type.AttributeAdapters.Add(primaryEmail);

            AdapterPropertyValue isAdmin = new AdapterPropertyValue
            {
                AttributeType = AttributeType.Boolean,
                FieldName = "isAdmin",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                AttributeName = "isAdmin",
                PropertyName = "IsAdmin",
                Api = "usermakeadmin",
                SupportsPatch = true,
                UseNullPlaceHolder = true,
                IsArrayAttribute = false,
                CastForImport = (i) => i ?? false
            };

            type.AttributeAdapters.Add(isAdmin);

            AdapterPropertyValue isDelegatedAdmin = new AdapterPropertyValue
            {
                AttributeType = AttributeType.Boolean,
                FieldName = "isDelegatedAdmin",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportOnly,
                AttributeName = "isDelegatedAdmin",
                PropertyName = "IsDelegatedAdmin",
                Api = "user",
                SupportsPatch = true,
                IsArrayAttribute = false,
                CastForImport = (i) => i ?? false
            };

            type.AttributeAdapters.Add(isDelegatedAdmin);

            AdapterPropertyValue lastLoginTime = new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                FieldName = "lastLoginTime",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportOnly,
                AttributeName = "lastLoginTime",
                PropertyName = "LastLoginTime",
                Api = "user",
                SupportsPatch = true,
                IsArrayAttribute = false
            };

            type.AttributeAdapters.Add(lastLoginTime);

            AdapterPropertyValue creationTime = new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                FieldName = "creationTime",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportOnly,
                AttributeName = "creationTime",
                PropertyName = "CreationTime",
                Api = "user",
                SupportsPatch = true,
                IsArrayAttribute = false
            };

            type.AttributeAdapters.Add(creationTime);

            AdapterPropertyValue deletionTime = new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                FieldName = "deletionTime",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportOnly,
                AttributeName = "deletionTime",
                PropertyName = "DeletionTime",
                Api = "user",
                SupportsPatch = true,
                IsArrayAttribute = false
            };

            type.AttributeAdapters.Add(deletionTime);

            AdapterPropertyValue agreedToTerms = new AdapterPropertyValue
            {
                AttributeType = AttributeType.Boolean,
                FieldName = "agreedToTerms",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportOnly,
                AttributeName = "agreedToTerms",
                PropertyName = "AgreedToTerms",
                Api = "user",
                SupportsPatch = true,
                IsArrayAttribute = false,
                CastForImport = (i) => i ?? false
            };

            type.AttributeAdapters.Add(agreedToTerms);

            AdapterPropertyValue suspensionReason = new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                FieldName = "suspensionReason",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportOnly,
                AttributeName = "suspensionReason",
                PropertyName = "SuspensionReason",
                Api = "user",
                SupportsPatch = true,
                IsArrayAttribute = false
            };

            type.AttributeAdapters.Add(suspensionReason);

            AdapterPropertyValue isMailboxSetup = new AdapterPropertyValue
            {
                AttributeType = AttributeType.Boolean,
                FieldName = "isMailboxSetup",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportOnly,
                AttributeName = "isMailboxSetup",
                PropertyName = "IsMailboxSetup",
                Api = "user",
                SupportsPatch = true,
                IsArrayAttribute = false,
                CastForImport = (i) => i ?? false
            };

            type.AttributeAdapters.Add(isMailboxSetup);

            AdapterPropertyValue thumbnailPhotoUrl = new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                FieldName = "thumbnailPhotoUrl",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportOnly,
                AttributeName = "thumbnailPhotoUrl",
                PropertyName = "ThumbnailPhotoUrl",
                Api = "user",
                SupportsPatch = true,
                IsArrayAttribute = false
            };

            type.AttributeAdapters.Add(thumbnailPhotoUrl);

            AdapterPropertyValue thumbnailPhotoEtag = new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                FieldName = "thumbnailPhotoEtag",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportOnly,
                AttributeName = "thumbnailPhotoEtag",
                PropertyName = "ThumbnailPhotoEtag",
                Api = "user",
                SupportsPatch = true,
                IsArrayAttribute = false
            };

            type.AttributeAdapters.Add(thumbnailPhotoEtag);

            AdapterPropertyValue customerId = new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                FieldName = "customerId",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportOnly,
                AttributeName = "customerId",
                PropertyName = "CustomerId",
                Api = "user",
                SupportsPatch = true,
                IsArrayAttribute = false
            };

            type.AttributeAdapters.Add(customerId);

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
            SchemaBuilder.AddUserCustomSchema(type, config);

            return type;
        }

        public static MASchemaType GetAdvancedUserSchema(IManagementAgentParameters config)
        {
            MASchemaType userType = SchemaBuilder.GetUserSchema(config);
            userType.Name = SchemaConstants.AdvancedUser;
            userType.ApiInterface = new ApiInterfaceAdvancedUser(userType);

            AdapterCollection<string> delegates = new AdapterCollection<string>
            {
                AttributeType = AttributeType.Reference,
                FieldName = null,
                Operation = AttributeOperation.ImportExport,
                AttributeName = SchemaConstants.Delegate,
                PropertyName = "Delegates",
                Api = "userdelegates",
                SupportsPatch = true,
            };

            userType.AttributeAdapters.Add(delegates);

            return userType;
        }

        public static MASchemaType GetGroupSchema()
        {
            MASchemaType type = new MASchemaType
            {
                AttributeAdapters = new List<IAttributeAdapter>(),
                Name = "group",
                AnchorAttributeName = "id",
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
                UseNullPlaceHolder = true,
                IsArrayAttribute = false,
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
                UseNullPlaceHolder = true,
                IsArrayAttribute = false,
                CastForImport = (i) => string.IsNullOrEmpty((string)i) ? null : i,
                CastForExport = (i) => (string)i == Constants.NullValuePlaceholder ? "" : i
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
                IsArrayAttribute = false
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
                IsArrayAttribute = false
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
                IsArrayAttribute = false
            };

            type.AttributeAdapters.Add(name);

            SchemaBuilder.AddGroupAliases(type);
            SchemaBuilder.AddGroupSettings(type);
            SchemaBuilder.AddGroupMembers(type);
            return type;
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
                UseNullPlaceHolder = true,
                IsArrayAttribute = false
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
                UseNullPlaceHolder = true,
                IsArrayAttribute = false
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
                UseNullPlaceHolder = true,
                IsArrayAttribute = false
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
                UseNullPlaceHolder = true,
                IsArrayAttribute = false
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
                UseNullPlaceHolder = true,
                IsArrayAttribute = false
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
                UseNullPlaceHolder = true,
                IsArrayAttribute = false
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
                UseNullPlaceHolder = true,
                IsArrayAttribute = false
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
                UseNullPlaceHolder = true,
                IsArrayAttribute = false,
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
                UseNullPlaceHolder = true,
                IsArrayAttribute = false
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
                UseNullPlaceHolder = true,
                IsArrayAttribute = false,
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
                UseNullPlaceHolder = true,
                IsArrayAttribute = false
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
                UseNullPlaceHolder = true,
                IsArrayAttribute = false,
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
                UseNullPlaceHolder = true,
                IsArrayAttribute = false,
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
                UseNullPlaceHolder = true,
                IsArrayAttribute = false,
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
                UseNullPlaceHolder = true,
                IsArrayAttribute = false
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
                UseNullPlaceHolder = true,
                IsArrayAttribute = false
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
                UseNullPlaceHolder = true,
                IsArrayAttribute = false
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
                UseNullPlaceHolder = true,
                IsArrayAttribute = false
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
                UseNullPlaceHolder = true,
                IsArrayAttribute = false,
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
                UseNullPlaceHolder = true,
                IsArrayAttribute = false
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
                UseNullPlaceHolder = true,
                IsArrayAttribute = false,
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
                UseNullPlaceHolder = true,
                IsArrayAttribute = false,
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
                UseNullPlaceHolder = true,
                IsArrayAttribute = false
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
                UseNullPlaceHolder = true,
                IsArrayAttribute = false
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
                UseNullPlaceHolder = true,
                IsArrayAttribute = false,
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
                UseNullPlaceHolder = true,
                IsArrayAttribute = false
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
                UseNullPlaceHolder = true,
                IsArrayAttribute = false
            };

            type.AttributeAdapters.Add(whoCanContactOwner);
        }

        private static void AddUserNames(MASchemaType type)
        {
            AdapterSubfield givenName = new AdapterSubfield
            {
                AttributeType = AttributeType.String,
                FieldName = "givenName",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                PropertyName = "GivenName",
                AttributeNamePart = "givenName"
            };

            AdapterSubfield familyName = new AdapterSubfield
            {
                AttributeType = AttributeType.String,
                FieldName = "familyName",
                IsMultivalued = false,
                PropertyName = "FamilyName",
                Operation = AttributeOperation.ImportExport,
                AttributeNamePart = "familyName"
            };

            AdapterNestedType schemaItem = new AdapterNestedType
            {
                Api = "user",
                AttributeName = "name",
                Fields = new List<AdapterSubfield>() { givenName, familyName },
                FieldName = "name",
                PropertyName = "Name",
                SupportsPatch = false
            };

            type.AttributeAdapters.Add(schemaItem);
        }

        private static void AddContactNames(MASchemaType type)
        {
            AdapterSubfield givenName = new AdapterSubfield
            {
                AttributeType = AttributeType.String,
                FieldName = "givenName",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                PropertyName = "GivenName",
                AttributeNamePart = "givenName"
            };

            AdapterSubfield familyName = new AdapterSubfield
            {
                AttributeType = AttributeType.String,
                FieldName = "familyName",
                IsMultivalued = false,
                PropertyName = "FamilyName",
                Operation = AttributeOperation.ImportExport,
                AttributeNamePart = "familyName"
            };

            AdapterSubfield fullName = new AdapterSubfield
            {
                AttributeType = AttributeType.String,
                FieldName = "fullName",
                IsMultivalued = false,
                PropertyName = "FullName",
                Operation = AttributeOperation.ImportExport,
                AttributeNamePart = "fullName"
            };

            AdapterNestedType schemaItem = new AdapterNestedType
            {
                Api = "contact",
                AttributeName = "name",
                Fields = new List<AdapterSubfield>() { givenName, familyName, fullName },
                FieldName = "name",
                PropertyName = "Name",
                SupportsPatch = false
            };

            type.AttributeAdapters.Add(schemaItem);
        }

        private static void AddUserNotes(MASchemaType type)
        {
            AdapterSubfield notesValue = new AdapterSubfield
            {
                AttributeType = AttributeType.String,
                FieldName = "value",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                PropertyName = "Value",
                AttributeNamePart = "value"
            };

            AdapterSubfield notesContentType = new AdapterSubfield
            {
                AttributeType = AttributeType.String,
                FieldName = "contentType",
                IsMultivalued = false,
                PropertyName = "ContentType",
                Operation = AttributeOperation.ImportExport,
                AttributeNamePart = "contentType"
            };

            AdapterNestedType notesType = new AdapterNestedType
            {
                Api = "user",
                AttributeName = "notes",
                Fields = new List<AdapterSubfield>() { notesContentType, notesValue },
                FieldName = "notes",
                PropertyName = "Notes",
                SupportsPatch = false
            };

            type.AttributeAdapters.Add(notesType);
        }

        private static void AddUserWebSites(MASchemaType type, IManagementAgentParameters config)
        {
            AdapterSubfield webSiteValue = new AdapterSubfield
            {
                AttributeType = AttributeType.String,
                FieldName = "value",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                PropertyName = "Value",
                AttributeNamePart = null
            };

            AdapterSubfield webSitePrimary = new AdapterSubfield
            {
                AttributeType = AttributeType.Boolean,
                FieldName = "primary",
                IsMultivalued = false,
                PropertyName = "Primary",
                Operation = AttributeOperation.ImportExport,
                AttributeNamePart = "primary"
            };

            AdapterCustomTypeList<Website> webSiteType = new AdapterCustomTypeList<Website>
            {
                Api = "user",
                AttributeName = "websites",
                Fields = new List<AdapterSubfield>() { webSitePrimary, webSiteValue },
                FieldName = "websites",
                PropertyName = "Websites",
                IsPrimaryCandidateType = true,
                KnownTypes = config.WebsitesAttributeFixedTypes?.ToList(),
                SupportsPatch = false
            };

            type.AttributeAdapters.Add(webSiteType);
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

        private static void AddUserAliases(MASchemaType type)
        {
            AdapterCollection<string> aliasesList = new AdapterCollection<string>
            {
                Api = "useraliases",
                AttributeName = "aliases",
                FieldName = "aliases",
                PropertyName = "Aliases",
                SupportsPatch = true,
                Operation = AttributeOperation.ImportExport
            };

            type.AttributeAdapters.Add(aliasesList);

            AdapterCollection<string> nonEditableAliasesList = new AdapterCollection<string>
            {
                Api = "user",
                AttributeName = "nonEditableAliases",
                FieldName = "nonEditableAliases",
                PropertyName = "NonEditableAliases",
                SupportsPatch = false,
                Operation = AttributeOperation.ImportOnly
            };

            type.AttributeAdapters.Add(nonEditableAliasesList);
        }

        private static void AddUserPhonesAttributes(MASchemaType type, IManagementAgentParameters config)
        {
            AdapterSubfield phonesValue = new AdapterSubfield
            {
                AttributeType = AttributeType.String,
                FieldName = "value",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                PropertyName = "Value",
                AttributeNamePart = null
            };

            AdapterCustomTypeList<Phone> phonesType = new AdapterCustomTypeList<Phone>
            {
                Api = "user",
                AttributeName = "phones",
                Fields = new List<AdapterSubfield>() { phonesValue },
                FieldName = "phones",
                PropertyName = "Phones",
                IsPrimaryCandidateType = true,
                KnownTypes = config.PhonesAttributeFixedTypes?.ToList(),
                SupportsPatch = false
            };

            type.AttributeAdapters.Add(phonesType);
        }

        private static void AddUserOrganizationsAttributes(MASchemaType type, IManagementAgentParameters config)
        {
            AdapterSubfield name = new AdapterSubfield
            {
                AttributeType = AttributeType.String,
                FieldName = "name",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                PropertyName = "Name",
                AttributeNamePart = "name"
            };

            AdapterSubfield title = new AdapterSubfield
            {
                AttributeType = AttributeType.String,
                FieldName = "title",
                IsMultivalued = false,
                PropertyName = "Title",
                Operation = AttributeOperation.ImportExport,
                AttributeNamePart = "title"
            };

            AdapterSubfield department = new AdapterSubfield
            {
                AttributeType = AttributeType.String,
                FieldName = "department",
                IsMultivalued = false,
                PropertyName = "Department",
                Operation = AttributeOperation.ImportExport,
                AttributeNamePart = "department"
            };

            AdapterSubfield symbol = new AdapterSubfield
            {
                AttributeType = AttributeType.String,
                FieldName = "symbol",
                IsMultivalued = false,
                PropertyName = "Symbol",
                Operation = AttributeOperation.ImportExport,
                AttributeNamePart = "symbol"
            };

            AdapterSubfield location = new AdapterSubfield
            {
                AttributeType = AttributeType.String,
                FieldName = "location",
                IsMultivalued = false,
                PropertyName = "Location",
                Operation = AttributeOperation.ImportExport,
                AttributeNamePart = "location"
            };

            AdapterSubfield description = new AdapterSubfield
            {
                AttributeType = AttributeType.String,
                FieldName = "description",
                IsMultivalued = false,
                PropertyName = "Description",
                Operation = AttributeOperation.ImportExport,
                AttributeNamePart = "description"
            };

            AdapterSubfield domain = new AdapterSubfield
            {
                AttributeType = AttributeType.String,
                FieldName = "domain",
                IsMultivalued = false,
                PropertyName = "Domain",
                Operation = AttributeOperation.ImportExport,
                AttributeNamePart = "domain"
            };

            AdapterSubfield costCenter = new AdapterSubfield
            {
                AttributeType = AttributeType.String,
                FieldName = "costCenter",
                IsMultivalued = false,
                PropertyName = "CostCenter",
                Operation = AttributeOperation.ImportExport,
                AttributeNamePart = "costCenter"
            };

            AdapterCustomTypeList<Organization> customType = new AdapterCustomTypeList<Organization>
            {
                Api = "user",
                AttributeName = "organizations",
                Fields = new List<AdapterSubfield>() { name, title, department, symbol, location, description, domain, costCenter },
                FieldName = "organizations",
                PropertyName = "Organizations",
                IsPrimaryCandidateType = true,
                KnownTypes = config.OrganizationsAttributeFixedTypes?.ToList(),
                SupportsPatch = false
            };

            type.AttributeAdapters.Add(customType);
        }

        private static void AddUserAddresses(MASchemaType type, IManagementAgentParameters config)
        {
            AdapterSubfield sourceIsStructured = new AdapterSubfield
            {
                AttributeType = AttributeType.Boolean,
                FieldName = "sourceIsStructured",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                PropertyName = "SourceIsStructured",
                AttributeNamePart = "sourceIsStructured"
            };

            AdapterSubfield formatted = new AdapterSubfield
            {
                AttributeType = AttributeType.String,
                FieldName = "formatted",
                IsMultivalued = false,
                PropertyName = "Formatted",
                Operation = AttributeOperation.ImportExport,
                AttributeNamePart = "formatted"
            };

            AdapterSubfield poBox = new AdapterSubfield
            {
                AttributeType = AttributeType.String,
                FieldName = "poBox",
                IsMultivalued = false,
                PropertyName = "POBox",
                Operation = AttributeOperation.ImportExport,
                AttributeNamePart = "poBox"
            };

            AdapterSubfield extendedAddress = new AdapterSubfield
            {
                AttributeType = AttributeType.String,
                FieldName = "extendedAddress",
                IsMultivalued = false,
                PropertyName = "ExtendedAddress",
                Operation = AttributeOperation.ImportExport,
                AttributeNamePart = "extendedAddress"
            };


            AdapterSubfield streetAddress = new AdapterSubfield
            {
                AttributeType = AttributeType.String,
                FieldName = "streetAddress",
                IsMultivalued = false,
                PropertyName = "StreetAddress",
                Operation = AttributeOperation.ImportExport,
                AttributeNamePart = "streetAddress"
            };

            AdapterSubfield locality = new AdapterSubfield
            {
                AttributeType = AttributeType.String,
                FieldName = "locality",
                IsMultivalued = false,
                PropertyName = "Locality",
                Operation = AttributeOperation.ImportExport,
                AttributeNamePart = "locality"
            };

            AdapterSubfield region = new AdapterSubfield
            {
                AttributeType = AttributeType.String,
                FieldName = "region",
                IsMultivalued = false,
                PropertyName = "Region",
                Operation = AttributeOperation.ImportExport,
                AttributeNamePart = "region"
            };

            AdapterSubfield postalCode = new AdapterSubfield
            {
                AttributeType = AttributeType.String,
                FieldName = "postalCode",
                IsMultivalued = false,
                PropertyName = "PostalCode",
                Operation = AttributeOperation.ImportExport,
                AttributeNamePart = "postalCode"
            };

            AdapterSubfield country = new AdapterSubfield
            {
                AttributeType = AttributeType.String,
                FieldName = "country",
                IsMultivalued = false,
                PropertyName = "Country",
                Operation = AttributeOperation.ImportExport,
                AttributeNamePart = "country"
            };

            AdapterSubfield countryCode = new AdapterSubfield
            {
                AttributeType = AttributeType.String,
                FieldName = "countryCode",
                IsMultivalued = false,
                PropertyName = "CountryCode",
                Operation = AttributeOperation.ImportExport,
                AttributeNamePart = "countryCode"
            };

            AdapterCustomTypeList<Address> customType = new AdapterCustomTypeList<Address>
            {
                Api = "user",
                AttributeName = "addresses",
                Fields = new List<AdapterSubfield>() { sourceIsStructured, formatted, poBox, extendedAddress, streetAddress, locality, region, postalCode, country, countryCode },
                FieldName = "addresses",
                PropertyName = "Addresses",
                IsPrimaryCandidateType = true,
                KnownTypes = config.AddressesAttributeFixedTypes?.ToList(),
                SupportsPatch = false
            };

            type.AttributeAdapters.Add(customType);
        }

        private static void AddUserRelations(MASchemaType type, IManagementAgentParameters config)
        {
            AdapterSubfield value = new AdapterSubfield
            {
                AttributeType = AttributeType.String,
                FieldName = "value",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                PropertyName = "Value",
                AttributeNamePart = null
            };

            AdapterCustomTypeList<Relation> customType = new AdapterCustomTypeList<Relation>
            {
                Api = "user",
                AttributeName = "relations",
                Fields = new List<AdapterSubfield>() { value },
                FieldName = "relations",
                PropertyName = "Relations",
                KnownTypes = config.RelationsAttributeFixedTypes?.ToList(),
                SupportsPatch = false
            };

            type.AttributeAdapters.Add(customType);
        }

        private static void AddUserExternalIds(MASchemaType type, IManagementAgentParameters config)
        {
            AdapterSubfield value = new AdapterSubfield
            {
                AttributeType = AttributeType.String,
                FieldName = "value",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                PropertyName = "Value",
                AttributeNamePart = null
            };

            AdapterCustomTypeList<ExternalID> customType = new AdapterCustomTypeList<ExternalID>
            {
                Api = "user",
                AttributeName = "externalIds",
                Fields = new List<AdapterSubfield>() { value },
                FieldName = "externalIds",
                PropertyName = "ExternalIds",
                KnownTypes = config.ExternalIDsAttributeFixedTypes?.ToList(),
                SupportsPatch = false
            };

            type.AttributeAdapters.Add(customType);
        }

        private static void AddUserIms(MASchemaType type, IManagementAgentParameters config)
        {
            AdapterSubfield im = new AdapterSubfield
            {
                AttributeType = AttributeType.String,
                FieldName = "im",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                PropertyName = "IMAddress",
                AttributeNamePart = "address"
            };

            AdapterSubfield protocol = new AdapterSubfield
            {
                AttributeType = AttributeType.String,
                FieldName = "protocol",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                PropertyName = "Protocol",
                AttributeNamePart = "protocol"
            };

            AdapterCustomTypeList<IM> customType = new AdapterCustomTypeList<IM>
            {
                Api = "user",
                AttributeName = "ims",
                Fields = new List<AdapterSubfield>() { im, protocol },
                FieldName = "ims",
                PropertyName = "Ims",
                IsPrimaryCandidateType = true,
                KnownTypes = config.IMsAttributeFixedTypes?.ToList(),
                SupportsPatch = false
            };

            type.AttributeAdapters.Add(customType);
        }

        private static void AddUserCustomSchema(MASchemaType type, IManagementAgentParameters config)
        {
            G.Schemas schemas = null;

            try
            {
                schemas = SchemaRequestFactory.ListSchemas(config.CustomerID);
            }
            catch (Google.GoogleApiException ex)
            {
                if (ex.HttpStatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    return;
                }

                throw;
            }

            if (schemas == null)
            {
                return;
            }

            AdapterCustomSchemas customSchemas = new AdapterCustomSchemas();
            customSchemas.Api = "user";

            foreach (G.Schema schema in schemas.SchemasValue)
            {
                if (schema.SchemaName.Equals(SchemaConstants.CustomGoogleAppsSchemaName))
                {
                    continue;
                }

                AdapterCustomSchema customSchema = new AdapterCustomSchema();

                customSchema.SchemaName = schema.SchemaName;
                
                foreach (G.SchemaFieldSpec field in schema.Fields)
                {
                    AdapterCustomSchemaField f;

                    if (field.MultiValued ?? false)
                    {
                        f = new AdapterCustomSchemaMultivaluedField();
                    }
                    else
                    {
                        f = new AdapterCustomSchemaSingleValuedField();
                    }

                    switch (field.FieldType.ToLowerInvariant())
                    {
                        case "int64":
                            f.AttributeType = AttributeType.Integer;
                            break;

                        case "bool":
                            f.AttributeType = AttributeType.Boolean;
                            break;

                        default:
                            f.AttributeType = AttributeType.String;
                            break;
                    }
                    
                    if (f.IsMultivalued && !(f.AttributeType == AttributeType.String || f.AttributeType == AttributeType.Integer))
                    {
                        continue;
                    }

                    f.FieldName = $"{field.FieldName}";
                    f.SchemaName = schema.SchemaName;
                    f.Operation = AttributeOperation.ImportExport;
                    f.MmsAttributeName = Regex.Replace($"{schema.SchemaName}_{field.FieldName}", "[^a-zA-Z0-9_\\-]", "_", RegexOptions.IgnoreCase);
                    f.PropertyName = $"{field.FieldName}";
                    f.UseNullPlaceHolder = true;
                    f.FieldSpec = field;

                    customSchema.Fields.Add(f);
                }

                if (customSchema.Fields.Count > 0)
                {
                    customSchemas.CustomSchemas.Add(customSchema);
                }
            }

            if (customSchemas.CustomSchemas.Count > 0)
            {
                type.AttributeAdapters.Add(customSchemas);
            }
        }

        public static void CreateGoogleAppsCustomSchema()
        {
            G.Schema schema = new G.Schema();

            schema.SchemaName = SchemaConstants.CustomGoogleAppsSchemaName;
            schema.Fields = new List<G.SchemaFieldSpec>();
            schema.Fields.Add(new G.SchemaFieldSpec()
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
