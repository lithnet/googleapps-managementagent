using System;
using System.Collections.Generic;
using Google;
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
            return ExportProcessor.PutCSEntryChangeObject(csentry, type, config);
        }

        public static CSEntryChangeResult PutCSEntryChangeObject(CSEntryChange csentry, SchemaType type, IManagementAgentParameters config)
        {
            MASchemaType maType = ManagementAgent.Schema[type.Name];

            CSEntryChangeDetached deltaCSEntry = new CSEntryChangeDetached(Guid.NewGuid(), ObjectModificationType.Unconfigured, MAImportError.Success, null);
            deltaCSEntry.ObjectType = csentry.ObjectType;

            foreach (var anchorAttributeName in maType.AnchorAttributeNames)
            {
                AnchorAttribute anchor = csentry.GetAnchorAttribute(anchorAttributeName);

                if (anchor != null)
                {
                    deltaCSEntry.AnchorAttributes.Add(anchor);
                }
            }

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
                if (deltaCSEntry.ObjectModificationType != ObjectModificationType.Unconfigured)
                {
                    if (!string.IsNullOrWhiteSpace(deltaCSEntry.DN))
                    {
                        CSEntryChangeQueue.Add(deltaCSEntry);
                    }
                    else
                    {
                        Logger.Write("Dropping delta CSEntryChange as it had no DN", LogLevel.Debug);
                    }
                }
                else
                {
                    Logger.Write("Dropping delta CSEntryChange as it was incomplete", LogLevel.Debug);
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
            MAExportError error = MAExportError.Success;
            List<AttributeChange> anchorChanges = null;
            IApiInterfaceObject primaryInterface = maType.ApiInterface;
            object instance = null;
            string errorName = null;
            string errorDetail = null;

            try
            {
                instance = primaryInterface.CreateInstance(csentry);
                primaryInterface.ApplyChanges(csentry, deltaCSEntry, type, ref instance);
            }
            catch (Exception ex)
            {
                Logger.WriteLine($"An error occurred during the export of object {csentry.DN} failed");
                Logger.WriteException(ex);

                if (ex is GoogleApiException gex && gex.HttpStatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    error = MAExportError.ExportErrorPermissionIssue;
                }
                else
                {
                    error = MAExportError.ExportErrorCustomContinueRun;
                }

                errorName = ex.Message;
                errorDetail = ex.ToString();
            }

            if (instance != null && deltaCSEntry.ObjectModificationType != ObjectModificationType.Unconfigured)
            {
                foreach (string anchorAttributeName in maType.AnchorAttributeNames)
                {
                    object value = primaryInterface.GetAnchorValue(anchorAttributeName, instance);

                    if (value != null)
                    {
                        deltaCSEntry.AnchorAttributes.Add(AnchorAttribute.Create(anchorAttributeName, value));
                        anchorChanges = new List<AttributeChange>();
                        anchorChanges.Add(AttributeChange.CreateAttributeAdd(anchorAttributeName, value));
                    }
                    else
                    {
                        throw new UnexpectedDataException($"The anchor attribute '{anchorAttributeName}' was not present on object of type '{type.Name}'. The DN is {deltaCSEntry.DN ?? csentry.DN}");
                    }
                }
            }

            if (error == MAExportError.Success)
            {
                return CSEntryChangeResult.Create(csentry.Identifier, anchorChanges, error);
            }
            else
            {
                return CSEntryChangeResult.Create(csentry.Identifier, anchorChanges, error, errorName, errorDetail);
            }
        }

        private static CSEntryChangeResult PutCSEntryChangeUpdate(CSEntryChange csentry, CSEntryChange deltaCSEntry, MASchemaType maType, SchemaType type, IManagementAgentParameters config)
        {
            bool canPatch = maType.CanPatch(csentry.AttributeChanges);
            IApiInterfaceObject primaryInterface = maType.ApiInterface;

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

            primaryInterface.ApplyChanges(csentry, deltaCSEntry, type, ref instance, canPatch);

            return CSEntryChangeResult.Create(csentry.Identifier, null, MAExportError.Success);
        }
    }
}
