using System;
using System.Collections.Generic;
using System.Linq;
using Lithnet.GoogleApps.ManagedObjects;
using Microsoft.MetadirectoryServices;
using Microsoft.MetadirectoryServices.DetachedObjectModel;
using Lithnet.MetadirectoryServices;

namespace Lithnet.GoogleApps.MA
{
    public static class CSEntryChangeFactoryUser
    {
        public static CSEntryChange UserToCSEntryChange(User user, IManagementAgentParameters config, Schema types)
        {
            if (!types.Types.Contains("user"))
            {
                throw new InvalidOperationException("The type user was not requested");
            }

            SchemaType type = types.Types["user"];
            CSEntryChange csentry = CSEntryChange.Create();
            csentry.ObjectModificationType = ObjectModificationType.Add;
            csentry.ObjectType = "user";
            csentry.DN = user.PrimaryEmail;

            user.UserAllToCSEntryChange(config, type, csentry);

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
                        return PutCSEntryChangeUserAdd(csentry, deltaCSEntry, config, type);

                    case ObjectModificationType.Delete:
                        return PutCSEntryChangeUserDelete(csentry, deltaCSEntry, config, type);

                    case ObjectModificationType.Replace:
                        return PutCSEntryChangeUserReplace(csentry, deltaCSEntry, config, type);

                    case ObjectModificationType.Update:
                        return PutCSEntryChangeUserUpdate(csentry, deltaCSEntry, config, type);

                    default:
                    case ObjectModificationType.None:
                    case ObjectModificationType.Unconfigured:
                        throw new InvalidOperationException(string.Format("Unknown modification type: {0} on object {1}", csentry.ObjectModificationType, csentry.DN));
                }
            }
            finally
            {
                if (deltaCSEntry.AttributeChanges.Count > 0 || deltaCSEntry.ObjectModificationType == ObjectModificationType.Delete)
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
            User user = new User();
            user.Name = new UserName();
            user.Password = csentry.GetValueAdd<string>("export_password") ?? Guid.NewGuid().ToString("B");
            csentry.CSEntryChangeToUserAll(user, config, type);
            user = UserRequestFactory.Add(user);
            deltaCSEntry.AnchorAttributes.Add(AnchorAttribute.Create("id", user.Id));

            user.UserAllToCSEntryChange(config, type, deltaCSEntry);

            Action x = () => CSEntryChangeToUser.ApplyAliasChanges(csentry, user, deltaCSEntry);
            x.ExecuteWithRetryOnNotFound();

            return CSEntryChangeResult.Create(csentry.Identifier, null, MAExportError.Success);
        }

        private static CSEntryChangeResult PutCSEntryChangeUserReplace(CSEntryChange csentry, CSEntryChange deltaCSEntry, IManagementAgentParameters config, SchemaType type)
        {
            deltaCSEntry.ObjectModificationType = csentry.ObjectModificationType;
            User user = new User();
            user.Name = new UserName();

            csentry.CSEntryChangeToUserAll(user, config, type);
            user = UserRequestFactory.Update(user, csentry.DN);

            user.UserAllToCSEntryChange(config, type, deltaCSEntry);

            return CSEntryChangeResult.Create(csentry.Identifier, null, MAExportError.Success);
        }

        private static CSEntryChangeResult PutCSEntryChangeUserUpdate(CSEntryChange csentry, CSEntryChange deltaCSEntry, IManagementAgentParameters config, SchemaType type)
        {
            User userToUpdate;
            deltaCSEntry.ObjectModificationType = ObjectModificationType.Replace;

            if (CanPatchUser(csentry))
            {
                userToUpdate = new User();

                if (csentry.CSEntryChangeToUserAll(userToUpdate, config, type))
                {
                    userToUpdate = UserRequestFactory.Patch(userToUpdate, csentry.DN);
                }
            }
            else
            {
                userToUpdate = UserRequestFactory.Get(csentry.DN);

                if (csentry.CSEntryChangeToUserAll(userToUpdate, config, type))
                {
                    userToUpdate = UserRequestFactory.Update(userToUpdate, csentry.DN);
                }
            }

            userToUpdate.UserAllToCSEntryChange(config, type, deltaCSEntry);

            return CSEntryChangeResult.Create(csentry.Identifier, null, MAExportError.Success);
        }

        private static bool CanPatchUser(CSEntryChange csentry)
        {
            return !csentry.AttributeChanges.Any(t => ManagementAgentSchema.UserAttributesRequiringFullUpdate.Any(u => t.Name.StartsWith(u)));
        }
    }
}
