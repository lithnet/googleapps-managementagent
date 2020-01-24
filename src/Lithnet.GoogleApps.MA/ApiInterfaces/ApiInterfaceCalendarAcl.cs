using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Google.Apis.Admin.Directory.directory_v1.Data;
using Google.Apis.Calendar.v3.Data;
using Lithnet.Logging;
using Lithnet.MetadirectoryServices;
using Microsoft.MetadirectoryServices;

namespace Lithnet.GoogleApps.MA
{
    internal class ApiInterfaceCalendarAcl : IApiInterface
    {
        private IManagementAgentParameters config;

        public string Api => "calendaracl";

        public ApiInterfaceCalendarAcl(IManagementAgentParameters config)
        {
            this.config = config;
        }

        private Dictionary<string, string> attributeRoleMapping = new Dictionary<string, string>()
        {
            {"freeBusyReader", "freeBusyReader"},
            {"reader", "reader"},
            {"writer", "writer"},
            {"owner", "owner"},
        };

        public void ApplyChanges(CSEntryChange csentry, CSEntryChange committedChanges, SchemaType type, ref object target, bool patch = false)
        {
            if (!this.attributeRoleMapping.Keys.Any(type.HasAttribute))
            {
                return;
            }

            string calendarEmail = ((CalendarResource)target).ResourceEmail;

            IList<AclRule> existingRules;

            if (csentry.ObjectModificationType == ObjectModificationType.Add)
            {
                existingRules = new List<AclRule>();
            }
            else
            {
                existingRules = this.GetNonDefaultRules(this.config.CustomerID, calendarEmail);
            }

            Dictionary<string, AttributeChange> keyedAttributeChanges = new Dictionary<string, AttributeChange>();

            if (csentry.ObjectModificationType == ObjectModificationType.Update)
            {
                foreach (KeyValuePair<string, string> kvp in this.attributeRoleMapping)
                {
                    if (!type.HasAttribute(kvp.Key))
                    {
                        continue;
                    }

                    AttributeChange change = csentry.AttributeChanges.FirstOrDefault(t => t.Name == kvp.Key);

                    if (change == null)
                    {
                        continue;
                    }

                    IList<ValueChange> valueChanges = this.DeleteFromRole(change, existingRules, kvp.Value, calendarEmail);

                    if (valueChanges.Count > 0)
                    {
                        if (!keyedAttributeChanges.ContainsKey(kvp.Key))
                        {
                            keyedAttributeChanges.Add(kvp.Key, AttributeChange.CreateAttributeUpdate(kvp.Key, valueChanges));
                        }
                        else
                        {
                            foreach (ValueChange valueChange in valueChanges)
                            {
                                keyedAttributeChanges[kvp.Key].ValueChanges.Add(valueChange);
                            }
                        }
                    }
                }
            }

            if (csentry.ObjectModificationType == ObjectModificationType.Replace || csentry.ObjectModificationType == ObjectModificationType.Add)
            {
                // If we are replacing, we don't know the existing values, so we can delete and re-add them.
                // If we are adding, we want to clear out the Google-default ACLs, to avoid exported-change-not-reimported errors.
                this.DeleteRules(this.config.CustomerID, calendarEmail, existingRules.ToArray());
            }

            if (committedChanges.ObjectModificationType == ObjectModificationType.Unconfigured)
            {
                if (csentry.ObjectModificationType == ObjectModificationType.Update || csentry.ObjectModificationType == ObjectModificationType.Replace)
                {
                    committedChanges.ObjectModificationType = ObjectModificationType.Update;
                }

                committedChanges.ObjectModificationType = csentry.ObjectModificationType;
            }

            foreach (KeyValuePair<string, string> kvp in this.attributeRoleMapping)
            {
                if (!type.HasAttribute(kvp.Key))
                {
                    continue;
                }

                AttributeChange change = csentry.AttributeChanges.FirstOrDefault(t => t.Name == kvp.Key);

                if (change == null)
                {
                    continue;
                }

                IList<ValueChange> valueChanges = this.AddToRole(change, existingRules, kvp.Value, calendarEmail);

                if (valueChanges.Count == 0)
                {
                    continue;
                }

                if (!keyedAttributeChanges.ContainsKey(kvp.Key))
                {
                    if (csentry.ObjectModificationType == ObjectModificationType.Update)
                    {
                        keyedAttributeChanges.Add(kvp.Key, AttributeChange.CreateAttributeUpdate(kvp.Key, valueChanges));
                    }
                    else
                    {
                        keyedAttributeChanges.Add(kvp.Key, AttributeChange.CreateAttributeAdd(kvp.Key, valueChanges.Select(t => t.Value).ToList()));
                    }
                }
                else
                {
                    foreach (ValueChange valueChange in valueChanges)
                    {
                        keyedAttributeChanges[kvp.Key].ValueChanges.Add(valueChange);
                    }
                }
            }

            foreach (KeyValuePair<string, AttributeChange> changes in keyedAttributeChanges)
            {
                committedChanges.AttributeChanges.Add(changes.Value);
            }
        }

