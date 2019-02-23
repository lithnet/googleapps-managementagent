    using System;
    using System.Configuration;

namespace Lithnet.GoogleApps.MA
{
    internal class MAConfigurationSection : ConfigurationSection
    {
        private const string SectionName = "lithnet-google-ma";
        private const string PropDirectoryApi = "directory-api";
        private const string PropGroupsSettingsApi = "groupssettings-api";
        private const string PropContactApi = "contacts-api";
        private const string PropCalendarApi = "calendar-api";
        private const string PropClassroomApi = "classroom-api";
        private const string PropGmailApi = "gmail-api";
        private const string PropHttpDebugEnabled = "http-debug-enabled";
        private const string PropExportThreads = "export-threads";
        private const string PropImportThreads = "import-threads";

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

        [ConfigurationProperty(MAConfigurationSection.PropImportThreads, IsRequired = false, DefaultValue = 50)]
        public int ImportThreads => (int)this[MAConfigurationSection.PropImportThreads];

        [ConfigurationProperty(MAConfigurationSection.PropDirectoryApi, IsRequired = false)]
        public DirectoryApiElement DirectoryApi => (DirectoryApiElement) this[MAConfigurationSection.PropDirectoryApi];

        [ConfigurationProperty(MAConfigurationSection.PropGroupsSettingsApi, IsRequired = false)]
        public GroupsSettingsApiElement GroupSettingsApi => (GroupsSettingsApiElement)this[MAConfigurationSection.PropGroupsSettingsApi];

        [ConfigurationProperty(MAConfigurationSection.PropContactApi, IsRequired = false)]
        public ContactsApiElement ContactsApi => (ContactsApiElement)this[MAConfigurationSection.PropContactApi];

        [ConfigurationProperty(MAConfigurationSection.PropCalendarApi, IsRequired = false)]
        public CalendarApiElement CalendarApi => (CalendarApiElement)this[MAConfigurationSection.PropCalendarApi];

        [ConfigurationProperty(MAConfigurationSection.PropGmailApi, IsRequired = false)]
        public GmailApiElement GmailApi => (GmailApiElement)this[MAConfigurationSection.PropGmailApi];

        [ConfigurationProperty(MAConfigurationSection.PropClassroomApi, IsRequired = false)]
        public ClassroomApiElement ClassroomApi => (ClassroomApiElement)this[MAConfigurationSection.PropClassroomApi];
    }
}