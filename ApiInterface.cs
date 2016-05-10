using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lithnet.GoogleApps.MA
{
    using Microsoft.MetadirectoryServices;

    public abstract class ApiInterface
    {
        public string Api { get; protected set; }

        public abstract bool IsPrimary { get; }

        public abstract object CreateInstance(CSEntryChange csentry);

        public abstract object GetInstance(CSEntryChange csentry);

        public abstract void DeleteInstance(CSEntryChange csentry);

        public abstract IList<AttributeChange> ApplyChanges(CSEntryChange csentry, SchemaType type, object target, bool patch = false);

        public abstract IList<AttributeChange> GetChanges(ObjectModificationType modType, SchemaType type, object source);

        public abstract string GetAnchorValue(object target);

        public abstract string GetDNValue(object target);
    }
}
