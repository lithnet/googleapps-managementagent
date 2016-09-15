using System;
using System.Collections.Generic;
using System.Linq;
using Lithnet.GoogleApps.ManagedObjects;
using Microsoft.MetadirectoryServices;

namespace Lithnet.GoogleApps.MA
{
    internal class ApiInterfaceGroupSettings : IApiInterface
    {
        public string Api => "groupsettings";

        public IList<AttributeChange> ApplyChanges(CSEntryChange csentry, SchemaType type, IManagementAgentParameters config, ref object target, bool patch = false)
        {
            bool hasChanged = false;

            GroupSettings settings;

            if (patch)
            {
                settings = new GroupSettings();
            }
            else
            {
                settings = GroupSettingsRequestFactory.Get(this.GetDNValue(target));
            }

            foreach (IAttributeAdapter typeDef in ManagementAgent.Schema[SchemaConstants.Group].Attributes.Where(t => t.Api == this.Api))
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
                return new List<AttributeChange>();
            }

            GroupSettings result;

            if (patch)
            {
                result = GroupSettingsRequestFactory.Patch(this.GetDNValue(target), settings);
            }
            else
            {
                result = GroupSettingsRequestFactory.Update(this.GetDNValue(target), settings);
            }

            return this.GetChanges(csentry.DN, csentry.ObjectModificationType, type, result);
        }

        public IList<AttributeChange> GetChanges(string dn, ObjectModificationType modType, SchemaType type, object source)
        {
            List<AttributeChange> attributeChanges = new List<AttributeChange>();

            GroupSettings settings = source as GroupSettings;

            if (settings == null)
            {
                GoogleGroup group = source as GoogleGroup;

                if (group == null)
                {
                    throw new InvalidOperationException();
                }
                else
                {
                    settings = group.Settings;
                }
            }


            foreach (IAttributeAdapter typeDef in ManagementAgent.Schema[SchemaConstants.Group].Attributes.Where(t => t.Api == this.Api))
            {
                if (type.HasAttribute(typeDef.AttributeName))
                {
                    attributeChanges.AddRange(typeDef.CreateAttributeChanges(dn, modType, settings));
                }
            }

            return attributeChanges;
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