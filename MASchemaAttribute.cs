using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using Microsoft.MetadirectoryServices;

namespace Lithnet.GoogleApps.MA
{
    [DataContract(Name = "schema-attribute")]
    public class MASchemaAttribute
    {
        [DataMember(Name = "attribute-name")]
        public string AttributeName { get; set; }

        [DataMember(Name = "field-name")]
        public string FieldName { get; set; }

        [DataMember(Name = "parent-field-name")]
        public string ParentFieldName { get; set; }

        [DataMember(Name = "attribute-type")]
        public AttributeType AttributeType { get; set; }

        [DataMember(Name = "operation")]
        public AttributeOperation Operation { get; set; }

        [DataMember(Name = "api")]
        public string Api { get; set; }

        [DataMember(Name = "is-full-update-required")]
        public bool IsFullUpdateRequired { get; set; }

        [DataMember(Name = "is-multivalued")]
        public bool IsMultivalued { get; set; }

        [DataMember(Name = "is-array-attribute")]
        public bool IsArrayAttribute { get; set; }
    }
}
