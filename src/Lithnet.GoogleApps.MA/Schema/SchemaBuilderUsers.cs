using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using Google.Apis.Auth.OAuth2.Responses;
using Lithnet.GoogleApps.ManagedObjects;
using Microsoft.MetadirectoryServices;
using G = Google.Apis.Admin.Directory.directory_v1.Data;
using Website = Lithnet.GoogleApps.ManagedObjects.Website;

namespace Lithnet.GoogleApps.MA
{
    internal class SchemaBuilderUsers : ISchemaTypeBuilder
    {
        public virtual string TypeName => "user";

        public IEnumerable<MASchemaType> GetSchemaTypes(IManagementAgentParameters config)
        {
            MASchemaType type = new MASchemaType
            {
                AttributeAdapters = new List<IAttributeAdapter>(),
                Name = SchemaConstants.User,
                AnchorAttributeNames = new[] { "id" },
                SupportsPatch = true,
            };

            this.BuildBaseSchema(type, config);

            if (config.EnableAdvancedUserAttributes)
            {
                SchemaBuilderUsers.AddGmailSettingsAttributes(type, config);
            }

            yield return type;

            if (!config.SchemaService.HasSchema(config.CustomerID, SchemaConstants.CustomGoogleAppsSchemaName))
            {
                yield break;
            }

            foreach (string objectType in config.CustomUserObjectClasses)
            {
                type = new MASchemaType
                {
                    AttributeAdapters = new List<IAttributeAdapter>(),
                    Name = objectType,
                    AnchorAttributeNames = new[] { "id" },
                    SupportsPatch = true,
                };

                this.BuildBaseSchema(type, config);
                SchemaBuilderUsers.AddGmailSettingsAttributes(type, config);

                yield return type;
            }
        }

        protected MASchemaType BuildBaseSchema(MASchemaType type, IManagementAgentParameters config)
        {
            type.ApiInterface = new ApiInterfaceUser(type, config);

            AdapterPropertyValue orgUnitPath = new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                GoogleApiFieldName = "orgUnitPath",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                MmsAttributeName = "orgUnitPath",
                ManagedObjectPropertyName = "OrgUnitPath",
                Api = "user",
                SupportsPatch = true,
                NullValueRepresentation = NullValueRepresentation.NullPlaceHolder,
            };

            type.AttributeAdapters.Add(orgUnitPath);

            AdapterPropertyValue includeInGal = new AdapterPropertyValue
            {
                AttributeType = AttributeType.Boolean,
                GoogleApiFieldName = "includeInGlobalAddressList",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                MmsAttributeName = "includeInGlobalAddressList",
                ManagedObjectPropertyName = "IncludeInGlobalAddressList",
                Api = "user",
                SupportsPatch = true,
                NullValueRepresentation = NullValueRepresentation.NullPlaceHolder,
                CastForImport = (i) => i ?? false
            };

            type.AttributeAdapters.Add(includeInGal);

            AdapterPropertyValue suspended = new AdapterPropertyValue
            {
                AttributeType = AttributeType.Boolean,
                GoogleApiFieldName = "suspended",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                MmsAttributeName = "suspended",
                ManagedObjectPropertyName = "Suspended",
                Api = "user",
                SupportsPatch = true,
                NullValueRepresentation = NullValueRepresentation.NullPlaceHolder,
                CastForImport = (i) => i ?? false
            };

            type.AttributeAdapters.Add(suspended);

            AdapterPropertyValue changePasswordAtNextLogin = new AdapterPropertyValue
            {
                AttributeType = AttributeType.Boolean,
                GoogleApiFieldName = "changePasswordAtNextLogin",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                MmsAttributeName = "changePasswordAtNextLogin",
                ManagedObjectPropertyName = "ChangePasswordAtNextLogin",
                Api = "user",
                SupportsPatch = true,
                NullValueRepresentation = NullValueRepresentation.NullPlaceHolder,
                CastForImport = (i) => i ?? false
            };

            type.AttributeAdapters.Add(changePasswordAtNextLogin);

            AdapterPropertyValue ipWhitelisted = new AdapterPropertyValue
            {
                AttributeType = AttributeType.Boolean,
                GoogleApiFieldName = "ipWhitelisted",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportOnly,
                MmsAttributeName = "ipWhitelisted",
                ManagedObjectPropertyName = "IpWhitelisted",
                Api = "user",
                SupportsPatch = true,
                CastForImport = (i) => i ?? false
            };

