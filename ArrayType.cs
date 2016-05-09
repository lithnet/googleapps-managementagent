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
        HasPrimaryTypes = 2,
        HasComplexFields = 4
    }
}
