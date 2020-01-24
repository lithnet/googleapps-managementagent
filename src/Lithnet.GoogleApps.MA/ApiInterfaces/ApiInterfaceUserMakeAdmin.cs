using System;
using System.Collections.Generic;
using System.Linq;
using Lithnet.MetadirectoryServices;
using Microsoft.MetadirectoryServices;

namespace Lithnet.GoogleApps.MA
{
    internal class ApiInterfaceUserMakeAdmin : IApiInterface
    {
        private IManagementAgentParameters config;

        public string Api => "usermakeadmin";

        public ApiInterfaceUserMakeAdmin(IManagementAgentParameters config)
        {
            this.config = config;
        }

        public void ApplyChanges(CSEntryChange csentry, CSEntryChange committedChanges, SchemaType type, ref object target, bool patch = false)
        {
            AttributeChange change = csentry.AttributeChanges.FirstOrDefault((t => t.Name == "isAdmin"));

            if (change != null)
            {
                bool makeAdmin = change.GetValueAdd<bool>();
                string id = csentry.GetAnchorValueOrDefault<string>("id") ?? csentry.DN;

                if (change.ModificationType == AttributeModificationType.Add)
                {
                    if (makeAdmin)
                    {
                        this.config.UsersService.MakeAdmin(true, id);
                    }

                    committedChanges.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("isAdmin", makeAdmin));
                }
                else if (change.ModificationType == AttributeModificationType.Replace ||
                         change.ModificationType == AttributeModificationType.Update)
                {
                    this.config.UsersService.MakeAdmin(makeAdmin, id);

                    if (change.ModificationType == AttributeModificationType.Replace)
                    {
                        committedChanges.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("isAdmin", makeAdmin));
                    }
                    else
                    {
                        committedChanges.AttributeChanges.Add(AttributeChange.CreateAttributeReplace("isAdmin", makeAdmin));
                    }
                }
            }
        }

        public IEnumerable<AttributeChange> GetChanges(string dn, ObjectModificationType modType, SchemaType type, object source)
        {
            foreach (IAttributeAdapter typeDef in ManagementAgent.Schema[SchemaConstants.User].AttributeAdapters.Where(t => t.Api == this.Api))
            {
                foreach (string attributeName in typeDef.MmsAttributeNames)
                {
                    if (type.HasAttribute(attributeName))
                    {
                        foreach (AttributeChange change in typeDef.CreateAttributeChanges(dn, modType, source))
                        {
                            yield return change;
                        }
                    }
                }
            }
        }
    }
}