using System.Collections.ObjectModel;

namespace Lithnet.GoogleApps.MA
{
    using System.Runtime.Serialization;
    using Microsoft.MetadirectoryServices;

    internal class MASchemaTypes : KeyedCollection<string, MASchemaType>
    {
        /// <summary>When implemented in a derived class, extracts the key from the specified element.</summary>
        /// <returns>The key for the specified element.</returns>
        /// <param name="item">The element from which to extract the key.</param>
        protected override string GetKeyForItem(MASchemaType item)
        {
            return item.Name;
        }

        public Schema GetSchema()
        {
            Schema schema = Schema.Create();

            foreach (MASchemaType type in this)
            {
                schema.Types.Add(type.GetSchemaType());
            }

            return schema;
        }
    }
}
