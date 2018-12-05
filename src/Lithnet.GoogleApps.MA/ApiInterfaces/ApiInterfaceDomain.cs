using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Google.GData.Contacts;
using Lithnet.GoogleApps.ManagedObjects;
using Lithnet.Logging;
using Lithnet.MetadirectoryServices;
using Microsoft.MetadirectoryServices;

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
            return new ContactEntry();
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

        public IList<AttributeChange> ApplyChanges(CSEntryChange csentry, SchemaType type, ref object target, bool patch = false)
        {
            bool hasChanged = false;
            List<AttributeChange> changes = new List<AttributeChange>();
            Domain obj = (Domain)target;

            if (this.SetDNValue(csentry, obj))
            {
                hasChanged = true;
            }

            foreach (IAttributeAdapter typeDef in this.SchemaType.AttributeAdapters.Where(t => t.Api == this.Api))
            {
                if (typeDef.UpdateField(csentry, obj))
                {
                    hasChanged = true;
                }
            }

            if (hasChanged)
            {
                Domain result;

                if (csentry.ObjectModificationType == ObjectModificationType.Add)
                {
                    result = this.config.DomainsService.Insert(this.customerID, obj);
                    target = result;
                }
                else if (csentry.ObjectModificationType == ObjectModificationType.Replace || csentry.ObjectModificationType == ObjectModificationType.Update)
                {
                    throw new InvalidOperationException("Domain objects are read only");
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

            Domain entry = source as Domain;

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
            if (target is Domain d)
            {
                return d.DomainName;
            }

            throw new InvalidOperationException();
        }

        public string GetDNValue(object target)
        {
            if (!(target is Domain d))
            {
                throw new InvalidOperationException();
            }

            return d.DomainName;
        }

        public Task GetObjectImportTask(Schema schema, BlockingCollection<object> collection, CancellationToken cancellationToken)
        {
            Task t = new Task(() =>
            {
                Logger.WriteLine("Starting domains import task");

                DomainList list = this.config.DomainsService.List(this.config.CustomerID);

                foreach (Domain d in list.Domains)
                {
                    string dn = this.GetDNValue(d);

                    if (dn == null)
                    {
                        Logger.WriteLine($"Domain {d} had no DN, ignoring");
                        continue;
                    }

                    collection.Add(ImportProcessor.GetCSEntryChange(d, schema.Types[SchemaConstants.Domain], this.config));
                }

                Logger.WriteLine("Domains import task complete");
            }, cancellationToken);

            t.Start();

            return t;
        }

        public bool SetDNValue(CSEntryChange csentry, Domain e)
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
