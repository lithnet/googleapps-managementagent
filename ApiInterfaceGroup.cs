﻿using System;
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

        public IList<AttributeChange> ApplyChanges(CSEntryChange csentry, SchemaType type, object target, bool patch = false)
        {
            bool hasChanged = false;
            List<AttributeChange> changes = new List<AttributeChange>();

            foreach (IMASchemaAttribute typeDef in ManagementAgent.Schema[SchemaConstants.Group].Attributes.Where(t => t.Api == this.Api))
            {
                if (typeDef.UpdateField(csentry, target))
                {
                    hasChanged = true;
                }
            }

            if (hasChanged)
            {
                Group result;

                if (csentry.ObjectModificationType == ObjectModificationType.Add)
                {
                    result = GroupRequestFactory.Add((Group)target);
                }
                else if (csentry.ObjectModificationType == ObjectModificationType.Replace || csentry.ObjectModificationType == ObjectModificationType.Update)
                {
                    if (patch)
                    {
                        result = GroupRequestFactory.Patch(this.GetAnchorValue(target), (Group)target);
                    }
                    else
                    {
                        result = GroupRequestFactory.Update(this.GetAnchorValue(target), (Group)target);
                    }
                }
                else
                {
                    throw new InvalidOperationException();
                }

                changes.AddRange(this.GetChanges(csentry.ObjectModificationType, type, result));
            }

            foreach (IApiInterface i in ApiInterfaceGroup.internalInterfaces)
            {
                changes.AddRange(i.ApplyChanges(csentry, type, target, patch));
            }

            return changes;
        }

        public IList<AttributeChange> GetChanges(ObjectModificationType modType, SchemaType type, object source)
        {
            List<AttributeChange> attributeChanges = new List<AttributeChange>();

            GoogleGroup googleGroup = source as GoogleGroup;

            if (googleGroup == null)
            {
                throw new InvalidOperationException();
            }

            foreach (IMASchemaAttribute typeDef in ManagementAgent.Schema[SchemaConstants.Group].Attributes.Where(t => t.Api == this.Api))
            {
                foreach (AttributeChange change in typeDef.CreateAttributeChanges(modType, googleGroup.Group))
                {
                    if (type.HasAttribute(change.Name))
                    {
                        attributeChanges.Add(change);
                    }
                }
            }

            foreach (IApiInterface i in ApiInterfaceGroup.internalInterfaces)
            {
                attributeChanges.AddRange(i.GetChanges(modType, type, source));
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
    }
}