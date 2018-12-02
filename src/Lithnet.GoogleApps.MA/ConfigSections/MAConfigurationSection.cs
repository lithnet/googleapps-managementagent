using System.Configuration;

namespace Lithnet.GoogleApps.MA
{
    internal class MAConfigurationSection : ConfigurationSection
    {
        private const string SectionName = "lithnet-google-ma";
        private const string PropDirectoryApi = "directory-api";
        private const string PropGroupsSettingsApi = "groupssettings-api";
        private const string PropEmailSettingsApi = "emailsettings-api";
        private const string PropContactApi = "contacts-api";
        private const string PropCalendarApi = "calendar-api";
        private const string PropGmailApi = "gmail-api";
        private const string PropHttpDebugEnabled = "http-debug-enabled";
        private const string PropExportThreads = "export-threads";

        internal static MAConfigurationSection GetConfiguration()
        {
            MAConfigurationSection section = (MAConfigurationSection) ConfigurationManager.GetSection(SectionName);

            if (section == null)
            {
                section = new MAConfigurationSection();
            }

            return section;
        }

        internal static MAConfigurationSection Configuration { get; private set; }

        static MAConfigurationSection()
        {
            MAConfigurationSection.Configuration = MAConfigurationSection.GetConfiguration();
        }

        [ConfigurationProperty(MAConfigurationSection.PropHttpDebugEnabled, IsRequired = false, DefaultValue = false)]
        public bool HttpDebugEnabled => (bool)this[MAConfigurationSection.PropHttpDebugEnabled];

        [ConfigurationProperty(MAConfigurationSection.PropExportThreads, IsRequired = false, DefaultValue = 30)]
        public int ExportThreads => (int)this[MAConfigurationSection.PropExportThreads];
        
        [ConfigurationProperty(MAConfigurationSection.PropDirectoryApi, IsRequired = false)]
        public DirectoryApiElement DirectoryApi => (DirectoryApiElement) this[MAConfigurationSection.PropDirectoryApi];

        [ConfigurationProperty(MAConfigurationSection.PropGroupsSettingsApi, IsRequired = false)]
        public GroupsSettingsApiElement GroupSettingsApi => (GroupsSettingsApiElement)this[MAConfigurationSection.PropGroupsSettingsApi];

        [ConfigurationProperty(MAConfigurationSection.PropContactApi, IsRequired = false)]
        public ContactsApiElement ContactsApi => (ContactsApiElement)this[MAConfigurationSection.PropContactApi];

        [ConfigurationProperty(MAConfigurationSection.PropEmailSettingsApi, IsRequired = false)]
        public EmailSettingsApiElement EmailSettingsApi => (EmailSettingsApiElement)this[MAConfigurationSection.PropEmailSettingsApi];

        [ConfigurationProperty(MAConfigurationSection.PropCalendarApi, IsRequired = false)]
        public CalendarApiElement CalendarApi => (CalendarApiElement)this[MAConfigurationSection.PropCalendarApi];
        
        [ConfigurationProperty(MAConfigurationSection.PropGmailApi, IsRequired = false)]
        public GmailApiElement GmailApi => (GmailApiElement)this[MAConfigurationSection.PropGmailApi];
    }
}