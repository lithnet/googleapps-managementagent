using System.Collections.Generic;
using Lithnet.Licensing.Core;

namespace Lithnet.GoogleApps.MA
{
    internal interface IManagementAgentParameters
    {
        ILicenseManager<Features, Skus> LicenseManager { get; }

        GmailServiceRequestFactory GmailService { get; }

        DomainsRequestFactory DomainsService { get; }

        OrgUnitsRequestFactory OrgUnitsService { get; }

        UserRequestFactory UsersService { get; }

        ContactRequestFactory ContactsService { get; }

        GroupRequestFactory GroupsService { get; }

        ResourceRequestFactory ResourcesService { get; }

        ChromeDeviceRequestFactory ChromeDeviceService { get; }

        ClassroomRequestFactory ClassroomService { get; }

        SchemaRequestFactory SchemaService { get; }

        string CalendarBuildingAttributeType { get; }

        string CalendarFeatureAttributeType { get; }

        string CustomerID { get; }

        string LicenseKey { get; }

        string ServiceAccountEmailAddress { get; }

        string GroupRegexFilter { get; }

        string UserRegexFilter { get; }

        string UserQueryFilter { get; }

        string ContactRegexFilter { get; }

        string UserEmailAddress { get; }

        string KeyFilePath { get; }

        string MALogFile { get; }

        string PasswordOperationLogFile { get; }

        bool DoNotGenerateDelta { get; }

        bool MembersAsNonReference { get; }

        bool EnableAdvancedUserAttributes { get; }

        bool MakeNewSendAsAddressesDefault { get; }

        bool SkipMemberImportOnArchivedCourses { get; }

        string GroupMemberAttributeName { get; }

        string GroupManagerAttributeName { get; }

        string GroupOwnerAttributeName { get; }

        string ContactDNPrefix { get; }

        string Domain { get; }

        bool InheritGroupRoles { get; }

        bool CalendarSendNotificationOnPermissionChange { get; }

        IEnumerable<string> PhonesAttributeFixedTypes { get; }

        IEnumerable<string> LocationsAttributeFixedTypes { get; }

        IEnumerable<string> KeywordsAttributeFixedTypes { get; }

        IEnumerable<string> OrganizationsAttributeFixedTypes { get; }

        IEnumerable<string> EmailsAttributeFixedTypes { get; }

        IEnumerable<string> IMsAttributeFixedTypes { get; }

        IEnumerable<string> ExternalIDsAttributeFixedTypes { get; }

        IEnumerable<string> RelationsAttributeFixedTypes { get; }

        IEnumerable<string> AddressesAttributeFixedTypes { get; }

        IEnumerable<string> WebsitesAttributeFixedTypes { get; }

        IEnumerable<string> CustomUserObjectClasses { get; }

        bool ExcludeUserCreated { get; }

        string KeyFilePassword { get; }
    }
}