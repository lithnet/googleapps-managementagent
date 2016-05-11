using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.MetadirectoryServices;
using System.Collections.ObjectModel;

namespace Lithnet.GoogleApps.MA
{
    internal static class ManagementAgentSchema
    {
        public static Schema GetSchema(IManagementAgentParameters config)
        {
            Schema schema = Schema.Create();

            SchemaType type = ManagementAgentSchema.GetUserType(config, false);
            schema.Types.Add(type);

            if (SchemaRequestFactory.HasSchema(config.CustomerID, SchemaConstants.CustomGoogleAppsSchemaName))
            {
                type = ManagementAgentSchema.GetUserType(config, true);
                schema.Types.Add(type);
            }

            type = ManagementAgentSchema.GetGroupType(config);
            schema.Types.Add(type);

            return schema;
        }

        private static SchemaType GetGroupType(IManagementAgentParameters config)
        {
            SchemaType groupType = SchemaType.Create(SchemaConstants.Group, true);

            // Group API
            groupType.Attributes.Add(SchemaAttribute.CreateAnchorAttribute(SchemaConstants.ID, AttributeType.String, AttributeOperation.ImportOnly));
            groupType.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(SchemaConstants.Name, AttributeType.String, AttributeOperation.ImportExport));
            groupType.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(SchemaConstants.Description, AttributeType.String, AttributeOperation.ImportExport));
            groupType.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(SchemaConstants.AdminCreated, AttributeType.Boolean, AttributeOperation.ImportExport));
            groupType.Attributes.Add(SchemaAttribute.CreateMultiValuedAttribute(SchemaConstants.Aliases, AttributeType.String, AttributeOperation.ImportExport));
            groupType.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(SchemaConstants.PrimaryEmail, AttributeType.String, AttributeOperation.ImportOnly));

            // Group settings API
            groupType.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(SchemaConstants.MaxMessageBytes, AttributeType.Integer, AttributeOperation.ImportExport));
            groupType.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(SchemaConstants.IncludeInGlobalAddressList, AttributeType.Boolean, AttributeOperation.ImportExport));
            groupType.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(SchemaConstants.AllowExternalMembers, AttributeType.Boolean, AttributeOperation.ImportExport));
            groupType.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(SchemaConstants.AllowGoogleCommunication, AttributeType.Boolean, AttributeOperation.ImportExport));
            groupType.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(SchemaConstants.AllowWebPosting, AttributeType.Boolean, AttributeOperation.ImportExport));
            groupType.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(SchemaConstants.ArchiveOnly, AttributeType.Boolean, AttributeOperation.ImportExport));
            groupType.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(SchemaConstants.IsArchived, AttributeType.Boolean, AttributeOperation.ImportExport));
            groupType.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(SchemaConstants.MembersCanPostAsTheGroup, AttributeType.Boolean, AttributeOperation.ImportExport));
            groupType.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(SchemaConstants.SendMessageDenyNotification, AttributeType.Boolean, AttributeOperation.ImportExport));
            groupType.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(SchemaConstants.ShowInGroupDirectory, AttributeType.Boolean, AttributeOperation.ImportExport));
            groupType.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(SchemaConstants.CustomReplyTo, AttributeType.String, AttributeOperation.ImportExport));
            groupType.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(SchemaConstants.DefaultMessageDenyNotificationText, AttributeType.String, AttributeOperation.ImportExport));
            groupType.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(SchemaConstants.MessageDisplayFont, AttributeType.String, AttributeOperation.ImportExport));
            groupType.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(SchemaConstants.MessageModerationLevel, AttributeType.String, AttributeOperation.ImportExport));
            groupType.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(SchemaConstants.PrimaryLanguage, AttributeType.String, AttributeOperation.ImportExport));
            groupType.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(SchemaConstants.ReplyTo, AttributeType.String, AttributeOperation.ImportExport));
            groupType.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(SchemaConstants.SpamModerationLevel, AttributeType.String, AttributeOperation.ImportExport));
            groupType.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(SchemaConstants.WhoCanContactOwner, AttributeType.String, AttributeOperation.ImportExport));
            groupType.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(SchemaConstants.WhoCanInvite, AttributeType.String, AttributeOperation.ImportExport));
            groupType.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(SchemaConstants.WhoCanJoin, AttributeType.String, AttributeOperation.ImportExport));
            groupType.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(SchemaConstants.WhoCanLeaveGroup, AttributeType.String, AttributeOperation.ImportExport));
            groupType.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(SchemaConstants.WhoCanPostMessage, AttributeType.String, AttributeOperation.ImportExport));
            groupType.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(SchemaConstants.WhoCanViewGroup, AttributeType.String, AttributeOperation.ImportExport));
            groupType.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(SchemaConstants.WhoCanViewMembership, AttributeType.String, AttributeOperation.ImportExport));

            // Group member API
            foreach (string attribute in ManagementAgentSchema.GroupMemberAttributes)
            {
                groupType.Attributes.Add(SchemaAttribute.CreateMultiValuedAttribute(attribute, AttributeType.Reference, AttributeOperation.ImportExport));
            }

            return groupType;
        }

        private static SchemaType GetUserType(IManagementAgentParameters config, bool advanced)
        {
            SchemaType userType = SchemaType.Create(advanced ? SchemaConstants.AdvancedUser : SchemaConstants.User, true);
            userType.Attributes.Add(SchemaAttribute.CreateAnchorAttribute(SchemaConstants.ID, AttributeType.String, AttributeOperation.ImportOnly));
            userType.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(SchemaConstants.NameGivenName, AttributeType.String, AttributeOperation.ImportExport));
            userType.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(SchemaConstants.NameFamilyName, AttributeType.String, AttributeOperation.ImportExport));
            userType.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(SchemaConstants.NameFullName, AttributeType.String, AttributeOperation.ImportOnly));
            userType.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(SchemaConstants.Suspended, AttributeType.Boolean, AttributeOperation.ImportExport));
            userType.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(SchemaConstants.IncludeInGlobalAddressList, AttributeType.Boolean, AttributeOperation.ImportExport));
            userType.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(SchemaConstants.SuspensionReason, AttributeType.String, AttributeOperation.ImportOnly));
            userType.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(SchemaConstants.OrgUnitPath, AttributeType.String, AttributeOperation.ImportExport));
            userType.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(SchemaConstants.NotesValue, AttributeType.String, AttributeOperation.ImportExport));
            userType.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(SchemaConstants.NotesContentType, AttributeType.String, AttributeOperation.ImportExport));
            userType.Attributes.Add(SchemaAttribute.CreateMultiValuedAttribute(SchemaConstants.Aliases, AttributeType.String, AttributeOperation.ImportExport));
            userType.Attributes.Add(SchemaAttribute.CreateMultiValuedAttribute(SchemaConstants.NonEditableAliases, AttributeType.String, AttributeOperation.ImportOnly));
            userType.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(SchemaConstants.IsAdmin, AttributeType.Boolean, AttributeOperation.ImportExport));
            userType.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(SchemaConstants.IsDelegatedAdmin, AttributeType.Boolean, AttributeOperation.ImportOnly));
            userType.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(SchemaConstants.AgreedToTerms, AttributeType.Boolean, AttributeOperation.ImportOnly));
            userType.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(SchemaConstants.ChangePasswordAtNextLogin, AttributeType.Boolean, AttributeOperation.ImportExport));
            userType.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(SchemaConstants.IpWhitelisted, AttributeType.Boolean, AttributeOperation.ImportExport));
            userType.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(SchemaConstants.IsMailboxSetup, AttributeType.Boolean, AttributeOperation.ImportOnly));

            userType.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(SchemaConstants.LastLoginTime, AttributeType.String, AttributeOperation.ImportOnly));
            userType.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(SchemaConstants.CreationTime, AttributeType.String, AttributeOperation.ImportOnly));
            userType.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(SchemaConstants.ThumbnailPhotoUrl, AttributeType.String, AttributeOperation.ImportOnly));
            userType.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(SchemaConstants.DeletionTime, AttributeType.String, AttributeOperation.ImportOnly));
            userType.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(SchemaConstants.PrimaryEmail, AttributeType.String, AttributeOperation.ImportOnly));

            return userType;
        }

        public static string GetFieldNamesFromType(string subType, SchemaType type)
        {
            return string.Join(",", ManagementAgentSchema.GetFieldNamesFromAttributeNames(subType, type.Attributes.Select(t => t.Name)));
        }

        private static IEnumerable<string> GetFieldNamesFromAttributeNamesUser(IEnumerable<string> attributeNames)
        {
            HashSet<string> names = new HashSet<string>();

            foreach (string name in attributeNames)
            {
                if (name.StartsWith($"{SchemaConstants.Organizations}_"))
                {
                    names.Add(SchemaConstants.Organizations);
                }
                else if (name.StartsWith("externalIds_"))
                {
                    names.Add(SchemaConstants.ExternalIds);
                }
                else if (name.StartsWith("ims_"))
                {
                    names.Add(SchemaConstants.Ims);
                }
                else if (name.StartsWith("phones_"))
                {
                    names.Add(SchemaConstants.Phones);
                }
                else if (name.StartsWith("relations_"))
                {
                    names.Add(SchemaConstants.Relations);
                }
                else if (name.StartsWith("notes_"))
                {
                    names.Add(SchemaConstants.Notes);
                }
                else if (name.StartsWith("name_"))
                {
                    names.Add(SchemaConstants.Name);
                }
                else if (name.StartsWith("addresses_"))
                {
                    names.Add(SchemaConstants.Addresses);
                }
                else if (name.StartsWith("websites_"))
                {
                    names.Add(SchemaConstants.Websites);
                }
                else
                {
                    names.Add(name);
                }
            }

            names.Add(SchemaConstants.PrimaryEmail);
            names.Add(SchemaConstants.ID);
            names.Add(SchemaConstants.Kind);

            return names;
        }

        private static IEnumerable<string> GetFieldNamesFromAttributeNamesGroup(IEnumerable<string> attributeNames)
        {
            HashSet<string> names = new HashSet<string>();

            foreach (string name in attributeNames)
            {
                switch (name)
                {
                    case SchemaConstants.Name:
                        names.Add(name);
                        break;

                    case SchemaConstants.Description:
                        names.Add(name);
                        break;

                    case SchemaConstants.AdminCreated:
                        names.Add(name);
                        break;

                    case SchemaConstants.Aliases:
                        names.Add(name);
                        break;

                    case SchemaConstants.PrimaryEmail:
                        names.Add(SchemaConstants.Email);
                        break;

                    default:
                        break;
                }
            }

            names.Add(SchemaConstants.ID);
            names.Add(SchemaConstants.Email);
            names.Add(SchemaConstants.Kind);

            return names;
        }

        private static IEnumerable<string> GetFieldNamesFromAttributeNamesGroupSettings(IEnumerable<string> attributeNames)
        {
            HashSet<string> names = new HashSet<string>();

            foreach (string name in attributeNames)
            {
                switch (name)
                {
                    case SchemaConstants.Name:
                    case SchemaConstants.Description:
                    case SchemaConstants.AdminCreated:
                    case SchemaConstants.Aliases:
                    case SchemaConstants.Member:
                    case SchemaConstants.ExternalMember:
                    case SchemaConstants.Manager:
                    case SchemaConstants.ExternalManager:
                    case SchemaConstants.Owner:
                    case SchemaConstants.ExternalOwner:
                        continue;

                    default:
                        break;
                }

                names.Add(name);
            }

            names.Add(SchemaConstants.Email);
            names.Add(SchemaConstants.Kind);

            return names;
        }

        public static IEnumerable<string> GetFieldNamesFromAttributeNames(string subType, IEnumerable<string> attributeNames)
        {
            switch (subType)
            {
                case SchemaConstants.User:
                    return ManagementAgentSchema.GetFieldNamesFromAttributeNamesUser(attributeNames);

                case SchemaConstants.Group:
                    return ManagementAgentSchema.GetFieldNamesFromAttributeNamesGroup(attributeNames);

                case SchemaConstants.GroupSettings:
                    return ManagementAgentSchema.GetFieldNamesFromAttributeNamesGroupSettings(attributeNames);

                default:
                    throw new InvalidOperationException();
            }
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
                yield return SchemaConstants.Member;
                yield return SchemaConstants.ExternalMember;
                yield return SchemaConstants.Manager;
                yield return SchemaConstants.ExternalManager;
                yield return SchemaConstants.Owner;
                yield return SchemaConstants.ExternalOwner;
            }
        }

        private static IEnumerable<string> GroupSettingsAttributes
        {
            get
            {
                yield return SchemaConstants.WhoCanViewMembership;
                yield return SchemaConstants.WhoCanViewGroup;
                yield return SchemaConstants.WhoCanPostMessage;
                yield return SchemaConstants.WhoCanLeaveGroup;
                yield return SchemaConstants.WhoCanInvite;
                yield return SchemaConstants.WhoCanJoin;
                yield return SchemaConstants.WhoCanContactOwner;
                yield return SchemaConstants.SpamModerationLevel;
                yield return SchemaConstants.ReplyTo;
                yield return SchemaConstants.PrimaryLanguage;
                yield return SchemaConstants.MessageModerationLevel;
                yield return SchemaConstants.MessageDisplayFont;
                yield return SchemaConstants.DefaultMessageDenyNotificationText;
                yield return SchemaConstants.CustomReplyTo;
                yield return SchemaConstants.ShowInGroupDirectory;
                yield return SchemaConstants.SendMessageDenyNotification;
                yield return SchemaConstants.MembersCanPostAsTheGroup;
                yield return SchemaConstants.IsArchived;
                yield return SchemaConstants.ArchiveOnly;
                yield return SchemaConstants.AllowWebPosting;
                yield return SchemaConstants.AllowGoogleCommunication;
                yield return SchemaConstants.AllowExternalMembers;
                yield return SchemaConstants.IncludeInGlobalAddressList;
                yield return SchemaConstants.MaxMessageBytes;
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
                yield return SchemaConstants.Ims;
                yield return SchemaConstants.ExternalIds;
                yield return SchemaConstants.Relations;
                yield return SchemaConstants.Addresses;
                yield return SchemaConstants.Organizations;
                yield return SchemaConstants.Phones;
                yield return SchemaConstants.Aliases;
                yield return SchemaConstants.Websites;
                yield return SchemaConstants.Name;
                yield return SchemaConstants.Notes;
                yield return SchemaConstants.Name;
            }
        }
    }
}
