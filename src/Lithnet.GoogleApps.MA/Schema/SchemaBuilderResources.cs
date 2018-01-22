using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.MetadirectoryServices;
using G = Google.Apis.Admin.Directory.directory_v1.Data;

namespace Lithnet.GoogleApps.MA
{
    internal static class SchemaBuilderResources
    {

        public static MASchemaType GetFeatureSchema(IManagementAgentParameters config)
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

        public static MASchemaType GetBuildingSchema(IManagementAgentParameters config)
        {
            MASchemaType type = new MASchemaType
            {
                AttributeAdapters = new List<IAttributeAdapter>(),
                Name = "building",
                AnchorAttributeNames = new[] { "id" },
                SupportsPatch = true,
            };

            type.ApiInterface = new ApiInterfaceBuilding(config.CustomerID, type);

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
                CastForExport = i => ((string)i)?.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList(),
                CastForImport = i => i == null ? null : string.Join(",", ((IList<string>)i)),
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
                CastForImport = value => ((double?)value)?.ToString("R")
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
                CastForImport = value => ((double?)value)?.ToString("R")
            };

            AdapterNestedType schemaItem = new AdapterNestedType
            {
                Api = "building",
                AttributeName = "coordinates",
                Fields = new List<AdapterSubfield>() { latitude, longitude },
                FieldName = "coordinates",
                PropertyName = "Coordinates",
                SupportsPatch = false
            };

            type.AttributeAdapters.Add(schemaItem);

            return type;
        }

        public static MASchemaType GetCalendarSchema(IManagementAgentParameters config)
        {
            MASchemaType type = new MASchemaType
            {
                AttributeAdapters = new List<IAttributeAdapter>(),
                Name = "calendar",
                AnchorAttributeNames = new[] { "id", "resourceEmail" },
                SupportsPatch = true,
            };

            type.ApiInterface = new ApiInterfaceCalendar(config.CustomerID, type);

            type.AttributeAdapters.Add(new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                FieldName = "resourceId",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportOnly,
                AttributeName = "id",
                PropertyName = "ResourceId",
                Api = "calendar",
                SupportsPatch = true,
                IsAnchor = true
            });

