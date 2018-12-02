using System.Collections.Generic;
using Microsoft.MetadirectoryServices;

namespace Lithnet.GoogleApps.MA
{
    internal class SchemaBuilderDomains : ISchemaTypeBuilder
    {
        public string TypeName => "domain";

        public MASchemaType GetSchemaType(IManagementAgentParameters config)
        {
            MASchemaType type = new MASchemaType
            {
                AttributeAdapters = new List<IAttributeAdapter>(),
                Name = "domain",
                AnchorAttributeNames = new[] { SchemaConstants.DomainName },
                SupportsPatch = false,
            };

            type.ApiInterface = new ApiInterfaceDomain(config.CustomerID, type, config);

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
    }
}