using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Google.Apis.Admin.Directory.directory_v1.Data;
using Microsoft.MetadirectoryServices;
using Newtonsoft.Json.Linq;
using User = Lithnet.GoogleApps.ManagedObjects.User;

namespace Lithnet.GoogleApps.MA
{
    internal abstract class AdapterCustomSchemaField
    {
        public string MmsAttributeName { get; set; }

        public string GoogleApiFieldName { get; set; }

        public string PropertyName { get; set; }

        public string SchemaName { get; set; }

        public AttributeType AttributeType { get; set; }

        public AttributeOperation Operation { get; set; }

        public string Api { get; set; }

        public bool SupportsPatch => !this.IsMultivalued;

        public abstract bool IsMultivalued { get; }

        public bool IsReadOnly => this.Operation == AttributeOperation.ImportOnly;

        public NullValueRepresentation NullValueRepresentation { get; set; }

        internal SchemaFieldSpec FieldSpec { get; set; }

        public bool IsAnchor { get; set; }

        protected IDictionary<string, object> GetOrCreateSchema(User user, out bool created)
        {
            IDictionary<string, object> schema;

            if (user.CustomSchemas == null)
            {
                schema = new Dictionary<string, object>();
                user.CustomSchemas = new Dictionary<string, IDictionary<string, object>>();
                user.CustomSchemas.Add(this.SchemaName, schema);
                created = true;
            }
            else if (!user.CustomSchemas.ContainsKey(this.SchemaName))
            {
                schema = new Dictionary<string, object>();
                user.CustomSchemas.Add(this.SchemaName, schema);
                created = true;
            }
            else
            {
                schema = user.CustomSchemas[this.SchemaName];
                created = false;
            }

            return schema;
        }

        protected bool HasSchema(User user)
        {
            return user.CustomSchemas != null && user.CustomSchemas.ContainsKey(this.SchemaName);
        }

        protected bool HasSchemaField(User user)
        {
            return user.CustomSchemas != null && user.CustomSchemas.ContainsKey(this.SchemaName) && user.CustomSchemas[this.SchemaName].ContainsKey(this.GoogleApiFieldName);
        }

        public abstract bool UpdateField(CSEntryChange csentry, object obj);

        public IEnumerable<SchemaAttribute> GetSchemaAttributes()
        {
            if (this.IsAnchor)
            {
                yield return SchemaAttribute.CreateAnchorAttribute(this.MmsAttributeName, this.AttributeType, this.Operation);
            }
            else
            {
                if (this.IsMultivalued)
                {
                    yield return SchemaAttribute.CreateMultiValuedAttribute(this.MmsAttributeName, this.AttributeType, this.Operation);
                }
                else
                {
                    yield return SchemaAttribute.CreateSingleValuedAttribute(this.MmsAttributeName, this.AttributeType, this.Operation);
                }
            }
        }

        public IEnumerable<string> GetFieldNames(SchemaType type, string api)
        {
            if (this.GoogleApiFieldName != null)
            {
                if (type.HasAttribute(this.MmsAttributeName))
                {
                    yield return this.GoogleApiFieldName;
                }
            }
        }
        public bool CanPatch(KeyedCollection<string, AttributeChange> changes)
        {
            return this.SupportsPatch || !changes.Contains(this.MmsAttributeName);
        }

        public abstract IEnumerable<AttributeChange> CreateAttributeChanges(string dn, ObjectModificationType modType, object obj);

        protected object ConvertToNativeGoogleFormat(object value)
        {
            if (this.FieldSpec.FieldType.Equals("double", StringComparison.OrdinalIgnoreCase))
            {
                return double.Parse(value.ToString());
            }

            if (this.FieldSpec.FieldType.Equals("int64", StringComparison.OrdinalIgnoreCase))
            {
                return long.Parse(value.ToString());
            }

            return value;
        }

        protected IList<object> ConvertToNativeFimFormat(IEnumerable<object> values)
        {
            List<object> newList = new List<object>();

            foreach (object value in values)
            {
                newList.Add(this.ConvertToNativeFimFormat(value));
            }

            return newList;
        }

        protected IList<object> ConvertToNativeGoogleFormat(IEnumerable<object> values)
        {
            List<object> newList = new List<object>();

            foreach (object value in values)
            {
                newList.Add(this.ConvertToNativeGoogleFormat(value));
            }

            return newList;
        }

        protected object ConvertToNativeFimFormat(object value)
        {
            if (this.FieldSpec.FieldType.Equals("double", StringComparison.OrdinalIgnoreCase))
            {
                if (value is double d)
                {
                    return d.ToString("R");
                }
            }

            if (this.FieldSpec.FieldType.Equals("int64", StringComparison.OrdinalIgnoreCase))
            {
                if (value is long d)
                {
                    return d.ToString();
                }
            }

            return value;
        }

        protected IList<object> GetValuesFromArray(object value, string key)
        {
            if (value is JArray jarray)
            {
                return this.GetValuesFromJArray(jarray, key);
            }

            if (value is IList list)
            {
                return this.GetValuesFromList(list, key);
            }

            throw new NotSupportedException("The array type was unknown");
        }

        protected IList<object> GetValuesFromList(IList list, string key)
        {
            List<object> newList = new List<object>();

            if (list is null)
            {
                return newList;
            }

            foreach (object item in list)
            {
                if (item is IDictionary<string, object> d)
                {
                    if (d.ContainsKey(key))
                    {
                        newList.Add(d[key]);
                    }
                }
            }

            return newList;
        }

        protected IList<object> GetValuesFromJArray(JArray jarray, string key)
        {
            List<object> newList = new List<object>();

            if (jarray is null)
            {
                return newList;
            }

            foreach (JToken i in jarray.Children())
            {
                JEnumerable<JProperty> itemProperties = i.Children<JProperty>();

                foreach (JProperty myElement in itemProperties.Where(x => x.Name == key))
                {
                    newList.Add((string)myElement.Value);
                }
            }

            return newList;
        }
    }
}
