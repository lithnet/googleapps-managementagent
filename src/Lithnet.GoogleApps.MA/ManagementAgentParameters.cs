using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using Google.Apis.Auth.OAuth2;
using Microsoft.MetadirectoryServices;

namespace Lithnet.GoogleApps.MA
{
    internal class ManagementAgentParameters : ManagementAgentParametersBase, IManagementAgentParameters
    {
        private KeyedCollection<string, ConfigParameter> configParameters;

        public ManagementAgentParameters(KeyedCollection<string, ConfigParameter> configParameters)
        {
            this.configParameters = configParameters;
        }

        public string CustomerID
        {
            get
            {
                if (this.configParameters.Contains(ManagementAgentParametersBase.CustomerIDParameter))
                {
                    return this.configParameters[ManagementAgentParametersBase.CustomerIDParameter].Value;
                }
                else
                {
                    return "my_customer";
                }
            }
        }

        public string CalendarBuildingAttributeType
        {
            get
            {
                if (this.configParameters.Contains(ManagementAgentParametersBase.CalendarBuildingAttributeTypeParameter))
                {
                    string value = this.configParameters[ManagementAgentParametersBase.CalendarBuildingAttributeTypeParameter].Value;

                    if (value == "String" || value == "Reference")
                    {
                        return value;
                    }
                }

                return "String";
            }
        }

        public string CalendarFeatureAttributeType
        {
            get
            {
                if (this.configParameters.Contains(ManagementAgentParametersBase.CalendarFeatureAttributeTypeParameter))
                {
                    string value = this.configParameters[ManagementAgentParametersBase.CalendarFeatureAttributeTypeParameter].Value;

                    if (value == "String" || value == "Reference")
                    {
                        return value;
                    }
                }

                return "String";
            }
        }


        public bool InheritGroupRoles
        {
            get
            {
                if (this.configParameters.Contains(ManagementAgentParametersBase.InheritGroupRolesParameter))
                {
                    string value = this.configParameters[ManagementAgentParametersBase.InheritGroupRolesParameter].Value;

                    if (string.IsNullOrWhiteSpace(value))
                    {
                        return false;
                    }
                    else
                    {
                        return Convert.ToBoolean(Convert.ToInt32(value));
                    }
                }
                else
                {
                    return false;
                }
            }
        }

        public bool CalendarSendNotificationOnPermissionChange
        {
            get
            {
                if (this.configParameters.Contains(ManagementAgentParametersBase.CalendarSendNotificationOnPermissionChangeParameter))
                {
                    string value = this.configParameters[ManagementAgentParametersBase.CalendarSendNotificationOnPermissionChangeParameter].Value;

                    if (string.IsNullOrWhiteSpace(value))
                    {
                        return false;
                    }
                    else
                    {
                        return Convert.ToBoolean(Convert.ToInt32(value));
                    }
                }
                else
                {
                    return false;
                }
            }
        }

        public string ServiceAccountEmailAddress
        {
            get
            {
                if (this.configParameters.Contains(ManagementAgentParametersBase.ServiceAccountEmailAddressParameter))
                {
                    return this.configParameters[ManagementAgentParametersBase.ServiceAccountEmailAddressParameter].Value;
                }
                else
                {
                    return null;
                }
            }
        }

        public string GroupRegexFilter
        {
            get
            {
                if (this.configParameters.Contains(ManagementAgentParametersBase.GroupRegexFilterParameter))
                {
                    return this.configParameters[ManagementAgentParametersBase.GroupRegexFilterParameter].Value;
                }
                else
                {
                    return null;
                }
            }
        }

        public string UserRegexFilter
        {
            get
            {
                if (this.configParameters.Contains(ManagementAgentParametersBase.UserRegexFilterParameter))
                {
                    return this.configParameters[ManagementAgentParametersBase.UserRegexFilterParameter].Value;
                }
                else
                {
                    return null;
                }
            }
        }

        public string ContactRegexFilter
        {
            get
            {
                if (this.configParameters.Contains(ManagementAgentParametersBase.ContactRegexFilterParameter))
                {
                    return this.configParameters[ManagementAgentParametersBase.ContactRegexFilterParameter].Value;
                }
                else
                {
                    return null;
                }
            }
        }

