using System.Collections.Generic;

namespace Lithnet.GoogleApps.MA
{
    using Microsoft.MetadirectoryServices;

    internal interface IApiInterface
    {
        string Api { get; }

        IList<AttributeChange> ApplyChanges(CSEntryChange csentry, SchemaType type, IManagementAgentParameters config, ref object target, bool patch = false);

        IList<AttributeChange> GetChanges(string dn, ObjectModificationType modType, SchemaType type, object source);
    }
}
