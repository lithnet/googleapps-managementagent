using System;
using System.Collections.Generic;
using Microsoft.MetadirectoryServices;

namespace Lithnet.GoogleApps.MA
{
    internal class SchemaBuilderChromeDevices : ISchemaTypeBuilder
    {
        public string TypeName => "chromeDevice";

        public IEnumerable<MASchemaType> GetSchemaTypes(IManagementAgentParameters config)
        {
            MASchemaType type = new MASchemaType
            {
                AttributeAdapters = new List<IAttributeAdapter>(),
                Name = "chromeDevice",
                AnchorAttributeNames = new[] { "id" },
                SupportsPatch = true,
            };

            type.ApiInterface = new ApiInterfaceChromeDevice(type, config);

            AdapterPropertyValue p = new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                GoogleApiFieldName = "serialNumber",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportOnly,
                MmsAttributeName = "serialNumber",
                ManagedObjectPropertyName = "SerialNumber",
                Api = "chromeDevice",
                SupportsPatch = true, 
                NullValueRepresentation = NullValueRepresentation.NullPlaceHolder,
                CastForImport = (i) => string.IsNullOrEmpty((string)i) ? null : i,
            };

            type.AttributeAdapters.Add(p);

            p = new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                GoogleApiFieldName = "status",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportOnly,
                MmsAttributeName = "status",
                ManagedObjectPropertyName = "Status",
                Api = "chromeDevice",
                SupportsPatch = true,
                NullValueRepresentation = NullValueRepresentation.NullPlaceHolder,
                CastForImport = (i) => string.IsNullOrEmpty((string)i) ? null : i,
            };

            type.AttributeAdapters.Add(p);

