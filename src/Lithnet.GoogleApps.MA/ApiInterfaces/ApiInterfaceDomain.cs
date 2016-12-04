using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics.CodeAnalysis;
using Google.GData.Contacts;
using Lithnet.GoogleApps.ManagedObjects;
using Lithnet.Logging;
using Lithnet.MetadirectoryServices;
using Microsoft.MetadirectoryServices;

namespace Lithnet.GoogleApps.MA
{
    internal class ApiInterfaceDomain : IApiInterfaceObject
    {
        [SuppressMessage("ReSharper", "CollectionNeverUpdated.Local")]
        private static ApiInterfaceKeyedCollection internalInterfaces;

        protected MASchemaType SchemaType { get; set; }

        private string customerID;

        static ApiInterfaceDomain()
        {
            ApiInterfaceDomain.internalInterfaces = new ApiInterfaceKeyedCollection();
        }

        public ApiInterfaceDomain(string customerID, MASchemaType type)
        {
            this.SchemaType = type;
            this.customerID = customerID;
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

            return DomainsRequestFactory.Get(this.customerID, id);
        }

        public void DeleteInstance(CSEntryChange csentry)
        {
            string id = csentry.GetAnchorValueOrDefault<string>("domainName");

            if (id == null)
            {
                throw new AttributeNotPresentException("domainName");
            }

            DomainsRequestFactory.Delete(this.customerID, id);
        }

        public IList<AttributeChange> ApplyChanges(CSEntryChange csentry, SchemaType type, IManagementAgentParameters config, ref object target, bool patch = false)
        {
            bool hasChanged = false;
            List<AttributeChange> changes = new List<AttributeChange>();
            Domain obj = (Domain)target;

            if (this.SetDNValue(csentry, obj))
            {
                hasChanged = true;
            }

            foreach (IAttributeAdapter typeDef in this.SchemaType.Attributes.Where(t => t.Api == this.Api))
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
                    result = DomainsRequestFactory.Insert(this.customerID, obj);
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

            foreach (IApiInterface i in ApiInterfaceDomain.internalInterfaces)
            {
                changes.AddRange(i.ApplyChanges(csentry, type, config, ref target, patch));
            }

            return changes;
        }

        public IList<AttributeChange> GetChanges(string dn, ObjectModificationType modType, SchemaType type, object source, IManagementAgentParameters config)
        {
            List<AttributeChange> attributeChanges = this.GetLocalChanges(dn, modType, type, source);

            foreach (IApiInterface i in ApiInterfaceDomain.internalInterfaces)
            {
                attributeChanges.AddRange(i.GetChanges(dn, modType, type, source, config));
            }

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

            foreach (IAttributeAdapter typeDef in this.SchemaType.Attributes.Where(t => t.Api == this.Api))
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

        public string GetAnchorValue(object target)
        {
            Domain d = target as Domain;

            if (d != null)
            {
                return d.DomainName;
            }

            throw new InvalidOperationException();
        }

        public string GetDNValue(object target)
        {
            Domain d = target as Domain;

            if (d == null)
            {
                throw new InvalidOperationException();
            }

            return d.DomainName;
        }

        public Task GetItems(IManagementAgentParameters config, Schema schema, BlockingCollection<object> collection)
        {
            Task t = new Task(() =>
            {
                Logger.WriteLine("Starting domains import task");

                DomainList list = DomainsRequestFactory.List(config.CustomerID);

                foreach (Domain d in list.Domains)
                {
                    string dn = this.GetDNValue(d);

                    if (dn == null)
                    {
                        Logger.WriteLine($"Domain {d} had no DN, ignoring");
                        continue;
                    }

                    collection.Add(ImportProcessor.GetCSEntryChange(d, schema.Types[SchemaConstants.Domain], config));
                }

                Logger.WriteLine("Domains import task complete");
            });

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
