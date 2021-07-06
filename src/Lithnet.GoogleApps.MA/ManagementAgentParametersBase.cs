using System;
using System.Security.Cryptography.X509Certificates;
using Google.Apis.Admin.Directory.directory_v1;
using Google.Apis.Groupssettings.v1;

namespace Lithnet.GoogleApps.MA
{
    internal abstract class ManagementAgentParametersBase
    {
        private volatile X509Certificate2 certificate;

        private object lockObject = new object();

        private volatile GmailServiceRequestFactory gmailService;

        private volatile DomainsRequestFactory domainsService;

        private volatile OrgUnitsRequestFactory orgUnitsService;

        private volatile UserRequestFactory usersService;

        private volatile ContactRequestFactory contactsService;

        private volatile GroupRequestFactory groupsService;

        private volatile ChromeDeviceRequestFactory chromeDeviceService;

        private volatile ResourceRequestFactory resourcesService;

        private volatile SchemaRequestFactory schemaService;

        private volatile ClassroomRequestFactory classroomService;

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

        protected const string LocationsFixedTypeFormatParameter = "Locations fixed types";
        
        protected const string KeywordsFixedTypeFormatParameter = "Keywords fixed types";

        protected const string EmailsFixedTypeFormatParameter = "Email fixed types";

        protected const string CustomUserObjectClassesParameter = "Custom user object classes";

        protected const string WebsitesFormatParameter = "Websites attribute format";

        protected const string WebsitesFixedTypeFormatParameter = "Websites fixed types";

        protected const string LogFilePathParameter = "Log file path";

        protected const string ContactsPrefixParameter = "Contact DN prefix";

        protected const string CalendarBuildingAttributeTypeParameter = "Calendar resource building attribute type";

        protected const string CalendarFeatureAttributeTypeParameter = "Calendar resource features attribute type";

        protected const string GroupMemberAttributeTypeParameter = "Group member attributes type";

        protected const string CalendarSendNotificationOnPermissionChangeParameter = "Send notifications when changing calendar permissions";

        protected const string EnableAdvancedUserAttributesParameter = "Enable advanced user attributes";

        protected const string MakeNewSendAsAddressesDefaultParameter = "Make new SendAs addresses default";

        protected const string SkipMemberImportOnArchivedCoursesParameter = "Skip Member import on ARCHIVED Courses";

        protected const string LicenseKeyParameter = "License key";

        public abstract bool MembersAsNonReference { get; }

        public string GroupMemberAttributeName => this.MembersAsNonReference ? "member_raw" : "member";

        public string GroupManagerAttributeName => this.MembersAsNonReference ? "manager_raw" : "manager";

        public string GroupOwnerAttributeName => this.MembersAsNonReference ? "owner_raw" : "owner";

        public abstract string ServiceAccountEmailAddress { get; }

        public abstract string UserEmailAddress { get; }

        public abstract string KeyFilePath { get; }

        public abstract string KeyFilePassword { get; }

        public GmailServiceRequestFactory GmailService
        {
            get
            {
                if (this.gmailService == null)
                {
                    lock (this.lockObject)
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

                            RateLimiter.SetRateLimitGmailService(MAConfigurationSection.Configuration.GmailApi.RateLimit, new TimeSpan(0, 0, 1));
                        }
                    }
                }

