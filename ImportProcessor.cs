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
        public static CSEntryChange GetCSEntryChange(object source, Schema types)
        {
            User user = source as User;

            if (user != null)
            {
                if (types.Types.Contains(SchemaConstants.User))
                {
                    return ImportProcessor.GetCSEntryChange(user, types.Types[SchemaConstants.User]);
                }
            }

            GoogleGroup group = source as GoogleGroup;

            if (group != null)
            {
                if (types.Types.Contains(SchemaConstants.Group))
                {
                    return ImportProcessor.GetCSEntryChange(group, types.Types[SchemaConstants.Group]);
                }
            }

            throw new InvalidOperationException();
        }

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
