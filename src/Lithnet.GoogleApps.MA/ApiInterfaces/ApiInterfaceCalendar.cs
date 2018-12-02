using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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

        public IList<AttributeChange> ApplyChanges(CSEntryChange csentry, SchemaType type, ref object target, bool patch = false)
        {
            bool hasChanged = false;
            List<AttributeChange> changes = new List<AttributeChange>();

            CalendarResource calendar = target as CalendarResource;

            if (calendar == null)
            {
                throw new InvalidOperationException();
            }

            hasChanged |= this.SetDNValue(csentry, calendar);

            foreach (IAttributeAdapter typeDef in this.SchemaType.AttributeAdapters.Where(t => t.Api == this.Api))
            {
                hasChanged |= typeDef.UpdateField(csentry, calendar);
            }

            if (hasChanged)
            {
                CalendarResource result;

                if (csentry.ObjectModificationType == ObjectModificationType.Add)
                {
                    result = this.config.ResourcesService.AddCalendar(this.customerID, calendar);
                    calendar.ResourceEmail = result.ResourceEmail;
                    System.Threading.Thread.Sleep(1500);
                }
                else if (csentry.ObjectModificationType == ObjectModificationType.Replace || csentry.ObjectModificationType == ObjectModificationType.Update)
                {
                    string id = csentry.GetAnchorValueOrDefault<string>("id");

                    if (patch)
                    {
                        result = this.config.ResourcesService.PatchCalendar(this.customerID, id, calendar);
                    }
                    else
                    {
                        result = this.config.ResourcesService.UpdateCalendar(this.customerID, id, calendar);
                    }
                }
                else
                {
                    throw new InvalidOperationException();
                }

                changes.AddRange(this.GetLocalChanges(csentry.DN, csentry.ObjectModificationType, type, result));
            }

            foreach (IApiInterface i in this.internalInterfaces)
            {
                foreach (AttributeChange c in i.ApplyChanges(csentry, type, ref target, patch))
                {
                    changes.Add(c);
                }
            }

            return changes;
        }

        public IList<AttributeChange> GetChanges(string dn, ObjectModificationType modType, SchemaType type, object source)
        {
            List<AttributeChange> attributeChanges = this.GetLocalChanges(dn, modType, type, source);

            foreach (IApiInterface i in this.internalInterfaces)
            {
                attributeChanges.AddRange(i.GetChanges(dn, modType, type, source));
            }

            return attributeChanges;
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

        public Task GetItems(MmsSchema schema, BlockingCollection<object> collection)
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
                Logger.WriteLine("Starting calendar import task");
                Logger.WriteLine("Requesting calendar fields: " + fields);

                Parallel.ForEach(this.config.ResourcesService.GetCalendars(this.config.CustomerID, fields), calendar =>
                {
                    collection.Add(this.GetCSEntryForCalendar(calendar, schema, this.config));
                    Debug.WriteLine($"Created CSEntryChange for calendar: {calendar.ResourceEmail}");
                });

                Logger.WriteLine("Calendar import task complete");
            });

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

