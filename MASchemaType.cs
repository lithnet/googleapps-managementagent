using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using Microsoft.MetadirectoryServices;

namespace Lithnet.GoogleApps.MA
{
    [DataContract(Name = "schema-type")]
    [KnownType(typeof(MASchemaAttribute))]
    [KnownType(typeof(MASchemaCustomTypeArray))]
    [KnownType(typeof(MASchemaNestedType))]
    [KnownType(typeof(MASchemaSimpleList))]
    public class MASchemaType
    {
        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "attributes")]
        public List<IMASchemaAttribute> Attributes { get; set; }

        [DataMember(Name = "anchor-name")]
        public string AnchorAttributeName { get; set; }

        [DataMember(Name = "can-patch")]
        public bool CanPatch { get; set; }

        public ApiInterface ApiInterface { get; set; }
    }
}
