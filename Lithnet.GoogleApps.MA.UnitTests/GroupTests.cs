using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Lithnet.GoogleApps;
using Lithnet.GoogleApps.MA;
using Lithnet.GoogleApps.ManagedObjects;

namespace Lithnet.GoogleApps.MA.UnitTests
{
    using System.Linq;
    using Lithnet.GoogleApps.MA;

    [TestClass]
    public class GroupTests
    {
        [TestMethod]
        public void TestMethod1()
        {
            TestParameters r = new TestParameters();

            ConnectionPools.InitializePools(r.Credentials, 1, 1);

            List<string> items = UserSettingsRequestFactory.GetDelegates("amut0001-student.monash.edu-d1@ga-staff-dev.monash.edu").ToList();


        }
    }
}
