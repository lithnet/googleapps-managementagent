using System;
using Microsoft.MetadirectoryServices;

namespace Lithnet.GoogleApps.MA
{
    internal class AdapterSubfield
    {
        public string MmsAttributeNameSuffix { get; set; }

        public string GoogleApiFieldName { get; set; }

        public string ManagedObjectPropertyName { get; set; }

        public AttributeType AttributeType { get; set; }

        public AttributeOperation Operation { get; set; }

        public bool IsMultivalued { get; set; }

        public Func<object, object> CastForImport { get; set; }

        public Func<object, object> CastForExport { get; set; }

        public NullValueRepresentation NullValueRepresentation { get; set; }

        public string GetAttributeName(string prefix)
        {
            if (this.MmsAttributeNameSuffix == null)
            {
                return prefix;
            }
            else
            {
                return $"{prefix}_{this.MmsAttributeNameSuffix}";
            }
        }

        public SchemaAttribute GetSchemaAttribute(string prefix)
        {
            string attributeName = this.GetAttributeName(prefix);

            if (this.IsMultivalued)
            {
                return SchemaAttribute.CreateMultiValuedAttribute(attributeName, this.AttributeType, this.Operation);
            }
            else
            {
                return SchemaAttribute.CreateSingleValuedAttribute(attributeName, this.AttributeType, this.Operation);
            }
        }
    }
}
