using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using Microsoft.MetadirectoryServices;

namespace Lithnet.GoogleApps.MA
{
    [DataContract(Name = "schema-array-field")]
    public class MASchemaArrayField
    {
        [DataMember(Name = "attribute-name")]
        public string AttributeNamePart { get; set; }

        [DataMember(Name = "field-name")]
        public string FieldName { get; set; }

        [DataMember(Name = "property-name")]
        public string PropertyName { get; set; }

        [DataMember(Name = "attribute-type")]
        public AttributeType AttributeType { get; set; }

        [DataMember(Name = "operation")]
        public AttributeOperation Operation { get; set; }
        
        [DataMember(Name = "is-multivalued")]
        public bool IsMultivalued { get; set; }
    }
}
