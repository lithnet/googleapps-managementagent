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

        private static void BuildSchema()
        {
            UnitTestControl.schema = SchemaBuilder.GetSchema(new TestParameters());
        }
    }
}
