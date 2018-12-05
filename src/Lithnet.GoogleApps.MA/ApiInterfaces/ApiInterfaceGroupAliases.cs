using System;
using System.Collections.Generic;
using System.Linq;
using Google.Apis.Admin.Directory.directory_v1.Data;
using Lithnet.Logging;
using Lithnet.MetadirectoryServices;
using Microsoft.MetadirectoryServices;

namespace Lithnet.GoogleApps.MA
{
    internal class ApiInterfaceGroupAliases : IApiInterface
    {
        private IManagementAgentParameters config;

        public string Api => "groupaliases";

        public ApiInterfaceGroupAliases(IManagementAgentParameters config)
        {
            this.config = config;
        }

        public IList<AttributeChange> ApplyChanges(CSEntryChange csentry, SchemaType type, ref object target, bool patch = false)
        {
            GoogleGroup g = (GoogleGroup)target;

            AttributeChange change = this.ApplyGroupAliasChanges(csentry, g.Group);

            List<AttributeChange> changes = new List<AttributeChange>();

            if (change != null)
            {
                changes.Add(change);
            }

            return changes;
        }

        public IList<AttributeChange> GetChanges(string dn, ObjectModificationType modType, SchemaType type, object source)
        {
            GoogleGroup group = source as GoogleGroup;

            if (group == null)
            {
                throw new InvalidOperationException();
            }

            List<AttributeChange> attributeChanges = new List<AttributeChange>();

            foreach (IAttributeAdapter typeDef in ManagementAgent.Schema[SchemaConstants.Group].AttributeAdapters.Where(t => t.Api == this.Api))
            {
                foreach (string attributeName in typeDef.MmsAttributeNames)
                {
                    if (type.HasAttribute(attributeName))
                    {
                        attributeChanges.AddRange(typeDef.CreateAttributeChanges(dn, modType, group.Group));
                    }
                }
            }

            return attributeChanges;
        }

        private void GetGroupAliasChanges(CSEntryChange csentry, out IList<string> aliasAdds, out IList<string> aliasDeletes, out bool deletingAll)
        {
            aliasAdds = new List<string>();
            aliasDeletes = new List<string>();
            AttributeChange change = csentry.AttributeChanges.FirstOrDefault(t => t.Name == "aliases");
            deletingAll = false;

            if (csentry.ObjectModificationType == ObjectModificationType.Replace)
            {
                if (change != null)
                {
                    aliasAdds = change.GetValueAdds<string>();
                }

                foreach (string alias in this.config.GroupsService.GetAliases(csentry.DN).Except(aliasAdds))
                {
                    aliasDeletes.Add(alias);
                }
            }
            else
            {
                if (change == null)
                {
                    return;
                }

                switch (change.ModificationType)
                {
                    case AttributeModificationType.Add:
                        aliasAdds = change.GetValueAdds<string>();
                        break;

                    case AttributeModificationType.Delete:
                        foreach (string alias in this.config.GroupsService.GetAliases(csentry.DN))
                        {
                            aliasDeletes.Add(alias);
                        }

                        deletingAll = true;
                        break;

                    case AttributeModificationType.Replace:
                        aliasAdds = change.GetValueAdds<string>();
                        foreach (string alias in this.config.GroupsService.GetAliases(csentry.DN).Except(aliasAdds))
                        {
                            aliasDeletes.Add(alias);
                        }
                        break;

                    case AttributeModificationType.Update:
                        aliasAdds = change.GetValueAdds<string>();
                        aliasDeletes = change.GetValueDeletes<string>();
                        break;

                    case AttributeModificationType.Unconfigured:
                    default:
                        throw new InvalidOperationException("Unknown or unsupported modification type");
                }
            }
        }

        private AttributeChange ApplyGroupAliasChanges(CSEntryChange csentry, Group group)
        {
            this.GetGroupAliasChanges(csentry, out IList<string> aliasAdds, out IList<string> aliasDeletes, out bool deletingAll);

            if (aliasAdds.Count == 0 && aliasDeletes.Count == 0)
            {
                return null;
            }

            AttributeChange change = null;
            IList<ValueChange> valueChanges = new List<ValueChange>();

            try
            {
                if (aliasDeletes != null)
                {
                    foreach (string alias in aliasDeletes)
                    {
                        if (!group.Email.Equals(alias, StringComparison.CurrentCultureIgnoreCase))
                        {
                            Logger.WriteLine($"Removing alias {alias}", LogLevel.Debug);
                            this.config.GroupsService.RemoveAlias(csentry.DN, alias);
                        }

                        valueChanges.Add(ValueChange.CreateValueDelete(alias));
                    }
                }

                foreach (string alias in aliasAdds)
                {
                    if (!csentry.DN.Equals(alias, StringComparison.CurrentCultureIgnoreCase))
                    {
                        Logger.WriteLine($"Adding alias {alias}", LogLevel.Debug);
                        this.config.GroupsService.AddAlias(csentry.DN, alias);
                    }

                    valueChanges.Add(ValueChange.CreateValueAdd(alias));
                }
            }
            finally
            {
                if (valueChanges.Count > 0)
                {
                    if (csentry.ObjectModificationType == ObjectModificationType.Update)
                    {
                        if (deletingAll && valueChanges.Count == aliasDeletes?.Count)
                        {
                            change = AttributeChange.CreateAttributeDelete("aliases");
                        }
                        else
                        {
                            change = AttributeChange.CreateAttributeUpdate("aliases", valueChanges);
                        }
                    }
                    else
                    {
                        change = AttributeChange.CreateAttributeAdd("aliases", valueChanges.Where(u => u.ModificationType == ValueModificationType.Add).Select(t => t.Value).ToList());
                    }
                }
            }

            return change;
        }
    }
}