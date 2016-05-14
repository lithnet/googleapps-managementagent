using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using Microsoft.MetadirectoryServices;

namespace Lithnet.GoogleApps.MA
{
    using System.Linq.Expressions;
    using System.Reflection;
    using Google.GData.Client;
    using Logging;
    using MetadirectoryServices;

    internal class AdapterPropertyValue : IAttributeAdapter
    {
        private PropertyInfo propInfo;

        public string AttributeName { get; set; }

        public string FieldName { get; set; }

        public string PropertyName { get; set; }

        public string ParentFieldName { get; set; }

        public AttributeType AttributeType { get; set; }

        public AttributeOperation Operation { get; set; }

        public string Api { get; set; }

        public bool CanPatch { get; set; }

        public bool IsMultivalued { get; set; }

        public bool IsArrayAttribute { get; set; }

        public bool IsReadOnly => this.Operation == AttributeOperation.ImportOnly;

        public Func<object, object> CastForImport { get; set; }

        public bool UseNullPlaceHolder { get; set; }

        internal string AssignedType { get; set; }

        public bool IsAnchor { get; set; }

        public bool CanProcessAttribute(string attribute)
        {
            return this.AttributeName == attribute;
        }

        public bool UpdateField(CSEntryChange csentry, object obj)
        {
            if (this.IsReadOnly)
            {
                return false;
            }

            if (!csentry.HasAttributeChange(this.AttributeName))
            {
                return false;
            }

            if (this.propInfo == null)
            {
                this.propInfo = obj.GetType().GetProperty(this.PropertyName);
            }

            object value = csentry.GetValueAdd<object>(this.AttributeName);

            if (value == null)
            {
                if (this.propInfo.PropertyType == typeof(string) && this.UseNullPlaceHolder)
                {
                    value = Constants.NullValuePlaceholder;
                }
            }

            this.propInfo.SetValue(obj, value, null);

            Logger.WriteLine($"Updating {this.AttributeName} -> {value ?? "<null>"}");

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

            object value = this.propInfo.GetValue(obj);
            if (this.CastForImport != null)
            {
                value = this.CastForImport(value);
            }
            
            if (value == null)
            {
                if (modType == ObjectModificationType.Update)
                {
                    yield return AttributeChange.CreateAttributeDelete(this.AttributeName);
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
