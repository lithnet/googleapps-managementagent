using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using G = Google.Apis.Admin.Directory.directory_v1.Data;

namespace Lithnet.GoogleApps.MA
{
    internal static class SchemaBuilder
    {
        public static MASchemaTypes GetSchema(IManagementAgentParameters config)
        {
            MASchemaTypes types = new MASchemaTypes();

            Assembly assembly = Assembly.GetExecutingAssembly();

            foreach (TypeInfo type in assembly.DefinedTypes)
            {
                if (!type.ImplementedInterfaces.Contains(typeof(ISchemaTypeBuilder)))
                {
                    continue;
                }

                ISchemaTypeBuilder builder = (ISchemaTypeBuilder)Activator.CreateInstance(type);
                MASchemaType schemaType = builder.GetSchemaType(config);

                if (schemaType != null)
                {
                    types.Add(schemaType);
                }
            }
            
            return types;
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
