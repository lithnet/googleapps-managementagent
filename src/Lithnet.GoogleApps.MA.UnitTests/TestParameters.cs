using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using Lithnet.Licensing.Core;

namespace Lithnet.GoogleApps.MA.UnitTests
{
    internal class TestParameters : ManagementAgentParametersBase, IManagementAgentParameters, ILicenseDataProvider
    {
        private ILicenseManager<Features, Skus> licenseManager;

        private const string productId = "A5E0C3DC-2003-4C0F-A03E-5413074DB5E2";

        public bool GroupMembersAsString { get; set; } = false;

        public TestParameters()
        {
            this.CalendarBuildingAttributeType = "String";
            this.CalendarFeatureAttributeType = "String";
        }

        public string CalendarFeatureAttributeType { get; set; }

        public string CustomerID => "my_customer";

        public string LicenseKey => ConfigurationManager.AppSettings["licenseKey"];

        public override string ServiceAccountEmailAddress => ConfigurationManager.AppSettings["serviceAccountEmailAddress"];

        public string GroupRegexFilter { get; set; } = "^(?!(abuse|postmaster)).+@?(.+).idmdev1.+$";

        public string UserRegexFilter { get; set; } = "^(?!(abuse|postmaster)).+@?(.+).idmdev1.+$";

        public string UserQueryFilter { get; set; }

        public string ContactRegexFilter { get; set; } = "^(?!(abuse|postmaster)).+@?(.+).idmdev1.+$";

        public bool InheritGroupRoles { get; set; }

        public bool CalendarSendNotificationOnPermissionChange { get; set; }

        public override string UserEmailAddress => ConfigurationManager.AppSettings["userEmailAddress"];

        public string ContactDNPrefix => "contact:";

        public string Domain => ConfigurationManager.AppSettings["domain"];

        public override string KeyFilePath => ConfigurationManager.AppSettings["keyFilePath"];

        public string LogFilePath => ConfigurationManager.AppSettings["logFilePath"];

        public override string KeyFilePassword => ConfigurationManager.AppSettings["keyFilePassword"];

        public bool DoNotGenerateDelta { get; set; }

        public IEnumerable<string> PhonesAttributeFixedTypes
        {
            get
            {
                yield return "work";
                yield return "home";
                yield return "other";
            }
        }

        public IEnumerable<string> OrganizationsAttributeFixedTypes
        {
            get
            {
                yield return "work";
                yield return "home";
                yield return "other";
            }
        }

        public IEnumerable<string> EmailsAttributeFixedTypes
        {
            get
            {
                yield return "work";
                yield return "home";
                yield return "other";
            }
        }

        public IEnumerable<string> IMsAttributeFixedTypes
        {
            get
            {
                yield return "work";
                yield return "home";
                yield return "other";
            }
        }

        public IEnumerable<string> ExternalIDsAttributeFixedTypes
        {
            get
            {
                yield return "work";
                yield return "home";
                yield return "other";
            }
        }

        public IEnumerable<string> RelationsAttributeFixedTypes
        {
            get
            {
                yield return "manager";
            }
        }

        public IEnumerable<string> AddressesAttributeFixedTypes
        {
            get
            {
                yield return "work";
                yield return "home";
                yield return "other";
            }
        }

        public IEnumerable<string> WebsitesAttributeFixedTypes
        {
            get
            {
                yield return "work";
                yield return "home";
                yield return "other";
            }
        }

        public IEnumerable<string> CustomUserObjectClasses
        {
            get
            {
                yield return UnitTestControl.TestUser;
            }
        }

        public bool ExcludeUserCreated { get; set; }

        public string CalendarBuildingAttributeType { get; set; }

        public string MALogFile => Path.Combine(this.LogFilePath, "ma-operations.log");

        public string PasswordOperationLogFile => Path.Combine(this.LogFilePath, "password-operations.log");

        public override bool MembersAsNonReference => this.GroupMembersAsString;
        public bool EnableAdvancedUserAttributes => true;

        public bool MakeNewSendAsAddressesDefault => true;

        public bool SkipMemberImportOnArchivedCourses => false;

        public ILicenseManager<Features, Skus> LicenseManager
        {
            get
            {
                if (this.licenseManager == null)
                {
                    this.licenseManager = new LicenseManager<Features, Skus>(this, this.Domain, productId);
                }

                return this.licenseManager;
            }
        }

        public event EventHandler OnLicenseDataChanged;

        public string GetRawLicenseData()
        {
            return this.LicenseKey;
        }

        public void LicenseDataChanged()
        {
        }
    }
}