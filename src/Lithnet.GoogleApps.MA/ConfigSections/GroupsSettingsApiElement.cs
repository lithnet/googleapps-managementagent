using System.Configuration;

namespace Lithnet.GoogleApps.MA
{
    internal class GroupsSettingsApiElement : ConfigurationElement
    {
        private const string PropRateLimit = "rate-limit";
        private const string PropPoolSize = "pool-size";
        private const string PropImportThreadsGroupSettings = "import-threads-group-settings";

        [ConfigurationProperty(GroupsSettingsApiElement.PropRateLimit, IsRequired = false, DefaultValue = 500)]
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

        [ConfigurationProperty(PropImportThreadsGroupSettings, IsRequired = false, DefaultValue = 30)]
        public int ImportThreadsGroupSettings
        {
            get
            {
                return (int)this[GroupsSettingsApiElement.PropImportThreadsGroupSettings];
            }
            set
            {
                this[GroupsSettingsApiElement.PropImportThreadsGroupSettings] = value;
            }
        }
    }
}