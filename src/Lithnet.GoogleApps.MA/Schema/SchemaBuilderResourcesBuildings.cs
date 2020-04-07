using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.MetadirectoryServices;

namespace Lithnet.GoogleApps.MA
{
    internal class SchemaBuilderResourcesBuildings : ISchemaTypeBuilder
    {
        public string TypeName => "building";

        public IEnumerable<MASchemaType> GetSchemaTypes(IManagementAgentParameters config)
        {
            MASchemaType type = new MASchemaType
            {
                AttributeAdapters = new List<IAttributeAdapter>(),
                Name = "building",
                AnchorAttributeNames = new[] {"id"},
                SupportsPatch = true,
            };

            type.ApiInterface = new ApiInterfaceBuilding(config.CustomerID, type, config);

            type.AttributeAdapters.Add(new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                GoogleApiFieldName = "buildingId",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportOnly,
                MmsAttributeName = "id",
                ManagedObjectPropertyName = "BuildingId",
                Api = "building",
                SupportsPatch = true,
                IsAnchor = true
            });

            type.AttributeAdapters.Add(new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                GoogleApiFieldName = "buildingName",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                MmsAttributeName = "buildingName",
                ManagedObjectPropertyName = "BuildingName",
                Api = "building",
                SupportsPatch = true,
                NullValueRepresentation = NullValueRepresentation.EmptyString,
                IsAnchor = false
            });

            type.AttributeAdapters.Add(new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                GoogleApiFieldName = "description",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                MmsAttributeName = "description",
                ManagedObjectPropertyName = "Description",
                Api = "building",
                SupportsPatch = true,
                NullValueRepresentation = NullValueRepresentation.EmptyString,
                IsAnchor = false
            });

            type.AttributeAdapters.Add(new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                GoogleApiFieldName = "floorNames",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                MmsAttributeName = "floorNames",
                ManagedObjectPropertyName = "FloorNames",
                Api = "building",
                SupportsPatch = true,
                NullValueRepresentation = NullValueRepresentation.EmptyString,
                CastForExport = i => ((string) i)?.Split(new char[] {','}, StringSplitOptions.RemoveEmptyEntries).ToList(),
                CastForImport = i => i == null ? null : string.Join(",", ((IList<string>) i)),
                IsAnchor = false
            });

            AdapterSubfield latitude = new AdapterSubfield
            {
                AttributeType = AttributeType.String,
                GoogleApiFieldName = "latitude",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                ManagedObjectPropertyName = "Latitude",
                MmsAttributeNameSuffix = "latitude",
                NullValueRepresentation = NullValueRepresentation.DoubleZero,
                CastForExport = value =>
                {
                    if (value == null)
                    {
                        return null;
                    }

                    return double.Parse(value.ToString());
                },
                CastForImport = value => ((double?) value)?.ToString("R")
            };

            AdapterSubfield longitude = new AdapterSubfield
            {
                AttributeType = AttributeType.String,
                GoogleApiFieldName = "longitude",
                IsMultivalued = false,
                ManagedObjectPropertyName = "Longitude",
                Operation = AttributeOperation.ImportExport,
                MmsAttributeNameSuffix = "longitude",
                NullValueRepresentation = NullValueRepresentation.DoubleZero,
                CastForExport = value =>
                {
                    if (value == null)
                    {
                        return null;
                    }

                    return double.Parse(value.ToString());
                },
                CastForImport = value => ((double?) value)?.ToString("R")
            };

            AdapterNestedType schemaItem = new AdapterNestedType
            {
                Api = "building",
                MmsAttributeNameBase = "coordinates",
                Fields = new List<AdapterSubfield>() {latitude, longitude},
                GoogleApiFieldName = "coordinates",
                ManagedObjectPropertyName = "Coordinates",
                SupportsPatch = false
            };

            type.AttributeAdapters.Add(schemaItem);

            yield return type;
        }
    }
}