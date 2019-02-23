using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Lithnet.GoogleApps.ManagedObjects;
using Lithnet.Logging;
using Lithnet.MetadirectoryServices;
using Microsoft.MetadirectoryServices;

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

        private Stopwatch timer;

        private Task importTask;

        private int opCount;

        private Schema operationSchemaTypes;

        private BlockingCollection<object> importCollection;

        private CancellationTokenSource cancellationToken;

        internal static MASchemaTypes Schema { get; set; }

        public int ExportDefaultPageSize => 100;

        public int ExportMaxPageSize => 9999;

        public int ImportDefaultPageSize => 100;

        public int ImportMaxPageSize => 9999;

        public string DeltaPath { get; set; }

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
            if (MAConfigurationSection.Configuration.HttpDebugEnabled)
            {
                ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                Lithnet.GoogleApps.Settings.DisableGzip = true;
                Logger.WriteLine("WARNING: HTTPS Debugging enabled. Service certificate validation and GZIP compression are both disabled");
            }
        }

        public void OpenExportConnection(KeyedCollection<string, ConfigParameter> configParameters, Schema types, OpenExportConnectionRunStep exportRunStep)
        {
            this.cancellationToken = new CancellationTokenSource();
            this.Configuration = new ManagementAgentParameters(configParameters);
            this.DeltaPath = Path.Combine(MAUtils.MAFolder, ManagementAgent.DeltaFile);

            Logger.LogPath = this.Configuration.MALogFile;
            Logger.WriteLine("Opening export connection");
            this.SetHttpDebugMode();

            this.timer = new Stopwatch();

            ManagementAgent.Schema = SchemaBuilder.GetSchema(this.Configuration);
            this.operationSchemaTypes = types;

            CSEntryChangeQueue.LoadQueue(this.DeltaPath);

            this.LoadInternalDomainsIfRequired(types);
            this.timer.Start();
        }

        public PutExportEntriesResults PutExportEntries(IList<CSEntryChange> csentries)
        {
            ParallelOptions po = new ParallelOptions
            {
                MaxDegreeOfParallelism = MAConfigurationSection.Configuration.ExportThreads,
                CancellationToken = this.cancellationToken.Token
            };

            PutExportEntriesResults results = new PutExportEntriesResults();

            try
            {
                Parallel.ForEach(csentries, po, (csentry) =>
                {
                    try
                    {
                        Interlocked.Increment(ref this.opCount);
                        Logger.StartThreadLog();
                        Logger.WriteSeparatorLine('-');
                        Logger.WriteLine("Starting export {0} for {1} with {2} attribute changes", csentry.ObjectModificationType, csentry.DN, csentry.AttributeChanges.Count);
                        SchemaType type = this.operationSchemaTypes.Types[csentry.ObjectType];

                        CSEntryChangeResult result = ExportProcessor.PutCSEntryChange(csentry, type, this.Configuration);
                        lock (results)
                        {
                            results.CSEntryChangeResults.Add(result);
                        }
                    }
                    catch (AggregateException ex)
                    {
                        Logger.WriteLine("An unexpected error occurred while processing {0}", csentry.DN);
                        Logger.WriteException(ex);
                        CSEntryChangeResult result = CSEntryChangeResult.Create(csentry.Identifier, null, MAExportError.ExportErrorCustomContinueRun, ex.InnerException?.Message ?? ex.Message, ex.InnerException?.StackTrace ?? ex.StackTrace);
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
            }
            catch (OperationCanceledException)
            {
            }

            return results;
        }

        public void CloseExportConnection(CloseExportConnectionRunStep exportRunStep)
        {
            Logger.WriteLine("Closing export connection: {0}", exportRunStep.Reason);
            this.timer.Stop();

            if (exportRunStep.Reason != CloseReason.Normal)
            {
                this.cancellationToken?.Cancel();
            }

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
            try
            {
                this.cancellationToken = new CancellationTokenSource();
                this.Configuration = new ManagementAgentParameters(configParameters);
                Logger.LogPath = this.Configuration.MALogFile;

                this.importRunStep = importRunStep;
                this.operationSchemaTypes = types;
                this.timer = new Stopwatch();

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
            catch (Exception ex)
            {
                Logger.WriteException(ex);
                throw;
            }
        }

        private void LoadInternalDomainsIfRequired(Schema schema)
        {
            if (this.Configuration.MembersAsNonReference)
            {
                return;
            }

            if (schema.Types.Contains(SchemaConstants.Group))
            {
                GroupMembership.GetInternalDomains(this.Configuration.DomainsService, this.Configuration.CustomerID);
            }
        }

        private void OpenImportConnectionFull(Schema types)
        {
            this.importCollection = new BlockingCollection<object>();
            this.SetHttpDebugMode();

            this.LoadInternalDomainsIfRequired(types);

            ManagementAgent.Schema = SchemaBuilder.GetSchema(this.Configuration);

            List<Task> tasks = new List<Task>();

            foreach (MASchemaType item in ManagementAgent.Schema.Where(t => types.Types.Contains(t.Name)))
            {
                Task t = item.ApiInterface.GetObjectImportTask(types, this.importCollection, this.cancellationToken.Token);

                if (t != null)
                {
                    Stopwatch stopwatch = new Stopwatch();
                    stopwatch.Start();

                    Logger.WriteLine($"Import task started for object type '{item.Name}'");

                    Task resultTask = t.ContinueWith(u =>
                    {
                        stopwatch.Stop();

                        if (u.IsFaulted)
                        {
                            Logger.WriteLine($"Import task failed for object type '{item.Name}'. Duration {stopwatch.Elapsed}");
                            Logger.WriteException(u.Exception);
                        }
                        else if (u.IsCanceled)
                        {
                            Logger.WriteLine($"Import task canceled for object type '{item.Name}'. Duration {stopwatch.Elapsed}");
                        }
                        else
                        {
                            Logger.WriteLine($"Import task completed successfully for object type '{item.Name}'. Duration {stopwatch.Elapsed}");
                        }
                    });

                    tasks.Add(t);
                    tasks.Add(resultTask);
                }
            }

            this.importTask = Task.WhenAny(Task.WhenAll(tasks.ToArray()), Task.Delay(Timeout.Infinite, this.cancellationToken.Token)).ContinueWith(_ =>
            {
                try
                {
                    List<Exception> exceptions = new List<Exception>();

                    foreach (Task t in tasks)
                    {
                        if (t.IsFaulted)
                        {
                            if (t.Exception != null)
                            {
                                Logger.WriteException(t.Exception);

                                if (t.Exception is AggregateException ae)
                                {
                                    exceptions.AddRange(ae.InnerExceptions);
                                }
                                else
                                {
                                    exceptions.Add(t.Exception);
                                }
                            }
                            else
                            {
                                exceptions.Add(new Exception("A task faulted but did not return an exception"));
                            }
                        }
                    }

                    if (exceptions.Count == 1)
                    {
                        throw exceptions[0];
                    }
                    else if (exceptions.Count > 0)
                    {
                        throw new AggregateException("One or more import operations failed", exceptions);
                    }
                }
                finally
                {
                    this.importCollection.CompleteAdding();
                }
            }, this.cancellationToken.Token);
        }

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

            try
            {
                for (int i = 0; i < this.importRunStep.PageSize; i++)
                {
                    if (this.importTask.IsFaulted || this.importTask.IsCanceled || this.cancellationToken.IsCancellationRequested)
                    {
                        break;
                    }

                    if (this.importCollection.IsCompleted && this.importCollection.Count == 0)
                    {
                        break;
                    }

                    if (!this.importCollection.TryTake(out object item, 50, this.cancellationToken.Token))
                    {
                        continue;
                    }

                    Interlocked.Increment(ref this.opCount);

                    if (item is CSEntryChange csentry)
                    {
                        results.CSEntries.Add(csentry);
                        continue;
                    }

                    throw new NotSupportedException("The object enumeration returned an unsupported type: " + item.GetType().Name);
                }

                results.MoreToImport = !(this.importCollection.IsCompleted && this.importCollection.Count == 0);

                if (results.MoreToImport && results.CSEntries.Count == 0)
                {
                    Thread.Sleep(1000);
                }
            }
            catch (OperationCanceledException)
            {
            }

            if (this.importTask.IsFaulted)
            {
                throw this.importTask.Exception ?? new Exception("The task was faulted, but an exception was not provided");
            }

            return results;
        }

        public CloseImportConnectionResults CloseImportConnection(CloseImportConnectionRunStep importRunStep)
        {
            Logger.WriteLine("Closing import connection: {0}", importRunStep.Reason);

            try
            {
                if (importRunStep.Reason != CloseReason.Normal)
                {
                    this.cancellationToken?.Cancel();
                }

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
                Logger.WriteLine("An unexpected error occurred");
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
        }

        public void SetPassword(CSEntry csentry, System.Security.SecureString newPassword, PasswordOptions options)
        {
            try
            {
                this.Configuration.UsersService.SetPassword(csentry.DN.ToString(), newPassword);
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