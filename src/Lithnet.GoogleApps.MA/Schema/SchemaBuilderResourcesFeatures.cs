using System.Collections.Generic;
using Microsoft.MetadirectoryServices;

namespace Lithnet.GoogleApps.MA
{
    internal class SchemaBuilderResourcesFeatures : ISchemaTypeBuilder
    {
        public string TypeName => "feature";

        public IEnumerable<MASchemaType> GetSchemaTypes(IManagementAgentParameters config)
        {
            MASchemaType type = new MASchemaType
            {
                AttributeAdapters = new List<IAttributeAdapter>(),
                Name = "feature",
                AnchorAttributeNames = new[] { "id" },
                SupportsPatch = true,
            };

            type.ApiInterface = new ApiInterfaceFeature(config.CustomerID, type, config);

            type.AttributeAdapters.Add(new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                GoogleApiFieldName = "name",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportOnly,
                MmsAttributeName = "id",
                ManagedObjectPropertyName = "Name",
                Api = "feature",
                SupportsPatch = true,
                IsAnchor = true
            });

            yield return type;
        }
    }
}
