using System;
using System.Collections.Generic;
using System.Linq;
using Google;
using Google.Apis.Admin.Directory.directory_v1.Data;
using Lithnet.GoogleApps.ManagedObjects;
using Lithnet.Logging;
using Lithnet.MetadirectoryServices;
using Microsoft.MetadirectoryServices;

namespace Lithnet.GoogleApps.MA
{
    internal class ApiInterfaceGroupMembership : IApiInterface
    {
        public string Api => "groupmembership";

        public IList<AttributeChange> ApplyChanges(CSEntryChange csentry, SchemaType type, IManagementAgentParameters config, ref object target, bool patch = false)
        {
            GroupMembership membershipToDelete;
            GroupMembership membershipToAdd;
            IList<Member> roleChanges;
            GroupMembership reportedAdds;
            GroupMembership reportedDeletes;

            List<AttributeChange> changes = new List<AttributeChange>();

            ApiInterfaceGroupMembership.GetMemberChangesFromCSEntryChange(csentry, out membershipToAdd, out membershipToDelete, out reportedAdds, out reportedDeletes, out roleChanges, csentry.ObjectModificationType == ObjectModificationType.Replace, config);

            HashSet<string> allMembersToDelete = membershipToDelete.GetAllMembers();
            List<Member> allMembersToAdd = membershipToAdd.ToMemberList();

            AttributeModificationType modificationType = csentry.ObjectModificationType == ObjectModificationType.Update ? AttributeModificationType.Update : AttributeModificationType.Add;
            try
            {
                if (csentry.ObjectModificationType != ObjectModificationType.Add && allMembersToDelete.Count > 0)
                {
                    try
                    {
                        GroupMemberRequestFactory.RemoveMembers(csentry.DN, allMembersToDelete.ToList(), false);

                        foreach (string member in allMembersToDelete)
                        {
                            Logger.WriteLine($"Deleted member {member} from group {csentry.DN}", LogLevel.Debug);
                        }

                        if (allMembersToDelete.Count == 1)
                        {
                            Logger.WriteLine($"Deleted {allMembersToDelete.Count} member from group {csentry.DN}");
                        }
                        else
                        {
                            Logger.WriteLine($"Deleted {allMembersToDelete.Count} members from group {csentry.DN}");
                        }
                    }
                    catch (AggregateGroupUpdateException ex)
                    {
                        Logger.WriteLine("The following member removals failed");
                        foreach (Exception e in ex.Exceptions)
                        {
                            Logger.WriteException(e);
                        }

                        reportedDeletes.RemoveMembers(ex.FailedMembers);
                        throw;
                    }
                }

                if (csentry.ObjectModificationType != ObjectModificationType.Add && roleChanges?.Count > 0)
                {
                    try
                    {
                        foreach (Member change in roleChanges)
                        {
                            try
                            {
                                GroupMemberRequestFactory.ChangeMemberRole(csentry.DN, change);
                                Logger.WriteLine($"Changed member role {change.Email} to {change.Role}", LogLevel.Debug);
                            }
                            catch (GoogleApiException ex)
                            {
                                if (ex.HttpStatusCode == System.Net.HttpStatusCode.NotFound)
                                {
                                    throw new UnexpectedDataException($"Member {change.Email} cannot be assigned {change.Role} without being made a member", ex);
                                }

                                throw;
                            }
                        }
                        
                    }
                    catch (AggregateGroupUpdateException ex)
                    {
                        Logger.WriteLine("The following member role changes failed");

                        foreach (Exception e in ex.Exceptions)
                        {
                            Logger.WriteException(e);
                        }

                        throw;
                    }
                }

                if (allMembersToAdd.Count > 0)
                {
                    try
                    {
                        GroupMemberRequestFactory.AddMembers(csentry.DN, this.NormalizeMembershipList(config, allMembersToAdd), false);

                        foreach (Member member in allMembersToAdd)
                        {
                            Logger.WriteLine($"Added {member.Role} {member.Email}", LogLevel.Debug);
                        }

                        if (allMembersToAdd.Count == 1)
                        {
                            Logger.WriteLine($"Added {allMembersToAdd.Count} member");
                        }
                        else
                        {
                            Logger.WriteLine($"Added {allMembersToAdd.Count} members");
                        }
                    }
                    catch (AggregateGroupUpdateException ex)
                    {
                        Logger.WriteLine("The following member additions failed");
                        foreach (Exception e in ex.Exceptions)
                        {
                            Logger.WriteException(e);
                        }

                        reportedAdds.RemoveMembers(ex.FailedMembers);
                        throw;
                    }
                }
            }
            finally
            {

                ApiInterfaceGroupMembership.AddAttributeChange("member", modificationType, reportedDeletes.Members.ToValueChange(ValueModificationType.Delete), changes);
                ApiInterfaceGroupMembership.AddAttributeChange("externalMember", modificationType, reportedDeletes.ExternalMembers.ToValueChange(ValueModificationType.Delete), changes);
                ApiInterfaceGroupMembership.AddAttributeChange("manager", modificationType, reportedDeletes.Managers.ToValueChange(ValueModificationType.Delete), changes);
                ApiInterfaceGroupMembership.AddAttributeChange("externalManager", modificationType, reportedDeletes.ExternalManagers.ToValueChange(ValueModificationType.Delete), changes);
                ApiInterfaceGroupMembership.AddAttributeChange("owner", modificationType, reportedDeletes.Owners.ToValueChange(ValueModificationType.Delete), changes);
                ApiInterfaceGroupMembership.AddAttributeChange("externalOwner", modificationType, reportedDeletes.ExternalOwners.ToValueChange(ValueModificationType.Delete), changes);

                ApiInterfaceGroupMembership.AddAttributeChange("member", modificationType, reportedAdds.Members.ToValueChange(ValueModificationType.Add), changes);
                ApiInterfaceGroupMembership.AddAttributeChange("externalMember", modificationType, reportedAdds.ExternalMembers.ToValueChange(ValueModificationType.Add), changes);
                ApiInterfaceGroupMembership.AddAttributeChange("manager", modificationType, reportedAdds.Managers.ToValueChange(ValueModificationType.Add), changes);
                ApiInterfaceGroupMembership.AddAttributeChange("externalManager", modificationType, reportedAdds.ExternalManagers.ToValueChange(ValueModificationType.Add), changes);
                ApiInterfaceGroupMembership.AddAttributeChange("owner", modificationType, reportedAdds.Owners.ToValueChange(ValueModificationType.Add), changes);
                ApiInterfaceGroupMembership.AddAttributeChange("externalOwner", modificationType, reportedAdds.ExternalOwners.ToValueChange(ValueModificationType.Add), changes);
            }

            return changes;
        }

