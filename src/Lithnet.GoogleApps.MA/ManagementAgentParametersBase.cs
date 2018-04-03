using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Google.Apis.Admin.Directory.directory_v1;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Groupssettings.v1;
using Microsoft.MetadirectoryServices;

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

        protected const string UserQueryFilterParameter = "User API query parameter";
        
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

        protected const string CalendarBuildingAttributeTypeParameter = "Calendar resource building attribute type";

        protected const string CalendarFeatureAttributeTypeParameter = "Calendar resource features attribute type";

        protected const string CalendarSendNotificationOnPermissionChangeParameter = "Send notifications when changing calendar permissions";

        protected static string[] AllScopes = new string[]
        {
            DirectoryService.Scope.AdminDirectoryUser,
            DirectoryService.Scope.AdminDirectoryGroup,
            DirectoryService.Scope.AdminDirectoryGroupMember,
            DirectoryService.Scope.AdminDirectoryUserschemaReadonly,
            DirectoryService.Scope.AdminDirectoryResourceCalendar,
            GroupssettingsService.Scope.AppsGroupsSettings,
            "https://www.googleapis.com/auth/admin.directory.domain",
            "https://apps-apis.google.com/a/feeds/emailsettings/2.0/",
            "http://www.google.com/m8/feeds/contacts/",
            "https://www.googleapis.com/auth/calendar"
        };

        internal static string[] PasswordChangeScopes = new string[]
        {
            DirectoryService.Scope.AdminDirectoryUser,
        };

        internal static string[] SchemaDiscoveryScopes = new string[]
        {
            DirectoryService.Scope.AdminDirectoryUserschemaReadonly,
        };

        internal static string[] GetRequiredScopes(Schema types)
        {
            HashSet<string> requiredScopes = new HashSet<string>();

            if (types.Types.Contains(SchemaConstants.User))
            {
                requiredScopes.Add(DirectoryService.Scope.AdminDirectoryUser);
                requiredScopes.Add(DirectoryService.Scope.AdminDirectoryUserschemaReadonly);
            }

            if (types.Types.Contains(SchemaConstants.AdvancedUser))
            {
                requiredScopes.Add(DirectoryService.Scope.AdminDirectoryUser);
                requiredScopes.Add(DirectoryService.Scope.AdminDirectoryUserschemaReadonly);
                requiredScopes.Add("https://apps-apis.google.com/a/feeds/emailsettings/2.0/");
            }

            if (types.Types.Contains(SchemaConstants.Group))
            {
                requiredScopes.Add(DirectoryService.Scope.AdminDirectoryGroup);
                requiredScopes.Add(DirectoryService.Scope.AdminDirectoryGroupMember);
                requiredScopes.Add(GroupssettingsService.Scope.AppsGroupsSettings);
                requiredScopes.Add("https://www.googleapis.com/auth/admin.directory.domain.readonly");
            }

            if (types.Types.Contains(SchemaConstants.Contact))
            {
                requiredScopes.Add("http://www.google.com/m8/feeds/contacts/");
            }

            if (types.Types.Contains(SchemaConstants.Calendar))
            {
                requiredScopes.Add(DirectoryService.Scope.AdminDirectoryResourceCalendar);
                requiredScopes.Add("https://www.googleapis.com/auth/calendar");
            }

            if (types.Types.Contains(SchemaConstants.Feature))
            {
                requiredScopes.Add(DirectoryService.Scope.AdminDirectoryResourceCalendar);
            }

            if (types.Types.Contains(SchemaConstants.Building))
            {
                requiredScopes.Add(DirectoryService.Scope.AdminDirectoryResourceCalendar);
            }

            if (types.Types.Contains(SchemaConstants.Domain))
            {
                requiredScopes.Add("https://www.googleapis.com/auth/admin.directory.domain.readonly");
            }

            return requiredScopes.ToArray();
        }

        private X509Certificate2 certificate;

        private ServiceAccountCredential credentials;

        protected ServiceAccountCredential GetCredentials(string serviceAccountEmailAddress, string userEmailAddress, X509Certificate2 cert, string[] scopes)
        {
            if (this.credentials == null)
            {
                this.credentials = new ServiceAccountCredential(
                    new ServiceAccountCredential.Initializer(serviceAccountEmailAddress)
                    {
                        Scopes = scopes,
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
    }
}