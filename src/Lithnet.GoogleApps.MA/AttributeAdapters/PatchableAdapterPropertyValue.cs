using Microsoft.MetadirectoryServices;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lithnet.GoogleApps.MA.AttributeAdapters
{
    internal class PatchableAdapterPropertyValue : AdapterPropertyValue
    {

        public override bool CanPatch(KeyedCollection<string, AttributeChange> changes)
        {
            // Patching appeared to not be working correctly using AdapterPropertyValue for Course, so override
            // and handle separatly
            return this.SupportsPatch || !changes.Contains(this.AttributeName);
        }

    }
}