        public IList<AttributeChange> GetChanges(string dn, ObjectModificationType modType, SchemaType type, object source, IManagementAgentParameters config)
        {
            List<AttributeChange> attributeChanges = new List<AttributeChange>();

            GroupMembership membership = source as GroupMembership;

            if (membership == null)
            {
                GoogleGroup group = source as GoogleGroup;

                if (group == null)
                {
                    throw new InvalidOperationException();
                }
                else
                {
                    membership = group.Membership;
                }
            }

            this.ApplyRoleInheritance(config, membership);

            foreach (IAttributeAdapter typeDef in ManagementAgent.Schema[SchemaConstants.Group].AttributeAdapters.Where(t => t.Api == this.Api))
            {
                foreach (string attributeName in typeDef.MmsAttributeNames)
                {
                    if (type.HasAttribute(attributeName))
                    {
                        attributeChanges.AddRange(typeDef.CreateAttributeChanges(dn, modType, membership));
                    }
                }
            }

            return attributeChanges;
        }
        
        private List<Member> NormalizeMembershipList(IManagementAgentParameters config, List<Member> members)
        {
            if (!config.InheritGroupRoles)
            {
                return members;
            }

            Dictionary<string, Member> normalizedList = new Dictionary<string, Member>(StringComparer.CurrentCultureIgnoreCase);

            foreach (Member member in members)
            {
                if (normalizedList.ContainsKey(member.Email))
                {
                    Member existingMember = normalizedList[member.Email];

                    if (existingMember.Role == "OWNER")
                    {
                        continue;
                    }

                    if (existingMember.Role == "MANAGER")
                    {
                        if (member.Role == "OWNER")
                        {
                            normalizedList[member.Email] = member;
                        }
                        else
                        {
                            continue;
                        }
                    }
                    else
                    {
                        normalizedList[member.Email] = member;
                    }
                }
                else
                {
                    normalizedList.Add(member.Email, member);
                }
            }

            return normalizedList.Select(t => t.Value).ToList();
        }

