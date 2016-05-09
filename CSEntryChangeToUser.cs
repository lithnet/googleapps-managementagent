using System;
using System.Collections.Generic;
using System.Linq;
using Lithnet.GoogleApps.ManagedObjects;
using Microsoft.MetadirectoryServices;
using Lithnet.MetadirectoryServices;
using Lithnet.Logging;

namespace Lithnet.GoogleApps.MA
{
    public static class CSEntryChangeToUser
    {
        private static GoogleJsonSerializer serializer = new GoogleJsonSerializer();

        public static bool ToUser(this CSEntryChange csentry, User user, IManagementAgentParameters config)
        {
            user.PrimaryEmail = csentry.DN;

            bool updateRequired = false;

            csentry.UpdateTargetFromCSEntryChange(user, x => x.PrimaryEmail, "primaryEmail", ref updateRequired);
            csentry.UpdateTargetFromCSEntryChange(user, x => x.OrgUnitPath, "orgUnitPath", ref updateRequired);
            csentry.UpdateTargetFromCSEntryChange(user, x => x.Suspended, "suspended", ref updateRequired);
            csentry.UpdateTargetFromCSEntryChange(user, x => x.IncludeInGlobalAddressList, "includeInGlobalAddressList", ref updateRequired);
            csentry.UpdateTargetFromCSEntryChange(user, x => x.ChangePasswordAtNextLogin, "changePasswordAtNextLogin", ref updateRequired);
            csentry.UpdateTargetFromCSEntryChange(user, x => x.IpWhitelisted, "ipWhitelisted", ref updateRequired);
            csentry.UpdateTargetFromCSEntryChange(user.Name, x => x.GivenName, "name_givenName", ref updateRequired);
            csentry.UpdateTargetFromCSEntryChange(user.Name, x => x.FamilyName, "name_familyName", ref updateRequired);


            if (csentry.ToUserExternalIDs(user, config))
            {
                updateRequired = true;
            }

            if (csentry.ToUserPhones(user, config))
            {
                updateRequired = true;
            }

            if (csentry.ToUserIMs(user, config))
            {
                updateRequired = true;
            }

            if (csentry.ToUserOrganizations(user, config))
            {
                updateRequired = true;
            }

            if (csentry.ToUserAddresses(user, config))
            {
                updateRequired = true;
            }

            if (csentry.ToUserRelations(user, config))
            {
                updateRequired = true;
            }

            if (csentry.ToUserWebsites(user, config))
            {
                updateRequired = true;
            }

            if (csentry.ToUserNotes(user, config))
            {
                updateRequired = true;
            }

            return updateRequired;
        }

        public static void MakeAdmin(this CSEntryChange csentry, CSEntryChange delta, User user, IManagementAgentParameters config)
        {
            AttributeChange attributeChange = csentry.AttributeChanges.FirstOrDefault((t => t.Name == "isAdmin"));

            if (attributeChange == null)
            {
                return;
            }

            bool makeAdmin = attributeChange.GetValueAdd<bool>();

            if (attributeChange.ModificationType == AttributeModificationType.Add)
            {
                if (makeAdmin)
                {
                    UserRequestFactory.MakeAdmin(true, user.Id);
                    delta.CreateAttributeAdd("isAdmin", true);
                }
            }
            else if (attributeChange.ModificationType == AttributeModificationType.Replace ||
                     attributeChange.ModificationType == AttributeModificationType.Update)
            {
                UserRequestFactory.MakeAdmin(makeAdmin, user.Id);
                AttributeChange existingChange = delta.AttributeChanges.FirstOrDefault((t => t.Name == "isAdmin"));

                if (existingChange != null)
                {
                    delta.AttributeChanges.Remove("isAdmin");
                }

                delta.CreateAttributeReplace("isAdmin", makeAdmin);
            }
        }

        private static bool ToUserNotes(this CSEntryChange csentry, User user, IManagementAgentParameters config)
        {
            bool updateRequired = false;
            if (!csentry.AttributeChanges.Any(t => t.Name.StartsWith("notes_")))
            {
                return false;
            }


            if (user.Notes == null)
            {
                user.Notes = new Notes();
            }

            csentry.UpdateTargetFromCSEntryChange(user.Notes, x => x.ContentType, "notes_contentType", ref updateRequired);
            csentry.UpdateTargetFromCSEntryChange(user.Notes, x => x.Value, "notes_value", ref updateRequired);

            if (user.Notes.IsEmpty())
            {
                user.Notes = null;
            }

            return updateRequired;
        }

