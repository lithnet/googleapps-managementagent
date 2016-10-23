using System;
using System.Security.Cryptography.X509Certificates;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Admin.Directory.directory_v1;
using Google.Apis.Groupssettings.v1;
using System.Configuration;

namespace Lithnet.GoogleApps.MA
{
    internal abstract class ManagementAgentParametersBase
    {
        protected const string CustomerIDParameter = "Customer ID";

        protected const string InheritGroupRolesParameter = "Inherit group roles";
        
        protected const string DomainParameter = "Primary domain";

        protected const string ServiceAccountEmailAddressParameter = "Service account email address";

        protected const string UserEmailAddressParameter = "User email address";

        protected const string KeyFilePathParameter = "Key file (p12)";

        protected const string KeyFilePasswordParameter = "Key file password";

        protected const string ExcludeUserCreatedGroupsParameter = "Exclude user-created groups";

        protected const string UserRegexFilterParameter = "User regex email address filter";

        protected const string GroupRegexFilterParameter = "Group regex email address filter";

        protected const string ContactRegexFilterParameter = "Contact regex email address filter";

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

        protected const string EmailsFixedTypeFormatParameter = "Email fixed types";

        protected const string WebsitesFormatParameter = "Websites attribute format";

        protected const string WebsitesFixedTypeFormatParameter = "Websites fixed types";

        protected const string LogFilePathParameter = "Log file path";

        protected const string SettingHttpDebugEnabled = "lithnet-google-ma-http-debug-enabled";
        protected const string SettingImportThreadsGroupSettings = "lithnet-google-ma-import-threads-group-settings";
        protected const string SettingImportThreadsGroupMembers = "lithnet-google-ma-import-threads-group-members";
        protected const string SettingExportThreads = "lithnet-google-ma-export-threads";
        protected const string SettingExportThreadsGroupMember = "lithnet-google-ma-export-threads-group-member";
        protected const string SettingExportBatchSizeGroupMember = "lithnet-google-ma-export-batch-size-group-member";
        protected const string SettingPoolSizeContacts = "lithnet-google-ma-pool-size-contacts";
        protected const string SettingPoolSizeEmailSettings = "lithnet-google-ma-pool-size-email-settings";

        protected static string[] RequiredScopes = new string[]
                    {
                        DirectoryService.Scope.AdminDirectoryUser,
                        DirectoryService.Scope.AdminDirectoryGroup,
                        DirectoryService.Scope.AdminDirectoryGroupMember,
                        GroupssettingsService.Scope.AppsGroupsSettings,
                        DirectoryService.Scope.AdminDirectoryUserschema,
                        "https://www.googleapis.com/auth/admin.directory.domain",
                        "https://apps-apis.google.com/a/feeds/emailsettings/2.0/",
                        "http://www.google.com/m8/feeds/contacts/"
                    };

        private X509Certificate2 certificate;

        private ServiceAccountCredential credentials;

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
                this.certificate = new X509Certificate2(Environment.ExpandEnvironmentVariables(path), password, X509KeyStorageFlags.Exportable);
            }

            return this.certificate;
        }

        public static bool HttpDebugEnabled => ConfigurationManager.AppSettings[ManagementAgentParametersBase.SettingHttpDebugEnabled] == "1";

        public static int GroupMemberBatchSize
        {
            get
            {
                string value = ConfigurationManager.AppSettings[ManagementAgentParametersBase.SettingExportBatchSizeGroupMember];

                int result;

                if (int.TryParse(value, out result))
                {
                    return result > 0 ? result : 1;
                }
                else
                {
                    return 1;
                }
            }
        }


        public int GroupSettingsImportThreadCount
        {
            get
            {
                string value = ConfigurationManager.AppSettings[ManagementAgentParametersBase.SettingImportThreadsGroupSettings];

                int result;

                if (int.TryParse(value, out result))
                {
                    return result > 0 ? result : 30;
                }
                else
                {
                    return 30;
                }
            }
        }

        public int GroupMembersImportThreadCount
        {
            get
            {
                string value = ConfigurationManager.AppSettings[ManagementAgentParametersBase.SettingImportThreadsGroupMembers];

                int result;

                if (int.TryParse(value, out result))
                {
                    return result > 0 ? result : 10;
                }
                else
                {
                    return 10;
                }
            }
        }

        public int ExportThreadCount
        {
            get
            {
                string value = ConfigurationManager.AppSettings[ManagementAgentParametersBase.SettingExportThreads];

                int result;

                if (int.TryParse(value, out result))
                {
                    return result > 0 ? result : 30;
                }
                else
                {
                    return 30;
                }
            }
        }

        public int GroupMemberExportThreadCount
        {
            get
            {
                string value = ConfigurationManager.AppSettings[ManagementAgentParametersBase.SettingExportThreadsGroupMember];

                int result;

                if (int.TryParse(value, out result))
                {
                    return result > 0 ? result : 10;
                }
                else
                {
                    return 10;
                }
            }
        }

        public int ContactsServicePoolSize
        {
            get
            {
                string value = ConfigurationManager.AppSettings[ManagementAgentParametersBase.SettingPoolSizeContacts];

                int result;

                if (int.TryParse(value, out result))
                {
                    return result > 0 ? result : 30;
                }
                else
                {
                    return 30;
                }
            }
        }

        public int EmailSettingsServicePoolSize
        {
            get
            {
                string value = ConfigurationManager.AppSettings[ManagementAgentParametersBase.SettingPoolSizeEmailSettings];

                int result;

                if (int.TryParse(value, out result))
                {
                    return result > 0 ? result : 30;
                }
                else
                {
                    return 30;
                }
            }
        }
    }
}