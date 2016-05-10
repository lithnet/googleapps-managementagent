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

    internal class MASchemaSimpleList<TItem> : IMASchemaAttribute
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

            IList<TItem> list;

            switch (change.ModificationType)
            {
                case AttributeModificationType.Add:
                case AttributeModificationType.Replace:
                    list = this.CreateList(obj);
                    break;

                case AttributeModificationType.Update:
                    bool created;
                    list = this.GetList(obj, out created);
                    break;

                case AttributeModificationType.Delete:
                    list = this.CreateList(obj);
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            IList<TItem> valueAdds = csentry.GetValueAdds<TItem>(this.AttributeName);
            IList<TItem> valueDeletes = csentry.GetValueDeletes<TItem>(this.AttributeName);

            foreach (TItem value in valueAdds)
            {
                list.Add(value);
                Logger.WriteLine($"Adding value {this.AttributeName} -> {value}");
            }

            foreach (TItem value in valueDeletes)
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

        private IList<TItem> GetList(object obj, out bool created)
        {
            if (this.propInfo == null)
            {
                this.propInfo = obj.GetType().GetProperty(this.PropertyName);
            }

            IList<TItem> childObject = this.propInfo.GetValue(obj) as IList<TItem>;
            created = false;

            if (childObject == null)
            {
                childObject = new List<TItem>();
                created = true;
            }

            return childObject;
        }

        private IList<TItem> CreateList(object obj)
        {
            return new List<TItem>();
        }

        public IEnumerable<AttributeChange> CreateAttributeChanges(ObjectModificationType modType, object obj)
        {
            bool created;

            IList<TItem> list = this.GetList(obj, out created);

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