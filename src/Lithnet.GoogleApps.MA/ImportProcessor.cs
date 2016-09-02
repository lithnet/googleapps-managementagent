using System;
using Lithnet.MetadirectoryServices;
using Microsoft.MetadirectoryServices;

namespace Lithnet.GoogleApps.MA
{
    internal static class ImportProcessor
    {
        public static CSEntryChange GetCSEntryChange(object source, SchemaType type)
        {
            MASchemaType maType = ManagementAgent.Schema[type.Name];

            CSEntryChange csentry = CSEntryChange.Create();
            csentry.ObjectModificationType = ObjectModificationType.Add;
            csentry.ObjectType = type.Name;
            string anchorValue = maType.ApiInterface.GetAnchorValue(source);

            if (anchorValue == null)
            {
                throw new AttributeNotPresentException(maType.AnchorAttributeName);
            }

            csentry.CreateAttributeAdd(maType.AnchorAttributeName, anchorValue);

            try
            {
                csentry.DN = maType.ApiInterface.GetDNValue(source);

                foreach (AttributeChange change in maType.ApiInterface.GetChanges(csentry.DN, csentry.ObjectModificationType, type, source))
                {
                    csentry.AttributeChanges.Add(change);
                }
            }
            catch (Exception ex)
            {
                csentry.ErrorCodeImport = MAImportError.ImportErrorCustomContinueRun;
                csentry.ErrorDetail = ex.StackTrace;
                csentry.ErrorName = ex.Message;
            }

            return csentry;
        }
    }
}
