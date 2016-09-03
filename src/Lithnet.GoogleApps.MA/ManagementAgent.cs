using System;
using System.IO;
using Microsoft.MetadirectoryServices;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Lithnet.Logging;
using System.Threading;
using System.Collections.Concurrent;
using System.Diagnostics;
using Lithnet.MetadirectoryServices;
using Lithnet.GoogleApps.ManagedObjects;
using System.Net;

namespace Lithnet.GoogleApps.MA
{
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

        private Task importTask;

        private int opCount;

        private Schema operationSchemaTypes;

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
                Logger.WriteLine("WARNING: HTTPS Debugging enabled. Service certificate validation and GZIP compression are both disabled");
            }
        }

        public void OpenExportConnection(KeyedCollection<string, ConfigParameter> configParameters, Schema types, OpenExportConnectionRunStep exportRunStep)
        {
            this.Configuration = new ManagementAgentParameters(configParameters);
            this.DeltaPath = Path.Combine(MAUtils.MAFolder, ManagementAgent.DeltaFile);

            Logger.LogPath = this.Configuration.MALogFile;
            Logger.WriteLine("Opening export connection");
            this.SetHttpDebugMode();

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
                    Logger.WriteLine("Starting export {0} for {1}", csentry.ObjectModificationType, csentry.DN);
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

            List<Task> tasks = new List<Task>();

            foreach (MASchemaType item in ManagementAgent.Schema)
            {
                Task t = item.ApiInterface.GetItems(this.Configuration, types, this.importCollection);

                if (t != null)
                {
                    tasks.Add(t);
                }
            }

            this.importTask = Task.WhenAll(tasks.ToArray());
            this.importTask.ContinueWith(z => this.importCollection.CompleteAdding());
        }

        //private void ThrowOnFaultedTask()
        //{
        //    if (this.importGroupsTask != null)
        //    {
        //        if (this.importGroupsTask.IsFaulted)
        //        {
        //            throw this.importGroupsTask.Exception;
        //        }
        //    }

        //    if (this.importUsersTask != null)
        //    {
        //        if (this.importUsersTask.IsFaulted)
        //        {
        //            throw this.importUsersTask.Exception;
        //        }
        //    }


        //    if (this.importContactsTask != null)
        //    {
        //        if (this.importContactsTask.IsFaulted)
        //        {
        //            throw this.importContactsTask.Exception;
        //        }
        //    }

        //    if (this.importDomainsTask != null)
        //    {
        //        if (this.importDomainsTask.IsFaulted)
        //        {
        //            throw this.importDomainsTask.Exception;
        //        }
        //    }
        //}

        public GetImportEntriesResults GetImportEntries(GetImportEntriesRunStep importRunStep)
        {
            GetImportEntriesResults results;

            switch (this.importRunStep.ImportType)
            {
                case OperationType.Full:
                    results = this.GetImportEntriesFull();
                    break;

                case OperationType.Delta:
                    results = this.GetImportEntriesDelta();
                    break;

                default:
                    throw new NotSupportedException();
            }

            return results;
        }

        private GetImportEntriesResults GetImportEntriesDelta()
        {
            GetImportEntriesResults results = new GetImportEntriesResults { CSEntries = new List<CSEntryChange>() };

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
            GetImportEntriesResults results = new GetImportEntriesResults { CSEntries = new List<CSEntryChange>() };

            for (int i = 0; i < this.importRunStep.PageSize; i++)
            {
                //this.ThrowOnFaultedTask();
                if (this.importTask.IsFaulted)
                {
                    throw this.importTask.Exception;    
                }

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

                CSEntryChange csentry = item as CSEntryChange;

                if (csentry != null)
                {
                    results.CSEntries.Add(csentry);
                    continue;
                }


                throw new NotSupportedException("The object enumeration returned an unsupported type: " + item.GetType().Name);
            }

            results.MoreToImport = !this.importCollection.IsCompleted;

            if (results.MoreToImport && results.CSEntries.Count == 0)
            {
                Thread.Sleep(1000);
            }

            return results;
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
            return ManagementAgentParameters.GetParameters(configParameters, page);
        }

        public ParameterValidationResult ValidateConfigParameters(KeyedCollection<string, ConfigParameter> configParameters, ConfigParameterPage page)
        {
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
            Logger.LogPath = this.Configuration.PasswordOperationLogFile;
            this.SetHttpDebugMode();

            ConnectionPools.InitializePools(this.Configuration.Credentials, 1, 1, 1, 1);
        }

        public void SetPassword(CSEntry csentry, System.Security.SecureString newPassword, PasswordOptions options)
        {
            try
            {
                UserRequestFactory.SetPassword(csentry.DN.ToString(), newPassword);
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
            throw new EntryPointNotImplementedException();
        }

        public void ClosePasswordConnection()
        {
        }
    }
}