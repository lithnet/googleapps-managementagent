using System.Collections.Generic;
using System.Linq;
using Microsoft.MetadirectoryServices;

namespace Lithnet.GoogleApps.MA
{
    using System.Collections.ObjectModel;

    internal class MASchemaType
    {
        public string Name { get; set; }

        public List<IAttributeAdapter> AttributeAdapters { get; set; }

        public string[] AnchorAttributeNames { get; set; }

        public bool SupportsPatch { get; set; }

        public IApiInterfaceObject ApiInterface { get; set; }

        public SchemaType GetSchemaType()
        {
            SchemaType type = SchemaType.Create(this.Name, true);

            foreach (IAttributeAdapter attribute in this.AttributeAdapters)
            {
                foreach (SchemaAttribute maAttribute in attribute.GetSchemaAttributes())
                {
                    type.Attributes.Add(maAttribute);
                }
            }

            return type;
        }

        public IAttributeAdapter GetAdapterForMmsAttribute(string attributeName)
        {
            foreach (IAttributeAdapter a in this.AttributeAdapters)
            {
                foreach (string mmsName in a.MmsAttributeNames)
                {
                    if (mmsName == attributeName)
                    {
                        return a;
                    }
                }
            }

            throw new KeyNotFoundException($"There was no adapter found for the attribute {attributeName}");
        }

        public bool CanPatch(KeyedCollection<string, AttributeChange> changes)
        {
            return this.SupportsPatch && this.AttributeAdapters.All(t => t.CanPatch(changes));
        }

        public IEnumerable<string> GetFieldNames(SchemaType type, string api = null)
        {
            foreach (IAttributeAdapter attribute in this.AttributeAdapters)
            {
                foreach (string field in attribute.GetFieldNames(type, api))
                {
                    yield return field;
                }
            }
        }
    }
}
