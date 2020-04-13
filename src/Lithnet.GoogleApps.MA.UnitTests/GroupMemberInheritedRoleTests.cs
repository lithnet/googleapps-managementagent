using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.MetadirectoryServices;
using System.Linq;
using System.Threading;
using Google.Apis.Admin.Directory.directory_v1.Data;
using Lithnet.GoogleApps.ManagedObjects;
using User = Lithnet.GoogleApps.ManagedObjects.User;

namespace Lithnet.GoogleApps.MA.UnitTests
{
    [TestClass]
    public class GroupMemberInheritedRoleTests
    {
        public GroupMemberInheritedRoleTests()
        {
            UnitTestControl.TestParameters.InheritGroupRoles = true;
        }

        [TestMethod]
        public void RemoveExternalMembers()
        {
            Group e = null;

            try
            {
                e = UnitTestControl.CreateGroup();
                string member1 = "user1@lithnet.io";
                string member2 = "user2@lithnet.io";

                UnitTestControl.TestParameters.GroupsService.MemberFactory.AddMember(e.Email, new Member() { Email = member1 });
                UnitTestControl.TestParameters.GroupsService.MemberFactory.AddMember(e.Email, new Member() { Email = member2 });

                Thread.Sleep(1000);

                CSEntryChange cs = CSEntryChange.Create();
                cs.ObjectModificationType = ObjectModificationType.Update;
                cs.DN = e.Email;
                cs.ObjectType = SchemaConstants.Group;
                cs.AnchorAttributes.Add(AnchorAttribute.Create("id", e.Id));

                cs.AttributeChanges.Add(AttributeChange.CreateAttributeDelete("externalMember"));

                CSEntryChangeResult result = ExportProcessor.PutCSEntryChange(cs, UnitTestControl.Schema.GetSchema().Types[SchemaConstants.Group], UnitTestControl.TestParameters);

                if (result.ErrorCode != MAExportError.Success)
                {
                    Assert.Fail(result.ErrorName);
                }

                Thread.Sleep(5000);

                Assert.AreEqual(0, UnitTestControl.TestParameters.GroupsService.MemberFactory.GetMembership(cs.DN).Members.Count);
                Assert.AreEqual(0, UnitTestControl.TestParameters.GroupsService.MemberFactory.GetMembership(cs.DN).ExternalMembers.Count);
                Assert.AreEqual(0, UnitTestControl.TestParameters.GroupsService.MemberFactory.GetMembership(cs.DN).Managers.Count);
                Assert.AreEqual(0, UnitTestControl.TestParameters.GroupsService.MemberFactory.GetMembership(cs.DN).ExternalManagers.Count);
                Assert.AreEqual(0, UnitTestControl.TestParameters.GroupsService.MemberFactory.GetMembership(cs.DN).Owners.Count);
                Assert.AreEqual(0, UnitTestControl.TestParameters.GroupsService.MemberFactory.GetMembership(cs.DN).ExternalOwners.Count);
            }
            finally
            {
                UnitTestControl.Cleanup(e);
            }
        }

        [TestMethod]
        public void TestExternalManagerIsExternalMember()
        {
            Group e = null;

            try
            {
                e = UnitTestControl.CreateGroup();
                Thread.Sleep(1000);

                string member2 = "test@lithnet.io";
                UnitTestControl.TestParameters.GroupsService.MemberFactory.AddMember(e.Email, new Member() { Email = member2, Role = "MANAGER" });

                System.Threading.Thread.Sleep(UnitTestControl.PostGoogleOperationSleepInterval);

                GroupMembership members = UnitTestControl.TestParameters.GroupsService.MemberFactory.GetMembership(e.Email);
                ApiInterfaceGroupMembership i = new ApiInterfaceGroupMembership(UnitTestControl.TestParameters);
                IEnumerable<AttributeChange> changes = i.GetChanges(e.Email, ObjectModificationType.Add, UnitTestControl.MmsSchema.Types["group"], members);

                AttributeChange manager = changes.First(t => t.Name == "externalManager");
                AttributeChange member = changes.First(t => t.Name == "externalMember");

                Assert.AreEqual(manager.ValueChanges.First().Value, member2);
                Assert.AreEqual(member.ValueChanges.First().Value, member2);
            }
            finally
            {
                UnitTestControl.Cleanup(e);
            }
        }

