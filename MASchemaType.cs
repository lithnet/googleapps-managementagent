using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Lithnet.GoogleApps.MA
{
    [DataContract(Name = "schema-type")]
    [KnownType(typeof(MASchemaAttribute))]
    [KnownType(typeof(MASchemaCustomTypeArray))]
    [KnownType(typeof(MASchemaNestedType))]
    public class MASchemaType
    {
        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "attributes")]
        public List<IMASchemaAttribute> Attributes { get; set; }
    }
}
