using System.Collections.Generic;
using System.Linq;
using Microsoft.MetadirectoryServices;

namespace Lithnet.GoogleApps.MA
{
    internal class SchemaBuilderDomains : ISchemaTypeBuilder
    {
        public string TypeName => "domain";

        public IEnumerable<MASchemaType> GetSchemaTypes(IManagementAgentParameters config)
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
                GoogleApiFieldName = "domainName",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportOnly,
                MmsAttributeName = "domainName",
                ManagedObjectPropertyName = "DomainName",
                Api = "domain",
                SupportsPatch = false,
                IsAnchor = true
            };

            type.AttributeAdapters.Add(domainName);

            AdapterPropertyValue isPrimary = new AdapterPropertyValue
            {
                AttributeType = AttributeType.Boolean,
                GoogleApiFieldName = "isPrimary",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportOnly,
                MmsAttributeName = "isPrimary",
                ManagedObjectPropertyName = "IsPrimary",
                Api = "domain",
                SupportsPatch = false,
            };

            type.AttributeAdapters.Add(isPrimary);

            AdapterPropertyValue verified = new AdapterPropertyValue
            {
                AttributeType = AttributeType.Boolean,
                GoogleApiFieldName = "verified",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportOnly,
                MmsAttributeName = "verified",
                ManagedObjectPropertyName = "Verified",
                Api = "domain",
                SupportsPatch = false,
            };

            type.AttributeAdapters.Add(verified);

            AdapterCollection<string> domainAliases = new AdapterCollection<string>
            {
                Api = "domain",
                MmsAttributeName = "domainAliases",
                GoogleApiFieldName = "domainAliases",
                ManagedObjectPropertyName = "DomainAliasNames",
                SupportsPatch = false,
                AttributeType = AttributeType.String,
                Operation = AttributeOperation.ImportOnly,
                GetList = (obj) =>
                {
                    return ((Google.Apis.Admin.Directory.directory_v1.Data.Domains)obj).DomainAliases?.Select(u => u.DomainAliasName).ToList();
                }
            };

            type.AttributeAdapters.Add(domainAliases);

            yield return type;
        }
    }
}