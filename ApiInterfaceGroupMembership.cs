using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lithnet.GoogleApps.MA
{
    using Google.Apis.Admin.Directory.directory_v1.Data;
    using Logging;
    using ManagedObjects;
    using MetadirectoryServices;
    using Microsoft.MetadirectoryServices;
    using User = ManagedObjects.User;

    public class ApiInterfaceGroupMembership : ApiInterface
    {
        private static MASchemaType maType = SchemaBuilder.GetUserSchema();

        public ApiInterfaceGroupMembership()
        {
            this.Api = "groupmembership";
        }

        public override bool IsPrimary => false;

        public override object CreateInstance(CSEntryChange csentry)
        {
            throw new NotSupportedException();
        }

        public override object GetInstance(CSEntryChange csentry)
        {
            throw new NotSupportedException();
        }

        public override void DeleteInstance(CSEntryChange csentry)
        {
            throw new NotSupportedException();
        }

        public override IList<AttributeChange> ApplyChanges(CSEntryChange csentry, SchemaType type, object target, bool patch = false)
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

                    ApiInterfaceGroupMembership.AddAttributeChange("member", modificationType, membershipToDelete.Members.ToValueChange(ValueModificationType.Delete), changes);
                    ApiInterfaceGroupMembership.AddAttributeChange("externalMember", modificationType, membershipToDelete.ExternalMembers.ToValueChange(ValueModificationType.Delete), changes);
                    ApiInterfaceGroupMembership.AddAttributeChange("manager", modificationType, membershipToDelete.Managers.ToValueChange(ValueModificationType.Delete), changes);
                    ApiInterfaceGroupMembership.AddAttributeChange("externalManager", modificationType, membershipToDelete.ExternalManagers.ToValueChange(ValueModificationType.Delete), changes);
                    ApiInterfaceGroupMembership.AddAttributeChange("owner", modificationType, membershipToDelete.Owners.ToValueChange(ValueModificationType.Delete), changes);
                    ApiInterfaceGroupMembership.AddAttributeChange("externalOwner", modificationType, membershipToDelete.ExternalOwners.ToValueChange(ValueModificationType.Delete), changes);
                }
                catch (AggregateGroupUpdateException ex)
                {
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
                    GroupMemberRequestFactory.AddMembers(csentry.DN, allMembersToAdd, true);

                    foreach (string member in allMembersToDelete)
                    {
                        Logger.WriteLine($"Added member {member}", LogLevel.Debug);
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

        private static void AddAttributeChange(string attributeName, AttributeModificationType modificationType, IList<ValueChange> changes, IList<AttributeChange> attributeChanges)
        {
            if (modificationType == AttributeModificationType.Delete)
            {
                attributeChanges.Add(AttributeChange.CreateAttributeDelete(attributeName));
                return;
            }

            if (changes == null || changes.Count == 0)
            {
                return;
            }

            switch (modificationType)
            {
                case AttributeModificationType.Add:
                    attributeChanges.Add(AttributeChange.CreateAttributeAdd(attributeName, changes.Where(t => t.ModificationType == ValueModificationType.Add).Select(t => t.Value).ToList()));
                    break;

                case AttributeModificationType.Replace:
                    attributeChanges.Add(AttributeChange.CreateAttributeReplace(attributeName, changes.Where(t => t.ModificationType == ValueModificationType.Add).Select(t => t.Value).ToList()));
                    break;

                case AttributeModificationType.Update:
                    attributeChanges.Add(AttributeChange.CreateAttributeUpdate(attributeName, changes));

                    break;

                case AttributeModificationType.Delete:
                case AttributeModificationType.Unconfigured:
                default:
                    throw new InvalidOperationException();
            }
        }

        public override IList<AttributeChange> GetChanges(ObjectModificationType modType, SchemaType type, object source)
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

            foreach (IMASchemaAttribute typeDef in ApiInterfaceGroupMembership.maType.Attributes.Where(t => t.Api == this.Api))
            {
                if (type.HasAttribute(typeDef.AttributeName))
                {
                    attributeChanges.AddRange(typeDef.CreateAttributeChanges(modType, membership));
                }
            }

            return attributeChanges;
        }

        public override string GetAnchorValue(object target)
        {
            return ((Group)target).Id;
        }

        public override string GetDNValue(object target)
        {
            return ((Group)target).Email;
        }

        private static void GetMemberChangesFromCSEntryChange(CSEntryChange csentry, out GroupMembership adds, out GroupMembership deletes, bool replacing)
        {
            adds = new GroupMembership();
            deletes = new GroupMembership();
            GroupMembership existingGroupMembership = null;

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
