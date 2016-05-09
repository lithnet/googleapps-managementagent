using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace Lithnet.GoogleApps.MA
{
    using System.Runtime.Serialization;

    [CollectionDataContract(Name = "schema")]
    public class MASchemaTypes : KeyedCollection<string, MASchemaType>
    {
        /// <summary>When implemented in a derived class, extracts the key from the specified element.</summary>
        /// <returns>The key for the specified element.</returns>
        /// <param name="item">The element from which to extract the key.</param>
        protected override string GetKeyForItem(MASchemaType item)
        {
            return item.Name;
        }
    }
}
