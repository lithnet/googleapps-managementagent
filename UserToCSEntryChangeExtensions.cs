using System;
using System.Collections.Generic;
using System.Linq;
using Lithnet.GoogleApps.ManagedObjects;
using Microsoft.MetadirectoryServices;
using Lithnet.MetadirectoryServices;

namespace Lithnet.GoogleApps.MA
{
    using System.Diagnostics.CodeAnalysis;

    public static class UserToCSEntryChangeExtensions
    {
        private static GoogleJsonSerializer serializer = new GoogleJsonSerializer();

        internal static void ToCSEntryChange(this User user, IManagementAgentParameters config, SchemaType type, CSEntryChange csentry)
        {
            UserToCSEntryChangeExtensions.UserCoreToCSEntryChange(user, type, csentry);
            UserToCSEntryChangeExtensions.UserOrganizationsToCSEntryChange(user, type, config, csentry);
            UserToCSEntryChangeExtensions.UserIMsToCSEntryChange(user, type, config, csentry);
            UserToCSEntryChangeExtensions.UserPhonesToCSEntryChange(user, type, config, csentry);
            UserToCSEntryChangeExtensions.UserExternalIDsToCSEntryChange(user, type, config, csentry);
            UserToCSEntryChangeExtensions.UserNotesToCSEntryChange(user, type, config, csentry);
            UserToCSEntryChangeExtensions.UserWebsitesToCSEntryChange(user, type, config, csentry);
            UserToCSEntryChangeExtensions.UserAddressesToCSEntryChange(user, type, config, csentry);
            UserToCSEntryChangeExtensions.UserRelationsToCSEntryChange(user, type, config, csentry);
        }

        private static void UserCoreToCSEntryChange(User user, SchemaType type, CSEntryChange csentry)
        {
            AttributeModificationType modificationType = csentry.ObjectModificationType == ObjectModificationType.Update ? AttributeModificationType.Replace : AttributeModificationType.Add;
            csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("id", user.Id));
            
            csentry.CreateAttributeChangeIfInSchema(type, "orgUnitPath", modificationType, user.OrgUnitPath);
            csentry.CreateAttributeChangeIfInSchema(type, "primaryEmail", modificationType, user.PrimaryEmail);
            csentry.CreateAttributeChangeIfInSchema(type, "suspended", modificationType, user.Suspended);
            csentry.CreateAttributeChangeIfInSchema(type, "suspensionReason", modificationType, user.SuspensionReason);
            csentry.CreateAttributeChangeIfInSchema(type, "includeInGlobalAddressList", modificationType, user.IncludeInGlobalAddressList);

            if (user.Name != null)
            {
                csentry.CreateAttributeChangeIfInSchema(type, "name_givenName", modificationType, user.Name.GivenName);
                csentry.CreateAttributeChangeIfInSchema(type, "name_familyName", modificationType, user.Name.FamilyName);
                csentry.CreateAttributeChangeIfInSchema(type, "name_fullName", modificationType, user.Name.FullName);
            }

            csentry.CreateAttributeChangeIfInSchema(type, "isAdmin", modificationType, user.IsAdmin);
            csentry.CreateAttributeChangeIfInSchema(type, "isDelegatedAdmin", modificationType, user.IsDelegatedAdmin);
            csentry.CreateAttributeChangeIfInSchema(type, "lastLoginTime", modificationType, user.LastLoginTime.ToResourceManagementServiceDateFormat(true));
            csentry.CreateAttributeChangeIfInSchema(type, "creationTime", modificationType, user.CreationTime.ToResourceManagementServiceDateFormat(true));
            csentry.CreateAttributeChangeIfInSchema(type, "agreedToTerms", modificationType, user.AgreedToTerms);
            csentry.CreateAttributeChangeIfInSchema(type, "changePasswordAtNextLogin", modificationType, user.ChangePasswordAtNextLogin);
            csentry.CreateAttributeChangeIfInSchema(type, "ipWhitelisted", modificationType, user.IpWhitelisted);
            csentry.CreateAttributeChangeIfInSchema(type, "isMailboxSetup", modificationType, user.IsMailboxSetup);
            csentry.CreateAttributeChangeIfInSchema(type, "thumbnailPhotoUrl", modificationType, user.ThumbnailPhotoUrl);
            csentry.CreateAttributeChangeIfInSchema(type, "deletionTime", modificationType, user.DeletionTime.ToResourceManagementServiceDateFormat(true));
            csentry.CreateAttributeChangeIfInSchema(type, "aliases", modificationType, user.Aliases?.ToList<object>());
            csentry.CreateAttributeChangeIfInSchema(type, "nonEditableAliases", modificationType, user.NonEditableAliases?.ToList<object>());
        }

