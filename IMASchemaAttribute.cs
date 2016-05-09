namespace Lithnet.GoogleApps.MA
{
    using System.Collections.Generic;
    using Microsoft.MetadirectoryServices;

    public interface IMASchemaAttribute
    {
        string AttributeName { get; set; }
        string FieldName { get; set; }
        string PropertyName { get; set; }
        string Api { get; set; }
        bool CanPatch { get; set; }
        bool UpdateField<T>(CSEntryChange csentry, T obj);
        IEnumerable<AttributeChange> CreateAttributeChanges<T>(ObjectModificationType modType, T obj);
    }
}