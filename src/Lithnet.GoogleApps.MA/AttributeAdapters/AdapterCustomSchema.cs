using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.MetadirectoryServices;
using G = Google.Apis.Admin.Directory.directory_v1.Data;

namespace Lithnet.GoogleApps.MA
{
    internal class AdapterCustomSchema
    {
        public IEnumerable<string> MmsAttributeNames
        {
            get
            {
                foreach (AdapterCustomSchemaField f in this.Fields)
                {
                    yield return f.MmsAttributeName;
                }
            }
        }

        public string SchemaName { get; set; }

        public List<AdapterCustomSchemaField> Fields { get; private set; }

        public AdapterCustomSchema()
        {
            this.Fields = new List<AdapterCustomSchemaField>();
        }

        public bool UpdateField(CSEntryChange csentry, object obj)
        {
            bool changed = false;

            foreach (AdapterCustomSchemaField field in this.Fields)
            {
                changed |= field.UpdateField(csentry, obj);
            }

            return changed;
        }

        public bool CanPatch(KeyedCollection<string, AttributeChange> changes)
        {
            return this.Fields.All(t => t.CanPatch(changes));
        }

        public IEnumerable<SchemaAttribute> GetSchemaAttributes()
        {
            foreach (AdapterCustomSchemaField field in this.Fields)
            {
                foreach (SchemaAttribute attribute in field.GetSchemaAttributes())
                {
                    yield return attribute;
                }
            }
        }

        public IEnumerable<string> GetFieldNames(SchemaType type, string api)
        {
            // customSchemas/SCHEMA_NAME(field1,field2);

            List<string> names = new List<string>();

            foreach (AdapterCustomSchemaField field in this.Fields)
            {
                names.AddRange(field.GetFieldNames(type, api));
            }

            if (names.Count > 0)
            {
                yield return $"customSchemas/{this.SchemaName}({string.Join(",", names)})";
            }
        }

        public IEnumerable<AttributeChange> CreateAttributeChanges(string dn, ObjectModificationType modType, object obj)
        {
            foreach (AdapterCustomSchemaField field in this.Fields)
            {
                foreach (AttributeChange change in field.CreateAttributeChanges(dn, modType, obj))
                {
                    yield return change;
                }
            }
        }
    }
}