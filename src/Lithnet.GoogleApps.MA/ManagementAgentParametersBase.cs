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

        protected const string ContactsPrefixParameter = "Contact DN prefix";

        protected const string CalendarBuildingAttributeTypeParameter = "Calendar resource building attribute type";

        protected const string CalendarFeatureAttributeTypeParameter = "Calendar resource features attribute type";

        protected const string GroupMemberAttributeTypeParameter = "Group member attributes type";

        protected const string CalendarSendNotificationOnPermissionChangeParameter = "Send notifications when changing calendar permissions";

        protected const string EnableAdvancedUserAttributesParameter = "Enable advanced user attributes";


        //protected static string[] AllScopes = new string[]
        //{
        //    DirectoryService.Scope.AdminDirectoryUser,
        //    DirectoryService.Scope.AdminDirectoryGroup,
        //    DirectoryService.Scope.AdminDirectoryGroupMember,
        //    DirectoryService.Scope.AdminDirectoryUserschemaReadonly,
        //    DirectoryService.Scope.AdminDirectoryResourceCalendar,
        //    GroupssettingsService.Scope.AppsGroupsSettings,
        //    "https://www.googleapis.com/auth/admin.directory.domain.readonly",
        //    "https://apps-apis.google.com/a/feeds/emailsettings/2.0/",
        //    "http://www.google.com/m8/feeds/contacts/",
        //    "https://www.googleapis.com/auth/calendar",
        //    Google.Apis.Gmail.v1.GmailService.Scope.GmailSettingsBasic,
        //    Google.Apis.Gmail.v1.GmailService.Scope.GmailSettingsSharing
        //};

        //internal static string[] PasswordChangeScopes = new string[]
        //{
        //    DirectoryService.Scope.AdminDirectoryUser,
        //};

        //internal static string[] SchemaDiscoveryScopes = new string[]
        //{
        //    DirectoryService.Scope.AdminDirectoryUserschemaReadonly,
        //};

        //internal static string[] GetRequiredScopes(Schema types)
        //{
        //    HashSet<string> requiredScopes = new HashSet<string>();

        //    if (types.Types.Contains(SchemaConstants.User))
        //    {
        //        requiredScopes.Add(DirectoryService.Scope.AdminDirectoryUser);
        //        requiredScopes.Add(DirectoryService.Scope.AdminDirectoryUserschemaReadonly);

        //        if (types.Types[SchemaConstants.User].Attributes.Contains(SchemaConstants.Delegate)
        //            || types.Types[SchemaConstants.User].Attributes.Contains(SchemaConstants.SendAs))
        //        {
        //            requiredScopes.Add(Google.Apis.Gmail.v1.GmailService.Scope.GmailSettingsBasic);
        //            requiredScopes.Add(Google.Apis.Gmail.v1.GmailService.Scope.GmailSettingsSharing);
        //        }
        //    }

        //    if (types.Types.Contains(SchemaConstants.AdvancedUser))
        //    {
        //        requiredScopes.Add(DirectoryService.Scope.AdminDirectoryUser);
        //        requiredScopes.Add(DirectoryService.Scope.AdminDirectoryUserschemaReadonly);

        //        if (types.Types[SchemaConstants.AdvancedUser].Attributes.Contains(SchemaConstants.Delegate)
        //            || types.Types[SchemaConstants.AdvancedUser].Attributes.Contains(SchemaConstants.SendAs))
        //        {
        //            requiredScopes.Add(Google.Apis.Gmail.v1.GmailService.Scope.GmailSettingsBasic);
        //            requiredScopes.Add(Google.Apis.Gmail.v1.GmailService.Scope.GmailSettingsSharing);
        //        }
        //    }

        //    if (types.Types.Contains(SchemaConstants.Group))
        //    {
        //        requiredScopes.Add(DirectoryService.Scope.AdminDirectoryGroup);
        //        requiredScopes.Add(DirectoryService.Scope.AdminDirectoryGroupMember);
        //        requiredScopes.Add(GroupssettingsService.Scope.AppsGroupsSettings);
        //        requiredScopes.Add("https://www.googleapis.com/auth/admin.directory.domain.readonly");
        //    }

        //    if (types.Types.Contains(SchemaConstants.Contact))
        //    {
        //        requiredScopes.Add("http://www.google.com/m8/feeds/contacts/");
        //    }

        //    if (types.Types.Contains(SchemaConstants.Calendar))
        //    {
        //        requiredScopes.Add(DirectoryService.Scope.AdminDirectoryResourceCalendar);
        //        requiredScopes.Add("https://www.googleapis.com/auth/calendar");
        //    }

        //    if (types.Types.Contains(SchemaConstants.Feature))
        //    {
        //        requiredScopes.Add(DirectoryService.Scope.AdminDirectoryResourceCalendar);
        //    }

        //    if (types.Types.Contains(SchemaConstants.Building))
        //    {
        //        requiredScopes.Add(DirectoryService.Scope.AdminDirectoryResourceCalendar);
        //    }

        //    if (types.Types.Contains(SchemaConstants.Domain))
        //    {
        //        requiredScopes.Add("https://www.googleapis.com/auth/admin.directory.domain.readonly");
        //    }

        //    return requiredScopes.ToArray();
        //}

        public abstract bool MembersAsNonReference { get; }

        public string GroupMemberAttributeName => this.MembersAsNonReference ? "member_raw" : "member";

        public string GroupManagerAttributeName => this.MembersAsNonReference ? "manager_raw" : "manager";

        public string GroupOwnerAttributeName => this.MembersAsNonReference ? "owner_raw" : "owner";

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

            new ServiceAccountCredential.Initializer(serviceAccountEmailAddress)
            {
                Scopes = scopes,
                User = userEmailAddress
            }
                .FromCertificate(cert);

            return this.credentials;
        }

        public abstract string ServiceAccountEmailAddress { get; }

        public abstract string UserEmailAddress { get; }

        public abstract string KeyFilePath { get; }

        public abstract string KeyFilePassword { get; }

        private GmailServiceRequestFactory gmailService;

        private DomainsRequestFactory domainsService;

        private UserRequestFactory usersService;

        private ContactRequestFactory contactsService;

        private GroupRequestFactory groupsService;

        private ResourceRequestFactory resourcesService;

        private SchemaRequestFactory schemaService;

        public GmailServiceRequestFactory GmailService
        {
            get
            {
                if (this.gmailService == null)
                {
                    this.gmailService = new GmailServiceRequestFactory(
                        this.ServiceAccountEmailAddress,
                        this.Certificate,
                        new string[]
                        {
                            Google.Apis.Gmail.v1.GmailService.Scope.GmailSettingsSharing,
                            Google.Apis.Gmail.v1.GmailService.Scope.GmailSettingsBasic
                        }
                    );
                }

                return this.gmailService;
            }
        }

        public DomainsRequestFactory DomainsService
        {
            get
            {
                if (this.domainsService == null)
                {
                    this.domainsService = new DomainsRequestFactory(new GoogleServiceCredentials(this.ServiceAccountEmailAddress, this.UserEmailAddress, this.Certificate), new[] { "https://www.googleapis.com/auth/admin.directory.domain.readonly" }, 1);
                }

                return this.domainsService;
            }
        }

        public UserRequestFactory UsersService
        {
            get
            {
                if (this.usersService == null)
                {
                    this.usersService = new UserRequestFactory(
                        new GoogleServiceCredentials(this.ServiceAccountEmailAddress, this.UserEmailAddress, this.Certificate),
                        new[] {
                            DirectoryService.Scope.AdminDirectoryUser,
                            DirectoryService.Scope.AdminDirectoryUserschemaReadonly },
                        MAConfigurationSection.Configuration.DirectoryApi.PoolSize);
                }

                return this.usersService;
            }
        }

        public ContactRequestFactory ContactsService
        {
            get
            {
                if (this.contactsService == null)
                {
                    this.contactsService = new ContactRequestFactory(
                        new GoogleServiceCredentials(this.ServiceAccountEmailAddress, this.UserEmailAddress, this.Certificate),
                        new[] { "http://www.google.com/m8/feeds/contacts/" },
                        MAConfigurationSection.Configuration.ContactsApi.PoolSize);
                }

                return this.contactsService;
            }
        }

        public GroupRequestFactory GroupsService
        {
            get
            {
                if (this.groupsService == null)
                {
                    this.groupsService = new GroupRequestFactory(
                        new GoogleServiceCredentials(this.ServiceAccountEmailAddress, this.UserEmailAddress, this.Certificate),
                        new[]
                        {
                            DirectoryService.Scope.AdminDirectoryGroup,
                            DirectoryService.Scope.AdminDirectoryGroupMember,
                        },
                        new[]
                        {
                            GroupssettingsService.Scope.AppsGroupsSettings
                        },
                        MAConfigurationSection.Configuration.DirectoryApi.PoolSize,
                        MAConfigurationSection.Configuration.GroupSettingsApi.PoolSize
                        );

                    GroupMemberRequestFactory.BatchSize = MAConfigurationSection.Configuration.DirectoryApi.BatchSizeGroupMember;
                    GroupMemberRequestFactory.ConcurrentOperationLimitDefault = MAConfigurationSection.Configuration.DirectoryApi.ConcurrentOperationGroupMember;
                }

                return this.groupsService;
            }
        }

        public ResourceRequestFactory ResourcesService
        {
            get
            {
                if (this.resourcesService == null)
                {
                    this.resourcesService = new ResourceRequestFactory(
                        new GoogleServiceCredentials(this.ServiceAccountEmailAddress, this.UserEmailAddress, this.Certificate),
                        new[] { DirectoryService.Scope.AdminDirectoryResourceCalendar },
                        new[] { "https://www.googleapis.com/auth/calendar" },
                        MAConfigurationSection.Configuration.DirectoryApi.PoolSize,
                        MAConfigurationSection.Configuration.CalendarApi.PoolSize);
                }

                return this.resourcesService;
            }
        }

        public SchemaRequestFactory SchemaService
        {
            get
            {
                if (this.schemaService == null)
                {
                    this.schemaService = new SchemaRequestFactory(
                        new GoogleServiceCredentials(this.ServiceAccountEmailAddress, this.UserEmailAddress, this.Certificate),
                        new[] { DirectoryService.Scope.AdminDirectoryUserschemaReadonly },
                        MAConfigurationSection.Configuration.DirectoryApi.PoolSize);
                }

                return this.schemaService;
            }
        }

        public X509Certificate2 Certificate
        {
            get
            {
                if (this.certificate == null)
                {
                    this.certificate = new X509Certificate2(Environment.ExpandEnvironmentVariables(this.KeyFilePath), this.KeyFilePassword, X509KeyStorageFlags.Exportable);
                }

                return this.certificate;
            }
        }
    }
}