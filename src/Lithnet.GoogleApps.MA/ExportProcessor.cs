using System;
using System.Collections.Generic;
using Lithnet.Logging;
using Lithnet.MetadirectoryServices;
using Microsoft.MetadirectoryServices;
using Microsoft.MetadirectoryServices.DetachedObjectModel;

namespace Lithnet.GoogleApps.MA
{
    internal static class ExportProcessor
    {
        public static CSEntryChangeResult PutCSEntryChange(CSEntryChange csentry, SchemaType type, IManagementAgentParameters config)
        {
            try
            {
                return ExportProcessor.PutCSEntryChangeObject(csentry, type, config);
            }
            catch (Google.GoogleApiException ex)
            {
                string errortype = ex.Message;
                string detail = ex.StackTrace;
                Logger.WriteException(ex);

                if (ex.HttpStatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    return CSEntryChangeResult.Create(csentry.Identifier, null, MAExportError.ExportErrorPermissionIssue, errortype, detail);
                }
                else
                {
                    return CSEntryChangeResult.Create(csentry.Identifier, null, MAExportError.ExportErrorCustomContinueRun, errortype, detail);
                }
            }
        }

        public static CSEntryChangeResult PutCSEntryChangeObject(CSEntryChange csentry, SchemaType type, IManagementAgentParameters config)
        {
            MASchemaType maType = ManagementAgent.Schema[type.Name];

            CSEntryChangeDetached deltaCSEntry = new CSEntryChangeDetached(Guid.NewGuid(), ObjectModificationType.Unconfigured, MAImportError.Success, null);

            foreach (var anchorAttributeName in maType.AnchorAttributeNames)
            {
                AnchorAttribute anchor = csentry.GetAnchorAttribute(anchorAttributeName);

                if (anchor != null)
                {
                    deltaCSEntry.AnchorAttributes.Add(anchor);
                }
            }

            deltaCSEntry.ObjectType = csentry.ObjectType;

            try
            {
                switch (csentry.ObjectModificationType)
                {
                    case ObjectModificationType.Add:
                        return ExportProcessor.PutCSEntryChangeAdd(csentry, deltaCSEntry, maType, type, config);

                    case ObjectModificationType.Delete:
                        return ExportProcessor.PutCSEntryChangeDelete(csentry, deltaCSEntry, maType);

                    case ObjectModificationType.Update:
                        return ExportProcessor.PutCSEntryChangeUpdate(csentry, deltaCSEntry, maType, type, config);

                    default:
                    case ObjectModificationType.None:
                    case ObjectModificationType.Replace:
                    case ObjectModificationType.Unconfigured:
                        throw new InvalidOperationException($"Unknown or unsupported modification type: {csentry.ObjectModificationType} on object {csentry.DN}");
                }
            }
            finally
            {
                if (deltaCSEntry.AnchorAttributes.Count > 0 && !string.IsNullOrWhiteSpace(deltaCSEntry.DN))
                {
                    CSEntryChangeQueue.Add(deltaCSEntry);
                }
                else
                {
                    Logger.Write("Dropping delta CSEntryChange as it had no anchor attributes or DN", LogLevel.Debug);
                }
            }
        }

        private static CSEntryChangeResult PutCSEntryChangeDelete(CSEntryChange csentry, CSEntryChange deltaCSEntry, MASchemaType maType)
        {
            deltaCSEntry.ObjectModificationType = csentry.ObjectModificationType;
            deltaCSEntry.DN = csentry.DN;

            maType.ApiInterface.DeleteInstance(csentry);

            return CSEntryChangeResult.Create(csentry.Identifier, null, MAExportError.Success);
        }

        private static CSEntryChangeResult PutCSEntryChangeAdd(CSEntryChange csentry, CSEntryChange deltaCSEntry, MASchemaType maType, SchemaType type, IManagementAgentParameters config)
        {
            deltaCSEntry.ObjectModificationType = csentry.ObjectModificationType;

            IApiInterfaceObject primaryInterface = maType.ApiInterface;

            object instance = primaryInterface.CreateInstance(csentry);

            deltaCSEntry.DN = csentry.DN; // While the export process may change the DN, we should assign the incoming DN for now to the delta object

            // This next line is problematic, because we either get all or nothing. If the group added, but a member failed, we don't see any changes at all. That casues issues when FIM tries to rexport the same values again
            foreach (AttributeChange change in primaryInterface.ApplyChanges(csentry, type, ref instance))
            {
                deltaCSEntry.AttributeChanges.Add(change);
            }

            deltaCSEntry.DN = primaryInterface.GetDNValue(instance);

            List<AttributeChange> anchorChanges = new List<AttributeChange>();

            foreach (string anchorAttributeName in maType.AnchorAttributeNames)
            {
                object value = primaryInterface.GetAnchorValue(anchorAttributeName, instance);

                if (value == null)
                {
                    throw new UnexpectedDataException($"The delta entry could not be created because the anchor attribute '{anchorAttributeName}' was not present on object of type '{type.Name}'. The DN is {deltaCSEntry.DN ?? csentry.DN}");
                }

                deltaCSEntry.AnchorAttributes.Add(AnchorAttribute.Create(anchorAttributeName, value));
                anchorChanges.Add(AttributeChange.CreateAttributeAdd(anchorAttributeName, value));
            }

            return CSEntryChangeResult.Create(csentry.Identifier, anchorChanges, MAExportError.Success);
        }

        private static CSEntryChangeResult PutCSEntryChangeUpdate(CSEntryChange csentry, CSEntryChange deltaCSEntry, MASchemaType maType, SchemaType type, IManagementAgentParameters config)
        {
            deltaCSEntry.DN = csentry.GetNewDNOrDefault<string>() ?? csentry.DN;

            if (csentry.DN != deltaCSEntry.DN)
            {
                Logger.WriteLine($"DN rename {csentry.DN} -> {deltaCSEntry.DN}");
            }

            bool canPatch = maType.CanPatch(csentry.AttributeChanges);

            IApiInterfaceObject primaryInterface = maType.ApiInterface;
            deltaCSEntry.ObjectModificationType = primaryInterface.DeltaUpdateType;

            object instance;

            if (canPatch)
            {
                Logger.WriteLine($"Performing PATCH update operation for {csentry.DN}");
                instance = primaryInterface.CreateInstance(csentry);
            }
            else
            {
                Logger.WriteLine($"Performing FULL update operation for {csentry.DN}");
                instance = primaryInterface.GetInstance(csentry);
            }

            foreach (AttributeChange change in primaryInterface.ApplyChanges(csentry, type, ref instance, canPatch))
            {
                deltaCSEntry.AttributeChanges.Add(change);
            }

            return CSEntryChangeResult.Create(csentry.Identifier, null, MAExportError.Success);
        }
    }
}
