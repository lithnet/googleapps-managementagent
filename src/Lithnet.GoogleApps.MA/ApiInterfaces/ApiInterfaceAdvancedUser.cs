using System.Collections.Generic;
using Lithnet.GoogleApps.ManagedObjects;
using Lithnet.MetadirectoryServices;
using Microsoft.MetadirectoryServices;

namespace Lithnet.GoogleApps.MA
{
    internal class ApiInterfaceAdvancedUser : ApiInterfaceUser
    {
        public ApiInterfaceAdvancedUser(MASchemaType type)
            :base (type)
        {
            this.InternalInterfaces.Add(new ApiInterfaceUserDelegates());
            this.ObjectClass = SchemaConstants.AdvancedUser;
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
                    Id = csentry.GetAnchorValueOrDefault<string>("id")
                };
            }
        }
    }
}
