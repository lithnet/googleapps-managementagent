using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lithnet.GoogleApps.MA
{
    using Microsoft.MetadirectoryServices;

    internal interface IApiInterfaceObject :IApiInterface
    {
        object CreateInstance(CSEntryChange csentry);

        object GetInstance(CSEntryChange csentry);

        void DeleteInstance(CSEntryChange csentry);

        string GetAnchorValue(object target);

        string GetDNValue(object target);
    }
}