        private static void AddAttributeChange(string attributeName, AttributeModificationType modificationType, IList<ValueChange> changes, IList<AttributeChange> attributeChanges)
        {
            AttributeChange existingChange = attributeChanges.FirstOrDefault(t => t.Name == attributeName);

            if (modificationType == AttributeModificationType.Delete)
            {
                if (existingChange != null)
                {
                    attributeChanges.Remove(existingChange);
                }

                attributeChanges.Add(AttributeChange.CreateAttributeDelete(attributeName));
                return;
            }

            if (changes == null || changes.Count == 0)
            {
                return;
            }

            IList<object> adds;
            switch (modificationType)
            {
                case AttributeModificationType.Add:
                    if (existingChange != null)
                    {
                        foreach (ValueChange valueChange in changes.Where(t => t.ModificationType == ValueModificationType.Add))
                        {
                            existingChange.ValueChanges.Add(valueChange);
                        }
                    }
                    else
                    {
                        adds = changes.Where(t => t.ModificationType == ValueModificationType.Add).Select(t => t.Value).ToList();

                        if (adds.Count > 0)
                        {
                            attributeChanges.Add(AttributeChange.CreateAttributeAdd(attributeName, adds));
                        }
                    }
                    break;

                case AttributeModificationType.Replace:
                    if (existingChange != null)
                    {
                        attributeChanges.Remove(existingChange);
                    }

                    adds = changes.Where(t => t.ModificationType == ValueModificationType.Add).Select(t => t.Value).ToList();
                    if (adds.Count > 0)
                    {
                        attributeChanges.Add(AttributeChange.CreateAttributeReplace(attributeName, adds));
                    }

                    break;

                case AttributeModificationType.Update:
                    if (existingChange != null)
                    {
                        if (existingChange.ModificationType != AttributeModificationType.Update)
                        {
                            throw new InvalidOperationException();
                        }

                        foreach (ValueChange valueChange in changes)
                        {
                            existingChange.ValueChanges.Add(valueChange);
                        }
                    }
                    else
                    {
                        if (changes.Count > 0)
                        {
                            attributeChanges.Add(AttributeChange.CreateAttributeUpdate(attributeName, changes));
                        }
                    }

                    break;

                case AttributeModificationType.Delete:
                case AttributeModificationType.Unconfigured:
                default:
                    throw new InvalidOperationException();
            }
        }
        
        private void ApplyRoleInheritance(IManagementAgentParameters config, GroupMembership membership)
        {
            if (config.InheritGroupRoles && membership != null)
            {
                foreach (string value in membership.Owners)
                {
                    membership.Managers.Add(value);
                }

                foreach (string value in membership.ExternalOwners)
                {
                    membership.ExternalManagers.Add(value);
                }

                foreach (string value in membership.Managers)
                {
                    membership.Members.Add(value);
                }

                foreach (string value in membership.ExternalManagers)
                {
                    membership.ExternalMembers.Add(value);
                }
            }
        }

        private static void GetMemberChangesFromCSEntryChange(CSEntryChange csentry, out GroupMembership adds, out GroupMembership deletes, out GroupMembership reportedAdds, out GroupMembership reportedDeletes, out IList<Member> roleChanges, bool replacing, IManagementAgentParameters config)
        {
            adds = new GroupMembership();
            deletes = new GroupMembership();
            reportedAdds = new GroupMembership();
            reportedDeletes = new GroupMembership();
            roleChanges = new List<Member>();

            GroupMembership existingGroupMembership;
            bool hasExistingMembership = false;

            if (replacing | ApiInterfaceGroupMembership.ExistingMembershipRequiredForUpdate(csentry, config))
            {
                existingGroupMembership = GroupMemberRequestFactory.GetMembership(csentry.DN);
                hasExistingMembership = true;
            }
            else
            {
                existingGroupMembership = new GroupMembership();
            }

            ApiInterfaceGroupMembership.GetMemberChangesFromCSEntryChange(csentry, adds.Members, deletes.Members, existingGroupMembership.Members, "member", replacing);
            ApiInterfaceGroupMembership.GetMemberChangesFromCSEntryChange(csentry, adds.ExternalMembers, deletes.ExternalMembers, existingGroupMembership.ExternalMembers, "externalMember", replacing);
            ApiInterfaceGroupMembership.GetMemberChangesFromCSEntryChange(csentry, adds.Managers, deletes.Managers, existingGroupMembership.Managers, "manager", replacing);
            ApiInterfaceGroupMembership.GetMemberChangesFromCSEntryChange(csentry, adds.ExternalManagers, deletes.ExternalManagers, existingGroupMembership.ExternalManagers, "externalManager", replacing);
            ApiInterfaceGroupMembership.GetMemberChangesFromCSEntryChange(csentry, adds.Owners, deletes.Owners, existingGroupMembership.Owners, "owner", replacing);
            ApiInterfaceGroupMembership.GetMemberChangesFromCSEntryChange(csentry, adds.ExternalOwners, deletes.ExternalOwners, existingGroupMembership.ExternalOwners, "externalOwner", replacing);

            if (hasExistingMembership && config.InheritGroupRoles)
            {
                ApiInterfaceGroupMembership.ApplyRoleChanges(adds, deletes, out reportedAdds, out reportedDeletes, out roleChanges);
            }

            reportedAdds.MergeMembership(adds);
            reportedDeletes.MergeMembership(deletes);
        }

