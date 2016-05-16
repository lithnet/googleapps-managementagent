using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.MetadirectoryServices;
using Lithnet.Logging;
using G=Google.Apis.Admin.Directory.directory_v1.Data;
using Lithnet.GoogleApps.ManagedObjects;
using Lithnet.MetadirectoryServices;

namespace Lithnet.GoogleApps.MA
{
    internal static class CSEntryChangeToGroup
    {
        //public static bool CSEntryChangeToGroupCore(CSEntryChange csentry, G.Group group)
        //{
        //    group.Email = csentry.DN;
        //    bool updateRequired = false;
        //    // Group core

        //    csentry.UpdateTargetFromCSEntryChange(group, x => x.Description, "description", ref updateRequired);
        //    csentry.UpdateTargetFromCSEntryChange(group, x => x.Name, "name", ref updateRequired);
        //    csentry.UpdateTargetFromCSEntryChange(group, x => x.Email, "primaryEmail", ref updateRequired);

        //    return updateRequired;
        //}

        //private static void CSEntryChangeToGroupAliases(CSEntryChange csentry, G.Group group, out IList<string> aliasAdds, out IList<string> aliasDeletes)
        //{
        //    aliasAdds = new List<string>();
        //    aliasDeletes = new List<string>();
        //    AttributeChange change = csentry.AttributeChanges.FirstOrDefault(t => t.Name == "aliases");

        //    if (csentry.ObjectModificationType == ObjectModificationType.Replace)
        //    {
        //        if (change != null)
        //        {
        //            aliasAdds = change.GetValueAdds<string>();
        //        }

        //        foreach (string alias in GroupRequestFactory.GetAliases(csentry.DN).Except(aliasAdds))
        //        {
        //            aliasDeletes.Add(alias);
        //        }
        //    }
        //    else
        //    {
        //        if (change == null)
        //        {
        //            return;
        //        }

        //        switch (change.ModificationType)
        //        {
        //            case AttributeModificationType.Add:
        //                aliasAdds = change.GetValueAdds<string>();
        //                break;

        //            case AttributeModificationType.Delete:
        //                foreach (string alias in group.Aliases)
        //                {
        //                    aliasDeletes.Add(alias);
        //                }
        //                break;

        //            case AttributeModificationType.Replace:
        //                aliasAdds = change.GetValueAdds<string>();
        //                foreach (string alias in group.Aliases.Except(aliasAdds))
        //                {
        //                    aliasDeletes.Add(alias);
        //                }
        //                break;

        //            case AttributeModificationType.Update:
        //                aliasAdds = change.GetValueAdds<string>();
        //                aliasDeletes = change.GetValueDeletes<string>();
        //                break;

        //            case AttributeModificationType.Unconfigured:
        //            default:
        //                throw new InvalidOperationException("Unknown or unsupported modification type");
        //        }
        //    }
        //}

        //public static bool CSEntryChangeToGroupSettings(CSEntryChange csentry, GroupSettings settings)
        //{
        //    bool updateRequired = false;