        public string Domain
        {
            get
            {
                if (this.configParameters.Contains(ManagementAgentParametersBase.DomainParameter))
                {
                    return this.configParameters[ManagementAgentParametersBase.DomainParameter].Value;
                }
                else
                {
                    return null;
                }
            }
        }

        public string UserEmailAddress
        {
            get
            {
                if (this.configParameters.Contains(ManagementAgentParametersBase.UserEmailAddressParameter))
                {
                    return this.configParameters[ManagementAgentParametersBase.UserEmailAddressParameter].Value;
                }
                else
                {
                    return null;
                }
            }
        }

        public string KeyFilePath
        {
            get
            {
                if (this.configParameters.Contains(ManagementAgentParametersBase.KeyFilePathParameter))
                {
                    return this.configParameters[ManagementAgentParametersBase.KeyFilePathParameter].Value;
                }
                else
                {
                    return null;
                }
            }
        }

        public string LogFilePath
        {
            get
            {
                if (this.configParameters.Contains(ManagementAgentParametersBase.LogFilePathParameter))
                {
                    return this.configParameters[ManagementAgentParametersBase.LogFilePathParameter].Value;
                }
                else
                {
                    return null;
                }
            }
        }

        public string MALogFile
        {
            get
            {
                string root = Path.GetDirectoryName(this.LogFilePath);

                return root == null ? null : Path.Combine(root, "ma-operations.log");
            }
        }

        public string PasswordOperationLogFile
        {
            get
            {
                string root = Path.GetDirectoryName(this.LogFilePath);

                return root == null ? null : Path.Combine(root, "password-operations.log");
            }
        }

        public bool DoNotGenerateDelta
        {
            get
            {
                if (this.configParameters.Contains(ManagementAgentParametersBase.DoNotGenerateDeltaParameter))
                {
                    string value = this.configParameters[ManagementAgentParametersBase.DoNotGenerateDeltaParameter].Value;

                    if (string.IsNullOrWhiteSpace(value))
                    {
                        return false;
                    }
                    else
                    {
                        return Convert.ToBoolean(Convert.ToInt32(value));
                    }
                }
                else
                {
                    return false;
                }
            }
        }

        public IEnumerable<string> PhonesAttributeFixedTypes
        {
            get
            {
                if (this.configParameters.Contains(ManagementAgentParametersBase.PhonesFixedTypeFormatParameter))
                {
                    string value = this.configParameters[ManagementAgentParametersBase.PhonesFixedTypeFormatParameter].Value;

                    if (string.IsNullOrWhiteSpace(value))
                    {
                        yield break;
                    }

                    foreach (string name in value.Split('\n'))
                    {
                        yield return name;
                    }
                }
                else
                {
                    yield break;
                }
            }
        }

        public IEnumerable<string> OrganizationsAttributeFixedTypes
        {
            get
            {
                if (this.configParameters.Contains(ManagementAgentParametersBase.OrganizationsFixedTypeFormatParameter))
                {
                    string value = this.configParameters[ManagementAgentParametersBase.OrganizationsFixedTypeFormatParameter].Value;

                    if (string.IsNullOrWhiteSpace(value))
                    {
                        yield break;
                    }

                    foreach (string name in value.Split('\n'))
                    {
                        yield return name;
                    }
                }
                else
                {
                    yield break;
                }
            }
        }

        public IEnumerable<string> IMsAttributeFixedTypes
        {
            get
            {
                if (this.configParameters.Contains(ManagementAgentParametersBase.IMsFixedTypeFormatParameter))
                {
                    string value = this.configParameters[ManagementAgentParametersBase.IMsFixedTypeFormatParameter].Value;

                    if (string.IsNullOrWhiteSpace(value))
                    {
                        yield break;
                    }

                    foreach (string name in value.Split('\n'))
                    {
                        yield return name;
                    }
                }
                else
                {
                    yield break;
                }
            }
        }

        public IEnumerable<string> ExternalIDsAttributeFixedTypes
        {
            get
            {
                if (this.configParameters.Contains(ManagementAgentParametersBase.ExternalIDsFixedTypeFormatParameter))
                {
                    string value = this.configParameters[ManagementAgentParametersBase.ExternalIDsFixedTypeFormatParameter].Value;

                    if (string.IsNullOrWhiteSpace(value))
                    {
                        yield break;
                    }

                    foreach (string name in value.Split('\n'))
                    {
                        yield return name;
                    }
                }
                else
                {
                    yield break;
                }
            }
        }

