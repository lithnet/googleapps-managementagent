using System;
using System.Collections.Generic;
using System.Linq;
using Lithnet.GoogleApps.ManagedObjects;
using Microsoft.MetadirectoryServices;
using Microsoft.MetadirectoryServices.DetachedObjectModel;
using Lithnet.MetadirectoryServices;

namespace Lithnet.GoogleApps.MA
{
    internal static class CSEntryChangeFactoryUser
    {
        public static CSEntryChange UserToCSEntryChange(User user, IManagementAgentParameters config, Schema types)
        {
            if (!types.Types.Contains(SchemaConstants.User))
            {
                throw new InvalidOperationException("The type user was not requested");
            }

            SchemaType type = types.Types[SchemaConstants.User];
            CSEntryChange csentry = CSEntryChange.Create();
            csentry.ObjectModificationType = ObjectModificationType.Add;
            csentry.ObjectType = SchemaConstants.User;
            csentry.DN = user.PrimaryEmail;

            //user.ToCSEntryChange(config, type, csentry);

            return csentry;
        }

        public static CSEntryChangeResult PutCSEntryChangeUser(CSEntryChange csentry, IManagementAgentParameters config, SchemaType type)
        {
            CSEntryChangeDetached deltaCSEntry = new CSEntryChangeDetached(Guid.NewGuid(), ObjectModificationType.Unconfigured, MAImportError.Success, null);
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
                        return CSEntryChangeFactoryUser.PutCSEntryChangeUserAdd(csentry, deltaCSEntry, config, type);

                    case ObjectModificationType.Delete:
                        return CSEntryChangeFactoryUser.PutCSEntryChangeUserDelete(csentry, deltaCSEntry, config, type);

                    case ObjectModificationType.Replace:
                        return CSEntryChangeFactoryUser.PutCSEntryChangeUserReplace(csentry, deltaCSEntry, config, type);

                    case ObjectModificationType.Update:
                        return CSEntryChangeFactoryUser.PutCSEntryChangeUserUpdate(csentry, deltaCSEntry, config, type);

                    default:
                    case ObjectModificationType.None:
                    case ObjectModificationType.Unconfigured:
                        throw new InvalidOperationException($"Unknown modification type: {csentry.ObjectModificationType} on object {csentry.DN}");
                }
            }
            finally
            {
                if ((deltaCSEntry.AttributeChanges.Count > 0) || (deltaCSEntry.ObjectModificationType == ObjectModificationType.Delete))
                {
                    CSEntryChangeQueue.Add(deltaCSEntry);
                }
            }
        }

        private static CSEntryChangeResult PutCSEntryChangeUserDelete(CSEntryChange csentry, CSEntryChange deltaCSEntry, IManagementAgentParameters config, SchemaType type)
        {
            deltaCSEntry.ObjectModificationType = csentry.ObjectModificationType;
            UserRequestFactory.Delete(csentry.DN);
            return CSEntryChangeResult.Create(csentry.Identifier, null, MAExportError.Success);
        }

        private static CSEntryChangeResult PutCSEntryChangeUserAdd(CSEntryChange csentry, CSEntryChange deltaCSEntry, IManagementAgentParameters config, SchemaType type)
        {
            deltaCSEntry.ObjectModificationType = csentry.ObjectModificationType;
            User user = new User
            {
                Password = csentry.GetValueAdd<string>("export_password") ?? Guid.NewGuid().ToString("B")
            };

            csentry.ToUser(user, config);
            user = UserRequestFactory.Add(user);
            deltaCSEntry.AnchorAttributes.Add(AnchorAttribute.Create("id", user.Id));

           // user.ToCSEntryChange(config, type, deltaCSEntry);

            Action x = () => CSEntryChangeToUser.ApplyUserAliasChanges(csentry, user, deltaCSEntry);
            x.ExecuteWithRetryOnNotFound();

            csentry.MakeAdmin(deltaCSEntry, user, config);

            List<AttributeChange> anchorChanges = new List<AttributeChange>
            {
                AttributeChange.CreateAttributeAdd("id", user.Id)
            };

            return CSEntryChangeResult.Create(csentry.Identifier, anchorChanges, MAExportError.Success);
        }

        private static CSEntryChangeResult PutCSEntryChangeUserReplace(CSEntryChange csentry, CSEntryChange deltaCSEntry, IManagementAgentParameters config, SchemaType type)
        {
            deltaCSEntry.ObjectModificationType = csentry.ObjectModificationType;

            User user = new User();

            csentry.ToUser(user, config);
            user = UserRequestFactory.Update(user, csentry.DN);

           // user.ToCSEntryChange(config, type, deltaCSEntry);
            
            csentry.MakeAdmin(deltaCSEntry, user, config);
            CSEntryChangeToUser.ApplyUserAliasChanges(csentry, user, deltaCSEntry);

            return CSEntryChangeResult.Create(csentry.Identifier, null, MAExportError.Success);
        }

        private static CSEntryChangeResult PutCSEntryChangeUserUpdate(CSEntryChange csentry, CSEntryChange deltaCSEntry, IManagementAgentParameters config, SchemaType type)
        {
            User user;
            deltaCSEntry.ObjectModificationType = ObjectModificationType.Replace;

            if (CSEntryChangeFactoryUser.CanPatchUser(csentry))
            {
                user = new User();

                if (csentry.ToUser(user, config))
                {
                    user = UserRequestFactory.Patch(user, csentry.DN);
                }
            }
            else
            {
                user = UserRequestFactory.Get(csentry.DN);

                if (csentry.ToUser(user, config))
                {
                    user = UserRequestFactory.Update(user, csentry.DN);
                }
            }

           // user.ToCSEntryChange(config, type, deltaCSEntry);
            csentry.MakeAdmin(deltaCSEntry, user, config);
            CSEntryChangeToUser.ApplyUserAliasChanges(csentry, user, deltaCSEntry);

            return CSEntryChangeResult.Create(csentry.Identifier, null, MAExportError.Success);
        }

        private static bool CanPatchUser(CSEntryChange csentry)
        {
            return false;  // return !csentry.AttributeChanges.Any(t => ManagementAgentSchema.UserAttributesRequiringFullUpdate.Any(u => t.Name.StartsWith(u)));
        }
    }
}
