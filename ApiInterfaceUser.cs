using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lithnet.GoogleApps.MA
{
    using ManagedObjects;
    using MetadirectoryServices;
    using Microsoft.MetadirectoryServices;

    public class ApiInterfaceUser : ApiInterface
    {
        private static MASchemaType userType = SchemaBuilder.GetUserSchema();

        public ApiInterfaceUser()
        {
            this.Api = "user";
        }

        public override bool IsPrimary => true;

        public override object CreateInstance(CSEntryChange csentry)
        {
            if (csentry.ObjectModificationType == ObjectModificationType.Add)
            {
                return new User
                {
                    Password = Guid.NewGuid().ToString("B"),
                    PrimaryEmail = csentry.DN
                };
            }
            else
            {
                return new User
                {
                    Id = csentry.GetAnchorValueOrDefault<string>(ApiInterfaceUser.userType.AnchorAttributeName)
                };
            }
        }

        public override object GetInstance(CSEntryChange csentry)
        {
            return UserRequestFactory.Get(csentry.GetAnchorValueOrDefault<string>("id") ?? csentry.DN);
        }

        public override void DeleteInstance(CSEntryChange csentry)
        {
            UserRequestFactory.Delete(csentry.GetAnchorValueOrDefault<string>("id") ?? csentry.DN);
        }

        public override IList<AttributeChange> ApplyChanges(CSEntryChange csentry, SchemaType type, object target, bool patch=false)
        {
            bool hasChanged = false;

            foreach (IMASchemaAttribute typeDef in ApiInterfaceUser.userType.Attributes.Where(t => t.Api == this.Api))
            {
                if (typeDef.UpdateField(csentry, target))
                {
                    hasChanged = true;
                }
            }

            if (!hasChanged)
            {
                return new List<AttributeChange>();
            }

            User result;

            if (csentry.ObjectModificationType == ObjectModificationType.Add)
            {
                result = UserRequestFactory.Add((User)target);
            }
            else if (csentry.ObjectModificationType == ObjectModificationType.Replace || csentry.ObjectModificationType == ObjectModificationType.Update)
            {
                if (patch)
                {
                    result = UserRequestFactory.Patch((User) target, this.GetAnchorValue(target));
                }
                else
                {
                    result = UserRequestFactory.Update((User) target, this.GetAnchorValue(target));
                }
            }
            else
            {
                throw new InvalidOperationException();
            }

            return this.GetChanges(csentry.ObjectModificationType, type, result);
        }

        public override IList<AttributeChange> GetChanges(ObjectModificationType modType, SchemaType type, object source)
        {
            List<AttributeChange> attributeChanges = new List<AttributeChange>();

            foreach (IMASchemaAttribute typeDef in ApiInterfaceUser.userType.Attributes.Where(t => t.Api == this.Api))
            {
                if (type.HasAttribute(typeDef.AttributeName))
                {
                    attributeChanges.AddRange(typeDef.CreateAttributeChanges(modType, source));
                }
            }

            return attributeChanges;
        }

        public override string GetAnchorValue(object target)
        {
            return ((User)target).Id;
        }

        public override string GetDNValue(object target)
        {
            return ((User)target).PrimaryEmail;
        }
    }
}