        public IEnumerable<string> EmailsAttributeFixedTypes
        {
            get
            {
                if (this.configParameters.Contains(ManagementAgentParametersBase.EmailsFixedTypeFormatParameter))
                {
                    string value = this.configParameters[ManagementAgentParametersBase.EmailsFixedTypeFormatParameter].Value;

                    if (string.IsNullOrWhiteSpace(value))
                    {
                        yield break;
                    }

                    foreach (string name in value.Split('\n'))
                    {
                        yield return name;
                    }
                }
                else
                {
                    yield break;
                }
            }
        }


        public IEnumerable<string> RelationsAttributeFixedTypes
        {
            get
            {
                if (this.configParameters.Contains(ManagementAgentParametersBase.RelationsFixedTypeFormatParameter))
                {
                    string value = this.configParameters[ManagementAgentParametersBase.RelationsFixedTypeFormatParameter].Value;

                    if (string.IsNullOrWhiteSpace(value))
                    {
                        yield break;
                    }

                    foreach (string name in value.Split('\n'))
                    {
                        yield return name;
                    }
                }
                else
                {
                    yield break;
                }
            }
        }

        public IEnumerable<string> AddressesAttributeFixedTypes
        {
            get
            {
                if (this.configParameters.Contains(ManagementAgentParametersBase.AddressesFixedTypeFormatParameter))
                {
                    string value = this.configParameters[ManagementAgentParametersBase.AddressesFixedTypeFormatParameter].Value;

                    if (string.IsNullOrWhiteSpace(value))
                    {
                        yield break;
                    }

                    foreach (string name in value.Split('\n'))
                    {
                        yield return name;
                    }
                }
                else
                {
                    yield break;
                }
            }
        }

        public IEnumerable<string> WebsitesAttributeFixedTypes
        {
            get
            {
                if (this.configParameters.Contains(ManagementAgentParametersBase.WebsitesFixedTypeFormatParameter))
                {
                    string value = this.configParameters[ManagementAgentParametersBase.WebsitesFixedTypeFormatParameter].Value;

                    if (string.IsNullOrWhiteSpace(value))
                    {
                        yield break;
                    }

                    foreach (string name in value.Split('\n'))
                    {
                        yield return name;
                    }
                }
                else
                {
                    yield break;
                }
            }
        }

        public bool ExcludeUserCreated
        {
            get
            {
                if (this.configParameters.Contains(ManagementAgentParametersBase.ExcludeUserCreatedGroupsParameter))
                {
                    string value = this.configParameters[ManagementAgentParametersBase.ExcludeUserCreatedGroupsParameter].Value;

                    if (string.IsNullOrWhiteSpace(value))
                    {
                        return false;
                    }
                    else
                    {
                        return Convert.ToBoolean(Convert.ToInt32(value));
                    }
                }
                else
                {
                    return false;
                }
            }
        }

        public string KeyFilePassword
        {
            get
            {
                if (this.configParameters.Contains(ManagementAgentParametersBase.KeyFilePasswordParameter))
                {
                    return this.configParameters[ManagementAgentParametersBase.KeyFilePasswordParameter].SecureValue.ConvertToUnsecureString();
                }
                else
                {
                    return null;
                }
            }
        }

