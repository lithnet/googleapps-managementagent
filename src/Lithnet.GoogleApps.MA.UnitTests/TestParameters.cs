using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using Google.Apis.Auth.OAuth2;

namespace Lithnet.GoogleApps.MA.UnitTests
{
    internal class TestParameters : ManagementAgentParametersBase, IManagementAgentParameters
    {
        public TestParameters()
        {
            this.CalendarBuildingAttributeType = "String";
            this.CalendarFeatureAttributeType = "String";
        }

        public string CalendarFeatureAttributeType { get; set; }

        public string CustomerID => "my_customer";

        public string ServiceAccountEmailAddress => ConfigurationManager.AppSettings["serviceAccountEmailAddress"];

        public string GroupRegexFilter { get; set; }

        public string UserRegexFilter { get; set; }
        public string ContactRegexFilter { get; set; }

        public bool InheritGroupRoles { get; set; }

        public string UserEmailAddress => ConfigurationManager.AppSettings["userEmailAddress"];
        public string Domain => ConfigurationManager.AppSettings["domain"];

        public string KeyFilePath => ConfigurationManager.AppSettings["keyFilePath"];

        public string LogFilePath => ConfigurationManager.AppSettings["logFilePath"];

        public string KeyFilePassword => ConfigurationManager.AppSettings["keyFilePassword"];

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

        public bool ExcludeUserCreated { get; set; }

        public ServiceAccountCredential Credentials => this.GetCredentials(
            this.ServiceAccountEmailAddress,
            this.UserEmailAddress,
            this.GetCertificate(this.KeyFilePath, this.KeyFilePassword));

        public string CalendarBuildingAttributeType { get; set; }

        public string MALogFile => Path.Combine(this.LogFilePath, "ma-operations.log");

        public string PasswordOperationLogFile => Path.Combine(this.LogFilePath, "password-operations.log");
    }
}