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
    internal class ApiInterfaceChromeDevice : IApiInterfaceObject
    {
        private IManagementAgentParameters config;

        public static string DNSuffix => "@chromedevice";

        protected MASchemaType SchemaType { get; set; }

        public ApiInterfaceChromeDevice(MASchemaType type, IManagementAgentParameters config)
        {
            this.SchemaType = type;
            this.config = config;
        }

        public string Api => "chromeDevice";

        public ObjectModificationType DeltaUpdateType => ObjectModificationType.Update;

        public object CreateInstance(CSEntryChange csentry)
        {
            throw new UnknownOrUnsupportedModificationTypeException();
        }

        public object GetInstance(CSEntryChange csentry)
        {
            throw new UnknownOrUnsupportedModificationTypeException();
        }

        public void DeleteInstance(CSEntryChange csentry)
        {
            throw new UnknownOrUnsupportedModificationTypeException();
        }

        public void ApplyChanges(CSEntryChange csentry, CSEntryChange committedChanges, SchemaType type, ref object target, bool patch = false)
        {
            throw new UnknownOrUnsupportedModificationTypeException();
        }

        public IEnumerable<AttributeChange> GetChanges(string dn, ObjectModificationType modType, SchemaType type, object source)
        {
            return this.GetLocalChanges(dn, modType, type, source);
        }

        private List<AttributeChange> GetLocalChanges(string dn, ObjectModificationType modType, SchemaType type, object source)
        {
            List<AttributeChange> attributeChanges = new List<AttributeChange>();

            if (!(source is ChromeOsDevice cod))
            {
                throw new InvalidOperationException();
            }
            
            foreach (IAttributeAdapter typeDef in this.SchemaType.AttributeAdapters.Where(t => t.Api == this.Api))
            {
                if (typeDef.IsAnchor)
                {
                    continue;
                }

                foreach (AttributeChange change in typeDef.CreateAttributeChanges(dn, modType, cod))
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
            ChromeOsDevice cod = target as ChromeOsDevice;

            if (cod == null)
            {
                throw new InvalidOperationException();
            }

            return cod.DeviceId;
        }

        public string GetDNValue(object target)
        {
            if (!(target is ChromeOsDevice cod))
            {
                throw new InvalidOperationException();
            }

            return $"{cod.DeviceId}{ApiInterfaceChromeDevice.DNSuffix}";
        }

        public Task GetObjectImportTask(MmsSchema schema, BlockingCollection<object> collection, CancellationToken cancellationToken)
        {
            HashSet<string> fieldList = new HashSet<string>
            {
                "deviceId"
            };

            foreach (string fieldName in ManagementAgent.Schema[SchemaConstants.ChromeDevice].GetFieldNames(schema.Types[SchemaConstants.ChromeDevice], "chromeDevice"))
            {
                fieldList.Add(fieldName);
            }

            string fields = string.Format("chromeosdevices({0}), nextPageToken", string.Join(",", fieldList));

            Task t = new Task(() =>
            {
                Logger.WriteLine("Requesting chromeosdevices fields: " + fields);

                foreach (ChromeOsDevice cd in this.config.ChromeDeviceService.GetChromeDevices(this.config.CustomerID, fields))
                {
                    collection.Add(this.GetCSEntry(cd, schema), cancellationToken);
                    Debug.WriteLine($"Created CSEntryChange for chrome device: {cd.DeviceId}");
                }

            }, cancellationToken);

            t.Start();

            return t;
        }

        private CSEntryChange GetCSEntry(ChromeOsDevice cd, MmsSchema schema)
        {
            return ImportProcessor.GetCSEntryChange(cd, schema.Types[SchemaConstants.ChromeDevice], this.config);
        }
    }
}

