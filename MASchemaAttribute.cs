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
    using Logging;
    using MetadirectoryServices;

    [DataContract(Name = "schema-attribute")]
    public class MASchemaAttribute : IMASchemaAttribute
    {
        private PropertyInfo propInfo;

        [DataMember(Name = "attribute-name")]
        public string AttributeName { get; set; }

        [DataMember(Name = "field-name")]
        public string FieldName { get; set; }

        [DataMember(Name = "property-name")]
        public string PropertyName { get; set; }

        //[DataMember(Name = "parent-field-name")]
        public string ParentFieldName { get; set; }

        [DataMember(Name = "attribute-type")]
        public AttributeType AttributeType { get; set; }

        [DataMember(Name = "operation")]
        public AttributeOperation Operation { get; set; }

        [DataMember(Name = "api")]
        public string Api { get; set; }

        [DataMember(Name = "can-patch")]
        public bool CanPatch { get; set; }

        [DataMember(Name = "is-multivalued")]
        public bool IsMultivalued { get; set; }

        [DataMember(Name = "is-array-attribute")]
        public bool IsArrayAttribute { get; set; }

        [DataMember(Name = "is-read-only")]
        public bool IsReadOnly { get; set; }

        internal string AssignedType { get; set; }

        public bool UpdateField<T>(CSEntryChange csentry, T obj)
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
                if (this.propInfo.PropertyType == typeof(string))
                {
                    value = Constants.NullValuePlaceholder;
                }
            }

            this.propInfo.SetValue(obj, value, null);

            Logger.WriteLine($"Updating {this.AttributeName} -> {value}");

            return true;
        }

        public AttributeChange GetAttributeChange(CSEntryChange csentry)
        {
            return csentry.AttributeChanges.FirstOrDefault(t => t.Name == this.AttributeName);
        }

        public IEnumerable<AttributeChange> CreateAttributeChanges<T>(ObjectModificationType modType, T obj)
        {
            if (this.propInfo == null)
            {
                this.propInfo = obj.GetType().GetProperty(this.PropertyName);
            }

            object value = this.propInfo.GetValue(obj);

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
                    yield return AttributeChange.CreateAttributeAdd(this.AttributeName, value);
                    break;

                case ObjectModificationType.Update:
                    yield return AttributeChange.CreateAttributeReplace(this.AttributeName, value);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(modType), modType, null);
            }
        }
    }
}
