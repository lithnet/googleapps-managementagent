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

        private static ApiInterfaceKeyedCollection internalInterfaces;

        protected MASchemaType SchemaType { get; set; }

        public static string DNSuffix => "@calendar.resource";

        static ApiInterfaceCalendar()
        {
            ApiInterfaceCalendar.internalInterfaces = new ApiInterfaceKeyedCollection
            {
                new ApiInterfaceCalendarAcl(),
            };
        }

        public ApiInterfaceCalendar(string customerID, MASchemaType type)
        {
            this.SchemaType = type;
            this.customerID = customerID;
        }

        public string Api => "calendar";

        public ObjectModificationType DeltaUpdateType => ObjectModificationType.Update;

        public object CreateInstance(CSEntryChange csentry)
        {
            CalendarResource calendar = new CalendarResource();
            calendar.ResourceId = Guid.NewGuid().ToString("n");

            ApiInterfaceCalendar.ThrowOnInvalidDN(csentry.DN);

            calendar.ResourceName = csentry.DN.Replace(ApiInterfaceCalendar.DNSuffix, string.Empty);
            return calendar;
        }

        private static void ThrowOnInvalidDN(string dn)
        {
            if (dn == null || !dn.EndsWith(ApiInterfaceCalendar.DNSuffix))
            {
                throw new InvalidDNException($"The DN must end with '{ApiInterfaceCalendar.DNSuffix}'");
            }
        }

        public object GetInstance(CSEntryChange csentry)
        {
            return ResourceRequestFactory.GetCalendar(this.customerID, csentry.GetAnchorValueOrDefault<string>("id"));
        }

        public void DeleteInstance(CSEntryChange csentry)
        {
            ResourceRequestFactory.DeleteCalendar(this.customerID, csentry.GetAnchorValueOrDefault<string>("id"));
        }

        public IList<AttributeChange> ApplyChanges(CSEntryChange csentry, SchemaType type, IManagementAgentParameters config, ref object target, bool patch = false)
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
                    result = ResourceRequestFactory.AddCalendar(this.customerID, calendar);
                }
                else if (csentry.ObjectModificationType == ObjectModificationType.Replace || csentry.ObjectModificationType == ObjectModificationType.Update)
                {
                    string id = csentry.GetAnchorValueOrDefault<string>(this.SchemaType.AnchorAttributeName);

                    if (patch)
                    {
                        result = ResourceRequestFactory.PatchCalendar(this.customerID, id, calendar);
                    }
                    else
                    {
                        result = ResourceRequestFactory.UpdateCalendar(this.customerID, id, calendar);
                    }
                }
                else
                {
                    throw new InvalidOperationException();
                }

                changes.AddRange(this.GetLocalChanges(csentry.DN, csentry.ObjectModificationType, type, result));
            }

            foreach (IApiInterface i in ApiInterfaceCalendar.internalInterfaces)
            {
                foreach (AttributeChange c in i.ApplyChanges(csentry, type, config, ref target, patch))
                {
                    changes.Add(c);
                }
            }

            return changes;
        }

        public IList<AttributeChange> GetChanges(string dn, ObjectModificationType modType, SchemaType type, object source, IManagementAgentParameters config)
        {
            List<AttributeChange> attributeChanges = this.GetLocalChanges(dn, modType, type, source);

            foreach (IApiInterface i in ApiInterfaceCalendar.internalInterfaces)
            {
                attributeChanges.AddRange(i.GetChanges(dn, modType, type, source, config));
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

        public string GetAnchorValue(object target)
        {
            CalendarResource calendar = target as CalendarResource;

            if (calendar == null)
            {
                throw new InvalidOperationException();
            }

            return calendar.ResourceId;
        }

        public string GetDNValue(object target)
        {
            CalendarResource calendar = target as CalendarResource;

            if (calendar == null)
            {
                throw new InvalidOperationException();
            }

            return $"{calendar.ResourceName ?? calendar.ResourceId}{ApiInterfaceCalendar.DNSuffix}";
        }

        public Task GetItems(IManagementAgentParameters config, MmsSchema schema, BlockingCollection<object> collection)
        {
            HashSet<string> fieldList = new HashSet<string>
            {
                "resourceName",
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

                Parallel.ForEach(ResourceRequestFactory.GetCalendars(config.CustomerID, fields), calendar =>
                {
                    collection.Add(this.GetCSEntryForCalendar(calendar, schema, config));
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

            ApiInterfaceCalendar.ThrowOnInvalidDN(csentry.DN);

            calendar.ResourceName = newDN.Replace($"{ApiInterfaceCalendar.DNSuffix}", string.Empty);

            return true;
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

