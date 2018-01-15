using System;
using System.Collections.Generic;
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
        public string Api => "calendaracl";

        private Dictionary<string, string> attributeRoleMapping = new Dictionary<string, string>()
        {
            {"freeBusyReaders", "freeBusyReader"},
            {"readers", "reader"},
            {"writers", "writer"},
            {"owners", "owner"},
            {"noAccess", "none"},
        };

        private IList<ValueChange> DeleteFromRole(CSEntryChange csentry, IList<AclRule> existingRules, string role, string attributeName, IManagementAgentParameters config, string calendarId)
        {
            List<ValueChange> valueChanges = new List<ValueChange>();

            AttributeChange change = csentry.AttributeChanges.FirstOrDefault(t => t.Name == attributeName);

            if (change == null)
            {
                return null;
            }

            foreach (string valueToDelete in change.GetValueDeletes<string>())
            {
                AclRule matchedRule = existingRules.FirstOrDefault(t => t.Role == role && valueToDelete.Equals(t.Scope.Value, StringComparison.CurrentCultureIgnoreCase));

                if (matchedRule == null)
                {
                    Logger.WriteLine($"{valueToDelete} does not have role {role} on calendar {csentry.DN}. The request to delete the value will be ignored");
                    valueChanges.Add(ValueChange.CreateValueDelete(valueToDelete));

                    continue;
                }

                if (matchedRule.Role == "owner" && calendarId.Equals(matchedRule.Scope.Value, StringComparison.CurrentCultureIgnoreCase))
                {

                }

                ResourceRequestFactory.DeleteCalendarAclRule(config.CustomerID, calendarId, matchedRule.Id);

                valueChanges.Add(ValueChange.CreateValueDelete(valueToDelete));
            }

            return valueChanges;
        }

        private IList<ValueChange> AddToRole(CSEntryChange csentry, IList<AclRule> existingRules, string role, string attributeName, IManagementAgentParameters config, string calendarId)
        {
            List<ValueChange> valueChanges = new List<ValueChange>();

            foreach (string valueToAdd in csentry.GetValueAdds<string>(attributeName))
            {
                AclRule matchedRule = existingRules.FirstOrDefault(t => t.Role == role && valueToAdd.Equals(t.Scope.Value, StringComparison.CurrentCultureIgnoreCase));

                if (matchedRule != null)
                {
                    Logger.WriteLine($"{valueToAdd} already has role {role} on calendar {csentry.DN}. The request to add the value will be ignored");
                    valueChanges.Add(ValueChange.CreateValueAdd(valueToAdd));
                    continue;
                }

                AclRule rule = new AclRule();
                rule.Scope = new AclRule.ScopeData();
                rule.Scope.Value = valueToAdd;
                rule.Scope.Type = this.GetScopeType(valueToAdd);

                ResourceRequestFactory.AddCalendarAclRule(config.CustomerID, calendarId, rule, config.CalendarSendNotificationOnPermissionChange);

                valueChanges.Add(ValueChange.CreateValueAdd(valueToAdd));
            }

            return valueChanges;
        }

        private IEnumerable<string> GetRoleMembers(string role, IList<AclRule> existingRules)
        {
            return existingRules.Where(t => t.Role == role).Select(rule => rule.Scope.Value);
        }

        private IList<AclRule> GetExistingRules(string customerId, string calendarEmail)
        {
            return ResourceRequestFactory.GetCalendarAclRules(customerId, calendarEmail).Where(t =>
                    // Ignore domain objects        
                    t.Scope?.Type != "domain" &&
                    // Ignore public permissions
                    t.Scope?.Type != "default" &&
                    // Ignore the default owner ACL
                    !(t.Role == "owner" && calendarEmail.Equals(t.Scope?.Value, StringComparison.CurrentCultureIgnoreCase))
            ).ToList();
        }

        private void DeleteExistingRules(string customerId, string calendarEmail, params AclRule[] rules)
        {
            foreach (AclRule rule in rules)
            {
                ResourceRequestFactory.DeleteCalendarAclRule(customerId, calendarEmail, rule.Id);
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

        public IList<AttributeChange> ApplyChanges(CSEntryChange csentry, SchemaType type, IManagementAgentParameters config, ref object target, bool patch = false)
        {
            if (!this.attributeRoleMapping.Keys.Any(type.HasAttribute))
            {
                return new List<AttributeChange>();
            }

            string calendarEmail = ((CalendarResource)target).ResourceEmail;

            List<AttributeChange> attributeChanges = new List<AttributeChange>();

            IList<AclRule> existingRules;

            if (csentry.ObjectModificationType == ObjectModificationType.Add)
            {
                existingRules = new List<AclRule>();
            }
            else
            {
                existingRules = this.GetExistingRules(config.CustomerID, calendarEmail);
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

                    IList<ValueChange> valueChanges = this.DeleteFromRole(csentry, existingRules, kvp.Value, kvp.Key, config, calendarEmail);

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
                this.DeleteExistingRules(config.CustomerID, calendarEmail, existingRules.ToArray());
            }

            foreach (KeyValuePair<string, string> kvp in this.attributeRoleMapping)
            {
                if (!type.HasAttribute(kvp.Key))
                {
                    continue;
                }

                IList<ValueChange> valueChanges = this.AddToRole(csentry, existingRules, kvp.Value, kvp.Key, config, calendarEmail);

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

            return attributeChanges;
        }

        public IList<AttributeChange> GetChanges(string dn, ObjectModificationType modType, SchemaType type, object source, IManagementAgentParameters config)
        {
            CalendarResource calendar = source as CalendarResource;

            if (calendar == null)
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
                    existingRules = this.GetExistingRules(config.CustomerID, calendar.ResourceEmail);
                }

                IList<string> items = this.GetRoleMembers(kvp.Value, existingRules).ToList();

                if (modType == ObjectModificationType.Update)
                {
                    ApiInterfaceCalendarAcl.AddAttributeChange(kvp.Key, AttributeModificationType.Replace, items.ToValueChangeAdd(), changes);
                }
                else
                {
                    ApiInterfaceCalendarAcl.AddAttributeChange(kvp.Key, AttributeModificationType.Add, items.ToValueChangeAdd(), changes);
                }
            }

            return changes;
        }

        private static void AddAttributeChange(string attributeName, AttributeModificationType modificationType, IList<ValueChange> changes, IList<AttributeChange> attributeChanges)
        {
            AttributeChange existingChange = attributeChanges.FirstOrDefault(t => t.Name == attributeName);

            if (modificationType == AttributeModificationType.Delete)
            {
                if (existingChange != null)
                {
                    attributeChanges.Remove(existingChange);
                }

                attributeChanges.Add(AttributeChange.CreateAttributeDelete(attributeName));
                return;
            }

            if (changes == null || changes.Count == 0)
            {
                return;
            }

            IList<object> adds;
            switch (modificationType)
            {
                case AttributeModificationType.Add:
                    if (existingChange != null)
                    {
                        foreach (ValueChange valueChange in changes.Where(t => t.ModificationType == ValueModificationType.Add))
                        {
                            existingChange.ValueChanges.Add(valueChange);
                        }
                    }
                    else
                    {
                        adds = changes.Where(t => t.ModificationType == ValueModificationType.Add).Select(t => t.Value).ToList();

                        if (adds.Count > 0)
                        {
                            attributeChanges.Add(AttributeChange.CreateAttributeAdd(attributeName, adds));
                        }
                    }
                    break;

                case AttributeModificationType.Replace:
                    if (existingChange != null)
                    {
                        attributeChanges.Remove(existingChange);
                    }

                    adds = changes.Where(t => t.ModificationType == ValueModificationType.Add).Select(t => t.Value).ToList();
                    if (adds.Count > 0)
                    {
                        attributeChanges.Add(AttributeChange.CreateAttributeReplace(attributeName, adds));
                    }

                    break;

                case AttributeModificationType.Update:
                    if (existingChange != null)
                    {
                        if (existingChange.ModificationType != AttributeModificationType.Update)
                        {
                            throw new InvalidOperationException();
                        }

                        foreach (ValueChange valueChange in changes)
                        {
                            existingChange.ValueChanges.Add(valueChange);
                        }
                    }
                    else
                    {
                        if (changes.Count > 0)
                        {
                            attributeChanges.Add(AttributeChange.CreateAttributeUpdate(attributeName, changes));
                        }
                    }

                    break;

                case AttributeModificationType.Delete:
                case AttributeModificationType.Unconfigured:
                default:
                    throw new InvalidOperationException();
            }
        }
    }
}