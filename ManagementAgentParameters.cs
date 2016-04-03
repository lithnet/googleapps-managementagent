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
    public class ManagementAgentParameters : ManagementAgentParametersBase, IManagementAgentParameters
    {
        private KeyedCollection<string, ConfigParameter> configParameters;

        private X509Certificate2 certificate;

        private ServiceAccountCredential credentials;

        public ManagementAgentParameters(KeyedCollection<string, ConfigParameter> configParameters)
        {
            this.configParameters = configParameters;
        }

        public string CustomerID
        {
            get
            {
                if (this.configParameters.Contains(ManagementAgentParameters.CustomerIDParameter))
                {
                    return this.configParameters[ManagementAgentParameters.CustomerIDParameter].Value;
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
                if (this.configParameters.Contains(ManagementAgentParameters.ServiceAccountEmailAddressParameter))
                {
                    return this.configParameters[ManagementAgentParameters.ServiceAccountEmailAddressParameter].Value;
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
                if (this.configParameters.Contains(ManagementAgentParameters.GroupRegexFilterParameter))
                {
                    return this.configParameters[ManagementAgentParameters.GroupRegexFilterParameter].Value;
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
                if (this.configParameters.Contains(ManagementAgentParameters.UserRegexFilterParameter))
                {
                    return this.configParameters[ManagementAgentParameters.UserRegexFilterParameter].Value;
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
                if (this.configParameters.Contains(ManagementAgentParameters.UserEmailAddressParameter))
                {
                    return this.configParameters[ManagementAgentParameters.UserEmailAddressParameter].Value;
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
                if (this.configParameters.Contains(ManagementAgentParameters.KeyFilePathParameter))
                {
                    return this.configParameters[ManagementAgentParameters.KeyFilePathParameter].Value;
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
                if (this.configParameters.Contains(ManagementAgentParameters.DoNotGenerateDeltaParameter))
                {
                    string value = this.configParameters[ManagementAgentParameters.DoNotGenerateDeltaParameter].Value;

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
                if (this.configParameters.Contains(ManagementAgentParameters.PhonesFormatParameter))
                {
                    string value = this.configParameters[ManagementAgentParameters.PhonesFormatParameter].Value;

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
                if (this.configParameters.Contains(ManagementAgentParameters.PhonesFixedTypeFormatParameter))
                {
                    string value = this.configParameters[ManagementAgentParameters.PhonesFixedTypeFormatParameter].Value;

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
                if (this.configParameters.Contains(ManagementAgentParameters.OrganizationsFormatParameter))
                {
                    string value = this.configParameters[ManagementAgentParameters.OrganizationsFormatParameter].Value;

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
                if (this.configParameters.Contains(ManagementAgentParameters.OrganizationsFixedTypeFormatParameter))
                {
                    string value = this.configParameters[ManagementAgentParameters.OrganizationsFixedTypeFormatParameter].Value;

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
                if (this.configParameters.Contains(ManagementAgentParameters.IMsFormatParameter))
                {
                    string value = this.configParameters[ManagementAgentParameters.IMsFormatParameter].Value;

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
                if (this.configParameters.Contains(ManagementAgentParameters.IMsFixedTypeFormatParameter))
                {
                    string value = this.configParameters[ManagementAgentParameters.IMsFixedTypeFormatParameter].Value;

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
                if (this.configParameters.Contains(ManagementAgentParameters.ExternalIDsFormatParameter))
                {
                    string value = this.configParameters[ManagementAgentParameters.ExternalIDsFormatParameter].Value;

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
                if (this.configParameters.Contains(ManagementAgentParameters.ExternalIDsFixedTypeFormatParameter))
                {
                    string value = this.configParameters[ManagementAgentParameters.ExternalIDsFixedTypeFormatParameter].Value;

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
                if (this.configParameters.Contains(ManagementAgentParameters.RelationsFormatParameter))
                {
                    string value = this.configParameters[ManagementAgentParameters.RelationsFormatParameter].Value;

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
                if (this.configParameters.Contains(ManagementAgentParameters.RelationsFixedTypeFormatParameter))
                {
                    string value = this.configParameters[ManagementAgentParameters.RelationsFixedTypeFormatParameter].Value;

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
                if (this.configParameters.Contains(ManagementAgentParameters.AddressesFormatParameter))
                {
                    string value = this.configParameters[ManagementAgentParameters.AddressesFormatParameter].Value;

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
                if (this.configParameters.Contains(ManagementAgentParameters.AddressesFixedTypeFormatParameter))
                {
                    string value = this.configParameters[ManagementAgentParameters.AddressesFixedTypeFormatParameter].Value;

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
                if (this.configParameters.Contains(ManagementAgentParameters.WebsitesFormatParameter))
                {
                    string value = this.configParameters[ManagementAgentParameters.WebsitesFormatParameter].Value;

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
                if (this.configParameters.Contains(ManagementAgentParameters.WebsitesFixedTypeFormatParameter))
                {
                    string value = this.configParameters[ManagementAgentParameters.WebsitesFixedTypeFormatParameter].Value;

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
                if (this.configParameters.Contains(ManagementAgentParameters.ExcludeUserCreatedGroupsParameter))
                {
                    string value = this.configParameters[ManagementAgentParameters.ExcludeUserCreatedGroupsParameter].Value;

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
                if (this.configParameters.Contains(ManagementAgentParameters.KeyFilePasswordParameter))
                {
                    return this.configParameters[ManagementAgentParameters.KeyFilePasswordParameter].SecureValue.ConvertToUnsecureString();
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
                    parameters.Add(ConfigParameterDefinition.CreateStringParameter(ManagementAgentParameters.CustomerIDParameter, null, "my_customer"));
                    parameters.Add(ConfigParameterDefinition.CreateStringParameter(ManagementAgentParameters.ServiceAccountEmailAddressParameter, null, null));
                    parameters.Add(ConfigParameterDefinition.CreateStringParameter(ManagementAgentParameters.UserEmailAddressParameter, null, null));
                    parameters.Add(ConfigParameterDefinition.CreateStringParameter(ManagementAgentParameters.KeyFilePathParameter, null));
                    parameters.Add(ConfigParameterDefinition.CreateEncryptedStringParameter(ManagementAgentParameters.KeyFilePasswordParameter, null, null));

                    parameters.Add(ConfigParameterDefinition.CreateDividerParameter());
                    parameters.Add(ConfigParameterDefinition.CreateLabelParameter("The following attributes are represented by arrays in the Google API. You can choose to present array values as a raw JSON string, or the MA can flatten the attributes based on the object types you specify"));

                    parameters.Add(ConfigParameterDefinition.CreateDropDownParameter(ManagementAgentParameters.PhonesFormatParameter, GoogleArrayModeConverters.ParameterNamesPrimarySupported, false, GoogleArrayModeConverters.ParameterNamesPrimarySupported[0]));
                    parameters.Add(ConfigParameterDefinition.CreateTextParameter(ManagementAgentParameters.PhonesFixedTypeFormatParameter, null));
                    parameters.Add(ConfigParameterDefinition.CreateDividerParameter());

                    parameters.Add(ConfigParameterDefinition.CreateDropDownParameter(ManagementAgentParameters.OrganizationsFormatParameter, GoogleArrayModeConverters.ParameterNamesPrimarySupported, false, GoogleArrayModeConverters.ParameterNamesPrimarySupported[0]));
                    parameters.Add(ConfigParameterDefinition.CreateTextParameter(ManagementAgentParameters.OrganizationsFixedTypeFormatParameter, null));
                    parameters.Add(ConfigParameterDefinition.CreateDividerParameter());

                    parameters.Add(ConfigParameterDefinition.CreateDropDownParameter(ManagementAgentParameters.IMsFormatParameter, GoogleArrayModeConverters.ParameterNamesPrimarySupported, false, GoogleArrayModeConverters.ParameterNamesPrimarySupported[0]));
                    parameters.Add(ConfigParameterDefinition.CreateTextParameter(ManagementAgentParameters.IMsFixedTypeFormatParameter, null));
                    parameters.Add(ConfigParameterDefinition.CreateDividerParameter());

                    parameters.Add(ConfigParameterDefinition.CreateDropDownParameter(ManagementAgentParameters.ExternalIDsFormatParameter, GoogleArrayModeConverters.ParameterNamesPrimaryNotSupported, false, GoogleArrayModeConverters.ParameterNamesPrimaryNotSupported[0]));
                    parameters.Add(ConfigParameterDefinition.CreateTextParameter(ManagementAgentParameters.ExternalIDsFixedTypeFormatParameter, null));
                    parameters.Add(ConfigParameterDefinition.CreateDividerParameter());

                    parameters.Add(ConfigParameterDefinition.CreateDropDownParameter(ManagementAgentParameters.RelationsFormatParameter, GoogleArrayModeConverters.ParameterNamesPrimaryNotSupported, false, GoogleArrayModeConverters.ParameterNamesPrimaryNotSupported[0]));
                    parameters.Add(ConfigParameterDefinition.CreateTextParameter(ManagementAgentParameters.RelationsFixedTypeFormatParameter, null));
                    parameters.Add(ConfigParameterDefinition.CreateDividerParameter());

                    parameters.Add(ConfigParameterDefinition.CreateDropDownParameter(ManagementAgentParameters.AddressesFormatParameter, GoogleArrayModeConverters.ParameterNamesPrimarySupported, false, GoogleArrayModeConverters.ParameterNamesPrimarySupported[0]));
                    parameters.Add(ConfigParameterDefinition.CreateTextParameter(ManagementAgentParameters.AddressesFixedTypeFormatParameter, null));
                    parameters.Add(ConfigParameterDefinition.CreateDividerParameter());

                    parameters.Add(ConfigParameterDefinition.CreateDropDownParameter(ManagementAgentParameters.WebsitesFormatParameter, GoogleArrayModeConverters.ParameterNamesPrimarySupported, false, GoogleArrayModeConverters.ParameterNamesPrimarySupported[0]));
                    parameters.Add(ConfigParameterDefinition.CreateTextParameter(ManagementAgentParameters.WebsitesFixedTypeFormatParameter, null));
                    parameters.Add(ConfigParameterDefinition.CreateDividerParameter());

                    parameters.Add(ConfigParameterDefinition.CreateCheckBoxParameter(ManagementAgentParameters.ExcludeUserCreatedGroupsParameter, false));
                    parameters.Add(ConfigParameterDefinition.CreateStringParameter(ManagementAgentParameters.UserRegexFilterParameter, null, null));
                    parameters.Add(ConfigParameterDefinition.CreateStringParameter(ManagementAgentParameters.GroupRegexFilterParameter, null, null));
                    break;

                case ConfigParameterPage.Global:

                    break;
                case ConfigParameterPage.Partition:
                    break;
                case ConfigParameterPage.RunStep:
                    parameters.Add(ConfigParameterDefinition.CreateCheckBoxParameter(ManagementAgentParameters.DoNotGenerateDeltaParameter, false));

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
                        result.ErrorParameter = ManagementAgentParameters.ServiceAccountEmailAddressParameter;
                        return result;
                    }

                    if (string.IsNullOrWhiteSpace(this.UserEmailAddress))
                    {
                        result.Code = ParameterValidationResultCode.Failure;
                        result.ErrorMessage = "A user email address is required";
                        result.ErrorParameter = ManagementAgentParameters.UserEmailAddressParameter;
                        return result;
                    }

                    if (string.IsNullOrWhiteSpace(this.KeyFilePath))
                    {
                        result.Code = ParameterValidationResultCode.Failure;
                        result.ErrorMessage = "A key file is required";
                        result.ErrorParameter = ManagementAgentParameters.KeyFilePathParameter;
                        return result;
                    }
                    else
                    {
                        if (!File.Exists(this.KeyFilePath))
                        {
                            result.Code = ParameterValidationResultCode.Failure;
                            result.ErrorMessage = "The specified key file could not be found";
                            result.ErrorParameter = ManagementAgentParameters.KeyFilePathParameter;
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
                                result.ErrorParameter = ManagementAgentParameters.KeyFilePathParameter;
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
                            result.ErrorParameter = ManagementAgentParameters.GroupRegexFilterParameter;
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
                            result.ErrorParameter = ManagementAgentParameters.UserRegexFilterParameter;
                            return result;
                        }
                    }

                    if (this.OrganizationsAttributeFormat != GoogleArrayMode.Json)
                    {
                        if (!this.OrganizationsAttributeFixedTypes.All(new HashSet<string>().Add))
                        {
                            result.Code = ParameterValidationResultCode.Failure;
                            result.ErrorMessage = "The organization types cannot contain duplicates";
                            result.ErrorParameter = ManagementAgentParameters.OrganizationsFixedTypeFormatParameter;
                            return result;
                        }
                    }

                    if (this.IMsAttributeFormat != GoogleArrayMode.Json)
                    {
                        if (!this.IMsAttributeFixedTypes.All(new HashSet<string>().Add))
                        {
                            result.Code = ParameterValidationResultCode.Failure;
                            result.ErrorMessage = "The IM types cannot contain duplicates";
                            result.ErrorParameter = ManagementAgentParameters.IMsFixedTypeFormatParameter;
                            return result;
                        }
                    }

                    if (this.AddressesAttributeFormat != GoogleArrayMode.Json)
                    {
                        if (!this.AddressesAttributeFixedTypes.All(new HashSet<string>().Add))
                        {
                            result.Code = ParameterValidationResultCode.Failure;
                            result.ErrorMessage = "The address types cannot contain duplicates";
                            result.ErrorParameter = ManagementAgentParameters.AddressesFixedTypeFormatParameter;
                            return result;
                        }
                    }

                    if (this.WebsitesAttributeFormat != GoogleArrayMode.Json)
                    {
                        if (!this.WebsitesAttributeFixedTypes.All(new HashSet<string>().Add))
                        {
                            result.Code = ParameterValidationResultCode.Failure;
                            result.ErrorMessage = "The website types cannot contain duplicates";
                            result.ErrorParameter = ManagementAgentParameters.WebsitesFixedTypeFormatParameter;
                            return result;
                        }
                    }

                    if (this.ExternalIDsAttributeFormat != GoogleArrayMode.Json)
                    {
                        if (!this.ExternalIDsAttributeFixedTypes.Any())
                        {
                            result.Code = ParameterValidationResultCode.Failure;
                            result.ErrorMessage = "The external IDs types field must not be empty";
                            result.ErrorParameter = ManagementAgentParameters.ExternalIDsFixedTypeFormatParameter;
                            return result;
                        }

                        if (!this.ExternalIDsAttributeFixedTypes.All(new HashSet<string>().Add))
                        {
                            result.Code = ParameterValidationResultCode.Failure;
                            result.ErrorMessage = "The external ID types cannot contain duplicates";
                            result.ErrorParameter = ManagementAgentParameters.ExternalIDsFixedTypeFormatParameter;
                            return result;
                        }
                    }

                    if (this.RelationsAttributeFormat != GoogleArrayMode.Json)
                    {
                        if (!this.RelationsAttributeFixedTypes.Any())
                        {
                            result.Code = ParameterValidationResultCode.Failure;
                            result.ErrorMessage = "The relations types field must not be empty";
                            result.ErrorParameter = ManagementAgentParameters.RelationsFixedTypeFormatParameter;
                            return result;
                        }

                        if (!this.RelationsAttributeFixedTypes.All(new HashSet<string>().Add))
                        {
                            result.Code = ParameterValidationResultCode.Failure;
                            result.ErrorMessage = "The relations types field cannot contain duplicates";
                            result.ErrorParameter = ManagementAgentParameters.RelationsFixedTypeFormatParameter;
                            return result;
                        }
                    }

                    if (this.PhonesAttributeFormat != GoogleArrayMode.Json)
                    {
                        if (!this.PhonesAttributeFixedTypes.All(new HashSet<string>().Add))
                        {
                            result.Code = ParameterValidationResultCode.Failure;
                            result.ErrorMessage = "The phone types cannot contain duplicates";
                            result.ErrorParameter = ManagementAgentParameters.PhonesFixedTypeFormatParameter;
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
