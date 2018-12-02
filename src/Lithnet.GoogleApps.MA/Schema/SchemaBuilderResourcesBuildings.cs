using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.MetadirectoryServices;

namespace Lithnet.GoogleApps.MA
{
    internal class SchemaBuilderResourcesBuildings : ISchemaTypeBuilder
    {
        public string TypeName => "building";

        public MASchemaType GetSchemaType(IManagementAgentParameters config)
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
                FieldName = "buildingId",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportOnly,
                AttributeName = "id",
                PropertyName = "BuildingId",
                Api = "building",
                SupportsPatch = true,
                IsAnchor = true
            });

            type.AttributeAdapters.Add(new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                FieldName = "buildingName",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                AttributeName = "buildingName",
                PropertyName = "BuildingName",
                Api = "building",
                SupportsPatch = true,
                NullValueRepresentation = NullValueRepresentation.EmptyString,
                IsAnchor = false
            });

            type.AttributeAdapters.Add(new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                FieldName = "description",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                AttributeName = "description",
                PropertyName = "Description",
                Api = "building",
                SupportsPatch = true,
                NullValueRepresentation = NullValueRepresentation.EmptyString,
                IsAnchor = false
            });

            type.AttributeAdapters.Add(new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                FieldName = "floorNames",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                AttributeName = "floorNames",
                PropertyName = "FloorNames",
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
                FieldName = "latitude",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                PropertyName = "Latitude",
                AttributeNamePart = "latitude",
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
                FieldName = "longitude",
                IsMultivalued = false,
                PropertyName = "Longitude",
                Operation = AttributeOperation.ImportExport,
                AttributeNamePart = "longitude",
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
                AttributeName = "coordinates",
                Fields = new List<AdapterSubfield>() {latitude, longitude},
                FieldName = "coordinates",
                PropertyName = "Coordinates",
                SupportsPatch = false
            };

            type.AttributeAdapters.Add(schemaItem);

            return type;
        }
    }
}