using System;
using System.Diagnostics;
using Lithnet.MetadirectoryServices;
using Microsoft.MetadirectoryServices;

namespace Lithnet.GoogleApps.MA
{
    internal static class ImportProcessor
    {
        public static CSEntryChange GetCSEntryChange(object source, SchemaType type, IManagementAgentParameters config)
        {
            MASchemaType maType = ManagementAgent.Schema[type.Name];

            CSEntryChange csentry = CSEntryChange.Create();
            csentry.ObjectModificationType = ObjectModificationType.Add;
            csentry.ObjectType = type.Name;

            foreach (string anchorAttributeName in maType.AnchorAttributeNames)
            {
                string anchorValue = maType.ApiInterface.GetAnchorValue(anchorAttributeName, source);

                if (anchorValue == null)
                {
                    throw new AttributeNotPresentException(anchorAttributeName);
                }

                csentry.CreateAttributeAdd(anchorAttributeName, anchorValue);
            }

            try
            {
                csentry.DN = maType.ApiInterface.GetDNValue(source);

                foreach (AttributeChange change in maType.ApiInterface.GetChanges(csentry.DN, csentry.ObjectModificationType, type, source))
                {
                    csentry.AttributeChanges.Add(change);
                }

                //Trace.WriteLine($"Created CSEntryChange for {csentry.ObjectType} {csentry.DN}");
            }
            catch (Exception ex)
            {
                Logging.Logger.WriteException(ex);
                csentry.ErrorCodeImport = MAImportError.ImportErrorCustomContinueRun;
                csentry.ErrorDetail = ex.StackTrace;
                csentry.ErrorName = ex.Message;
               // Trace.WriteLine($"Error creating CSEntryChange for {csentry.ObjectType} {csentry.DN} - {ex.Message}");
            }

            return csentry;
        }
    }
}