        //    csentry.UpdateTargetFromCSEntryChange(settings, x => x.MaxMessageBytes, "maxMessageBytes", ref updateRequired);
        //    csentry.UpdateTargetFromCSEntryChange(settings, x => x.MessageDisplayFont, "messageDisplayFont", ref updateRequired);
        //    csentry.UpdateTargetFromCSEntryChange(settings, x => x.MessageModerationLevel, "messageModerationLevel", ref updateRequired);
        //    csentry.UpdateTargetFromCSEntryChange(settings, x => x.PrimaryLanguage, "primaryLanguage", ref updateRequired);
        //    csentry.UpdateTargetFromCSEntryChange(settings, x => x.ReplyTo, "replyTo", ref updateRequired);
        //    csentry.UpdateTargetFromCSEntryChange(settings, x => x.SpamModerationLevel, "spamModerationLevel", ref updateRequired);
        //    csentry.UpdateTargetFromCSEntryChange(settings, x => x.WhoCanContactOwner, "whoCanContactOwner", ref updateRequired);
        //    csentry.UpdateTargetFromCSEntryChange(settings, x => x.WhoCanInvite, "whoCanInvite", ref updateRequired);
        //    csentry.UpdateTargetFromCSEntryChange(settings, x => x.WhoCanJoin, "whoCanJoin", ref updateRequired);
        //    csentry.UpdateTargetFromCSEntryChange(settings, x => x.WhoCanLeaveGroup, "whoCanLeaveGroup", ref updateRequired);
        //    csentry.UpdateTargetFromCSEntryChange(settings, x => x.WhoCanViewGroup, "whoCanViewGroup", ref updateRequired);
        //    csentry.UpdateTargetFromCSEntryChange(settings, x => x.WhoCanViewMembership, "whoCanViewMembership", ref updateRequired);
        //    csentry.UpdateTargetFromCSEntryChange(settings, x => x.CustomReplyTo, "customReplyTo", ref updateRequired);
        //    csentry.UpdateTargetFromCSEntryChange(settings, x => x.DefaultMessageDenyNotificationText, "defaultMessageDenyNotificationText", ref updateRequired);
        //    csentry.UpdateTargetFromCSEntryChange(settings, x => x.IncludeInGlobalAddressList, "includeInGlobalAddressList", ref updateRequired);
        //    csentry.UpdateTargetFromCSEntryChange(settings, x => x.AllowExternalMembers, "allowExternalMembers", ref updateRequired);
        //    csentry.UpdateTargetFromCSEntryChange(settings, x => x.AllowGoogleCommunication, "allowGoogleCommunication", ref updateRequired);
        //    csentry.UpdateTargetFromCSEntryChange(settings, x => x.AllowWebPosting, "allowWebPosting", ref updateRequired);
        //    csentry.UpdateTargetFromCSEntryChange(settings, x => x.IsArchived, "isArchived", ref updateRequired);
        //    csentry.UpdateTargetFromCSEntryChange(settings, x => x.MembersCanPostAsTheGroup, "membersCanPostAsTheGroup", ref updateRequired);
        //    csentry.UpdateTargetFromCSEntryChange(settings, x => x.SendMessageDenyNotification, "sendMessageDenyNotification", ref updateRequired);
        //    csentry.UpdateTargetFromCSEntryChange(settings, x => x.ShowInGroupDirectory, "showInGroupDirectory", ref updateRequired);

        //    if (csentry.HasAttributeChange("whoCanPostMessage") || csentry.HasAttributeChange("archiveOnly"))
        //    {
        //        settings.WhoCanPostMessage = csentry.GetValueAdd<string>("whoCanPostMessage");
        //        settings.ArchiveOnly = csentry.GetValueAdd<bool>("archiveOnly");

        //        if (settings.WhoCanPostMessage != null)
        //        {
        //            updateRequired = true;
        //            Logger.WriteLine("Updating {0} -> {1}", "whoCanPostMessage", settings.WhoCanPostMessage);

        //            if (settings.WhoCanPostMessage == "NONE_CAN_POST")
        //            {
        //                if (settings.ArchiveOnly != true)
        //                {
        //                    settings.ArchiveOnly = true;
        //                    updateRequired = true;
        //                    Logger.WriteLine("Updating archiveOnly to true as whoCanPostMessage was set to NONE_CAN_POST");
        //                }
        //            }
        //        }
        //        else
        //        {
        //            if (settings.ArchiveOnly == true && settings.WhoCanPostMessage != "NONE_CAN_POST")
        //            {
        //                settings.WhoCanPostMessage = "NONE_CAN_POST";
        //                updateRequired = true;
        //                Logger.WriteLine("Updating whoCanPostMessage to NONE_CAN_POST as archiveOnly was true");
        //            }
        //            else if (settings.WhoCanPostMessage == "NONE_CAN_POST")
        //            {
        //                settings.ArchiveOnly = true;
        //                updateRequired = true;
        //                Logger.WriteLine("Updating archiveOnly to true as whoCanPostMessage was set to NONE_CAN_POST");
        //            }
        //        }
        //    }

        //    return updateRequired;
        //}

        //public static bool ApplyAliasChanges(CSEntryChange csentry, CSEntryChange deltacsentry, G.Group group)
        //{
        //    IList<string> aliasAdds;
        //    IList<string> aliasDeletes;

        //    CSEntryChangeToGroup.CSEntryChangeToGroupAliases(csentry, group, out aliasAdds, out aliasDeletes);

        //    if (aliasAdds.Count == 0 && aliasDeletes.Count == 0)
        //    {
        //        return false;
        //    }

        //    AttributeChange existingChange = deltacsentry.AttributeChanges.FirstOrDefault(t => t.Name == "aliases");
        //    IList<ValueChange> valueChanges;

