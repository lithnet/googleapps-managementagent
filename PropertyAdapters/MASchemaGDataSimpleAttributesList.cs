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
    using Google.GData.Extensions;
    using ManagedObjects;

    internal class MASchemaGDataSimpleAttributesList<T> : IMASchemaAttribute where T : SimpleAttribute, new()
    {
        private PropertyInfo propInfo;

        public string AttributeName { get; set; }

        public string FieldName { get; set; }

        public string PropertyName { get; set; }

        public string Api { get; set; }

        public bool CanPatch { get; set; }

        public IList<string> KnownTypes { get; set; }

        public IList<MASchemaField> Fields { get; set; }

        public HashSet<string> KnownRels { get; set; }

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

            IList<T> list = this.GetList(obj);

            Dictionary<string, T> typedObjects = new Dictionary<string, T>();

            foreach (T item in list)
            {
                string type = this.GetTypeName(item);
                this.SetType(item, type);
                typedObjects.Add(type, item);
            }

            foreach (IGrouping<string, Tuple<AttributeChange, MASchemaAttribute>> group in changes)
            {
                if (!typedObjects.ContainsKey(group.Key))
                {
                    T o = new T();

                    this.SetType(o, group.Key);
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

            if (hasChanged)
            {
                this.propInfo.SetValue(obj, list);
            }

            return hasChanged;
        }

        public IEnumerable<string> GetFieldNames(SchemaType type)
        {
            yield break;
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

                foreach (MASchemaAttribute attribute in this.Attributes)
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