        private static void ApplyRoleChanges(GroupMembership adds, GroupMembership deletes, out GroupMembership reportedAdds, out GroupMembership reportedDeletes, out IList<Member> roleChanges)
        {
            roleChanges = new List<Member>();
            reportedAdds = new GroupMembership();
            reportedDeletes = new GroupMembership();

            // Downgrades to member
            foreach (var member in deletes.Managers.Except(deletes.Members).ToList())
            {
                roleChanges.Add(new Member() { Email = member, Role = "MEMBER" });
                deletes.Managers.Remove(member);
                reportedDeletes.Managers.Add(member);
            }

            foreach (var member in deletes.ExternalManagers.Except(deletes.ExternalMembers).ToList())
            {
                roleChanges.Add(new Member() { Email = member, Role = "MEMBER" });
                deletes.ExternalManagers.Remove(member);
                reportedDeletes.ExternalManagers.Add(member);
            }

            // Downgrades to managers
            foreach (var member in deletes.Owners.Except(deletes.Managers).ToList())
            {
                roleChanges.Add(new Member() { Email = member, Role = "MANAGER" });
                deletes.Owners.Remove(member);
                reportedDeletes.Owners.Add(member);
            }

            foreach (var member in deletes.ExternalOwners.Except(deletes.ExternalManagers).ToList())
            {
                roleChanges.Add(new Member() { Email = member, Role = "MANAGER" });
                deletes.ExternalOwners.Remove(member);
                reportedDeletes.ExternalOwners.Add(member);
            }
            
            // Upgrades to manager
            foreach (var member in adds.Managers.Except(adds.Members).ToList())
            {
                roleChanges.Add(new Member() { Email = member, Role = "MANAGER" });
                adds.Managers.Remove(member);
                reportedAdds.Managers.Add(member);
            }

            foreach (var member in adds.ExternalManagers.Except(adds.ExternalMembers).ToList())
            {
                roleChanges.Add(new Member() { Email = member, Role = "MANAGER" });
                adds.ExternalManagers.Remove(member);
                reportedAdds.ExternalManagers.Add(member);
            }

            // Upgrades to owner
            foreach (var member in adds.Owners.Except(adds.Managers).ToList())
            {
                roleChanges.Add(new Member() { Email = member, Role = "OWNER" });
                adds.Owners.Remove(member);
                reportedAdds.Owners.Add(member);
            }

            foreach (var member in adds.ExternalOwners.Except(adds.ExternalManagers).ToList())
            {
                roleChanges.Add(new Member() { Email = member, Role = "OWNER" });
                adds.ExternalOwners.Remove(member);
                reportedAdds.ExternalOwners.Add(member);
            }
        }

