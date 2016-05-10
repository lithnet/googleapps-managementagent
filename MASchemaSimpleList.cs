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

    [DataContract(Name = "schema-list")]
    public class MASchemaSimpleList : IMASchemaAttribute
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

        [DataMember(Name = "is-read-only")]
        public bool IsReadOnly { get; set; }

        public bool UpdateField<T>(CSEntryChange csentry, T obj)
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

            IList list;

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

            IList<object> valueAdds = csentry.GetValueAdds<object>(this.AttributeName);
            IList<object> valueDeletes = csentry.GetValueDeletes<object>(this.AttributeName);

            foreach (object value in valueAdds)
            {
                list.Add(value);
                Logger.WriteLine($"Adding value {this.AttributeName} -> {value}");
            }

            foreach (object value in valueDeletes)
            {
                list.Remove(value);
                Logger.WriteLine($"Removing value {this.AttributeName} -> {value}");
            }

            this.propInfo.SetValue(obj, list, null);

            return true;
        }

        private IList GetList<T>(T obj, out bool created)
        {
            if (this.propInfo == null)
            {
                this.propInfo = obj.GetType().GetProperty(this.PropertyName);
            }

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

        private IList CreateList<T>(T obj)
        {
            if (this.propInfo == null)
            {
                this.propInfo = obj.GetType().GetProperty(this.PropertyName);
            }

            object childObject = Activator.CreateInstance(this.propInfo.PropertyType, new object[] { });

            IList list = childObject as IList;

            if (list == null)
            {
                throw new ArgumentException($"property {this.propInfo.Name} does not inherit from IList");
            }

            return list;
        }

        public IEnumerable<AttributeChange> CreateAttributeChanges<T>(ObjectModificationType modType, T obj)
        {
            bool created;

            IList list = this.GetList(obj, out created);

            if (list == null)
            {
                if (modType == ObjectModificationType.Update)
                {
                    yield return AttributeChange.CreateAttributeDelete(this.AttributeName);
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