        public static IList<ConfigParameterDefinition> GetParameters(KeyedCollection<string, ConfigParameter> configParameters, ConfigParameterPage page)
        {
            List<ConfigParameterDefinition> parameters = new List<ConfigParameterDefinition>();
            
            switch (page)
            {
                case ConfigParameterPage.Capabilities:
                    break;

                case ConfigParameterPage.Connectivity:
                    parameters.Add(ConfigParameterDefinition.CreateLabelParameter("Credentials"));
                    parameters.Add(ConfigParameterDefinition.CreateStringParameter(ManagementAgentParametersBase.CustomerIDParameter, null, "my_customer"));
                    parameters.Add(ConfigParameterDefinition.CreateStringParameter(ManagementAgentParametersBase.DomainParameter, null, null));
                    parameters.Add(ConfigParameterDefinition.CreateStringParameter(ManagementAgentParametersBase.ServiceAccountEmailAddressParameter, null, null));
                    parameters.Add(ConfigParameterDefinition.CreateStringParameter(ManagementAgentParametersBase.UserEmailAddressParameter, null, null));
                    parameters.Add(ConfigParameterDefinition.CreateStringParameter(ManagementAgentParametersBase.KeyFilePathParameter, null));
                    parameters.Add(ConfigParameterDefinition.CreateEncryptedStringParameter(ManagementAgentParametersBase.KeyFilePasswordParameter, null, null));
                    parameters.Add(ConfigParameterDefinition.CreateDividerParameter());
                    parameters.Add(ConfigParameterDefinition.CreateStringParameter(ManagementAgentParametersBase.LogFilePathParameter, null));

                    break;

                case ConfigParameterPage.Global:
                    parameters.Add(ConfigParameterDefinition.CreateStringParameter(ManagementAgentParametersBase.UserRegexFilterParameter, null, null));
                    parameters.Add(ConfigParameterDefinition.CreateStringParameter(ManagementAgentParametersBase.GroupRegexFilterParameter, null, null));
                    parameters.Add(ConfigParameterDefinition.CreateStringParameter(ManagementAgentParametersBase.ContactRegexFilterParameter, null, null));
                    parameters.Add(ConfigParameterDefinition.CreateCheckBoxParameter(ManagementAgentParametersBase.ExcludeUserCreatedGroupsParameter, false));
                    parameters.Add(ConfigParameterDefinition.CreateDividerParameter());
                    parameters.Add(ConfigParameterDefinition.CreateCheckBoxParameter(ManagementAgentParametersBase.InheritGroupRolesParameter, false));
                    parameters.Add(ConfigParameterDefinition.CreateLabelParameter("Inheriting group roles forces the MA to include owners in the managers list, and managers in the members list"));
                    parameters.Add(ConfigParameterDefinition.CreateDividerParameter());
                    parameters.Add(ConfigParameterDefinition.CreateCheckBoxParameter(ManagementAgentParametersBase.CalendarSendNotificationOnPermissionChangeParameter, false));

                    break;

                case ConfigParameterPage.Partition:
                    break;

                case ConfigParameterPage.RunStep:
                    parameters.Add(ConfigParameterDefinition.CreateCheckBoxParameter(ManagementAgentParametersBase.DoNotGenerateDeltaParameter, false));

                    break;
                case ConfigParameterPage.Schema:
                    parameters.Add(ConfigParameterDefinition.CreateLabelParameter("The values from the following objects are flattened based on the type of object specified. Enter the types you wish to expose, each on a separate line. For example, entering 'work' and 'home' in the phone numbers text box will expose the attributes phones_work and phones_home"));

                    parameters.Add(ConfigParameterDefinition.CreateTextParameter(ManagementAgentParametersBase.PhonesFixedTypeFormatParameter, null));
                    parameters.Add(ConfigParameterDefinition.CreateDividerParameter());

                    parameters.Add(ConfigParameterDefinition.CreateTextParameter(ManagementAgentParametersBase.OrganizationsFixedTypeFormatParameter, null));
                    parameters.Add(ConfigParameterDefinition.CreateDividerParameter());

                    parameters.Add(ConfigParameterDefinition.CreateTextParameter(ManagementAgentParametersBase.IMsFixedTypeFormatParameter, null));
                    parameters.Add(ConfigParameterDefinition.CreateDividerParameter());

                    parameters.Add(ConfigParameterDefinition.CreateTextParameter(ManagementAgentParametersBase.ExternalIDsFixedTypeFormatParameter, null));
                    parameters.Add(ConfigParameterDefinition.CreateDividerParameter());

                    parameters.Add(ConfigParameterDefinition.CreateTextParameter(ManagementAgentParametersBase.RelationsFixedTypeFormatParameter, null));
                    parameters.Add(ConfigParameterDefinition.CreateDividerParameter());

                    parameters.Add(ConfigParameterDefinition.CreateTextParameter(ManagementAgentParametersBase.AddressesFixedTypeFormatParameter, null));
                    parameters.Add(ConfigParameterDefinition.CreateDividerParameter());

                    parameters.Add(ConfigParameterDefinition.CreateTextParameter(ManagementAgentParametersBase.EmailsFixedTypeFormatParameter, null));
                    parameters.Add(ConfigParameterDefinition.CreateDividerParameter());

                    parameters.Add(ConfigParameterDefinition.CreateTextParameter(ManagementAgentParametersBase.WebsitesFixedTypeFormatParameter, null));
                    parameters.Add(ConfigParameterDefinition.CreateDividerParameter());

                    parameters.Add(ConfigParameterDefinition.CreateDividerParameter());
                    parameters.Add(ConfigParameterDefinition.CreateDropDownParameter(ManagementAgentParametersBase.CalendarBuildingAttributeTypeParameter, new string[] {"String", "Reference"}, false, "String"));
                    parameters.Add(ConfigParameterDefinition.CreateDropDownParameter(ManagementAgentParametersBase.CalendarFeatureAttributeTypeParameter, new string[] { "String", "Reference" }, false, "String"));

                    break;
                default:
                    break;
            }

            return parameters;
        }

