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
        public static CSEntryChangeResult PutCSEntryChange(CSEntryChange csentry, SchemaType type)
        {
            try
            {
                return ExportProcessor.PutCSEntryChangeObject(csentry, type);
            }
            catch (Google.GoogleApiException ex)
            {
                string errortype = ex.Message;
                string detail = ex.StackTrace;
                Logger.WriteException(ex);

                if (ex.HttpStatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return CSEntryChangeResult.Create(csentry.Identifier, null, MAExportError.ExportErrorConnectedDirectoryMissingObject, errortype, detail);
                }
                else if (ex.HttpStatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    return CSEntryChangeResult.Create(csentry.Identifier, null, MAExportError.ExportErrorPermissionIssue, errortype, detail);
                }
                else
                {
                    return CSEntryChangeResult.Create(csentry.Identifier, null, MAExportError.ExportErrorCustomContinueRun, errortype, detail);
                }
            }
        }

        public static CSEntryChangeResult PutCSEntryChangeObject(CSEntryChange csentry, SchemaType type)
        {
            MASchemaType maType = ManagementAgent.Schema[type.Name];

            CSEntryChangeDetached deltaCSEntry = new CSEntryChangeDetached(Guid.NewGuid(), ObjectModificationType.Unconfigured, MAImportError.Success, null);

            AnchorAttribute anchor = csentry.GetAnchorAttribute(maType.AnchorAttributeName);

            if (anchor != null)
            {
                deltaCSEntry.AnchorAttributes.Add(anchor);
            }

            deltaCSEntry.ObjectType = csentry.ObjectType;

            try
            {
                switch (csentry.ObjectModificationType)
                {
                    case ObjectModificationType.Add:
                        return ExportProcessor.PutCSEntryChangeAdd(csentry, deltaCSEntry, maType, type);

                    case ObjectModificationType.Delete:
                        return ExportProcessor.PutCSEntryChangeDelete(csentry, deltaCSEntry, maType);

                    case ObjectModificationType.Update:
                        return ExportProcessor.PutCSEntryChangeUpdate(csentry, deltaCSEntry, maType, type);

                    default:
                    case ObjectModificationType.None:
                    case ObjectModificationType.Replace:
                    case ObjectModificationType.Unconfigured:
                        throw new InvalidOperationException($"Unknown or unsupported modification type: {csentry.ObjectModificationType} on object {csentry.DN}");
                }
            }
            finally
            {
                if ((deltaCSEntry.AttributeChanges.Count > 0) || (deltaCSEntry.ObjectModificationType == ObjectModificationType.Delete))
                {
                    CSEntryChangeQueue.Add(deltaCSEntry);
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

        private static CSEntryChangeResult PutCSEntryChangeAdd(CSEntryChange csentry, CSEntryChange deltaCSEntry, MASchemaType maType, SchemaType type)
        {
            deltaCSEntry.ObjectModificationType = csentry.ObjectModificationType;
            deltaCSEntry.DN =  csentry.DN;

            IApiInterfaceObject primaryInterface = maType.ApiInterface;

            object instance = primaryInterface.CreateInstance(csentry);

            foreach (AttributeChange change in primaryInterface.ApplyChanges(csentry, type, ref instance))
            {
                deltaCSEntry.AttributeChanges.Add(change);
            }

            deltaCSEntry.AnchorAttributes.Add(AnchorAttribute.Create(maType.AnchorAttributeName, primaryInterface.GetAnchorValue(instance)));

            List<AttributeChange> anchorChanges = new List<AttributeChange>
            {
                AttributeChange.CreateAttributeAdd(maType.AnchorAttributeName, primaryInterface.GetAnchorValue(instance))
            };

            return CSEntryChangeResult.Create(csentry.Identifier, anchorChanges, MAExportError.Success);
        }

        private static CSEntryChangeResult PutCSEntryChangeUpdate(CSEntryChange csentry, CSEntryChange deltaCSEntry, MASchemaType maType, SchemaType type)
        {
            deltaCSEntry.ObjectModificationType = csentry.ObjectModificationType;
            deltaCSEntry.DN = csentry.GetNewDNOrDefault<string>() ?? csentry.DN;

            if (csentry.DN != deltaCSEntry.DN)
            {
                Logger.WriteLine($"DN rename {csentry.DN} -> {deltaCSEntry.DN}");
            }

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

            foreach (AttributeChange change in primaryInterface.ApplyChanges(csentry, type, ref instance, canPatch))
            {
                deltaCSEntry.AttributeChanges.Add(change);
            }

            return CSEntryChangeResult.Create(csentry.Identifier, null, MAExportError.Success);
        }
    }
}
