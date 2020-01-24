using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Admin.Directory.directory_v1.Data;
using Lithnet.Logging;
using Lithnet.MetadirectoryServices;
using Microsoft.MetadirectoryServices;
using Newtonsoft.Json.Linq;
using MmsSchema = Microsoft.MetadirectoryServices.Schema;

namespace Lithnet.GoogleApps.MA
{
    internal class ApiInterfaceCalendar : IApiInterfaceObject
    {
        private string customerID;

        private IManagementAgentParameters config;

        private ApiInterfaceKeyedCollection internalInterfaces;

        protected MASchemaType SchemaType { get; set; }

        public ApiInterfaceCalendar(string customerID, MASchemaType type, IManagementAgentParameters config)
        {
            this.SchemaType = type;
            this.customerID = customerID;
            this.config = config;

            this.internalInterfaces = new ApiInterfaceKeyedCollection
            {
                new ApiInterfaceCalendarAcl(config),
            };
        }

        public string Api => "calendar";

        public ObjectModificationType DeltaUpdateType => ObjectModificationType.Update;

        public object CreateInstance(CSEntryChange csentry)
        {
            CalendarResource calendar = new CalendarResource();
            calendar.ResourceId = Guid.NewGuid().ToString("n");

            return calendar;
        }

        public object GetInstance(CSEntryChange csentry)
        {
            return this.config.ResourcesService.GetCalendar(this.customerID, csentry.GetAnchorValueOrDefault<string>("id"));
        }

        public void DeleteInstance(CSEntryChange csentry)
        {
            this.config.ResourcesService.DeleteCalendar(this.customerID, csentry.GetAnchorValueOrDefault<string>("id"));
        }

        public void ApplyChanges(CSEntryChange csentry, CSEntryChange committedChanges, SchemaType type, ref object target, bool patch = false)
        {
            bool hasChanged = false;

            if (!(target is CalendarResource calendar))
            {
                throw new InvalidOperationException();
            }

            hasChanged |= this.SetDNValue(csentry, calendar);

            foreach (IAttributeAdapter typeDef in this.SchemaType.AttributeAdapters.Where(t => t.Api == this.Api))
            {
                hasChanged |= typeDef.UpdateField(csentry, calendar);
            }

            if (csentry.ObjectModificationType == ObjectModificationType.Add)
            {
                calendar = this.config.ResourcesService.AddCalendar(this.customerID, calendar);
                calendar.ResourceEmail = calendar.ResourceEmail;
                committedChanges.ObjectModificationType = ObjectModificationType.Add;
                committedChanges.DN = this.GetDNValue(calendar);

                Thread.Sleep(1500);
            }

            if (csentry.IsUpdateOrReplace() && hasChanged)
            {
                string id = csentry.GetAnchorValueOrDefault<string>("id");

                if (patch)
                {
                    calendar = this.config.ResourcesService.PatchCalendar(this.customerID, id, calendar);
                }
                else
                {
                    calendar = this.config.ResourcesService.UpdateCalendar(this.customerID, id, calendar);
                }
            }

            if (csentry.IsUpdateOrReplace())
            {
                committedChanges.ObjectModificationType = this.DeltaUpdateType;
                committedChanges.DN = this.GetDNValue(calendar);
            }

            foreach (AttributeChange change in this.GetLocalChanges(csentry.DN, csentry.ObjectModificationType, type, calendar))
            {
                committedChanges.AttributeChanges.Add(change);
            }

            target = calendar;

            foreach (IApiInterface i in this.internalInterfaces)
            {
                i.ApplyChanges(csentry, committedChanges, type, ref target, patch);
            }
        }

        public IEnumerable<AttributeChange> GetChanges(string dn, ObjectModificationType modType, SchemaType type, object source)
        {
            foreach (AttributeChange change in this.GetLocalChanges(dn, modType, type, source))
            {
                yield return change;
            }

            foreach (IApiInterface i in this.internalInterfaces)
            {
                foreach (AttributeChange change in i.GetChanges(dn, modType, type, source))
                {
                    yield return change;
                }
            }
        }

