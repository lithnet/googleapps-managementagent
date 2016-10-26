using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.MetadirectoryServices;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Google;
using Google.Apis.Admin.Directory.directory_v1.Data;
using Lithnet.GoogleApps.ManagedObjects;
using Lithnet.MetadirectoryServices;
using User = Lithnet.GoogleApps.ManagedObjects.User;

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
            Group e = null;

            try
            {
                e = this.CreateGroup();

                System.Threading.Thread.Sleep(1000);

                GroupMemberRequestFactory.AddMember(e.Id, "test@test.com", "MEMBER");
                GroupMemberRequestFactory.RemoveMember(e.Id, "test@test.com");
                System.Threading.Thread.Sleep(500);

                GroupMemberRequestFactory.AddMember(e.Id, "test@test.com", "MEMBER");

            }
            finally
            {
                this.Cleanup(e);
            }
        }

        [TestMethod]
        public void AddGroupWithMembers()
        {
            Group member1 = null;
            Group member2 = null;
            User member3 = null;

            try
            {
                string dn = $"{Guid.NewGuid()}@{UnitTestControl.TestParameters.Domain}";

                CSEntryChange cs = CSEntryChange.Create();
                cs.ObjectModificationType = ObjectModificationType.Add;
                cs.DN = dn;
                cs.ObjectType = SchemaConstants.Group;
                cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("name", Guid.NewGuid().ToString()));

                member1 = this.CreateGroup();
                member2 = this.CreateGroup();
                member3 = UserTests.CreateUser();

                cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("member", new List<object>() { member1.Email, member2.Email }));
                cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("owner", new List<object>() { member3.PrimaryEmail }));

                CSEntryChangeResult result = null;

                result = ExportProcessor.PutCSEntryChange(cs, UnitTestControl.Schema.GetSchema().Types[SchemaConstants.Group], UnitTestControl.TestParameters);

                if (result.ErrorCode != MAExportError.Success)
                {
                    Assert.Fail(result.ErrorName);
                }

                System.Threading.Thread.Sleep(1000);

                CollectionAssert.AreEquivalent(new string[] { member1.Email, member2.Email }, GroupMemberRequestFactory.GetMembership(cs.DN).Members.ToArray());
            }
            finally
            {
                this.Cleanup(member1, member2, member3);
            }
        }

        [TestMethod]
        public void AddMembers()
        {
            Group e = null;
            Group member1 = null;
            Group member2 = null;

            try
            {
                e = this.CreateGroup();
                member1 = this.CreateGroup();
                member2 = this.CreateGroup();

                CSEntryChange cs = CSEntryChange.Create();
                cs.ObjectModificationType = ObjectModificationType.Update;
                cs.DN = e.Email;
                cs.ObjectType = SchemaConstants.Group;
                cs.AnchorAttributes.Add(AnchorAttribute.Create("id", e.Id));

                cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("member", new List<object>() { member1.Email, member2.Email }));


                CSEntryChangeResult result =
                    ExportProcessor.PutCSEntryChange(cs, UnitTestControl.Schema.GetSchema().Types[SchemaConstants.Group], UnitTestControl.TestParameters);

                if (result.ErrorCode != MAExportError.Success)
                {
                    Assert.Fail(result.ErrorName);
                }

                System.Threading.Thread.Sleep(1000);

                CollectionAssert.AreEquivalent(new string[] { member1.Email, member2.Email }, GroupMemberRequestFactory.GetMembership(cs.DN).Members.ToArray());
            }
            finally
            {
                this.Cleanup(e, member1, member2);
            }
        }

        [TestMethod]
        public void AddMember()
        {
            Group e = null;
            Group member1 = null;
            Group member2 = null;

            try
            {
                e = this.CreateGroup();
                member1 = this.CreateGroup();
                member2 = this.CreateGroup();

                GroupMemberRequestFactory.AddMember(e.Email, new Member() { Email = member1.Email });

                System.Threading.Thread.Sleep(1000);

                CSEntryChange cs = CSEntryChange.Create();
                cs.ObjectModificationType = ObjectModificationType.Update;
                cs.DN = e.Email;
                cs.ObjectType = SchemaConstants.Group;
                cs.AnchorAttributes.Add(AnchorAttribute.Create("id", e.Id));

                cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("member", new List<object>() { member2.Email }));

                CSEntryChangeResult result =
                    ExportProcessor.PutCSEntryChange(cs, UnitTestControl.Schema.GetSchema().Types[SchemaConstants.Group], UnitTestControl.TestParameters);

                if (result.ErrorCode != MAExportError.Success)
                {
                    Assert.Fail(result.ErrorName);
                }

                System.Threading.Thread.Sleep(1000);

                CollectionAssert.AreEquivalent(new string[] { member1.Email, member2.Email }, GroupMemberRequestFactory.GetMembership(cs.DN).Members.ToArray());
            }
            finally
            {
                this.Cleanup(e, member1, member2);
            }
        }

        [TestMethod]
        public void AddAndRemoveLargeNumberOfMembers()
        {
            Group e = null;

            try
            {
                e = this.CreateGroup();

                System.Threading.Thread.Sleep(1000);

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
                cs.DN = e.Email;
                cs.ObjectType = SchemaConstants.Group;
                cs.AnchorAttributes.Add(AnchorAttribute.Create("id", e.Id));

                cs.AttributeChanges.Add(AttributeChange.CreateAttributeDelete("externalMember"));

                result =
                   ExportProcessor.PutCSEntryChange(cs, UnitTestControl.Schema.GetSchema().Types[SchemaConstants.Group], UnitTestControl.TestParameters);

                if (result.ErrorCode != MAExportError.Success)
                {
                    Assert.Fail(result.ErrorName);
                }

                System.Threading.Thread.Sleep(1000);

                Assert.AreEqual(0, GroupMemberRequestFactory.GetMembership(cs.DN).Members.Count);

            }
            finally
            {
                this.Cleanup(e);
            }
        }

        [TestMethod]
        public void TriggerBackOff()
        {
            return;

            List<Group> groups = new List<Group>();

            try
            {
                List<CSEntryChange> changes = new List<CSEntryChange>();

                for (int i = 0; i < 50; i++)
                {
                    Group e = this.CreateGroup();
                    groups.Add(e);
                    CSEntryChange cs = GroupMemberTests.CreateCSEntryUpdate(e);

                    List<object> addresses = new List<object>();

                    for (int j = 0; j < 100; j++)
                    {
                        string address = $"user-{Guid.NewGuid()}@lithnet.io";
                        addresses.Add(address);
                    }

                    cs.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("externalMember", addresses));
                    changes.Add(cs);
                }

                int directoryServicePoolSize = 30;
                ConnectionPools.InitializePools(UnitTestControl.TestParameters.Credentials, directoryServicePoolSize, 30, 30, 30);
                int threadCount = 0;
                GroupMemberRequestFactory.BatchSize = 100;
                ConnectionPools.SetConcurrentOperationLimitGroupMember(5);
                
                Task q = new Task(() =>
                {
                    Parallel.For(0, 1000, u =>
                    {
                        User x = UserTests.CreateUser();
                        Trace.WriteLine($"Created user {x.PrimaryEmail}");
                        UserRequestFactory.Delete(x.Id);
                    });

                });

                q.Start();

                ParallelOptions op = new ParallelOptions();
                op.MaxDegreeOfParallelism = directoryServicePoolSize;

                Parallel.ForEach(changes, op, t =>
                {
                    int threadID = Interlocked.Increment(ref threadCount);

                    Trace.WriteLine($"Thread count {threadID}");

                    try
                    {
                        CSEntryChangeResult result =
                            ExportProcessor.PutCSEntryChange(t, UnitTestControl.Schema.GetSchema().Types[SchemaConstants.Group], UnitTestControl.TestParameters);

                        if (result.ErrorCode != MAExportError.Success)
                        {
                            Assert.Fail(result.ErrorName);
                        }
                    }
                    finally
                    {
                        Interlocked.Decrement(ref threadCount);
                    }
                });
            }
            finally
            {
                this.Cleanup(groups.ToArray<object>());
                ConnectionPools.InitializePools(UnitTestControl.TestParameters.Credentials, 1, 1, 1, 1);
            }
        }

        private static CSEntryChange CreateCSEntryUpdate(Group e)
        {
            CSEntryChange cs = CSEntryChange.Create();
            cs.ObjectModificationType = ObjectModificationType.Update;
            cs.DN = e.Email;
            cs.ObjectType = SchemaConstants.Group;
            cs.AnchorAttributes.Add(AnchorAttribute.Create("id", e.Id));

            return cs;
        }

        [TestMethod]
        public void RemoveMember()
        {
            Group e = null;
            Group member1 = null;
            Group member2 = null;

            try
            {
                e = this.CreateGroup();
                member1 = this.CreateGroup();
                member2 = this.CreateGroup();
                GroupMemberRequestFactory.AddMember(e.Email, new Member() { Email = member1.Email });
                GroupMemberRequestFactory.AddMember(e.Email, new Member() { Email = member2.Email });

                System.Threading.Thread.Sleep(1000);

                CSEntryChange cs = CSEntryChange.Create();
                cs.ObjectModificationType = ObjectModificationType.Update;
                cs.DN = e.Email;
                cs.ObjectType = SchemaConstants.Group;
                cs.AnchorAttributes.Add(AnchorAttribute.Create("id", e.Id));

                cs.AttributeChanges.Add(AttributeChange.CreateAttributeUpdate("member", new List<ValueChange>() { new ValueChange(member2.Email, ValueModificationType.Delete) }));

                CSEntryChangeResult result =
                    ExportProcessor.PutCSEntryChange(cs, UnitTestControl.Schema.GetSchema().Types[SchemaConstants.Group], UnitTestControl.TestParameters);

                if (result.ErrorCode != MAExportError.Success)
                {
                    Assert.Fail(result.ErrorName);
                }

                System.Threading.Thread.Sleep(1000);

                CollectionAssert.AreEquivalent(new string[] { member1.Email }, GroupMemberRequestFactory.GetMembership(cs.DN).Members.ToArray());
            }
            finally
            {
                this.Cleanup(e, member1, member2);
            }
        }

        [TestMethod]
        public void RemoveMembers()
        {
            Group e = null;
            Group member1 = null;
            Group member2 = null;

            try
            {
                e = this.CreateGroup();
                member1 = this.CreateGroup();
                member2 = this.CreateGroup();

                GroupMemberRequestFactory.AddMember(e.Email, new Member() { Email = member1.Email });
                GroupMemberRequestFactory.AddMember(e.Email, new Member() { Email = member2.Email });

                System.Threading.Thread.Sleep(1000);

                CSEntryChange cs = CSEntryChange.Create();
                cs.ObjectModificationType = ObjectModificationType.Update;
                cs.DN = e.Email;
                cs.ObjectType = SchemaConstants.Group;
                cs.AnchorAttributes.Add(AnchorAttribute.Create("id", e.Id));

                cs.AttributeChanges.Add(AttributeChange.CreateAttributeDelete("member"));

                CSEntryChangeResult result = ExportProcessor.PutCSEntryChange(cs, UnitTestControl.Schema.GetSchema().Types[SchemaConstants.Group], UnitTestControl.TestParameters);

                if (result.ErrorCode != MAExportError.Success)
                {
                    Assert.Fail(result.ErrorName);
                }

                System.Threading.Thread.Sleep(1000);

                Assert.AreEqual(0, GroupMemberRequestFactory.GetMembership(cs.DN).Members.Count);
            }
            finally
            {
                this.Cleanup(e, member1, member2);
            }
        }

        [TestMethod]
        public void ReplaceMembers()
        {
            Group e = null;
            Group member1 = null;
            Group member2 = null;
            Group member3 = null;
            Group member4 = null;

            try
            {
                e = this.CreateGroup();

                CSEntryChange cs = CSEntryChange.Create();
                cs.ObjectModificationType = ObjectModificationType.Update;
                cs.DN = e.Email;
                cs.ObjectType = SchemaConstants.Group;
                cs.AnchorAttributes.Add(AnchorAttribute.Create("id", e.Id));

                member1 = this.CreateGroup();
                member2 = this.CreateGroup();
                member3 = this.CreateGroup();
                member4 = this.CreateGroup();
                GroupMemberRequestFactory.AddMember(e.Email, new Member() { Email = member1.Email });
                GroupMemberRequestFactory.AddMember(e.Email, new Member() { Email = member2.Email });

                System.Threading.Thread.Sleep(1000);

                cs.AttributeChanges.Add(AttributeChange.CreateAttributeReplace("member", new List<object>() { member3.Email, member4.Email }));


                CSEntryChangeResult result =
                    ExportProcessor.PutCSEntryChange(cs, UnitTestControl.Schema.GetSchema().Types[SchemaConstants.Group], UnitTestControl.TestParameters);

                if (result.ErrorCode != MAExportError.Success)
                {
                    Assert.Fail(result.ErrorName);
                }

                System.Threading.Thread.Sleep(1000);

                CollectionAssert.AreEquivalent(new string[] { member3.Email, member4.Email }, GroupMemberRequestFactory.GetMembership(cs.DN).Members.ToArray());
            }
            finally
            {
                this.Cleanup(e, member1, member2, member3, member4);
            }

        }

        private void Cleanup(params object[] objects)
        {
            if (objects == null)
            {
                return;
            }

            foreach (Group g in objects.OfType<Group>())
            {
                if (g != null)
                {
                    GroupRequestFactory.Delete(g.Id);
                }
            }

            foreach (User u in objects.OfType<User>())
            {
                if (u != null)
                {
                    UserRequestFactory.Delete(u.Id);
                }
            }
        }

        private Group CreateGroup()
        {
            string dn = $"{Guid.NewGuid()}@{UnitTestControl.TestParameters.Domain}";
            Group e = new Group
            {
                Email = dn,
                Name = Guid.NewGuid().ToString()
            };

            e = GroupRequestFactory.Add(e);

            return e;
        }
    }
}