        private static bool ToUserExternalIDs(this CSEntryChange csentry, User user, IManagementAgentParameters config)
        {
            bool updateRequired = false;

            if (config.ExternalIDsAttributeFormat == GoogleArrayMode.Json)
            {
                csentry.UpdateTargetFromCSEntryChange<string, User, List<ExternalID>>(user, x => x.ExternalIds, (t) => CSEntryChangeToUser.serializer.Deserialize<List<ExternalID>>(t), "externalIds", ref updateRequired);
                return updateRequired;
            }


            foreach (string itemType in config.ExternalIDsAttributeFixedTypes)
            {
                string attribute = "externalIds_" + itemType;
                AttributeChange change = csentry.AttributeChanges.FirstOrDefault(t => t.Name == attribute);

                if (change == null)
                {
                    continue;
                }

                if (user.ExternalIds == null)
                {
                    user.ExternalIds = new List<ExternalID>();
                }

                ExternalID existing = user.ExternalIds.GetObjectOfTypeOrDefault(itemType);

                switch (change.ModificationType)
                {
                    case AttributeModificationType.Delete:
                        if (existing != null)
                        {
                            Logger.WriteLine("Removing {0}: {1}", attribute, existing.Value);
                            user.ExternalIds.Remove(existing);
                            updateRequired = true;
                        }
                        break;

                    case AttributeModificationType.Add:
                    case AttributeModificationType.Update:
                    case AttributeModificationType.Replace:
                        string existingValue = null;

                        if (existing == null)
                        {
                            existing = new ExternalID();
                            user.ExternalIds.Add(existing);
                        }
                        else
                        {
                            existingValue = existing.Value;
                        }

                        existing.Type = itemType;
                        existing.Value = change.GetValueAdd<string>();
                        updateRequired = true;
                        user.ExternalIds.AddOrRemoveIfEmpty(existing);
                        if (existingValue == null)
                        {
                            Logger.WriteLine("Adding {0}: {1}", attribute, existing.Value);
                        }
                        else
                        {
                            Logger.WriteLine("Replacing {0}: {1}", attribute, existing.Value);
                        }
                        break;

                    case AttributeModificationType.Unconfigured:
                    default:
                        throw new InvalidOperationException("The modification type is unknown or not supported");
                }
            }

            if (user.ExternalIds.RemoveEmptyItems())
            {
                updateRequired = true;
            }

            return updateRequired;
        }

        private static bool ToUserPhones(this CSEntryChange csentry, User user, IManagementAgentParameters config)
        {
            bool updateRequired = false;

            if (config.PhonesAttributeFormat == GoogleArrayMode.Json)
            {
                csentry.UpdateTargetFromCSEntryChange<string, User, List<Phone>>(user, x => x.Phones, (t) => CSEntryChangeToUser.serializer.Deserialize<List<Phone>>(t), "phones", ref updateRequired);
                return updateRequired;
            }

            if (user.Phones == null)
            {
                user.Phones = new List<Phone>();
            }

            Phone primary = user.Phones.GetOrCreatePrimary();
            csentry.UpdateTargetFromCSEntryChange(primary, x => x.Value, "phones_primary", ref updateRequired);
            csentry.UpdateTargetFromCSEntryChange(primary, x => x.Type, "phones_primary_type", ref updateRequired);
            user.Phones.AddOrRemoveIfEmpty(primary);

            foreach (string itemType in config.PhonesAttributeFixedTypes)
            {
                string attribute = "phones_" + itemType;
                AttributeChange change = csentry.AttributeChanges.FirstOrDefault(t => t.Name == attribute);

                if (change == null)
                {
                    continue;
                }

                Phone existing = user.Phones.GetObjectOfTypeOrDefault(itemType, false);

                switch (change.ModificationType)
                {
                    case AttributeModificationType.Delete:
                        if (existing != null)
                        {
                            user.Phones.Remove(existing);
                            updateRequired = true;
                            Logger.WriteLine("Removing {0}: {1}", attribute, existing.Value);
                        }
                        break;

                    case AttributeModificationType.Add:
                    case AttributeModificationType.Update:
                    case AttributeModificationType.Replace:
                        string existingValue = null;

                        if (existing == null)
                        {
                            existing = new Phone();
                            user.Phones.Add(existing);
                        }
                        else
                        {
                            existingValue = existing.Value;
                        }

                        existing.Type = itemType;
                        existing.Value = change.GetValueAdd<string>();
                        updateRequired = true;
                        user.Phones.AddOrRemoveIfEmpty(existing);

                        if (existingValue == null)
                        {
                            Logger.WriteLine("Adding {0}: {1}", attribute, existing.Value);
                        }
                        else
                        {
                            Logger.WriteLine("Replacing {0}: {1}", attribute, existing.Value);
                        }
                        break;

                    case AttributeModificationType.Unconfigured:
                    default:
                        throw new InvalidOperationException("The modification type is unknown or not supported");
                }
            }

            if (user.Phones.RemoveEmptyItems())
            {
                updateRequired = true;
            }

            return updateRequired;
        }

