using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Google.Apis.Admin.Directory.directory_v1.Data;
using Lithnet.GoogleApps.ManagedObjects;
using Lithnet.Logging;
using Lithnet.MetadirectoryServices;
using Microsoft.MetadirectoryServices;
using MmsSchema = Microsoft.MetadirectoryServices.Schema;

namespace Lithnet.GoogleApps.MA
{
    internal class ApiInterfaceBuilding : IApiInterfaceObject
    {
        private string customerID;

        protected MASchemaType SchemaType { get; set; }

        public static string DNSuffix => "@building.resource";

        public ApiInterfaceBuilding(string customerID, MASchemaType type)
        {
            this.SchemaType = type;
            this.customerID = customerID;
        }

        public string Api => "building";

        public ObjectModificationType DeltaUpdateType => ObjectModificationType.Update;

        public object CreateInstance(CSEntryChange csentry)
        {
            Building building = new Building();
            building.BuildingId = csentry.DN.Replace(ApiInterfaceBuilding.DNSuffix, string.Empty);
            return building;
        }

        public object GetInstance(CSEntryChange csentry)
        {
            return ResourceRequestFactory.GetBuilding(this.customerID, csentry.GetAnchorValueOrDefault<string>("id"));
        }

        public void DeleteInstance(CSEntryChange csentry)
        {
            ResourceRequestFactory.DeleteBuilding(this.customerID, csentry.GetAnchorValueOrDefault<string>("id"));
        }

        public IList<AttributeChange> ApplyChanges(CSEntryChange csentry, SchemaType type, IManagementAgentParameters config, ref object target, bool patch = false)
        {
            bool hasChanged = false;
            List<AttributeChange> changes = new List<AttributeChange>();

            Building building = target as Building;

            if (building == null)
            {
                throw new InvalidOperationException();
            }

            hasChanged |= this.SetDNValue(csentry, building);

            foreach (IAttributeAdapter typeDef in this.SchemaType.AttributeAdapters.Where(t => t.Api == this.Api))
            {
                hasChanged |= typeDef.UpdateField(csentry, building);
            }

            if (hasChanged)
            {
                Building result;

                if (csentry.ObjectModificationType == ObjectModificationType.Add)
                {
                    result = ResourceRequestFactory.AddBuilding(this.customerID, building);
                }
                else if (csentry.ObjectModificationType == ObjectModificationType.Replace || csentry.ObjectModificationType == ObjectModificationType.Update)
                {
                    string id = csentry.GetAnchorValueOrDefault<string>(this.SchemaType.AnchorAttributeName);

                    if (patch)
                    {
                        result = ResourceRequestFactory.PatchBuilding(this.customerID, id, building);
                    }
                    else
                    {
                        result = ResourceRequestFactory.UpdateBuilding(this.customerID, id, building);
                    }
                }
                else
                {
                    throw new InvalidOperationException();
                }

                changes.AddRange(this.GetLocalChanges(csentry.DN, csentry.ObjectModificationType, type, result));
            }


            return changes;
        }

        public IList<AttributeChange> GetChanges(string dn, ObjectModificationType modType, SchemaType type, object source, IManagementAgentParameters config)
        {
            List<AttributeChange> attributeChanges = this.GetLocalChanges(dn, modType, type, source);

            return attributeChanges;
        }

        private List<AttributeChange> GetLocalChanges(string dn, ObjectModificationType modType, SchemaType type, object source)
        {
            List<AttributeChange> attributeChanges = new List<AttributeChange>();

            Building building = source as Building;

            if (building == null)
            {
                throw new InvalidOperationException();
            }

            foreach (IAttributeAdapter typeDef in this.SchemaType.AttributeAdapters.Where(t => t.Api == this.Api))
            {
                if (typeDef.IsAnchor)
                {
                    continue;
                }

                foreach (AttributeChange change in typeDef.CreateAttributeChanges(dn, modType, building))
                {
                    if (type.HasAttribute(change.Name))
                    {
                        attributeChanges.Add(change);
                    }
                }
            }

            return attributeChanges;
        }

        public string GetAnchorValue(object target)
        {
            Building building = target as Building;

            if (building == null)
            {
                throw new InvalidOperationException();
            }

            return building.BuildingId;
        }

        public string GetDNValue(object target)
        {
            Building building = target as Building;

            if (building == null)
            {
                throw new InvalidOperationException();
            }

            return $"{building.BuildingId}{ApiInterfaceBuilding.DNSuffix}";
        }

        public Task GetItems(IManagementAgentParameters config, MmsSchema schema, BlockingCollection<object> collection)
        {
            HashSet<string> fieldList = new HashSet<string>
            {
                "buildingName",
                "buildingId"
            };

            foreach (string fieldName in ManagementAgent.Schema[SchemaConstants.Building].GetFieldNames(schema.Types[SchemaConstants.Building], "building"))
            {
                fieldList.Add(fieldName);
            }

            string fields = string.Format("buildings({0}), nextPageToken", string.Join(",", fieldList));

            Task t = new Task(() =>
            {
                Logger.WriteLine("Starting building import task");
                Logger.WriteLine("Requesting building fields: " + fields);

                foreach (Building building in ResourceRequestFactory.GetBuildings(config.CustomerID, fields))
                {
                    collection.Add(this.GetCSEntryForBuilding(building, schema, config));
                    Debug.WriteLine($"Created CSEntryChange for building: {building.BuildingId}");

                    continue;
                }

                Logger.WriteLine("Building import task complete");
            });

            t.Start();

            return t;
        }

        private CSEntryChange GetCSEntryForBuilding(Building building, MmsSchema schema, IManagementAgentParameters config)
        {
            return ImportProcessor.GetCSEntryChange(building, schema.Types[SchemaConstants.Building], config);
        }

        private bool SetDNValue(CSEntryChange csentry, Building building)
        {
            if (csentry.ObjectModificationType != ObjectModificationType.Replace && csentry.ObjectModificationType != ObjectModificationType.Update)
            {
                return false;
            }

            string newDN = csentry.GetNewDNOrDefault<string>();

            if (newDN == null)
            {
                return false;
            }

            throw new NotSupportedException("Renaming a building object is not supported");
        }
    }
}

