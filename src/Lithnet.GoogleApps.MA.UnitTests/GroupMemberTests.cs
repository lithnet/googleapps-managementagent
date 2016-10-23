using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.MetadirectoryServices;
using System.Linq;
using System.Net;
using Google;
using Google.Apis.Admin.Directory.directory_v1.Data;
using Lithnet.GoogleApps.ManagedObjects;
using Lithnet.MetadirectoryServices;

namespace Lithnet.GoogleApps.MA.UnitTests
{
    [TestClass]
    public class GroupMemberTests
    {
        public GroupMemberTests()
        {
            UnitTestControl.TestParameters.InheritGroupRoles = false;
        }
      
        [TestMethod]
        public void DeleteAddMember()
        {
            string id = null;
            try
            {
                Group e;
                string dn = this.CreateGroup(out e);
                id = e.Id;

                System.Threading.Thread.Sleep(2000);

                GroupMemberRequestFactory.AddMember(id, "test@test.com", "MEMBER");
                GroupMemberRequestFactory.RemoveMember(id, "test@test.com");
                System.Threading.Thread.Sleep(500);

                GroupMemberRequestFactory.AddMember(id, "test@test.com", "MEMBER");

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
        public void AddGroupWithMembers()
        {
            string dn = $"{Guid.NewGuid()}@{UnitTestControl.TestParameters.Domain}";

            CSEntryChange cs = CSEntryChange.Create();
            cs.ObjectModificationType = ObjectModificationType.Add;
            cs.DN = dn;
            cs.ObjectType = SchemaConstants.Group;
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("name", Guid.NewGuid().ToString()));

            Group group1;
            Group group2;
            string member1 = this.CreateGroup(out group1);
            string member2 = this.CreateGroup(out group2);
            string member3 = $"{Guid.NewGuid()}@{UnitTestControl.TestParameters.Domain}";

            ManagedObjects.User e = new ManagedObjects.User
            {
                PrimaryEmail = member3,
                Password = Guid.NewGuid().ToString(),
                Name =
                    {
                        GivenName = "test",
                        FamilyName = "test"
                    }
            };

            e = UserRequestFactory.Add(e);

            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("member", new List<object>() { member1, member2 }));
            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("owner", new List<object>() { member3 }));

            CSEntryChangeResult result = null;

            try
            {
                result = ExportProcessor.PutCSEntryChange(cs, UnitTestControl.Schema.GetSchema().Types[SchemaConstants.Group], UnitTestControl.TestParameters);

                if (result.ErrorCode != MAExportError.Success)
                {
                    Assert.Fail(result.ErrorName);
                }

                System.Threading.Thread.Sleep(10000);

                CollectionAssert.AreEquivalent(new string[] { member1, member2 }, GroupMemberRequestFactory.GetMembership(cs.DN).Members.ToArray());
            }
            finally
            {
                string id = result?.AnchorAttributes.FirstOrDefault()?.GetValueAdd<string>();

                if (id != null)
                {
                    GroupRequestFactory.Delete(id);
                }

                if (group1?.Id != null)
                {
                    GroupRequestFactory.Delete(group1.Id);
                }

                if (group2?.Id != null)
                {
                    GroupRequestFactory.Delete(group2.Id);
                }

                if (e?.Id != null)
                {
                    UserRequestFactory.Delete(e.Id);
                }
            }
        }

        [TestMethod]
        public void AddMembers()
        {
            string id = null;
            Group e;
            string dn = this.CreateGroup(out e);
            id = e.Id;

            CSEntryChange cs = CSEntryChange.Create();
            cs.ObjectModificationType = ObjectModificationType.Update;
            cs.DN = dn;
            cs.ObjectType = SchemaConstants.Group;
            cs.AnchorAttributes.Add(AnchorAttribute.Create("id", id));

            Group x;
            string member1 = this.CreateGroup(out x);
            string member2 = this.CreateGroup(out x);

            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("member", new List<object>() { member1, member2 }));