        private static bool ToUserIMs(this CSEntryChange csentry, User user, IManagementAgentParameters config)
        {
            bool updateRequired = false;

            if (config.IMsAttributeFormat == GoogleArrayMode.Json)
            {
                csentry.UpdateTargetFromCSEntryChange<string, User, List<IM>>(user, x => x.Ims, (t) => CSEntryChangeToUser.serializer.Deserialize<List<IM>>(t), "ims", ref updateRequired);
                return updateRequired;
            }

            if (user.Ims == null)
            {
                user.Ims = new List<IM>();
            }

            IM primary = user.Ims.GetOrCreatePrimary();
            csentry.UpdateTargetFromCSEntryChange(primary, x => x.IMAddress, "ims_primary_im", ref updateRequired);
            csentry.UpdateTargetFromCSEntryChange(primary, x => x.Type, "ims_primary_type", ref updateRequired);
            csentry.UpdateTargetFromCSEntryChange(primary, x => x.Protocol, "ims_primary_protocol", ref updateRequired);
            user.Ims.AddOrRemoveIfEmpty(primary);

            foreach (string itemType in config.IMsAttributeFixedTypes)
            {
                IList<AttributeChange> typeChanges = csentry.AttributeChanges.Where(t => t.Name.StartsWith($"ims_{itemType}")).ToList();

                if (typeChanges.Count == 0)
                {
                    continue;
                }

                IM existing = user.Ims.GetOrCreateObjectOfType(itemType, false);
                foreach (AttributeChange typeChange in typeChanges)
                {
                    if (existing.ApplyAttributeChange(typeChange))
                    {
                        updateRequired = true;
                    }
                }

                user.Ims.AddOrRemoveIfEmpty(existing);
            }

            if (user.Ims.RemoveEmptyItems())
            {
                updateRequired = true;
            }

            return updateRequired;
        }

        private static bool ToUserOrganizations(this CSEntryChange csentry, User user, IManagementAgentParameters config)
        {
            bool updateRequired = false;

            if (config.OrganizationsAttributeFormat == GoogleArrayMode.Json)
            {
                csentry.UpdateTargetFromCSEntryChange<string, User, List<Organization>>(user, x => x.Organizations, (t) => CSEntryChangeToUser.serializer.Deserialize<List<Organization>>(t), "organizations", ref updateRequired);
                return updateRequired;
            }

            if (user.Organizations == null)
            {
                user.Organizations = new List<Organization>();
            }

            Organization primary = user.Organizations.GetOrCreatePrimary();
            csentry.UpdateTargetFromCSEntryChange(primary, x => x.CostCenter, "organizations_primary_costCenter", ref updateRequired);
            csentry.UpdateTargetFromCSEntryChange(primary, x => x.Department, "organizations_primary_department", ref updateRequired);
            csentry.UpdateTargetFromCSEntryChange(primary, x => x.Description, "organizations_primary_description", ref updateRequired);
            csentry.UpdateTargetFromCSEntryChange(primary, x => x.Domain, "organizations_primary_domain", ref updateRequired);
            csentry.UpdateTargetFromCSEntryChange(primary, x => x.Location, "organizations_primary_location", ref updateRequired);
            csentry.UpdateTargetFromCSEntryChange(primary, x => x.Name, "organizations_primary_name", ref updateRequired);
            csentry.UpdateTargetFromCSEntryChange(primary, x => x.Symbol, "organizations_primary_symbol", ref updateRequired);
            csentry.UpdateTargetFromCSEntryChange(primary, x => x.Title, "organizations_primary_title", ref updateRequired);
            csentry.UpdateTargetFromCSEntryChange(primary, x => x.Type, "organizations_primary_type", ref updateRequired);
            user.Organizations.AddOrRemoveIfEmpty(primary);

            foreach (string itemType in config.IMsAttributeFixedTypes)
            {
                IList<AttributeChange> typeChanges = csentry.AttributeChanges.Where(t => t.Name.StartsWith($"organizations_{itemType}")).ToList();

                if (typeChanges.Count == 0)
                {
                    continue;
                }

                Organization existing = user.Organizations.GetOrCreateObjectOfType(itemType, false);
                foreach (AttributeChange typeChange in typeChanges)
                {
                    if (existing.ApplyAttributeChange(typeChange))
                    {
                        updateRequired = true;
                    }
                }

                user.Organizations.AddOrRemoveIfEmpty(existing);
            }

            if (user.Organizations.RemoveEmptyItems())
            {
                updateRequired = true;
            }

            return updateRequired;
        }

