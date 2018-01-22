using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using Lithnet.Logging;
using Lithnet.MetadirectoryServices;
using Microsoft.MetadirectoryServices;

namespace Lithnet.GoogleApps.MA
{
    internal class AdapterCollection<T> : IAttributeAdapter
    {
        //private PropertyInfo propInfo;

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

        public bool IsMultivalued => true;

        public Func<object, ICollection<T>> GetList { get; set; }

        public Func<object, ICollection<T>> CreateList { get; set; }

        public Action<object, ICollection<T>> PutList { get; set; }

        public AttributeType AttributeType { get; set; }

        public AttributeOperation Operation { get; set; }

        public bool IsReadOnly => this.Operation == AttributeOperation.ImportOnly;

        public bool IsAnchor => false;

        public bool CanProcessAttribute(string attribute)
        {
            return this.AttributeName == attribute;
        }

        public bool CanPatch(KeyedCollection<string, AttributeChange> changes)
        {
            return this.SupportsPatch;
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

            ICollection<T> list;

            switch (change.ModificationType)
            {
                case AttributeModificationType.Add:
                case AttributeModificationType.Replace:
                    list = this.CreateListInternal(obj);
                    break;

                case AttributeModificationType.Update:
                    bool created;
                    list = this.GetOrCreateListInternal(obj, out created);
                    break;

                case AttributeModificationType.Delete:
                    list = this.CreateListInternal(obj);
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

            this.PutListInternal(obj, list);

            return true;
        }

        public IEnumerable<SchemaAttribute> GetSchemaAttributes()
        {
            yield return SchemaAttribute.CreateMultiValuedAttribute(this.AttributeName, this.AttributeType, this.Operation);
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

        private ICollection<T> GetOrCreateListInternal(object obj, out bool created)
        {
            ICollection<T> list = this.GetListInternal(obj);
            created = false;

            if (list == null)
            {
                list = this.CreateListInternal(obj);
                created = true;
            }

            return list;
        }

        private ICollection<T> GetListInternal(object obj)
        {
            if (this.GetList != null)
            {
                return this.GetList(obj);
            }

            PropertyInfo propInfo = obj.GetType().GetProperty(this.PropertyName);

            ICollection<T> childObject = propInfo.GetValue(obj) as ICollection<T>;

            return childObject;
        }

        private ICollection<T> CreateListInternal(object obj)
        {
            if (this.CreateList != null)
            {
                return this.CreateList(obj);
            }

            PropertyInfo propInfo = obj.GetType().GetProperty(this.PropertyName);

            return (ICollection<T>)Activator.CreateInstance(propInfo.PropertyType, null, new object[] { });
        }

        public IEnumerable<AttributeChange> CreateAttributeChanges(string dn, ObjectModificationType modType, object obj)
        {
            ICollection<T> list = this.GetListInternal(obj);

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

            if (list.Count == 0)
            {
                yield break;
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

        private void PutListInternal(object obj, ICollection<T> list)
        {
            if (this.PutList == null)
            {
                PropertyInfo propInfo = obj.GetType().GetProperty(this.PropertyName);
                propInfo.SetValue(obj, list, null);
            }
            else
            {
                this.PutList(obj, list);
            }
        }
    }
}