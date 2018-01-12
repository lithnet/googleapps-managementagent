using System;
using System.Collections.Generic;
using System.Linq;
using Lithnet.Logging;
using Lithnet.MetadirectoryServices;
using Microsoft.MetadirectoryServices;
using User = Lithnet.GoogleApps.ManagedObjects.User;

namespace Lithnet.GoogleApps.MA
{
    internal class AdapterCustomSchemaMultivaluedField : AdapterCustomSchemaField
    {
        public override bool IsMultivalued => true;

        public override bool UpdateField(CSEntryChange csentry, object obj)
        {
            if (this.IsReadOnly)
            {
                return false;
            }

            if (!csentry.HasAttributeChange(this.MmsAttributeName))
            {
                return false;
            }

            AttributeModificationType modType = csentry.AttributeChanges[this.MmsAttributeName].ModificationType;
            IList<object> valueAdds = csentry.GetValueAdds<object>(this.MmsAttributeName);
            IList<object> valueDeletes = csentry.GetValueDeletes<object>(this.MmsAttributeName);

            valueAdds = this.ConvertToNativeGoogleFormat(valueAdds);
            valueDeletes = this.ConvertToNativeGoogleFormat(valueDeletes);

            User user = obj as User;

            if (user == null)
            {
                throw new NotSupportedException("The provided object was not of a 'user' type");
            }

            if (valueAdds.Count == 0 && !this.HasSchemaField(user))
            {
                return false;
            }

            IDictionary<string, object> schema = this.GetOrCreateSchema(user, out bool created);

            if (modType == AttributeModificationType.Delete)
            {
                if (this.HasSchemaField(user))
                {
                    schema[this.FieldName] = Utilities.GetNullRepresentation(this.NullValueRepresentation);
                    Logger.WriteLine($"Deleting {this.MmsAttributeName}");
                    return true;
                }

                return false;
            }

            IList<object> list = null;

            if (!this.HasSchemaField(user) || modType == AttributeModificationType.Replace || modType == AttributeModificationType.Add)
            {
                list = new List<object>();
            }
            else
            {
                list = this.GetValuesFromArray(schema[this.FieldName]);
            }

            if (modType == AttributeModificationType.Update)
            {
                foreach (object value in valueDeletes)
                {
                    list.Remove(value);
                    Logger.WriteLine($"Removing value {this.MmsAttributeName} -> {value}");
                }
            }

            foreach (object value in valueAdds)
            {
                list.Add(value);
                Logger.WriteLine($"Adding value {this.MmsAttributeName} -> {value}");
            }

            if (list.Count > 0)
            {
                List<Dictionary<string, object>> items = new List<Dictionary<string, object>>();

                foreach (object value in list)
                {
                    Dictionary<string, object> item = new Dictionary<string, object>();
                    item.Add("value", value);
                    items.Add(item);
                }

                schema[this.FieldName] = items;
            }
            else
            {
                object value = null;

                value = Utilities.GetNullRepresentation(this.NullValueRepresentation);
                Logger.WriteLine($"Set {this.MmsAttributeName} -> {value ?? "<null>"}");

                schema[this.FieldName] = value;
            }

            return true;
        }

        public override IEnumerable<AttributeChange> CreateAttributeChanges(string dn, ObjectModificationType modType, object obj)
        {
            User user = obj as User;

            if (user == null)
            {
                throw new InvalidOperationException();
            }

            if (!this.HasSchemaField(user))
            {
                yield break;
            }

            IDictionary<string, object> schema = this.GetOrCreateSchema(user, out bool created);

            object value = null;

            if (schema.ContainsKey(this.FieldName))
            {
                value = schema[this.FieldName];
            }

            if (value == null)
            {
                if (modType == ObjectModificationType.Update)
                {
                    yield return AttributeChange.CreateAttributeDelete(this.MmsAttributeName);
                }

                yield break;
            }

            IList<object> values = this.GetValuesFromArray(value);
            values = this.ConvertToNativeFimFormat(values);

            if (modType == ObjectModificationType.Add || modType == ObjectModificationType.Replace)
            {
                yield return AttributeChange.CreateAttributeAdd(this.MmsAttributeName, values);
            }
            else if (modType == ObjectModificationType.Update)
            {
                yield return AttributeChange.CreateAttributeReplace(this.MmsAttributeName, values);
            }
        }
    }
}