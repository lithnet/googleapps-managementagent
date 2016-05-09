using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.MetadirectoryServices;
using Newtonsoft.Json.Linq;
using Lithnet.MetadirectoryServices;
using System.Linq.Expressions;
using System.Reflection;
using Lithnet.Logging;

namespace Lithnet.GoogleApps.MA
{
    public static class CSEntryChangeExtensions
    {
        public static string GetStringValueAddOrNullPlaceholder(this AttributeChange change)
        {
            if (change.ModificationType == AttributeModificationType.Delete)
            {
                return Constants.NullValuePlaceholder;
            }

            return change.GetValueAdd<string>() ?? Constants.NullValuePlaceholder;
        }

        public static AttributeModificationType GetSVAttributeModificationType(this CSEntryChange csentry)
        {
            switch (csentry.ObjectModificationType)
            {
                case ObjectModificationType.Add:
                case ObjectModificationType.Replace:
                    return AttributeModificationType.Add;

                case ObjectModificationType.Update:
                    return AttributeModificationType.Replace;

                default:
                case ObjectModificationType.None:
                case ObjectModificationType.Unconfigured:
                case ObjectModificationType.Delete:
                    throw new NotSupportedException("Unknown or unsupported modification type");
            }
        }

        public static AttributeModificationType GetMVAttributeModificationType(this CSEntryChange csentry)
        {
            switch (csentry.ObjectModificationType)
            {
                case ObjectModificationType.Add:
                case ObjectModificationType.Replace:
                    return AttributeModificationType.Add;

                case ObjectModificationType.Update:
                    return AttributeModificationType.Update;

                default:
                case ObjectModificationType.None:
                case ObjectModificationType.Unconfigured:
                case ObjectModificationType.Delete:
                    throw new NotSupportedException("Unknown or unsupported modification type");
            }
        }

        internal static void UpdateTargetFromCSEntryChange<T2>(this CSEntryChange csentry, T2 obj, Expression<Func<T2, string>> property, string attributeName, ref bool updateRequired)
        {
            if (csentry.HasAttributeChange(attributeName))
            {
                var expr = (MemberExpression)property.Body;
                var prop = (PropertyInfo)expr.Member;

                string value = csentry.GetValueAdd<string>(attributeName);

                if (value == null)
                {
                    value = Constants.NullValuePlaceholder;
                }

                prop.SetValue(obj, value, null);

                Logger.WriteLine("Updating {0} -> {1}", attributeName, prop.GetValue(obj, null));
                updateRequired = true;
            }
        }

        internal static void UpdateTargetFromCSEntryChange<T2>(this CSEntryChange csentry, T2 obj, Expression<Func<T2, int?>> property, string attributeName, ref bool updateRequired)
        {
            if (csentry.HasAttributeChange(attributeName))
            {
                var expr = (MemberExpression)property.Body;
                var prop = (PropertyInfo)expr.Member;

                int? value = csentry.GetValueAdd<int>(attributeName);

                prop.SetValue(obj, value, null);

                Logger.WriteLine("Updating {0} -> {1}", attributeName, prop.GetValue(obj, null));
                updateRequired = true;
            }
        }

        internal static void UpdateTargetFromCSEntryChange<T2>(this CSEntryChange csentry, T2 obj, Expression<Func<T2, bool?>> property, string attributeName, ref bool updateRequired)
        {
            if (csentry.HasAttributeChange(attributeName))
            {
                var expr = (MemberExpression)property.Body;
                var prop = (PropertyInfo)expr.Member;

                bool? value = csentry.GetValueAdd<bool>(attributeName);

                prop.SetValue(obj, value, null);

                Logger.WriteLine("Updating {0} -> {1}", attributeName, prop.GetValue(obj, null));
                updateRequired = true;
            }
        }

        internal static void UpdateTargetFromCSEntryChange<T1, T2, T3>(this CSEntryChange csentry, T2 obj, Expression<Func<T2, T3>> property, Func<T1, T3> targetTransform, string attributeName, ref bool updateRequired)
        {
            if (csentry.HasAttributeChange(attributeName))
            {
                var expr = (MemberExpression)property.Body;
                var prop = (PropertyInfo)expr.Member;
                prop.SetValue(obj, targetTransform(csentry.GetValueAdd<T1>(attributeName)), null);

                Logger.WriteLine("Updating {0} -> {1}", attributeName, prop.GetValue(obj, null));
                updateRequired = true;
            }
        }
     
    }
}
