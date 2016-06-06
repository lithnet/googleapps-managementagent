using System;
using System.IO;
using System.Linq;
using Microsoft.MetadirectoryServices;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Lithnet.Logging;
using System.Threading;
using System.Collections.Concurrent;
using Lithnet.GoogleApps.ManagedObjects;
using System.Diagnostics;
using Lithnet.MetadirectoryServices;
using Google.GData.Contacts;
using GroupMembership = Lithnet.GoogleApps.ManagedObjects.GroupMembership;

namespace Lithnet.GoogleApps.MA
{
    using System.Net;
    using System.Net.Security;
    using System.Security.Cryptography.X509Certificates;


    public class ManagementAgent :
        IMAExtensible2CallExport,
        IMAExtensible2CallImport,
        IMAExtensible2GetSchema,
        IMAExtensible2GetCapabilities,
        IMAExtensible2GetParameters,
        IMAExtensible2GetParametersEx,
        IMAExtensible2Password
    {
        private const string DeltaFile = "lithnet.googleapps.ma.delta.xml";

        private OpenImportConnectionRunStep importRunStep;

        private OpenExportConnectionRunStep exportRunStep;

        private Stopwatch timer;

        private int opCount;

        private Task importUsersTask;

        private Task importGroupsTask;

        private Task importContactsTask;

        private Schema operationSchemaTypes;

        private bool userImportTaskComplete;

        private bool groupImportTaskComplete;

        private bool contactsImportTaskComplete;

        private BlockingCollection<object> importCollection;

        internal static MASchemaTypes Schema { get; set; }

        public int ExportDefaultPageSize => 100;

        public int ExportMaxPageSize => 9999;

        public int ImportDefaultPageSize => 100;

        public int ImportMaxPageSize => 9999;

        public string DeltaPath { get; set; }

        private HashSet<string> seenDNs;

        public MACapabilities Capabilities
        {
            get
            {
                MACapabilities capabilities = new MACapabilities
                {
                    ConcurrentOperation = true,
                    DeleteAddAsReplace = false,
                    DeltaImport = true,
                    DistinguishedNameStyle = MADistinguishedNameStyle.Generic,
                    ExportPasswordInFirstPass = false,
                    ExportType = MAExportType.AttributeUpdate,
                    FullExport = false,
                    IsDNAsAnchor = false,
                    NoReferenceValuesInFirstExport = false,
                    Normalizations = MANormalizations.None,
                    ObjectConfirmation = MAObjectConfirmation.Normal,
                    ObjectRename = true
                };

                return capabilities;
            }
        }

        internal IManagementAgentParameters Configuration { get; private set; }

        private void SetHttpDebugMode()
        {
            if (ManagementAgentParametersBase.HttpDebugEnabled)
            {
                ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                ConnectionPools.DisableGzip = true;
            }
        }

        public void OpenExportConnection(KeyedCollection<string, ConfigParameter> configParameters, Schema types, OpenExportConnectionRunStep exportRunStep)
        {
            this.Configuration = new ManagementAgentParameters(configParameters);
            this.DeltaPath = Path.Combine(MAUtils.MAFolder, ManagementAgent.DeltaFile);
            this.SetHttpDebugMode();

            Logger.LogPath = this.Configuration.MALogFile;
            Logger.WriteLine("Opening export connection");

            this.timer = new Stopwatch();

            ConnectionPools.InitializePools(this.Configuration.Credentials,
             this.Configuration.ExportThreadCount,
             this.Configuration.ExportThreadCount,
             this.Configuration.EmailSettingsServicePoolSize,
             this.Configuration.ContactsServicePoolSize);

            ManagementAgent.Schema = SchemaBuilder.GetSchema(this.Configuration);
            this.exportRunStep = exportRunStep;
            this.operationSchemaTypes = types;

            CSEntryChangeQueue.LoadQueue(this.DeltaPath);


            GroupMembership.GetInternalDomains(this.Configuration.CustomerID);
            this.timer.Start();

        }

        public PutExportEntriesResults PutExportEntries(IList<CSEntryChange> csentries)
        {
            ParallelOptions po = new ParallelOptions { MaxDegreeOfParallelism = this.Configuration.ExportThreadCount };
            PutExportEntriesResults results = new PutExportEntriesResults();

            Parallel.ForEach(csentries, po, (csentry) =>
            {
                try
                {
                    Interlocked.Increment(ref this.opCount);
                    Logger.StartThreadLog();
                    Logger.WriteSeparatorLine('-');
                    Logger.WriteLine("Starting export {0} for user {1}", csentry.ObjectModificationType, csentry.DN);
                    SchemaType type = this.operationSchemaTypes.Types[csentry.ObjectType];

                    CSEntryChangeResult result = ExportProcessor.PutCSEntryChange(csentry, type);
                    lock (results)
                    {
                        results.CSEntryChangeResults.Add(result);
                    }
                }
                catch (AggregateException ex)
                {
                    Logger.WriteLine("An unexpected error occurred while processing {0}", csentry.DN);
                    Logger.WriteException(ex);
                    CSEntryChangeResult result = CSEntryChangeResult.Create(csentry.Identifier, null, MAExportError.ExportErrorCustomContinueRun, ex.InnerException.Message, ex.InnerException.StackTrace);
                    lock (results)
                    {
                        results.CSEntryChangeResults.Add(result);
                    }
                }
                catch (Exception ex)
                {
                    Logger.WriteLine("An unexpected error occurred while processing {0}", csentry.DN);
                    Logger.WriteException(ex);
                    CSEntryChangeResult result = CSEntryChangeResult.Create(csentry.Identifier, null, MAExportError.ExportErrorCustomContinueRun, ex.Message, ex.StackTrace);
                    lock (results)
                    {
                        results.CSEntryChangeResults.Add(result);
                    }
                }
                finally
                {
                    Logger.WriteSeparatorLine('-');
                    Logger.EndThreadLog();
                }
            });

            return results;
        }

        public void CloseExportConnection(CloseExportConnectionRunStep exportRunStep)
        {
            Logger.WriteLine("Closing export connection: {0}", exportRunStep.Reason);
            this.timer.Stop();

            try
            {
                Logger.WriteLine("Writing {0} delta entries to file", CSEntryChangeQueue.Count);
                CSEntryChangeQueue.SaveQueue(this.DeltaPath, this.operationSchemaTypes);
            }
            catch (Exception ex)
            {
                Logger.WriteLine("An error occurred while saving the delta file");
                Logger.WriteException(ex);
                throw;
            }

            Logger.WriteSeparatorLine('*');
            Logger.WriteLine("Operation statistics");
            Logger.WriteLine("Export objects: {0}", this.opCount);
            Logger.WriteLine("Operation time: {0}", this.timer.Elapsed);
            Logger.WriteLine("Ops/sec: {0:N3}", this.opCount / this.timer.Elapsed.TotalSeconds);
            Logger.WriteSeparatorLine('*');

        }

        public OpenImportConnectionResults OpenImportConnection(KeyedCollection<string, ConfigParameter> configParameters, Schema types, OpenImportConnectionRunStep importRunStep)
        {
            this.Configuration = new ManagementAgentParameters(configParameters);
            Logger.LogPath = this.Configuration.MALogFile;

            this.importRunStep = importRunStep;
            this.operationSchemaTypes = types;
            this.timer = new Stopwatch();
            this.seenDNs = new HashSet<string>();

            this.DeltaPath = Path.Combine(MAUtils.MAFolder, ManagementAgent.DeltaFile);

            Logger.WriteLine("Opening import connection. Page size {0}", this.importRunStep.PageSize);

            if (this.importRunStep.ImportType == OperationType.Delta)
            {
                CSEntryChangeQueue.LoadQueue(this.DeltaPath);
                Logger.WriteLine("Delta full import from file started. {0} entries to import", CSEntryChangeQueue.Count);
            }
            else
            {
                this.OpenImportConnectionFull(types);

                Logger.WriteLine("Background full import from Google started");
            }

            this.timer.Start();
            return new OpenImportConnectionResults("<placeholder>");
        }

        private void OpenImportConnectionFull(Schema types)
        {
            this.importCollection = new BlockingCollection<object>();
            this.SetHttpDebugMode();

            ConnectionPools.InitializePools(this.Configuration.Credentials,
                this.Configuration.GroupMembersImportThreadCount + 1,
                this.Configuration.GroupSettingsImportThreadCount,
                this.Configuration.EmailSettingsServicePoolSize,
                this.Configuration.ContactsServicePoolSize);

            GroupMembership.GetInternalDomains(this.Configuration.CustomerID);
            ManagementAgent.Schema = SchemaBuilder.GetSchema(this.Configuration);

            if (this.operationSchemaTypes.Types.Contains(SchemaConstants.User) || this.operationSchemaTypes.Types.Contains(SchemaConstants.AdvancedUser))
            {
                this.SetupUserImportTask(types);
                Logger.WriteLine("User import task setup complete");
            }
            else
            {
                this.userImportTaskComplete = true;
            }

            if (this.operationSchemaTypes.Types.Contains("group"))
            {
                this.SetupGroupsImportTask();
                Logger.WriteLine("Group import task setup complete");
            }
            else
            {
                this.groupImportTaskComplete = true;
            }

            if (this.operationSchemaTypes.Types.Contains(SchemaConstants.Contact))
            {
                this.SetupContactsImportTask();
                Logger.WriteLine("Contacts import task setup complete");
            }
            else
            {
                this.contactsImportTaskComplete = true;
            }

            if (this.importUsersTask != null)
            {
                this.importUsersTask.Start();
            }

            if (this.importGroupsTask != null)
            {
                this.importGroupsTask.Start();
            }

            if (this.importContactsTask != null)
            {
                this.importContactsTask.Start();
            }
        }

        private void ThrowOnFaultedTask()
        {
            if (this.importGroupsTask != null)
            {
                if (this.importGroupsTask.IsFaulted)
                {
                    throw this.importGroupsTask.Exception;
                }
            }

            if (this.importUsersTask != null)
            {
                if (this.importUsersTask.IsFaulted)
                {
                    throw this.importUsersTask.Exception;
                }
            }


            if (this.importContactsTask != null)
            {
                if (this.importContactsTask.IsFaulted)
                {
                    throw this.importContactsTask.Exception;
                }
            }
        }

        private void SetupGroupsImportTask()
        {

            HashSet<string> groupFieldList = new HashSet<string>
            {
                SchemaConstants.Email,
                SchemaConstants.ID
            };

            foreach (string fieldName in ManagementAgent.Schema[SchemaConstants.Group].GetFieldNames(this.operationSchemaTypes.Types[SchemaConstants.Group], "group"))
            {
                groupFieldList.Add(fieldName);
            }

            foreach (string fieldName in ManagementAgent.Schema[SchemaConstants.Group].GetFieldNames(this.operationSchemaTypes.Types[SchemaConstants.Group], "groupaliases"))
            {
                groupFieldList.Add(fieldName);
            }
            
            string groupFields = string.Format("groups({0}), nextPageToken", string.Join(",", groupFieldList));

            HashSet<string> groupSettingList = new HashSet<string>();

            foreach (string fieldName in ManagementAgent.Schema[SchemaConstants.Group].GetFieldNames(this.operationSchemaTypes.Types[SchemaConstants.Group], "groupsettings"))
            {
                groupSettingList.Add(fieldName);
            }

            bool settingsRequired = groupSettingList.Count > 0;

            string groupSettingsFields = string.Join(",", groupSettingList);

            bool membersRequired = ManagementAgent.Schema[SchemaConstants.Group].Attributes.Any(t => t.Api == "groupmembership" && this.operationSchemaTypes.Types[SchemaConstants.Group].Attributes.Contains(t.AttributeName));

            GroupRequestFactory.MemberThreads = this.Configuration.GroupMembersImportThreadCount;
            GroupRequestFactory.SettingsThreads = this.Configuration.GroupSettingsImportThreadCount;

            this.importGroupsTask = new Task(() =>
            {
                Logger.WriteLine("Starting group import task");
                Logger.WriteLine("Requesting group fields: " + groupFields);
                Logger.WriteLine("Requesting group settings fields: " + groupSettingsFields);

                Logger.WriteLine("Requesting settings: " + settingsRequired.ToString());
                Logger.WriteLine("Requesting members: " + membersRequired.ToString());

                GroupRequestFactory.ImportGroups(this.Configuration.CustomerID, membersRequired, settingsRequired, groupFields, groupSettingsFields, this.importCollection);

                Logger.WriteLine("Groups import task complete");

                this.groupImportTaskComplete = true;

                lock (this.importCollection)
                {
                    if (this.userImportTaskComplete && this.groupImportTaskComplete && this.contactsImportTaskComplete)
                    {
                        this.importCollection.CompleteAdding();
                    }
                }
            });
        }

        private void SetupContactsImportTask()
        {
            this.importContactsTask = new Task(() =>
            {
                Logger.WriteLine("Starting contacts import task");

                ContactRequestFactory.GetContacts(this.Configuration.Domain, this.importCollection);

                Logger.WriteLine("Contacts import task complete");

                this.contactsImportTaskComplete = true;

                lock (this.importCollection)
                {
                    if (this.userImportTaskComplete && this.groupImportTaskComplete && this.contactsImportTaskComplete)
                    {
                        this.importCollection.CompleteAdding();
                    }
                }
            });
        }

        private void SetupUserImportTask(Schema types)
        {
            HashSet<string> fieldNames = new HashSet<string>();

            fieldNames.Add(SchemaConstants.PrimaryEmail);
            fieldNames.Add(SchemaConstants.ID);

            if (types.Types.Contains(SchemaConstants.User) || types.Types.Contains(SchemaConstants.AdvancedUser))
            {
                foreach (string field in ManagementAgent.Schema[SchemaConstants.User].GetFieldNames(types.Types[SchemaConstants.User]))
                {
                    fieldNames.Add(field);
                }
            }

            if (this.operationSchemaTypes.Types.Contains(SchemaConstants.AdvancedUser))
            {
                fieldNames.Add($"customSchemas/{SchemaConstants.CustomGoogleAppsSchemaName}");
            }

            string fields = $"users({string.Join(",", fieldNames)}),nextPageToken";

            this.importUsersTask = new Task(() =>
            {
                Logger.WriteLine("Starting user import task");
                Logger.WriteLine("Requesting fields: " + fields);
                UserRequestFactory.StartImport(this.Configuration.CustomerID, fields, this.importCollection);
                Logger.WriteLine("User import task complete");

                this.userImportTaskComplete = true;

                lock (this.importCollection)
                {
                    if (this.userImportTaskComplete && this.groupImportTaskComplete && this.contactsImportTaskComplete)
                    {
                        this.importCollection.CompleteAdding();
                    }
                }
            });
        }

        public GetImportEntriesResults GetImportEntries(GetImportEntriesRunStep importRunStep)
        {
            GetImportEntriesResults results;

            if (this.importRunStep.ImportType == OperationType.Full)
            {
                results = this.GetImportEntriesFull();
            }
            else if (this.importRunStep.ImportType == OperationType.Delta)
            {
                results = this.GetImportEntriesDelta();
            }
            else
            {
                throw new NotSupportedException();
            }

            return results;
        }

        private GetImportEntriesResults GetImportEntriesDelta()
        {
            GetImportEntriesResults results = new GetImportEntriesResults();
            results.CSEntries = new List<CSEntryChange>();

            int count = 0;

            while (CSEntryChangeQueue.Count > 0 && (count < this.importRunStep.PageSize))
            {
                Interlocked.Increment(ref this.opCount);
                results.CSEntries.Add(CSEntryChangeQueue.Take());
                count++;
            }

            results.MoreToImport = CSEntryChangeQueue.Count > 0;

            return results;
        }

        private GetImportEntriesResults GetImportEntriesFull()
        {
            GetImportEntriesResults results = new GetImportEntriesResults();
            results.CSEntries = new List<CSEntryChange>();
            //Logger.WriteLine("Import batch starting for {0} objects", this.importRunStep.PageSize);

            for (int i = 0; i < this.importRunStep.PageSize; i++)
            {
                this.ThrowOnFaultedTask();

                if (this.importCollection.IsCompleted)
                {
                    break;
                }

                object item;

                if (!this.importCollection.TryTake(out item))
                {
                    Thread.Sleep(25);
                    continue;
                }

                Interlocked.Increment(ref this.opCount);

                User user = item as User;

                if (user != null)
                {
                    if (!string.IsNullOrWhiteSpace(this.Configuration.UserRegexFilter))
                    {
                        if (!Regex.IsMatch(user.PrimaryEmail, this.Configuration.UserRegexFilter, RegexOptions.IgnoreCase))
                        {
                            i--;
                            continue;
                        }
                    }

                    SchemaType type = this.operationSchemaTypes.Types[SchemaConstants.User];

                    if (user.CustomSchemas != null)
                    {
                        if (user.CustomSchemas.ContainsKey(SchemaConstants.CustomGoogleAppsSchemaName))
                        {
                            if (user.CustomSchemas[SchemaConstants.CustomGoogleAppsSchemaName].ContainsKey(SchemaConstants.CustomSchemaObjectType))
                            {
                                string objectType = (string)user.CustomSchemas[SchemaConstants.CustomGoogleAppsSchemaName][SchemaConstants.CustomSchemaObjectType];
                                if (this.operationSchemaTypes.Types.Contains(objectType))
                                {
                                    type = this.operationSchemaTypes.Types[objectType];
                                }
                            }
                        }
                    }

                    results.CSEntries.Add(ImportProcessor.GetCSEntryChange(user, type));
                    continue;
                }

                GoogleGroup group = item as GoogleGroup;

                if (group != null)
                {
                    if (!string.IsNullOrWhiteSpace(this.Configuration.GroupRegexFilter))
                    {
                        if (!Regex.IsMatch(group.Group.Email, this.Configuration.GroupRegexFilter, RegexOptions.IgnoreCase))
                        {
                            i--;
                            continue;
                        }
                    }

                    if (this.Configuration.ExcludeUserCreated)
                    {
                        if (!group.Group.AdminCreated.HasValue || !group.Group.AdminCreated.Value)
                        {
                            i--;
                            continue;
                        }
                    }

                    results.CSEntries.Add(this.GetCSEntryForGroup(group));
                    continue;
                }

                ContactEntry contact = item as ContactEntry;

                if (contact != null)
                {
                    if (!string.IsNullOrWhiteSpace(this.Configuration.ContactRegexFilter))
                    {
                        if (contact.PrimaryEmail != null)
                        {
                            if (!Regex.IsMatch(contact.PrimaryEmail.Address, this.Configuration.ContactRegexFilter, RegexOptions.IgnoreCase))
                            {
                                i--;
                                continue;
                            }
                        }
                    }

                    string dn = Schema[SchemaConstants.Contact].ApiInterface.GetDNValue(contact);

                    if (dn == null)
                    {
                        Logger.WriteLine($"Contact {contact.SelfUri.Content} had no DN or primary email attribute, ignoring");
                        continue;
                    }

                    if (!this.seenDNs.Add(dn))
                    {
                        Logger.WriteLine($"Ignoring contact {contact.SelfUri.Content} with duplicate dn {dn}");
                        continue;
                    }

                    results.CSEntries.Add(ImportProcessor.GetCSEntryChange(contact, this.operationSchemaTypes.Types[SchemaConstants.Contact]));
                    continue;

                }

                throw new NotSupportedException("The object enumeration returned an unsupported type: " + item.GetType().Name);
            }

            results.MoreToImport = !this.importCollection.IsCompleted;

            if (results.MoreToImport && results.CSEntries.Count == 0)
            {
                Thread.Sleep(1000);
            }

            Logger.WriteLine("Import page complete. Returning {0} objects to sync engine", LogLevel.Debug, results.CSEntries.Count);
            return results;
        }

        private CSEntryChange GetCSEntryForGroup(GoogleGroup group)
        {
            CSEntryChange csentry;

            if (group.Errors.Count > 0)
            {
                csentry = CSEntryChange.Create();
                csentry.ObjectType = "group";
                csentry.ObjectModificationType = ObjectModificationType.Add;
                csentry.DN = group.Group.Email;
                csentry.ErrorCodeImport = MAImportError.ImportErrorCustomContinueRun;
                csentry.ErrorDetail = group.Errors.FirstOrDefault()?.StackTrace;
                csentry.ErrorName = group.Errors.FirstOrDefault()?.Message;
            }
            else
            {
                csentry = ImportProcessor.GetCSEntryChange(group, this.operationSchemaTypes.Types[SchemaConstants.Group]);
            }
            return csentry;
        }

        public CloseImportConnectionResults CloseImportConnection(CloseImportConnectionRunStep importRunStep)
        {
            Logger.WriteLine("Closing import connection: {0}", importRunStep.Reason);
            try
            {
                if (this.importRunStep.ImportType == OperationType.Full)
                {
                    CSEntryChangeQueue.Clear();
                    CSEntryChangeQueue.SaveQueue(this.DeltaPath, this.operationSchemaTypes);
                    Logger.WriteLine("Cleared delta file");

                }
                else
                {
                    Logger.WriteLine("Writing {0} delta entries to file", CSEntryChangeQueue.Count);
                    CSEntryChangeQueue.SaveQueue(this.DeltaPath, this.operationSchemaTypes);
                }
            }
            catch (Exception ex)
            {
                Logger.WriteLine("An unexpected error occured");
                Logger.WriteException(ex);
                throw;
            }
            Logger.WriteSeparatorLine('*');
            Logger.WriteLine("Operation statistics");
            Logger.WriteLine("Import objects: {0}", this.opCount);
            Logger.WriteLine("Operation time: {0}", this.timer.Elapsed);
            Logger.WriteLine("Ops/sec: {0:N3}", this.opCount / this.timer.Elapsed.TotalSeconds);
            Logger.WriteSeparatorLine('*');

            return new CloseImportConnectionResults(null);
        }

        public Schema GetSchema(KeyedCollection<string, ConfigParameter> configParameters)
        {
            this.Configuration = new ManagementAgentParameters(configParameters);

            ConnectionPools.InitializePools(this.Configuration.Credentials, 1, 1, 1, 1);

            return SchemaBuilder.GetSchema(this.Configuration).GetSchema();
        }

        public IList<ConfigParameterDefinition> GetConfigParametersEx(KeyedCollection<string, ConfigParameter> configParameters, ConfigParameterPage page, int pageNumber)
        {
            if (pageNumber == 1)
            {
                return ManagementAgentParameters.GetParameters(configParameters, page);
            }
            else
            {
                return new List<ConfigParameterDefinition>();
            }
        }

        public ParameterValidationResult ValidateConfigParametersEx(KeyedCollection<string, ConfigParameter> configParameters, ConfigParameterPage page, int pageNumber)
        {
            if (pageNumber == 1)
            {
                ManagementAgentParameters parameters = new ManagementAgentParameters(configParameters);
                return parameters.ValidateParameters(page);
            }
            else
            {
                return new ParameterValidationResult(ParameterValidationResultCode.Success, null, null);
            }
        }

        public IList<ConfigParameterDefinition> GetConfigParameters(KeyedCollection<string, ConfigParameter> configParameters, ConfigParameterPage page)
        {
            //if (pageNumber > 1)
            //{
            //    return null;
            //}

            return ManagementAgentParameters.GetParameters(configParameters, page);
        }

        public ParameterValidationResult ValidateConfigParameters(KeyedCollection<string, ConfigParameter> configParameters, ConfigParameterPage page)
        {
            //if (pageNumber > 1)
            //{
            //    return new ParameterValidationResult(ParameterValidationResultCode.Success, null, null);
            //}

            ManagementAgentParameters parameters = new ManagementAgentParameters(configParameters);
            return parameters.ValidateParameters(page);
        }

        public ConnectionSecurityLevel GetConnectionSecurityLevel()
        {
            return ConnectionSecurityLevel.Secure;
        }

        public void OpenPasswordConnection(KeyedCollection<string, ConfigParameter> configParameters, Partition partition)
        {
            this.Configuration = new ManagementAgentParameters(configParameters);
            this.SetHttpDebugMode();
            Logger.LogPath = this.Configuration.PasswordOperationLogFile;

            ConnectionPools.InitializePools(this.Configuration.Credentials, 1, 1, 1, 1);
        }

        public void SetPassword(CSEntry csentry, System.Security.SecureString newPassword, PasswordOptions options)
        {
            try
            {
                UserRequestFactory.SetPassword(csentry.DN.ToString(), newPassword.ConvertToUnsecureString());
                Logger.WriteLine("Set password for {0}", csentry.DN.ToString());
            }
            catch (Exception ex)
            {
                Logger.WriteLine("Error setting password for {0}", csentry.DN.ToString());
                Logger.WriteException(ex);
                throw;
            }
        }

        public void ChangePassword(CSEntry csentry, System.Security.SecureString oldPassword, System.Security.SecureString newPassword)
        {
            try
            {
                UserRequestFactory.SetPassword(csentry.DN.ToString(), newPassword.ConvertToUnsecureString());
                Logger.WriteLine("Changed password for {0}", csentry.DN.ToString());
            }
            catch (Exception ex)
            {
                Logger.WriteLine("Error changing password for {0}", csentry.DN.ToString());
                Logger.WriteException(ex);
                throw;
            }
        }

        public void ClosePasswordConnection()
        {
        }
    }
}