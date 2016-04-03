using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.MetadirectoryServices;
using Newtonsoft.Json.Linq;
using Lithnet.Logging;
using G = Google.Apis.Admin.Directory.directory_v1.Data;
using Lithnet.GoogleApps.ManagedObjects;
using Lithnet.MetadirectoryServices;

namespace Lithnet.GoogleApps.MA
{
    public static class CSEntryChangeFactoryGroup
    {
        public static CSEntryChange GroupToCSE(GoogleGroup group, IManagementAgentParameters config, Schema types)
        {
            if (!types.Types.Contains("group"))
            {
                throw new InvalidOperationException("The type 'group' was not requested");
            }

            SchemaType type = types.Types["group"];

            CSEntryChange csentry = CSEntryChange.Create();
            csentry.ObjectModificationType = ObjectModificationType.Add;
            csentry.ObjectType = "group";
            csentry.DN = group.Group.Email;
            csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("id", group.Group.Id));

            GroupToCSEntryChange.GroupCoreToCSEntryChange(group.Group, type, csentry);

            if (group.Settings != null)
            {
                GroupToCSEntryChange.GroupSettingsToCSEntryChange(group.Settings, type, csentry);
            }

            GroupToCSEntryChange.GroupMembersToCSEntryChange(group.Membership, type, csentry);

            return csentry;
        }

        private static bool CanPatchGroup(CSEntryChange csentry)
        {
            return false;
            return !csentry.AttributeChanges.Any(t => ManagementAgentSchema.GroupAttributesRequiringFullUpdate.Any(u => u == t.Name));
        }

        public static CSEntryChangeResult PutCSEntryChangeGroup(CSEntryChange csentry, IManagementAgentParameters config, SchemaType type)
        {
            CSEntryChange deltaCSEntry = CSEntryChange.Create();
            AnchorAttribute anchor = csentry.GetAnchorAttribute("id");
            if (anchor != null)
            {
                deltaCSEntry.AnchorAttributes.Add(anchor);
            }

            deltaCSEntry.ObjectType = csentry.ObjectType;
            CSEntryChangeFactory.SetDeltaDNOnRename(csentry, deltaCSEntry);

            try
            {
                switch (csentry.ObjectModificationType)
                {
                    case ObjectModificationType.Add:
                        return PutCSEntryChangeGroupAdd(csentry, deltaCSEntry, type);

                    case ObjectModificationType.Delete:
                        return PutCSEntryChangeGroupDelete(csentry, deltaCSEntry, type);

                    case ObjectModificationType.Replace:
                        return PutCSEntryChangeGroupReplace(csentry, deltaCSEntry, type);

                    case ObjectModificationType.Update:
                        return PutCSEntryChangeGroupUpdate(csentry, deltaCSEntry, type);

                    default:
                    case ObjectModificationType.None:
                    case ObjectModificationType.Unconfigured:
                        throw new InvalidOperationException(string.Format("Unknown modification type: {0} on object {1}", csentry.ObjectModificationType, csentry.DN));
                }
            }
            finally
            {
                if (deltaCSEntry.AttributeChanges.Count > 0 || deltaCSEntry.ObjectModificationType == ObjectModificationType.Delete)
                {
                    CSEntryChangeQueue.Add(deltaCSEntry);
                }
            }
        }

        private static CSEntryChangeResult PutCSEntryChangeGroupDelete(CSEntryChange csentry, CSEntryChange deltaCSEntry, SchemaType type)
        {
            deltaCSEntry.ObjectModificationType = csentry.ObjectModificationType;
            GroupRequestFactory.Delete(csentry.DN);
            return CSEntryChangeResult.Create(csentry.Identifier, null, MAExportError.Success);
        }

