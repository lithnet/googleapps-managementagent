using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lithnet.GoogleApps.MA;

namespace Lithnet.GoogleApps.MA.UnitTests
{
    using Lithnet.GoogleApps.MA;

    internal static class UnitTestControl
    {
        private static MASchemaTypes schema;

        private static TestParameters parameters;

        public static MASchemaTypes Schema
        {
            get
            {
                if (UnitTestControl.schema == null)
                {
                    UnitTestControl.BuildSchema();
                }

                return UnitTestControl.schema;

            }
        }

        public static TestParameters TestParameters
        {
            get
            {
                if (UnitTestControl.parameters == null)
                {
                    UnitTestControl.parameters = new TestParameters();
                }

                return UnitTestControl.parameters;
            }
        }


        private static void BuildSchema()
        {
            ConnectionPools.InitializePools(TestParameters.Credentials, 1, 1, 1, 1);
            UnitTestControl.schema = SchemaBuilder.GetSchema(UnitTestControl.TestParameters);
        }
    }
}