        private static bool ToUserAddresses(this CSEntryChange csentry, User user, IManagementAgentParameters config)
        {
            bool updateRequired = false;

            if (config.AddressesAttributeFormat == GoogleArrayMode.Json)
            {
                csentry.UpdateTargetFromCSEntryChange<string, User, List<Address>>(user, x => x.Addresses, (t) => CSEntryChangeToUser.serializer.Deserialize<List<Address>>(t), "addresses", ref updateRequired);
                return updateRequired;
            }

            if (user.Addresses == null)
            {
                user.Addresses = new List<Address>();
            }

            Address primary = user.Addresses.GetOrCreatePrimary();
            csentry.UpdateTargetFromCSEntryChange(primary, x => x.Country, "addresses_primary_country", ref updateRequired);
            csentry.UpdateTargetFromCSEntryChange(primary, x => x.CountryCode, "addresses_primary_countryCode", ref updateRequired);
            csentry.UpdateTargetFromCSEntryChange(primary, x => x.ExtendedAddress, "addresses_primary_extendedAddress", ref updateRequired);
            csentry.UpdateTargetFromCSEntryChange(primary, x => x.Formatted, "addresses_primary_formatted", ref updateRequired);
            csentry.UpdateTargetFromCSEntryChange(primary, x => x.Locality, "addresses_primary_locality", ref updateRequired);
            csentry.UpdateTargetFromCSEntryChange(primary, x => x.POBox, "addresses_primary_poBox", ref updateRequired);
            csentry.UpdateTargetFromCSEntryChange(primary, x => x.PostalCode, "addresses_primary_postalCode", ref updateRequired);
            csentry.UpdateTargetFromCSEntryChange(primary, x => x.Region, "addresses_primary_region", ref updateRequired);
            csentry.UpdateTargetFromCSEntryChange(primary, x => x.SourceIsStructured, "addresses_primary_sourceIsStructured", ref updateRequired);
            csentry.UpdateTargetFromCSEntryChange(primary, x => x.StreetAddress, "addresses_primary_streetAddress", ref updateRequired);
            csentry.UpdateTargetFromCSEntryChange(primary, x => x.Type, "addresses_primary_type", ref updateRequired);
            user.Addresses.AddOrRemoveIfEmpty(primary);

            foreach (string itemType in config.AddressesAttributeFixedTypes)
            {
                IList<AttributeChange> typeChanges = csentry.AttributeChanges.Where(t => t.Name.StartsWith($"addresses_{itemType}")).ToList();

                if (typeChanges.Count == 0)
                {
                    continue;
                }

                Address existing = user.Addresses.GetOrCreateObjectOfType(itemType, false);
                foreach (AttributeChange typeChange in typeChanges)
                {
                    if (existing.ApplyAttributeChange(typeChange))
                    {
                        updateRequired = true;
                    }
                }

                user.Addresses.AddOrRemoveIfEmpty(existing);
            }

            if (user.Addresses.RemoveEmptyItems())
            {
                updateRequired = true;
            }

            return updateRequired;
        }

