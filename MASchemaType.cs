using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Lithnet.GoogleApps.MA
{
    [DataContract(Name = "schema-type")]
    public class MASchemaType
    {
        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "array-types")]
        public List<MASchemaArray> ArrayTypes { get; set; }

        [DataMember(Name = "attributes")]
        public List<MASchemaAttribute> Attributes { get; set; }
    }
}
