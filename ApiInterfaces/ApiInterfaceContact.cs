using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lithnet.GoogleApps.MA
{
    using Google.Contacts;
    using Google.GData.Contacts;
    using MetadirectoryServices;
    using Microsoft.MetadirectoryServices;

    internal class ApiInterfaceContact : IApiInterfaceObject
    {
        private static ApiInterfaceKeyedCollection internalInterfaces;

        private string domain;

        static ApiInterfaceContact()
        {
            ApiInterfaceContact.internalInterfaces = new ApiInterfaceKeyedCollection();
        }

        public ApiInterfaceContact(string domain)
        {
            this.domain = domain;
        }

        public string Api => "contact";


        public object CreateInstance(CSEntryChange csentry)
        {
            return new ContactEntry();
        }

        public object GetInstance(CSEntryChange csentry)
        {
            return ContactRequestFactory.GetContact(csentry.GetAnchorValueOrDefault<string>("id"));
        }

        public void DeleteInstance(CSEntryChange csentry)
        {
            ContactRequestFactory.DeleteContact(csentry.GetAnchorValueOrDefault<string>("id"));
        }

        public IList<AttributeChange> ApplyChanges(CSEntryChange csentry, SchemaType type, object target, bool patch = false)
        {
            bool hasChanged = false;
            List<AttributeChange> changes = new List<AttributeChange>();
            Contact obj = (Contact)target;

            foreach (IMASchemaAttribute typeDef in ManagementAgent.Schema[SchemaConstants.Contact].Attributes.Where(t => t.Api == this.Api))
            {
                if (typeDef.UpdateField(csentry, obj.ContactEntry))
                {
                    hasChanged = true;
                }
            }

            if (hasChanged)
            {
                Contact result;

                if (csentry.ObjectModificationType == ObjectModificationType.Add)
                {
                    result = ContactRequestFactory.CreateContact(obj, this.domain);
                }
                else if (csentry.ObjectModificationType == ObjectModificationType.Replace || csentry.ObjectModificationType == ObjectModificationType.Update)
                {
                    if (patch)
                    {
                        throw new InvalidOperationException();
                    }
                    else
                    {
                        result = ContactRequestFactory.UpdateContact(obj);
                    }
                }
                else
                {
                    throw new InvalidOperationException();
                }

                changes.AddRange(this.GetChanges(csentry.ObjectModificationType, type, result));
            }

            foreach (IApiInterface i in ApiInterfaceContact.internalInterfaces)
            {
                changes.AddRange(i.ApplyChanges(csentry, type, target, patch));
            }

            return changes;
        }

        public IList<AttributeChange> GetChanges(ObjectModificationType modType, SchemaType type, object source)
        {
            List<AttributeChange> attributeChanges = new List<AttributeChange>();

            Contact entry = source as Contact;

            if (entry == null)
            {
                throw new InvalidOperationException();
            }

            foreach (IMASchemaAttribute typeDef in ManagementAgent.Schema[SchemaConstants.Contact].Attributes.Where(t => t.Api == this.Api))
            {
                foreach (AttributeChange change in typeDef.CreateAttributeChanges(modType, entry.ContactEntry))
                {
                    if (type.HasAttribute(change.Name))
                    {
                        attributeChanges.Add(change);
                    }
                }
            }

            foreach (IApiInterface i in ApiInterfaceContact.internalInterfaces)
            {
                attributeChanges.AddRange(i.GetChanges(modType, type, source));
            }

            return attributeChanges;
        }

        public string GetAnchorValue(object target)
        {
            ContactEntry contactEntry = target as ContactEntry;

            if (contactEntry != null)
            {
                return contactEntry.Id.ToString();
            }

            Contact contact = target as Contact;

            if (contact != null)
            {
                return contact.Id;
            }

            throw new InvalidOperationException();
        }

        public string GetDNValue(object target)
        {
            ContactEntry contactEntry = target as ContactEntry;

            if (contactEntry != null)
            {
                return "contact:" + contactEntry.PrimaryEmail.Address;
            }

            Contact contact = target as Contact;

            if (contact != null)
            {
                return "contact:" + contact.PrimaryEmail.Address;
            }

            throw new InvalidOperationException();
        }
    }
}
