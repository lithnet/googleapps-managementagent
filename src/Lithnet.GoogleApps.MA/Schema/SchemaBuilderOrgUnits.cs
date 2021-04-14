using System.Collections.Generic;
using Microsoft.MetadirectoryServices;

namespace Lithnet.GoogleApps.MA
{
    internal class SchemaBuilderOrgUnits : ISchemaTypeBuilder
    {
        public string TypeName => "orgUnit";

        public IEnumerable<MASchemaType> GetSchemaTypes(IManagementAgentParameters config)
        {
            if (!config.LicenseManager.IsFeatureEnabled(Features.OrgUnits))
            {
                yield break;
            }

            MASchemaType type = new MASchemaType
            {
                AttributeAdapters = new List<IAttributeAdapter>(),
                Name = "orgUnit",
                AnchorAttributeNames = new[] { "id" },
                SupportsPatch = true,
            };

            type.ApiInterface = new ApiInterfaceOrgUnit(config.CustomerID, type, config);

            type.AttributeAdapters.Add(new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                GoogleApiFieldName = "orgUnitId",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportOnly,
                MmsAttributeName = "id",
                ManagedObjectPropertyName = "OrgUnitId",
                Api = "orgUnit",
                SupportsPatch = false,
                IsAnchor = true
            });

            type.AttributeAdapters.Add(new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                GoogleApiFieldName = "parentOrgUnitId",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportOnly,
                MmsAttributeName = "parentId",
                ManagedObjectPropertyName = "ParentOrgUnitId",
                Api = "orgUnit",
                SupportsPatch = false,
                IsAnchor = false
            });

            type.AttributeAdapters.Add(new AdapterPropertyValue
            {
                AttributeType = AttributeType.Reference,
                GoogleApiFieldName = "parentOrgUnitPath",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportOnly,
                MmsAttributeName = "parentOrgUnit",
                ManagedObjectPropertyName = "ParentOrgUnitPath",
                Api = "orgUnit",
                SupportsPatch = false,
                IsAnchor = false,
                CastForExport = (val) =>
                {
                    if (val == null || !(val is string s))
                    {
                        return "/";
                    }

                    return s;
                },
                CastForImport = (val) =>
                {
                    if (val == null || !(val is string s))
                    {
                        return null;
                    }

                    if (s == "/")
                    {
                        return null;
                    }

                    return s;
                }
            });

            type.AttributeAdapters.Add(new AdapterPropertyValue
            {
                AttributeType = AttributeType.Boolean,
                GoogleApiFieldName = "blockInheritance",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                MmsAttributeName = "blockInheritance",
                ManagedObjectPropertyName = "BlockInheritance",
                Api = "orgUnit",
                SupportsPatch = true,
            });

            type.AttributeAdapters.Add(new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                GoogleApiFieldName = "description",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                MmsAttributeName = "description",
                ManagedObjectPropertyName = "Description",
                Api = "orgUnit",
                SupportsPatch = true,
                NullValueRepresentation = NullValueRepresentation.EmptyString
            });

            type.AttributeAdapters.Add(new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                GoogleApiFieldName = "name",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportOnly,
                MmsAttributeName = "displayName",
                ManagedObjectPropertyName = "Name",
                Api = "orgUnit",
                SupportsPatch = true
            });

            yield return type;
        }
    }
}