        //    if (existingChange == null)
        //    {
        //        valueChanges = new List<ValueChange>();
        //    }
        //    else
        //    {
        //        valueChanges = existingChange.ValueChanges;
        //    }

        //    try
        //    {
        //        if (aliasDeletes != null)
        //        {
        //            foreach (string alias in aliasDeletes)
        //            {
        //                GroupRequestFactory.RemoveAlias(csentry.DN, alias);
        //                valueChanges.Add(ValueChange.CreateValueDelete(alias));
        //            }
        //        }

        //        if (aliasAdds != null)
        //        {
        //            foreach (string alias in aliasAdds)
        //            {
        //                GroupRequestFactory.AddAlias(csentry.DN, alias);
        //                valueChanges.Add(ValueChange.CreateValueAdd(alias));
        //            }
        //        }
        //    }
        //    finally
        //    {
        //        if (valueChanges.Count > 0)
        //        {
        //            if (deltacsentry.ObjectModificationType == ObjectModificationType.Update)
        //            {
        //                deltacsentry.AttributeChanges.Add(AttributeChange.CreateAttributeUpdate("aliases", valueChanges));
        //            }
        //            else
        //            {
        //                deltacsentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("aliases", valueChanges));
        //            }
        //        }
        //    }

        //    return true;
        //}

        //public static void ApplyMembershipChanges(SchemaType type, CSEntryChange csentry, CSEntryChange deltaCSEntry)
        //{
        //    GroupMembership membershipToDelete;
        //    GroupMembership membershipToAdd;

        //    CSEntryChangeToGroup.GetMemberChangesFromCSEntryChange(csentry, out membershipToAdd, out membershipToDelete, csentry.ObjectModificationType == ObjectModificationType.Replace);

        //    HashSet<string> allMembersToDelete = membershipToDelete.GetAllMembers();
        //    List<G.Member> allMembersToAdd = membershipToAdd.ToMemberList();

        //    AttributeModificationType modificationType = csentry.ObjectModificationType == ObjectModificationType.Update ? AttributeModificationType.Update : AttributeModificationType.Add;

        //    if (csentry.ObjectModificationType != ObjectModificationType.Add && allMembersToDelete.Count > 0)
        //    {
        //        try
        //        {
        //            GroupMemberRequestFactory.RemoveMembers(csentry.DN, allMembersToDelete.ToList(), false);

        //            foreach (string member in allMembersToDelete)
        //            {
        //                Logger.WriteLine($"Deleted member {member}", LogLevel.Debug);
        //            }
                    
        //            deltaCSEntry.CreateAttributeChangeIfInSchema(type, "member", modificationType, membershipToDelete.Members.ToValueChange(ValueModificationType.Delete));
        //            deltaCSEntry.CreateAttributeChangeIfInSchema(type, "externalMember", modificationType, membershipToDelete.ExternalMembers.ToValueChange(ValueModificationType.Delete));
        //            deltaCSEntry.CreateAttributeChangeIfInSchema(type, "manager", modificationType, membershipToDelete.Managers.ToValueChange(ValueModificationType.Delete));
        //            deltaCSEntry.CreateAttributeChangeIfInSchema(type, "externalManager", modificationType, membershipToDelete.ExternalManagers.ToValueChange(ValueModificationType.Delete));
        //            deltaCSEntry.CreateAttributeChangeIfInSchema(type, "owner", modificationType, membershipToDelete.Owners.ToValueChange(ValueModificationType.Delete));
        //            deltaCSEntry.CreateAttributeChangeIfInSchema(type, "externalOwner", modificationType, membershipToDelete.ExternalOwners.ToValueChange(ValueModificationType.Delete));
        //        }
        //        catch (AggregateGroupUpdateException ex)
        //        {
        //            deltaCSEntry.CreateAttributeChangeIfInSchema(type, "member", modificationType, membershipToDelete.Members.Except(ex.FailedMembers).ToValueChange(ValueModificationType.Delete));
        //            deltaCSEntry.CreateAttributeChangeIfInSchema(type, "externalMember", modificationType, membershipToDelete.ExternalMembers.Except(ex.FailedMembers).ToValueChange(ValueModificationType.Delete));
        //            deltaCSEntry.CreateAttributeChangeIfInSchema(type, "manager", modificationType, membershipToDelete.Managers.Except(ex.FailedMembers).ToValueChange(ValueModificationType.Delete));
        //            deltaCSEntry.CreateAttributeChangeIfInSchema(type, "externalManager", modificationType, membershipToDelete.ExternalManagers.Except(ex.FailedMembers).ToValueChange(ValueModificationType.Delete));
        //            deltaCSEntry.CreateAttributeChangeIfInSchema(type, "owner", modificationType, membershipToDelete.Owners.Except(ex.FailedMembers).ToValueChange(ValueModificationType.Delete));
        //            deltaCSEntry.CreateAttributeChangeIfInSchema(type, "externalOwner", modificationType, membershipToDelete.ExternalOwners.Except(ex.FailedMembers).ToValueChange(ValueModificationType.Delete));
        //            throw;
        //        }
        //    }

