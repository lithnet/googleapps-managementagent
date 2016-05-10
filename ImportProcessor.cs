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

    public static class ImportProcessor
    {
        public static CSEntryChange GetCSEntryChange(object source, IManagementAgentParameters config, Schema types)
        {
            User user = source as User;

            if (user != null)
            {
                if (types.Types.Contains(SchemaConstants.User))
                {
                    return ImportProcessor.GetCSEntryChange(user, config, types.Types[SchemaConstants.User]);
                }
            }

            GoogleGroup group = source as GoogleGroup;

            if (group != null)
            {
                if (types.Types.Contains(SchemaConstants.Group))
                {
                    return ImportProcessor.GetCSEntryChange(group, config, types.Types[SchemaConstants.Group]);
                }
            }

            throw new InvalidOperationException();
        }

        public static CSEntryChange GetCSEntryChange(object source, IManagementAgentParameters config, SchemaType type)
        {
            MASchemaType maType = SchemaBuilder.GetSchema(type.Name);

            CSEntryChange csentry = CSEntryChange.Create();
            csentry.ObjectModificationType = ObjectModificationType.Add;
            csentry.ObjectType = type.Name;
            csentry.DN = maType.ApiInterface.GetDNValue(source);

            foreach (IGrouping<string, IMASchemaAttribute> group in maType.Attributes.GroupBy(t => t.Api))
            {
                foreach (AttributeChange item in SchemaBuilder.ApiInterfaces[group.Key].ApplyChanges(csentry, type, source))
                {
                    csentry.AttributeChanges.Add(item);
                }
            }

            return csentry;
        }
    }
}
