using System;
using System.Collections.Generic;
using System.Linq;
using Lithnet.MetadirectoryServices;
using Microsoft.MetadirectoryServices;

namespace Lithnet.GoogleApps.MA
{
    internal class ApiInterfaceUserMakeAdmin : IApiInterface
    {
        public string Api => "usermakeadmin";

        public IList<AttributeChange> ApplyChanges(CSEntryChange csentry, SchemaType type, ref object target, bool patch = false)
        {
            AttributeChange change = csentry.AttributeChanges.FirstOrDefault((t => t.Name == "isAdmin"));
            List<AttributeChange> changes = new List<AttributeChange>();

            if (change != null)
            {
                bool makeAdmin = change.GetValueAdd<bool>();
                string id = csentry.GetAnchorValueOrDefault<string>(ManagementAgent.Schema[SchemaConstants.User].AnchorAttributeName) ?? csentry.DN;

                if (change.ModificationType == AttributeModificationType.Add)
                {
                    if (makeAdmin)
                    {
                        Action x = () => UserRequestFactory.MakeAdmin(true, id);
                        x.ExecuteWithRetryOnNotFound();
                    }

                    changes.Add(AttributeChange.CreateAttributeAdd("isAdmin", makeAdmin));
                }
                else if (change.ModificationType == AttributeModificationType.Replace ||
                         change.ModificationType == AttributeModificationType.Update)
                {
                    Action x = () => UserRequestFactory.MakeAdmin(makeAdmin, id);
                    x.ExecuteWithRetryOnNotFound();

                    if (change.ModificationType == AttributeModificationType.Replace)
                    {
                        changes.Add(AttributeChange.CreateAttributeAdd("isAdmin", makeAdmin));
                    }
                    else
                    {
                        changes.Add(AttributeChange.CreateAttributeReplace("isAdmin", makeAdmin));
                    }
                }
            }

            return changes;
        }

        public IList<AttributeChange> GetChanges(string dn, ObjectModificationType modType, SchemaType type, object source)
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
    }
}