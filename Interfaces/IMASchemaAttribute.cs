namespace Lithnet.GoogleApps.MA
{
    using System.Collections.Generic;
    using Microsoft.MetadirectoryServices;

    internal interface IMASchemaAttribute
    {
        string AttributeName { get; set; }

        string FieldName { get; set; }

        string PropertyName { get; set; }

        string Api { get; set; }

        bool CanPatch { get; set; }

        bool IsReadOnly { get; }

        bool CanProcessAttribute(string attribute);

        bool UpdateField(CSEntryChange csentry, object obj);

        IEnumerable<SchemaAttribute> GetSchemaAttributes();

        IEnumerable<string> GetFieldNames(SchemaType type);

        IEnumerable<AttributeChange> CreateAttributeChanges(ObjectModificationType modType, object obj);
    }
}