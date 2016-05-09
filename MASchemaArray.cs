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

    [DataContract(Name = "schema-array")]
    public class MASchemaArray : IMASchemaAttribute
    {
        private PropertyInfo propInfo;

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

        [DataMember(Name = "known-types")]
        public IList<string> KnownTypes { get; set; }

        [DataMember(Name = "array-type")]
        public ArrayType ArrayType { get; set; }

        [DataMember(Name = "attributes")]
        public IList<MASchemaArrayField> Fields { get; set; }

        private IList<MASchemaAttribute> attributes;

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
            if (this.ArrayType.HasFlag(ArrayType.HasTypes))
            {
                return this.GetFlattenedKnownTypes().ToList();
            }
            else
            {
                return this.GetAttributesWithoutType().ToList();
            }
        }

        private IEnumerable<MASchemaAttribute> GetFlattenedKnownTypes()
        {
            if (!this.ArrayType.HasFlag(ArrayType.HasTypes))
            {
                throw new InvalidOperationException();
            }

            foreach (string type in this.KnownTypes)
            {
                if (this.ArrayType.HasFlag(ArrayType.HasComplexFields))
                {
                    foreach (MASchemaAttribute maSchemaAttribute in this.GetAttributesOfType(type))
                    {
                        yield return maSchemaAttribute;
                    }
                }
                else
                {
                    yield return this.GetSingleValueAttribute(type);
                }
            }
        }

        private MASchemaAttribute GetSingleValueAttribute(string type)
        {
            if (this.Fields.Count != 1)
            {
                throw new InvalidOperationException();
            }

            MASchemaArrayField source = this.Fields.First();

            return new MASchemaAttribute
            {
                Api = this.Api,
                AttributeType = AttributeType.String,
                FieldName = source.FieldName,
                IsArrayAttribute = true,
                CanPatch = this.CanPatch,
                IsMultivalued = source.IsMultivalued,
                AttributeName = $"{this.AttributeName}_{type}",
                Operation = AttributeOperation.ImportExport,
                ParentFieldName = this.FieldName,
                AssignedType = type
            };
        }

        private IEnumerable<MASchemaAttribute> GetAttributesOfType(string type)
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
                    AttributeName = $"{this.AttributeName}_{type}_{item.AttributeNamePart}",
                    Operation = item.Operation,
                    ParentFieldName = this.FieldName,
                    PropertyName =  item.PropertyName,
                    AssignedType = type
                };
            }
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
                    Operation = item.Operation,
                    ParentFieldName = this.FieldName,
                    PropertyName =  item.PropertyName
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

            foreach (MASchemaAttribute attribute in this.Attributes)
            {
                if (csentry.HasAttributeChange(attribute.AttributeName))
                {
                    if (this.ArrayType.HasFlag(ArrayType.HasTypes))
                    {

                    }
                    else
                    {

                    }


                }
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

        public IEnumerable<AttributeChange> CreateAttributeChanges<T>(ObjectModificationType modType, T obj)
        {
            throw new NotImplementedException();
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

        private IEnumerable<IGrouping<string, Tuple<AttributeChange, MASchemaAttribute>>> GetAttributeChangesByType(CSEntryChange csentry)
        {
            return this.GetAttributeChanges(csentry).GroupBy(t => t.Item2.AssignedType);
        }

        public AttributeChange CreateAttributeChange<T>(ObjectModificationType modType, T obj)
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
                    return AttributeChange.CreateAttributeDelete(this.AttributeName);
                }
                else
                {
                    return null;
                }
            }

            switch (modType)
            {
                case ObjectModificationType.Add:
                case ObjectModificationType.Replace:
                    return AttributeChange.CreateAttributeAdd(this.AttributeName, value);

                case ObjectModificationType.Update:
                    return AttributeChange.CreateAttributeReplace(this.AttributeName, value);

                default:
                    throw new ArgumentOutOfRangeException(nameof(modType), modType, null);
            }
        }
    }
}