        private List<AttributeChange> GetLocalChanges(string dn, ObjectModificationType modType, SchemaType type, object source)
        {
            List<AttributeChange> attributeChanges = new List<AttributeChange>();

            CalendarResource calendar = source as CalendarResource;

            if (calendar == null)
            {
                throw new InvalidOperationException();
            }

            foreach (IAttributeAdapter typeDef in this.SchemaType.AttributeAdapters.Where(t => t.Api == this.Api))
            {
                if (typeDef.IsAnchor)
                {
                    continue;
                }

                foreach (AttributeChange change in typeDef.CreateAttributeChanges(dn, modType, calendar))
                {
                    if (type.HasAttribute(change.Name))
                    {
                        attributeChanges.Add(change);
                    }
                }
            }

            return attributeChanges;
        }

        public string GetAnchorValue(string name, object target)
        {
            CalendarResource calendar = target as CalendarResource;

            if (calendar == null)
            {
                throw new InvalidOperationException();
            }

            switch (name)
            {
                case "id":
                    return calendar.ResourceId;

                case "resourceEmail":
                    return calendar.ResourceEmail;

                default:
                    throw new InvalidOperationException();
            }
        }

        public string GetDNValue(object target)
        {
            CalendarResource calendar = target as CalendarResource;

            if (calendar == null)
            {
                throw new InvalidOperationException();
            }

            return calendar.ResourceEmail;
        }

        public Task GetObjectImportTask(MmsSchema schema, BlockingCollection<object> collection, CancellationToken cancellationToken)
        {
            HashSet<string> fieldList = new HashSet<string>
            {
                "resourceName",
                "resourceEmail",
                "resourceId"
            };

            foreach (string fieldName in ManagementAgent.Schema[SchemaConstants.Calendar].GetFieldNames(schema.Types[SchemaConstants.Calendar], "calendar"))
            {
                fieldList.Add(fieldName);
            }

            string fields = String.Format("items({0}), nextPageToken", String.Join(",", fieldList));

            Task t = new Task(() =>
            {
                Logger.WriteLine("Requesting calendar fields: " + fields);

                Parallel.ForEach(this.config.ResourcesService.GetCalendars(this.config.CustomerID, fields), calendar =>
                {
                    collection.Add(this.GetCSEntryForCalendar(calendar, schema, this.config));
                    Debug.WriteLine($"Created CSEntryChange for calendar: {calendar.ResourceEmail}");
                });
            }, cancellationToken);

            t.Start();

            return t;
        }

        private CSEntryChange GetCSEntryForCalendar(CalendarResource calendar, MmsSchema schema, IManagementAgentParameters config)
        {
            return ImportProcessor.GetCSEntryChange(calendar, schema.Types[SchemaConstants.Calendar], config);
        }

        private bool SetDNValue(CSEntryChange csentry, CalendarResource calendar)
        {
            if (csentry.ObjectModificationType != ObjectModificationType.Replace && csentry.ObjectModificationType != ObjectModificationType.Update)
            {
                return false;
            }

            string newDN = csentry.GetNewDNOrDefault<string>();

            if (newDN == null)
            {
                return false;
            }

            throw new NotSupportedException("Renaming the DN of this object is not supported");
        }

        internal static IEnumerable<string> GetFeatureNames(CalendarResource calendar, string attributeType)
        {
            return ApiInterfaceCalendar.GetFeatureNames(calendar, attributeType == "Reference" ? AttributeType.Reference : AttributeType.String);
        }

        internal static IEnumerable<string> GetFeatureNames(CalendarResource calendar, AttributeType type = AttributeType.String)
        {
            List<FeatureInstance> items = ((JArray)calendar?.FeatureInstances)?.ToObject<List<FeatureInstance>>();

            return items?.Select(t =>
            {
                string featureName = t.Feature.Name;

                if (type == AttributeType.Reference)
                {
                    featureName = $"{featureName}{ApiInterfaceFeature.DNSuffix}";
                }

                return featureName;
            });
        }
    }
}

