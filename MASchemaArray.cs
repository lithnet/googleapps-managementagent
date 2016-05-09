using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using Microsoft.MetadirectoryServices;
namespace Lithnet.GoogleApps.MA
{
    [DataContract(Name = "schema-array")]
    public class MASchemaArray
    {
        [DataMember(Name = "attribute-name")]
        public string AttributeName { get; set; }

        [DataMember(Name = "field-name")]
        public string FieldName { get; set; }

        [DataMember(Name = "api")]
        public string Api { get; set; }

        [DataMember(Name = "is-full-update-required")]
        public bool IsFullUpdateRequired { get; set; }

        public GoogleArrayMode ArrayMode { get; private set; }

        [DataMember(Name = "known-types")]
        public IList<string> KnownTypes { get; set; }

        [DataMember(Name = "array-type")]
        public ArrayType ArrayType { get; set; }

        [DataMember(Name = "attributes")]
        public IList<MASchemaArrayField> Attributes { get; set; }

        public IEnumerable<MASchemaAttribute> GetConstructedAttributes(GoogleArrayMode arrayMode)
        {
            this.ArrayMode = arrayMode;

            switch (arrayMode)
            {
                case GoogleArrayMode.FlattenValues:
                    return this.GetAttributesWithoutType();

                case GoogleArrayMode.FlattenKnownTypes:
                    return this.GetFlattenedKnownTypes();

                case GoogleArrayMode.Json:
                    return this.GetJsonValues();

                case GoogleArrayMode.PrimaryValueOnly:
                    return this.GetPrimaryValue();

                default:
                    break;
            }

            return new List<MASchemaAttribute>();
        }

        private IEnumerable<MASchemaAttribute> GetJsonValues()
        {
            yield return new MASchemaAttribute()
            {
                Api = this.Api,
                AttributeName = this.AttributeName,
                AttributeType = AttributeType.String,
                FieldName = this.FieldName,
                IsArrayAttribute = true,
                IsFullUpdateRequired = false,
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                ParentFieldName = this.FieldName
            };
        }

        private IEnumerable<MASchemaAttribute> GetPrimaryValue()
        {
            if (!this.ArrayType.HasFlag(ArrayType.HasPrimaryTypes))
            {
                throw new InvalidOperationException();
            }

            string type = "primary";

            if (this.ArrayType.HasFlag(ArrayType.HasComplexFields))
            {
                foreach (MASchemaAttribute maSchemaAttribute in this.GetAttributesOfType(type))
                {
                    yield return maSchemaAttribute;
                }

                yield return this.GetPrimaryTypeAttribute(type);
            }
            else
            {
                yield return this.GetPrimaryTypeAttribute(type);
                yield return this.GetSingleValueAttribute(type);
            }
        }

        private IEnumerable<MASchemaAttribute> GetFlattenedKnownTypes()
        {
            if (!this.ArrayType.HasFlag(ArrayType.HasTypes))
            {
                throw new InvalidOperationException();
            }

            if (this.ArrayType.HasFlag(ArrayType.HasPrimaryTypes))
            {
                foreach (MASchemaAttribute attribute in this.GetPrimaryValue())
                {
                    yield return attribute;
                }
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
            if (this.Attributes.Count != 1)
            {
                throw new InvalidOperationException();
            }

            MASchemaArrayField source = this.Attributes.First();

            return new MASchemaAttribute
            {
                Api = this.Api,
                AttributeType = AttributeType.String,
                FieldName = source.FieldName,
                IsArrayAttribute = true,
                IsFullUpdateRequired = this.IsFullUpdateRequired,
                IsMultivalued = source.IsMultivalued,
                AttributeName = $"{this.AttributeName}_{type}",
                Operation = AttributeOperation.ImportExport,
                ParentFieldName = this.FieldName
            };
        }

        private MASchemaAttribute GetPrimaryTypeAttribute(string type)
        {
            return new MASchemaAttribute
            {
                Api = this.Api,
                AttributeType = AttributeType.String,
                FieldName = "type",
                IsArrayAttribute = true,
                IsFullUpdateRequired = this.IsFullUpdateRequired,
                IsMultivalued = false,
                AttributeName = $"{this.AttributeName}_primary_type",
                Operation = AttributeOperation.ImportExport,
                ParentFieldName = this.FieldName
            };
        }

        private IEnumerable<MASchemaAttribute> GetAttributesOfType(string type)
        {
            foreach (MASchemaArrayField item in this.Attributes)
            {
                yield return new MASchemaAttribute
                {
                    AttributeType = item.AttributeType,
                    FieldName = item.FieldName,
                    IsArrayAttribute = true,
                    IsFullUpdateRequired = this.IsFullUpdateRequired,
                    IsMultivalued = item.IsMultivalued,
                    AttributeName = $"{this.AttributeName}_{type}_{item.AttributeNamePart}",
                    Operation = item.Operation,
                    ParentFieldName = this.FieldName
                };
            }
        }

        private IEnumerable<MASchemaAttribute> GetAttributesWithoutType()
        {
            foreach (MASchemaArrayField item in this.Attributes)
            {
                yield return new MASchemaAttribute
                {
                    AttributeType = item.AttributeType,
                    FieldName = item.FieldName,
                    IsArrayAttribute = true,
                    IsFullUpdateRequired = this.IsFullUpdateRequired ,
                    IsMultivalued = item.IsMultivalued,
                    AttributeName = $"{this.AttributeName}_{item.AttributeNamePart}",
                    Operation = item.Operation,
                    ParentFieldName = this.FieldName
                };
            }
        }
    }
}
