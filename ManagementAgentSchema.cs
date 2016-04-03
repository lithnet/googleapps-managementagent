using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.MetadirectoryServices;
using System.Collections.ObjectModel;

namespace Lithnet.GoogleApps.MA
{
    public static class ManagementAgentSchema
    {
        public const string CustomGoogleAppsSchemaName = "LithnetGoogleAppsMA";

        public static Schema GetSchema(IManagementAgentParameters config)
        {
            Schema schema = Schema.Create();

            SchemaType type = GetUserType(config, false);
            schema.Types.Add(type);

            if (SchemaRequestFactory.HasSchema(config.CustomerID, ManagementAgentSchema.CustomGoogleAppsSchemaName))
            {
                type = GetUserType(config, true);
                schema.Types.Add(type);
            }

            type = GetGroupType(config);
            schema.Types.Add(type);

            return schema;
        }

        private static SchemaType GetGroupType(IManagementAgentParameters config)
        {
            SchemaType groupType = SchemaType.Create("group", true);
            // Group API
            groupType.Attributes.Add(SchemaAttribute.CreateAnchorAttribute("id", AttributeType.String, AttributeOperation.ImportOnly));
            groupType.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute("name", AttributeType.String, AttributeOperation.ImportExport));
            groupType.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute("description", AttributeType.String, AttributeOperation.ImportExport));
            groupType.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute("adminCreated", AttributeType.Boolean, AttributeOperation.ImportExport));
            groupType.Attributes.Add(SchemaAttribute.CreateMultiValuedAttribute("aliases", AttributeType.String, AttributeOperation.ImportExport));
            groupType.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute("primaryEmail", AttributeType.String, AttributeOperation.ImportExport));

            // Group settings API
            groupType.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute("maxMessageBytes", AttributeType.Integer, AttributeOperation.ImportExport));
            groupType.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute("includeInGlobalAddressList", AttributeType.Boolean, AttributeOperation.ImportExport));
            groupType.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute("allowExternalMembers", AttributeType.Boolean, AttributeOperation.ImportExport));
            groupType.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute("allowGoogleCommunication", AttributeType.Boolean, AttributeOperation.ImportExport));
            groupType.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute("allowWebPosting", AttributeType.Boolean, AttributeOperation.ImportExport));
            groupType.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute("archiveOnly", AttributeType.Boolean, AttributeOperation.ImportExport));
            groupType.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute("isArchived", AttributeType.Boolean, AttributeOperation.ImportExport));
            groupType.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute("membersCanPostAsTheGroup", AttributeType.Boolean, AttributeOperation.ImportExport));
            groupType.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute("sendMessageDenyNotification", AttributeType.Boolean, AttributeOperation.ImportExport));
            groupType.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute("showInGroupDirectory", AttributeType.Boolean, AttributeOperation.ImportExport));
            groupType.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute("customReplyTo", AttributeType.String, AttributeOperation.ImportExport));
            groupType.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute("defaultMessageDenyNotificationText", AttributeType.String, AttributeOperation.ImportExport));
            groupType.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute("messageDisplayFont", AttributeType.String, AttributeOperation.ImportExport));
            groupType.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute("messageModerationLevel", AttributeType.String, AttributeOperation.ImportExport));
            groupType.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute("primaryLanguage", AttributeType.String, AttributeOperation.ImportExport));
            groupType.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute("replyTo", AttributeType.String, AttributeOperation.ImportExport));
            groupType.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute("spamModerationLevel", AttributeType.String, AttributeOperation.ImportExport));
            groupType.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute("whoCanContactOwner", AttributeType.String, AttributeOperation.ImportExport));
            groupType.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute("whoCanInvite", AttributeType.String, AttributeOperation.ImportExport));
            groupType.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute("whoCanJoin", AttributeType.String, AttributeOperation.ImportExport));
            groupType.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute("whoCanLeaveGroup", AttributeType.String, AttributeOperation.ImportExport));
            groupType.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute("whoCanPostMessage", AttributeType.String, AttributeOperation.ImportExport));
            groupType.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute("whoCanViewGroup", AttributeType.String, AttributeOperation.ImportExport));
            groupType.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute("whoCanViewMembership", AttributeType.String, AttributeOperation.ImportExport));

            // Group member API
            foreach (string attribute in ManagementAgentSchema.GroupMemberAttributes)
            {
                groupType.Attributes.Add(SchemaAttribute.CreateMultiValuedAttribute(attribute, AttributeType.Reference, AttributeOperation.ImportExport));
            }

            return groupType;
        }

        private static SchemaType GetUserType(IManagementAgentParameters config, bool advanced)
        {
            SchemaType userType = SchemaType.Create(advanced ? "advancedUser" : "user", true);
            userType.Attributes.Add(SchemaAttribute.CreateAnchorAttribute("id", AttributeType.String, AttributeOperation.ImportOnly));
            userType.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute("name_givenName", AttributeType.String, AttributeOperation.ImportExport));
            userType.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute("name_familyName", AttributeType.String, AttributeOperation.ImportExport));
            userType.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute("name_fullName", AttributeType.String, AttributeOperation.ImportOnly));
            userType.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute("suspended", AttributeType.Boolean, AttributeOperation.ImportExport));
            userType.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute("includeInGlobalAddressList", AttributeType.Boolean, AttributeOperation.ImportExport));
            userType.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute("suspensionReason", AttributeType.String, AttributeOperation.ImportOnly));
            userType.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute("orgUnitPath", AttributeType.String, AttributeOperation.ImportExport));
            userType.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute("notes_value", AttributeType.String, AttributeOperation.ImportExport));
            userType.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute("notes_contentType", AttributeType.String, AttributeOperation.ImportExport));
            userType.Attributes.Add(SchemaAttribute.CreateMultiValuedAttribute("aliases", AttributeType.String, AttributeOperation.ImportExport));
            userType.Attributes.Add(SchemaAttribute.CreateMultiValuedAttribute("nonEditableAliases", AttributeType.String, AttributeOperation.ImportOnly));

            userType.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute("isAdmin", AttributeType.Boolean, AttributeOperation.ImportOnly));
            userType.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute("isDelegatedAdmin", AttributeType.Boolean, AttributeOperation.ImportOnly));
            userType.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute("agreedToTerms", AttributeType.Boolean, AttributeOperation.ImportOnly));
            userType.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute("changePasswordAtNextLogin", AttributeType.Boolean, AttributeOperation.ImportExport));
            userType.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute("ipWhitelisted", AttributeType.Boolean, AttributeOperation.ImportExport));
            userType.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute("isMailboxSetup", AttributeType.Boolean, AttributeOperation.ImportOnly));

            userType.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute("lastLoginTime", AttributeType.String, AttributeOperation.ImportOnly));
            userType.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute("creationTime", AttributeType.String, AttributeOperation.ImportOnly));
            userType.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute("thumbnailPhotoUrl", AttributeType.String, AttributeOperation.ImportOnly));
            userType.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute("deletionTime", AttributeType.String, AttributeOperation.ImportOnly));
            userType.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute("primaryEmail", AttributeType.String, AttributeOperation.ImportExport));

            CreateExternalIDsAttributes(config, userType);
            CreatePhonesAttributes(config, userType);
            CreateOrganizationAttributes(config, userType);
            CreateIMAttributes(config, userType);
            CreateRelationsAttributes(config, userType);
            CreateAddressesAttributes(config, userType);
            CreateWebsitesAttributes(config, userType);

            return userType;
        }

        private static void CreateExternalIDsAttributes(IManagementAgentParameters config, SchemaType userType)
        {
            if (config.ExternalIDsAttributeFormat == GoogleArrayMode.Json)
            {
                userType.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute("externalIds", AttributeType.String, AttributeOperation.ImportExport));
            }
            else
            {
                foreach (string type in config.ExternalIDsAttributeFixedTypes)
                {
                    userType.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute("externalIds_" + type, AttributeType.String, AttributeOperation.ImportExport));
                }
            }
        }

        private static void CreateRelationsAttributes(IManagementAgentParameters config, SchemaType userType)
        {
            if (config.RelationsAttributeFormat == GoogleArrayMode.Json)
            {
                userType.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute("relations", AttributeType.String, AttributeOperation.ImportExport));
            }
            else
            {
                foreach (string type in config.RelationsAttributeFixedTypes)
                {
                    userType.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute("relations_" + type, AttributeType.String, AttributeOperation.ImportExport));
                }
            }
        }

        private static void CreatePhonesAttributes(IManagementAgentParameters config, SchemaType userType)
        {
            if (config.PhonesAttributeFormat == GoogleArrayMode.Json)
            {
                userType.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute("phones", AttributeType.String, AttributeOperation.ImportExport));
            }
            else if (config.PhonesAttributeFormat == GoogleArrayMode.PrimaryValueOnly)
            {
                userType.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute("phones_primary", AttributeType.String, AttributeOperation.ImportExport));
                userType.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute("phones_primary_type", AttributeType.String, AttributeOperation.ImportExport));
            }
            else if (config.PhonesAttributeFormat == GoogleArrayMode.FlattenKnownTypes)
            {
                userType.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute("phones_primary", AttributeType.String, AttributeOperation.ImportExport));
                userType.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute("phones_primary_type", AttributeType.String, AttributeOperation.ImportExport));

                foreach (string type in config.PhonesAttributeFixedTypes)
                {
                    userType.Attributes.Add(SchemaAttribute.CreateMultiValuedAttribute("phones_" + type, AttributeType.String, AttributeOperation.ImportExport));
                }
            }
        }

        private static void CreateWebsitesAttributes(IManagementAgentParameters config, SchemaType userType)
        {
            if (config.WebsitesAttributeFormat == GoogleArrayMode.Json)
            {
                userType.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute("websites", AttributeType.String, AttributeOperation.ImportExport));
            }
            else if (config.WebsitesAttributeFormat == GoogleArrayMode.PrimaryValueOnly)
            {
                userType.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute("websites_primary", AttributeType.String, AttributeOperation.ImportExport));
                userType.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute("websites_primary_type", AttributeType.String, AttributeOperation.ImportExport));
            }
            else if (config.WebsitesAttributeFormat == GoogleArrayMode.FlattenKnownTypes)
            {
                userType.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute("websites_primary", AttributeType.String, AttributeOperation.ImportExport));
                userType.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute("websites_primary_type", AttributeType.String, AttributeOperation.ImportExport));

                foreach (string type in config.WebsitesAttributeFixedTypes)
                {
                    userType.Attributes.Add(SchemaAttribute.CreateMultiValuedAttribute("websites_" + type, AttributeType.String, AttributeOperation.ImportExport));
                }
            }
        }

        private static void CreateOrganizationAttributes(IManagementAgentParameters config, SchemaType userType)
        {
            if (config.OrganizationsAttributeFormat == GoogleArrayMode.Json)
            {
                userType.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute("organizations", AttributeType.String, AttributeOperation.ImportExport));
            }
            else if (config.OrganizationsAttributeFormat == GoogleArrayMode.PrimaryValueOnly)
            {
                CreateOrganizationAttributes(userType, "primary");
            }
            else if (config.OrganizationsAttributeFormat == GoogleArrayMode.FlattenKnownTypes)
            {
                CreateOrganizationAttributes(userType, "primary");

                foreach (string type in config.OrganizationsAttributeFixedTypes)
                {
                    CreateOrganizationAttributes(userType, type);
                }
            }
        }

        private static void CreateIMAttributes(IManagementAgentParameters config, SchemaType userType)
        {
            if (config.IMsAttributeFormat == GoogleArrayMode.Json)
            {
                userType.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute("ims", AttributeType.String, AttributeOperation.ImportExport));
            }
            else if (config.IMsAttributeFormat == GoogleArrayMode.PrimaryValueOnly)
            {
                CreateIMAttributes(userType, "primary");
            }
            else if (config.IMsAttributeFormat == GoogleArrayMode.FlattenKnownTypes)
            {
                CreateIMAttributes(userType, "primary");

                foreach (string type in config.IMsAttributeFixedTypes)
                {
                    CreateIMAttributes(userType, type);
                }
            }
        }

        private static void CreateIMAttributes(SchemaType userType, string type)
        {
            userType.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(string.Format("ims_{0}_protocol", type), AttributeType.String, AttributeOperation.ImportExport));
            userType.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(string.Format("ims_{0}_im", type), AttributeType.String, AttributeOperation.ImportExport));

            if (type == "primary")
            {
                userType.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(string.Format("ims_{0}_type", type), AttributeType.String, AttributeOperation.ImportExport));
            }
        }

        private static void CreateAddressesAttributes(IManagementAgentParameters config, SchemaType userType)
        {
            if (config.AddressesAttributeFormat == GoogleArrayMode.Json)
            {
                userType.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute("addresses", AttributeType.String, AttributeOperation.ImportExport));
            }
            else if (config.AddressesAttributeFormat == GoogleArrayMode.PrimaryValueOnly)
            {
                CreateAddressesAttributes(userType, "primary");
            }
            else if (config.AddressesAttributeFormat == GoogleArrayMode.FlattenKnownTypes)
            {
                CreateAddressesAttributes(userType, "primary");

                foreach (string type in config.AddressesAttributeFixedTypes)
                {
                    CreateAddressesAttributes(userType, type);
                }
            }
        }

        private static void CreateAddressesAttributes(SchemaType userType, string type)
        {
            userType.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(string.Format("addresses_{0}_sourceIsStructured", type), AttributeType.Boolean, AttributeOperation.ImportExport));
            userType.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(string.Format("addresses_{0}_formatted", type), AttributeType.String, AttributeOperation.ImportExport));
            userType.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(string.Format("addresses_{0}_poBox", type), AttributeType.String, AttributeOperation.ImportExport));
            userType.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(string.Format("addresses_{0}_extendedAddress", type), AttributeType.String, AttributeOperation.ImportExport));
            userType.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(string.Format("addresses_{0}_streetAddress", type), AttributeType.String, AttributeOperation.ImportExport));
            userType.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(string.Format("addresses_{0}_locality", type), AttributeType.String, AttributeOperation.ImportExport));
            userType.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(string.Format("addresses_{0}_region", type), AttributeType.String, AttributeOperation.ImportExport));
            userType.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(string.Format("addresses_{0}_postalCode", type), AttributeType.String, AttributeOperation.ImportExport));
            userType.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(string.Format("addresses_{0}_country", type), AttributeType.String, AttributeOperation.ImportExport));
            userType.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(string.Format("addresses_{0}_countryCode", type), AttributeType.String, AttributeOperation.ImportExport));

            if (type == "primary")
            {
                userType.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(string.Format("addresses_{0}_type", type), AttributeType.String, AttributeOperation.ImportExport));
            }
        }

        private static void CreateOrganizationAttributes(SchemaType userType, string type)
        {
            userType.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(string.Format("organizations_{0}_title", type), AttributeType.String, AttributeOperation.ImportExport));
            userType.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(string.Format("organizations_{0}_location", type), AttributeType.String, AttributeOperation.ImportExport));
            userType.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(string.Format("organizations_{0}_name", type), AttributeType.String, AttributeOperation.ImportExport));

            if (type == "primary")
            {
                userType.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(string.Format("organizations_{0}_type", type), AttributeType.String, AttributeOperation.ImportExport));
            }

            userType.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(string.Format("organizations_{0}_description", type), AttributeType.String, AttributeOperation.ImportExport));
            userType.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(string.Format("organizations_{0}_department", type), AttributeType.String, AttributeOperation.ImportExport));
            userType.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(string.Format("organizations_{0}_symbol", type), AttributeType.String, AttributeOperation.ImportExport));
            userType.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(string.Format("organizations_{0}_domain", type), AttributeType.String, AttributeOperation.ImportExport));
            userType.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(string.Format("organizations_{0}_costCenter", type), AttributeType.String, AttributeOperation.ImportExport));
        }

        public static string ConvertTypesToFieldParameter(string subType, SchemaType type)
        {
            return ConvertTypesToFieldParameter(subType, type.Attributes.Select(t => t.Name));
        }

        public static string ConvertTypesToFieldParameter(string subType, IEnumerable<string> attributeNames)
        {
            HashSet<string> names = new HashSet<string>();

            if (subType == "user")
            {
                foreach (string name in attributeNames)
                {
                    if (name.StartsWith("organizations_"))
                    {
                        names.Add("organizations");
                    }
                    else if (name.StartsWith("externalIds_"))
                    {
                        names.Add("externalIds");
                    }
                    else if (name.StartsWith("ims_"))
                    {
                        names.Add("ims");
                    }
                    else if (name.StartsWith("phones_"))
                    {
                        names.Add("phones");
                    }
                    else if (name.StartsWith("relations_"))
                    {
                        names.Add("relations");
                    }
                    else if (name.StartsWith("notes_"))
                    {
                        names.Add("notes");
                    }
                    else if (name.StartsWith("name_"))
                    {
                        names.Add("name");
                    }
                    else if (name.StartsWith("addresses_"))
                    {
                        names.Add("addresses");
                    }
                    else if (name.StartsWith("websites_"))
                    {
                        names.Add("websites");
                    }
                    else
                    {
                        names.Add(name);
                    }
                }

                names.Add("primaryEmail");
                names.Add("id");
                names.Add("kind");
            }
            else if (subType == "group")
            {
                foreach (string name in attributeNames)
                {
                    if (name == "name")
                    {
                        names.Add(name);
                    }
                    else if (name == "description")
                    {
                        names.Add(name);
                    }
                    else if (name == "adminCreated")
                    {
                        names.Add(name);
                    }
                    else if (name == "aliases")
                    {
                        names.Add(name);
                    }
                    else if (name == "primaryEmail")
                    {
                        names.Add("email");
                    }
                }

                names.Add("id");
                names.Add("email");
                names.Add("kind");
            }
            else if (subType == "groupSettings")
            {
                foreach (string name in attributeNames)
                {
                    if (name == "name" || name == "description" || name == "adminCreated" || name == "aliases")
                    {
                        continue;
                    }
                    else if (name == "member" || name == "externalMember" || name == "manager" || name == "externalManager" || name == "owner" || name == "externalOwner")
                    {
                        continue;
                    }

                    names.Add(name);
                }

                names.Add("email");
                names.Add("kind");
            }

            return names.ToSeparatedString(",");
        }

        public static bool IsGroupMembershipRequired(SchemaType type)
        {
            return type.Attributes.Any(t => ManagementAgentSchema.GroupMemberAttributes.Contains(t.Name));
        }

        public static bool IsGroupSettingsRequired(SchemaType type)
        {
            return type.Attributes.Any(t => ManagementAgentSchema.GroupSettingsAttributes.Contains(t.Name));
        }

        private static IEnumerable<string> GroupMemberAttributes
        {
            get
            {
                yield return "member";
                yield return "externalMember";
                yield return "manager";
                yield return "externalManager";
                yield return "owner";
                yield return "externalOwner";
            }
        }

        private static IEnumerable<string> GroupSettingsAttributes
        {
            get
            {
                yield return "whoCanViewMembership";
                yield return "whoCanViewGroup";
                yield return "whoCanPostMessage";
                yield return "whoCanLeaveGroup";
                yield return "whoCanInvite";
                yield return "whoCanJoin";
                yield return "whoCanContactOwner";
                yield return "spamModerationLevel";
                yield return "replyTo";
                yield return "primaryLanguage";
                yield return "messageModerationLevel";
                yield return "messageDisplayFont";
                yield return "defaultMessageDenyNotificationText";
                yield return "customReplyTo";
                yield return "showInGroupDirectory";
                yield return "sendMessageDenyNotification";
                yield return "membersCanPostAsTheGroup";
                yield return "isArchived";
                yield return "archiveOnly";
                yield return "allowWebPosting";
                yield return "allowGoogleCommunication";
                yield return "allowExternalMembers";
                yield return "includeInGlobalAddressList";
                yield return "maxMessageBytes";
            }
        }

        public static IEnumerable<string> GroupAttributesRequiringFullUpdate
        {
            get
            {
                yield break;
            }
        }

        public static IEnumerable<string> UserAttributesRequiringFullUpdate
        {
            get
            {
                yield return "ims";
                yield return "externalIds";
                yield return "relations";
                yield return "addresses";
                yield return "organizations";
                yield return "phones";
                yield return "aliases";
                yield return "websites";
                yield return "name";
                yield return "notes";
                yield return "name";
            }
        }
    }
}