        //    if (allMembersToAdd.Count > 0)
        //    {
        //        try
        //        {
        //            GroupMemberRequestFactory.AddMembers(csentry.DN, allMembersToAdd, true);

        //            foreach (string member in allMembersToDelete)
        //            {
        //                Logger.WriteLine($"Added member {member}", LogLevel.Debug);
        //            }

        //            deltaCSEntry.CreateAttributeChangeIfInSchema(type,"member", modificationType, membershipToAdd.Members.ToValueChange(ValueModificationType.Add));
        //            deltaCSEntry.CreateAttributeChangeIfInSchema(type, "externalMember", modificationType, membershipToAdd.ExternalMembers.ToValueChange(ValueModificationType.Add));
        //            deltaCSEntry.CreateAttributeChangeIfInSchema(type, "manager", modificationType, membershipToAdd.Managers.ToValueChange(ValueModificationType.Add));
        //            deltaCSEntry.CreateAttributeChangeIfInSchema(type, "externalManager", modificationType, membershipToAdd.ExternalManagers.ToValueChange(ValueModificationType.Add));
        //            deltaCSEntry.CreateAttributeChangeIfInSchema(type, "owner", modificationType, membershipToAdd.Owners.ToValueChange(ValueModificationType.Add));
        //            deltaCSEntry.CreateAttributeChangeIfInSchema(type, "externalOwner", modificationType, membershipToAdd.ExternalOwners.ToValueChange(ValueModificationType.Add));
        //        }
        //        catch (AggregateGroupUpdateException ex)
        //        {
        //            deltaCSEntry.CreateAttributeChangeIfInSchema(type, "member", modificationType, membershipToAdd.Members.Except(ex.FailedMembers).ToValueChange(ValueModificationType.Add));
        //            deltaCSEntry.CreateAttributeChangeIfInSchema(type, "externalMember", modificationType, membershipToAdd.ExternalMembers.Except(ex.FailedMembers).ToValueChange(ValueModificationType.Add));
        //            deltaCSEntry.CreateAttributeChangeIfInSchema(type, "manager", modificationType, membershipToAdd.Managers.Except(ex.FailedMembers).ToValueChange(ValueModificationType.Add));
        //            deltaCSEntry.CreateAttributeChangeIfInSchema(type, "externalManager", modificationType, membershipToAdd.ExternalManagers.Except(ex.FailedMembers).ToValueChange(ValueModificationType.Add));
        //            deltaCSEntry.CreateAttributeChangeIfInSchema(type, "owner", modificationType, membershipToAdd.Owners.Except(ex.FailedMembers).ToValueChange(ValueModificationType.Add));
        //            deltaCSEntry.CreateAttributeChangeIfInSchema(type, "externalOwner", modificationType, membershipToAdd.ExternalOwners.Except(ex.FailedMembers).ToValueChange(ValueModificationType.Add));
        //            throw;
        //        }
        //    }
        //}

        //private static void GetMemberChangesFromCSEntryChange(CSEntryChange csentry, out GroupMembership adds, out GroupMembership deletes, bool replacing)
        //{
        //    adds = new GroupMembership();
        //    deletes = new GroupMembership();
        //    GroupMembership existingGroupMembership = null;

        //    if (CSEntryChangeToGroup.ExistingMembershipRequiredForUpdate(csentry) | replacing)
        //    {
        //        existingGroupMembership = GroupMemberRequestFactory.GetMembership(csentry.DN);
        //    }
        //    else
        //    {
        //        existingGroupMembership = new GroupMembership();
        //    }

