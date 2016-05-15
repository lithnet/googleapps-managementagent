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
    internal interface IManagementAgentParameters
    {
        int GroupSettingsImportThreadCount { get; }

        int GroupMembersImportThreadCount { get; }
        int ContactsServicePoolSize { get; }

        int EmailSettingsServicePoolSize { get;  }

        int ExportThreadCount { get; }

        ServiceAccountCredential Credentials { get; }

        string CustomerID { get; }

        string ServiceAccountEmailAddress { get; }

        string GroupRegexFilter { get; }

        string UserRegexFilter { get; }

        string ContactRegexFilter { get; }
        
        string UserEmailAddress { get; }

        string KeyFilePath { get; }

        string LogFilePath { get; }
        
        bool DoNotGenerateDelta { get; }

        string Domain { get; }

        IEnumerable<string> PhonesAttributeFixedTypes { get; }

        IEnumerable<string> OrganizationsAttributeFixedTypes { get; }

        IEnumerable<string> EmailsAttributeFixedTypes { get; }
        
        IEnumerable<string> IMsAttributeFixedTypes { get; }

        IEnumerable<string> ExternalIDsAttributeFixedTypes { get; }

        IEnumerable<string> RelationsAttributeFixedTypes { get; }

        IEnumerable<string> AddressesAttributeFixedTypes { get; }

        IEnumerable<string> WebsitesAttributeFixedTypes { get; }

        bool ExcludeUserCreated { get; }

        string KeyFilePassword { get; }
    }
}