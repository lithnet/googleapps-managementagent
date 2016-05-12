using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Apis.Admin.Directory.directory_v1.Data;
using Lithnet.GoogleApps.ManagedObjects;
using Lithnet.MetadirectoryServices;
using Microsoft.MetadirectoryServices;

namespace Lithnet.GoogleApps.MA
{
    internal class ApiInterfaceGroupSettings : IApiInterface
    {
        public string Api => "groupsettings";

        public IList<AttributeChange> ApplyChanges(CSEntryChange csentry, SchemaType type, object target, bool patch = false)
        {
            bool hasChanged = false;

            GroupSettings settings;

            if (csentry.ObjectModificationType == ObjectModificationType.Add || patch)
            {
                settings = new GroupSettings();
            }
            else
            {
                settings = GroupSettingsRequestFactory.Get(this.GetAnchorValue(target));
            }

            foreach (IMASchemaAttribute typeDef in ManagementAgent.Schema[SchemaConstants.Group].Attributes.Where(t => t.Api == this.Api))
            {
                if (typeDef.UpdateField(csentry, settings))
                {
                    hasChanged = true;
                }
            }

            if (!hasChanged)
            {
                return new List<AttributeChange>();
            }

            GroupSettings result;

            if (patch)
            {
                result = GroupSettingsRequestFactory.Patch(this.GetAnchorValue(target), settings);
            }
            else
            {
                result = GroupSettingsRequestFactory.Update(this.GetAnchorValue(target), settings);
            }

            return this.GetChanges(csentry.ObjectModificationType, type, result);
        }

        public IList<AttributeChange> GetChanges(ObjectModificationType modType, SchemaType type, object source)
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


            foreach (IMASchemaAttribute typeDef in ManagementAgent.Schema[SchemaConstants.Group].Attributes.Where(t => t.Api == this.Api))
            {
                if (type.HasAttribute(typeDef.AttributeName))
                {
                    attributeChanges.AddRange(typeDef.CreateAttributeChanges(modType, settings));
                }
            }

            return attributeChanges;
        }

        private string GetAnchorValue(object target)
        {
            return ((Group)target).Id;
        }

        private string GetDNValue(object target)
        {
            return ((Group)target).Email;
        }
    }
}