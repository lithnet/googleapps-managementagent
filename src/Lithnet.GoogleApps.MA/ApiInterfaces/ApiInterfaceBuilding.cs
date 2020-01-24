using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Admin.Directory.directory_v1.Data;
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

        private IManagementAgentParameters config;

        public ApiInterfaceBuilding(string customerID, MASchemaType type, IManagementAgentParameters config)
        {
            this.SchemaType = type;
            this.customerID = customerID;
            this.config = config;
        }

        public string Api => "building";

        public ObjectModificationType DeltaUpdateType => ObjectModificationType.Update;

        public object CreateInstance(CSEntryChange csentry)
        {
            Building building = new Building();
            ApiInterfaceBuilding.ThrowOnInvalidDN(csentry.DN);
            building.BuildingId = csentry.DN.Replace(ApiInterfaceBuilding.DNSuffix, string.Empty);
            return building;
        }

        private static void ThrowOnInvalidDN(string dn)
        {
            if (dn == null || !dn.EndsWith(ApiInterfaceBuilding.DNSuffix))
            {
                throw new InvalidDNException($"The DN must end with '{ApiInterfaceBuilding.DNSuffix}'");
            }
        }

        public object GetInstance(CSEntryChange csentry)
        {
            return this.config.ResourcesService.GetBuilding(this.customerID, csentry.GetAnchorValueOrDefault<string>("id"));
        }

        public void DeleteInstance(CSEntryChange csentry)
        {
            this.config.ResourcesService.DeleteBuilding(this.customerID, csentry.GetAnchorValueOrDefault<string>("id"));
        }

        public void ApplyChanges(CSEntryChange csentry, CSEntryChange committedChanges, SchemaType type, ref object target, bool patch = false)
        {
            bool hasChanged = false;

            if (!(target is Building building))
            {
                throw new InvalidOperationException();
            }

            hasChanged |= this.SetDNValue(csentry, building);

            foreach (IAttributeAdapter typeDef in this.SchemaType.AttributeAdapters.Where(t => t.Api == this.Api))
            {
                hasChanged |= typeDef.UpdateField(csentry, building);
            }

            if (csentry.ObjectModificationType == ObjectModificationType.Add)
            {
                building = this.config.ResourcesService.AddBuilding(this.customerID, building);
                committedChanges.ObjectModificationType = ObjectModificationType.Add;
                committedChanges.DN = this.GetDNValue(building);
            }

            if (csentry.IsUpdateOrReplace() && hasChanged)
            {
                string id = csentry.GetAnchorValueOrDefault<string>("id");

                if (patch)
                {
                    building = this.config.ResourcesService.PatchBuilding(this.customerID, id, building);
                }
                else
                {
                    building = this.config.ResourcesService.UpdateBuilding(this.customerID, id, building);
                }
            }

            if (csentry.IsUpdateOrReplace())
            {
                committedChanges.ObjectModificationType = this.DeltaUpdateType;
                committedChanges.DN = this.GetDNValue(building);
            }

            foreach (AttributeChange change in this.GetLocalChanges(csentry.DN, csentry.ObjectModificationType, type, building))
            {
                committedChanges.AttributeChanges.Add(change);
            }

            target = building;
        }

        public IEnumerable<AttributeChange> GetChanges(string dn, ObjectModificationType modType, SchemaType type, object source)
        {
            return this.GetLocalChanges(dn, modType, type, source);
        }

        private IEnumerable<AttributeChange> GetLocalChanges(string dn, ObjectModificationType modType, SchemaType type, object source)
        {
            if (!(source is Building building))
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
                        yield return change;
                    }
                }
            }
        }

        public string GetAnchorValue(string attributeName, object target)
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

        public Task GetObjectImportTask(MmsSchema schema, BlockingCollection<object> collection, CancellationToken cancellationToken)
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
                Logger.WriteLine("Requesting building fields: " + fields);

                foreach (Building building in this.config.ResourcesService.GetBuildings(this.config.CustomerID, fields))
                {
                    collection.Add(this.GetCSEntryForBuilding(building, schema, this.config));
                    Debug.WriteLine($"Created CSEntryChange for building: {building.BuildingId}");

                    continue;
                }
            }, cancellationToken);

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

