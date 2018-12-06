using System.Collections.Generic;

namespace Lithnet.GoogleApps.MA
{
    internal interface ISchemaTypeBuilder
    {
        string TypeName { get; }

        IEnumerable<MASchemaType> GetSchemaTypes(IManagementAgentParameters config);
    }
}
