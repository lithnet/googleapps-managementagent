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

    internal class ApiInterfaceUserDelegates : IApiInterface
    {
        public string Api => "userdelegates";

        public IList<AttributeChange> ApplyChanges(CSEntryChange csentry, SchemaType type, ref object target, bool patch = false)
        {
            Func<AttributeChange> x = () => ApiInterfaceUserDelegates.ApplyDelegateChanges(csentry);
            AttributeChange change = x.ExecuteWithRetryOnNotFound();

            List<AttributeChange> changes = new List<AttributeChange>();

            if (change != null)
            {
                changes.Add(change);
            }

            return changes;
        }

        public IList<AttributeChange> GetChanges(string dn, ObjectModificationType modType, SchemaType type, object source)
        {
            List<AttributeChange> attributeChanges = new List<AttributeChange>();

            foreach (IAttributeAdapter typeDef in ManagementAgent.Schema[SchemaConstants.AdvancedUser].Attributes.Where(t => t.Api == this.Api && t.AttributeName == SchemaConstants.Delegate))
            {
                if (!type.HasAttribute(typeDef.AttributeName))
                {
                    continue;
                }

                List<string> delegates = UserSettingsRequestFactory.GetDelegates(((User)source).PrimaryEmail).ToList();
                attributeChanges.AddRange(typeDef.CreateAttributeChanges(dn, modType, new { Delegates = delegates }));
                break;
            }

            return attributeChanges;
        }

        private static void GetUserDelegateChanges(CSEntryChange csentry, out IList<string> adds, out IList<string> deletes)
        {
            adds = new List<string>();
            deletes = new List<string>();

            AttributeChange change = csentry.AttributeChanges.FirstOrDefault(t => t.Name == SchemaConstants.Delegate);

            if (csentry.ObjectModificationType == ObjectModificationType.Replace)
            {
                if (change != null)
                {
                    adds = change.GetValueAdds<string>();
                }

                foreach (string @delegate in UserSettingsRequestFactory.GetDelegates(csentry.DN).Except(adds))
                {
                    deletes.Add(@delegate);
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
                        adds = change.GetValueAdds<string>();
                        break;

                    case AttributeModificationType.Delete:
                        foreach (string @delegate in UserSettingsRequestFactory.GetDelegates(csentry.DN))
                        {
                            deletes.Add(@delegate);
                        }
                        break;

                    case AttributeModificationType.Replace:
                        adds = change.GetValueAdds<string>();
                        foreach (string @delegate in UserSettingsRequestFactory.GetDelegates(csentry.DN).Except(adds))
                        {
                            deletes.Add(@delegate);
                        }
                        break;

                    case AttributeModificationType.Update:
                        adds = change.GetValueAdds<string>();
                        deletes = change.GetValueDeletes<string>();
                        break;

                    case AttributeModificationType.Unconfigured:
                    default:
                        throw new InvalidOperationException("Unknown or unsupported modification type");
                }
            }
        }

        private static AttributeChange ApplyDelegateChanges(CSEntryChange csentry)
        {
            IList<string> adds;
            IList<string> deletes;

            ApiInterfaceUserDelegates.GetUserDelegateChanges(csentry, out adds, out deletes);

            if (adds.Count == 0 && deletes.Count == 0)
            {
                return null;
            }

            AttributeChange change = null;
            IList<ValueChange> valueChanges = new List<ValueChange>();

            try
            {
                if (deletes != null)
                {
                    foreach (string delete in deletes)
                    {
                        UserSettingsRequestFactory.RemoveDelegate(csentry.DN, delete);
                        valueChanges.Add(ValueChange.CreateValueDelete(delete));
                    }
                }

                foreach (string add in adds)
                {
                    UserSettingsRequestFactory.AddDelegate(csentry.DN, add);
                    valueChanges.Add(ValueChange.CreateValueAdd(add));
                }
            }
            finally
            {
                if (valueChanges.Count > 0)
                {
                    if (csentry.ObjectModificationType == ObjectModificationType.Update)
                    {
                        change = AttributeChange.CreateAttributeUpdate("delegates", valueChanges);
                    }
                    else
                    {
                        change = AttributeChange.CreateAttributeAdd("delegates", valueChanges.Where(u => u.ModificationType == ValueModificationType.Add).Select(t => t.Value).ToList());
                    }
                }
            }

            return change;
        }
    }
}
