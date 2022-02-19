using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Admin.Directory.directory_v1.Data;
using Lithnet.Logging;
using Lithnet.MetadirectoryServices;
using Microsoft.MetadirectoryServices;
using Schema = Microsoft.MetadirectoryServices.Schema;

namespace Lithnet.GoogleApps.MA
{
    internal class ApiInterfaceDomain : IApiInterfaceObject
    {
        protected MASchemaType SchemaType { get; set; }

        private string customerID;

        private IManagementAgentParameters config;

        public ApiInterfaceDomain(string customerID, MASchemaType type, IManagementAgentParameters config)
        {
            this.SchemaType = type;
            this.customerID = customerID;
            this.config = config;
        }

        public string Api => "domain";

        public ObjectModificationType DeltaUpdateType => ObjectModificationType.Update;

        public object CreateInstance(CSEntryChange csentry)
        {
            throw new NotSupportedException("Cannot create objects of the type 'Domain'");
        }

        public object GetInstance(CSEntryChange csentry)
        {
            string id = csentry.GetAnchorValueOrDefault<string>("domainName");

            if (id == null)
            {
                throw new AttributeNotPresentException("domainName");
            }

            return this.config.DomainsService.Get(this.customerID, id);
        }

        public void DeleteInstance(CSEntryChange csentry)
        {
            string id = csentry.GetAnchorValueOrDefault<string>("domainName");

            if (id == null)
            {
                throw new AttributeNotPresentException("domainName");
            }

            this.config.DomainsService.Delete(this.customerID, id);
        }

        public void ApplyChanges(CSEntryChange csentry, CSEntryChange committedChanges, SchemaType type, ref object target, bool patch = false)
        {
            if (csentry.IsUpdateOrReplace())
            {
                throw new InvalidOperationException("Domain objects are read only");
            }

            Domains domain = (Domains)target;

            this.SetDNValue(csentry, domain);

            foreach (IAttributeAdapter typeDef in this.SchemaType.AttributeAdapters.Where(t => t.Api == this.Api))
            {
                typeDef.UpdateField(csentry, domain);
            }

            if (csentry.ObjectModificationType == ObjectModificationType.Add)
            {
                domain = this.config.DomainsService.Insert(this.customerID, domain);
                committedChanges.ObjectModificationType = ObjectModificationType.Add;
                committedChanges.DN = this.GetDNValue(domain);
                target = domain;
            }

            foreach (AttributeChange change in this.GetLocalChanges(csentry.DN, csentry.ObjectModificationType, type, domain))
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

            Domains entry = source as Domains;

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
            if (target is Domains d)
            {
                return d.DomainName;
            }

            throw new InvalidOperationException();
        }

        public string GetDNValue(object target)
        {
            if (!(target is Domains d))
            {
                throw new InvalidOperationException();
            }

            return d.DomainName;
        }

        public Task GetObjectImportTask(Schema schema, BlockingCollection<object> collection, CancellationToken cancellationToken)
        {
            Task t = new Task(() =>
            {
                var list = this.config.DomainsService.List(this.config.CustomerID);

                foreach (Domains d in list.Domains)
                {
                    string dn = this.GetDNValue(d);

                    if (dn == null)
                    {
                        Logger.WriteLine($"Domain {d} had no DN, ignoring");
                        continue;
                    }

                    collection.Add(ImportProcessor.GetCSEntryChange(d, schema.Types[SchemaConstants.Domain], this.config));
                }
            }, cancellationToken);

            t.Start();

            return t;
        }

        public bool SetDNValue(CSEntryChange csentry, Domains e)
        {
            if (csentry.ObjectModificationType == ObjectModificationType.Add)
            {
                e.DomainName = csentry.DN;
                return true;
            }
            else
            {
                throw new NotSupportedException($"The DN value is read only and cannot be changed");
            }
        }
    }
}
