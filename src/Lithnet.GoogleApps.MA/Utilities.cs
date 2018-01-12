using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lithnet.GoogleApps.MA
{
    internal static class Utilities
    {
        public static object SetPlaceholderIfNull(object value, NullValueRepresentation nullValueRepresentation)
        {
            if (value == null)
            {
                switch (nullValueRepresentation)
                {
                    case NullValueRepresentation.EmptyString:
                        value = "";
                        break;

                    case NullValueRepresentation.NullPlaceHolder:
                        value = Constants.NullValuePlaceholder;
                        break;

                    case NullValueRepresentation.IntZero:
                        value = 0;
                        break;
                }
            }

            return value;
        }

        public static object GetNullRepresentation(NullValueRepresentation nullValueRepresentation)
        {
            switch (nullValueRepresentation)
            {
                case NullValueRepresentation.IntZero:
                    return 0;

                case NullValueRepresentation.EmptyString:
                    return string.Empty;

                case NullValueRepresentation.NullPlaceHolder:
                    return Constants.NullValuePlaceholder;

                default:
                case NullValueRepresentation.Null:
                    return null;
            }
        }

        public static object GetNullIfPlaceholder(object value, NullValueRepresentation nullValueRepresentation)
        {
            switch (nullValueRepresentation)
            {
                case NullValueRepresentation.IntZero:
                    try
                    {
                        int v = Convert.ToInt32(value);
                        if (v == 0)
                        {
                            value = null;
                        }
                    }
                    catch
                    {
                    }
                    break;

                case NullValueRepresentation.EmptyString:
                    if (value is string s)
                    {
                        if (string.IsNullOrEmpty(s))
                        {
                            value = null;
                        }
                    }
                    break;

                case NullValueRepresentation.NullPlaceHolder:
                    if (value is string s2)
                    {
                        if (s2 == Constants.NullValuePlaceholder)
                        {
                            value = null;
                        }
                    }
                    break;
            }

            return value;
        }
    }
}