        private static void UserNotesToCSEntryChange(User user, SchemaType type, IManagementAgentParameters config, CSEntryChange csentry)
        {
            AttributeModificationType modificationType = csentry.ObjectModificationType == ObjectModificationType.Update ? AttributeModificationType.Replace : AttributeModificationType.Add;

            if (user.Notes == null)
            {
                return;
            }

            csentry.CreateAttributeChangeIfInSchema(type, "notes_value", modificationType, user.Notes.Value);
            csentry.CreateAttributeChangeIfInSchema(type, "notes_contentType", modificationType, user.Notes.ContentType);
        }

        private static void UserIMsToCSEntryChange(User user, SchemaType type, IManagementAgentParameters config, CSEntryChange csentry)
        {
            AttributeModificationType modificationType = csentry.ObjectModificationType == ObjectModificationType.Update ? AttributeModificationType.Replace : AttributeModificationType.Add;
            if (user.Ims == null)
            {
                return;
            }

            if (config.IMsAttributeFormat == GoogleArrayMode.Json)
            {
                csentry.CreateAttributeChangeIfInSchema(type, "ims", modificationType, UserToCSEntryChangeExtensions.serializer.Serialize(user.Ims));
                return;
            }

            HashSet<string> processedTypes = new HashSet<string>();

            foreach (IM im in user.Ims)
            {
                if (!im.IsPrimary && config.IMsAttributeFormat == GoogleArrayMode.PrimaryValueOnly)
                {
                    continue;
                }

                if (!im.IsPrimary && !config.IMsAttributeFixedTypes.Contains(im.Type))
                {
                    continue;
                }

                string attributePrefix;
                if (im.IsPrimary)
                {
                    attributePrefix = "ims_primary_";
                }
                else
                {
                    attributePrefix = $"ims_{im.Type}_";

                }

                if (!processedTypes.Add(im.Type))
                {
                    Logging.Logger.WriteLine("Cannot add another instance of type {0}. Ignoring element {1} for user {2}", im.Type, "ims", csentry.DN);
                    continue;
                }

                csentry.CreateAttributeChangeIfInSchema(type, attributePrefix + "im", modificationType, im.IMAddress);
                csentry.CreateAttributeChangeIfInSchema(type, attributePrefix + "protocol", modificationType, im.Protocol);

                if (im.IsPrimary)
                {
                    csentry.CreateAttributeChangeIfInSchema(type, attributePrefix + "type", modificationType, im.Type);
                }
            }
        }

        private static void UserAddressesToCSEntryChange(User user, SchemaType type, IManagementAgentParameters config, CSEntryChange csentry)
        {
            AttributeModificationType modificationType = csentry.ObjectModificationType == ObjectModificationType.Update ? AttributeModificationType.Replace : AttributeModificationType.Add;
            if (user.Addresses == null)
            {
                return;
            }

            if (config.AddressesAttributeFormat == GoogleArrayMode.Json)
            {
                csentry.CreateAttributeChangeIfInSchema(type, "addresses", modificationType, UserToCSEntryChangeExtensions.serializer.Serialize(user.Addresses));
                return;
            }

            HashSet<string> processedTypes = new HashSet<string>();

            foreach (Address address in user.Addresses)
            {
                if (!address.IsPrimary && config.AddressesAttributeFormat == GoogleArrayMode.PrimaryValueOnly)
                {
                    continue;
                }

                if (!address.IsPrimary && !config.AddressesAttributeFixedTypes.Contains(address.Type))
                {
                    continue;
                }

                string attributePrefix;
                if (address.IsPrimary)
                {
                    attributePrefix = "addresses_primary_";
                }
                else
                {
                    attributePrefix = $"addresses_{address.Type}_";

                }

                if (!processedTypes.Add(address.Type))
                {
                    Logging.Logger.WriteLine("Cannot add another instance of type {0}. Ignoring element {1} for user {2}", address.Type, "addresses", csentry.DN);
                    continue;
                }

                csentry.CreateAttributeChangeIfInSchema(type, attributePrefix + "sourceIsStructured", modificationType, address.SourceIsStructured);
                csentry.CreateAttributeChangeIfInSchema(type, attributePrefix + "formatted", modificationType, address.Formatted);
                csentry.CreateAttributeChangeIfInSchema(type, attributePrefix + "poBox", modificationType, address.POBox);
                csentry.CreateAttributeChangeIfInSchema(type, attributePrefix + "extendedAddress", modificationType, address.ExtendedAddress);
                csentry.CreateAttributeChangeIfInSchema(type, attributePrefix + "streetAddress", modificationType, address.StreetAddress);
                csentry.CreateAttributeChangeIfInSchema(type, attributePrefix + "locality", modificationType, address.Locality);
                csentry.CreateAttributeChangeIfInSchema(type, attributePrefix + "region", modificationType, address.Region);
                csentry.CreateAttributeChangeIfInSchema(type, attributePrefix + "postalCode", modificationType, address.PostalCode);
                csentry.CreateAttributeChangeIfInSchema(type, attributePrefix + "country", modificationType, address.Country);
                csentry.CreateAttributeChangeIfInSchema(type, attributePrefix + "countryCode", modificationType, address.CountryCode);

                if (address.IsPrimary)
                {
                    csentry.CreateAttributeChangeIfInSchema(type, attributePrefix + "type", modificationType, address.Type);
                }
            }
        }

