using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lithnet.GoogleApps.MA
{
    using ManagedObjects;

    public class UserUpdateTemplate
    {
        public User User { get; set; }

        public List<string> Aliases { get; set; }

        public List<string> NonAliases { get; set; }
    }
}
