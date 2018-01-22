using System;
using System.Collections.Generic;
using G = Google.Apis.Admin.Directory.directory_v1.Data;

namespace Lithnet.GoogleApps.MA
{
    internal static class SchemaBuilder
    {
        public static MASchemaTypes GetSchema(IManagementAgentParameters config)
        {
            MASchemaTypes types = new MASchemaTypes
            {
                SchemaBuilder.GetSchema(SchemaConstants.User, config),
                SchemaBuilder.GetSchema(SchemaConstants.Group, config),
                SchemaBuilder.GetSchema(SchemaConstants.Contact, config),
                SchemaBuilder.GetSchema(SchemaConstants.Calendar, config),
                SchemaBuilder.GetSchema(SchemaConstants.Domain, config),
                SchemaBuilder.GetSchema(SchemaConstants.Building, config),
                SchemaBuilder.GetSchema(SchemaConstants.Feature, config)
            };

            if (SchemaRequestFactory.HasSchema(config.CustomerID, SchemaConstants.CustomGoogleAppsSchemaName))
            {
                types.Add(SchemaBuilder.GetSchema(SchemaConstants.AdvancedUser, config));
            }

            return types;
        }

        public static MASchemaType GetSchema(string type, IManagementAgentParameters config)
        {
            switch (type)
            {
                case SchemaConstants.User:
                    return SchemaBuilderUsers.GetUserSchema(config);

                case SchemaConstants.AdvancedUser:
                    return SchemaBuilderUsers.GetAdvancedUserSchema(config);

                case SchemaConstants.Group:
                    return SchemaBuilderGroups.GetGroupSchema();

                case SchemaConstants.Contact:
                    return SchemaBuilderContacts.GetContactSchema(config);

                case SchemaConstants.Domain:
                    return SchemaBuilderDomains.GetDomainSchema(config);

                case SchemaConstants.Calendar:
                    return SchemaBuilderResources.GetCalendarSchema(config);

                case SchemaConstants.Building:
                    return SchemaBuilderResources.GetBuildingSchema(config);

                case SchemaConstants.Feature:
                    return SchemaBuilderResources.GetFeatureSchema(config);
            }

            throw new InvalidOperationException();
        }

        public static void CreateGoogleAppsCustomSchema()
        {
            G.Schema schema = new G.Schema();

            schema.SchemaName = SchemaConstants.CustomGoogleAppsSchemaName;
            schema.Fields = new List<G.SchemaFieldSpec>();
            schema.Fields.Add(new G.SchemaFieldSpec()
            {
                FieldName = SchemaConstants.CustomSchemaObjectType,
                FieldType = "STRING",
                MultiValued = false,
                ReadAccessType = "ADMINS_AND_SELF"
            });

            SchemaRequestFactory.CreateSchema("my_customer", schema);
        }
    }
}
