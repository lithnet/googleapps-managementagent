﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Lithnet.GoogleApps.ManagedObjects;
using Lithnet.Logging;
using Lithnet.MetadirectoryServices;
using Microsoft.MetadirectoryServices;

namespace Lithnet.GoogleApps.MA
{
    internal class ApiInterfaceUser : IApiInterfaceObject
    {
        protected ApiInterfaceKeyedCollection InternalInterfaces { get; private set; }

        private static RNGCryptoServiceProvider cryptoProvider = new RNGCryptoServiceProvider();

        protected string ObjectClass;

        protected internal MASchemaType SchemaType { get; set; }

        protected internal IManagementAgentParameters config;

        public ApiInterfaceUser(MASchemaType type, IManagementAgentParameters config)
        {
            this.InternalInterfaces = new ApiInterfaceKeyedCollection
            {
                new ApiInterfaceUserAliases(config),
                new ApiInterfaceUserMakeAdmin(config),
                new ApiInterfaceUserDelegates(config),
                new ApiInterfaceUserSendAs(config)
            };

            this.ObjectClass = SchemaConstants.User;
            this.SchemaType = type;
            this.config = config;
        }

        public virtual string Api => "user";

        public ObjectModificationType DeltaUpdateType => ObjectModificationType.Update;

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
                    Id = csentry.GetAnchorValueOrDefault<string>("id"),
                    PrimaryEmail = csentry.DN
                };
            }
        }

        public object GetInstance(CSEntryChange csentry)
        {
            return this.config.UsersService.Get(csentry.GetAnchorValueOrDefault<string>("id") ?? csentry.DN);
        }

        public void DeleteInstance(CSEntryChange csentry)
        {
            this.config.UsersService.Delete(csentry.GetAnchorValueOrDefault<string>("id") ?? csentry.DN);
        }

        public IList<AttributeChange> ApplyChanges(CSEntryChange csentry, SchemaType type, ref object target, bool patch = false)
        {
            bool hasChanged = false;

            List<AttributeChange> changes = new List<AttributeChange>();

            if (!(target is User user))
            {
                throw new InvalidOperationException();
            }

            if (ApiInterfaceUser.SetDNValue(csentry, user))
            {
                hasChanged = true;
            }

            foreach (IAttributeAdapter typeDef in this.SchemaType.AttributeAdapters.Where(t => t.Api == this.Api))
            {
                if (typeDef.UpdateField(csentry, target))
                {
                    hasChanged = true;
                }
            }

            if (hasChanged)
            {
                Trace.WriteLine($"Object {csentry.DN} has one or more changes to commit");

                User result;

                if (csentry.ObjectModificationType == ObjectModificationType.Add)
                {
                    result = this.config.UsersService.Add(user);
                    target = result;
                }
                else if (csentry.ObjectModificationType == ObjectModificationType.Replace || csentry.ObjectModificationType == ObjectModificationType.Update)
                {
                    string id = csentry.GetAnchorValueOrDefault<string>("id");

                    if (patch)
                    {
                        result = this.config.UsersService.Patch(user, id);
                    }
                    else
                    {
                        result = this.config.UsersService.Update(user, id);
                    }

                    target = result;
                }
                else
                {
                    throw new InvalidOperationException();
                }

                changes.AddRange(this.GetLocalChanges(csentry.DN, csentry.ObjectModificationType, type, result));
            }
            else
            {
                Trace.WriteLine($"Object {csentry.DN} has no changes to commit");
            }

            foreach (IApiInterface i in this.InternalInterfaces)
            {
                foreach (AttributeChange c in i.ApplyChanges(csentry, type, ref target, patch))
                {
                    changes.Add(c);
                }
            }

            this.AddMissingDeletes(changes, csentry);

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

            foreach (IAttributeAdapter typeDef in this.SchemaType.AttributeAdapters.Where(t => t.Api == this.Api))
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

        private void AddMissingDeletes(List<AttributeChange> deltaChanges, CSEntryChange csentry)
        {
            // This is a workaround for an issue where when we delete the last value from a CustomTypeListT, we do not see the change
            // come in when we parse the updated object from google, as the value is null. There is no way to tell if it is null because
            // it was deleted, or never present, making it difficult to send an appropriate 'delete' value back to FIM. The workaround is to
            // replay any 'deletes' from the original CSEntryChange back in the delta, provided that one of the API interfaces hasn't already
            // contributed an AttributeChange for it.

            foreach (AttributeChange change in csentry.AttributeChanges.Where(t => t.ModificationType == AttributeModificationType.Delete))
            {
                if (deltaChanges.All(t => t.Name != change.Name))
                {
                    deltaChanges.Add(change);
                }
            }
        }

        public string GetAnchorValue(string name, object target)
        {
            return ((User)target).Id;
        }

        public string GetDNValue(object target)
        {
            return ((User)target).PrimaryEmail;
        }

        public Task GetItems(Schema schema, BlockingCollection<object> collection)
        {
            if (this.GetType() == typeof(ApiInterfaceAdvancedUser))
            {
                if (schema.Types.Contains(SchemaConstants.User))
                {
                    // This function doesn't need to run for advanced users, if the user class will be called.
                    return null;
                }
            }

            HashSet<string> fieldNames = new HashSet<string>
            {
                SchemaConstants.PrimaryEmail,
                SchemaConstants.ID
            };

            if (schema.Types.Contains(SchemaConstants.User))
            {
                foreach (string field in ManagementAgent.Schema[SchemaConstants.User].GetFieldNames(schema.Types[SchemaConstants.User]))
                {
                    fieldNames.Add(field);
                }
            }

            if (schema.Types.Contains(SchemaConstants.AdvancedUser))
            {
                foreach (string field in ManagementAgent.Schema[SchemaConstants.AdvancedUser].GetFieldNames(schema.Types[SchemaConstants.AdvancedUser]))
                {
                    fieldNames.Add(field);
                }
            }

            if (schema.Types.Contains(SchemaConstants.AdvancedUser))
            {
                fieldNames.Add($"customSchemas/{SchemaConstants.CustomGoogleAppsSchemaName}");
            }

            string fields = $"users({string.Join(",", fieldNames)}),nextPageToken";

            Task t = new Task(() =>
            {
                Logger.WriteLine("Starting user import task");
                Logger.WriteLine("Requesting fields: " + fields);
                Logger.WriteLine("Query filter: " + (this.config.UserQueryFilter ?? "<none>"));
                SchemaType type = schema.Types[SchemaConstants.User];

                foreach (User user in this.config.UsersService.GetUsers(this.config.CustomerID, fields, this.config.UserQueryFilter))
                {
                    if (!string.IsNullOrWhiteSpace(this.config.UserRegexFilter))
                    {
                        if (!Regex.IsMatch(user.PrimaryEmail, this.config.UserRegexFilter, RegexOptions.IgnoreCase))
                        {
                            continue;
                        }
                    }

                    if (user.CustomSchemas != null)
                    {
                        if (user.CustomSchemas.ContainsKey(SchemaConstants.CustomGoogleAppsSchemaName))
                        {
                            if (user.CustomSchemas[SchemaConstants.CustomGoogleAppsSchemaName].ContainsKey(SchemaConstants.CustomSchemaObjectType))
                            {
                                string objectType = (string)user.CustomSchemas[SchemaConstants.CustomGoogleAppsSchemaName][SchemaConstants.CustomSchemaObjectType];
                                if (schema.Types.Contains(objectType))
                                {
                                    type = schema.Types[objectType];
                                }
                            }
                        }
                    }

                    collection.Add(ImportProcessor.GetCSEntryChange(user, type, this.config));
                    continue;
                }

                Logger.WriteLine("User import task complete");

            });

            t.Start();

            return t;
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