            p = new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                GoogleApiFieldName = "annotatedUser",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportOnly,
                MmsAttributeName = "annotatedUser",
                ManagedObjectPropertyName = "AnnotatedUser",
                Api = "chromeDevice",
                SupportsPatch = true,
                CastForImport = (i) => string.IsNullOrEmpty((string)i) ? null : i,
            };

            type.AttributeAdapters.Add(p);

            p = new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                GoogleApiFieldName = "deviceId",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportOnly,
                MmsAttributeName = "id",
                ManagedObjectPropertyName = "DeviceId",
                Api = "chromeDevice",
                SupportsPatch = true,
                IsAnchor = true,
            };

            type.AttributeAdapters.Add(p);

            p = new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                GoogleApiFieldName = "lastSync",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportOnly,
                MmsAttributeName = "lastSync",
                ManagedObjectPropertyName = "LastSyncRaw",
                Api = "chromeDevice",
                SupportsPatch = true,
                IsAnchor = false,
                CastForImport = (i) => string.IsNullOrEmpty((string)i) ? null : i,
            };

            type.AttributeAdapters.Add(p);

            p = new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                GoogleApiFieldName = "supportEndDate",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportOnly,
                MmsAttributeName = "supportEndDate",
                ManagedObjectPropertyName = "SupportEndDateRaw",
                Api = "chromeDevice",
                SupportsPatch = true,
                IsAnchor = false,
                CastForImport = (i) => string.IsNullOrEmpty((string)i) ? null : i,
            };

            type.AttributeAdapters.Add(p);

            p = new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                GoogleApiFieldName = "annotatedLocation",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportOnly,
                MmsAttributeName = "annotatedLocation",
                ManagedObjectPropertyName = "AnnotatedLocation",
                Api = "chromeDevice",
                SupportsPatch = true,
                IsAnchor = false,
                CastForImport = (i) => string.IsNullOrEmpty((string)i) ? null : i,
            };

            type.AttributeAdapters.Add(p);

            p = new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                GoogleApiFieldName = "annotatedAssetId",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportOnly,
                MmsAttributeName = "annotatedAssetId",
                ManagedObjectPropertyName = "AnnotatedAssetId",
                Api = "chromeDevice",
                SupportsPatch = true,
                IsAnchor = false,
                CastForImport = (i) => string.IsNullOrEmpty((string)i) ? null : i,
            };

            type.AttributeAdapters.Add(p);

            p = new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                GoogleApiFieldName = "notes",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportOnly,
                MmsAttributeName = "deviceNotes",
                ManagedObjectPropertyName = "Notes",
                Api = "chromeDevice",
                SupportsPatch = true,
                IsAnchor = false,
                CastForImport = (i) => string.IsNullOrEmpty((string)i) ? null : i,
            };

            type.AttributeAdapters.Add(p);

            p = new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                GoogleApiFieldName = "model",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportOnly,
                MmsAttributeName = "model",
                ManagedObjectPropertyName = "Model",
                Api = "chromeDevice",
                SupportsPatch = true,
                IsAnchor = false,
                CastForImport = (i) => string.IsNullOrEmpty((string)i) ? null : i,
            };

            type.AttributeAdapters.Add(p);

            p = new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                GoogleApiFieldName = "meid",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportOnly,
                MmsAttributeName = "meid",
                ManagedObjectPropertyName = "Meid",
                Api = "chromeDevice",
                SupportsPatch = true,
                IsAnchor = false,
                CastForImport = (i) => string.IsNullOrEmpty((string)i) ? null : i,
            };

            type.AttributeAdapters.Add(p);

            p = new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                GoogleApiFieldName = "orderNumber",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportOnly,
                MmsAttributeName = "orderNumber",
                ManagedObjectPropertyName = "OrderNumber",
                Api = "chromeDevice",
                SupportsPatch = true,
                IsAnchor = false,
                CastForImport = (i) => string.IsNullOrEmpty((string)i) ? null : i,
            };

            type.AttributeAdapters.Add(p);


            p = new AdapterPropertyValue
            {
                AttributeType = AttributeType.Boolean,
                GoogleApiFieldName = "willAutoRenew",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportOnly,
                MmsAttributeName = "willAutoRenew",
                ManagedObjectPropertyName = "WillAutoRenew",
                Api = "chromeDevice",
                SupportsPatch = true,
                IsAnchor = false,
            };

            type.AttributeAdapters.Add(p);

            p = new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                GoogleApiFieldName = "osVersion",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportOnly,
                MmsAttributeName = "osVersion",
                ManagedObjectPropertyName = "OsVersion",
                Api = "chromeDevice",
                SupportsPatch = true,
                IsAnchor = false,
                CastForImport = (i) => string.IsNullOrEmpty((string)i) ? null : i,
            };

            type.AttributeAdapters.Add(p);

            p = new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                GoogleApiFieldName = "platformVersion",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportOnly,
                MmsAttributeName = "platformVersion",
                ManagedObjectPropertyName = "PlatformVersion",
                Api = "chromeDevice",
                SupportsPatch = true,
                IsAnchor = false,
                CastForImport = (i) => string.IsNullOrEmpty((string)i) ? null : i,
            };

            type.AttributeAdapters.Add(p);

            p = new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                GoogleApiFieldName = "firmwareVersion",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportOnly,
                MmsAttributeName = "firmwareVersion",
                ManagedObjectPropertyName = "FirmwareVersion",
                Api = "chromeDevice",
                SupportsPatch = true,
                IsAnchor = false,
                CastForImport = (i) => string.IsNullOrEmpty((string)i) ? null : i,
            };

            type.AttributeAdapters.Add(p);

            p = new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                GoogleApiFieldName = "macAddress",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportOnly,
                MmsAttributeName = "macAddress",
                ManagedObjectPropertyName = "MacAddress",
                Api = "chromeDevice",
                SupportsPatch = true,
                IsAnchor = false,
                CastForImport = (i) => string.IsNullOrEmpty((string)i) ? null : i,
            };

            type.AttributeAdapters.Add(p);

            p = new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                GoogleApiFieldName = "bootMode",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportOnly,
                MmsAttributeName = "bootMode",
                ManagedObjectPropertyName = "BootMode",
                Api = "chromeDevice",
                SupportsPatch = true,
                IsAnchor = false,
                CastForImport = (i) => string.IsNullOrEmpty((string)i) ? null : i,
            };

            type.AttributeAdapters.Add(p);

            p = new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                GoogleApiFieldName = "orgUnitPath",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                MmsAttributeName = "orgUnitPath",
                ManagedObjectPropertyName = "OrgUnitPath",
                Api = "chromeDevice",
                SupportsPatch = true,
                IsAnchor = false,
                CastForImport = (i) => string.IsNullOrEmpty((string)i) ? null : i,
            };

            type.AttributeAdapters.Add(p);

            p = new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                GoogleApiFieldName = "lastEnrollmentTime",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportOnly,
                MmsAttributeName = "lastEnrollmentTime",
                ManagedObjectPropertyName = "LastEnrollmentTimeRaw",
                Api = "chromeDevice",
                SupportsPatch = true,
                IsAnchor = false,
                CastForImport = (i) => string.IsNullOrEmpty((string)i) ? null : i,
            };

            type.AttributeAdapters.Add(p);

            p = new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                GoogleApiFieldName = "ethernetMacAddress",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportOnly,
                MmsAttributeName = "ethernetMacAddress",
                ManagedObjectPropertyName = "EthernetMacAddress",
                Api = "chromeDevice",
                SupportsPatch = true,
                IsAnchor = false,
                CastForImport = (i) => string.IsNullOrEmpty((string)i) ? null : i,
            };

            type.AttributeAdapters.Add(p);

            //p = new AdapterPropertyValue
            //{
            //    AttributeType = AttributeType.String,
            //    FieldName = "dockMacAddress",
            //    IsMultivalued = false,
            //    Operation = AttributeOperation.ImportOnly,
            //    AttributeName = "dockMacAddress",
            //    PropertyName = "DockMacAddress",
            //    Api = "chromeDevice",
            //    SupportsPatch = true,
            //    IsAnchor = false,
            //};

            //type.AttributeAdapters.Add(p);

            //p = new AdapterPropertyValue
            //{
            //    AttributeType = AttributeType.String,
            //    FieldName = "manufactureDate",
            //    IsMultivalued = false,
            //    Operation = AttributeOperation.ImportOnly,
            //    AttributeName = "manufactureDate",
            //    PropertyName = "ManufactureDateRaw",
            //    Api = "chromeDevice",
            //    SupportsPatch = true,
            //    IsAnchor = false,
            //};

            //type.AttributeAdapters.Add(p);

            yield return type;
        }
    }
}
