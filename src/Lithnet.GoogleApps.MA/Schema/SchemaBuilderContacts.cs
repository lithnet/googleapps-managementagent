using System.Collections.Generic;
using System.Linq;
using Google.GData.Client;
using Google.GData.Contacts;
using Google.GData.Extensions;
using Microsoft.MetadirectoryServices;

namespace Lithnet.GoogleApps.MA
{
    internal class SchemaBuilderContacts : ISchemaTypeBuilder
    {
        public string TypeName => "contact";

        public IEnumerable<MASchemaType> GetSchemaTypes(IManagementAgentParameters config)
        {
            MASchemaType type = new MASchemaType
            {
                AttributeAdapters = new List<IAttributeAdapter>(),
                Name = "contact",
                AnchorAttributeNames = new[] { "id" },
                SupportsPatch = false,
            };

            type.ApiInterface = new ApiInterfaceContact(config.Domain, config.ContactDNPrefix, type, config);

            AdapterPropertyValue occupation = new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                GoogleApiFieldName = "occupation",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                MmsAttributeName = "occupation",
                ManagedObjectPropertyName = "Occupation",
                Api = "contact",
                SupportsPatch = false,
            };

            type.AttributeAdapters.Add(occupation);

            AdapterPropertyValue billingInformation = new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                GoogleApiFieldName = "billingInformation",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                MmsAttributeName = "billingInformation",
                ManagedObjectPropertyName = "BillingInformation",
                Api = "contact",
                SupportsPatch = false,
            };

            type.AttributeAdapters.Add(billingInformation);