        [TestMethod]
        public void TestManagerIsMember()
        {
            Group e = null;
            User user = null;

            try
            {
                e = UnitTestControl.CreateGroup();
                Thread.Sleep(1000);

                user = UserTests.CreateUser();

                UnitTestControl.TestParameters.GroupsService.MemberFactory.AddMember(e.Email, new Member() { Email = user.PrimaryEmail, Role = "MANAGER" });

                Thread.Sleep(5000);

                GroupMembership members = UnitTestControl.TestParameters.GroupsService.MemberFactory.GetMembership(e.Email);
                ApiInterfaceGroupMembership i = new ApiInterfaceGroupMembership(UnitTestControl.TestParameters);
                IEnumerable<AttributeChange> changes = i.GetChanges(e.Email, ObjectModificationType.Add, UnitTestControl.MmsSchema.Types["group"], members);

                AttributeChange manager = changes.First(t => t.Name == "manager");
                AttributeChange member = changes.First(t => t.Name == "member");

                Assert.AreEqual(manager.ValueChanges.First().Value, user.PrimaryEmail);
                Assert.AreEqual(member.ValueChanges.First().Value, user.PrimaryEmail);
            }
            finally
            {
                UnitTestControl.Cleanup(e, user);
            }
        }

        [TestMethod]
        public void TestExternalOwnerIsExternalManagerAndExternalMember()
        {
            Group e = null;

            try
            {
                e = UnitTestControl.CreateGroup();
                Thread.Sleep(1000);

                string member2 = "test@lithnet.io";
                UnitTestControl.TestParameters.GroupsService.MemberFactory.AddMember(e.Email, new Member() { Email = member2, Role = "OWNER" });

                Thread.Sleep(5000);

                GroupMembership members = UnitTestControl.TestParameters.GroupsService.MemberFactory.GetMembership(e.Email);
                ApiInterfaceGroupMembership i = new ApiInterfaceGroupMembership(UnitTestControl.TestParameters);
                IEnumerable<AttributeChange> changes = i.GetChanges(e.Email, ObjectModificationType.Add, UnitTestControl.MmsSchema.Types["group"], members);

                AttributeChange manager = changes.First(t => t.Name == "externalManager");
                AttributeChange member = changes.First(t => t.Name == "externalMember");
                AttributeChange owner = changes.First(t => t.Name == "externalOwner");

                Assert.AreEqual(manager.ValueChanges.First().Value, member2);
                Assert.AreEqual(member.ValueChanges.First().Value, member2);
                Assert.AreEqual(owner.ValueChanges.First().Value, member2);
            }
            finally
            {
                UnitTestControl.Cleanup(e);
            }
        }

        [TestMethod]
        public void TestOwnerIsManagerAndMember()
        {
            Group e = null;
            User user = null;

            try
            {
                e = UnitTestControl.CreateGroup();
                Thread.Sleep(1000);

                user = UserTests.CreateUser();

                UnitTestControl.TestParameters.GroupsService.MemberFactory.AddMember(e.Email, new Member() { Email = user.PrimaryEmail, Role = "OWNER" });

                Thread.Sleep(5000);

                GroupMembership members = UnitTestControl.TestParameters.GroupsService.MemberFactory.GetMembership(e.Email);
                ApiInterfaceGroupMembership i = new ApiInterfaceGroupMembership(UnitTestControl.TestParameters);
                IEnumerable<AttributeChange> changes = i.GetChanges(e.Email, ObjectModificationType.Add, UnitTestControl.MmsSchema.Types["group"], members);

                AttributeChange owner = changes.First(t => t.Name == "owner");
                AttributeChange manager = changes.First(t => t.Name == "manager");
                AttributeChange member = changes.First(t => t.Name == "member");

                Assert.AreEqual(owner.ValueChanges.First().Value, user.PrimaryEmail);
                Assert.AreEqual(manager.ValueChanges.First().Value, user.PrimaryEmail);
                Assert.AreEqual(member.ValueChanges.First().Value, user.PrimaryEmail);
            }
            finally
            {
                UnitTestControl.Cleanup(e, user);
            }
        }

