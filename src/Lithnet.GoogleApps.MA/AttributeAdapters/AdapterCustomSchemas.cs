using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.MetadirectoryServices;

namespace Lithnet.GoogleApps.MA
{
    internal class AdapterCustomSchemas : IAttributeAdapter
    {
        public IEnumerable<string> MmsAttributeNames
        {
            get
            {
                foreach (AdapterCustomSchema s in this.CustomSchemas)
                {
                    foreach (string value in s.MmsAttributeNames)
                    {
                        yield return value;
                    }
                }
            }
        }

        public string FieldName { get; set; }

        public string PropertyName { get; set; }

        public string Api { get; set; }

        public bool IsReadOnly => false;

        public bool IsAnchor { get; set; }

        public bool CanProcessAttribute(string attribute)
        {
            return this.MmsAttributeNames.Any(t => t == attribute);
        }

        public bool CanPatch(KeyedCollection<string, AttributeChange> changes)
        {
            return this.CustomSchemas.All(t => t.CanPatch(changes));
        }

        public List<AdapterCustomSchema> CustomSchemas { get; private set; }

        public AdapterCustomSchemas()
        {
            this.CustomSchemas = new List<AdapterCustomSchema>();
        }

        public bool UpdateField(CSEntryChange csentry, object obj)
        {
            bool changed = false;

            foreach (AdapterCustomSchema customSchema in this.CustomSchemas)
            {
                changed |= customSchema.UpdateField(csentry, obj);
            }

            return changed;
        }

        public IEnumerable<SchemaAttribute> GetSchemaAttributes()
        {
            foreach (AdapterCustomSchema customSchema in this.CustomSchemas)
            {
                foreach (SchemaAttribute s in customSchema.GetSchemaAttributes())
                {
                    yield return s;
                }
            }
        }

        public IEnumerable<string> GetFieldNames(SchemaType type, string api)
        {
            // customSchemas/SCHEMA_NAME(field1,field2);

            List<string> names = new List<string>();

            foreach (AdapterCustomSchema s in this.CustomSchemas)
            {
                names.AddRange(s.GetFieldNames(type, api));
            }

            if (names.Count > 0)
            {
                yield return string.Join(",", names);
            }
        }

        public IEnumerable<AttributeChange> CreateAttributeChanges(string dn, ObjectModificationType modType, object obj)
        {
            foreach (AdapterCustomSchema s in this.CustomSchemas)
            {
                foreach (AttributeChange c in s.CreateAttributeChanges(dn, modType, obj))
                {
                    yield return c;
                }
            }
        }
    }
}