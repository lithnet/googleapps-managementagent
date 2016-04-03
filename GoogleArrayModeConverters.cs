using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lithnet.GoogleApps.MA
{
    public static class GoogleArrayModeConverters
    {
        private const string PrimaryValueOnlyDescription = "Use the primary value only";
        private const string FlattenKnownTypesDescription = "Flatten known types";
        private const string JsonDescription = "Raw JSON string";
        
        public static string ToDescription(this GoogleArrayMode mode)
        {
            switch (mode)
            {
                case GoogleArrayMode.PrimaryValueOnly:
                    return GoogleArrayModeConverters.PrimaryValueOnlyDescription;

                case GoogleArrayMode.Json:
                    return GoogleArrayModeConverters.JsonDescription;

                case GoogleArrayMode.FlattenKnownTypes:
                    return GoogleArrayModeConverters.FlattenKnownTypesDescription;

                default:
                    throw new InvalidOperationException();
            }
        }

        public static GoogleArrayMode FromDescription(string description)
        {
            switch (description)
            {
                case GoogleArrayModeConverters.PrimaryValueOnlyDescription:
                    return GoogleArrayMode.PrimaryValueOnly;

                case GoogleArrayModeConverters.JsonDescription:
                    return GoogleArrayMode.Json;

                case GoogleArrayModeConverters.FlattenKnownTypesDescription:
                    return GoogleArrayMode.FlattenKnownTypes;

                default:
                    throw new InvalidOperationException();
            }
        }

        public static string[] ParameterNamesPrimarySupported
        {
            get
            {
                return new string[]
                {
                    GoogleArrayModeConverters.PrimaryValueOnlyDescription,
                    GoogleArrayModeConverters.JsonDescription,
                    GoogleArrayModeConverters.FlattenKnownTypesDescription
                };
            }
        }

        public static string[] ParameterNamesPrimaryNotSupported
        {
            get
            {
                return new string[]
                {
                    GoogleArrayModeConverters.JsonDescription,
                    GoogleArrayModeConverters.FlattenKnownTypesDescription
                };
            }
        }
    }
}
