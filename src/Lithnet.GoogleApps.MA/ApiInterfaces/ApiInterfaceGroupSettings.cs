using System;
using System.Collections.Generic;
using System.Linq;
using Lithnet.GoogleApps.ManagedObjects;
using Microsoft.MetadirectoryServices;

namespace Lithnet.GoogleApps.MA
{
    internal class ApiInterfaceGroupSettings : IApiInterface
    {
        private IManagementAgentParameters config;

        public string Api => "groupsettings";

        public ApiInterfaceGroupSettings(IManagementAgentParameters config)
        {
            this.config = config;
        }

        public void ApplyChanges(CSEntryChange csentry, CSEntryChange committedChanges, SchemaType type, ref object target, bool patch = false)
        {
            bool hasChanged = false;

            GroupSettings settings;

            if (patch)
            {
                settings = new GroupSettings();
            }
            else
            {
                settings = this.config.GroupsService.SettingsFactory.Get(this.GetDNValue(target));
            }

            foreach (IAttributeAdapter typeDef in ManagementAgent.Schema[SchemaConstants.Group].AttributeAdapters.Where(t => t.Api == this.Api))
            {
                if (typeDef.UpdateField(csentry, settings))
                {
                    hasChanged = true;
                }
            }

            if (settings.WhoCanPostMessage != null)
            {
                if (settings.WhoCanPostMessage == "NONE_CAN_POST")
                {
                    if (settings.ArchiveOnly != true)
                    {
                        settings.ArchiveOnly = true;
                        hasChanged = true;
                    }
                }
                else
                {
                    if (settings.ArchiveOnly != false)
                    {
                        settings.ArchiveOnly = false;
                        hasChanged = true;
                    }
                }
            }

            if (!hasChanged)
            {
                return;
            }

            GroupSettings result;

            if (patch)
            {
                result = this.config.GroupsService.SettingsFactory.Patch(this.GetDNValue(target), settings);
            }
            else
            {
                result = this.config.GroupsService.SettingsFactory.Update(this.GetDNValue(target), settings);
            }

            foreach (AttributeChange change in this.GetChanges(csentry.DN, csentry.ObjectModificationType, type, result))
            {
                committedChanges.AttributeChanges.Add(change);
            }
        }

        public IEnumerable<AttributeChange> GetChanges(string dn, ObjectModificationType modType, SchemaType type, object source)
        {
            if (!(source is GroupSettings settings))
            {
                if (!(source is GoogleGroup @group))
                {
                    throw new InvalidOperationException();
                }
                else
                {
                    settings = group.Settings;
                }
            }


            foreach (IAttributeAdapter typeDef in ManagementAgent.Schema[SchemaConstants.Group].AttributeAdapters.Where(t => t.Api == this.Api))
            {
                foreach (string attributeName in typeDef.MmsAttributeNames)
                {
                    if (type.HasAttribute(attributeName))
                    {
                        foreach (AttributeChange change in typeDef.CreateAttributeChanges(dn, modType, settings))
                        {
                            yield return change;
                        }
                    }
                }
            }
        }

        private string GetAnchorValue(object target)
        {
            return ((GoogleGroup)target).Group.Id;
        }

        private string GetDNValue(object target)
        {
            return ((GoogleGroup)target).Group.Email;
        }
    }
}