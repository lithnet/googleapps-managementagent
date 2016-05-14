using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lithnet.GoogleApps.MA
{
    using Google.Apis.Admin.Directory.directory_v1.Data;
    using MetadirectoryServices;
    using Microsoft.MetadirectoryServices;
    using User = ManagedObjects.User;

    internal class ApiInterfaceGroup : IApiInterfaceObject
    {
        private static ApiInterfaceKeyedCollection internalInterfaces;

        static ApiInterfaceGroup()
        {
            ApiInterfaceGroup.internalInterfaces = new ApiInterfaceKeyedCollection
            {
                new ApiInterfaceGroupAliases(),
                new ApiInterfaceGroupMembership(),
                new ApiInterfaceGroupSettings()
            };
        }

        public ApiInterfaceGroup()
        {
        }

        public string Api => "group";


        public object CreateInstance(CSEntryChange csentry)
        {
            return new GoogleGroup();
        }

        public object GetInstance(CSEntryChange csentry)
        {
            return GroupRequestFactory.Get(csentry.GetAnchorValueOrDefault<string>("id") ?? csentry.DN);
        }

        public void DeleteInstance(CSEntryChange csentry)
        {
            GroupRequestFactory.Delete(csentry.GetAnchorValueOrDefault<string>("id") ?? csentry.DN);
        }

        public IList<AttributeChange> ApplyChanges(CSEntryChange csentry, SchemaType type, ref object target, bool patch = false)
        {
            bool hasChanged = false;
            List<AttributeChange> changes = new List<AttributeChange>();

            GoogleGroup group = target as GoogleGroup;

            if (group == null)
            {
                throw new InvalidOperationException();
            }

            if (this.SetDNValue(csentry, group))
            {
                hasChanged = true;
            }

            foreach (IAttributeAdapter typeDef in ManagementAgent.Schema[SchemaConstants.Group].Attributes.Where(t => t.Api == this.Api))
            {
                if (typeDef.UpdateField(csentry, target))
                {
                    hasChanged = true;
                }
            }

            if (hasChanged)
            {
                GoogleGroup result = new GoogleGroup();

                if (csentry.ObjectModificationType == ObjectModificationType.Add)
                {
                    result.Group = GroupRequestFactory.Add(group.Group);
                }
                else if (csentry.ObjectModificationType == ObjectModificationType.Replace || csentry.ObjectModificationType == ObjectModificationType.Update)
                {
                    if (patch)
                    {
                        result.Group = GroupRequestFactory.Patch(this.GetAnchorValue(target), group.Group);
                    }
                    else
                    {
                        result.Group = GroupRequestFactory.Update(this.GetAnchorValue(target), group.Group);
                    }
                }
                else
                {
                    throw new InvalidOperationException();
                }

                changes.AddRange(this.GetChanges(csentry.DN, csentry.ObjectModificationType, type, result));
            }

            foreach (IApiInterface i in ApiInterfaceGroup.internalInterfaces)
            {
                changes.AddRange(i.ApplyChanges(csentry, type, ref target, patch));
            }

            return changes;
        }

        public IList<AttributeChange> GetChanges(string dn, ObjectModificationType modType, SchemaType type, object source)
        {
            List<AttributeChange> attributeChanges = new List<AttributeChange>();

            GoogleGroup googleGroup = source as GoogleGroup;

            if (googleGroup == null)
            {
                throw new InvalidOperationException();
            }

            foreach (IAttributeAdapter typeDef in ManagementAgent.Schema[SchemaConstants.Group].Attributes.Where(t => t.Api == this.Api))
            {
                foreach (AttributeChange change in typeDef.CreateAttributeChanges(dn, modType, googleGroup.Group))
                {
                    if (type.HasAttribute(change.Name))
                    {
                        attributeChanges.Add(change);
                    }
                }
            }

            foreach (IApiInterface i in ApiInterfaceGroup.internalInterfaces)
            {
                attributeChanges.AddRange(i.GetChanges(dn, modType, type, source));
            }

            return attributeChanges;
        }

        public string GetAnchorValue(object target)
        {
            Group group;

            GoogleGroup googleGroup = target as GoogleGroup;

            if (googleGroup != null)
            {
                group = googleGroup.Group;
            }
            else
            {
                group = target as Group;
            }

            if (group == null)
            {
                throw new InvalidOperationException();
            }

            return group.Id;
        }

        public string GetDNValue(object target)
        {
            Group group;

            GoogleGroup googleGroup = target as GoogleGroup;

            if (googleGroup != null)
            {
                group = googleGroup.Group;
            }
            else
            {
                group = target as Group;
            }

            if (group == null)
            {
                throw new InvalidOperationException();
            }

            return group.Email;
        }
        
        private bool SetDNValue(CSEntryChange csentry, GoogleGroup e)
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

            e.Group.Email = newDN;

            return true;
        }
    }
}
