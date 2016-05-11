using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using Microsoft.MetadirectoryServices;
using Lithnet.MetadirectoryServices;

namespace Lithnet.GoogleApps.MA
{
    using System.Collections;
    using Logging;
    using System.Reflection;
    using ManagedObjects;

    internal class MASchemaCustomTypeList<T> : IMASchemaAttribute where T : CustomTypeObject, new()
    {
        private PropertyInfo propInfo;

        public string AttributeName { get; set; }

        public string FieldName { get; set; }

        public string PropertyName { get; set; }

        public string Api { get; set; }

        public bool CanPatch { get; set; }

        [DataMember(Name = "known-types")]
        public IList<string> KnownTypes { get; set; }

        [DataMember(Name = "attributes")]
        public IList<MASchemaField> Fields { get; set; }

        [DataMember(Name = "is-read-only")]
        public bool IsReadOnly { get; set; }

        private IList<MASchemaAttribute> attributes;

        public IList<MASchemaAttribute> Attributes
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

        private IList<MASchemaAttribute> GetConstructedAttributes()
        {
            return this.GetFlattenedKnownTypes().ToList();
        }

        private IEnumerable<MASchemaAttribute> GetFlattenedKnownTypes()
        {
            foreach (string type in this.KnownTypes)
            {
                foreach (MASchemaAttribute maSchemaAttribute in this.GetAttributesOfType(type))
                {
                    yield return maSchemaAttribute;
                }
            }
        }

        private string PrimaryType => this.KnownTypes.FirstOrDefault();

        private bool IsPrimaryType(string type)
        {
            return this.PrimaryType == type;
        }

        private IEnumerable<MASchemaAttribute> GetAttributesOfType(string type)
        {
            foreach (MASchemaField item in this.Fields)
            {
                yield return new MASchemaAttribute
                {
                    AttributeType = item.AttributeType,
                    FieldName = item.FieldName,
                    IsArrayAttribute = true,
                    CanPatch = this.CanPatch,
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

                foreach (MASchemaAttribute attribute in this.Attributes)
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

            IList<IGrouping<string, Tuple<AttributeChange, MASchemaAttribute>>> changes = this.GetAttributeChangesByType(csentry).ToList();

            if (changes.Count == 0)
            {
                return false;
            }

            if (this.propInfo == null)
            {
                this.propInfo = obj.GetType().GetProperty(this.PropertyName);
            }

            bool created;
            IList<T> list = this.GetOrCreateList(obj, out created);

            Dictionary<string, T> typedObjects = new Dictionary<string, T>();

            foreach (T item in list)
            {
                if (this.SetPrimaryCandidate(item, item.Type))
                {
                    hasChanged = true;
                }

                if (this.SetPrimaryOnMissingType(item))
                {
                    hasChanged = true;
                }

                typedObjects.Add(item.Type, item);
            }

            foreach (IGrouping<string, Tuple<AttributeChange, MASchemaAttribute>> group in changes)
            {
                if (!typedObjects.ContainsKey(group.Key))
                {
                    T o = new T { Type = group.Key };
                    this.SetPrimaryCandidate(o, group.Key);
                    typedObjects.Add(group.Key, o);
                    list.Add(o);
                }

                foreach (Tuple<AttributeChange, MASchemaAttribute> item in group)
                {
                    if (item.Item2.UpdateField(csentry, typedObjects[group.Key]))
                    {
                        hasChanged = true;
                    }
                }
            }

            if (this.RemoveEmptyItems((IList)list))
            {
                hasChanged = true;
            }

            if (hasChanged && created)
            {
                this.propInfo.SetValue(obj, list);
            }

            return hasChanged;
        }

        private bool SetPrimaryOnMissingType(T item)
        {
            if (item.Type == null)
            {
                if (this.IsPrimary(item))
                {
                    item.Type = this.PrimaryType;
                }
                else
                {
                    return true;
                }
            }

            return false;
        }

        private bool SetPrimaryCandidate(T o, string type)
        {
            IPrimaryCandidateObject primaryObject = o as IPrimaryCandidateObject;

            if (primaryObject != null)
            {
                if (primaryObject.Primary != this.IsPrimaryType(type))
                {
                    primaryObject.Primary = this.IsPrimaryType(type);
                    return true;
                }
            }

            return false;
        }
        private bool IsPrimary(T o)
        {
            IPrimaryCandidateObject primaryObject = o as IPrimaryCandidateObject;
            return primaryObject?.IsPrimary ?? false;
        }

        public IEnumerable<SchemaAttribute> GetSchemaAttributes()
        {
            foreach (MASchemaField field in this.Fields)
            {
                foreach (string type in this.KnownTypes)
                {
                    yield return field.GetSchemaAttribute($"{this.AttributeName}_{type}");
                }
            }
        }

        private bool RemoveEmptyItems(IList items)
        {
            bool updated = false;

            if (items == null)
            {
                return false;
            }

            foreach (IIsEmptyObject item in items.OfType<IIsEmptyObject>().ToList())
            {
                if (item.IsEmpty())
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

        private IList<T> GetOrCreateList(object obj, out bool created)
        {
            if (this.propInfo == null)
            {
                this.propInfo = obj.GetType().GetProperty(this.PropertyName);
            }

            IList<T> list = this.propInfo.GetValue(obj) as IList<T>;
            created = false;

            if (list == null)
            {
                list = this.CreateList();
                created = true;
            }

            return list;
        }

        private IList<T> CreateList()
        {
            return new List<T>();
        }

        public IEnumerable<AttributeChange> CreateAttributeChanges(ObjectModificationType modType, object obj)
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
                if (item.Type == null)
                {
                    this.SetPrimaryOnMissingType(item);
                }

                foreach (MASchemaAttribute attribute in this.Attributes)
                {
                    if (attribute.AssignedType == item.Type)
                    {
                        foreach (AttributeChange change in attribute.CreateAttributeChanges(modType, item))
                        {
                            yield return change;
                        }
                    }
                }
            }
        }

        private IEnumerable<Tuple<AttributeChange, MASchemaAttribute>> GetAttributeChanges(CSEntryChange csentry)
        {
            foreach (MASchemaAttribute attribute in this.Attributes)
            {
                if (csentry.HasAttributeChange(attribute.AttributeName))
                {
                    yield return new Tuple<AttributeChange, MASchemaAttribute>(csentry.AttributeChanges[attribute.AttributeName], attribute);
                }
            }
        }

        private IEnumerable<IGrouping<string, Tuple<AttributeChange, MASchemaAttribute>>> GetAttributeChangesByType(CSEntryChange csentry)
        {
            return this.GetAttributeChanges(csentry).GroupBy(t => t.Item2.AssignedType);
        }
    }
}
