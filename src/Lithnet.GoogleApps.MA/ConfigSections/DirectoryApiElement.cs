using System.Configuration;

namespace Lithnet.GoogleApps.MA
{
    internal class DirectoryApiElement : ConfigurationElement
    {
        private const string PropRateLimit = "rate-limit";
        private const string PropPoolSize = "pool-size";
        private const string PropExportThreadsGroupMember = "export-threads-group-member";
        private const string PropImportThreadsGroupMember = "import-threads-group-member";
        private const string PropBatchSizeGroupMember = "batch-size-group-member";
        private const string PropConcurrentOperaionGroupMember = "concurrent-operations-group-member";

        [ConfigurationProperty(DirectoryApiElement.PropRateLimit, IsRequired = false, DefaultValue = 1500)]
        public int RateLimit
        {
            get
            {
                return (int)this[DirectoryApiElement.PropRateLimit];
            }
            set
            {
                this[DirectoryApiElement.PropRateLimit] = value;
            }
        }

        [ConfigurationProperty(DirectoryApiElement.PropPoolSize, IsRequired = false, DefaultValue = 30)]
        public int PoolSize
        {
            get
            {
                return (int)this[DirectoryApiElement.PropPoolSize];
            }
            set
            {
                this[DirectoryApiElement.PropPoolSize] = value;
            }
        }

        [ConfigurationProperty(PropExportThreadsGroupMember, IsRequired = false, DefaultValue = 5)]
        public int ExportThreadsGroupMember
        {
            get
            {
                return (int)this[DirectoryApiElement.PropExportThreadsGroupMember];
            }
            set
            {
                this[DirectoryApiElement.PropExportThreadsGroupMember] = value;
            }
        }

        [ConfigurationProperty(PropImportThreadsGroupMember, IsRequired = false, DefaultValue = 10)]
        public int ImportThreadsGroupMember
        {
            get
            {
                return (int)this[DirectoryApiElement.PropImportThreadsGroupMember];
            }
            set
            {
                this[DirectoryApiElement.PropImportThreadsGroupMember] = value;
            }
        }

        [ConfigurationProperty(PropBatchSizeGroupMember, IsRequired = false, DefaultValue = 100)]
        public int BatchSizeGroupMember
        {
            get
            {
                return (int)this[DirectoryApiElement.PropBatchSizeGroupMember];
            }
            set
            {
                this[DirectoryApiElement.PropBatchSizeGroupMember] = value;
            }
        }

        [ConfigurationProperty(PropConcurrentOperaionGroupMember, IsRequired = false, DefaultValue = 10)]
        public int ConcurrentOperationGroupMember
        {
            get
            {
                return (int)this[DirectoryApiElement.PropConcurrentOperaionGroupMember];
            }
            set
            {
                this[DirectoryApiElement.PropConcurrentOperaionGroupMember] = value;
            }
        }
    }
}