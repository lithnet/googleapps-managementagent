using System;
using System.Collections.Generic;
using System.Linq;
using Lithnet.GoogleApps.ManagedObjects;
using Lithnet.Logging;
using Lithnet.MetadirectoryServices;
using Microsoft.MetadirectoryServices;

namespace Lithnet.GoogleApps.MA
{
    internal class ApiInterfaceUserDelegates : IApiInterface
    {
        private IManagementAgentParameters config;

        public string Api => "userdelegates";

        private string attributeName;

        private string typeName;

        public ApiInterfaceUserDelegates(IManagementAgentParameters config, string typeName)
        {
            this.config = config;
            this.typeName = typeName;
            this.attributeName = $"{typeName}_{SchemaConstants.Delegate}";
        }

        public void ApplyChanges(CSEntryChange csentry, CSEntryChange committedChanges, SchemaType type, ref object target, bool patch = false)
        {
            AttributeChange change = this.ApplyDelegateChanges(csentry);

            if (change != null)
            {
                committedChanges.AttributeChanges.Add(change);
            }
        }

        public IEnumerable<AttributeChange> GetChanges(string dn, ObjectModificationType modType, SchemaType type, object source)
        {
            if (!type.HasAttribute(this.attributeName))
            {
                yield break;
            }

            IAttributeAdapter typeDef = ManagementAgent.Schema[this.typeName].AttributeAdapters.First(t => t.Api == this.Api);

            List<string> delegates = this.config.GmailService.GetDelegates(((User)source).PrimaryEmail).ToList();

            foreach(AttributeChange change in typeDef.CreateAttributeChanges(dn, modType, new { Delegates = delegates }))
            {
                yield return change;
            }
        }

        private void GetUserDelegateChanges(CSEntryChange csentry, out IList<string> adds, out IList<string> deletes)
        {
            adds = new List<string>();
            deletes = new List<string>();

            AttributeChange change = csentry.AttributeChanges.FirstOrDefault(t => t.Name == this.attributeName);

            if (csentry.ObjectModificationType == ObjectModificationType.Replace)
            {
                if (change != null)
                {
                    adds = change.GetValueAdds<string>();
                }

                foreach (string @delegate in this.config.GmailService.GetDelegates(csentry.DN).Except(adds))
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
                        foreach (string @delegate in this.config.GmailService.GetDelegates(csentry.DN))
                        {
                            deletes.Add(@delegate);
                        }
                        break;

                    case AttributeModificationType.Replace:
                        adds = change.GetValueAdds<string>();
                        foreach (string @delegate in this.config.GmailService.GetDelegates(csentry.DN).Except(adds))
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

        private AttributeChange ApplyDelegateChanges(CSEntryChange csentry)
        {
            this.GetUserDelegateChanges(csentry, out IList<string> adds, out IList<string> deletes);

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
                        Logger.WriteLine($"Removing delegate {delete}");
                        this.config.GmailService.RemoveDelegate(csentry.DN, delete);
                        valueChanges.Add(ValueChange.CreateValueDelete(delete));
                    }
                }

                foreach (string add in adds)
                {
                    Logger.WriteLine($"Adding delegate {add}");
                    this.config.GmailService.AddDelegate(csentry.DN, add);
                    valueChanges.Add(ValueChange.CreateValueAdd(add));
                }
            }
            finally
            {
                if (valueChanges.Count > 0)
                {
                    if (csentry.ObjectModificationType == ObjectModificationType.Update)
                    {
                        change = AttributeChange.CreateAttributeUpdate(this.attributeName, valueChanges);
                    }
                    else
                    {
                        change = AttributeChange.CreateAttributeAdd(this.attributeName, valueChanges.Where(u => u.ModificationType == ValueModificationType.Add).Select(t => t.Value).ToList());
                    }
                }
            }

            return change;
        }
    }
}
