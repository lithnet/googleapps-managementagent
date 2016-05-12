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

    internal class MASchemaCollection<T> : IMASchemaAttribute
    {
        private PropertyInfo propInfo;

        public string AttributeName { get; set; }

        public string FieldName { get; set; }

        public string PropertyName { get; set; }

        public string Api { get; set; }

        public bool CanPatch { get; set; }

        public bool IsMultivalued => true;

        public AttributeType AttributeType { get; set; }

        public AttributeOperation Operation { get; set; }

        public bool IsReadOnly { get; set; }

        public bool CanProcessAttribute(string attribute)
        {
            return this.AttributeName == attribute;
        }

        public bool UpdateField(CSEntryChange csentry, object obj)
        {
            if (this.IsReadOnly)
            {
                return false;
            }

            if (!csentry.HasAttributeChange(this.AttributeName))
            {
                return false;
            }

            AttributeChange change = csentry.AttributeChanges[this.AttributeName];

            if (this.propInfo == null)
            {
                this.propInfo = obj.GetType().GetProperty(this.PropertyName);
            }

            ICollection<T> list;

            switch (change.ModificationType)
            {
                case AttributeModificationType.Add:
                case AttributeModificationType.Replace:
                    list = this.CreateList(obj);
                    break;

                case AttributeModificationType.Update:
                    bool created;
                    list = this.GetOrCreateList(obj, out created);
                    break;

                case AttributeModificationType.Delete:
                    list = this.CreateList(obj);
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            ICollection<T> valueAdds = csentry.GetValueAdds<T>(this.AttributeName);
            ICollection<T> valueDeletes = csentry.GetValueDeletes<T>(this.AttributeName);

            foreach (T value in valueAdds)
            {
                list.Add(value);
                Logger.WriteLine($"Adding value {this.AttributeName} -> {value}");
            }

            foreach (T value in valueDeletes)
            {
                list.Remove(value);
                Logger.WriteLine($"Removing value {this.AttributeName} -> {value}");
            }

            this.propInfo.SetValue(obj, list, null);

            return true;
        }

        public IEnumerable<SchemaAttribute> GetSchemaAttributes()
        {
            yield return SchemaAttribute.CreateMultiValuedAttribute(this.AttributeName, this.AttributeType, this.Operation);
        }

        public IEnumerable<string> GetFieldNames(SchemaType type)
        {
            if (this.FieldName != null)
            {
                if (type.HasAttribute(this.AttributeName))
                {
                    yield return this.FieldName;
                }
            }
        }

        private ICollection<T> GetOrCreateList(object obj, out bool created)
        {
            if (this.propInfo == null)
            {
                this.propInfo = obj.GetType().GetProperty(this.PropertyName);
            }
            

            ICollection<T> list = this.propInfo.GetValue(obj) as ICollection<T>;
            created = false;

            if (list == null)
            {
                list = this.CreateList(obj);
                created = true;
            }

            return list;
        }

        private ICollection<T> GetList(object obj)
        {
            if (this.propInfo == null)
            {
                this.propInfo = obj.GetType().GetProperty(this.PropertyName);
            }

            ICollection<T> childObject = this.propInfo.GetValue(obj) as ICollection<T>;
            

            return childObject;
        }

        private ICollection<T> CreateList(object obj)
        {
            if (this.propInfo == null)
            {
                this.propInfo = obj.GetType().GetProperty(this.PropertyName);
            }

            return (ICollection<T>)Activator.CreateInstance(this.propInfo.PropertyType, null, new object[] {});
        }

        public IEnumerable<AttributeChange> CreateAttributeChanges(ObjectModificationType modType, object obj)
        {
            ICollection<T> list = this.GetList(obj);

            if (list == null)
            {
                if (modType == ObjectModificationType.Update)
                {
                    yield return AttributeChange.CreateAttributeDelete(this.AttributeName);
                    yield break;
                }
                else
                {
                    yield break;
                }
            }

            switch (modType)
            {
                case ObjectModificationType.Add:
                case ObjectModificationType.Replace:
                    yield return AttributeChange.CreateAttributeAdd(this.AttributeName, list.Cast<object>().ToList());
                    break;

                case ObjectModificationType.Update:
                    yield return AttributeChange.CreateAttributeReplace(this.AttributeName, list.Cast<object>().ToList());
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(modType), modType, null);
            }
        }
    }
}