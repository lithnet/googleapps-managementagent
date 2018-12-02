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

            MASchemaType userType = base.GetSchemaType(config);
            userType.Name = SchemaConstants.AdvancedUser;
            userType.ApiInterface = new ApiInterfaceAdvancedUser(userType, config);

            return userType;
        }
    }
}