        private static void UserOrganizationsToCSEntryChange(User user, SchemaType type, IManagementAgentParameters config, CSEntryChange csentry)
        {
            AttributeModificationType modificationType = csentry.ObjectModificationType == ObjectModificationType.Update ? AttributeModificationType.Replace : AttributeModificationType.Add;

            if (user.Organizations == null)
            {
                return;
            }

            if (config.OrganizationsAttributeFormat == GoogleArrayMode.Json)
            {
                csentry.CreateAttributeChangeIfInSchema(type, "organizations", modificationType, UserToCSEntryChangeExtensions.serializer.Serialize(user.Organizations));
                return;
            }

            HashSet<string> processedTypes = new HashSet<string>();

            foreach (Organization organization in user.Organizations)
            {
                if (!organization.IsPrimary && config.OrganizationsAttributeFormat == GoogleArrayMode.PrimaryValueOnly)
                {
                    continue;
                }

                if (!organization.IsPrimary && !config.OrganizationsAttributeFixedTypes.Contains(organization.Type))
                {
                    continue;
                }

                string attributePrefix;
                if (organization.IsPrimary)
                {
                    attributePrefix = "organizations_primary_";
                }
                else
                {
                    attributePrefix = $"organizations_{organization.Type}_";

                }

                if (!processedTypes.Add(organization.Type))
                {
                    Logging.Logger.WriteLine("Cannot add another instance of type {0}. Ignoring element {1} for user {2}", organization.Type, "organization", csentry.DN);
                    continue;
                }

                csentry.CreateAttributeChangeIfInSchema(type, attributePrefix + "name", modificationType, organization.Name);
                csentry.CreateAttributeChangeIfInSchema(type, attributePrefix + "title", modificationType, organization.Title);
                csentry.CreateAttributeChangeIfInSchema(type, attributePrefix + "department", modificationType, organization.Department);
                csentry.CreateAttributeChangeIfInSchema(type, attributePrefix + "symbol", modificationType, organization.Symbol);
                csentry.CreateAttributeChangeIfInSchema(type, attributePrefix + "location", modificationType, organization.Location);
                csentry.CreateAttributeChangeIfInSchema(type, attributePrefix + "description", modificationType, organization.Description);
                csentry.CreateAttributeChangeIfInSchema(type, attributePrefix + "domain", modificationType, organization.Domain);
                csentry.CreateAttributeChangeIfInSchema(type, attributePrefix + "costCenter", modificationType, organization.CostCenter);

                if (organization.IsPrimary)
                {
                    csentry.CreateAttributeChangeIfInSchema(type, attributePrefix + "type", modificationType, organization.Type);
                }
            }
        }

        private static void UserPhonesToCSEntryChange(User user, SchemaType type, IManagementAgentParameters config, CSEntryChange csentry)
        {
            AttributeModificationType modificationType = csentry.ObjectModificationType == ObjectModificationType.Update ? AttributeModificationType.Replace : AttributeModificationType.Add;

            if (user.Phones == null)
            {
                return;
            }

            if (config.PhonesAttributeFormat == GoogleArrayMode.Json)
            {
                csentry.CreateAttributeChangeIfInSchema(type, "phones", modificationType, UserToCSEntryChangeExtensions.serializer.Serialize(user.Phones));
                return;
            }

            Dictionary<string, List<string>> values = new Dictionary<string, List<string>>();

            foreach (Phone phone in user.Phones)
            {
                if (string.IsNullOrEmpty(phone.Value))
                {
                    continue;
                }

                if (phone.IsPrimary)
                {
                    csentry.CreateAttributeChangeIfInSchema(type, "phones_primary", modificationType, phone.Value);
                    csentry.CreateAttributeChangeIfInSchema(type, "phones_primary_type", modificationType, phone.Type);
                }

                if (config.PhonesAttributeFormat != GoogleArrayMode.FlattenKnownTypes)
                {
                    continue;
                }

                if (!config.PhonesAttributeFixedTypes.Contains(phone.Type))
                {
                    continue;
                }

                if (!values.ContainsKey(phone.Type))
                {
                    values.Add(phone.Type, new List<string>());
                }

                values[phone.Type].Add(phone.Value);
            }

            foreach (KeyValuePair<string, List<string>> kvp in values)
            {
                if (kvp.Value != null && kvp.Value.Count > 0)
                {
                    csentry.CreateAttributeChangeIfInSchema(type, $"phones_{kvp.Key}", modificationType, kvp.Value.ToList<object>());
                }
            }
        }

