using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Runtime.InteropServices;
using Microsoft.MetadirectoryServices;

namespace Lithnet.GoogleApps.MA
{
    internal static class GenericExtensions
    {
        public const string FimServiceDateFormat = @"yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff";

        public const string FimServiceDateFormatZeroedMilliseconds = @"yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'000";

        public static bool HasAttribute(this SchemaType type, string attribute)
        {
            return type.Attributes.Any(t => t.Name == attribute);
        }

        public static bool HasAttribute(this SchemaTypeKeyedCollection types, string objectClass, string attribute)
        {
            return types.Contains(objectClass) && types[objectClass].Attributes.Any(t => t.Name == attribute);
        }

        public static bool HasObjectClass(this SchemaTypeKeyedCollection types, string objectClass)
        {
            return types.Contains(objectClass);
        }

        /// <summary>
        /// Converts a date time to the ISO 8601 date string required by the Resource Management Service
        /// </summary>
        /// <param name="dateTime">The date and time to convert</param>
        /// <returns>An ISO 8601 date format string</returns>
        public static string ToResourceManagementServiceDateFormat(this DateTime dateTime)
        {
            return dateTime.ToResourceManagementServiceDateFormat(false);
        }

        /// <summary>
        /// Converts a date time to the ISO 8601 date string required by the Resource Management Service
        /// </summary>
        /// <param name="dateTime">The date and time to convert</param>
        /// <returns>An ISO 8601 date format string</returns>
        public static string ToResourceManagementServiceDateFormat(this DateTime? dateTime, bool zeroMilliseconds)
        {
            return dateTime?.ToResourceManagementServiceDateFormat(zeroMilliseconds);
        }

        /// <summary>
        /// Converts a date time to the ISO 8601 date string required by the Resource Management Service
        /// </summary>
        /// <param name="dateTime">The date and time to convert</param>
        /// <param name="zeroMilliseconds">A value indicating whether the millisecond component of the date should be zeroed to avoid rounding/round-trip issues</param>
        /// <returns>An ISO 8601 date format string</returns>
        public static string ToResourceManagementServiceDateFormat(this DateTime dateTime, bool zeroMilliseconds)
        {
            DateTime convertedDateTime = dateTime.ToUniversalTime();

            if (zeroMilliseconds)
            {
                return convertedDateTime.ToString(GenericExtensions.FimServiceDateFormatZeroedMilliseconds);
            }
            else
            {
                return convertedDateTime.ToString(GenericExtensions.FimServiceDateFormat);

            }
        }

        public static string ConvertToUnsecureString(this SecureString securePassword)
        {
            if (securePassword == null)
                throw new ArgumentNullException(nameof(securePassword));

            IntPtr unmanagedString = IntPtr.Zero;
            try
            {
                unmanagedString = Marshal.SecureStringToGlobalAllocUnicode(securePassword);
                return Marshal.PtrToStringUni(unmanagedString);
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(unmanagedString);
            }
        }

        /// <summary>
        /// Gets an informative string representation of an object
        /// </summary>
        /// <param name="obj">The object to convert</param>
        /// <returns>An informative string representation of an object</returns>
        public static string ToSmartString(this object obj)
        {
            if (obj is byte[])
            {
                byte[] cast = (byte[])obj;
                return Convert.ToBase64String(cast);
            }
            else if (obj is long)
            {
                return ((long)obj).ToString();
            }
            else if (obj is string)
            {
                return ((string)obj).ToString();
            }
            else if (obj is bool)
            {
                return ((bool)obj).ToString();
            }
            else if (obj is Guid)
            {
                return ((Guid)obj).ToString();
            }
            else if (obj is DateTime)
            {
                return ((DateTime)obj).ToString(GenericExtensions.FimServiceDateFormat);
            }
            else if (obj == null)
            {
                return "null";
            }
            else
            {
                return obj.ToString();
            }
        }

        /// <summary>
        /// Gets an informative string representation of an object
        /// </summary>
        /// <param name="obj">The object to convert</param>
        /// <returns>An informative string representation of an object, or a null value if the object is null</returns>
        public static string ToSmartStringOrNull(this object obj)
        {
            return obj?.ToSmartString();
        }

        /// <summary>
        /// Gets an informative string representation of an object
        /// </summary>
        /// <param name="obj">The object to convert</param>
        /// <returns>An informative string representation of an object, or a null value if the object is null</returns>
        public static string ToSmartStringOrEmptyString(this object obj)
        {
            return obj == null ? string.Empty : obj.ToSmartString();
        }

        public static void AddIfNotNull<T>(this List<T> list, T item)
        {
            if (item == null)
            {
                return;
            }

            list.Add(item);
        }

        public static void AddRange<T>(this HashSet<T> hashset, IEnumerable<T> items)
        {
            if (items == null)
            {
                return;
            }

            foreach (T item in items)
            {
                hashset.Add(item);
            }
        }

        public static void RemoveRange<T>(this HashSet<T> hashset, IEnumerable<T> items)
        {
            if (items == null)
            {
                return;
            }

            foreach (T item in items)
            {
                hashset.Remove(item);
            }
        }
    }
}