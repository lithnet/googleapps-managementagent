using System;
using System.Collections.Generic;
using Lithnet.Logging;
using Lithnet.MetadirectoryServices;
using Microsoft.MetadirectoryServices;
using User = Lithnet.GoogleApps.ManagedObjects.User;

namespace Lithnet.GoogleApps.MA
{
    internal class AdapterCustomSchemaSingleValuedField : AdapterCustomSchemaField
    {
        public override bool IsMultivalued => false;

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
            object value = csentry.GetValueAdd<object>(this.MmsAttributeName);

            User user = obj as User;

            if (user == null)
            {
                throw new NotSupportedException("The provided object was not of a 'user' type");
            }

            if (value == null && !this.HasSchemaField(user))
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

            if (value == null)
            {
                value = Utilities.GetNullRepresentation(this.NullValueRepresentation);
            }
            else
            {
                value = this.ConvertToNativeGoogleFormat(value);
            }

            schema[this.FieldName] = value;
            Logger.WriteLine($"Set {this.MmsAttributeName} -> {value ?? "<null>"}");

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

            value = this.ConvertToNativeFimFormat(value);

            if (modType == ObjectModificationType.Add || modType == ObjectModificationType.Replace)
            {
                yield return AttributeChange.CreateAttributeAdd(this.MmsAttributeName, TypeConverter.ConvertData(value, this.AttributeType));
            }
            else if (modType == ObjectModificationType.Update)
            {
                yield return AttributeChange.CreateAttributeReplace(this.MmsAttributeName, TypeConverter.ConvertData(value, this.AttributeType));
            }
        }
    }
}
