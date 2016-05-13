using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace Lithnet.GoogleApps.MA.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.MetadirectoryServices;
    using System.Collections.ObjectModel;
    using System.Security.Cryptography.X509Certificates;
    using System.IO;
    using Google.Apis.Auth.OAuth2;
    using Google.Apis.Admin.Directory.directory_v1;
    using Google.Apis.Groupssettings.v1;
    using System.Text.RegularExpressions;

    namespace Lithnet.GoogleApps.MA
    {
        internal class TestParameters : ManagementAgentParametersBase, IManagementAgentParameters
        {
            public TestParameters()
            {
            }

            public string CustomerID => "my_customer";

            public string ServiceAccountEmailAddress => ConfigurationManager.AppSettings["serviceAccountEmailAddress"];

            public string GroupRegexFilter { get; set; }

            public string UserRegexFilter { get; set; }
            public string ContactRegexFilter { get; set;  }

            public string UserEmailAddress => ConfigurationManager.AppSettings["userEmailAddress"];
            public string Domain => ConfigurationManager.AppSettings["domain"];
            
            public string KeyFilePath => ConfigurationManager.AppSettings["keyFilePath"];

            public string KeyFilePassword => ConfigurationManager.AppSettings["keyFilePassword"];

            public bool DoNotGenerateDelta { get; set; }

            public IEnumerable<string> PhonesAttributeFixedTypes
            {
                get
                {
                    yield return "work";
                    yield return "home";
                    yield return "other";
                }
            }

            public IEnumerable<string> OrganizationsAttributeFixedTypes
            {
                get
                {
                    yield return "work";
                    yield return "home";
                    yield return "other";
                }
            }

            public IEnumerable<string> EmailsAttributeFixedTypes
            {
                get
                {
                    yield return "work";
                    yield return "home";
                    yield return "other";
                }
            }
            
            public IEnumerable<string> IMsAttributeFixedTypes
            {
                get
                {
                    yield return "work";
                    yield return "home";
                    yield return "other";
                }
            }

            public IEnumerable<string> ExternalIDsAttributeFixedTypes
            {
                get
                {
                    yield return "work";
                    yield return "home";
                    yield return "other";
                }
            }

            public IEnumerable<string> RelationsAttributeFixedTypes
            {
                get
                {
                    yield return "manager";
                }
            }

            public IEnumerable<string> AddressesAttributeFixedTypes
            {
                get
                {
                    yield return "work";
                    yield return "home";
                    yield return "other";
                }
            }

            public IEnumerable<string> WebsitesAttributeFixedTypes
            {
                get
                {
                    yield return "work";
                    yield return "home";
                    yield return "other";
                }
            }
            
            public bool ExcludeUserCreated { get; set; }
           
          
            public new int GroupSettingsImportThreadCount => 1;
            public new int GroupMembersImportThreadCount => 1;

            public new int ExportThreadCount = 1;

            public ServiceAccountCredential Credentials => this.GetCredentials(
                this.ServiceAccountEmailAddress,
                this.UserEmailAddress,
                this.GetCertificate(this.KeyFilePath, this.KeyFilePassword));
        }
    }
}
