using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.MetadirectoryServices;

namespace Lithnet.GoogleApps.MA
{
    internal class AdapterPlaceholder : IAttributeAdapter
    {
        public IEnumerable<string> MmsAttributeNames
        {
            get
            {
                yield return this.AttributeName;
            }
        }

        public string AttributeName { get; set; }

        public string FieldName { get; set; }
        
        public string Api { get; set; }
        
        public bool IsMultivalued { get; set; }

        public AttributeType AttributeType { get; set; }

        public AttributeOperation Operation { get; set; }

        public bool IsReadOnly => this.Operation == AttributeOperation.ImportOnly;

        public bool IsAnchor => false;

        public bool CanProcessAttribute(string attribute)
        {
            throw new NotImplementedException();
        }

        public bool CanPatch(KeyedCollection<string, AttributeChange> changes)
        {
            throw new NotImplementedException();
        }

        public bool UpdateField(CSEntryChange csentry, object obj)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<SchemaAttribute> GetSchemaAttributes()
        {
            if (this.IsMultivalued)
            {
                yield return SchemaAttribute.CreateMultiValuedAttribute(this.AttributeName, this.AttributeType, this.Operation);
            }
            else
            {
                yield return SchemaAttribute.CreateSingleValuedAttribute(this.AttributeName, this.AttributeType, this.Operation);
            }
        }

        public IEnumerable<string> GetFieldNames(SchemaType type, string api)
        {
            if (api != null && this.Api != api)
            {
                yield break;
            }

            if (this.FieldName != null)
            {
                if (type.HasAttribute(this.AttributeName))
                {
                    yield return this.FieldName;
                }
            }
        }

        public IEnumerable<AttributeChange> CreateAttributeChanges(string dn, ObjectModificationType modType, object obj)
        {
            throw new NotImplementedException();
        }
    }
}