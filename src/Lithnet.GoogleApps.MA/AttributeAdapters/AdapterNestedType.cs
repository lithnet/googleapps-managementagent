using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.MetadirectoryServices;
using Lithnet.MetadirectoryServices;
using System.Reflection;

namespace Lithnet.GoogleApps.MA
{
    internal class AdapterNestedType : IAttributeAdapter
    {
        private PropertyInfo propInfo;

        private IList<AdapterPropertyValue> attributes;

        public string AttributeName { get; set; }

        public string FieldName { get; set; }

        public string PropertyName { get; set; }

        public string Api { get; set; }

        public bool SupportsPatch { get; set; }

        public IList<AdapterSubfield> Fields { get; set; }

        public bool IsReadOnly { get; set; }

        public bool IsAnchor => false;

        public IList<AdapterPropertyValue> Attributes
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

        private IList<AdapterPropertyValue> GetConstructedAttributes()
        {
            return this.GetAttributesWithoutType().ToList();
        }

        private IEnumerable<AdapterPropertyValue> GetAttributesWithoutType()
        {
            foreach (AdapterSubfield item in this.Fields)
            {
                yield return new AdapterPropertyValue
                {
                    AttributeType = item.AttributeType,
                    FieldName = item.FieldName,
                    IsArrayAttribute = true,
                    SupportsPatch = this.SupportsPatch,
                    IsMultivalued = item.IsMultivalued,
                    AttributeName = $"{this.AttributeName}_{item.AttributeNamePart}",
                    PropertyName = item.PropertyName,
                    Operation = item.Operation,
                    ParentFieldName = this.FieldName,
                    UseNullPlaceHolder = true
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

            IList<Tuple<AttributeChange, AdapterPropertyValue>> changes = this.GetAttributeChanges(csentry).ToList();

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

            foreach (Tuple<AttributeChange, AdapterPropertyValue> change in changes)
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
            foreach (AdapterSubfield field in this.Fields)
            {
                yield return field.GetSchemaAttribute(this.AttributeName);
            }
        }

        public IEnumerable<string> GetFieldNames(SchemaType type, string api)
        {
            if (api != null && this.Api != api)
            {
                yield break;
            }

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

        private IEnumerable<Tuple<AttributeChange, AdapterPropertyValue>> GetAttributeChanges(CSEntryChange csentry)
        {
            foreach (AdapterPropertyValue attribute in this.Attributes)
            {
                if (csentry.HasAttributeChange(attribute.AttributeName))
                {
                    yield return new Tuple<AttributeChange, AdapterPropertyValue>(csentry.AttributeChanges[attribute.AttributeName], attribute);
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

            if (value == null)
            {
                yield break;
            }

            foreach (AdapterPropertyValue attribute in this.Attributes)
            {
                foreach (AttributeChange change in attribute.CreateAttributeChanges(dn, modType, value))
                {
                    yield return change;
                }
            }
        }
    }
}
