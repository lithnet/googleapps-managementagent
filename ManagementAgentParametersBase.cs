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
    public abstract class ManagementAgentParametersBase
    {
        protected const string CustomerIDParameter = "Customer ID";

        protected const string ServiceAccountEmailAddressParameter = "Service account email address";

        protected const string UserEmailAddressParameter = "User email address";

        protected const string KeyFilePathParameter = "Key file (p12)";

        protected const string KeyFilePasswordParameter = "Key file password";

        protected const string ExcludeUserCreatedGroupsParameter = "Exclude user-created groups";

        protected const string UserRegexFilterParameter = "User regex email address filter";

        protected const string GroupRegexFilterParameter = "Group regex email address filter";

        protected const string DoNotGenerateDeltaParameter = "Do not save delta data on export";

        protected const string ExternalIDsFormatParameter = "External IDs format";

        protected const string ExternalIDsFixedTypeFormatParameter = "External IDs fixed types";

        protected const string IMsFormatParameter = "IM Address attribute format";

        protected const string IMsFixedTypeFormatParameter = "IM Address fixed types";

        protected const string OrganizationsFormatParameter = "Organizations attribute format";

        protected const string OrganizationsFixedTypeFormatParameter = "Organizations fixed types";

        protected const string PhonesFormatParameter = "Phones attribute format";

        protected const string PhonesFixedTypeFormatParameter = "Phones fixed types";

        protected const string RelationsFormatParameter = "Relations attribute format";

        protected const string RelationsFixedTypeFormatParameter = "Relations fixed types";

        protected const string AddressesFormatParameter = "Addresses attribute format";

        protected const string AddressesFixedTypeFormatParameter = "Addresses fixed types";

        protected const string WebsitesFormatParameter = "Websites attribute format";

        protected const string WebsitesFixedTypeFormatParameter = "Websites fixed types";

        protected static string[] RequiredScopes = new string[] 
                    { 
                        DirectoryService.Scope.AdminDirectoryUser, 
                        DirectoryService.Scope.AdminDirectoryGroup, 
                        DirectoryService.Scope.AdminDirectoryGroupMember, 
                        GroupssettingsService.Scope.AppsGroupsSettings,
                        DirectoryService.Scope.AdminDirectoryUserschema,
                        "https://www.googleapis.com/auth/admin.directory.domain"
                    };

        private X509Certificate2 certificate;

        private ServiceAccountCredential credentials;

        protected ManagementAgentParametersBase()
        {
            this.GroupMembersImportThreadCount = 10;
            this.GroupSettingsImportThreadCount = 30;
            this.ExportThreadCount = 30;
        }

        public int GroupSettingsImportThreadCount { get; protected set; }

        public int GroupMembersImportThreadCount { get; protected set; }

        public int ExportThreadCount { get; protected set; }

        protected ServiceAccountCredential GetCredentials(string serviceAccountEmailAddress, string userEmailAddress, X509Certificate2 cert)
        {
            if (this.credentials == null)
            {
                this.credentials = new ServiceAccountCredential(
                new ServiceAccountCredential.Initializer(serviceAccountEmailAddress)
                {
                    Scopes = ManagementAgentParametersBase.RequiredScopes,
                    User = userEmailAddress
                }
                .FromCertificate(cert));
            }

            return this.credentials;
        }

        protected X509Certificate2 GetCertificate(string path, string password)
        {
            if (this.certificate == null)
            {
                this.certificate = new X509Certificate2(path, password, X509KeyStorageFlags.Exportable);
            }

            return this.certificate;
        }
    }
}