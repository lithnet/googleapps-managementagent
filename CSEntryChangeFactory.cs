using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.MetadirectoryServices;
using Newtonsoft.Json.Linq;
using Lithnet.Logging;
using G = Google.Apis.Admin.Directory.directory_v1.Data;
using GS = Google.Apis.Groupssettings.v1.Data;
using System.IO;
using Lithnet.MetadirectoryServices;
using System.Linq.Expressions;
using System.Reflection;

namespace Lithnet.GoogleApps.MA
{
    public static class CSEntryChangeFactory
    {
        public static CSEntryChangeResult PutCSEntryChange(CSEntryChange csentry, IManagementAgentParameters config, SchemaType type)
        {
            try
            {
                switch (csentry.ObjectType)
                {
                    case "user":
                        return CSEntryChangeFactoryUser.PutCSEntryChangeUser(csentry, config, type);

                    case "group":
                        return CSEntryChangeFactoryGroup.PutCSEntryChangeGroup(csentry, config, type);

                    default:
                        throw new InvalidOperationException();
                }
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
  
        internal static void SetDeltaDNOnRename(CSEntryChange csentry, CSEntryChange deltaCSEntry)
        {
            switch (csentry.ObjectModificationType)
            {
                case ObjectModificationType.Add:
                case ObjectModificationType.Delete:
                    deltaCSEntry.DN = csentry.DN;
                    break;

                case ObjectModificationType.Replace:
                case ObjectModificationType.Update:
                    string newDN = csentry.GetNewDNOrDefault<string>();
                    string newPrimaryEmail = csentry.GetNewPrimaryEmail();

                    if (newDN != null)
                    {
                        if (newPrimaryEmail == null)
                        {
                            if (!csentry.AttributeChanges.Any(t => t.Name == "primaryEmail"))
                            {
                                if (csentry.ObjectModificationType == ObjectModificationType.Replace)
                                {
                                    csentry.CreateAttributeAdd("primaryEmail", newDN);
                                }
                                else
                                {
                                    csentry.CreateAttributeReplace("primaryEmail", newDN);
                                }
                            }
                        }
                    }

                    deltaCSEntry.DN = newDN ?? newPrimaryEmail ?? csentry.DN;
                    break;

                case ObjectModificationType.Unconfigured:
                case ObjectModificationType.None:
                default:
                    throw new InvalidOperationException("Unsupported or unknown modification type");
            }
        }
    }
}