        private static bool ToUserRelations(this CSEntryChange csentry, User user, IManagementAgentParameters config)
        {
            bool updateRequired = false;

            if (config.RelationsAttributeFormat == GoogleArrayMode.Json)
            {
                csentry.UpdateTargetFromCSEntryChange<string, User, List<Relation>>(user, x => x.Relations, (t) => CSEntryChangeToUser.serializer.Deserialize<List<Relation>>(t), "relations", ref updateRequired);
                return updateRequired;
            }

            if (user.Relations == null)
            {
                user.Relations = new List<Relation>();
            }

            foreach (string itemType in config.RelationsAttributeFixedTypes)
            {
                string attribute = "relations_" + itemType;
                AttributeChange change = csentry.AttributeChanges.FirstOrDefault(t => t.Name == attribute);

                if (change == null)
                {
                    continue;
                }

                Relation existing = user.Relations.GetObjectOfTypeOrDefault(itemType);

                switch (change.ModificationType)
                {
                    case AttributeModificationType.Delete:
                        if (existing != null)
                        {
                            user.Relations.Remove(existing);
                            updateRequired = true;
                            Logger.WriteLine("Removing {0}: {1}", attribute, existing.Value);
                        }
                        break;

                    case AttributeModificationType.Add:
                    case AttributeModificationType.Update:
                    case AttributeModificationType.Replace:
                        string existingValue = null;

                        if (existing == null)
                        {
                            existing = new Relation();
                            user.Relations.Add(existing);
                        }
                        else
                        {
                            existingValue = existing.Value;

                        }

                        existing.Type = itemType;
                        existing.Value = change.GetValueAdd<string>();
                        updateRequired = true;
                        user.Relations.AddOrRemoveIfEmpty(existing);

                        if (existingValue == null)
                        {
                            Logger.WriteLine("Adding {0}: {1}", attribute, existing.Value);
                        }
                        else
                        {
                            Logger.WriteLine("Replacing {0}: {1}", attribute, existing.Value);
                        }
                        break;

                    case AttributeModificationType.Unconfigured:
                    default:
                        throw new InvalidOperationException("The modification type is unknown or not supported");
                }
            }

            if (user.Relations.RemoveEmptyItems())
            {
                updateRequired = true;
            }

            return updateRequired;
        }

        private static bool ToUserWebsites(this CSEntryChange csentry, User user, IManagementAgentParameters config)
        {
            bool updateRequired = false;

            if (config.WebsitesAttributeFormat == GoogleArrayMode.Json)
            {
                csentry.UpdateTargetFromCSEntryChange<string, User, List<Website>>(user, x => x.Websites, (t) => CSEntryChangeToUser.serializer.Deserialize<List<Website>>(t), "websites", ref updateRequired);
                return updateRequired;
            }

            if (user.Websites == null)
            {
                user.Websites = new List<Website>();
            }

            Website primary = user.Websites.GetOrCreatePrimary();
            csentry.UpdateTargetFromCSEntryChange(primary, x => x.Value, "websites_primary", ref updateRequired);
            csentry.UpdateTargetFromCSEntryChange(primary, x => x.Type, "websites_primary_type", ref updateRequired);
            user.Websites.AddOrRemoveIfEmpty(primary);

            foreach (string itemType in config.WebsitesAttributeFixedTypes)
            {
                string attribute = "websites_" + itemType;
                AttributeChange change = csentry.AttributeChanges.FirstOrDefault(t => t.Name == attribute);

                if (change == null)
                {
                    continue;
                }

                Website existing = user.Websites.GetObjectOfTypeOrDefault(itemType, false);

                switch (change.ModificationType)
                {
                    case AttributeModificationType.Delete:
                        if (existing != null)
                        {
                            user.Websites.Remove(existing);
                            updateRequired = true;
                        }

                        Logger.WriteLine("Removing {0}: {1}", attribute, existing.Value);
                        break;

                    case AttributeModificationType.Add:
                    case AttributeModificationType.Update:
                    case AttributeModificationType.Replace:
                        string existingValue = null;

                        if (existing == null)
                        {
                            existing = new Website();
                            user.Websites.Add(existing);
                        }
                        else
                        {
                            existingValue = existing.Value;

                        }

                        existing.Type = itemType;
                        existing.Value = change.GetValueAdd<string>();
                        updateRequired = true;
                        user.Websites.AddOrRemoveIfEmpty(existing);

                        if (existingValue == null)
                        {
                            Logger.WriteLine("Adding {0}: {1}", attribute, existing.Value);
                        }
                        else
                        {
                            Logger.WriteLine("Replacing {0}: {1}", attribute, existing.Value);
                        }
                        break;

                    case AttributeModificationType.Unconfigured:
                    default:
                        throw new InvalidOperationException("The modification type is unknown or not supported");
                }
            }

            if (user.Websites.RemoveEmptyItems())
            {
                updateRequired = true;
            }

            return updateRequired;
        }

