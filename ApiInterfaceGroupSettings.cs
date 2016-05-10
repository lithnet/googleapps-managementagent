using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lithnet.GoogleApps.MA
{
    using Google.Apis.Admin.Directory.directory_v1.Data;
    using ManagedObjects;
    using MetadirectoryServices;
    using Microsoft.MetadirectoryServices;
    using User = ManagedObjects.User;

    public class ApiInterfaceGroupSettings : ApiInterface
    {
        private static MASchemaType maType = SchemaBuilder.GetUserSchema();

        public ApiInterfaceGroupSettings()
        {
            this.Api = "groupsettings";
        }

        public override bool IsPrimary => false;

        public override object CreateInstance(CSEntryChange csentry)
        {
            throw new NotSupportedException();
        }

        public override object GetInstance(CSEntryChange csentry)
        {
            throw new NotSupportedException();
        }

        public override void DeleteInstance(CSEntryChange csentry)
        {
            throw new NotSupportedException();
        }

        public override IList<AttributeChange> ApplyChanges(CSEntryChange csentry, SchemaType type, object target, bool patch = false)
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

            foreach (IMASchemaAttribute typeDef in ApiInterfaceGroupSettings.maType.Attributes.Where(t => t.Api == this.Api))
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

        public override IList<AttributeChange> GetChanges(ObjectModificationType modType, SchemaType type, object source)
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
        

            foreach (IMASchemaAttribute typeDef in ApiInterfaceGroupSettings.maType.Attributes.Where(t => t.Api == this.Api))
            {
                if (type.HasAttribute(typeDef.AttributeName))
                {
                    attributeChanges.AddRange(typeDef.CreateAttributeChanges(modType, settings));
                }
            }

            return attributeChanges;
        }

        public override string GetAnchorValue(object target)
        {
            return ((Group)target).Id;
        }

        public override string GetDNValue(object target)
        {
            return ((Group)target).Email;
        }
    }
}