        public ParameterValidationResult ValidateParameters(ConfigParameterPage page)
        {
            ParameterValidationResult result = new ParameterValidationResult { Code = ParameterValidationResultCode.Success };

            switch (page)
            {
                case ConfigParameterPage.Capabilities:
                    break;

                case ConfigParameterPage.Connectivity:
                    if (string.IsNullOrWhiteSpace(this.ServiceAccountEmailAddress))
                    {
                        result.Code = ParameterValidationResultCode.Failure;
                        result.ErrorMessage = "A service account email address is required";
                        result.ErrorParameter = ManagementAgentParametersBase.ServiceAccountEmailAddressParameter;
                        return result;
                    }

                    if (string.IsNullOrWhiteSpace(this.UserEmailAddress))
                    {
                        result.Code = ParameterValidationResultCode.Failure;
                        result.ErrorMessage = "A user email address is required";
                        result.ErrorParameter = ManagementAgentParametersBase.UserEmailAddressParameter;
                        return result;
                    }

                    if (string.IsNullOrWhiteSpace(this.Domain))
                    {
                        result.Code = ParameterValidationResultCode.Failure;
                        result.ErrorMessage = "The primary domain is required";
                        result.ErrorParameter = ManagementAgentParametersBase.DomainParameter;
                        return result;
                    }

                    if (string.IsNullOrWhiteSpace(this.KeyFilePath))
                    {
                        result.Code = ParameterValidationResultCode.Failure;
                        result.ErrorMessage = "A key file is required";
                        result.ErrorParameter = ManagementAgentParametersBase.KeyFilePathParameter;
                        return result;
                    }
                    else
                    {
                        if (!File.Exists(this.KeyFilePath))
                        {
                            result.Code = ParameterValidationResultCode.Failure;
                            result.ErrorMessage = "The specified key file could not be found";
                            result.ErrorParameter = ManagementAgentParametersBase.KeyFilePathParameter;
                            return result;
                        }
                        else
                        {
                            try
                            {
                                X509Certificate2 cert = this.GetCertificate(this.KeyFilePath, this.KeyFilePassword);
                            }
                            catch (Exception ex)
                            {
                                result.Code = ParameterValidationResultCode.Failure;
                                result.ErrorMessage = "The specified key file could not be opened. " + ex.Message;
                                result.ErrorParameter = ManagementAgentParametersBase.KeyFilePathParameter;
                                return result;
                            }
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(this.GroupRegexFilter))
                    {
                        try
                        {
                            Regex r = new Regex(this.GroupRegexFilter);
                        }
                        catch (Exception ex)
                        {
                            result.Code = ParameterValidationResultCode.Failure;
                            result.ErrorMessage = "The specified group regular expression was not valid. " + ex.Message;
                            result.ErrorParameter = ManagementAgentParametersBase.GroupRegexFilterParameter;
                            return result;
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(this.ContactRegexFilter))
                    {
                        try
                        {
                            Regex r = new Regex(this.ContactRegexFilter);
                        }
                        catch (Exception ex)
                        {
                            result.Code = ParameterValidationResultCode.Failure;
                            result.ErrorMessage = "The specified contact regular expression was not valid. " + ex.Message;
                            result.ErrorParameter = ManagementAgentParametersBase.ContactRegexFilterParameter;
                            return result;
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(this.UserRegexFilter))
                    {
                        try
                        {
                            Regex r = new Regex(this.UserRegexFilter);
                        }
                        catch (Exception ex)
                        {
                            result.Code = ParameterValidationResultCode.Failure;
                            result.ErrorMessage = "The specified user regular expression was not valid. " + ex.Message;
                            result.ErrorParameter = ManagementAgentParametersBase.UserRegexFilterParameter;
                            return result;
                        }
                    }

                    break;

                case ConfigParameterPage.Global:
                    break;
                case ConfigParameterPage.Partition:
                    break;
                case ConfigParameterPage.RunStep:
                    break;
                case ConfigParameterPage.Schema:


                    if (this.OrganizationsAttributeFixedTypes.Any())
                    {
                        if (!this.OrganizationsAttributeFixedTypes.All(new HashSet<string>().Add))
                        {
                            result.Code = ParameterValidationResultCode.Failure;
                            result.ErrorMessage = "The organization types cannot contain duplicates";
                            result.ErrorParameter = ManagementAgentParametersBase.OrganizationsFixedTypeFormatParameter;
                            return result;
                        }
                    }

                    if (this.IMsAttributeFixedTypes.Any())
                    {
                        if (!this.IMsAttributeFixedTypes.All(new HashSet<string>().Add))
                        {
                            result.Code = ParameterValidationResultCode.Failure;
                            result.ErrorMessage = "The IM types cannot contain duplicates";
                            result.ErrorParameter = ManagementAgentParametersBase.IMsFixedTypeFormatParameter;
                            return result;
                        }
                    }

                    if (this.AddressesAttributeFixedTypes.Any())
                    {
                        if (!this.AddressesAttributeFixedTypes.All(new HashSet<string>().Add))
                        {
                            result.Code = ParameterValidationResultCode.Failure;
                            result.ErrorMessage = "The address types cannot contain duplicates";
                            result.ErrorParameter = ManagementAgentParametersBase.AddressesFixedTypeFormatParameter;
                            return result;
                        }
                    }

                    if (this.WebsitesAttributeFixedTypes.Any())
                    {
                        if (!this.WebsitesAttributeFixedTypes.All(new HashSet<string>().Add))
                        {
                            result.Code = ParameterValidationResultCode.Failure;
                            result.ErrorMessage = "The website types cannot contain duplicates";
                            result.ErrorParameter = ManagementAgentParametersBase.WebsitesFixedTypeFormatParameter;
                            return result;
                        }
                    }

                    if (this.ExternalIDsAttributeFixedTypes.Any())
                    {
                        if (!this.ExternalIDsAttributeFixedTypes.All(new HashSet<string>().Add))
                        {
                            result.Code = ParameterValidationResultCode.Failure;
                            result.ErrorMessage = "The external ID types cannot contain duplicates";
                            result.ErrorParameter = ManagementAgentParametersBase.ExternalIDsFixedTypeFormatParameter;
                            return result;
                        }
                    }

                    if (this.RelationsAttributeFixedTypes.Any())
                    {
                        if (!this.RelationsAttributeFixedTypes.All(new HashSet<string>().Add))
                        {
                            result.Code = ParameterValidationResultCode.Failure;
                            result.ErrorMessage = "The relations types field cannot contain duplicates";
                            result.ErrorParameter = ManagementAgentParametersBase.RelationsFixedTypeFormatParameter;
                            return result;
                        }
                    }

                    if (this.PhonesAttributeFixedTypes.Any())
                    {
                        if (!this.PhonesAttributeFixedTypes.All(new HashSet<string>().Add))
                        {
                            result.Code = ParameterValidationResultCode.Failure;
                            result.ErrorMessage = "The phone types cannot contain duplicates";
                            result.ErrorParameter = ManagementAgentParametersBase.PhonesFixedTypeFormatParameter;
                            return result;
                        }
                    }
                    break;
                default:
                    break;
            }

            return result;
        }

        public ServiceAccountCredential Credentials
        {
            get
            {
                return this.GetCredentials(
                    this.ServiceAccountEmailAddress,
                    this.UserEmailAddress,
                    this.GetCertificate(this.KeyFilePath, this.KeyFilePassword));
            }
        }
    }
}
