using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using Google.GData.Extensions;
using Lithnet.MetadirectoryServices;
using Microsoft.MetadirectoryServices;

namespace Lithnet.GoogleApps.MA
{
    internal class AdapterGDataSimpleAttributeList<T> : IAttributeAdapter where T : SimpleAttribute, new()
    {
        private PropertyInfo propInfo;

        public IEnumerable<string> MmsAttributeNames
        {
            get
            {
                yield return this.AttributeName;
            }
        }

        public string AttributeName { get; set; }

        public string FieldName { get; set; }

        public string PropertyName { get; set; }

        public string Api { get; set; }

        public bool SupportsPatch { get; set; }

        public IList<string> KnownTypes { get; set; }

        public IList<AdapterSubfield> Fields { get; set; }

        public HashSet<string> KnownRels { get; set; }

        public bool IsReadOnly { get; set; }

        public bool IsAnchor => false;

        private IList<AdapterPropertyValue> attributes;

        public IList<AdapterPropertyValue> Attributes
        {
            get
            {
                if (this.attributes == null)
                {
                    this.attributes = this.GetConstructedAttributes();
                }

                return this.attributes;
            }
        }

        private IList<AdapterPropertyValue> GetConstructedAttributes()
        {
            return this.GetFlattenedKnownTypes().ToList();
        }

        private IEnumerable<AdapterPropertyValue> GetFlattenedKnownTypes()
        {
            foreach (string type in this.KnownTypes)
            {
                foreach (AdapterPropertyValue maSchemaAttribute in this.GetAttributesOfType(type))
                {
                    yield return maSchemaAttribute;
                }
            }
        }

        public bool CanPatch(KeyedCollection<string, AttributeChange> changes)
        {
            return this.SupportsPatch;
        }

        private IEnumerable<AdapterPropertyValue> GetAttributesOfType(string type)
        {
            foreach (AdapterSubfield item in this.Fields)
            {
                yield return new AdapterPropertyValue
                {
                    AttributeType = item.AttributeType,
                    FieldName = item.FieldName,
                    SupportsPatch = this.SupportsPatch,
                    IsMultivalued = item.IsMultivalued,
                    AttributeName = item.GetAttributeName($"{this.AttributeName}_{type}"),
                    Operation = item.Operation,
                    ParentFieldName = this.FieldName,
                    PropertyName = item.PropertyName,
                    AssignedType = type
                };
            }
        }

        private IEnumerable<string> AttributeNames
        {
            get
            {
                yield return this.AttributeName;

                foreach (AdapterPropertyValue attribute in this.Attributes)
                {
                    yield return attribute.AttributeName;
                }
            }
        }

        public bool CanProcessAttribute(string attribute)
        {
            return this.AttributeName == attribute || this.Attributes.Any(t => t.AttributeName == attribute);
        }

        public bool UpdateField(CSEntryChange csentry, object obj)
        {
            if (this.IsReadOnly || this.KnownTypes.Count == 0)
            {
                return false;
            }

            bool hasChanged = false;

            IList<IGrouping<string, Tuple<AttributeChange, AdapterPropertyValue>>> changes = this.GetAttributeChangesByType(csentry).ToList();

            if (changes.Count == 0)
            {
                return false;
            }

            if (this.propInfo == null)
            {
                this.propInfo = obj.GetType().GetProperty(this.PropertyName);
            }

            IList<T> list = this.GetList(obj);

            Dictionary<string, T> typedObjects = new Dictionary<string, T>();

            foreach (T item in list)
            {
                string type = this.GetTypeName(item);
                this.SetType(item, type);
                typedObjects.Add(type, item);
            }

            foreach (IGrouping<string, Tuple<AttributeChange, AdapterPropertyValue>> group in changes)
            {
                if (!typedObjects.ContainsKey(group.Key))
                {
                    T o = new T();

                    this.SetType(o, group.Key);
                    typedObjects.Add(group.Key, o);
                    list.Add(o);
                }

                foreach (Tuple<AttributeChange, AdapterPropertyValue> item in group)
                {
                    if (item.Item2.UpdateField(csentry, typedObjects[group.Key]))
                    {
                        hasChanged = true;
                    }
                }
            }

            if (this.RemoveEmptyItems(list))
            {
                hasChanged = true;
            }

            return hasChanged;
        }

        public IEnumerable<string> GetFieldNames(SchemaType type, string api)
        {
            yield break;
        }

        public IEnumerable<SchemaAttribute> GetSchemaAttributes()
        {
            foreach (AdapterSubfield field in this.Fields)
            {
                foreach (string type in this.KnownTypes)
                {
                    yield return field.GetSchemaAttribute($"{this.AttributeName}_{type}");
                }
            }
        }
        private bool RemoveEmptyItems(IList<T> items)
        {
            bool updated = false;

            if (items == null)
            {
                return false;
            }

            foreach (T item in items.ToList())
            {
                if (string.IsNullOrWhiteSpace(item.Value))
                {
                    items.Remove(item);
                    updated = true;
                }
            }

            return updated;
        }

        private IList<T> GetList(object obj)
        {
            if (this.propInfo == null)
            {
                this.propInfo = obj.GetType().GetProperty(this.PropertyName);
            }

            IList<T> list = this.propInfo.GetValue(obj) as IList<T>;

            return list;
        }


        public IEnumerable<AttributeChange> CreateAttributeChanges(string dn, ObjectModificationType modType, object obj)
        {
            if (this.propInfo == null)
            {
                this.propInfo = obj.GetType().GetProperty(this.PropertyName);
            }

            IList<T> list = this.GetList(obj);

            if (list == null)
            {
                yield break;
            }

            foreach (T item in list)
            {
                string type = this.GetTypeName(item);

                foreach (AdapterPropertyValue attribute in this.Attributes)
                {
                    if (attribute.AssignedType == type)
                    {
                        foreach (AttributeChange change in attribute.CreateAttributeChanges(dn, modType, item))
                        {
                            yield return change;
                        }
                    }
                }
            }
        }

        private IEnumerable<Tuple<AttributeChange, AdapterPropertyValue>> GetAttributeChanges(CSEntryChange csentry)
        {
            foreach (AdapterPropertyValue attribute in this.Attributes)
            {
                if (csentry.HasAttributeChange(attribute.AttributeName))
                {
                    yield return new Tuple<AttributeChange, AdapterPropertyValue>(csentry.AttributeChanges[attribute.AttributeName], attribute);
                }
            }
        }

        private IEnumerable<IGrouping<string, Tuple<AttributeChange, AdapterPropertyValue>>> GetAttributeChangesByType(CSEntryChange csentry)
        {
            return this.GetAttributeChanges(csentry).GroupBy(t => t.Item2.AssignedType);
        }

        public void SetType(T o, string type)
        {
            if (this.KnownRels.Contains(type))
            {
                o.Attributes["rel"] = type;
            }
            else
            {
                o.Attributes["label"] = type;
            }
        }

        public string GetTypeName(T o)
        {
            return (o.Attributes["rel"] ?? o.Attributes["label"]) as string;
        }
    }
}
