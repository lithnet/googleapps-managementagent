using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lithnet.GoogleApps.MA
{
    using System.Diagnostics.Tracing;
    using ManagedObjects;
    using Microsoft.MetadirectoryServices;

    internal static class ImportProcessor
    {
        public static CSEntryChange GetCSEntryChange(object source, SchemaType type)
        {
            MASchemaType maType = ManagementAgent.Schema[type.Name];

            CSEntryChange csentry = CSEntryChange.Create();
            csentry.ObjectModificationType = ObjectModificationType.Add;
            csentry.ObjectType = type.Name;
            csentry.DN = maType.ApiInterface.GetDNValue(source);

            foreach (AttributeChange change in maType.ApiInterface.GetChanges(csentry.ObjectModificationType, type, source))
            {
                csentry.AttributeChanges.Add(change);
            }

            return csentry;
        }
    }
}
