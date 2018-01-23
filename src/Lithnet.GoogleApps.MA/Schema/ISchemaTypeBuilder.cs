namespace Lithnet.GoogleApps.MA
{
    internal interface ISchemaTypeBuilder
    {
        string TypeName { get; }

        MASchemaType GetSchemaType(IManagementAgentParameters config);
    }
}
