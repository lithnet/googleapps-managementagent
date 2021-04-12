using Google.Apis.Admin.Directory.directory_v1.Data;
using Lithnet.Logging;
using Lithnet.MetadirectoryServices;
using Microsoft.MetadirectoryServices;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Schema = Microsoft.MetadirectoryServices.Schema;

namespace Lithnet.GoogleApps.MA
{
    internal class ApiInterfaceOrgUnit : IApiInterfaceObject
    {
        protected MASchemaType SchemaType { get; set; }

        private string customerID;

        private IManagementAgentParameters config;

        public ApiInterfaceOrgUnit(string customerID, MASchemaType type, IManagementAgentParameters config)
        {
            this.SchemaType = type;
            this.customerID = customerID;
            this.config = config;
            config.LicenseManager.ThrowOnMissingFeature(Features.OrgUnits);
        }

        public string Api => "orgUnit";

        public ObjectModificationType DeltaUpdateType => ObjectModificationType.Update;

        public object CreateInstance(CSEntryChange csentry)
        {
            OrgUnit o = new OrgUnit();
            return o;
        }

        public object GetInstance(CSEntryChange csentry)
        {
            string id = csentry.GetAnchorValueOrDefault<string>("id");

            if (id == null)
            {
                throw new AttributeNotPresentException("id");
            }

            return this.config.OrgUnitsService.Get(this.customerID, id);
        }


        public void DeleteInstance(CSEntryChange csentry)
        {
            string id = csentry.GetAnchorValueOrDefault<string>("id");

            if (id == null)
            {
                throw new AttributeNotPresentException("id");
            }

            this.config.OrgUnitsService.Delete(this.customerID, id);
        }

        public void ApplyChanges(CSEntryChange csentry, CSEntryChange committedChanges, SchemaType type, ref object target, bool patch = false)
        {
            bool hasChanged = false;

            OrgUnit orgUnit = (OrgUnit)target;

            hasChanged |= this.SetDNValue(csentry, orgUnit);

            if (hasChanged)
            {
                patch = false; // Can't patch on an OU parent update
            }

            foreach (IAttributeAdapter typeDef in this.SchemaType.AttributeAdapters.Where(t => t.Api == this.Api))
            {
                hasChanged |= typeDef.UpdateField(csentry, orgUnit);
            }

            if (csentry.ObjectModificationType == ObjectModificationType.Add)
            {
                if (string.IsNullOrWhiteSpace(orgUnit.ParentOrgUnitPath))
                {
                    orgUnit.ParentOrgUnitPath = "/";
                }

                orgUnit = this.config.OrgUnitsService.Insert(this.customerID, orgUnit);
                committedChanges.ObjectModificationType = ObjectModificationType.Add;
                committedChanges.DN = this.GetDNValue(orgUnit);
                target = orgUnit;
            }

            if (csentry.IsUpdateOrReplace() && hasChanged)
            {
                if (csentry.HasAttributeChange("parentOrgUnit"))
                {
                    orgUnit.ParentOrgUnitId = null;
                }

                string id = csentry.GetAnchorValueOrDefault<string>("id");

                if (patch)
                {
                    orgUnit = this.config.OrgUnitsService.Patch(orgUnit, this.customerID, id);
                }
                else
                {
                    orgUnit = this.config.OrgUnitsService.Update(orgUnit, this.customerID, id);
                }
            }

            if (csentry.IsUpdateOrReplace())
            {
                committedChanges.ObjectModificationType = this.DeltaUpdateType;
                committedChanges.DN = this.GetDNValue(orgUnit);
            }

            foreach (AttributeChange change in this.GetLocalChanges(csentry.DN, csentry.ObjectModificationType, type, orgUnit))
            {
                committedChanges.AttributeChanges.Add(change);
            }
        }

        public IEnumerable<AttributeChange> GetChanges(string dn, ObjectModificationType modType, SchemaType type, object source)
        {
            return this.GetLocalChanges(dn, modType, type, source);
        }

        private List<AttributeChange> GetLocalChanges(string dn, ObjectModificationType modType, SchemaType type, object source)
        {
            List<AttributeChange> attributeChanges = new List<AttributeChange>();

            OrgUnit entry = source as OrgUnit;

            if (entry == null)
            {
                throw new InvalidOperationException();
            }

            foreach (IAttributeAdapter typeDef in this.SchemaType.AttributeAdapters.Where(t => t.Api == this.Api))
            {
                if (typeDef.IsAnchor)
                {
                    continue;
                }

                foreach (AttributeChange change in typeDef.CreateAttributeChanges(dn, modType, entry))
                {
                    if (type.HasAttribute(change.Name))
                    {
                        attributeChanges.Add(change);
                    }
                }
            }

            return attributeChanges;
        }

        public string GetAnchorValue(string attributeName, object target)
        {
            if (target is OrgUnit d)
            {
                return d.OrgUnitId;
            }

            throw new InvalidOperationException();
        }

        public string GetDNValue(object target)
        {
            if (!(target is OrgUnit d))
            {
                throw new InvalidOperationException();
            }

            return this.BuildDn(d);
        }

        public Task GetObjectImportTask(Schema schema, BlockingCollection<object> collection, CancellationToken cancellationToken)
        {
            Task t = new Task(() =>
            {
                var list = this.config.OrgUnitsService.List(this.config.CustomerID);

                foreach (var d in list.OrganizationUnits)
                {
                    string dn = this.GetDNValue(d);

                    if (dn == null)
                    {
                        Logger.WriteLine($"OrgUnit {d} had no DN, ignoring");
                        continue;
                    }

                    collection.Add(ImportProcessor.GetCSEntryChange(d, schema.Types[SchemaConstants.OrgUnit], this.config), cancellationToken);
                }
            }, cancellationToken);

            t.Start();

            return t;
        }

        public bool SetDNValue(CSEntryChange csentry, OrgUnit e)
        {
            bool changed = false;
            string dn = csentry.GetNewDNOrDefault<string>();

            if (dn != null)
            {
                Logger.WriteLine($"Object is being renamed {csentry.DN} -> {dn}");
            }
            else
            {
                dn = csentry.DN;
            }

            string name = this.GetOuNameFromDN(dn);
            string parentPath = this.GetOuParentFromDN(dn);

            if (e.Name != name)
            {
                changed = true;
                e.Name = name;
            }

            if (e.ParentOrgUnitPath != parentPath)
            {
                e.ParentOrgUnitId = null;
                e.ParentOrgUnitPath = this.GetOuParentFromDN(dn);
                changed = true;
            }

            return changed;
        }

        private string GetOuNameFromDN(string dn)
        {
            // /First/Second/Third
            return dn.Split('/').Last();
        }

        private string GetOuParentFromDN(string dn)
        {
            // /First/Second/Third
            string name = dn.Split('/').Last();
            return dn.Substring(0, dn.Length - name.Length - 1);
        }

        private string BuildDn(OrgUnit ou)
        {
            return ou.OrgUnitPath;
        }
    }
}
