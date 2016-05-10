using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            if (types.Contains(objectClass))
            {
                return types[objectClass].Attributes.Any(t => t.Name == attribute);
            }
            else
            {
                return false;
            }
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
            return GenericExtensions.ToResourceManagementServiceDateFormat(dateTime, false);
        }

        /// <summary>
        /// Converts a date time to the ISO 8601 date string required by the Resource Management Service
        /// </summary>
        /// <param name="dateTime">The date and time to convert</param>
        /// <returns>An ISO 8601 date format string</returns>
        public static string ToResourceManagementServiceDateFormat(this DateTime? dateTime, bool zeroMilliseconds)
        {
            if (dateTime.HasValue)
            {
                return GenericExtensions.ToResourceManagementServiceDateFormat(dateTime.Value, zeroMilliseconds);
            }
            else
            {
                return null;
            }
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
                throw new ArgumentNullException("securePassword");

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
        /// Converts an enumeration of strings into a comma separated list
        /// </summary>
        /// <param name="strings">The enumeration of string objects</param>
        /// <returns>The comma separated list of strings</returns>
        public static string ToCommaSeparatedString(this IEnumerable<string> strings)
        {
            string newString = string.Empty;

            if (strings != null)
            {
                foreach (string text in strings)
                {
                    newString = newString.AppendWithCommaSeparator(text);
                }
            }

            return newString;
        }

        /// <summary>
        /// Converts an enumeration of strings into a comma separated list
        /// </summary>
        /// <param name="strings">The enumeration of string objects</param>
        /// <param name="separator">The character or string to use to separate the strings</param>
        /// <returns>The comma separated list of strings</returns>
        public static string ToSeparatedString(this IEnumerable<string> strings, string separator)
        {
            string newString = string.Empty;

            foreach (string text in strings)
            {
                newString = newString.AppendWithSeparator(separator, text);
            }

            return newString;
        }

        /// <summary>
        /// Converts an enumeration of strings into a comma separated list
        /// </summary>
        /// <param name="strings">The enumeration of string objects</param>
        /// <returns>The comma separated list of strings</returns>
        public static string ToNewLineSeparatedString(this IEnumerable<string> strings)
        {
            StringBuilder builder = new StringBuilder();

            foreach (string text in strings)
            {
                builder.AppendLine(text);
            }

            return builder.ToString().TrimEnd();
        }

        /// <summary>
        /// Appends two string together with a comma and a space
        /// </summary>
        /// <param name="text">The original string</param>
        /// <param name="textToAppend">The string to append</param>
        /// <returns>The concatenated string</returns>
        public static string AppendWithCommaSeparator(this string text, string textToAppend)
        {
            string newString = string.Empty;

            if (!string.IsNullOrWhiteSpace(text))
            {
                text += ", ";
            }
            else
            {
                text = string.Empty;
            }

            newString = text + textToAppend;
            return newString;
        }

        /// <summary>
        /// Appends two string together with a comma and a space
        /// </summary>
        /// <param name="text">The original string</param>
        /// <param name="separator">The character or string to use to separate the strings</param>
        /// <param name="textToAppend">The string to append</param>
        /// <returns>The concatenated string</returns>
        public static string AppendWithSeparator(this string text, string separator, string textToAppend)
        {
            string newString = string.Empty;

            if (!string.IsNullOrWhiteSpace(text))
            {
                text += separator;
            }
            else
            {
                text = string.Empty;
            }

            newString = text + textToAppend;
            return newString;
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
            if (obj == null)
            {
                return null;
            }
            else
            {
                return obj.ToSmartString();
            }
        }

        /// <summary>
        /// Gets an informative string representation of an object
        /// </summary>
        /// <param name="obj">The object to convert</param>
        /// <returns>An informative string representation of an object, or a null value if the object is null</returns>
        public static string ToSmartStringOrEmptyString(this object obj)
        {
            if (obj == null)
            {
                return string.Empty;
            }
            else
            {
                return obj.ToSmartString();
            }
        }

        /// <summary>
        /// Truncates a string to the specified length
        /// </summary>
        /// <param name="obj">The string to truncate</param>
        /// <param name="totalLength">The length to truncate to</param>
        /// <returns></returns>
        public static string TruncateString(this string obj, int totalLength)
        {
            if (string.IsNullOrWhiteSpace(obj))
            {
                return obj;
            }

            if (totalLength <= 3)
            {
                throw new ArgumentException("The maxlength value must be greater than 3", "totalLength");
            }

            if (obj.Length > totalLength)
            {
                return obj.Substring(0, totalLength - 3) + "...";
            }
            else
            {
                return obj;
            }
        }

        /// <summary>
        /// Gets a value indicating whether two enumerations contain the same elements, even if they are in different orders
        /// </summary>
        /// <typeparam name="T">The type of items in the enumerations</typeparam>
        /// <param name="enumeration1">The first list to compare</param>
        /// <param name="enumeration2">The second list to compare</param>
        /// <returns>A value indicating if the two enumerations contain the same objects</returns>
        public static bool ContainsSameElements<T>(this IEnumerable<T> enumeration1, IEnumerable<T> enumeration2)
        {
            List<T> list1 = enumeration1.ToList();
            List<T> list2 = enumeration2.ToList();

            if (list1.Count != list2.Count)
            {
                return false;
            }

            return list1.Intersect(list2).Count() == list1.Count;
        }

        /// <summary>
        /// <para>Truncates a DateTime to a specified resolution.</para>
        /// <para>A convenient source for resolution is TimeSpan.TicksPerXXXX constants.</para>
        /// </summary>
        /// <param name="date">The DateTime object to truncate</param>
        /// <param name="resolution">e.g. to round to nearest second, TimeSpan.TicksPerSecond</param>
        /// <returns>Truncated DateTime</returns>
        public static DateTime Truncate(this DateTime date, long resolution)
        {
            return new DateTime(date.Ticks - (date.Ticks % resolution), date.Kind);
        }

        public static string Serialize<T>(this IEnumerable<T> item)
        {
            GoogleJsonSerializer s = new GoogleJsonSerializer();
            return s.Serialize(item);
        }
    }
}