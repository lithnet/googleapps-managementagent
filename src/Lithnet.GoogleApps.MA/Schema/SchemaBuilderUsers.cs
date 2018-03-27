using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Lithnet.GoogleApps.ManagedObjects;
using Microsoft.MetadirectoryServices;
using G = Google.Apis.Admin.Directory.directory_v1.Data;
using Website = Lithnet.GoogleApps.ManagedObjects.Website;

namespace Lithnet.GoogleApps.MA
{
    internal class SchemaBuilderUsers : ISchemaTypeBuilder
    {
        public virtual string TypeName => "user";

        public virtual MASchemaType GetSchemaType(IManagementAgentParameters config)
        {
            MASchemaType type = new MASchemaType
            {
                AttributeAdapters = new List<IAttributeAdapter>(),
                Name = "user",
                AnchorAttributeNames = new[] { "id" },
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
                NullValueRepresentation = NullValueRepresentation.NullPlaceHolder,
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
                NullValueRepresentation = NullValueRepresentation.NullPlaceHolder,
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
                NullValueRepresentation = NullValueRepresentation.NullPlaceHolder,
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
                NullValueRepresentation = NullValueRepresentation.NullPlaceHolder,
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
                NullValueRepresentation = NullValueRepresentation.NullPlaceHolder,
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

        private static void AddUserNames(MASchemaType type)
        {
            AdapterSubfield givenName = new AdapterSubfield
            {
                AttributeType = AttributeType.String,
                FieldName = "givenName",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                PropertyName = "GivenName",
                AttributeNamePart = "givenName",
                NullValueRepresentation = NullValueRepresentation.NullPlaceHolder
            };

            AdapterSubfield familyName = new AdapterSubfield
            {
                AttributeType = AttributeType.String,
                FieldName = "familyName",
                IsMultivalued = false,
                PropertyName = "FamilyName",
                Operation = AttributeOperation.ImportExport,
                NullValueRepresentation = NullValueRepresentation.NullPlaceHolder,
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

        private static void AddUserNotes(MASchemaType type)
        {
            AdapterSubfield notesValue = new AdapterSubfield
            {
                AttributeType = AttributeType.String,
                FieldName = "value",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                PropertyName = "Value",
                AttributeNamePart = "value",
                NullValueRepresentation = NullValueRepresentation.NullPlaceHolder
            };

            AdapterSubfield notesContentType = new AdapterSubfield
            {
                AttributeType = AttributeType.String,
                FieldName = "contentType",
                IsMultivalued = false,
                PropertyName = "ContentType",
                Operation = AttributeOperation.ImportExport,
                AttributeNamePart = "contentType",
                NullValueRepresentation = NullValueRepresentation.NullPlaceHolder
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

            AdapterCustomTypeList<ManagedObjects.Website> webSiteType = new AdapterCustomTypeList<Website>
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

            AdapterCustomTypeList<ManagedObjects.Organization> customType = new AdapterCustomTypeList<ManagedObjects.Organization>
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

            AdapterCustomTypeList<ManagedObjects.Relation> customType = new AdapterCustomTypeList<ManagedObjects.Relation>
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

                    f.FieldName = $"{field.FieldName}";
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