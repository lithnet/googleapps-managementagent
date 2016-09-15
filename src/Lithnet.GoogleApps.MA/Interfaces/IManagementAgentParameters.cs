using System.Collections.Generic;
using Google.Apis.Auth.OAuth2;

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

        string MALogFile { get; }
        
        string PasswordOperationLogFile { get; }

        bool DoNotGenerateDelta { get; }

        string Domain { get; }

        bool InheritGroupRoles { get; }

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