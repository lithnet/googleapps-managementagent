using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.MetadirectoryServices;
using G = Google.Apis.Admin.Directory.directory_v1.Data;

namespace Lithnet.GoogleApps.MA
{
    internal class SchemaBuilderResourcesCalendars : ISchemaTypeBuilder
    {
        public string TypeName => "calendar";

        public IEnumerable<MASchemaType> GetSchemaTypes(IManagementAgentParameters config)
        {
            MASchemaType type = new MASchemaType
            {
                AttributeAdapters = new List<IAttributeAdapter>(),
                Name = "calendar",
                AnchorAttributeNames = new[] { "id", "resourceEmail" },
                SupportsPatch = true,
            };

            type.ApiInterface = new ApiInterfaceCalendar(config.CustomerID, type, config);

            type.AttributeAdapters.Add(new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                GoogleApiFieldName = "resourceId",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportOnly,
                MmsAttributeName = "id",
                ManagedObjectPropertyName = "ResourceId",
                Api = "calendar",
                SupportsPatch = true,
                IsAnchor = true
            });

            type.AttributeAdapters.Add(new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                GoogleApiFieldName = "resourceName",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                MmsAttributeName = "name",
                ManagedObjectPropertyName = "ResourceName",
                Api = "calendar",
                SupportsPatch = true,
            });

            type.AttributeAdapters.Add(new AdapterPropertyValue
            {
                AttributeType = config.CalendarBuildingAttributeType == "Reference" ? AttributeType.Reference : AttributeType.String,
                GoogleApiFieldName = "buildingId",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                MmsAttributeName = "buildingId",
                ManagedObjectPropertyName = "BuildingId",
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
                GoogleApiFieldName = "capacity",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                MmsAttributeName = "capacity",
                ManagedObjectPropertyName = "Capacity",
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
                GoogleApiFieldName = "floorName",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                MmsAttributeName = "floorName",
                ManagedObjectPropertyName = "FloorName",
                Api = "calendar",
                SupportsPatch = true,
                NullValueRepresentation = NullValueRepresentation.EmptyString,
            });

            type.AttributeAdapters.Add(new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                GoogleApiFieldName = "floorSection",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                MmsAttributeName = "floorSection",
                ManagedObjectPropertyName = "FloorSection",
                Api = "calendar",
                SupportsPatch = true,
                NullValueRepresentation = NullValueRepresentation.EmptyString,
            });

            // resourceCategory cannot be deleted. The API ignores requests to delete the value
            type.AttributeAdapters.Add(new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                GoogleApiFieldName = "resourceCategory",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                MmsAttributeName = "resourceCategory",
                ManagedObjectPropertyName = "ResourceCategory",
                Api = "calendar",
                SupportsPatch = true,
                NullValueRepresentation = NullValueRepresentation.Null,
            });

            type.AttributeAdapters.Add(new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                GoogleApiFieldName = "resourceDescription",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                MmsAttributeName = "resourceDescription",
                ManagedObjectPropertyName = "ResourceDescription",
                Api = "calendar",
                SupportsPatch = true,
                NullValueRepresentation = NullValueRepresentation.EmptyString,
            });

            type.AttributeAdapters.Add(new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                GoogleApiFieldName = "resourceType",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                MmsAttributeName = "resourceType",
                ManagedObjectPropertyName = "ResourceType",
                Api = "calendar",
                SupportsPatch = true,
                NullValueRepresentation = NullValueRepresentation.EmptyString,
            });

            type.AttributeAdapters.Add(new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                GoogleApiFieldName = "userVisibleDescription",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                MmsAttributeName = "userVisibleDescription",
                ManagedObjectPropertyName = "UserVisibleDescription",
                Api = "calendar",
                SupportsPatch = true,
                NullValueRepresentation = NullValueRepresentation.EmptyString,
            });

            type.AttributeAdapters.Add(new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                GoogleApiFieldName = "generatedResourceName",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportOnly,
                MmsAttributeName = "generatedResourceName",
                ManagedObjectPropertyName = "GeneratedResourceName",
                Api = "calendar",
                SupportsPatch = false,
            });

            type.AttributeAdapters.Add(new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                GoogleApiFieldName = "resourceEmail",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportOnly,
                MmsAttributeName = "resourceEmail",
                ManagedObjectPropertyName = "ResourceEmail",
                Api = "calendar",
                SupportsPatch = true,
                IsAnchor = true
            });

            type.AttributeAdapters.Add(new AdapterCollection<string>
            {
                AttributeType = config.CalendarFeatureAttributeType == "Reference" ? AttributeType.Reference : AttributeType.String,
                GoogleApiFieldName = "featureInstances",
                Operation = AttributeOperation.ImportExport,
                MmsAttributeName = "features",
                ManagedObjectPropertyName = "FeatureInstances",
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

            SchemaBuilderResourcesCalendars.AddCalendarAcls(type);

            yield return type;
        }

        private static void AddCalendarAcls(MASchemaType type)
        {
            type.AttributeAdapters.Add(new AdapterPlaceholder
            {
                AttributeType = AttributeType.Reference,
                IsMultivalued = true,
                GoogleApiFieldName = "scope.value",
                Operation = AttributeOperation.ImportExport,
                AttributeName = "freeBusyReader",
                Api = "calendaracl",
            });

            type.AttributeAdapters.Add(new AdapterPlaceholder
            {
                AttributeType = AttributeType.Reference,
                IsMultivalued = true,
                GoogleApiFieldName = "scope.value",
                Operation = AttributeOperation.ImportExport,
                AttributeName = "reader",
                Api = "calendaracl",
            });

            type.AttributeAdapters.Add(new AdapterPlaceholder
            {
                AttributeType = AttributeType.Reference,
                IsMultivalued = true,
                GoogleApiFieldName = "scope.value",
                Operation = AttributeOperation.ImportExport,
                AttributeName = "writer",
                Api = "calendaracl",
            });

            type.AttributeAdapters.Add(new AdapterPlaceholder
            {
                AttributeType = AttributeType.Reference,
                IsMultivalued = true,
                GoogleApiFieldName = "scope.value",
                Operation = AttributeOperation.ImportExport,
                AttributeName = "owner",
                Api = "calendaracl",
            });
        }
    }
}