        [TestMethod]
        public void DowngradeManagerToMember()
        {
            Group e = null;

            try
            {
                e = UnitTestControl.CreateGroup();

                string member2 = "test@lithnet.io";
                Thread.Sleep(1000);

                UnitTestControl.TestParameters.GroupsService.MemberFactory.AddMember(e.Email, member2, "MANAGER");

                Thread.Sleep(5000);

                CSEntryChange cs = CSEntryChange.Create();
                cs.ObjectModificationType = ObjectModificationType.Update;
                cs.DN = e.Email;
                cs.ObjectType = SchemaConstants.Group;
                cs.AnchorAttributes.Add(AnchorAttribute.Create("id", e.Email));

                cs.AttributeChanges.Add(AttributeChange.CreateAttributeUpdate("externalManager", new List<ValueChange>() { new ValueChange(member2, ValueModificationType.Delete) }));

                CSEntryChangeResult result =
                    ExportProcessor.PutCSEntryChange(cs, UnitTestControl.Schema.GetSchema().Types[SchemaConstants.Group], UnitTestControl.TestParameters);

                if (result.ErrorCode != MAExportError.Success)
                {
                    Assert.Fail(result.ErrorName);
                }

                Thread.Sleep(5000);
                GroupMembership membership = UnitTestControl.TestParameters.GroupsService.MemberFactory.GetMembership(cs.DN);

                Assert.AreEqual(0, membership.ExternalManagers.Count);
                Assert.AreEqual(0, membership.Managers.Count);
                Assert.AreEqual(0, membership.Members.Count);
                CollectionAssert.AreEquivalent(new string[] { member2 }, membership.ExternalMembers.ToArray());
            }
            finally
            {
                UnitTestControl.Cleanup(e);
            }
        }

        [TestMethod]
        public void DowngradeManagersToMembers()
        {
            Group e = null;
            User member1 = null;
            User member2 = null;
            User member3 = null;
            User member4 = null;

            try
            {
                e = UnitTestControl.CreateGroup();

                member1 = UserTests.CreateUser();
                member2 = UserTests.CreateUser();
                member3 = UserTests.CreateUser();
                member4 = UserTests.CreateUser();

                UnitTestControl.TestParameters.GroupsService.MemberFactory.AddMember(e.Email, member1.PrimaryEmail, "MANAGER");
                UnitTestControl.TestParameters.GroupsService.MemberFactory.AddMember(e.Email, member2.PrimaryEmail, "MANAGER");
                UnitTestControl.TestParameters.GroupsService.MemberFactory.AddMember(e.Email, member3.PrimaryEmail, "MEMBER");
                UnitTestControl.TestParameters.GroupsService.MemberFactory.AddMember(e.Email, member4.PrimaryEmail, "MEMBER");
                Thread.Sleep(1000);

                CSEntryChange cs = CSEntryChange.Create();
                cs.ObjectModificationType = ObjectModificationType.Update;
                cs.DN = e.Email;
                cs.ObjectType = SchemaConstants.Group;
                cs.AnchorAttributes.Add(AnchorAttribute.Create("id", e.Email));

                cs.AttributeChanges.Add(AttributeChange.CreateAttributeDelete("manager"));

                CSEntryChangeResult result =
                    ExportProcessor.PutCSEntryChange(cs, UnitTestControl.Schema.GetSchema().Types[SchemaConstants.Group], UnitTestControl.TestParameters);

                if (result.ErrorCode != MAExportError.Success)
                {
                    Assert.Fail(result.ErrorName);
                }

                System.Threading.Thread.Sleep(UnitTestControl.PostGoogleOperationSleepInterval);
                GroupMembership membership = UnitTestControl.TestParameters.GroupsService.MemberFactory.GetMembership(cs.DN);

                Assert.AreEqual(0, membership.ExternalManagers.Count);
                Assert.AreEqual(0, membership.Managers.Count);
                Assert.AreEqual(0, membership.ExternalMembers.Count);
                CollectionAssert.AreEquivalent(new string[] { member1.PrimaryEmail, member2.PrimaryEmail, member3.PrimaryEmail, member4.PrimaryEmail }, membership.Members.ToArray());
            }
            finally
            {
                UnitTestControl.Cleanup(e, member1, member2, member3, member4);
            }
        }

