using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Google.Apis.Admin.Directory.directory_v1.Data;
using Lithnet.Logging;
using Lithnet.MetadirectoryServices;
using Microsoft.MetadirectoryServices;
using MmsSchema = Microsoft.MetadirectoryServices.Schema;

namespace Lithnet.GoogleApps.MA
{
    internal class ApiInterfaceFeature : IApiInterfaceObject
    {
        private string customerID;

        private IManagementAgentParameters config;

        public static string DNSuffix => "@feature.resource";

        protected MASchemaType SchemaType { get; set; }
        public ApiInterfaceFeature(string customerID, MASchemaType type, IManagementAgentParameters config)
        {
            this.SchemaType = type;
            this.customerID = customerID;
            this.config = config;
        }

        public string Api => "feature";

        public ObjectModificationType DeltaUpdateType => ObjectModificationType.Update;

        public object CreateInstance(CSEntryChange csentry)
        {
            Feature feature = new Feature();
            ApiInterfaceFeature.ThrowOnInvalidDN(csentry.DN);

            feature.Name = csentry.DN.Replace(ApiInterfaceFeature.DNSuffix, string.Empty);
            return feature;
        }

        private static void ThrowOnInvalidDN(string dn)
        {
            if (dn == null || !dn.EndsWith(ApiInterfaceFeature.DNSuffix))
            {
                throw new InvalidDNException($"The DN must end with '{ApiInterfaceFeature.DNSuffix}'");
            }
        }

        public object GetInstance(CSEntryChange csentry)
        {
            return this.config.ResourcesService.GetFeature(this.customerID, csentry.GetAnchorValueOrDefault<string>("id") ?? csentry.DN);
        }

        public void DeleteInstance(CSEntryChange csentry)
        {
            this.config.ResourcesService.DeleteFeature(this.customerID, csentry.GetAnchorValueOrDefault<string>("id") ?? csentry.DN);
        }

        public IList<AttributeChange> ApplyChanges(CSEntryChange csentry, SchemaType type, ref object target, bool patch = false)
        {
            bool hasChanged = false;
            List<AttributeChange> changes = new List<AttributeChange>();

            if (!(target is Feature feature))
            {
                throw new InvalidOperationException();
            }

            hasChanged |= this.SetDNValue(csentry, feature);

            foreach (IAttributeAdapter typeDef in this.SchemaType.AttributeAdapters.Where(t => t.Api == this.Api))
            {
                hasChanged |= typeDef.UpdateField(csentry, feature);
            }

            if (hasChanged || csentry.ObjectModificationType == ObjectModificationType.Add)
            {
                Feature result;

                if (csentry.ObjectModificationType == ObjectModificationType.Add)
                {
                    result = this.config.ResourcesService.AddFeature(this.customerID, feature);
                }
                else if (csentry.ObjectModificationType == ObjectModificationType.Replace || csentry.ObjectModificationType == ObjectModificationType.Update)
                {
                    string id = csentry.GetAnchorValueOrDefault<string>("id");

                    if (patch)
                    {
                        result = this.config.ResourcesService.PatchFeature(this.customerID, id, feature);
                    }
                    else
                    {
                        result = this.config.ResourcesService.UpdateFeature(this.customerID, id, feature);
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

        public IList<AttributeChange> GetChanges(string dn, ObjectModificationType modType, SchemaType type, object source)
        {
            List<AttributeChange> attributeChanges = this.GetLocalChanges(dn, modType, type, source);

            return attributeChanges;
        }

        private List<AttributeChange> GetLocalChanges(string dn, ObjectModificationType modType, SchemaType type, object source)
        {
            List<AttributeChange> attributeChanges = new List<AttributeChange>();

            if (!(source is Feature feature))
            {
                throw new InvalidOperationException();
            }

            foreach (IAttributeAdapter typeDef in this.SchemaType.AttributeAdapters.Where(t => t.Api == this.Api))
            {
                if (typeDef.IsAnchor)
                {
                    continue;
                }

                foreach (AttributeChange change in typeDef.CreateAttributeChanges(dn, modType, feature))
                {
                    if (type.HasAttribute(change.Name))
                    {
                        attributeChanges.Add(change);
                    }
                }
            }

            return attributeChanges;
        }

        public string GetAnchorValue(string name, object target)
        {
            Feature feature = target as Feature;

            if (feature == null)
            {
                throw new InvalidOperationException();
            }

            return feature.Name;
        }

        public string GetDNValue(object target)
        {
            if (!(target is Feature feature))
            {
                throw new InvalidOperationException();
            }

            return $"{feature.Name}{ApiInterfaceFeature.DNSuffix}";
        }

        public Task GetItems(MmsSchema schema, BlockingCollection<object> collection)
        {
            HashSet<string> fieldList = new HashSet<string>
            {
                "name"
            };

            foreach (string fieldName in ManagementAgent.Schema[SchemaConstants.Feature].GetFieldNames(schema.Types[SchemaConstants.Feature], "feature"))
            {
                fieldList.Add(fieldName);
            }

            string fields = string.Format("features({0}), nextPageToken", string.Join(",", fieldList));

            Task t = new Task(() =>
            {
                Logger.WriteLine("Starting feature import task");
                Logger.WriteLine("Requesting feature fields: " + fields);

                foreach (Feature feature in this.config.ResourcesService.GetFeatures(this.config.CustomerID, fields))
                {
                    collection.Add(this.GetCSEntryForFeature(feature, schema));
                    Debug.WriteLine($"Created CSEntryChange for feature: {feature.Name}");

                    continue;
                }

                Logger.WriteLine("Feature import task complete");
            });

            t.Start();

            return t;
        }

        private CSEntryChange GetCSEntryForFeature(Feature feature, MmsSchema schema)
        {
            return ImportProcessor.GetCSEntryChange(feature, schema.Types[SchemaConstants.Feature], this.config);
        }

        private bool SetDNValue(CSEntryChange csentry, Feature feature)
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

            throw new NotSupportedException("Renaming a feature object is not supported");
        }
    }
}

