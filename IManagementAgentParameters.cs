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
    public interface IManagementAgentParameters
    {
        int GroupSettingsImportThreadCount { get; }

        int GroupMembersImportThreadCount { get; }

        int ExportThreadCount { get; }

        ServiceAccountCredential Credentials { get; }

        string CustomerID { get; }

        string ServiceAccountEmailAddress { get; }

        string GroupRegexFilter { get; }

        string UserRegexFilter { get; }

        string UserEmailAddress { get; }

        string KeyFilePath { get; }

        bool DoNotGenerateDelta { get; }

        GoogleArrayMode PhonesAttributeFormat { get; }

        IEnumerable<string> PhonesAttributeFixedTypes { get; }

        GoogleArrayMode OrganizationsAttributeFormat { get; }

        IEnumerable<string> OrganizationsAttributeFixedTypes { get; }

        GoogleArrayMode IMsAttributeFormat { get; }

        IEnumerable<string> IMsAttributeFixedTypes { get; }

        GoogleArrayMode ExternalIDsAttributeFormat { get; }

        IEnumerable<string> ExternalIDsAttributeFixedTypes { get; }

        GoogleArrayMode RelationsAttributeFormat { get; }

        IEnumerable<string> RelationsAttributeFixedTypes { get; }

        GoogleArrayMode AddressesAttributeFormat { get; }

        IEnumerable<string> AddressesAttributeFixedTypes { get; }

        GoogleArrayMode WebsitesAttributeFormat { get; }

        IEnumerable<string> WebsitesAttributeFixedTypes { get; }

        bool ExcludeUserCreated { get; }

        string KeyFilePassword { get; }
    }
}