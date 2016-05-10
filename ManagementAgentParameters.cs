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

        public GoogleArrayMode PhonesAttributeFormat
        {
            get
            {
                if (this.configParameters.Contains(ManagementAgentParametersBase.PhonesFormatParameter))
                {
                    string value = this.configParameters[ManagementAgentParametersBase.PhonesFormatParameter].Value;

                    if (string.IsNullOrWhiteSpace(value))
                    {
                        return GoogleArrayMode.PrimaryValueOnly;
                    }
                    else
                    {
                        return GoogleArrayModeConverters.FromDescription(value);
                    }
                }
                else
                {
                    return GoogleArrayMode.PrimaryValueOnly;
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

        public GoogleArrayMode OrganizationsAttributeFormat
        {
            get
            {
                if (this.configParameters.Contains(ManagementAgentParametersBase.OrganizationsFormatParameter))
                {
                    string value = this.configParameters[ManagementAgentParametersBase.OrganizationsFormatParameter].Value;

                    if (string.IsNullOrWhiteSpace(value))
                    {
                        return GoogleArrayMode.PrimaryValueOnly;
                    }
                    else
                    {
                        return GoogleArrayModeConverters.FromDescription(value);
                    }
                }
                else
                {
                    return GoogleArrayMode.PrimaryValueOnly;
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


        public GoogleArrayMode IMsAttributeFormat
        {
            get
            {
                if (this.configParameters.Contains(ManagementAgentParametersBase.IMsFormatParameter))
                {
                    string value = this.configParameters[ManagementAgentParametersBase.IMsFormatParameter].Value;

                    if (string.IsNullOrWhiteSpace(value))
                    {
                        return GoogleArrayMode.PrimaryValueOnly;
                    }
                    else
                    {
                        return GoogleArrayModeConverters.FromDescription(value);
                    }
                }
                else
                {
                    return GoogleArrayMode.PrimaryValueOnly;
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

        public GoogleArrayMode ExternalIDsAttributeFormat
        {
            get
            {
                if (this.configParameters.Contains(ManagementAgentParametersBase.ExternalIDsFormatParameter))
                {
                    string value = this.configParameters[ManagementAgentParametersBase.ExternalIDsFormatParameter].Value;

                    if (string.IsNullOrWhiteSpace(value))
                    {
                        return GoogleArrayMode.Json;
                    }
                    else
                    {
                        return GoogleArrayModeConverters.FromDescription(value);
                    }
                }
                else
                {
                    return GoogleArrayMode.Json;
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


        public GoogleArrayMode RelationsAttributeFormat
        {
            get
            {
                if (this.configParameters.Contains(ManagementAgentParametersBase.RelationsFormatParameter))
                {
                    string value = this.configParameters[ManagementAgentParametersBase.RelationsFormatParameter].Value;

                    if (string.IsNullOrWhiteSpace(value))
                    {
                        return GoogleArrayMode.Json;
                    }
                    else
                    {
                        return GoogleArrayModeConverters.FromDescription(value);
                    }
                }
                else
                {
                    return GoogleArrayMode.Json;
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


        public GoogleArrayMode AddressesAttributeFormat
        {
            get
            {
                if (this.configParameters.Contains(ManagementAgentParametersBase.AddressesFormatParameter))
                {
                    string value = this.configParameters[ManagementAgentParametersBase.AddressesFormatParameter].Value;

                    if (string.IsNullOrWhiteSpace(value))
                    {
                        return GoogleArrayMode.PrimaryValueOnly;
                    }
                    else
                    {
                        return GoogleArrayModeConverters.FromDescription(value);
                    }
                }
                else
                {
                    return GoogleArrayMode.PrimaryValueOnly;
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


        public GoogleArrayMode WebsitesAttributeFormat
        {
            get
            {
                if (this.configParameters.Contains(ManagementAgentParametersBase.WebsitesFormatParameter))
                {
                    string value = this.configParameters[ManagementAgentParametersBase.WebsitesFormatParameter].Value;

                    if (string.IsNullOrWhiteSpace(value))
                    {
                        return GoogleArrayMode.PrimaryValueOnly;
                    }
                    else
                    {
                        return GoogleArrayModeConverters.FromDescription(value);
                    }
                }
                else
                {
                    return GoogleArrayMode.PrimaryValueOnly;
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
                    parameters.Add(ConfigParameterDefinition.CreateStringParameter(ManagementAgentParametersBase.ServiceAccountEmailAddressParameter, null, null));
                    parameters.Add(ConfigParameterDefinition.CreateStringParameter(ManagementAgentParametersBase.UserEmailAddressParameter, null, null));
                    parameters.Add(ConfigParameterDefinition.CreateStringParameter(ManagementAgentParametersBase.KeyFilePathParameter, null));
                    parameters.Add(ConfigParameterDefinition.CreateEncryptedStringParameter(ManagementAgentParametersBase.KeyFilePasswordParameter, null, null));

                    parameters.Add(ConfigParameterDefinition.CreateDividerParameter());
                    parameters.Add(ConfigParameterDefinition.CreateLabelParameter("The following attributes are represented by arrays in the Google API. You can choose to present array values as a raw JSON string, or the MA can flatten the attributes based on the object types you specify"));

                    parameters.Add(ConfigParameterDefinition.CreateDropDownParameter(ManagementAgentParametersBase.PhonesFormatParameter, GoogleArrayModeConverters.ParameterNamesPrimarySupported, false, GoogleArrayModeConverters.ParameterNamesPrimarySupported[0]));
                    parameters.Add(ConfigParameterDefinition.CreateTextParameter(ManagementAgentParametersBase.PhonesFixedTypeFormatParameter, null));
                    parameters.Add(ConfigParameterDefinition.CreateDividerParameter());

                    parameters.Add(ConfigParameterDefinition.CreateDropDownParameter(ManagementAgentParametersBase.OrganizationsFormatParameter, GoogleArrayModeConverters.ParameterNamesPrimarySupported, false, GoogleArrayModeConverters.ParameterNamesPrimarySupported[0]));
                    parameters.Add(ConfigParameterDefinition.CreateTextParameter(ManagementAgentParametersBase.OrganizationsFixedTypeFormatParameter, null));
                    parameters.Add(ConfigParameterDefinition.CreateDividerParameter());

                    parameters.Add(ConfigParameterDefinition.CreateDropDownParameter(ManagementAgentParametersBase.IMsFormatParameter, GoogleArrayModeConverters.ParameterNamesPrimarySupported, false, GoogleArrayModeConverters.ParameterNamesPrimarySupported[0]));
                    parameters.Add(ConfigParameterDefinition.CreateTextParameter(ManagementAgentParametersBase.IMsFixedTypeFormatParameter, null));
                    parameters.Add(ConfigParameterDefinition.CreateDividerParameter());

                    parameters.Add(ConfigParameterDefinition.CreateDropDownParameter(ManagementAgentParametersBase.ExternalIDsFormatParameter, GoogleArrayModeConverters.ParameterNamesPrimaryNotSupported, false, GoogleArrayModeConverters.ParameterNamesPrimaryNotSupported[0]));
                    parameters.Add(ConfigParameterDefinition.CreateTextParameter(ManagementAgentParametersBase.ExternalIDsFixedTypeFormatParameter, null));
                    parameters.Add(ConfigParameterDefinition.CreateDividerParameter());

                    parameters.Add(ConfigParameterDefinition.CreateDropDownParameter(ManagementAgentParametersBase.RelationsFormatParameter, GoogleArrayModeConverters.ParameterNamesPrimaryNotSupported, false, GoogleArrayModeConverters.ParameterNamesPrimaryNotSupported[0]));
                    parameters.Add(ConfigParameterDefinition.CreateTextParameter(ManagementAgentParametersBase.RelationsFixedTypeFormatParameter, null));
                    parameters.Add(ConfigParameterDefinition.CreateDividerParameter());

                    parameters.Add(ConfigParameterDefinition.CreateDropDownParameter(ManagementAgentParametersBase.AddressesFormatParameter, GoogleArrayModeConverters.ParameterNamesPrimarySupported, false, GoogleArrayModeConverters.ParameterNamesPrimarySupported[0]));
                    parameters.Add(ConfigParameterDefinition.CreateTextParameter(ManagementAgentParametersBase.AddressesFixedTypeFormatParameter, null));
                    parameters.Add(ConfigParameterDefinition.CreateDividerParameter());

                    parameters.Add(ConfigParameterDefinition.CreateDropDownParameter(ManagementAgentParametersBase.WebsitesFormatParameter, GoogleArrayModeConverters.ParameterNamesPrimarySupported, false, GoogleArrayModeConverters.ParameterNamesPrimarySupported[0]));
                    parameters.Add(ConfigParameterDefinition.CreateTextParameter(ManagementAgentParametersBase.WebsitesFixedTypeFormatParameter, null));
                    parameters.Add(ConfigParameterDefinition.CreateDividerParameter());

                    parameters.Add(ConfigParameterDefinition.CreateCheckBoxParameter(ManagementAgentParametersBase.ExcludeUserCreatedGroupsParameter, false));
                    parameters.Add(ConfigParameterDefinition.CreateStringParameter(ManagementAgentParametersBase.UserRegexFilterParameter, null, null));
                    parameters.Add(ConfigParameterDefinition.CreateStringParameter(ManagementAgentParametersBase.GroupRegexFilterParameter, null, null));
                    break;

                case ConfigParameterPage.Global:

                    break;
                case ConfigParameterPage.Partition:
                    break;
                case ConfigParameterPage.RunStep:
                    parameters.Add(ConfigParameterDefinition.CreateCheckBoxParameter(ManagementAgentParametersBase.DoNotGenerateDeltaParameter, false));

                    break;
                case ConfigParameterPage.Schema:
                    break;
                default:
                    break;
            }

            return parameters;
        }

        public ParameterValidationResult ValidateParameters(ConfigParameterPage page)
        {
            ParameterValidationResult result = new ParameterValidationResult();
            result.Code = ParameterValidationResultCode.Success;

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

                    if (this.OrganizationsAttributeFormat != GoogleArrayMode.Json)
                    {
                        if (!this.OrganizationsAttributeFixedTypes.All(new HashSet<string>().Add))
                        {
                            result.Code = ParameterValidationResultCode.Failure;
                            result.ErrorMessage = "The organization types cannot contain duplicates";
                            result.ErrorParameter = ManagementAgentParametersBase.OrganizationsFixedTypeFormatParameter;
                            return result;
                        }
                    }

                    if (this.IMsAttributeFormat != GoogleArrayMode.Json)
                    {
                        if (!this.IMsAttributeFixedTypes.All(new HashSet<string>().Add))
                        {
                            result.Code = ParameterValidationResultCode.Failure;
                            result.ErrorMessage = "The IM types cannot contain duplicates";
                            result.ErrorParameter = ManagementAgentParametersBase.IMsFixedTypeFormatParameter;
                            return result;
                        }
                    }

                    if (this.AddressesAttributeFormat != GoogleArrayMode.Json)
                    {
                        if (!this.AddressesAttributeFixedTypes.All(new HashSet<string>().Add))
                        {
                            result.Code = ParameterValidationResultCode.Failure;
                            result.ErrorMessage = "The address types cannot contain duplicates";
                            result.ErrorParameter = ManagementAgentParametersBase.AddressesFixedTypeFormatParameter;
                            return result;
                        }
                    }

                    if (this.WebsitesAttributeFormat != GoogleArrayMode.Json)
                    {
                        if (!this.WebsitesAttributeFixedTypes.All(new HashSet<string>().Add))
                        {
                            result.Code = ParameterValidationResultCode.Failure;
                            result.ErrorMessage = "The website types cannot contain duplicates";
                            result.ErrorParameter = ManagementAgentParametersBase.WebsitesFixedTypeFormatParameter;
                            return result;
                        }
                    }

                    if (this.ExternalIDsAttributeFormat != GoogleArrayMode.Json)
                    {
                        if (!this.ExternalIDsAttributeFixedTypes.Any())
                        {
                            result.Code = ParameterValidationResultCode.Failure;
                            result.ErrorMessage = "The external IDs types field must not be empty";
                            result.ErrorParameter = ManagementAgentParametersBase.ExternalIDsFixedTypeFormatParameter;
                            return result;
                        }

                        if (!this.ExternalIDsAttributeFixedTypes.All(new HashSet<string>().Add))
                        {
                            result.Code = ParameterValidationResultCode.Failure;
                            result.ErrorMessage = "The external ID types cannot contain duplicates";
                            result.ErrorParameter = ManagementAgentParametersBase.ExternalIDsFixedTypeFormatParameter;
                            return result;
                        }
                    }

                    if (this.RelationsAttributeFormat != GoogleArrayMode.Json)
                    {
                        if (!this.RelationsAttributeFixedTypes.Any())
                        {
                            result.Code = ParameterValidationResultCode.Failure;
                            result.ErrorMessage = "The relations types field must not be empty";
                            result.ErrorParameter = ManagementAgentParametersBase.RelationsFixedTypeFormatParameter;
                            return result;
                        }

                        if (!this.RelationsAttributeFixedTypes.All(new HashSet<string>().Add))
                        {
                            result.Code = ParameterValidationResultCode.Failure;
                            result.ErrorMessage = "The relations types field cannot contain duplicates";
                            result.ErrorParameter = ManagementAgentParametersBase.RelationsFixedTypeFormatParameter;
                            return result;
                        }
                    }

                    if (this.PhonesAttributeFormat != GoogleArrayMode.Json)
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

                case ConfigParameterPage.Global:
                    break;
                case ConfigParameterPage.Partition:
                    break;
                case ConfigParameterPage.RunStep:
                    break;
                case ConfigParameterPage.Schema:
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
