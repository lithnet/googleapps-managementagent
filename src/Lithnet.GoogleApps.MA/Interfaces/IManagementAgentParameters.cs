using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Google.Apis.Auth.OAuth2;

namespace Lithnet.GoogleApps.MA
{
    internal interface IManagementAgentParameters
    {
        GmailServiceRequestFactory GmailService { get; }

        DomainsRequestFactory DomainsService { get; }

        UserRequestFactory UsersService { get; }

        ContactRequestFactory ContactsService { get; }

        GroupRequestFactory GroupsService { get; }

        ResourceRequestFactory ResourcesService { get; }

        SchemaRequestFactory SchemaService { get; }

        string CalendarBuildingAttributeType { get; }

        string CalendarFeatureAttributeType { get; }

        string CustomerID { get; }

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

        string GroupMemberAttributeName { get; }

        string GroupManagerAttributeName { get; }

        string GroupOwnerAttributeName { get; }

        string ContactDNPrefix { get; }

        string Domain { get; }

        bool InheritGroupRoles { get; }

        bool CalendarSendNotificationOnPermissionChange { get; }

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