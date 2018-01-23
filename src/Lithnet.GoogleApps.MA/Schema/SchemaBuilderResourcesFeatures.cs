using System.Collections.Generic;
using Microsoft.MetadirectoryServices;

namespace Lithnet.GoogleApps.MA
{
    internal class SchemaBuilderResourcesFeatures : ISchemaTypeBuilder
    {
        public string TypeName => "feature";

        public MASchemaType GetSchemaType(IManagementAgentParameters config)
        {
            MASchemaType type = new MASchemaType
            {
                AttributeAdapters = new List<IAttributeAdapter>(),
                Name = "feature",
                AnchorAttributeNames = new[] { "id" },
                SupportsPatch = true,
            };

            type.ApiInterface = new ApiInterfaceFeature(config.CustomerID, type);

            type.AttributeAdapters.Add(new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                FieldName = "name",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportOnly,
                AttributeName = "id",
                PropertyName = "Name",
                Api = "feature",
                SupportsPatch = true,
                IsAnchor = true
            });

            return type;
        }
    }
}
