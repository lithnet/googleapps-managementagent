using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using Microsoft.MetadirectoryServices;
using Lithnet.MetadirectoryServices;

namespace Lithnet.GoogleApps.MA
{
    using Logging;
    using System.Reflection;

    internal class MASchemaNestedType : IMASchemaAttribute
    {
        private PropertyInfo propInfo;

        private IList<MASchemaAttribute> attributes;

        public string AttributeName { get; set; }

        public string FieldName { get; set; }

        public string PropertyName { get; set; }

        public string Api { get; set; }

        public bool CanPatch { get; set; }

        public IList<MASchemaField> Fields { get; set; }

        public bool IsReadOnly { get; set; }

        public IList<MASchemaAttribute> Attributes
        {
            get
            {
                if (this.attributes == null)
                {
                    this.attributes = this.GetConstructedAttributes();
                }

                return this.attributes;
            }
        }

        private IList<MASchemaAttribute> GetConstructedAttributes()
        {
            return this.GetAttributesWithoutType().ToList();
        }

        private IEnumerable<MASchemaAttribute> GetAttributesWithoutType()
        {
            foreach (MASchemaField item in this.Fields)
            {
                yield return new MASchemaAttribute
                {
                    AttributeType = item.AttributeType,
                    FieldName = item.FieldName,
                    IsArrayAttribute = true,
                    CanPatch = this.CanPatch,
                    IsMultivalued = item.IsMultivalued,
                    AttributeName = $"{this.AttributeName}_{item.AttributeNamePart}",
                    PropertyName = item.PropertyName,
                    Operation = item.Operation,
                    ParentFieldName = this.FieldName
                };
            }
        }

        public bool CanProcessAttribute(string attribute)
        {
            return this.AttributeName == attribute || this.Attributes.Any(t => t.AttributeName == attribute);
        }

        public bool UpdateField(CSEntryChange csentry, object obj)
        {
            if (this.IsReadOnly)
            {
                return false;
            }

            bool hasChanged = false;

            IList<Tuple<AttributeChange, MASchemaAttribute>> changes = this.GetAttributeChanges(csentry).ToList();

            if (changes.Count == 0)
            {
                return false;
            }

            if (this.propInfo == null)
            {
                this.propInfo = obj.GetType().GetProperty(this.PropertyName);
            }

            object childObject = this.propInfo.GetValue(obj);
            bool created = false;
            if (childObject == null)
            {
                childObject = Activator.CreateInstance(this.propInfo.PropertyType, new object[] { });
                created = true;
            }

            foreach (Tuple<AttributeChange, MASchemaAttribute> change in changes)
            {
                if (change.Item2.UpdateField(csentry, childObject))
                {
                    hasChanged = true;
                }
            }

            if (hasChanged && created)
            {
                this.propInfo.SetValue(obj, childObject, null);
            }

            return hasChanged;
        }

        public IEnumerable<SchemaAttribute> GetSchemaAttributes()
        {
            foreach (MASchemaField field in this.Fields)
            {
                yield return field.GetSchemaAttribute(this.AttributeName);
            }
        }

        public IEnumerable<string> GetFieldNames(SchemaType type)
        {
            if (this.FieldName == null)
            {
                yield break;
            }

            string childFields = string.Join(",", this.Attributes.Where(t => t.FieldName != null && type.HasAttribute(t.AttributeName)).Select(t => t.FieldName));

            if (!string.IsNullOrWhiteSpace(childFields))
            {
                yield return $"{this.FieldName}({childFields})";
            }
        }

        private IEnumerable<Tuple<AttributeChange, MASchemaAttribute>> GetAttributeChanges(CSEntryChange csentry)
        {
            foreach (MASchemaAttribute attribute in this.Attributes)
            {
                if (csentry.HasAttributeChange(attribute.AttributeName))
                {
                    yield return new Tuple<AttributeChange, MASchemaAttribute>(csentry.AttributeChanges[attribute.AttributeName], attribute);
                }
            }
        }

        public IEnumerable<AttributeChange> CreateAttributeChanges(ObjectModificationType modType, object obj)
        {
            if (this.propInfo == null)
            {
                this.propInfo = obj.GetType().GetProperty(this.PropertyName);
            }

            object value = this.propInfo.GetValue(obj);

            if (value == null)
            {
                yield break;
            }

            foreach (MASchemaAttribute attribute in this.Attributes)
            {
                foreach (AttributeChange change in attribute.CreateAttributeChanges(modType, value))
                {
                    yield return change;
                }
            }
        }
    }
}