            try
            {
                CSEntryChangeResult result =
                    ExportProcessor.PutCSEntryChange(cs, UnitTestControl.Schema.GetSchema().Types[SchemaConstants.Group], UnitTestControl.TestParameters);

                if (result.ErrorCode != MAExportError.Success)
                {
                    Assert.Fail(result.ErrorName);
                }

                System.Threading.Thread.Sleep(10000);

                CollectionAssert.AreEquivalent(new string[] { member1, member2 }, GroupMemberRequestFactory.GetMembership(cs.DN).Members.ToArray());
            }
            finally
            {
                if (id != null)
                {
                    GroupRequestFactory.Delete(id);
                }

                GroupRequestFactory.Delete(member1);
                GroupRequestFactory.Delete(member2);
            }

        }

        [TestMethod]
        public void AddMember()
        {
            string id = null;
            Group e;
            string dn = this.CreateGroup(out e);
            id = e.Id;
            Group x;
            string member1 = this.CreateGroup(out x);
            GroupMemberRequestFactory.AddMember(dn, new Member() { Email = member1 });

            System.Threading.Thread.Sleep(10000);

            CSEntryChange cs = CSEntryChange.Create();
            cs.ObjectModificationType = ObjectModificationType.Update;
            cs.DN = dn;
            cs.ObjectType = SchemaConstants.Group;
            cs.AnchorAttributes.Add(AnchorAttribute.Create("id", id));

            string member2 = this.CreateGroup(out x);

            cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("member", new List<object>() { member2 }));

            try
            {
                CSEntryChangeResult result =
                    ExportProcessor.PutCSEntryChange(cs, UnitTestControl.Schema.GetSchema().Types[SchemaConstants.Group], UnitTestControl.TestParameters);

                if (result.ErrorCode != MAExportError.Success)
                {
                    Assert.Fail(result.ErrorName);
                }

                System.Threading.Thread.Sleep(10000);

                CollectionAssert.AreEquivalent(new string[] { member1, member2 }, GroupMemberRequestFactory.GetMembership(cs.DN).Members.ToArray());
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
        public void AddAndRemoveLargeNumberOfMembers()
        {
            string id = null;
            try
            {
                Group e;
                string dn = this.CreateGroup(out e);
                id = e.Id;

                System.Threading.Thread.Sleep(10000);

                CSEntryChange cs = CSEntryChange.Create();
                cs.ObjectModificationType = ObjectModificationType.Update;
                cs.DN = dn;
                cs.ObjectType = SchemaConstants.Group;
                cs.AnchorAttributes.Add(AnchorAttribute.Create("id", id));

                List<object> addresses = new List<object>();

                for (int i = 0; i < 1000; i++)
                {
                    string address = $"user{i}@lithnet.io";
                    addresses.Add(address);
                }

                cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("externalMember", addresses));


                CSEntryChangeResult result =
                    ExportProcessor.PutCSEntryChange(cs, UnitTestControl.Schema.GetSchema().Types[SchemaConstants.Group], UnitTestControl.TestParameters);

                if (result.ErrorCode != MAExportError.Success)
                {
                    Assert.Fail(result.ErrorName);
                }

                System.Threading.Thread.Sleep(10000);

                CollectionAssert.AreEquivalent(addresses.ToArray(), GroupMemberRequestFactory.GetMembership(cs.DN).ExternalMembers.ToArray());

                cs = CSEntryChange.Create();
                cs.ObjectModificationType = ObjectModificationType.Update;
                cs.DN = dn;
                cs.ObjectType = SchemaConstants.Group;
                cs.AnchorAttributes.Add(AnchorAttribute.Create("id", id));

                cs.AttributeChanges.Add(AttributeChange.CreateAttributeDelete("externalMember"));
                //cs.AttributeChanges.Add(AttributeChange.CreateAttributeUpdate("externalMember", new List<ValueChange>() { ValueChange.CreateValueAdd("user0@lithnet.io"), ValueChange.CreateValueDelete("test@test.com") }));

                result =
                   ExportProcessor.PutCSEntryChange(cs, UnitTestControl.Schema.GetSchema().Types[SchemaConstants.Group], UnitTestControl.TestParameters);

                if (result.ErrorCode != MAExportError.Success)
                {
                    Assert.Fail(result.ErrorName);
                }

                System.Threading.Thread.Sleep(10000);

                Assert.AreEqual(0, GroupMemberRequestFactory.GetMembership(cs.DN).Members.Count);

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
        public void RemoveMember()
        {
            string id = null;
            Group e;
            string dn = this.CreateGroup(out e);
            id = e.Id;
            Group x;
            string member1 = this.CreateGroup(out x);
            string member2 = this.CreateGroup(out x);
            GroupMemberRequestFactory.AddMember(dn, new Member() { Email = member1 });
            GroupMemberRequestFactory.AddMember(dn, new Member() { Email = member2 });

            System.Threading.Thread.Sleep(10000);

            CSEntryChange cs = CSEntryChange.Create();
            cs.ObjectModificationType = ObjectModificationType.Update;
            cs.DN = dn;
            cs.ObjectType = SchemaConstants.Group;
            cs.AnchorAttributes.Add(AnchorAttribute.Create("id", id));

            cs.AttributeChanges.Add(AttributeChange.CreateAttributeUpdate("member", new List<ValueChange>() { new ValueChange(member2, ValueModificationType.Delete) }));

            try
            {
                CSEntryChangeResult result =
                    ExportProcessor.PutCSEntryChange(cs, UnitTestControl.Schema.GetSchema().Types[SchemaConstants.Group], UnitTestControl.TestParameters);

                if (result.ErrorCode != MAExportError.Success)
                {
                    Assert.Fail(result.ErrorName);
                }

                System.Threading.Thread.Sleep(10000);

                CollectionAssert.AreEquivalent(new string[] { member1 }, GroupMemberRequestFactory.GetMembership(cs.DN).Members.ToArray());
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
        public void RemoveMembers()
        {
            string id = null;
            Group e;
            string dn = this.CreateGroup(out e);
            id = e.Id;
            Group x;
            string member1 = this.CreateGroup(out x);
            string member2 = this.CreateGroup(out x);
            GroupMemberRequestFactory.AddMember(dn, new Member() { Email = member1 });
            GroupMemberRequestFactory.AddMember(dn, new Member() { Email = member2 });

            System.Threading.Thread.Sleep(10000);

            CSEntryChange cs = CSEntryChange.Create();
            cs.ObjectModificationType = ObjectModificationType.Update;
            cs.DN = dn;
            cs.ObjectType = SchemaConstants.Group;
            cs.AnchorAttributes.Add(AnchorAttribute.Create("id", id));

            cs.AttributeChanges.Add(AttributeChange.CreateAttributeDelete("member"));

            try
            {
                CSEntryChangeResult result =
                    ExportProcessor.PutCSEntryChange(cs, UnitTestControl.Schema.GetSchema().Types[SchemaConstants.Group], UnitTestControl.TestParameters);

                if (result.ErrorCode != MAExportError.Success)
                {
                    Assert.Fail(result.ErrorName);
                }

                System.Threading.Thread.Sleep(10000);

                CollectionAssert.AreEquivalent(new string[] { }, GroupMemberRequestFactory.GetMembership(cs.DN).Members.ToArray());
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
        public void ReplaceMembers()
        {
            string id = null;
            Group e;
            string dn = this.CreateGroup(out e);
            id = e.Id;

            CSEntryChange cs = CSEntryChange.Create();
            cs.ObjectModificationType = ObjectModificationType.Update;
            cs.DN = dn;
            cs.ObjectType = SchemaConstants.Group;
            cs.AnchorAttributes.Add(AnchorAttribute.Create("id", id));

            Group x;
            string member1 = this.CreateGroup(out x);
            string member2 = this.CreateGroup(out x);
            string member3 = this.CreateGroup(out x);
            string member4 = this.CreateGroup(out x);
            GroupMemberRequestFactory.AddMember(dn, new Member() { Email = member1 });
            GroupMemberRequestFactory.AddMember(dn, new Member() { Email = member2 });

            System.Threading.Thread.Sleep(10000);

            cs.AttributeChanges.Add(AttributeChange.CreateAttributeReplace("member", new List<object>() { member3, member4 }));

            try
            {
                CSEntryChangeResult result =
                    ExportProcessor.PutCSEntryChange(cs, UnitTestControl.Schema.GetSchema().Types[SchemaConstants.Group], UnitTestControl.TestParameters);

                if (result.ErrorCode != MAExportError.Success)
                {
                    Assert.Fail(result.ErrorName);
                }

                System.Threading.Thread.Sleep(10000);

                CollectionAssert.AreEquivalent(new string[] { member3, member4 }, GroupMemberRequestFactory.GetMembership(cs.DN).Members.ToArray());
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