            type.AttributeAdapters.Add(new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                FieldName = "resourceName",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                AttributeName = "name",
                PropertyName = "ResourceName",
                Api = "calendar",
                SupportsPatch = true,
            });

            type.AttributeAdapters.Add(new AdapterPropertyValue
            {
                AttributeType = config.CalendarBuildingAttributeType == "Reference" ? AttributeType.Reference : AttributeType.String,
                FieldName = "buildingId",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                AttributeName = "buildingId",
                PropertyName = "BuildingId",
                Api = "calendar",
                SupportsPatch = true,
                NullValueRepresentation = NullValueRepresentation.EmptyString,
                CastForExport = i =>
                {
                    if (i == null)
                    {
                        return null;
                    }

                    string s = (string)i;

                    if (string.IsNullOrEmpty(s))
                    {
                        return i;
                    }

                    if (config.CalendarBuildingAttributeType == "Reference")
                    {
                        return s.Replace(ApiInterfaceBuilding.DNSuffix, string.Empty);
                    }

                    return i;
                },
                CastForImport = i =>
                {
                    if (i == null)
                    {
                        return null;
                    }

                    string s = (string)i;

                    if (string.IsNullOrEmpty(s))
                    {
                        return i;
                    }

                    if (config.CalendarBuildingAttributeType == "Reference")
                    {
                        return $"{i}{ApiInterfaceBuilding.DNSuffix}";
                    }

                    return i;
                }

            });

            type.AttributeAdapters.Add(new AdapterPropertyValue
            {
                AttributeType = AttributeType.Integer,
                FieldName = "capacity",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                AttributeName = "capacity",
                PropertyName = "Capacity",
                Api = "calendar",
                SupportsPatch = true,
                NullValueRepresentation = NullValueRepresentation.IntZero,
                CastForExport = value =>
                {
                    if (value == null)
                    {
                        return null;
                    }

                    return Convert.ToInt32((long)value);
                },
                CastForImport = value =>
                {
                    if (value == null)
                    {
                        return null;
                    }

                    return Convert.ToInt64((int)value);
                }
            });

            type.AttributeAdapters.Add(new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                FieldName = "floorName",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                AttributeName = "floorName",
                PropertyName = "FloorName",
                Api = "calendar",
                SupportsPatch = true,
                NullValueRepresentation = NullValueRepresentation.EmptyString,
            });

            type.AttributeAdapters.Add(new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                FieldName = "floorSection",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                AttributeName = "floorSection",
                PropertyName = "FloorSection",
                Api = "calendar",
                SupportsPatch = true,
                NullValueRepresentation = NullValueRepresentation.EmptyString,
            });

            // resourceCategory cannot be deleted. The API ignores requests to delete the value
            type.AttributeAdapters.Add(new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                FieldName = "resourceCategory",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                AttributeName = "resourceCategory",
                PropertyName = "ResourceCategory",
                Api = "calendar",
                SupportsPatch = true,
                NullValueRepresentation = NullValueRepresentation.Null,
            });

            type.AttributeAdapters.Add(new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                FieldName = "resourceDescription",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                AttributeName = "resourceDescription",
                PropertyName = "ResourceDescription",
                Api = "calendar",
                SupportsPatch = true,
                NullValueRepresentation = NullValueRepresentation.EmptyString,
            });

            type.AttributeAdapters.Add(new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                FieldName = "resourceType",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                AttributeName = "resourceType",
                PropertyName = "ResourceType",
                Api = "calendar",
                SupportsPatch = true,
                NullValueRepresentation = NullValueRepresentation.EmptyString,
            });

            type.AttributeAdapters.Add(new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                FieldName = "userVisibleDescription",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                AttributeName = "userVisibleDescription",
                PropertyName = "UserVisibleDescription",
                Api = "calendar",
                SupportsPatch = true,
                NullValueRepresentation = NullValueRepresentation.EmptyString,
            });

            type.AttributeAdapters.Add(new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                FieldName = "generatedResourceName",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportOnly,
                AttributeName = "generatedResourceName",
                PropertyName = "GeneratedResourceName",
                Api = "calendar",
                SupportsPatch = false,
            });

            type.AttributeAdapters.Add(new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                FieldName = "resourceEmail",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportOnly,
                AttributeName = "resourceEmail",
                PropertyName = "ResourceEmail",
                Api = "calendar",
                SupportsPatch = true,
                IsAnchor = true
            });

            type.AttributeAdapters.Add(new AdapterCollection<string>
            {
                AttributeType = config.CalendarFeatureAttributeType == "Reference" ? AttributeType.Reference : AttributeType.String,
                FieldName = "featureInstances",
                Operation = AttributeOperation.ImportExport,
                AttributeName = "features",
                PropertyName = "FeatureInstances",
                Api = "calendar",
                SupportsPatch = false,
                GetList = obj => ApiInterfaceCalendar.GetFeatureNames((G.CalendarResource)obj, config.CalendarFeatureAttributeType)?.ToList(),
                CreateList = obj => new List<string>(),
                PutList = (obj, list) =>
                {
                    G.CalendarResource cal = obj as G.CalendarResource;

                    if (cal == null)
                    {
                        throw new InvalidOperationException();
                    }

                    List<G.FeatureInstance> items = new List<G.FeatureInstance>();

                    if (list != null && list.Count > 0)
                    {
                        foreach (string name in list)
                        {
                            string featureName = name;

                            if (config.CalendarFeatureAttributeType == "Reference")
                            {
                                featureName = featureName.Replace(ApiInterfaceFeature.DNSuffix, string.Empty);
                            }

                            items.Add(new G.FeatureInstance() { Feature = new G.Feature { Name = featureName } });
                        }
                    }

                    cal.FeatureInstances = items;
                }
            });

            SchemaBuilderResources.AddCalendarAcls(type);

            return type;
        }
        private static void AddCalendarAcls(MASchemaType type)
        {
            type.AttributeAdapters.Add(new AdapterCollection<string>
            {
                AttributeType = AttributeType.Reference,
                FieldName = "scope.value",
                Operation = AttributeOperation.ImportExport,
                AttributeName = "freeBusyReaders",
                PropertyName = "FreeBusyReaders",
                Api = "calendaracl",
                SupportsPatch = false,
            });

            type.AttributeAdapters.Add(new AdapterCollection<string>
            {
                AttributeType = AttributeType.Reference,
                FieldName = "scope.value",
                Operation = AttributeOperation.ImportExport,
                AttributeName = "readers",
                PropertyName = "readers",
                Api = "calendaracl",
                SupportsPatch = false,
            });

            type.AttributeAdapters.Add(new AdapterCollection<string>
            {
                AttributeType = AttributeType.Reference,
                FieldName = "scope.value",
                Operation = AttributeOperation.ImportExport,
                AttributeName = "writers",
                PropertyName = "writers",
                Api = "calendaracl",
                SupportsPatch = false,
            });

            type.AttributeAdapters.Add(new AdapterCollection<string>
            {
                AttributeType = AttributeType.Reference,
                FieldName = "scope.value",
                Operation = AttributeOperation.ImportExport,
                AttributeName = "owners",
                PropertyName = "owners",
                Api = "calendaracl",
                SupportsPatch = false,
            });
        }
    }
}
