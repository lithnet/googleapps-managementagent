using System;
using System.Collections.Generic;
using System.Linq;
using Google.Apis.Admin.Directory.directory_v1.Data;
using Lithnet.Logging;
using Lithnet.GoogleApps.ManagedObjects;
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
            List<AttributeChange> changes = new List<AttributeChange>();

            ApiInterfaceGroupMembership.GetMemberChangesFromCSEntryChange(csentry, out membershipToAdd, out membershipToDelete, csentry.ObjectModificationType == ObjectModificationType.Replace);

            HashSet<string> allMembersToDelete = membershipToDelete.GetAllMembers();
            List<Member> allMembersToAdd = membershipToAdd.ToMemberList();

            AttributeModificationType modificationType = csentry.ObjectModificationType == ObjectModificationType.Update ? AttributeModificationType.Update : AttributeModificationType.Add;

            if (csentry.ObjectModificationType != ObjectModificationType.Add && allMembersToDelete.Count > 0)
            {
                try
                {
                    GroupMemberRequestFactory.RemoveMembers(csentry.DN, allMembersToDelete.ToList(), false);

                    foreach (string member in allMembersToDelete)
                    {
                        Logger.WriteLine($"Deleted member {member}", LogLevel.Debug);
                    }

                    if (allMembersToDelete.Count == 1)
                    {
                        Logger.WriteLine($"Deleted {allMembersToDelete.Count} member");
                    }
                    else
                    {
                        Logger.WriteLine($"Deleted {allMembersToDelete.Count} members");
                    }

                    ApiInterfaceGroupMembership.AddAttributeChange("member", modificationType, membershipToDelete.Members.ToValueChange(ValueModificationType.Delete), changes);
                    ApiInterfaceGroupMembership.AddAttributeChange("externalMember", modificationType, membershipToDelete.ExternalMembers.ToValueChange(ValueModificationType.Delete), changes);
                    ApiInterfaceGroupMembership.AddAttributeChange("manager", modificationType, membershipToDelete.Managers.ToValueChange(ValueModificationType.Delete), changes);
                    ApiInterfaceGroupMembership.AddAttributeChange("externalManager", modificationType, membershipToDelete.ExternalManagers.ToValueChange(ValueModificationType.Delete), changes);
                    ApiInterfaceGroupMembership.AddAttributeChange("owner", modificationType, membershipToDelete.Owners.ToValueChange(ValueModificationType.Delete), changes);
                    ApiInterfaceGroupMembership.AddAttributeChange("externalOwner", modificationType, membershipToDelete.ExternalOwners.ToValueChange(ValueModificationType.Delete), changes);
                }
                catch (AggregateGroupUpdateException ex)
                {
                    Logger.WriteLine("The following member removals failed");
                    foreach (Exception e in ex.Exceptions)
                    {
                        Logger.WriteException(e);
                    }

                    ApiInterfaceGroupMembership.AddAttributeChange("member", modificationType, membershipToDelete.Members.Except(ex.FailedMembers).ToValueChange(ValueModificationType.Delete), changes);
                    ApiInterfaceGroupMembership.AddAttributeChange("externalMember", modificationType, membershipToDelete.ExternalMembers.Except(ex.FailedMembers).ToValueChange(ValueModificationType.Delete), changes);
                    ApiInterfaceGroupMembership.AddAttributeChange("manager", modificationType, membershipToDelete.Managers.Except(ex.FailedMembers).ToValueChange(ValueModificationType.Delete), changes);
                    ApiInterfaceGroupMembership.AddAttributeChange("externalManager", modificationType, membershipToDelete.ExternalManagers.Except(ex.FailedMembers).ToValueChange(ValueModificationType.Delete), changes);
                    ApiInterfaceGroupMembership.AddAttributeChange("owner", modificationType, membershipToDelete.Owners.Except(ex.FailedMembers).ToValueChange(ValueModificationType.Delete), changes);
                    ApiInterfaceGroupMembership.AddAttributeChange("externalOwner", modificationType, membershipToDelete.ExternalOwners.Except(ex.FailedMembers).ToValueChange(ValueModificationType.Delete), changes);
                    throw;
                }
            }

            if (allMembersToAdd.Count > 0)
            {
                try
                {
                    GroupMemberRequestFactory.AddMembers(csentry.DN, this.NormalizeMembershipList(config, allMembersToAdd), true);

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

                    ApiInterfaceGroupMembership.AddAttributeChange("member", modificationType, membershipToAdd.Members.ToValueChange(ValueModificationType.Add), changes);
                    ApiInterfaceGroupMembership.AddAttributeChange("externalMember", modificationType, membershipToAdd.ExternalMembers.ToValueChange(ValueModificationType.Add), changes);
                    ApiInterfaceGroupMembership.AddAttributeChange("manager", modificationType, membershipToAdd.Managers.ToValueChange(ValueModificationType.Add), changes);
                    ApiInterfaceGroupMembership.AddAttributeChange("externalManager", modificationType, membershipToAdd.ExternalManagers.ToValueChange(ValueModificationType.Add), changes);
                    ApiInterfaceGroupMembership.AddAttributeChange("owner", modificationType, membershipToAdd.Owners.ToValueChange(ValueModificationType.Add), changes);
                    ApiInterfaceGroupMembership.AddAttributeChange("externalOwner", modificationType, membershipToAdd.ExternalOwners.ToValueChange(ValueModificationType.Add), changes);
                }
                catch (AggregateGroupUpdateException ex)
                {
                    Logger.WriteLine("The following member additions failed");
                    foreach (Exception e in ex.Exceptions)
                    {
                        Logger.WriteException(e);
                    }

                    ApiInterfaceGroupMembership.AddAttributeChange("member", modificationType, membershipToAdd.Members.Except(ex.FailedMembers).ToValueChange(ValueModificationType.Add), changes);
                    ApiInterfaceGroupMembership.AddAttributeChange("externalMember", modificationType, membershipToAdd.ExternalMembers.Except(ex.FailedMembers).ToValueChange(ValueModificationType.Add), changes);
                    ApiInterfaceGroupMembership.AddAttributeChange("manager", modificationType, membershipToAdd.Managers.Except(ex.FailedMembers).ToValueChange(ValueModificationType.Add), changes);
                    ApiInterfaceGroupMembership.AddAttributeChange("externalManager", modificationType, membershipToAdd.ExternalManagers.Except(ex.FailedMembers).ToValueChange(ValueModificationType.Add), changes);
                    ApiInterfaceGroupMembership.AddAttributeChange("owner", modificationType, membershipToAdd.Owners.Except(ex.FailedMembers).ToValueChange(ValueModificationType.Add), changes);
                    ApiInterfaceGroupMembership.AddAttributeChange("externalOwner", modificationType, membershipToAdd.ExternalOwners.Except(ex.FailedMembers).ToValueChange(ValueModificationType.Add), changes);
                    throw;
                }
            }

            return changes;
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

        public IList<AttributeChange> GetChanges(string dn, ObjectModificationType modType, SchemaType type, object source)
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

            foreach (IAttributeAdapter typeDef in ManagementAgent.Schema[SchemaConstants.Group].Attributes.Where(t => t.Api == this.Api))
            {
                if (type.HasAttribute(typeDef.AttributeName))
                {
                    attributeChanges.AddRange(typeDef.CreateAttributeChanges(dn, modType, membership));
                }
            }

            return attributeChanges;
        }

        private static void GetMemberChangesFromCSEntryChange(CSEntryChange csentry, out GroupMembership adds, out GroupMembership deletes, bool replacing)
        {
            adds = new GroupMembership();
            deletes = new GroupMembership();
            GroupMembership existingGroupMembership;

            if (ApiInterfaceGroupMembership.ExistingMembershipRequiredForUpdate(csentry) | replacing)
            {
                existingGroupMembership = GroupMemberRequestFactory.GetMembership(csentry.DN);
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
                            adds.Add(address);
                        }
                        break;

                    case AttributeModificationType.Delete:
                        foreach (string member in existingMembers)
                        {
                            deletes.Add(member);
                        }

                        break;

                    case AttributeModificationType.Replace:
                        IList<string> newMembers = csentry.GetValueAdds<string>(attributeName);
                        foreach (string address in newMembers)
                        {
                            adds.Add(address);
                        }

                        foreach (string member in existingMembers.Except(newMembers))
                        {
                            deletes.Add(member);
                        }

                        break;

                    case AttributeModificationType.Update:
                        foreach (string address in csentry.GetValueDeletes<string>(attributeName))
                        {
                            deletes.Add(address);
                        }

                        foreach (string address in csentry.GetValueAdds<string>(attributeName))
                        {
                            adds.Add(address);
                        }

                        break;

                    case AttributeModificationType.Unconfigured:
                    default:
                        throw new NotSupportedException("The modification type was unknown or unsupported");
                }
            }
        }

        private static bool ExistingMembershipRequiredForUpdate(CSEntryChange csentry)
        {
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
