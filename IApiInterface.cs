using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lithnet.GoogleApps.MA
{
    using Microsoft.MetadirectoryServices;

    internal interface IApiInterface
    {
        string Api { get; }

        IList<AttributeChange> ApplyChanges(CSEntryChange csentry, SchemaType type, object target, bool patch = false);

        IList<AttributeChange> GetChanges(ObjectModificationType modType, SchemaType type, object source);
    }
}