        private static bool ApplyAttributeChange(this IM im, AttributeChange change)
        {
            bool updated = false;

            if (change.Name.EndsWith("_protocol"))
            {
                if (im.Protocol != null)
                {
                    im.Protocol = change.GetStringValueAddOrNullPlaceholder();
                    updated = true;
                }
            }
            else if (change.Name.EndsWith("_im"))
            {
                if (im.IMAddress != null)
                {
                    im.IMAddress = change.GetStringValueAddOrNullPlaceholder();
                    updated = true;
                }
            }

            return updated;
        }

        private static bool ApplyAttributeChange(this Organization item, AttributeChange change)
        {
            bool updated = false;

            if (change.Name.EndsWith("_costCenter"))
            {
                if (item.CostCenter != null)
                {
                    item.CostCenter = change.GetStringValueAddOrNullPlaceholder();
                    updated = true;
                }
            }
            else if (change.Name.EndsWith("_department"))
            {
                if (item.Department != null)
                {
                    item.Department = change.GetStringValueAddOrNullPlaceholder();
                    updated = true;
                }
            }
            else if (change.Name.EndsWith("_description"))
            {
                if (item.Description != null)
                {
                    item.Description = change.GetStringValueAddOrNullPlaceholder();
                    updated = true;
                }
            }
            else if (change.Name.EndsWith("_domain"))
            {
                if (item.Domain != null)
                {
                    item.Domain = change.GetStringValueAddOrNullPlaceholder();
                    updated = true;
                }
            }
            else if (change.Name.EndsWith("_location"))
            {
                if (item.Location != null)
                {
                    item.Location = change.GetStringValueAddOrNullPlaceholder();
                    updated = true;
                }
            }
            else if (change.Name.EndsWith("_name"))
            {
                if (item.Name != null)
                {
                    item.Name = change.GetStringValueAddOrNullPlaceholder();
                    updated = true;
                }
            }
            else if (change.Name.EndsWith("_symbol"))
            {
                if (item.Symbol != null)
                {
                    item.Symbol = change.GetStringValueAddOrNullPlaceholder();
                    updated = true;
                }
            }
            else if (change.Name.EndsWith("_title"))
            {
                if (item.Title != null)
                {
                    item.Title = change.GetStringValueAddOrNullPlaceholder();
                    updated = true;
                }
            }

            return updated;
        }

        private static bool ApplyAttributeChange(this Address item, AttributeChange change)
        {
            bool updated = false;

            if (change.Name.EndsWith("_country"))
            {
                if (item.Country != null)
                {
                    item.Country = change.GetStringValueAddOrNullPlaceholder();
                    updated = true;
                }
            }
            else if (change.Name.EndsWith("_countryCode"))
            {
                if (item.CountryCode != null)
                {
                    item.CountryCode = change.GetStringValueAddOrNullPlaceholder();
                    updated = true;
                }
            }
            else if (change.Name.EndsWith("_extendedAddress"))
            {
                if (item.ExtendedAddress != null)
                {
                    item.ExtendedAddress = change.GetStringValueAddOrNullPlaceholder();
                    updated = true;
                }
            }
            else if (change.Name.EndsWith("_formatted"))
            {
                if (item.Formatted != null)
                {
                    item.Formatted = change.GetStringValueAddOrNullPlaceholder();
                    updated = true;
                }
            }
            else if (change.Name.EndsWith("_locality"))
            {
                if (item.Locality != null)
                {
                    item.Locality = change.GetStringValueAddOrNullPlaceholder();
                    updated = true;
                }
            }
            else if (change.Name.EndsWith("_poBox"))
            {
                if (item.POBox != null)
                {
                    item.POBox = change.GetStringValueAddOrNullPlaceholder();
                    updated = true;
                }
            }
            else if (change.Name.EndsWith("_postalCode"))
            {
                if (item.PostalCode != null)
                {
                    item.PostalCode = change.GetStringValueAddOrNullPlaceholder();
                    updated = true;
                }
            }
            else if (change.Name.EndsWith("_region"))
            {
                if (item.Region != null)
                {
                    item.Region = change.GetStringValueAddOrNullPlaceholder();
                    updated = true;
                }
            }
            else if (change.Name.EndsWith("_sourceIsStructured"))
            {
                if (item.SourceIsStructured != null)
                {
                    item.SourceIsStructured = change.GetValueAdd<bool?>();
                    updated = true;
                }
            }
            else if (change.Name.EndsWith("_streetAddress"))
            {
                if (item.StreetAddress != null)
                {
                    item.StreetAddress = change.GetStringValueAddOrNullPlaceholder();
                    updated = true;
                }
            }

            return updated;
        }

