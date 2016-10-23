using System;
using System.Collections.Generic;
using System.Linq;
using Lithnet.Logging;
using Lithnet.GoogleApps.ManagedObjects;
using Lithnet.MetadirectoryServices;
using Microsoft.MetadirectoryServices;

namespace Lithnet.GoogleApps.MA
{
    internal class ApiInterfaceUserAliases : IApiInterface
    {
        public string Api => "useraliases";

        public IList<AttributeChange> ApplyChanges(CSEntryChange csentry, SchemaType type, IManagementAgentParameters config, ref object target, bool patch = false)
        {
            User user = (User)target;
            Func<AttributeChange> x = () => ApiInterfaceUserAliases.ApplyUserAliasChanges(csentry, user);
            AttributeChange change = x.ExecuteWithRetryOnNotFound();

            List<AttributeChange> changes = new List<AttributeChange>();

            if (change != null)
            {
                changes.Add(change);
            }

            return changes;
        }

        public IList<AttributeChange> GetChanges(string dn, ObjectModificationType modType, SchemaType type, object source, IManagementAgentParameters config)
        {
            List<AttributeChange> attributeChanges = new List<AttributeChange>();

            foreach (IAttributeAdapter typeDef in ManagementAgent.Schema[SchemaConstants.User].Attributes.Where(t => t.Api == this.Api))
            {
                if (type.HasAttribute(typeDef.AttributeName))
                {
                    attributeChanges.AddRange(typeDef.CreateAttributeChanges(dn, modType, source));
                }
            }

            return attributeChanges;
        }

        private static void GetUserAliasChanges(CSEntryChange csentry, out IList<string> aliasAdds, out IList<string> aliasDeletes, out bool deletingAll)
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

                foreach (string alias in UserRequestFactory.GetAliases(csentry.DN).Except(aliasAdds))
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
                        foreach (string alias in UserRequestFactory.GetAliases(csentry.DN))
                        {
                            aliasDeletes.Add(alias);
                        }

                        deletingAll = true;
                        break;

                    case AttributeModificationType.Replace:
                        aliasAdds = change.GetValueAdds<string>();
                        foreach (string alias in UserRequestFactory.GetAliases(csentry.DN).Except(aliasAdds))
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

        private static AttributeChange ApplyUserAliasChanges(CSEntryChange csentry, User user)
        {
            IList<string> aliasAdds;
            IList<string> aliasDeletes;
            bool deletingAll;

            ApiInterfaceUserAliases.GetUserAliasChanges(csentry, out aliasAdds, out aliasDeletes, out deletingAll);

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
                        if (!user.PrimaryEmail.Equals(alias, StringComparison.CurrentCultureIgnoreCase))
                        {
                            Logger.WriteLine($"Removing alias {alias}", LogLevel.Debug);

                            try
                            {
                                UserRequestFactory.RemoveAlias(csentry.DN, alias);
                            }
                            catch (Google.GoogleApiException ex)
                            {
                                if (ex.HttpStatusCode == System.Net.HttpStatusCode.NotFound || 
                                    ex.Error?.Message == "Invalid Input: resource_id")
                                {
                                    Logger.WriteLine($"Alias {alias} does not exist on object");
                                }
                                else
                                {
                                    throw;
                                }
                            }
                        }

                        valueChanges.Add(ValueChange.CreateValueDelete(alias));
                    }
                }

                foreach (string alias in aliasAdds)
                {
                    if (!csentry.DN.Equals(alias, StringComparison.CurrentCultureIgnoreCase))
                    {
                        Logger.WriteLine($"Adding alias {alias}", LogLevel.Debug);
                        UserRequestFactory.AddAlias(csentry.DN, alias);
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