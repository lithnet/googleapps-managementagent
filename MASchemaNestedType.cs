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

    [DataContract(Name = "schema-nested-type")]
    public class MASchemaNestedType : IMASchemaAttribute
    {
        private PropertyInfo propInfo;

        private IList<MASchemaAttribute> attributes;

        [DataMember(Name = "attribute-name")]
        public string AttributeName { get; set; }

        [DataMember(Name = "field-name")]
        public string FieldName { get; set; }

        [DataMember(Name = "property-name")]
        public string PropertyName { get; set; }

        [DataMember(Name = "api")]
        public string Api { get; set; }

        [DataMember(Name = "can-patch")]
        public bool CanPatch { get; set; }

        [DataMember(Name = "attributes")]
        public IList<MASchemaArrayField> Fields { get; set; }

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
            foreach (MASchemaArrayField item in this.Fields)
            {
                yield return new MASchemaAttribute
                {
                    AttributeType = item.AttributeType,
                    FieldName = item.FieldName,
                    IsArrayAttribute = true,
                    CanPatch = this.CanPatch,
                    IsMultivalued = item.IsMultivalued,
                    AttributeName = $"{this.AttributeName}_{item.AttributeNamePart}",
                    PropertyName =  item.PropertyName,
                    Operation = item.Operation,
                    ParentFieldName = this.FieldName
                };
            }
        }

        private IEnumerable<string> AttributeNames
        {
            get
            {
                yield return this.AttributeName;

                foreach (MASchemaAttribute attribute in this.Attributes)
                {
                    yield return attribute.AttributeName;
                }
            }
        }

        public bool UpdateField<T>(CSEntryChange csentry, T obj)
        {
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

            object parentObject = this.propInfo.GetValue(obj);

            if (parentObject == null)
            {
                parentObject = default(T);
            }

            foreach (var change in changes)
            {
                if (change.Item2.UpdateField(csentry, parentObject))
                {
                    hasChanged = true;
                }
            }

            return hasChanged;
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

        public IEnumerable<AttributeChange> CreateAttributeChanges<T>(ObjectModificationType modType, T obj)
        {
            if (this.propInfo == null)
            {
                this.propInfo = obj.GetType().GetProperty(this.PropertyName);
            }

            object value = this.propInfo.GetValue(obj);

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