        private static CSEntryChangeResult PutCSEntryChangeGroupAdd(CSEntryChange csentry, CSEntryChange deltaCSEntry, SchemaType type)
        {
            deltaCSEntry.ObjectModificationType = csentry.ObjectModificationType;
            GoogleGroup group = new GoogleGroup();

            if (!CSEntryChangeToGroup.CSEntryChangeToGroupCore(csentry, group.Group))
            {
                throw new NotSupportedException("The CSEntryChange did not contain the mandatory fields required to create a group");
            }

            group.Group = GroupRequestFactory.Add(group.Group);
            deltaCSEntry.AnchorAttributes.Add(AnchorAttribute.Create("id", group.Group.Id));

            Func<bool> x = () => CSEntryChangeToGroup.ApplyAliasChanges(csentry, deltaCSEntry, group.Group);
            x.ExecuteWithRetryOnNotFound();

            if (CSEntryChangeToGroup.CSEntryChangeToGroupSettings(csentry, group.Settings))
            {
                Func<GroupSettings> y = () => GroupSettingsRequestFactory.Update(csentry.DN, group.Settings);
                group.Settings = y.ExecuteWithRetryOnNotFound();
            }

            Action z = () => CSEntryChangeToGroup.ApplyMembershipChanges(csentry, deltaCSEntry);
            z.ExecuteWithRetryOnNotFound();

            GroupToCSEntryChange.GroupCoreToCSEntryChange(group.Group, type, deltaCSEntry);
            GroupToCSEntryChange.GroupSettingsToCSEntryChange(group.Settings, type, deltaCSEntry);

            return CSEntryChangeResult.Create(csentry.Identifier, null, MAExportError.Success);
        }

        private static CSEntryChangeResult PutCSEntryChangeGroupReplace(CSEntryChange csentry, CSEntryChange deltaCSEntry, SchemaType type)
        {
            deltaCSEntry.ObjectModificationType = csentry.ObjectModificationType;
            GoogleGroup group = new GoogleGroup();

            CSEntryChangeToGroup.CSEntryChangeToGroupCore(csentry, group.Group);

            group.Group = GroupRequestFactory.Update(deltaCSEntry.DN, group.Group);

            CSEntryChangeToGroup.ApplyAliasChanges(csentry, deltaCSEntry, group.Group);

            if (CSEntryChangeToGroup.CSEntryChangeToGroupSettings(csentry, group.Settings))
            {
                group.Settings = GroupSettingsRequestFactory.Update(csentry.DN, group.Settings);
            }

            CSEntryChangeToGroup.ApplyMembershipChanges(csentry, deltaCSEntry);

            GroupToCSEntryChange.GroupCoreToCSEntryChange(group.Group, type, deltaCSEntry);
            GroupToCSEntryChange.GroupSettingsToCSEntryChange(group.Settings, type, deltaCSEntry);

            return CSEntryChangeResult.Create(csentry.Identifier, null, MAExportError.Success);
        }

        private static CSEntryChangeResult PutCSEntryChangeGroupUpdate(CSEntryChange csentry, CSEntryChange deltaCSEntry, SchemaType type)
        {
            deltaCSEntry.ObjectModificationType = csentry.ObjectModificationType;
            GoogleGroup group = new GoogleGroup();

            CSEntryChangeToGroup.CSEntryChangeToGroupCore(csentry, group.Group);

            group.Group = GroupRequestFactory.Update(deltaCSEntry.DN, group.Group);

            CSEntryChangeToGroup.ApplyAliasChanges(csentry, deltaCSEntry, group.Group);

            if (CSEntryChangeToGroup.CSEntryChangeToGroupSettings(csentry, group.Settings))
            {
                group.Settings = GroupSettingsRequestFactory.Patch(csentry.DN, group.Settings);
            }

            CSEntryChangeToGroup.ApplyMembershipChanges(csentry, deltaCSEntry);

            GroupToCSEntryChange.GroupCoreToCSEntryChange(group.Group, type, deltaCSEntry);
            GroupToCSEntryChange.GroupSettingsToCSEntryChange(group.Settings, type, deltaCSEntry);

            return CSEntryChangeResult.Create(csentry.Identifier, null, MAExportError.Success);
        }
    }
}