        [TestMethod]
        public void DowngradeOwnerToManager()
        {
            Group e = null;

            try
            {
                e = UnitTestControl.CreateGroup();

                string member2 = "test@lithnet.io";
                Thread.Sleep(1000);

                UnitTestControl.TestParameters.GroupsService.MemberFactory.AddMember(e.Email, member2, "OWNER");

                Thread.Sleep(1000);

                CSEntryChange cs = CSEntryChange.Create();
                cs.ObjectModificationType = ObjectModificationType.Update;
                cs.DN = e.Email;
                cs.ObjectType = SchemaConstants.Group;
                cs.AnchorAttributes.Add(AnchorAttribute.Create("id", e.Id));

                cs.AttributeChanges.Add(AttributeChange.CreateAttributeUpdate("externalOwner", new List<ValueChange>() { new ValueChange(member2, ValueModificationType.Delete) }));

                CSEntryChangeResult result =
                    ExportProcessor.PutCSEntryChange(cs, UnitTestControl.Schema.GetSchema().Types[SchemaConstants.Group], UnitTestControl.TestParameters);

                if (result.ErrorCode != MAExportError.Success)
                {
                    Assert.Fail(result.ErrorName);
                }

                Thread.Sleep(1000);
                GroupMembership membership = UnitTestControl.TestParameters.GroupsService.MemberFactory.GetMembership(cs.DN);

                Assert.AreEqual(0, membership.ExternalOwners.Count);
                Assert.AreEqual(0, membership.Owners.Count);
                Assert.AreEqual(0, membership.Managers.Count);
                Assert.AreEqual(0, membership.Members.Count);
                CollectionAssert.AreEquivalent(new string[] { member2 }, membership.ExternalManagers.ToArray());
            }
            finally
            {
                UnitTestControl.Cleanup(e);
            }
        }

        [TestMethod]
        public void UpgradeMemberToManager()
        {
            Group e = null;

            try
            {
                e = UnitTestControl.CreateGroup();
                Thread.Sleep(1000);

                string member2 = "test@lithnet.io";
                UnitTestControl.TestParameters.GroupsService.MemberFactory.AddMember(e.Email, new Member() { Email = member2 });

                Thread.Sleep(5000);

                CSEntryChange cs = CSEntryChange.Create();
                cs.ObjectModificationType = ObjectModificationType.Update;
                cs.DN = e.Email;
                cs.ObjectType = SchemaConstants.Group;
                cs.AnchorAttributes.Add(AnchorAttribute.Create("id", e.Id));

                cs.AttributeChanges.Add(AttributeChange.CreateAttributeUpdate("externalManager", new List<ValueChange>() { new ValueChange(member2, ValueModificationType.Add) }));

                CSEntryChangeResult result =
                    ExportProcessor.PutCSEntryChange(cs, UnitTestControl.Schema.GetSchema().Types[SchemaConstants.Group], UnitTestControl.TestParameters);

                if (result.ErrorCode != MAExportError.Success)
                {
                    Assert.Fail(result.ErrorName);
                }

                Thread.Sleep(5000);

                CollectionAssert.AreEquivalent(new string[] { member2 }, UnitTestControl.TestParameters.GroupsService.MemberFactory.GetMembership(cs.DN).ExternalManagers.ToArray());
            }
            finally
            {
                UnitTestControl.Cleanup(e);
            }
        }

