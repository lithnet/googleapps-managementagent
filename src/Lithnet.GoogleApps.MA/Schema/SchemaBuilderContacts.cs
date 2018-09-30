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

        public MASchemaType GetSchemaType(IManagementAgentParameters config)
        {
            MASchemaType type = new MASchemaType
            {
                AttributeAdapters = new List<IAttributeAdapter>(),
                Name = "contact",
                AnchorAttributeNames = new[] { "id" },
                SupportsPatch = false,
            };

            type.ApiInterface = new ApiInterfaceContact(config.Domain, config.ContactDNPrefix, type);

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
                FieldName = "sensitivity",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                AttributeName = "sensitivity",
                PropertyName = "Sensitivity",
                Api = "contact",
                SupportsPatch = false,
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
                CastForImport = (value) => ((AtomId)value).AbsoluteUri,
            };

            type.AttributeAdapters.Add(id);

            this.AddContactNames(type);
            this.AddContactOrganizationsAttributes(type, config);
            this.AddContactExternalIds(type, config);
            this.AddContactEmailAttributes(type, config);
            this.AddContactIms(type, config);
            this.AddContactPhones(type, config);

            return type;
        }

        private void AddContactNames(MASchemaType type)
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
                AttributeNamePart = "familyName",
                NullValueRepresentation = NullValueRepresentation.NullPlaceHolder

            };

            AdapterSubfield fullName = new AdapterSubfield
            {
                AttributeType = AttributeType.String,
                FieldName = "fullName",
                IsMultivalued = false,
                PropertyName = "FullName",
                Operation = AttributeOperation.ImportExport,
                AttributeNamePart = "fullName",
                NullValueRepresentation = NullValueRepresentation.NullPlaceHolder
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

        private void AddContactEmailAttributes(MASchemaType type, IManagementAgentParameters config)
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

            AdapterGDataCommonAttributeList<EMail> customType = new AdapterGDataCommonAttributeList<EMail>
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

        private void AddContactOrganizationsAttributes(MASchemaType type, IManagementAgentParameters config)
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

            AdapterGDataCommonAttributeList<Organization> customType = new AdapterGDataCommonAttributeList<Organization>
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

        private void AddContactExternalIds(MASchemaType type, IManagementAgentParameters config)
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

        private void AddContactPhones(MASchemaType type, IManagementAgentParameters config)
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

        private void AddContactIms(MASchemaType type, IManagementAgentParameters config)
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

            AdapterGDataCommonAttributeList<IMAddress> customType = new AdapterGDataCommonAttributeList<IMAddress>
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
    }
}