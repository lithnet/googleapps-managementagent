using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lithnet.GoogleApps.MA
{
    using ManagedObjects;
    using MetadirectoryServices;
    using Microsoft.MetadirectoryServices;

    internal class ApiInterfaceUser : IApiInterfaceObject
    {
      
        private static ApiInterfaceKeyedCollection internalInterfaces;

        static ApiInterfaceUser()
        {
            ApiInterfaceUser.internalInterfaces = new ApiInterfaceKeyedCollection { new ApiInterfaceUserAliases(), new ApiInterfaceUserMakeAdmin() };
        }

        public string Api => "user";

        public object CreateInstance(CSEntryChange csentry)
        {
            if (csentry.ObjectModificationType == ObjectModificationType.Add)
            {
                return new User
                {
                    Password = Guid.NewGuid().ToString("B"),
                    PrimaryEmail = csentry.DN
                };
            }
            else
            {
                return new User
                {
                    Id = csentry.GetAnchorValueOrDefault<string>(ManagementAgent.Schema[SchemaConstants.User].AnchorAttributeName)
                };
            }
        }

        public object GetInstance(CSEntryChange csentry)
        {
            return UserRequestFactory.Get(csentry.GetAnchorValueOrDefault<string>("id") ?? csentry.DN);
        }

        public void DeleteInstance(CSEntryChange csentry)
        {
            UserRequestFactory.Delete(csentry.GetAnchorValueOrDefault<string>("id") ?? csentry.DN);
        }

        public IList<AttributeChange> ApplyChanges(CSEntryChange csentry, SchemaType type, object target, bool patch = false)
        {
            bool hasChanged = false;

            List<AttributeChange> changes = new List<AttributeChange>();

            foreach (IMASchemaAttribute typeDef in ManagementAgent.Schema[SchemaConstants.User].Attributes.Where(t => t.Api == this.Api))
            {
                if (typeDef.UpdateField(csentry, target))
                {
                    hasChanged = true;
                }
            }

            if (hasChanged)
            {
                User result;

                if (csentry.ObjectModificationType == ObjectModificationType.Add)
                {
                    result = UserRequestFactory.Add((User)target);
                }
                else if (csentry.ObjectModificationType == ObjectModificationType.Replace || csentry.ObjectModificationType == ObjectModificationType.Update)
                {
                    if (patch)
                    {
                        result = UserRequestFactory.Patch((User)target, this.GetAnchorValue(target));
                    }
                    else
                    {
                        result = UserRequestFactory.Update((User)target, this.GetAnchorValue(target));
                    }
                }
                else
                {
                    throw new InvalidOperationException();
                }

                changes.AddRange(this.GetChanges(csentry.ObjectModificationType, type, result));
            }

            foreach (IApiInterface i in ApiInterfaceUser.internalInterfaces)
            {
                changes.AddRange(i.ApplyChanges(csentry, type, target, patch));
            }

            return changes;
        }

        public IList<AttributeChange> GetChanges(ObjectModificationType modType, SchemaType type, object source)
        {
            List<AttributeChange> attributeChanges = new List<AttributeChange>();

            foreach (IMASchemaAttribute typeDef in ManagementAgent.Schema[SchemaConstants.User].Attributes.Where(t => t.Api == this.Api))
            {
                foreach (AttributeChange change in typeDef.CreateAttributeChanges(modType, source))
                {
                    if (type.HasAttribute(change.Name))
                    {
                        attributeChanges.Add(change);
                    }
                }
            }

            foreach (IApiInterface i in ApiInterfaceUser.internalInterfaces)
            {
                attributeChanges.AddRange(i.GetChanges(modType, type, source));
            }

            return attributeChanges;
        }

        public string GetAnchorValue(object target)
        {
            return ((User)target).Id;
        }

        public string GetDNValue(object target)
        {
            return ((User)target).PrimaryEmail;
        }
    }
}