                return this.gmailService;
            }
        }

        public OrgUnitsRequestFactory OrgUnitsService
        {
            get
            {
                if (this.orgUnitsService == null)
                {
                    lock (this.lockObject)
                    {
                        if (this.orgUnitsService == null)
                        {
                            this.orgUnitsService = new OrgUnitsRequestFactory(new GoogleServiceCredentials(this.ServiceAccountEmailAddress, this.UserEmailAddress, this.Certificate), new[] { DirectoryService.Scope.AdminDirectoryOrgunit }, 1);

                            RateLimiter.SetRateLimitDirectoryService(MAConfigurationSection.Configuration.DirectoryApi.RateLimit, new TimeSpan(0, 0, 100));
                        }
                    }
                }

                return this.orgUnitsService;
            }
        }


        public DomainsRequestFactory DomainsService
        {
            get
            {
                if (this.domainsService == null)
                {
                    lock (this.lockObject)
                    {
                        if (this.domainsService == null)
                        {
                            this.domainsService = new DomainsRequestFactory(new GoogleServiceCredentials(this.ServiceAccountEmailAddress, this.UserEmailAddress, this.Certificate), new[] { DirectoryService.Scope.AdminDirectoryDomainReadonly }, 1);

                            RateLimiter.SetRateLimitDirectoryService(MAConfigurationSection.Configuration.DirectoryApi.RateLimit, new TimeSpan(0, 0, 100));
                        }
                    }
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
                    lock (this.lockObject)
                    {
                        if (this.usersService == null)
                        {
                            this.usersService = new UserRequestFactory(
                                new GoogleServiceCredentials(this.ServiceAccountEmailAddress, this.UserEmailAddress, this.Certificate),
                                new[]
                                {
                                    DirectoryService.Scope.AdminDirectoryUser,
                                    DirectoryService.Scope.AdminDirectoryUserschemaReadonly
                                },
                                MAConfigurationSection.Configuration.DirectoryApi.PoolSize);

                            RateLimiter.SetRateLimitDirectoryService(MAConfigurationSection.Configuration.DirectoryApi.RateLimit, new TimeSpan(0, 0, 100));
                        }
                    }
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
                    lock (this.lockObject)
                    {
                        if (this.contactsService == null)
                        {
                            this.contactsService = new ContactRequestFactory(
                                new GoogleServiceCredentials(this.ServiceAccountEmailAddress, this.UserEmailAddress, this.Certificate),
                                new[] { "http://www.google.com/m8/feeds/contacts/" },
                                MAConfigurationSection.Configuration.ContactsApi.PoolSize);

                            RateLimiter.SetRateLimitContactsService(MAConfigurationSection.Configuration.ContactsApi.RateLimit, new TimeSpan(0, 0, 100));
                        }
                    }
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
                    lock (this.lockObject)
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

                            RateLimiter.SetRateLimitDirectoryService(MAConfigurationSection.Configuration.DirectoryApi.RateLimit, new TimeSpan(0, 0, 100));
                            RateLimiter.SetRateLimitGroupSettingsService(MAConfigurationSection.Configuration.GroupSettingsApi.RateLimit, new TimeSpan(0, 0, 100));
                        }
                    }
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
                    lock (this.lockObject)
                    {
                        if (this.resourcesService == null)
                        {
                            this.resourcesService = new ResourceRequestFactory(
                                new GoogleServiceCredentials(this.ServiceAccountEmailAddress, this.UserEmailAddress, this.Certificate),
                                new[] { DirectoryService.Scope.AdminDirectoryResourceCalendar },
                                new[] { "https://www.googleapis.com/auth/calendar" },
                                MAConfigurationSection.Configuration.DirectoryApi.PoolSize,
                                MAConfigurationSection.Configuration.CalendarApi.PoolSize);

                            RateLimiter.SetRateLimitDirectoryService(MAConfigurationSection.Configuration.DirectoryApi.RateLimit, new TimeSpan(0, 0, 100));
                            RateLimiter.SetRateLimitCalendarService(MAConfigurationSection.Configuration.CalendarApi.RateLimit, new TimeSpan(0, 0, 100));
                        }
                    }
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
                    lock (this.lockObject)
                    {
                        if (this.schemaService == null)
                        {
                            this.schemaService = new SchemaRequestFactory(
                                new GoogleServiceCredentials(this.ServiceAccountEmailAddress, this.UserEmailAddress, this.Certificate),
                                new[] { DirectoryService.Scope.AdminDirectoryUserschemaReadonly },
                                MAConfigurationSection.Configuration.DirectoryApi.PoolSize);

                            RateLimiter.SetRateLimitDirectoryService(MAConfigurationSection.Configuration.DirectoryApi.RateLimit, new TimeSpan(0, 0, 100));
                        }
                    }
                }

                return this.schemaService;
            }
        }

        public ChromeDeviceRequestFactory ChromeDeviceService
        {
            get
            {
                if (this.chromeDeviceService == null)
                {
                    lock (this.lockObject)
                    {
                        if (this.chromeDeviceService == null)
                        {
                            this.chromeDeviceService = new ChromeDeviceRequestFactory(
                                new GoogleServiceCredentials(this.ServiceAccountEmailAddress, this.UserEmailAddress, this.Certificate),
                                new[] { DirectoryService.Scope.AdminDirectoryDeviceChromeosReadonly },
                                MAConfigurationSection.Configuration.DirectoryApi.PoolSize);

                            RateLimiter.SetRateLimitDirectoryService(MAConfigurationSection.Configuration.DirectoryApi.RateLimit, new TimeSpan(0, 0, 100));
                        }
                    }
                }

                return this.chromeDeviceService;
            }
        }

        public ClassroomRequestFactory ClassroomService
        {
            get
            {
                if (this.classroomService == null)
                {
                    lock (this.lockObject)
                    {
                        if (this.classroomService == null)
                        {
                            this.classroomService = new ClassroomRequestFactory(
                                new GoogleServiceCredentials(this.ServiceAccountEmailAddress, this.UserEmailAddress, this.Certificate),
                                new[]
                                {
                                    Google.Apis.Classroom.v1.ClassroomService.Scope.ClassroomCourses,
                                    Google.Apis.Classroom.v1.ClassroomService.Scope.ClassroomRosters
                                },
                                MAConfigurationSection.Configuration.ClassroomApi.PoolSize);

                            RateLimiter.SetRateLimitClassroomService(MAConfigurationSection.Configuration.ClassroomApi.RateLimit, new TimeSpan(0, 0, 10));
                        }
                    }
                }

                return this.classroomService;
            }
        }

        public X509Certificate2 Certificate
        {
            get
            {
                if (this.certificate == null)
                {
                    lock (this.lockObject)
                    {
                        if (this.certificate == null)
                        {
                            this.certificate = new X509Certificate2(Environment.ExpandEnvironmentVariables(this.KeyFilePath), this.KeyFilePassword, X509KeyStorageFlags.Exportable);
                        }
                    }
                }

                return this.certificate;
            }
        }
    }
}