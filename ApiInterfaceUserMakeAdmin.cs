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

    internal class ApiInterfaceUserMakeAdmin : IApiInterface
    {
        public string Api => "usermakeadmin";

        public IList<AttributeChange> ApplyChanges(CSEntryChange csentry, SchemaType type, object target, bool patch = false)
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

                        changes.Add(AttributeChange.CreateAttributeAdd("isAdmin", true));
                    }
                }
                else if (change.ModificationType == AttributeModificationType.Replace ||
                         change.ModificationType == AttributeModificationType.Update)
                {
                    Action x = () => UserRequestFactory.MakeAdmin(makeAdmin, id);
                    x.ExecuteWithRetryOnNotFound();

                    changes.Add(AttributeChange.CreateAttributeAdd("isAdmin", makeAdmin));
                }
            }

            return changes;
        }

        public IList<AttributeChange> GetChanges(ObjectModificationType modType, SchemaType type, object source)
        {
            List<AttributeChange> attributeChanges = new List<AttributeChange>();

            foreach (IMASchemaAttribute typeDef in ManagementAgent.Schema[SchemaConstants.User].Attributes.Where(t => t.Api == this.Api))
            {
                if (type.HasAttribute(typeDef.AttributeName))
                {
                    attributeChanges.AddRange(typeDef.CreateAttributeChanges(modType, source));
                }
            }

            return attributeChanges;
        }
    }
}