        public IEnumerable<AttributeChange> GetChanges(string dn, ObjectModificationType modType, SchemaType type, object source)
        {
            if (!(source is CalendarResource calendar))
            {
                throw new InvalidOperationException();
            }

            IList<AclRule> existingRules = null;
            List<AttributeChange> changes = new List<AttributeChange>();

            foreach (var kvp in this.attributeRoleMapping)
            {
                if (!type.HasAttribute(kvp.Key))
                {
                    continue;
                }

                if (existingRules == null)
                {
                    // don't populate in advance. Only get the rules if we have a request for a related attribute
                    existingRules = this.GetNonDefaultRules(this.config.CustomerID, calendar.ResourceEmail);
                }

                IList<object> items = this.GetRoleMembers(kvp.Value, existingRules).ToList<object>();

                if (items.Count == 0)
                {
                    continue;
                }

                if (modType == ObjectModificationType.Update)
                {
                    changes.Add(AttributeChange.CreateAttributeReplace(kvp.Key, items));
                }
                else
                {
                    changes.Add(AttributeChange.CreateAttributeAdd(kvp.Key, items));
                }
            }

            return changes;
        }

        private IList<ValueChange> DeleteFromRole(AttributeChange change, IList<AclRule> existingRules, string role, string calendarId)
        {
            List<ValueChange> valueChanges = new List<ValueChange>();

            IList<string> deletes;

            if (change.ModificationType == AttributeModificationType.Delete || change.ModificationType == AttributeModificationType.Replace)
            {
                deletes = existingRules.Where(t => t.Role == role).Select(t => t.Scope.Value).ToList();
            }
            else
            {
                deletes = change.GetValueDeletes<string>();
            }

            foreach (string valueToDelete in deletes)
            {
                AclRule matchedRule = existingRules.FirstOrDefault(t => t.Role == role && valueToDelete.Equals(t.Scope.Value, StringComparison.CurrentCultureIgnoreCase));

                if (matchedRule == null)
                {
                    Logger.WriteLine($"{valueToDelete} does not have role {role} on calendar {calendarId}. The request to delete the value will be ignored");
                    valueChanges.Add(ValueChange.CreateValueDelete(valueToDelete));

                    continue;
                }

                if (matchedRule.Role == "owner" && calendarId.Equals(matchedRule.Scope.Value, StringComparison.CurrentCultureIgnoreCase))
                {
                    Debug.WriteLine($"Ignoring default ACL for {valueToDelete} to role {role} on calendar {calendarId}");
                    continue;
                }

                try
                {
                    this.config.ResourcesService.DeleteCalendarAclRule(this.config.CustomerID, calendarId, matchedRule.Id);
                    Logger.WriteLine($"Removed {valueToDelete} from role {role} on calendar {calendarId}");
                }
                catch
                {
                    Logger.WriteLine($"Failed to remove {valueToDelete} to role {role} on calendar {calendarId}");
                    throw;
                }

                valueChanges.Add(ValueChange.CreateValueDelete(valueToDelete));
            }

            return valueChanges;
        }

        private IList<ValueChange> AddToRole(AttributeChange change, IList<AclRule> existingRules, string role, string calendarId)
        {
            List<ValueChange> valueChanges = new List<ValueChange>();

            foreach (string valueToAdd in change.GetValueAdds<string>())
            {
                AclRule matchedRule = existingRules.FirstOrDefault(t => t.Role == role && valueToAdd.Equals(t.Scope.Value, StringComparison.CurrentCultureIgnoreCase));

                if (matchedRule != null)
                {
                    Logger.WriteLine($"{valueToAdd} already has role {role} on calendar {calendarId}. The request to add the value will be ignored");
                    valueChanges.Add(ValueChange.CreateValueAdd(valueToAdd));
                    continue;
                }

                AclRule rule = new AclRule();
                rule.Role = role;
                rule.Scope = new AclRule.ScopeData();
                rule.Scope.Value = valueToAdd;
                rule.Scope.Type = this.GetScopeType(valueToAdd);

                try
                {
                    this.config.ResourcesService.AddCalendarAclRule(this.config.CustomerID, calendarId, rule, this.config.CalendarSendNotificationOnPermissionChange);
                    Logger.WriteLine($"Added {valueToAdd} to role {role} on calendar {calendarId}");
                }
                catch
                {
                    Logger.WriteLine($"Failed to add {valueToAdd} to role {role} on calendar {calendarId}");
                    throw;
                }

                valueChanges.Add(ValueChange.CreateValueAdd(valueToAdd));
            }

            return valueChanges;
        }

        private IEnumerable<string> GetRoleMembers(string role, IList<AclRule> existingRules)
        {
            return existingRules.Where(t => t.Role == role).Select(rule => rule.Scope.Value);
        }

        private IList<AclRule> GetNonDefaultRules(string customerId, string calendarEmail)
        {
            return this.config.ResourcesService.GetCalendarAclRules(customerId, calendarEmail).Where(t =>
                    // Ignore domain objects        
                    t.Scope?.Type != "domain" &&
                    // Ignore public permissions
                    t.Scope?.Type != "default" &&
                    // Ignore the default owner ACL
                    !(t.Role == "owner" && calendarEmail.Equals(t.Scope?.Value, StringComparison.CurrentCultureIgnoreCase))
            ).ToList();
        }

        private void DeleteRules(string customerId, string calendarEmail, params AclRule[] rules)
        {
            foreach (AclRule rule in rules)
            {
                try
                {
                    this.config.ResourcesService.DeleteCalendarAclRule(customerId, calendarEmail, rule.Id);
                    Logger.WriteLine($"Deleted {rule.Scope.Value} from role {rule.Role} on calendar {calendarEmail}");
                }
                catch
                {
                    Logger.WriteLine($"Failed to delete {rule.Scope.Value} from role {rule.Role} on calendar {calendarEmail}");
                    throw;
                }
            }
        }

        private string GetScopeType(string scopeValue)
        {
            if (scopeValue.Contains("@"))
            {
                return "user";
            }
            else
            {
                return "domain";
            }
        }
    }
}