        private static void GetMemberChangesFromCSEntryChange(CSEntryChange csentry, HashSet<string> adds, HashSet<string> deletes, HashSet<string> existingMembers, string attributeName, bool replacing)
        {
            if (replacing)
            {
                foreach (string address in csentry.GetValueAdds<string>(attributeName))
                {
                    adds.Add(address);
                }

                foreach (string address in deletes.Except(adds))
                {
                    deletes.Add(address);
                }

                return;
            }
            else
            {
                AttributeChange change = csentry.AttributeChanges.FirstOrDefault(t => t.Name == attributeName);

                if (change == null)
                {
                    return;
                }

                switch (change.ModificationType)
                {
                    case AttributeModificationType.Add:
                        foreach (string address in csentry.GetValueAdds<string>(attributeName))
                        {
                            ApiInterfaceGroupMembership.ValidateAddress(address, attributeName);
                            adds.Add(address);
                        }
                        break;

                    case AttributeModificationType.Delete:
                        foreach (string member in existingMembers)
                        {
                            ApiInterfaceGroupMembership.ValidateAddress(member, attributeName);
                            deletes.Add(member);
                        }

                        break;

                    case AttributeModificationType.Replace:
                        IList<string> newMembers = csentry.GetValueAdds<string>(attributeName);
                        foreach (string address in newMembers)
                        {
                            ApiInterfaceGroupMembership.ValidateAddress(address, attributeName);
                            adds.Add(address);
                        }

                        foreach (string member in existingMembers.Except(newMembers))
                        {
                            ApiInterfaceGroupMembership.ValidateAddress(member, attributeName);
                            deletes.Add(member);
                        }

                        break;

                    case AttributeModificationType.Update:
                        foreach (string address in csentry.GetValueDeletes<string>(attributeName))
                        {
                            ApiInterfaceGroupMembership.ValidateAddress(address, attributeName);
                            deletes.Add(address);
                        }

                        foreach (string address in csentry.GetValueAdds<string>(attributeName))
                        {
                            ApiInterfaceGroupMembership.ValidateAddress(address, attributeName);
                            adds.Add(address);
                        }

                        break;

                    case AttributeModificationType.Unconfigured:
                    default:
                        throw new NotSupportedException("The modification type was unknown or unsupported");
                }
            }
        }

        private static void ValidateAddress(string address, string attributeName)
        {
            if (attributeName.StartsWith("external"))
            {
                if (GroupMembership.IsAddressInternal(address))
                {
                    throw new UnexpectedDataException($"The value {address} cannot be exported as {attributeName} as it is a known internal domain. It should be exported as {attributeName.Replace("external", string.Empty)}");
                }
            }
            else
            {
                if (GroupMembership.IsAddressExternal(address))
                {
                    throw new UnexpectedDataException($"The value {address} cannot be exported as {attributeName} as it is not a known internal domain. It should be exported as external{attributeName}");
                }
            }
        }

        private static bool ExistingMembershipRequiredForUpdate(CSEntryChange csentry, IManagementAgentParameters config)
        {
            if (csentry.ObjectModificationType == ObjectModificationType.Add)
            {
                return false;
            }

            if (config.InheritGroupRoles)
            {
                return (
               csentry.AttributeChanges.Any(t => (t.ModificationType == AttributeModificationType.Delete || t.ModificationType == AttributeModificationType.Replace) && t.Name == "member") ||
               csentry.AttributeChanges.Any(t => (t.ModificationType == AttributeModificationType.Delete || t.ModificationType == AttributeModificationType.Replace) && t.Name == "externalMember") ||
               csentry.AttributeChanges.Any(t => t.Name == "manager") ||
               csentry.AttributeChanges.Any(t => t.Name == "externalManager") ||
               csentry.AttributeChanges.Any(t => t.Name == "owner") ||
               csentry.AttributeChanges.Any(t => t.Name == "externalOwner"));
            }

            return (
                csentry.AttributeChanges.Any(t => (t.ModificationType == AttributeModificationType.Delete || t.ModificationType == AttributeModificationType.Replace) && t.Name == "member") ||
                csentry.AttributeChanges.Any(t => (t.ModificationType == AttributeModificationType.Delete || t.ModificationType == AttributeModificationType.Replace) && t.Name == "externalMember") ||
                csentry.AttributeChanges.Any(t => (t.ModificationType == AttributeModificationType.Delete || t.ModificationType == AttributeModificationType.Replace) && t.Name == "manager") ||
                csentry.AttributeChanges.Any(t => (t.ModificationType == AttributeModificationType.Delete || t.ModificationType == AttributeModificationType.Replace) && t.Name == "externalManager") ||
                csentry.AttributeChanges.Any(t => (t.ModificationType == AttributeModificationType.Delete || t.ModificationType == AttributeModificationType.Replace) && t.Name == "owner") ||
                csentry.AttributeChanges.Any(t => (t.ModificationType == AttributeModificationType.Delete || t.ModificationType == AttributeModificationType.Replace) && t.Name == "externalOwner"));
        }
    }
}
