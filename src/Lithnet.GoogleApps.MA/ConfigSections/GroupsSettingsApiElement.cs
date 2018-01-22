using System.Configuration;

namespace Lithnet.GoogleApps.MA
{
    internal class GroupsSettingsApiElement : ConfigurationElement 
    {
        private const string PropRateLimit = "rate-limit";
        private const string PropPoolSize = "pool-size";

        [ConfigurationProperty(GroupsSettingsApiElement.PropRateLimit, IsRequired = false, DefaultValue = 1500)]
        public int RateLimit
        {
            get
            {
                return (int)this[GroupsSettingsApiElement.PropRateLimit];
            }
            set
            {
                this[GroupsSettingsApiElement.PropRateLimit] = value;
            }
        }

        [ConfigurationProperty(GroupsSettingsApiElement.PropPoolSize, IsRequired = false, DefaultValue = 30)]
        public int PoolSize
        {
            get
            {
                return (int)this[GroupsSettingsApiElement.PropPoolSize];
            }
            set
            {
                this[GroupsSettingsApiElement.PropPoolSize] = value;
            }
        }
    }
}