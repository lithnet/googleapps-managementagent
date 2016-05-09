using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lithnet.GoogleApps.MA
{
    [Flags]
    public enum ArrayType
    {
        None = 0,
        HasTypes = 1,
        HasComplexFields = 4
    }
}
