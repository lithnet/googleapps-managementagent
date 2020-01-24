using System.Collections.Generic;

namespace Lithnet.GoogleApps.MA
{
    using Microsoft.MetadirectoryServices;

    internal interface IApiInterface
    {
        string Api { get; }

        void ApplyChanges(CSEntryChange csentry, CSEntryChange committedChanges, SchemaType type, ref object target, bool patch = false);

        IEnumerable<AttributeChange> GetChanges(string dn, ObjectModificationType modType, SchemaType type, object source);
    }
}