            type.AttributeAdapters.Add(ipWhitelisted);

            AdapterPropertyValue id = new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                GoogleApiFieldName = "id",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportOnly,
                MmsAttributeName = "id",
                ManagedObjectPropertyName = "Id",
                Api = "user",
                SupportsPatch = true,
                IsAnchor = true,
            };

            type.AttributeAdapters.Add(id);

            AdapterPropertyValue primaryEmail = new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                GoogleApiFieldName = "primaryEmail",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportOnly,
                MmsAttributeName = "primaryEmail",
                ManagedObjectPropertyName = "PrimaryEmail",
                Api = "user",
                SupportsPatch = true,
            };

            type.AttributeAdapters.Add(primaryEmail);

            AdapterPropertyValue isAdmin = new AdapterPropertyValue
            {
                AttributeType = AttributeType.Boolean,
                GoogleApiFieldName = "isAdmin",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                MmsAttributeName = "isAdmin",
                ManagedObjectPropertyName = "IsAdmin",
                Api = "usermakeadmin",
                SupportsPatch = true,
                NullValueRepresentation = NullValueRepresentation.NullPlaceHolder,
                CastForImport = (i) => i ?? false
            };

            type.AttributeAdapters.Add(isAdmin);

            AdapterPropertyValue isDelegatedAdmin = new AdapterPropertyValue
            {
                AttributeType = AttributeType.Boolean,
                GoogleApiFieldName = "isDelegatedAdmin",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportOnly,
                MmsAttributeName = "isDelegatedAdmin",
                ManagedObjectPropertyName = "IsDelegatedAdmin",
                Api = "user",
                SupportsPatch = true,
                CastForImport = (i) => i ?? false
            };

            type.AttributeAdapters.Add(isDelegatedAdmin);

            AdapterPropertyValue lastLoginTime = new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                GoogleApiFieldName = "lastLoginTime",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportOnly,
                MmsAttributeName = "lastLoginTime",
                ManagedObjectPropertyName = "LastLoginTime",
                Api = "user",
                SupportsPatch = true,
            };

            type.AttributeAdapters.Add(lastLoginTime);

            AdapterPropertyValue creationTime = new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                GoogleApiFieldName = "creationTime",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportOnly,
                MmsAttributeName = "creationTime",
                ManagedObjectPropertyName = "CreationTime",
                Api = "user",
                SupportsPatch = true,
            };

            type.AttributeAdapters.Add(creationTime);

            AdapterPropertyValue deletionTime = new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                GoogleApiFieldName = "deletionTime",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportOnly,
                MmsAttributeName = "deletionTime",
                ManagedObjectPropertyName = "DeletionTime",
                Api = "user",
                SupportsPatch = true,
            };

            type.AttributeAdapters.Add(deletionTime);

            AdapterPropertyValue agreedToTerms = new AdapterPropertyValue
            {
                AttributeType = AttributeType.Boolean,
                GoogleApiFieldName = "agreedToTerms",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportOnly,
                MmsAttributeName = "agreedToTerms",
                ManagedObjectPropertyName = "AgreedToTerms",
                Api = "user",
                SupportsPatch = true,
                CastForImport = (i) => i ?? false
            };

            type.AttributeAdapters.Add(agreedToTerms);

            AdapterPropertyValue suspensionReason = new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                GoogleApiFieldName = "suspensionReason",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportOnly,
                MmsAttributeName = "suspensionReason",
                ManagedObjectPropertyName = "SuspensionReason",
                Api = "user",
                SupportsPatch = true,
            };

            type.AttributeAdapters.Add(suspensionReason);

            AdapterPropertyValue isMailboxSetup = new AdapterPropertyValue
            {
                AttributeType = AttributeType.Boolean,
                GoogleApiFieldName = "isMailboxSetup",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportOnly,
                MmsAttributeName = "isMailboxSetup",
                ManagedObjectPropertyName = "IsMailboxSetup",
                Api = "user",
                SupportsPatch = true,
                CastForImport = (i) => i ?? false
            };

            type.AttributeAdapters.Add(isMailboxSetup);

            AdapterPropertyValue thumbnailPhotoUrl = new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                GoogleApiFieldName = "thumbnailPhotoUrl",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportOnly,
                MmsAttributeName = "thumbnailPhotoUrl",
                ManagedObjectPropertyName = "ThumbnailPhotoUrl",
                Api = "user",
                SupportsPatch = true,
            };

            type.AttributeAdapters.Add(thumbnailPhotoUrl);

            AdapterPropertyValue thumbnailPhotoEtag = new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                GoogleApiFieldName = "thumbnailPhotoEtag",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportOnly,
                MmsAttributeName = "thumbnailPhotoEtag",
                ManagedObjectPropertyName = "ThumbnailPhotoEtag",
                Api = "user",
                SupportsPatch = true,
            };

            type.AttributeAdapters.Add(thumbnailPhotoEtag);

            AdapterPropertyValue customerId = new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                GoogleApiFieldName = "customerId",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportOnly,
                MmsAttributeName = "customerId",
                ManagedObjectPropertyName = "CustomerId",
                Api = "user",
                SupportsPatch = true,
            };

            type.AttributeAdapters.Add(customerId);

            SchemaBuilderUsers.AddUserNames(type);
            SchemaBuilderUsers.AddUserNotes(type);
            SchemaBuilderUsers.AddUserWebSites(type, config);
            SchemaBuilderUsers.AddUserAliases(type);
            SchemaBuilderUsers.AddUserPhonesAttributes(type, config);
            SchemaBuilderUsers.AddUserOrganizationsAttributes(type, config);
            SchemaBuilderUsers.AddUserAddresses(type, config);
            SchemaBuilderUsers.AddUserRelations(type, config);
            SchemaBuilderUsers.AddUserExternalIds(type, config);
            SchemaBuilderUsers.AddUserIms(type, config);
            SchemaBuilderUsers.AddUserCustomSchema(type, config);

            return type;
        }

        private static void AddGmailSettingsAttributes(MASchemaType type, IManagementAgentParameters config)
        {
            AdapterCollection<string> delegates = new AdapterCollection<string>
            {
                AttributeType = AttributeType.Reference,
                GoogleApiFieldName = null,
                Operation = AttributeOperation.ImportExport,
                MmsAttributeName = $"{type.Name}_{SchemaConstants.Delegate}",
                ManagedObjectPropertyName = "Delegates",
                Api = "userdelegates",
                SupportsPatch = true,
            };

            type.AttributeAdapters.Add(delegates);

            AdapterCollection<string> sendas = new AdapterCollection<string>
            {
                AttributeType = AttributeType.String,
                GoogleApiFieldName = null,
                Operation = AttributeOperation.ImportExport,
                MmsAttributeName = $"{type.Name}_{SchemaConstants.SendAs}",
                ManagedObjectPropertyName = "SendAs",
                Api = "usersendas",
                SupportsPatch = true,
            };

            type.AttributeAdapters.Add(sendas);
        }

        private static void AddUserNames(MASchemaType type)
        {
            AdapterSubfield givenName = new AdapterSubfield
            {
                AttributeType = AttributeType.String,
                GoogleApiFieldName = "givenName",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                ManagedObjectPropertyName = "GivenName",
                MmsAttributeNameSuffix = "givenName",
                NullValueRepresentation = NullValueRepresentation.NullPlaceHolder
            };

            AdapterSubfield familyName = new AdapterSubfield
            {
                AttributeType = AttributeType.String,
                GoogleApiFieldName = "familyName",
                IsMultivalued = false,
                ManagedObjectPropertyName = "FamilyName",
                Operation = AttributeOperation.ImportExport,
                NullValueRepresentation = NullValueRepresentation.NullPlaceHolder,
                MmsAttributeNameSuffix = "familyName"
            };

            AdapterNestedType schemaItem = new AdapterNestedType
            {
                Api = "user",
                MmsAttributeNameBase = "name",
                Fields = new List<AdapterSubfield>() { givenName, familyName },
                GoogleApiFieldName = "name",
                ManagedObjectPropertyName = "Name",
                SupportsPatch = false
            };

            type.AttributeAdapters.Add(schemaItem);
        }

        private static void AddUserNotes(MASchemaType type)
        {
            AdapterSubfield notesValue = new AdapterSubfield
            {
                AttributeType = AttributeType.String,
                GoogleApiFieldName = "value",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                ManagedObjectPropertyName = "Value",
                MmsAttributeNameSuffix = "value",
                NullValueRepresentation = NullValueRepresentation.NullPlaceHolder
            };

            AdapterSubfield notesContentType = new AdapterSubfield
            {
                AttributeType = AttributeType.String,
                GoogleApiFieldName = "contentType",
                IsMultivalued = false,
                ManagedObjectPropertyName = "ContentType",
                Operation = AttributeOperation.ImportExport,
                MmsAttributeNameSuffix = "contentType",
                NullValueRepresentation = NullValueRepresentation.NullPlaceHolder
            };

            AdapterNestedType notesType = new AdapterNestedType
            {
                Api = "user",
                MmsAttributeNameBase = "notes",
                Fields = new List<AdapterSubfield>() { notesContentType, notesValue },
                GoogleApiFieldName = "notes",
                ManagedObjectPropertyName = "Notes",
                SupportsPatch = false
            };

            type.AttributeAdapters.Add(notesType);
        }

        private static void AddUserWebSites(MASchemaType type, IManagementAgentParameters config)
        {
            AdapterSubfield webSiteValue = new AdapterSubfield
            {
                AttributeType = AttributeType.String,
                GoogleApiFieldName = "value",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                ManagedObjectPropertyName = "Value",
                MmsAttributeNameSuffix = null
            };

            AdapterSubfield webSitePrimary = new AdapterSubfield
            {
                AttributeType = AttributeType.Boolean,
                GoogleApiFieldName = "primary",
                IsMultivalued = false,
                ManagedObjectPropertyName = "Primary",
                Operation = AttributeOperation.ImportExport,
                MmsAttributeNameSuffix = "primary"
            };

            AdapterCustomTypeList<ManagedObjects.Website> webSiteType = new AdapterCustomTypeList<Website>
            {
                Api = "user",
                MmsAttributeNameBase = "websites",
                SubFields = new List<AdapterSubfield>() { webSitePrimary, webSiteValue },
                GoogleApiFieldName = "websites",
                ManagedObjectPropertyName = "Websites",
                IsPrimaryCandidateType = true,
                KnownTypes = config.WebsitesAttributeFixedTypes?.ToList(),
                SupportsPatch = false
            };

            type.AttributeAdapters.Add(webSiteType);
        }

        private static void AddUserAliases(MASchemaType type)
        {
            AdapterCollection<string> aliasesList = new AdapterCollection<string>
            {
                Api = "useraliases",
                MmsAttributeName = "aliases",
                GoogleApiFieldName = "aliases",
                ManagedObjectPropertyName = "Aliases",
                SupportsPatch = true,
                Operation = AttributeOperation.ImportExport
            };

            type.AttributeAdapters.Add(aliasesList);

            AdapterCollection<string> nonEditableAliasesList = new AdapterCollection<string>
            {
                Api = "user",
                MmsAttributeName = "nonEditableAliases",
                GoogleApiFieldName = "nonEditableAliases",
                ManagedObjectPropertyName = "NonEditableAliases",
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
                GoogleApiFieldName = "value",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                ManagedObjectPropertyName = "Value",
                MmsAttributeNameSuffix = null
            };

            AdapterCustomTypeList<Phone> phonesType = new AdapterCustomTypeList<Phone>
            {
                Api = "user",
                MmsAttributeNameBase = "phones",
                SubFields = new List<AdapterSubfield>() { phonesValue },
                GoogleApiFieldName = "phones",
                ManagedObjectPropertyName = "Phones",
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
                GoogleApiFieldName = "name",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                ManagedObjectPropertyName = "Name",
                MmsAttributeNameSuffix = "name"
            };

            AdapterSubfield title = new AdapterSubfield
            {
                AttributeType = AttributeType.String,
                GoogleApiFieldName = "title",
                IsMultivalued = false,
                ManagedObjectPropertyName = "Title",
                Operation = AttributeOperation.ImportExport,
                MmsAttributeNameSuffix = "title"
            };

            AdapterSubfield department = new AdapterSubfield
            {
                AttributeType = AttributeType.String,
                GoogleApiFieldName = "department",
                IsMultivalued = false,
                ManagedObjectPropertyName = "Department",
                Operation = AttributeOperation.ImportExport,
                MmsAttributeNameSuffix = "department"
            };

            AdapterSubfield symbol = new AdapterSubfield
            {
                AttributeType = AttributeType.String,
                GoogleApiFieldName = "symbol",
                IsMultivalued = false,
                ManagedObjectPropertyName = "Symbol",
                Operation = AttributeOperation.ImportExport,
                MmsAttributeNameSuffix = "symbol"
            };

            AdapterSubfield location = new AdapterSubfield
            {
                AttributeType = AttributeType.String,
                GoogleApiFieldName = "location",
                IsMultivalued = false,
                ManagedObjectPropertyName = "Location",
                Operation = AttributeOperation.ImportExport,
                MmsAttributeNameSuffix = "location"
            };

            AdapterSubfield description = new AdapterSubfield
            {
                AttributeType = AttributeType.String,
                GoogleApiFieldName = "description",
                IsMultivalued = false,
                ManagedObjectPropertyName = "Description",
                Operation = AttributeOperation.ImportExport,
                MmsAttributeNameSuffix = "description"
            };

            AdapterSubfield domain = new AdapterSubfield
            {
                AttributeType = AttributeType.String,
                GoogleApiFieldName = "domain",
                IsMultivalued = false,
                ManagedObjectPropertyName = "Domain",
                Operation = AttributeOperation.ImportExport,
                MmsAttributeNameSuffix = "domain"
            };

            AdapterSubfield costCenter = new AdapterSubfield
            {
                AttributeType = AttributeType.String,
                GoogleApiFieldName = "costCenter",
                IsMultivalued = false,
                ManagedObjectPropertyName = "CostCenter",
                Operation = AttributeOperation.ImportExport,
                MmsAttributeNameSuffix = "costCenter"
            };

            AdapterCustomTypeList<ManagedObjects.Organization> customType = new AdapterCustomTypeList<ManagedObjects.Organization>
            {
                Api = "user",
                MmsAttributeNameBase = "organizations",
                SubFields = new List<AdapterSubfield>() { name, title, department, symbol, location, description, domain, costCenter },
                GoogleApiFieldName = "organizations",
                ManagedObjectPropertyName = "Organizations",
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
                GoogleApiFieldName = "sourceIsStructured",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                ManagedObjectPropertyName = "SourceIsStructured",
                MmsAttributeNameSuffix = "sourceIsStructured"
            };

            AdapterSubfield formatted = new AdapterSubfield
            {
                AttributeType = AttributeType.String,
                GoogleApiFieldName = "formatted",
                IsMultivalued = false,
                ManagedObjectPropertyName = "Formatted",
                Operation = AttributeOperation.ImportExport,
                MmsAttributeNameSuffix = "formatted"
            };

            AdapterSubfield poBox = new AdapterSubfield
            {
                AttributeType = AttributeType.String,
                GoogleApiFieldName = "poBox",
                IsMultivalued = false,
                ManagedObjectPropertyName = "POBox",
                Operation = AttributeOperation.ImportExport,
                MmsAttributeNameSuffix = "poBox"
            };

            AdapterSubfield extendedAddress = new AdapterSubfield
            {
                AttributeType = AttributeType.String,
                GoogleApiFieldName = "extendedAddress",
                IsMultivalued = false,
                ManagedObjectPropertyName = "ExtendedAddress",
                Operation = AttributeOperation.ImportExport,
                MmsAttributeNameSuffix = "extendedAddress"
            };

            AdapterSubfield streetAddress = new AdapterSubfield
            {
                AttributeType = AttributeType.String,
                GoogleApiFieldName = "streetAddress",
                IsMultivalued = false,
                ManagedObjectPropertyName = "StreetAddress",
                Operation = AttributeOperation.ImportExport,
                MmsAttributeNameSuffix = "streetAddress"
            };

            AdapterSubfield locality = new AdapterSubfield
            {
                AttributeType = AttributeType.String,
                GoogleApiFieldName = "locality",
                IsMultivalued = false,
                ManagedObjectPropertyName = "Locality",
                Operation = AttributeOperation.ImportExport,
                MmsAttributeNameSuffix = "locality"
            };

            AdapterSubfield region = new AdapterSubfield
            {
                AttributeType = AttributeType.String,
                GoogleApiFieldName = "region",
                IsMultivalued = false,
                ManagedObjectPropertyName = "Region",
                Operation = AttributeOperation.ImportExport,
                MmsAttributeNameSuffix = "region"
            };

            AdapterSubfield postalCode = new AdapterSubfield
            {
                AttributeType = AttributeType.String,
                GoogleApiFieldName = "postalCode",
                IsMultivalued = false,
                ManagedObjectPropertyName = "PostalCode",
                Operation = AttributeOperation.ImportExport,
                MmsAttributeNameSuffix = "postalCode"
            };

            AdapterSubfield country = new AdapterSubfield
            {
                AttributeType = AttributeType.String,
                GoogleApiFieldName = "country",
                IsMultivalued = false,
                ManagedObjectPropertyName = "Country",
                Operation = AttributeOperation.ImportExport,
                MmsAttributeNameSuffix = "country"
            };

            AdapterSubfield countryCode = new AdapterSubfield
            {
                AttributeType = AttributeType.String,
                GoogleApiFieldName = "countryCode",
                IsMultivalued = false,
                ManagedObjectPropertyName = "CountryCode",
                Operation = AttributeOperation.ImportExport,
                MmsAttributeNameSuffix = "countryCode"
            };

            AdapterCustomTypeList<Address> customType = new AdapterCustomTypeList<Address>
            {
                Api = "user",
                MmsAttributeNameBase = "addresses",
                SubFields = new List<AdapterSubfield>() { sourceIsStructured, formatted, poBox, extendedAddress, streetAddress, locality, region, postalCode, country, countryCode },
                GoogleApiFieldName = "addresses",
                ManagedObjectPropertyName = "Addresses",
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
                GoogleApiFieldName = "value",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                ManagedObjectPropertyName = "Value",
                MmsAttributeNameSuffix = null
            };

            AdapterCustomTypeList<ManagedObjects.Relation> customType = new AdapterCustomTypeList<ManagedObjects.Relation>
            {
                Api = "user",
                MmsAttributeNameBase = "relations",
                SubFields = new List<AdapterSubfield>() { value },
                GoogleApiFieldName = "relations",
                ManagedObjectPropertyName = "Relations",
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
                GoogleApiFieldName = "value",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                ManagedObjectPropertyName = "Value",
                MmsAttributeNameSuffix = null
            };

            AdapterCustomTypeList<ExternalID> customType = new AdapterCustomTypeList<ExternalID>
            {
                Api = "user",
                MmsAttributeNameBase = "externalIds",
                SubFields = new List<AdapterSubfield>() { value },
                GoogleApiFieldName = "externalIds",
                ManagedObjectPropertyName = "ExternalIds",
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
                GoogleApiFieldName = "im",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                ManagedObjectPropertyName = "IMAddress",
                MmsAttributeNameSuffix = "address"
            };

            AdapterSubfield protocol = new AdapterSubfield
            {
                AttributeType = AttributeType.String,
                GoogleApiFieldName = "protocol",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                ManagedObjectPropertyName = "Protocol",
                MmsAttributeNameSuffix = "protocol"
            };

            AdapterCustomTypeList<IM> customType = new AdapterCustomTypeList<IM>
            {
                Api = "user",
                MmsAttributeNameBase = "ims",
                SubFields = new List<AdapterSubfield>() { im, protocol },
                GoogleApiFieldName = "ims",
                ManagedObjectPropertyName = "Ims",
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
                schemas = config.SchemaService.ListSchemas(config.CustomerID);
            }
            catch (Google.GoogleApiException ex)
            {
                if (ex.HttpStatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    Trace.WriteLine("Permission to read the user custom schema was denied");
                    return;
                }

                throw;
            }
            catch (TokenResponseException ex)
            {
                if (ex.StatusCode == System.Net.HttpStatusCode.Forbidden || ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    Trace.WriteLine("Permission related TokenResponseException while reading the user custom schema");
                    return;
                }
            }

            if (schemas?.SchemasValue == null)
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

                    f.GoogleApiFieldName = $"{field.FieldName}";
                    f.SchemaName = schema.SchemaName;
                    f.Operation = AttributeOperation.ImportExport;
                    f.MmsAttributeName = Regex.Replace($"{schema.SchemaName}_{field.FieldName}", "[^a-zA-Z0-9_\\-]", "_", RegexOptions.IgnoreCase);
                    f.PropertyName = $"{field.FieldName}";
                    f.NullValueRepresentation = NullValueRepresentation.NullPlaceHolder;
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
    }
}