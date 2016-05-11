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

    internal class ApiInterfaceAdvancedUser : ApiInterfaceUser
    {
        public ApiInterfaceAdvancedUser()
        {
            this.InternalInterfaces.Add(new ApiInterfaceUserDelegates());
        }
        
        public override object CreateInstance(CSEntryChange csentry)
        {
            if (csentry.ObjectModificationType == ObjectModificationType.Add)
            {
                User u = new User();
                u.Password = Guid.NewGuid().ToString("B");
                u.PrimaryEmail = csentry.DN;
                u.CustomSchemas = new Dictionary<string, IDictionary<string, object>>();
                u.CustomSchemas.Add(SchemaConstants.CustomGoogleAppsSchemaName, new Dictionary<string, object>());
                u.CustomSchemas[SchemaConstants.CustomGoogleAppsSchemaName].Add(SchemaConstants.CustomSchemaObjectType, SchemaConstants.AdvancedUser);
                return u;
            }
            else
            {
                return new User
                {
                    Id = csentry.GetAnchorValueOrDefault<string>(ManagementAgent.Schema[SchemaConstants.User].AnchorAttributeName)
                };
            }
        }
    }
}