            AdapterPropertyValue birthday = new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                GoogleApiFieldName = "birthday",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                MmsAttributeName = "birthday",
                ManagedObjectPropertyName = "Birthday",
                Api = "contact",
                SupportsPatch = false,
            };

            type.AttributeAdapters.Add(birthday);

            AdapterPropertyValue directoryServer = new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                GoogleApiFieldName = "directoryServer",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                MmsAttributeName = "directoryServer",
                ManagedObjectPropertyName = "DirectoryServer",
                Api = "contact",
                SupportsPatch = false,
            };

            type.AttributeAdapters.Add(directoryServer);

            AdapterPropertyValue initials = new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                GoogleApiFieldName = "initials",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                MmsAttributeName = "initials",
                ManagedObjectPropertyName = "Initials",
                Api = "contact",
                SupportsPatch = false,
            };

            type.AttributeAdapters.Add(initials);

            AdapterPropertyValue maidenName = new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                GoogleApiFieldName = "maidenName",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                MmsAttributeName = "maidenName",
                ManagedObjectPropertyName = "MaidenName",
                Api = "contact",
                SupportsPatch = false,
            };

            type.AttributeAdapters.Add(maidenName);

            AdapterPropertyValue mileage = new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                GoogleApiFieldName = "mileage",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                MmsAttributeName = "mileage",
                ManagedObjectPropertyName = "Mileage",
                Api = "contact",
                SupportsPatch = false,
            };

            type.AttributeAdapters.Add(mileage);

            AdapterPropertyValue nickname = new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                GoogleApiFieldName = "nickname",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                MmsAttributeName = "nickname",
                ManagedObjectPropertyName = "Nickname",
                Api = "contact",
                SupportsPatch = false,
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
            //                //};
            //
            //type.Attributes.Add(priority);

            AdapterPropertyValue sensitivity = new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                GoogleApiFieldName = "sensitivity",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                MmsAttributeName = "sensitivity",
                ManagedObjectPropertyName = "Sensitivity",
                Api = "contact",
                SupportsPatch = false,
            };

            type.AttributeAdapters.Add(sensitivity);

            AdapterPropertyValue shortName = new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                GoogleApiFieldName = "shortName",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                MmsAttributeName = "shortName",
                ManagedObjectPropertyName = "ShortName",
                Api = "contact",
                SupportsPatch = false,
            };

            type.AttributeAdapters.Add(shortName);

            AdapterPropertyValue subject = new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                GoogleApiFieldName = "subject",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                MmsAttributeName = "subject",
                ManagedObjectPropertyName = "Subject",
                Api = "contact",
                SupportsPatch = false,
            };

            type.AttributeAdapters.Add(subject);

            AdapterPropertyValue location = new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                GoogleApiFieldName = "where",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                MmsAttributeName = "location",
                ManagedObjectPropertyName = "Location",
                Api = "contact",
                SupportsPatch = false,
            };

            type.AttributeAdapters.Add(location);

            AdapterPropertyValue id = new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                GoogleApiFieldName = "id",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportOnly,
                MmsAttributeName = "id",
                ManagedObjectPropertyName = "Id",
                Api = "contact",
                SupportsPatch = true,
                IsAnchor = true,
                CastForImport = (value) => ((AtomId)value).AbsoluteUri,
            };

            type.AttributeAdapters.Add(id);

            this.AddContactNames(type);
            this.AddContactOrganizationsAttributes(type, config);
            this.AddContactExternalIds(type, config);
            this.AddContactEmailAttributes(type, config);
            this.AddContactIms(type, config);
            this.AddContactPhones(type, config);

            yield return type;
        }

        private void AddContactNames(MASchemaType type)
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
                MmsAttributeNameSuffix = "familyName",
                NullValueRepresentation = NullValueRepresentation.NullPlaceHolder
            };

            AdapterSubfield fullName = new AdapterSubfield
            {
                AttributeType = AttributeType.String,
                GoogleApiFieldName = "fullName",
                IsMultivalued = false,
                ManagedObjectPropertyName = "FullName",
                Operation = AttributeOperation.ImportExport,
                MmsAttributeNameSuffix = "fullName",
                NullValueRepresentation = NullValueRepresentation.NullPlaceHolder
            };

            AdapterNestedType schemaItem = new AdapterNestedType
            {
                Api = "contact",
                MmsAttributeNameBase = "name",
                Fields = new List<AdapterSubfield>() { givenName, familyName, fullName },
                GoogleApiFieldName = "name",
                ManagedObjectPropertyName = "Name",
                SupportsPatch = false
            };

            type.AttributeAdapters.Add(schemaItem);
        }

        private void AddContactEmailAttributes(MASchemaType type, IManagementAgentParameters config)
        {
            AdapterSubfield address = new AdapterSubfield
            {
                AttributeType = AttributeType.String,
                GoogleApiFieldName = "address",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                ManagedObjectPropertyName = "Address",
                MmsAttributeNameSuffix = null
            };

            AdapterGDataCommonAttributeList<EMail> customType = new AdapterGDataCommonAttributeList<EMail>
            {
                Api = "contact",
                MmsAttributeNameBase = "email",
                Fields = new List<AdapterSubfield>() { address },
                GoogleApiFieldName = "email",
                PropertyName = "Emails",
                KnownTypes = config.EmailsAttributeFixedTypes?.ToList(),
                SupportsPatch = false,
                IsEmpty = (t) => string.IsNullOrWhiteSpace(t.Address),
                KnownRels = new Dictionary<string, string>() { { "http://schemas.google.com/g/2005#work", "work" }, { "http://schemas.google.com/g/2005#home", "home" }, { "http://schemas.google.com/g/2005#other", "other" } }
            };

            type.AttributeAdapters.Add(customType);
        }

        private void AddContactOrganizationsAttributes(MASchemaType type, IManagementAgentParameters config)
        {
            AdapterSubfield name = new AdapterSubfield
            {
                AttributeType = AttributeType.String,
                GoogleApiFieldName = "orgName",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                ManagedObjectPropertyName = "Name",
                MmsAttributeNameSuffix = "name"
            };

            AdapterSubfield title = new AdapterSubfield
            {
                AttributeType = AttributeType.String,
                GoogleApiFieldName = "orgTitle",
                IsMultivalued = false,
                ManagedObjectPropertyName = "Title",
                Operation = AttributeOperation.ImportExport,
                MmsAttributeNameSuffix = "title"
            };

            AdapterSubfield department = new AdapterSubfield
            {
                AttributeType = AttributeType.String,
                GoogleApiFieldName = "orgDepartment",
                IsMultivalued = false,
                ManagedObjectPropertyName = "Department",
                Operation = AttributeOperation.ImportExport,
                MmsAttributeNameSuffix = "department"
            };

            AdapterSubfield symbol = new AdapterSubfield
            {
                AttributeType = AttributeType.String,
                GoogleApiFieldName = "orgSymbol",
                IsMultivalued = false,
                ManagedObjectPropertyName = "Symbol",
                Operation = AttributeOperation.ImportExport,
                MmsAttributeNameSuffix = "symbol"
            };

            AdapterSubfield jobDescription = new AdapterSubfield
            {
                AttributeType = AttributeType.String,
                GoogleApiFieldName = "orgJobDescription",
                IsMultivalued = false,
                ManagedObjectPropertyName = "JobDescription",
                Operation = AttributeOperation.ImportExport,
                MmsAttributeNameSuffix = "jobDescription"
            };

            AdapterSubfield location = new AdapterSubfield
            {
                AttributeType = AttributeType.String,
                GoogleApiFieldName = "where",
                IsMultivalued = false,
                ManagedObjectPropertyName = "Location",
                Operation = AttributeOperation.ImportExport,
                MmsAttributeNameSuffix = "location"
            };

            AdapterGDataCommonAttributeList<Organization> customType = new AdapterGDataCommonAttributeList<Organization>
            {
                Api = "contact",
                MmsAttributeNameBase = "organizations",
                Fields = new List<AdapterSubfield>() { name, title, department, symbol, location, jobDescription },
                GoogleApiFieldName = "organizations",
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

        private void AddContactExternalIds(MASchemaType type, IManagementAgentParameters config)
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

            AdapterGDataSimpleAttributeList<ExternalId> customType = new AdapterGDataSimpleAttributeList<ExternalId>
            {
                Api = "contact",
                MmsAttributeNameBase = "externalIds",
                Fields = new List<AdapterSubfield>() { value },
                GoogleApiFieldName = "externalIds",
                ManagedObjectPropertyName = "ExternalIds",
                KnownTypes = config.ExternalIDsAttributeFixedTypes?.ToList(),
                SupportsPatch = false,
                KnownRels = new HashSet<string>() { "account", "customer", "network", "organization" }
            };

            type.AttributeAdapters.Add(customType);
        }

        private void AddContactPhones(MASchemaType type, IManagementAgentParameters config)
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

            AdapterGDataCommonAttributeList<PhoneNumber> phonesType = new AdapterGDataCommonAttributeList<PhoneNumber>
            {
                Api = "contact",
                MmsAttributeNameBase = "phones",
                Fields = new List<AdapterSubfield>() { phonesValue },
                GoogleApiFieldName = "phones",
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

        private void AddContactIms(MASchemaType type, IManagementAgentParameters config)
        {
            AdapterSubfield im = new AdapterSubfield
            {
                AttributeType = AttributeType.String,
                GoogleApiFieldName = "address",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                ManagedObjectPropertyName = "Address",
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

            AdapterGDataCommonAttributeList<IMAddress> customType = new AdapterGDataCommonAttributeList<IMAddress>
            {
                Api = "contact",
                MmsAttributeNameBase = "ims",
                Fields = new List<AdapterSubfield>() { im, protocol },
                GoogleApiFieldName = "ims",
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
    }
}