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
                FieldName = "serialNumber",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportOnly,
                AttributeName = "serialNumber",
                PropertyName = "SerialNumber",
                Api = "chromeDevice",
                SupportsPatch = true, 
                NullValueRepresentation = NullValueRepresentation.NullPlaceHolder,
                CastForImport = (i) => string.IsNullOrEmpty((string)i) ? null : i,
            };

            type.AttributeAdapters.Add(p);

            p = new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                FieldName = "status",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportOnly,
                AttributeName = "status",
                PropertyName = "Status",
                Api = "chromeDevice",
                SupportsPatch = true,
                NullValueRepresentation = NullValueRepresentation.NullPlaceHolder,
                CastForImport = (i) => string.IsNullOrEmpty((string)i) ? null : i,
            };

            type.AttributeAdapters.Add(p);

            p = new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                FieldName = "annotatedUser",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportOnly,
                AttributeName = "annotatedUser",
                PropertyName = "AnnotatedUser",
                Api = "chromeDevice",
                SupportsPatch = true,
                CastForImport = (i) => string.IsNullOrEmpty((string)i) ? null : i,
            };

            type.AttributeAdapters.Add(p);

            p = new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                FieldName = "deviceId",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportOnly,
                AttributeName = "id",
                PropertyName = "DeviceId",
                Api = "chromeDevice",
                SupportsPatch = true,
                IsAnchor = true,
            };

            type.AttributeAdapters.Add(p);

            p = new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                FieldName = "lastSync",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportOnly,
                AttributeName = "lastSync",
                PropertyName = "LastSyncRaw",
                Api = "chromeDevice",
                SupportsPatch = true,
                IsAnchor = false,
                CastForImport = (i) => string.IsNullOrEmpty((string)i) ? null : i,
            };

            type.AttributeAdapters.Add(p);

            p = new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                FieldName = "supportEndDate",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportOnly,
                AttributeName = "supportEndDate",
                PropertyName = "SupportEndDateRaw",
                Api = "chromeDevice",
                SupportsPatch = true,
                IsAnchor = false,
                CastForImport = (i) => string.IsNullOrEmpty((string)i) ? null : i,
            };

            type.AttributeAdapters.Add(p);

            p = new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                FieldName = "annotatedLocation",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportOnly,
                AttributeName = "annotatedLocation",
                PropertyName = "AnnotatedLocation",
                Api = "chromeDevice",
                SupportsPatch = true,
                IsAnchor = false,
                CastForImport = (i) => string.IsNullOrEmpty((string)i) ? null : i,
            };

            type.AttributeAdapters.Add(p);

            p = new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                FieldName = "annotatedAssetId",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportOnly,
                AttributeName = "annotatedAssetId",
                PropertyName = "AnnotatedAssetId",
                Api = "chromeDevice",
                SupportsPatch = true,
                IsAnchor = false,
                CastForImport = (i) => string.IsNullOrEmpty((string)i) ? null : i,
            };

            type.AttributeAdapters.Add(p);

            p = new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                FieldName = "notes",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportOnly,
                AttributeName = "deviceNotes",
                PropertyName = "Notes",
                Api = "chromeDevice",
                SupportsPatch = true,
                IsAnchor = false,
                CastForImport = (i) => string.IsNullOrEmpty((string)i) ? null : i,
            };

            type.AttributeAdapters.Add(p);

            p = new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                FieldName = "model",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportOnly,
                AttributeName = "model",
                PropertyName = "Model",
                Api = "chromeDevice",
                SupportsPatch = true,
                IsAnchor = false,
                CastForImport = (i) => string.IsNullOrEmpty((string)i) ? null : i,
            };

            type.AttributeAdapters.Add(p);

            p = new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                FieldName = "meid",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportOnly,
                AttributeName = "meid",
                PropertyName = "Meid",
                Api = "chromeDevice",
                SupportsPatch = true,
                IsAnchor = false,
                CastForImport = (i) => string.IsNullOrEmpty((string)i) ? null : i,
            };

            type.AttributeAdapters.Add(p);

            p = new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                FieldName = "orderNumber",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportOnly,
                AttributeName = "orderNumber",
                PropertyName = "OrderNumber",
                Api = "chromeDevice",
                SupportsPatch = true,
                IsAnchor = false,
                CastForImport = (i) => string.IsNullOrEmpty((string)i) ? null : i,
            };

            type.AttributeAdapters.Add(p);


            p = new AdapterPropertyValue
            {
                AttributeType = AttributeType.Boolean,
                FieldName = "willAutoRenew",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportOnly,
                AttributeName = "willAutoRenew",
                PropertyName = "WillAutoRenew",
                Api = "chromeDevice",
                SupportsPatch = true,
                IsAnchor = false,
            };

            type.AttributeAdapters.Add(p);

            p = new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                FieldName = "osVersion",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportOnly,
                AttributeName = "osVersion",
                PropertyName = "OsVersion",
                Api = "chromeDevice",
                SupportsPatch = true,
                IsAnchor = false,
                CastForImport = (i) => string.IsNullOrEmpty((string)i) ? null : i,
            };

            type.AttributeAdapters.Add(p);

            p = new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                FieldName = "platformVersion",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportOnly,
                AttributeName = "platformVersion",
                PropertyName = "PlatformVersion",
                Api = "chromeDevice",
                SupportsPatch = true,
                IsAnchor = false,
                CastForImport = (i) => string.IsNullOrEmpty((string)i) ? null : i,
            };

            type.AttributeAdapters.Add(p);

            p = new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                FieldName = "firmwareVersion",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportOnly,
                AttributeName = "firmwareVersion",
                PropertyName = "FirmwareVersion",
                Api = "chromeDevice",
                SupportsPatch = true,
                IsAnchor = false,
                CastForImport = (i) => string.IsNullOrEmpty((string)i) ? null : i,
            };

            type.AttributeAdapters.Add(p);

            p = new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                FieldName = "macAddress",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportOnly,
                AttributeName = "macAddress",
                PropertyName = "MacAddress",
                Api = "chromeDevice",
                SupportsPatch = true,
                IsAnchor = false,
                CastForImport = (i) => string.IsNullOrEmpty((string)i) ? null : i,
            };

            type.AttributeAdapters.Add(p);

            p = new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                FieldName = "bootMode",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportOnly,
                AttributeName = "bootMode",
                PropertyName = "BootMode",
                Api = "chromeDevice",
                SupportsPatch = true,
                IsAnchor = false,
                CastForImport = (i) => string.IsNullOrEmpty((string)i) ? null : i,
            };

            type.AttributeAdapters.Add(p);

            p = new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                FieldName = "orgUnitPath",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportExport,
                AttributeName = "orgUnitPath",
                PropertyName = "OrgUnitPath",
                Api = "chromeDevice",
                SupportsPatch = true,
                IsAnchor = false,
                CastForImport = (i) => string.IsNullOrEmpty((string)i) ? null : i,
            };

            type.AttributeAdapters.Add(p);

            p = new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                FieldName = "lastEnrollmentTime",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportOnly,
                AttributeName = "lastEnrollmentTime",
                PropertyName = "LastEnrollmentTimeRaw",
                Api = "chromeDevice",
                SupportsPatch = true,
                IsAnchor = false,
                CastForImport = (i) => string.IsNullOrEmpty((string)i) ? null : i,
            };

            type.AttributeAdapters.Add(p);

            p = new AdapterPropertyValue
            {
                AttributeType = AttributeType.String,
                FieldName = "ethernetMacAddress",
                IsMultivalued = false,
                Operation = AttributeOperation.ImportOnly,
                AttributeName = "ethernetMacAddress",
                PropertyName = "EthernetMacAddress",
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
