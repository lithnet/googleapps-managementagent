using System.Collections.ObjectModel;

namespace Lithnet.GoogleApps.MA
{
    using System.Collections.Generic;
    using Microsoft.MetadirectoryServices;

    internal interface IAttributeAdapter
    {
        IEnumerable<string> MmsAttributeNames { get; }

        string FieldName { get; set; }

        string Api { get; set; }

        bool IsReadOnly { get; }

        bool IsAnchor { get; }

        bool CanProcessAttribute(string attribute);

        bool CanPatch(KeyedCollection<string, AttributeChange> changes);

        bool UpdateField(CSEntryChange csentry, object obj);

        IEnumerable<SchemaAttribute> GetSchemaAttributes();

        IEnumerable<string> GetFieldNames(SchemaType type, string api = null);

        IEnumerable<AttributeChange> CreateAttributeChanges(string dn, ObjectModificationType modType, object obj);
    }
}