        private static void UserWebsitesToCSEntryChange(User user, SchemaType type, IManagementAgentParameters config, CSEntryChange csentry)
        {
            AttributeModificationType modificationType = csentry.ObjectModificationType == ObjectModificationType.Update ? AttributeModificationType.Replace : AttributeModificationType.Add;
            if (user.Websites == null)
            {
                return;
            }

            if (config.WebsitesAttributeFormat == GoogleArrayMode.Json)
            {
                csentry.CreateAttributeChangeIfInSchema(type, "websites", modificationType, user.Websites.Serialize());
                return;
            }

            Dictionary<string, List<string>> values = new Dictionary<string, List<string>>();

            foreach (Website website in user.Websites)
            {
                if (string.IsNullOrEmpty(website.Value))
                {
                    continue;
                }

                if (website.IsPrimary)
                {
                    csentry.CreateAttributeChangeIfInSchema(type, "websites_primary", modificationType, website.Value);
                    csentry.CreateAttributeChangeIfInSchema(type, "websites_primary_type", modificationType, website.Type);
                }

                if (config.WebsitesAttributeFormat != GoogleArrayMode.FlattenKnownTypes)
                {
                    continue;
                }

                if (!config.WebsitesAttributeFixedTypes.Contains(website.Type))
                {
                    continue;
                }

                if (!values.ContainsKey(website.Type))
                {
                    values.Add(website.Type, new List<string>());
                }

                values[website.Type].Add(website.Value);
            }

            foreach (KeyValuePair<string, List<string>> kvp in values)
            {
                if (kvp.Value != null && kvp.Value.Count > 0)
                {
                    csentry.CreateAttributeChangeIfInSchema(type, $"websites_{kvp.Key}", modificationType, kvp.Value.ToList<object>());
                }
            }
        }

        private static void UserExternalIDsToCSEntryChange(User user, SchemaType type, IManagementAgentParameters config, CSEntryChange csentry)
        {
            AttributeModificationType modificationType = csentry.ObjectModificationType == ObjectModificationType.Update ? AttributeModificationType.Replace : AttributeModificationType.Add;
            if (user.ExternalIds == null)
            {
                return;
            }

            if (config.ExternalIDsAttributeFormat == GoogleArrayMode.Json)
            {
                csentry.CreateAttributeChangeIfInSchema(type, "externalIds", modificationType, UserToCSEntryChangeExtensions.serializer.Serialize(user.ExternalIds));
                return;
            }

            IList<string> values = new List<string>();

            foreach (ExternalID id in user.ExternalIds.Where(t => config.ExternalIDsAttributeFixedTypes.Contains(t.Type)))
            {
                if (string.IsNullOrEmpty(id.Value))
                {
                    continue;
                }

                csentry.CreateAttributeChangeIfInSchema(type, $"externalIds_{id.Type}", modificationType, id.Value);
            }
        }

        private static void UserRelationsToCSEntryChange(User user, SchemaType type, IManagementAgentParameters config, CSEntryChange csentry)
        {
            AttributeModificationType modificationType = csentry.ObjectModificationType == ObjectModificationType.Update ? AttributeModificationType.Replace : AttributeModificationType.Add;

            if (user.Relations == null)
            {
                return;
            }

            if (config.RelationsAttributeFormat == GoogleArrayMode.Json)
            {
                csentry.CreateAttributeChangeIfInSchema(type, "relations", modificationType, UserToCSEntryChangeExtensions.serializer.Serialize(user.Relations));
                return;
            }

            IList<string> values = new List<string>();

            foreach (Relation relation in user.Relations)
            {
                if (string.IsNullOrEmpty(relation.Value))
                {
                    continue;
                }

                if (string.IsNullOrWhiteSpace(relation.Type))
                {
                    continue;
                }

                if (config.RelationsAttributeFixedTypes.Contains(relation.Type))
                {
                    csentry.CreateAttributeChangeIfInSchema(type, $"relations_{relation.Type}" , modificationType, relation.Value);
                }
            }
        }
    }
}
