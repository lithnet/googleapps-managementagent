using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reflection;
using Lithnet.Logging;
using Lithnet.MetadirectoryServices;
using Microsoft.MetadirectoryServices;

namespace Lithnet.GoogleApps.MA
{
    internal class AdapterPropertyValue : IAttributeAdapter
    {
        private PropertyInfo propInfo;

        public IEnumerable<string> MmsAttributeNames
        {
            get
            {
                yield return this.AttributeName;
            }
        }

        public string AttributeName { get; set; }

        public string FieldName { get; set; }

        public string PropertyName { get; set; }

        public string ParentFieldName { get; set; }

        public AttributeType AttributeType { get; set; }

        public AttributeOperation Operation { get; set; }

        public string Api { get; set; }

        public bool SupportsPatch { get; set; }

        public bool IsMultivalued { get; set; }

        public bool IsReadOnly => this.Operation == AttributeOperation.ImportOnly;

        public Func<object, object> CastForImport { get; set; }

        public Func<object, object> CastForExport { get; set; }

        public NullValueRepresentation NullValueRepresentation { get; set; }

        internal string AssignedType { get; set; }

        public bool IsAnchor { get; set; }

        public bool CanProcessAttribute(string attribute)
        {
            return this.AttributeName == attribute;
        }

        public bool CanPatch(KeyedCollection<string, AttributeChange> changes)
        {
            return this.SupportsPatch || !changes.Contains(this.AttributeName);
        }

        public bool UpdateField(CSEntryChange csentry, object obj)
        {
            if (this.IsReadOnly)
            {
                return false;
            }

            if (!csentry.HasAttributeChange(this.AttributeName))
            {
                Trace.WriteLine($"Skipping update of field {this.FieldName} because attribute {this.AttributeName} is not present in the CSEntryChange");
                return false;
            }

            if (this.propInfo == null)
            {
                this.propInfo = obj.GetType().GetProperty(this.PropertyName);
            }

            if (this.propInfo == null)
            {
                throw new InvalidOperationException($"The property {this.PropertyName} was not found on the object of type {obj.GetType().FullName}");
            }

            object value = csentry.GetValueAdd<object>(this.AttributeName);

            if (this.CastForExport != null)
            {
                value = this.CastForExport(value);
            }

            value = Utilities.SetPlaceholderIfNull(value, this.NullValueRepresentation);

            this.propInfo.SetValue(obj, value, null);

            Logger.WriteLine($"Updating {this.AttributeName} -> {value.ToSmartStringOrNull() ?? "<null>"}");

            return true;
        }

        public IEnumerable<SchemaAttribute> GetSchemaAttributes()
        {
            if (this.IsAnchor)
            {
                yield return SchemaAttribute.CreateAnchorAttribute(this.AttributeName, this.AttributeType, this.Operation);
            }
            else
            {
                if (this.IsMultivalued)
                {
                    yield return SchemaAttribute.CreateMultiValuedAttribute(this.AttributeName, this.AttributeType, this.Operation);
                }
                else
                {
                    yield return SchemaAttribute.CreateSingleValuedAttribute(this.AttributeName, this.AttributeType, this.Operation);
                }
            }
        }

        public IEnumerable<string> GetFieldNames(SchemaType type, string api)
        {
            if (api != null && this.Api != api)
            {
                yield break;
            }

            if (this.FieldName != null)
            {
                if (type.HasAttribute(this.AttributeName))
                {
                    yield return this.FieldName;
                }
            }
        }

        public IEnumerable<AttributeChange> CreateAttributeChanges(string dn, ObjectModificationType modType, object obj)
        {
            if (this.propInfo == null)
            {
                this.propInfo = obj.GetType().GetProperty(this.PropertyName);
            }

            if (this.propInfo == null)
            {
                throw new InvalidOperationException($"The property {this.PropertyName} was not found on the object of type {obj.GetType().FullName}");
            }

            object value = this.propInfo.GetValue(obj);

            if (this.CastForImport != null)
            {
                value = this.CastForImport(value);
            }

            value = Utilities.GetNullIfPlaceholder(value, this.NullValueRepresentation);

            if (value == null)
            {
                if (modType == ObjectModificationType.Update)
                {
                    yield return AttributeChange.CreateAttributeDelete(this.AttributeName);
                    yield break;
                }
                else
                {
                    yield break;
                }
            }

            switch (modType)
            {
                case ObjectModificationType.Add:
                case ObjectModificationType.Replace:
                    yield return AttributeChange.CreateAttributeAdd(this.AttributeName, TypeConverter.ConvertData(value, this.AttributeType));
                    break;

                case ObjectModificationType.Update:
                    yield return AttributeChange.CreateAttributeReplace(this.AttributeName, TypeConverter.ConvertData(value, this.AttributeType));
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(modType), modType, null);
            }
        }
    }
}
