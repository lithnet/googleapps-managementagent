using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.MetadirectoryServices;
using System.Linq;
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


        //

        [TestMethod]
        public void Test5()
        {
            string id = null;

            try
            {
                Group e = GroupRequestFactory.Get("phaneendra-test-2@d2-monash-edu.ga-staff-dev.monash.edu");

                
                
                GroupMembership members = GroupMemberRequestFactory.GetMembership(e.Email);
                ApiInterfaceGroupMembership i = new ApiInterfaceGroupMembership();
                IList<AttributeChange> changes = i.GetChanges(e.Email, ObjectModificationType.Add, UnitTestControl.MmsSchema.Types["group"], members, UnitTestControl.TestParameters);

                AttributeChange manager = changes.First(t => t.Name == "externalManager");
                AttributeChange member = changes.First(t => t.Name == "externalMember");

                
            }
            finally
            {
                if (id != null)
                {
                    GroupRequestFactory.Delete(id);
                }
            }
        }

        [TestMethod]
        public void TestExternalManagerIsExternalMember()
        {
            string id = null;

            try
            {
                Group e;
                string dn = this.CreateGroup(out e);
                id = e.Id;
                System.Threading.Thread.Sleep(1000);

                string member2 = "test@test.com";
                GroupMemberRequestFactory.AddMember(dn, new Member() { Email = member2, Role = "MANAGER" });

                System.Threading.Thread.Sleep(1000);

                GroupMembership members = GroupMemberRequestFactory.GetMembership(dn);
                ApiInterfaceGroupMembership i = new ApiInterfaceGroupMembership();
                IList<AttributeChange> changes = i.GetChanges(dn, ObjectModificationType.Add, UnitTestControl.MmsSchema.Types["group"], members, UnitTestControl.TestParameters);

                AttributeChange manager = changes.First(t => t.Name == "externalManager");
                AttributeChange member = changes.First(t => t.Name == "externalMember");

                Assert.AreEqual(manager.ValueChanges.First().Value, member2);
                Assert.AreEqual(member.ValueChanges.First().Value, member2);
            }
            finally
            {
                if (id != null)
                {
                    GroupRequestFactory.Delete(id);
                }
            }
        }

        [TestMethod]
        public void TestManagerIsMember()
        {
            string id = null;
            User user = null;
           
            try
            {
                Group e;
                string dn = this.CreateGroup(out e);
                id = e.Id;
                System.Threading.Thread.Sleep(1000);

                user = UserTests.CreateUser();

                GroupMemberRequestFactory.AddMember(dn, new Member() { Email = user.PrimaryEmail, Role = "MANAGER" });

                System.Threading.Thread.Sleep(1000);

                GroupMembership members = GroupMemberRequestFactory.GetMembership(dn);
                ApiInterfaceGroupMembership i = new ApiInterfaceGroupMembership();
                IList<AttributeChange> changes = i.GetChanges(dn, ObjectModificationType.Add, UnitTestControl.MmsSchema.Types["group"], members, UnitTestControl.TestParameters);

                AttributeChange manager = changes.First(t => t.Name == "manager");
                AttributeChange member = changes.First(t => t.Name == "member");

                Assert.AreEqual(manager.ValueChanges.First().Value, user.PrimaryEmail);
                Assert.AreEqual(member.ValueChanges.First().Value, user.PrimaryEmail);
            }
            finally
            {
                if (id != null)
                {
                    GroupRequestFactory.Delete(id);
                }

                if (user != null)
                {
                    UserRequestFactory.Delete(user.Id);
                }
            }
        }

        [TestMethod]
        public void TestExternalOwnerIsExternalManagerAndExternalMember()
        {
            string id = null;

            try
            {
                Group e;
                string dn = this.CreateGroup(out e);
                id = e.Id;
                System.Threading.Thread.Sleep(1000);

                string member2 = "test@test.com";
                GroupMemberRequestFactory.AddMember(dn, new Member() { Email = member2, Role = "OWNER" });

                System.Threading.Thread.Sleep(1000);

                GroupMembership members = GroupMemberRequestFactory.GetMembership(dn);
                ApiInterfaceGroupMembership i = new ApiInterfaceGroupMembership();
                IList<AttributeChange> changes = i.GetChanges(dn, ObjectModificationType.Add, UnitTestControl.MmsSchema.Types["group"], members, UnitTestControl.TestParameters);

                AttributeChange manager = changes.First(t => t.Name == "externalManager");
                AttributeChange member = changes.First(t => t.Name == "externalMember");
                AttributeChange owner = changes.First(t => t.Name == "externalOwner");

                Assert.AreEqual(manager.ValueChanges.First().Value, member2);
                Assert.AreEqual(member.ValueChanges.First().Value, member2);
                Assert.AreEqual(owner.ValueChanges.First().Value, member2);
            }
            finally
            {
                if (id != null)
                {
                    GroupRequestFactory.Delete(id);
                }
            }

        }

        [TestMethod]
        public void TestOwnerIsManagerAndMember()
        {
            string id = null;
            User user = null;

            try
            {
                Group e;
                string dn = this.CreateGroup(out e);
                id = e.Id;
                System.Threading.Thread.Sleep(1000);

                user = UserTests.CreateUser();

                GroupMemberRequestFactory.AddMember(dn, new Member() { Email = user.PrimaryEmail, Role = "OWNER" });

                System.Threading.Thread.Sleep(1000);

                GroupMembership members = GroupMemberRequestFactory.GetMembership(dn);
                ApiInterfaceGroupMembership i = new ApiInterfaceGroupMembership();
                IList<AttributeChange> changes = i.GetChanges(dn, ObjectModificationType.Add, UnitTestControl.MmsSchema.Types["group"], members, UnitTestControl.TestParameters);

                AttributeChange owner = changes.First(t => t.Name == "owner");
                AttributeChange manager = changes.First(t => t.Name == "manager");
                AttributeChange member = changes.First(t => t.Name == "member");

                Assert.AreEqual(owner.ValueChanges.First().Value, user.PrimaryEmail);
                Assert.AreEqual(manager.ValueChanges.First().Value, user.PrimaryEmail);
                Assert.AreEqual(member.ValueChanges.First().Value, user.PrimaryEmail);
            }
            finally
            {
                if (id != null)
                {
                    GroupRequestFactory.Delete(id);
                }

                if (user != null)
                {
                    UserRequestFactory.Delete(user.Id);
                }
            }
        }

        [TestMethod]
        public void DowngradeManagerToMember()
        {
            string id = null;

            try
            {
                Group e;
                string dn = this.CreateGroup(out e);
                id = e.Id;

                string member2 = "test@test.com";
                System.Threading.Thread.Sleep(1000);

                GroupMemberRequestFactory.AddMember(dn, member2, "MANAGER");

                System.Threading.Thread.Sleep(1000);

                CSEntryChange cs = CSEntryChange.Create();
                cs.ObjectModificationType = ObjectModificationType.Update;
                cs.DN = dn;
                cs.ObjectType = SchemaConstants.Group;
                cs.AnchorAttributes.Add(AnchorAttribute.Create("id", id));

                cs.AttributeChanges.Add(AttributeChange.CreateAttributeUpdate("externalManager", new List<ValueChange>() { new ValueChange(member2, ValueModificationType.Delete) }));

                CSEntryChangeResult result =
                       ExportProcessor.PutCSEntryChange(cs, UnitTestControl.Schema.GetSchema().Types[SchemaConstants.Group], UnitTestControl.TestParameters);

                if (result.ErrorCode != MAExportError.Success)
                {
                    Assert.Fail(result.ErrorName);
                }

                System.Threading.Thread.Sleep(1000);
                GroupMembership membership = GroupMemberRequestFactory.GetMembership(cs.DN);

                Assert.AreEqual(0, membership.ExternalManagers.Count);
                Assert.AreEqual(0, membership.Managers.Count);
                Assert.AreEqual(0, membership.Members.Count);
                CollectionAssert.AreEquivalent(new string[] { member2 }, membership.ExternalMembers.ToArray());
            }
            finally
            {
                if (id != null)
                {
                    GroupRequestFactory.Delete(id);
                }
            }

        }

        [TestMethod]
        public void DowngradeOwnerToManager()
        {
            string id = null;

            try
            {
                Group e;
                string dn = this.CreateGroup(out e);
                id = e.Id;

                string member2 = "test@test.com";
                System.Threading.Thread.Sleep(1000);

                GroupMemberRequestFactory.AddMember(dn, member2, "OWNER");

                System.Threading.Thread.Sleep(1000);

                CSEntryChange cs = CSEntryChange.Create();
                cs.ObjectModificationType = ObjectModificationType.Update;
                cs.DN = dn;
                cs.ObjectType = SchemaConstants.Group;
                cs.AnchorAttributes.Add(AnchorAttribute.Create("id", id));

                cs.AttributeChanges.Add(AttributeChange.CreateAttributeUpdate("externalOwner", new List<ValueChange>() { new ValueChange(member2, ValueModificationType.Delete) }));

                CSEntryChangeResult result =
                       ExportProcessor.PutCSEntryChange(cs, UnitTestControl.Schema.GetSchema().Types[SchemaConstants.Group], UnitTestControl.TestParameters);

                if (result.ErrorCode != MAExportError.Success)
                {
                    Assert.Fail(result.ErrorName);
                }

                System.Threading.Thread.Sleep(1000);
                GroupMembership membership = GroupMemberRequestFactory.GetMembership(cs.DN);

                Assert.AreEqual(0, membership.ExternalOwners.Count);
                Assert.AreEqual(0, membership.Owners.Count);
                Assert.AreEqual(0, membership.Managers.Count);
                Assert.AreEqual(0, membership.Members.Count);
                CollectionAssert.AreEquivalent(new string[] { member2 }, membership.ExternalManagers.ToArray());
            }
            finally
            {
                if (id != null)
                {
                    GroupRequestFactory.Delete(id);
                }
            }

        }

        [TestMethod]
        public void UpgradeMemberToManager()
        {
            string id = null;

            try
            {
                Group e;
                string dn = this.CreateGroup(out e);
                id = e.Id;
                System.Threading.Thread.Sleep(1000);

                string member2 = "test@test.com";
                GroupMemberRequestFactory.AddMember(dn, new Member() { Email = member2 });

                System.Threading.Thread.Sleep(1000);

                CSEntryChange cs = CSEntryChange.Create();
                cs.ObjectModificationType = ObjectModificationType.Update;
                cs.DN = dn;
                cs.ObjectType = SchemaConstants.Group;
                cs.AnchorAttributes.Add(AnchorAttribute.Create("id", id));

                cs.AttributeChanges.Add(AttributeChange.CreateAttributeUpdate("externalManager", new List<ValueChange>() { new ValueChange(member2, ValueModificationType.Add) }));

                CSEntryChangeResult result =
                       ExportProcessor.PutCSEntryChange(cs, UnitTestControl.Schema.GetSchema().Types[SchemaConstants.Group], UnitTestControl.TestParameters);

                if (result.ErrorCode != MAExportError.Success)
                {
                    Assert.Fail(result.ErrorName);
                }

                System.Threading.Thread.Sleep(1000);

                CollectionAssert.AreEquivalent(new string[] { member2 }, GroupMemberRequestFactory.GetMembership(cs.DN).ExternalManagers.ToArray());
            }
            finally
            {
                if (id != null)
                {
                    GroupRequestFactory.Delete(id);
                }
            }
        }

        [TestMethod]
        public void UpgradeManagerToOwner()
        {
            string id = null;

            try
            {
                Group e;
                string dn = this.CreateGroup(out e);
                id = e.Id;
                System.Threading.Thread.Sleep(1000);

                string member2 = "test@test.com";
                GroupMemberRequestFactory.AddMember(dn, new Member() { Email = member2, Role="MANAGER" });

                System.Threading.Thread.Sleep(1000);

                CSEntryChange cs = CSEntryChange.Create();
                cs.ObjectModificationType = ObjectModificationType.Update;
                cs.DN = dn;
                cs.ObjectType = SchemaConstants.Group;
                cs.AnchorAttributes.Add(AnchorAttribute.Create("id", id));

                cs.AttributeChanges.Add(AttributeChange.CreateAttributeUpdate("externalOwner", new List<ValueChange>() { new ValueChange(member2, ValueModificationType.Add) }));

                CSEntryChangeResult result =
                       ExportProcessor.PutCSEntryChange(cs, UnitTestControl.Schema.GetSchema().Types[SchemaConstants.Group], UnitTestControl.TestParameters);

                if (result.ErrorCode != MAExportError.Success)
                {
                    Assert.Fail(result.ErrorName);
                }

                System.Threading.Thread.Sleep(1000);

                CollectionAssert.AreEquivalent(new string[] { member2 }, GroupMemberRequestFactory.GetMembership(cs.DN).ExternalOwners.ToArray());
            }
            finally
            {
                if (id != null)
                {
                    GroupRequestFactory.Delete(id);
                }
            }
        }

        [TestMethod]
        public void ChangeRoleForLargeNumberOfMembers()
        {
            return;

            string id = null;
            try
            {
                Group e;
                string dn = this.CreateGroup(out e);
                id = e.Id;

                System.Threading.Thread.Sleep(1000);

                CSEntryChange cs = CSEntryChange.Create();
                cs.ObjectModificationType = ObjectModificationType.Update;
                cs.DN = dn;
                cs.ObjectType = SchemaConstants.Group;
                cs.AnchorAttributes.Add(AnchorAttribute.Create("id", id));

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

                System.Threading.Thread.Sleep(1000);

                CollectionAssert.AreEquivalent(addresses.ToArray(), GroupMemberRequestFactory.GetMembership(cs.DN).ExternalMembers.ToArray());

                cs = CSEntryChange.Create();
                cs.ObjectModificationType = ObjectModificationType.Update;
                cs.DN = dn;
                cs.ObjectType = SchemaConstants.Group;
                cs.AnchorAttributes.Add(AnchorAttribute.Create("id", id));
                cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("externalManager", addresses));

                result =
                   ExportProcessor.PutCSEntryChange(cs, UnitTestControl.Schema.GetSchema().Types[SchemaConstants.Group], UnitTestControl.TestParameters);

                if (result.ErrorCode != MAExportError.Success)
                {
                    Assert.Fail(result.ErrorName);
                }

                System.Threading.Thread.Sleep(1000);

                CollectionAssert.AreEquivalent(addresses.ToArray(), GroupMemberRequestFactory.GetMembership(cs.DN).ExternalManagers.ToArray());
            }
            finally
            {
                if (id != null)
                {
                    GroupRequestFactory.Delete(id);
                }
            }

        }


        private string CreateGroup(out Group e)
        {
            string dn = $"{Guid.NewGuid()}@{UnitTestControl.TestParameters.Domain}";
            e = new Group
            {
                Email = dn,
                Name = Guid.NewGuid().ToString()
            };

            e = GroupRequestFactory.Add(e);

            return dn;
        }
    }
}
