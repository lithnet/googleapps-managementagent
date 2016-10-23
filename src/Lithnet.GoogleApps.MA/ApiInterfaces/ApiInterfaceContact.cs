using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Lithnet.Logging;
using System.Diagnostics.CodeAnalysis;
using Google.GData.Contacts;
using Google.GData.Extensions;
using Lithnet.MetadirectoryServices;
using Microsoft.MetadirectoryServices;

namespace Lithnet.GoogleApps.MA
{
    internal class ApiInterfaceContact : IApiInterfaceObject
    {
        internal const string DNAttributeName = "lithnet-google-ma-dn";

        [SuppressMessage("ReSharper", "CollectionNeverUpdated.Local")]
        private static ApiInterfaceKeyedCollection internalInterfaces;

        private string domain;

        protected MASchemaType SchemaType { get; set; }

        static ApiInterfaceContact()
        {
            ApiInterfaceContact.internalInterfaces = new ApiInterfaceKeyedCollection();
        }

        public ApiInterfaceContact(string domain, MASchemaType type)
        {
            this.domain = domain;
            this.SchemaType = type;
        }

        public string Api => "contact";


        public object CreateInstance(CSEntryChange csentry)
        {
            return new ContactEntry();
        }

        public object GetInstance(CSEntryChange csentry)
        {
            string id = csentry.GetAnchorValueOrDefault<string>("id");

            if (id == null)
            {
                throw new AttributeNotPresentException("id");
            }

            return ContactRequestFactory.GetContact(id);
        }

        public void DeleteInstance(CSEntryChange csentry)
        {
            string id = csentry.GetAnchorValueOrDefault<string>("id");

            if (id == null)
            {
                throw new AttributeNotPresentException("id");
            }

            ContactRequestFactory.Delete(id);
        }

        public IList<AttributeChange> ApplyChanges(CSEntryChange csentry, SchemaType type, IManagementAgentParameters config, ref object target, bool patch = false)
        {
            bool hasChanged = false;
            List<AttributeChange> changes = new List<AttributeChange>();
            ContactEntry obj = (ContactEntry)target;

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
                ContactEntry result;

                if (csentry.ObjectModificationType == ObjectModificationType.Add)
                {
                    result = ContactRequestFactory.Add(obj, this.domain);
                    target = result;
                }
                else if (csentry.ObjectModificationType == ObjectModificationType.Replace || csentry.ObjectModificationType == ObjectModificationType.Update)
                {
                    if (patch)
                    {
                        throw new InvalidOperationException();
                    }
                    else
                    {
                        result = ContactRequestFactory.Update(obj);
                        target = result;
                    }
                }
                else
                {
                    throw new InvalidOperationException();
                }

                changes.AddRange(this.GetLocalChanges(csentry.DN, csentry.ObjectModificationType, type, result));
            }

            foreach (IApiInterface i in ApiInterfaceContact.internalInterfaces)
            {
                changes.AddRange(i.ApplyChanges(csentry, type, config, ref target, patch));
            }

            return changes;
        }

        public IList<AttributeChange> GetChanges(string dn, ObjectModificationType modType, SchemaType type, object source, IManagementAgentParameters config)
        {
            List<AttributeChange> attributeChanges = this.GetLocalChanges(dn, modType, type, source);

            foreach (IApiInterface i in ApiInterfaceContact.internalInterfaces)
            {
                attributeChanges.AddRange(i.GetChanges(dn, modType, type, source, config));
            }

            return attributeChanges;
        }

        private List<AttributeChange> GetLocalChanges(string dn, ObjectModificationType modType, SchemaType type, object source)
        {
            List<AttributeChange> attributeChanges = new List<AttributeChange>();

            ContactEntry entry = source as ContactEntry;

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
            ContactEntry contactEntry = target as ContactEntry;

            if (contactEntry != null)
            {
                return contactEntry.SelfUri.Content;
            }

            throw new InvalidOperationException();
        }

        public string GetDNValue(object target)
        {
            ContactEntry contactEntry = target as ContactEntry;

            if (contactEntry == null)
            {
                throw new InvalidOperationException();
            }

            if (contactEntry.ExtendedProperties.Count > 0)
            {
                ExtendedProperty dn = contactEntry.ExtendedProperties.FirstOrDefault(t => t.Name == ApiInterfaceContact.DNAttributeName);

                if (!string.IsNullOrEmpty(dn?.Value))
                {
                    return dn.Value;
                }
            }

            return contactEntry.PrimaryEmail == null ? null : "contact:" + contactEntry.PrimaryEmail.Address;
        }

        public Task GetItems(IManagementAgentParameters config, Schema schema, BlockingCollection<object> collection)
        {
            Task t = new Task(() =>
            {
                Logger.WriteLine("Starting contacts import task");

                HashSet<string> seenDNs = new HashSet<string>();

                foreach (ContactEntry contact in ContactRequestFactory.GetContacts(config.Domain))
                {
                    if (!string.IsNullOrWhiteSpace(config.ContactRegexFilter))
                    {
                        if (contact.PrimaryEmail != null)
                        {
                            if (!Regex.IsMatch(contact.PrimaryEmail.Address, config.ContactRegexFilter, RegexOptions.IgnoreCase))
                            {
                                continue;
                            }
                        }
                    }

                    string dn = this.GetDNValue(contact);

                    if (dn == null)
                    {
                        Logger.WriteLine($"Contact {contact.SelfUri.Content} had no DN or primary email attribute, ignoring");
                        continue;
                    }

                    if (!seenDNs.Add(dn))
                    {
                        Logger.WriteLine($"Ignoring contact {contact.SelfUri.Content} with duplicate dn {dn}");
                        continue;
                    }


                    collection.Add(ImportProcessor.GetCSEntryChange(contact, schema.Types[SchemaConstants.Contact], config));
                }

                Logger.WriteLine("Contacts import task complete");
            });

            t.Start();

            return t;
        }

        public bool SetDNValue(CSEntryChange csentry, ContactEntry e)
        {
            if (csentry.ObjectModificationType == ObjectModificationType.Add)
            {
                ExtendedProperty dn = e.ExtendedProperties.FirstOrDefault(t => t.Name == ApiInterfaceContact.DNAttributeName);

                if (dn == null)
                {
                    dn = new ExtendedProperty { Name = ApiInterfaceContact.DNAttributeName };
                    e.ExtendedProperties.Add(dn);
                }

                dn.Value = csentry.DN;
                return true;
            }
            else
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

                ExtendedProperty dn = e.ExtendedProperties.FirstOrDefault(t => t.Name == ApiInterfaceContact.DNAttributeName);

                if (dn == null)
                {
                    dn = new ExtendedProperty { Name = ApiInterfaceContact.DNAttributeName };
                    e.ExtendedProperties.Add(dn);
                }

                dn.Value = newDN;
                return true;
            }
        }
    }
}
