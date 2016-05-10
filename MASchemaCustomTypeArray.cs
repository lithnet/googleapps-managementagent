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

    [DataContract(Name = "schema-array")]
    public class MASchemaCustomTypeArray : IMASchemaAttribute
    {
        private PropertyInfo propInfo;

        [DataMember(Name = "attribute-name")]
        public string AttributeName { get; set; }

        [DataMember(Name = "field-name")]
        public string FieldName { get; set; }

        [DataMember(Name = "property-name")]
        public string PropertyName { get; set; }

        [DataMember(Name = "api")]
        public string Api { get; set; }

        [DataMember(Name = "can-patch")]
        public bool CanPatch { get; set; }

        [DataMember(Name = "type-name")]
        public string TypeName { get; set; }

        public Type Type
        {
            get
            {
                return Type.GetType(this.TypeName);

            }

            set
            {
                this.TypeName = value.AssemblyQualifiedName;

            }
        }

        [DataMember(Name = "known-types")]
        public IList<string> KnownTypes { get; set; }

        [DataMember(Name = "attributes")]
        public IList<MASchemaArrayField> Fields { get; set; }

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

        private IEnumerable<MASchemaAttribute> GetAttributesOfType(string type)
        {
            foreach (MASchemaArrayField item in this.Fields)
            {
                yield return new MASchemaAttribute
                {
                    AttributeType = item.AttributeType,
                    FieldName = item.FieldName,
                    IsArrayAttribute = true,
                    CanPatch = this.CanPatch,
                    IsMultivalued = item.IsMultivalued,
                    AttributeName = $"{this.AttributeName}_{type}_{item.AttributeNamePart}",
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

        public bool UpdateField<T>(CSEntryChange csentry, T obj)
        {
            if (this.IsReadOnly)
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
            IList list = this.GetList(obj, out created);

            Dictionary<string, CustomTypeObject> typedObjects = new Dictionary<string, CustomTypeObject>();

            foreach (CustomTypeObject item in list.OfType<CustomTypeObject>())
            {
                typedObjects.Add(item.Type, item);
            }

            foreach (IGrouping<string, Tuple<AttributeChange, MASchemaAttribute>> group in changes)
            {
                if (!typedObjects.ContainsKey(group.Key))
                {
                    CustomTypeObject o = (CustomTypeObject)Activator.CreateInstance(this.Type, new object[] {});
                    o.Type = group.Key;
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

            if (this.RemoveEmptyItems(list))
            {
                hasChanged = true;
            }

            if (hasChanged && created)
            {
                this.propInfo.SetValue(obj, list);
            }

            return hasChanged;
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

        private IList GetList<T>(T obj, out bool created)
        {
            object childObject = this.propInfo.GetValue(obj);
            created = false;

            if (childObject == null)
            {
                childObject = Activator.CreateInstance(this.propInfo.PropertyType, new object[] { });
                created = true;
            }

            IList list = childObject as IList;

            if (list == null)
            {
                throw new ArgumentException($"property {this.propInfo.Name} does not inherit from IList");
            }

            return list;
        }

        public IEnumerable<AttributeChange> CreateAttributeChanges<T>(ObjectModificationType modType, T obj)
        {
            if (this.propInfo == null)
            {
                this.propInfo = obj.GetType().GetProperty(this.PropertyName);
            }

            bool created;
            IList list = this.GetList(obj, out created);

            foreach (CustomTypeObject item in list.OfType<CustomTypeObject>())
            {
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
