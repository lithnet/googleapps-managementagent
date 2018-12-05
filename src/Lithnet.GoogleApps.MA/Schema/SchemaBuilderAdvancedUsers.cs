using System.Collections.Generic;
using Microsoft.MetadirectoryServices;

namespace Lithnet.GoogleApps.MA
{
    internal class SchemaBuilderAdvancedUsers : SchemaBuilderUsers
    {
        public override string TypeName => "advancedUser";

        public override MASchemaType GetSchemaType(IManagementAgentParameters config)
        {
            if (!config.SchemaService.HasSchema(config.CustomerID, SchemaConstants.CustomGoogleAppsSchemaName))
            {
                return null;
            }

            MASchemaType userType = new MASchemaType
            {
                AttributeAdapters = new List<IAttributeAdapter>(),
                Name = SchemaConstants.AdvancedUser,
                AnchorAttributeNames = new[] { "id" },
                SupportsPatch = true,
            };

            this.BuildBaseSchema(userType, config);

            userType.ApiInterface = new ApiInterfaceAdvancedUser(userType, config);

            AdapterCollection<string> delegates = new AdapterCollection<string>
            {
                AttributeType = AttributeType.Reference,
                FieldName = null,
                Operation = AttributeOperation.ImportExport,
                AttributeName = $"advancedUser_{SchemaConstants.Delegate}",
                PropertyName = "Delegates",
                Api = "userdelegates",
                SupportsPatch = true,
            };

            userType.AttributeAdapters.Add(delegates);

            AdapterCollection<string> sendAs = new AdapterCollection<string>
            {
                AttributeType = AttributeType.String,
                FieldName = null,
                Operation = AttributeOperation.ImportExport,
                AttributeName = $"advancedUser_{SchemaConstants.SendAs}",
                PropertyName = "Delegates",
                Api = "userdelegates",
                SupportsPatch = true,
            };

            userType.AttributeAdapters.Add(sendAs);

            return userType;
        }
    }
}