        //    CSEntryChangeToGroup.GetMemberChangesFromCSEntryChange(csentry, adds.Members, deletes.Members, existingGroupMembership.Members, "member", replacing);
        //    CSEntryChangeToGroup.GetMemberChangesFromCSEntryChange(csentry, adds.ExternalMembers, deletes.ExternalMembers, existingGroupMembership.ExternalMembers, "externalMember", replacing);
        //    CSEntryChangeToGroup.GetMemberChangesFromCSEntryChange(csentry, adds.Managers, deletes.Managers, existingGroupMembership.Managers, "manager", replacing);
        //    CSEntryChangeToGroup.GetMemberChangesFromCSEntryChange(csentry, adds.ExternalManagers, deletes.ExternalManagers, existingGroupMembership.ExternalManagers, "externalManager", replacing);
        //    CSEntryChangeToGroup.GetMemberChangesFromCSEntryChange(csentry, adds.Owners, deletes.Owners, existingGroupMembership.Owners, "owner", replacing);
        //    CSEntryChangeToGroup.GetMemberChangesFromCSEntryChange(csentry, adds.ExternalOwners, deletes.ExternalOwners, existingGroupMembership.ExternalOwners, "externalOwner", replacing);
        //}

        //private static void GetMemberChangesFromCSEntryChange(CSEntryChange csentry, HashSet<string> adds, HashSet<string> deletes, HashSet<string> existingMembers, string attributeName, bool replacing)
        //{
        //    if (replacing)
        //    {
        //        foreach (string address in csentry.GetValueAdds<string>(attributeName))
        //        {
        //            adds.Add(address);
        //        }

        //        foreach (string address in deletes.Except(adds))
        //        {
        //            deletes.Add(address);
        //        }

        //        return;
        //    }
        //    else
        //    {
        //        AttributeChange change = csentry.AttributeChanges.FirstOrDefault(t => t.Name == attributeName);

        //        if (change == null)
        //        {
        //            return;
        //        }

        //        switch (change.ModificationType)
        //        {
        //            case AttributeModificationType.Add:
        //                foreach (string address in csentry.GetValueAdds<string>(attributeName))
        //                {
        //                    adds.Add(address);
        //                }
        //                break;

        //            case AttributeModificationType.Delete:
        //                foreach (string member in existingMembers)
        //                {
        //                    deletes.Add(member);
        //                }

        //                break;

        //            case AttributeModificationType.Replace:
        //                IList<string> newMembers = csentry.GetValueAdds<string>(attributeName);
        //                foreach (string address in newMembers)
        //                {
        //                    adds.Add(address);
        //                }

        //                foreach (string member in existingMembers.Except(newMembers))
        //                {
        //                    deletes.Add(member);
        //                }

        //                break;

        //            case AttributeModificationType.Update:
        //                foreach (string address in csentry.GetValueDeletes<string>(attributeName))
        //                {
        //                    deletes.Add(address);
        //                }

        //                foreach (string address in csentry.GetValueAdds<string>(attributeName))
        //                {
        //                    adds.Add(address);
        //                }

        //                break;

        //            case AttributeModificationType.Unconfigured:
        //            default:
        //                throw new NotSupportedException("The modification type was unknown or unsupported");
        //        }
        //    }
        //}

        //private static bool ExistingMembershipRequiredForUpdate(CSEntryChange csentry)
        //{
        //    return (
        //        csentry.AttributeChanges.Any(t => t.Name == "member" && (t.ModificationType == AttributeModificationType.Delete || t.ModificationType == AttributeModificationType.Replace)) ||
        //        csentry.AttributeChanges.Any(t => t.Name == "externalMember" && (t.ModificationType == AttributeModificationType.Delete || t.ModificationType == AttributeModificationType.Replace)) ||
        //        csentry.AttributeChanges.Any(t => t.Name == "manager" && (t.ModificationType == AttributeModificationType.Delete || t.ModificationType == AttributeModificationType.Replace)) ||
        //        csentry.AttributeChanges.Any(t => t.Name == "externalManager" && (t.ModificationType == AttributeModificationType.Delete || t.ModificationType == AttributeModificationType.Replace)) ||
        //        csentry.AttributeChanges.Any(t => t.Name == "owner" && (t.ModificationType == AttributeModificationType.Delete || t.ModificationType == AttributeModificationType.Replace)) ||
        //        csentry.AttributeChanges.Any(t => t.Name == "externalOwner" && (t.ModificationType == AttributeModificationType.Delete || t.ModificationType == AttributeModificationType.Replace)));
        //}
    }
}
