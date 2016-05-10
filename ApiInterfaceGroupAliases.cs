using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lithnet.GoogleApps.MA
{
    using Google.Apis.Admin.Directory.directory_v1.Data;
    using ManagedObjects;
    using MetadirectoryServices;
    using Microsoft.MetadirectoryServices;

    public class ApiInterfaceGroupAliases : ApiInterface
    {
        private static MASchemaType maType = SchemaBuilder.GetGroupSchema();

        public ApiInterfaceGroupAliases()
        {
            this.Api = "groupaliases";
        }

        public override bool IsPrimary => false;

        public override object CreateInstance(CSEntryChange csentry)
        {
            return new List<string>();
        }

        public override object GetInstance(CSEntryChange csentry)
        {
            throw new NotImplementedException();
        }

        public override void DeleteInstance(CSEntryChange csentry)
        {
            throw new NotImplementedException();
        }

        public override IList<AttributeChange> ApplyChanges(CSEntryChange csentry, SchemaType type, object target, bool patch = false)
        {
            Func<AttributeChange> x = () => ApiInterfaceGroupAliases.ApplyGroupAliasChanges(csentry, (Group)target);
            AttributeChange change = x.ExecuteWithRetryOnNotFound();

            List<AttributeChange> changes = new List<AttributeChange>();

            if (change != null)
            {
                changes.Add(change);
            }

            return changes;
        }

        public override IList<AttributeChange> GetChanges(ObjectModificationType modType, SchemaType type, object source)
        {
            List<AttributeChange> attributeChanges = new List<AttributeChange>();

            foreach (IMASchemaAttribute typeDef in ApiInterfaceGroupAliases.maType.Attributes.Where(t => t.Api == this.Api))
            {
                if (type.HasAttribute(typeDef.AttributeName))
                {
                    attributeChanges.AddRange(typeDef.CreateAttributeChanges(modType, source));
                }
            }

            return attributeChanges;
        }

        public override string GetAnchorValue(object target)
        {
            throw new NotImplementedException();
        }

        public override string GetDNValue(object target)
        {
            throw new NotImplementedException();
        }

        private static void GetGroupAliasChanges(CSEntryChange csentry, Group group, out IList<string> aliasAdds, out IList<string> aliasDeletes)
        {
            aliasAdds = new List<string>();
            aliasDeletes = new List<string>();
            AttributeChange change = csentry.AttributeChanges.FirstOrDefault(t => t.Name == "aliases");

            if (csentry.ObjectModificationType == ObjectModificationType.Replace)
            {
                if (change != null)
                {
                    aliasAdds = change.GetValueAdds<string>();
                }

                foreach (string alias in GroupRequestFactory.GetAliases(csentry.DN).Except(aliasAdds))
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
                        foreach (string alias in group.Aliases)
                        {
                            aliasDeletes.Add(alias);
                        }
                        break;

                    case AttributeModificationType.Replace:
                        aliasAdds = change.GetValueAdds<string>();
                        foreach (string alias in group.Aliases.Except(aliasAdds))
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

        private static AttributeChange ApplyGroupAliasChanges(CSEntryChange csentry, Group group)
        {
            IList<string> aliasAdds;
            IList<string> aliasDeletes;

            ApiInterfaceGroupAliases.GetGroupAliasChanges(csentry, group, out aliasAdds, out aliasDeletes);

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
                        GroupRequestFactory.RemoveAlias(csentry.DN, alias);
                        valueChanges.Add(ValueChange.CreateValueDelete(alias));
                    }
                }

                foreach (string alias in aliasAdds)
                {
                    GroupRequestFactory.AddAlias(csentry.DN, alias);
                    valueChanges.Add(ValueChange.CreateValueAdd(alias));
                }
            }
            finally
            {
                if (valueChanges.Count > 0)
                {
                    if (csentry.ObjectModificationType == ObjectModificationType.Update)
                    {
                        change = AttributeChange.CreateAttributeUpdate("aliases", valueChanges);
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
