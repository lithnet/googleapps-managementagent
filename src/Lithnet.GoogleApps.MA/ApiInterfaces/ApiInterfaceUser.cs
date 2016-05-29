using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lithnet.GoogleApps.MA
{
    using System.Security.Cryptography;
    using ManagedObjects;
    using MetadirectoryServices;
    using Microsoft.MetadirectoryServices;

    internal class ApiInterfaceUser : IApiInterfaceObject
    {
        protected ApiInterfaceKeyedCollection InternalInterfaces { get; private set; }

        private static RNGCryptoServiceProvider cryptoProvider = new RNGCryptoServiceProvider();

        protected string objectClass;

        protected MASchemaType SchemaType { get; set; }

        public ApiInterfaceUser(MASchemaType type)
        {
            this.InternalInterfaces = new ApiInterfaceKeyedCollection { new ApiInterfaceUserAliases(), new ApiInterfaceUserMakeAdmin() };
            this.objectClass = SchemaConstants.User;
            this.SchemaType = type;
        }

        public virtual string Api => "user";

        public virtual object CreateInstance(CSEntryChange csentry)
        {
            if (csentry.ObjectModificationType == ObjectModificationType.Add)
            {
                return new User
                {
                    Password = ApiInterfaceUser.GenerateSecureString(60),
                    PrimaryEmail = csentry.DN
                };
            }
            else
            {
                return new User
                {
                    Id = csentry.GetAnchorValueOrDefault<string>(this.SchemaType.AnchorAttributeName),
                    PrimaryEmail = csentry.DN
                };
            }
        }

        public object GetInstance(CSEntryChange csentry)
        {
            return UserRequestFactory.Get(csentry.GetAnchorValueOrDefault<string>("id") ?? csentry.DN);
        }

        public void DeleteInstance(CSEntryChange csentry)
        {
            UserRequestFactory.Delete(csentry.GetAnchorValueOrDefault<string>("id") ?? csentry.DN);
        }

        public IList<AttributeChange> ApplyChanges(CSEntryChange csentry, SchemaType type, ref object target, bool patch = false)
        {
            bool hasChanged = false;

            List<AttributeChange> changes = new List<AttributeChange>();

            User user = target as User;

            if (user == null)
            {
                throw new InvalidOperationException();
            }

            if (ApiInterfaceUser.SetDNValue(csentry, user))
            {
                hasChanged = true;
            }

            foreach (IAttributeAdapter typeDef in this.SchemaType.Attributes.Where(t => t.Api == this.Api))
            {
                if (typeDef.UpdateField(csentry, target))
                {
                    hasChanged = true;
                }
            }

            if (hasChanged)
            {
                User result;

                if (csentry.ObjectModificationType == ObjectModificationType.Add)
                {
                    result = UserRequestFactory.Add(user);
                    target = result;
                }
                else if (csentry.ObjectModificationType == ObjectModificationType.Replace || csentry.ObjectModificationType == ObjectModificationType.Update)
                {
                    string id = csentry.GetAnchorValueOrDefault<string>(this.SchemaType.AnchorAttributeName);

                    if (patch)
                    {
                        result = UserRequestFactory.Patch(user, id);
                    }
                    else
                    {
                        result = UserRequestFactory.Update(user, id);
                    }

                    target = result;
                }
                else
                {
                    throw new InvalidOperationException();
                }

                changes.AddRange(this.GetLocalChanges(csentry.DN, csentry.ObjectModificationType, type, result));
            }

            foreach (IApiInterface i in this.InternalInterfaces)
            {
                foreach (AttributeChange c in i.ApplyChanges(csentry, type, ref target, patch))
                {
                    changes.Add(c);
                }
            }

            return changes;
        }

        public IList<AttributeChange> GetChanges(string dn, ObjectModificationType modType, SchemaType type, object source)
        {
            List<AttributeChange> attributeChanges = this.GetLocalChanges(dn, modType, type, source);

            foreach (IApiInterface i in this.InternalInterfaces)
            {
                attributeChanges.AddRange(i.GetChanges(dn, modType, type, source));
            }

            return attributeChanges;
        }

        private List<AttributeChange> GetLocalChanges(string dn, ObjectModificationType modType, SchemaType type, object source)
        {
            List<AttributeChange> attributeChanges = new List<AttributeChange>();

            foreach (IAttributeAdapter typeDef in this.SchemaType.Attributes.Where(t => t.Api == this.Api))
            {
                if (typeDef.IsAnchor)
                {
                    continue;
                }

                foreach (AttributeChange change in typeDef.CreateAttributeChanges(dn, modType, source))
                {
                    if (type.HasAttribute(change.Name))
                    {
                        attributeChanges.Add(change);
                    }
                }
            }
            return attributeChanges;
        }

        public string GetAnchorValue(object target)
        {
            return ((User)target).Id;
        }

        public string GetDNValue(object target)
        {
            return ((User)target).PrimaryEmail;
        }

        private static bool SetDNValue(CSEntryChange csentry, User e)
        {
            if (csentry.ObjectModificationType != ObjectModificationType.Replace && csentry.ObjectModificationType != ObjectModificationType.Update)
            {
                return false;
            }

            string newDN = csentry.GetNewDNOrDefault<string>();

            if (newDN == null)
            {
                return false;
            }

            e.PrimaryEmail = newDN;

            return true;
        }

        protected static string GenerateSecureString(int length, string alphabet = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!@#$%^&*()_+{}:'/?-")
        {
            int outOfRange = byte.MaxValue + 1 - (byte.MaxValue + 1) % alphabet.Length;

            return string.Concat(
                Enumerable
                    .Repeat(0, int.MaxValue)
                    .Select(e => ApiInterfaceUser.RandomByte())
                    .Where(randomByte => randomByte < outOfRange)
                    .Take(length)
                    .Select(randomByte => alphabet[randomByte % alphabet.Length])
            );
        }

        private static byte RandomByte()
        {
            byte[] randomBytes = new byte[1];
            ApiInterfaceUser.cryptoProvider.GetBytes(randomBytes);
            return randomBytes.Single();
        }
    }
}