        private static void GetUserAliasChanges(CSEntryChange csentry, User user, out IList<string> aliasAdds, out IList<string> aliasDeletes)
        {
            aliasAdds = new List<string>();
            aliasDeletes = new List<string>();
            AttributeChange change = csentry.AttributeChanges.FirstOrDefault(t => t.Name == "aliases");

            if (csentry.ObjectModificationType == ObjectModificationType.Replace)
            {
                if (change != null)
                {
                    aliasAdds = change.GetValueAdds<string>();
                }

                foreach (string alias in UserRequestFactory.GetAliases(csentry.DN).Except(aliasAdds))
                {
                    aliasDeletes.Add(alias);
                }
            }
            else
            {
                if (change == null)
                {
                    return;
                }

                switch (change.ModificationType)
                {
                    case AttributeModificationType.Add:
                        aliasAdds = change.GetValueAdds<string>();
                        break;

                    case AttributeModificationType.Delete:
                        foreach (string alias in user.Aliases)
                        {
                            aliasDeletes.Add(alias);
                        }
                        break;

                    case AttributeModificationType.Replace:
                        aliasAdds = change.GetValueAdds<string>();
                        foreach (string alias in user.Aliases.Except(aliasAdds))
                        {
                            aliasDeletes.Add(alias);
                        }
                        break;

                    case AttributeModificationType.Update:
                        aliasAdds = change.GetValueAdds<string>();
                        aliasDeletes = change.GetValueDeletes<string>();
                        break;

                    case AttributeModificationType.Unconfigured:
                    default:
                        throw new InvalidOperationException("Unknown or unsupported modification type");
                }
            }
        }

        public static bool ApplyUserAliasChanges(CSEntryChange csentry, User user, CSEntryChange deltacsentry)
        {
            IList<string> aliasAdds;
            IList<string> aliasDeletes;

            CSEntryChangeToUser.GetUserAliasChanges(csentry, user, out aliasAdds, out aliasDeletes);

            if (aliasAdds.Count == 0 && aliasDeletes.Count == 0)
            {
                return false;
            }

            AttributeChange existingChange = deltacsentry.AttributeChanges.FirstOrDefault(t => t.Name == "aliases");
            IList<ValueChange> valueChanges;

            if (existingChange == null)
            {
                valueChanges = new List<ValueChange>();
            }
            else
            {
                valueChanges = existingChange.ValueChanges;
            }

            try
            {
                if (aliasDeletes != null)
                {
                    foreach (string alias in aliasDeletes)
                    {
                        UserRequestFactory.RemoveAlias(csentry.DN, alias);
                        valueChanges.Add(ValueChange.CreateValueDelete(alias));
                    }
                }

                foreach (string alias in aliasAdds)
                {
                    UserRequestFactory.AddAlias(csentry.DN, alias);
                    valueChanges.Add(ValueChange.CreateValueAdd(alias));
                }
            }
            finally
            {
                if (valueChanges.Count > 0)
                {
                    if (deltacsentry.ObjectModificationType == ObjectModificationType.Update)
                    {
                        deltacsentry.AttributeChanges.Add(AttributeChange.CreateAttributeUpdate("aliases", valueChanges));
                    }
                    else
                    {
                        deltacsentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("aliases", valueChanges.Where(u => u.ModificationType == ValueModificationType.Add).Select(t => t.Value).ToList()));
                    }
                }
            }

            return true;
        }
    }
}

