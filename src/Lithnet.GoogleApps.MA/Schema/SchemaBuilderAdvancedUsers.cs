using Microsoft.MetadirectoryServices;

namespace Lithnet.GoogleApps.MA
{
    internal class SchemaBuilderAdvancedUsers : SchemaBuilderUsers
    {
        public override string TypeName => "advancedUser";

        public override MASchemaType GetSchemaType(IManagementAgentParameters config)
        {
            if (!SchemaRequestFactory.HasSchema(config.CustomerID, SchemaConstants.CustomGoogleAppsSchemaName))
            {
                return null;
            }

            MASchemaType userType = base.GetSchemaType(config);
            userType.Name = SchemaConstants.AdvancedUser;
            userType.ApiInterface = new ApiInterfaceAdvancedUser(userType);

            AdapterCollection<string> delegates = new AdapterCollection<string>
            {
                AttributeType = AttributeType.Reference,
                FieldName = null,
                Operation = AttributeOperation.ImportExport,
                AttributeName = SchemaConstants.Delegate,
                PropertyName = "Delegates",
                Api = "userdelegates",
                SupportsPatch = true,
            };

            userType.AttributeAdapters.Add(delegates);

            return userType;
        }
    }
}