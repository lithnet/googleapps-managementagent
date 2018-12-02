using System.Collections.Concurrent;
using System.Threading.Tasks;
using Microsoft.MetadirectoryServices;

namespace Lithnet.GoogleApps.MA
{

    internal interface IApiInterfaceObject : IApiInterface
    {
        object CreateInstance(CSEntryChange csentry);

        object GetInstance(CSEntryChange csentry);

        void DeleteInstance(CSEntryChange csentry);

        string GetAnchorValue(string name, object target);

        string GetDNValue(object target);

        ObjectModificationType DeltaUpdateType { get; }

        Task GetItems(Schema schema, BlockingCollection<object> collection);
    }
}