        [TestMethod]
        public void UpgradeManagerToOwner()
        {
            Group e = null;

            try
            {
                e = UnitTestControl.CreateGroup();
                Thread.Sleep(1000);

                string member2 = "test@lithnet.io";
                UnitTestControl.TestParameters.GroupsService.MemberFactory.AddMember(e.Email, new Member() { Email = member2, Role = "MANAGER" });

                Thread.Sleep(1000);

                CSEntryChange cs = CSEntryChange.Create();
                cs.ObjectModificationType = ObjectModificationType.Update;
                cs.DN = e.Email;
                cs.ObjectType = SchemaConstants.Group;
                cs.AnchorAttributes.Add(AnchorAttribute.Create("id", e.Id));

                cs.AttributeChanges.Add(AttributeChange.CreateAttributeUpdate("externalOwner", new List<ValueChange>() { new ValueChange(member2, ValueModificationType.Add) }));

                CSEntryChangeResult result =
                    ExportProcessor.PutCSEntryChange(cs, UnitTestControl.Schema.GetSchema().Types[SchemaConstants.Group], UnitTestControl.TestParameters);

                if (result.ErrorCode != MAExportError.Success)
                {
                    Assert.Fail(result.ErrorName);
                }

                Thread.Sleep(1000);

                CollectionAssert.AreEquivalent(new string[] { member2 }, UnitTestControl.TestParameters.GroupsService.MemberFactory.GetMembership(cs.DN).ExternalOwners.ToArray());
            }
            finally
            {
                UnitTestControl.Cleanup(e);
            }
        }

        [TestMethod]
        public void ChangeRoleForLargeNumberOfMembers()
        {
            return;

            Group e = null;

            try
            {
                e = UnitTestControl.CreateGroup();

                Thread.Sleep(1000);

                CSEntryChange cs = CSEntryChange.Create();
                cs.ObjectModificationType = ObjectModificationType.Update;
                cs.DN = e.Email;
                cs.ObjectType = SchemaConstants.Group;
                cs.AnchorAttributes.Add(AnchorAttribute.Create("id", e.Id));

                List<object> addresses = new List<object>();

                for (int i = 0; i < 100; i++)
                {
                    string address = $"user{i}@lithnet.io";
                    addresses.Add(address);
                }

                //addresses.Add("notanaddress");

                cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("externalMember", addresses));

                CSEntryChangeResult result =
                    ExportProcessor.PutCSEntryChange(cs, UnitTestControl.Schema.GetSchema().Types[SchemaConstants.Group], UnitTestControl.TestParameters);

                if (result.ErrorCode != MAExportError.Success)
                {
                    Assert.Fail(result.ErrorName);
                }

                Thread.Sleep(1000);

                CollectionAssert.AreEquivalent(addresses.ToArray(), UnitTestControl.TestParameters.GroupsService.MemberFactory.GetMembership(cs.DN).ExternalMembers.ToArray());

                cs = CSEntryChange.Create();
                cs.ObjectModificationType = ObjectModificationType.Update;
                cs.DN = e.Email;
                cs.ObjectType = SchemaConstants.Group;
                cs.AnchorAttributes.Add(AnchorAttribute.Create("id", e.Id));
                cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("externalManager", addresses));

                result =
                    ExportProcessor.PutCSEntryChange(cs, UnitTestControl.Schema.GetSchema().Types[SchemaConstants.Group], UnitTestControl.TestParameters);

                if (result.ErrorCode != MAExportError.Success)
                {
                    Assert.Fail(result.ErrorName);
                }

                Thread.Sleep(1000);

                CollectionAssert.AreEquivalent(addresses.ToArray(), UnitTestControl.TestParameters.GroupsService.MemberFactory.GetMembership(cs.DN).ExternalManagers.ToArray());
            }
            finally
            {
                UnitTestControl.Cleanup(e);
            }
        }
    }
}
