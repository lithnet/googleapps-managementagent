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
                User u = new User
                {
                    Password = ApiInterfaceUser.GenerateSecureString(60),
                    PrimaryEmail = csentry.DN,
                    CustomSchemas = new Dictionary<string, IDictionary<string, object>>
                    {
                        {
                            SchemaConstants.CustomGoogleAppsSchemaName, new Dictionary<string, object>()
                        }
                    }
                };

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
