using System.Configuration;

namespace Lithnet.GoogleApps.MA
{
    internal class ClassroomApiElement : ConfigurationElement
    {
        private const string PropRateLimit = "rate-limit";
        private const string PropPoolSize = "pool-size";
        private const string PropImportThreadsCourseMember = "import-threads-course-member";

        [ConfigurationProperty(ClassroomApiElement.PropRateLimit, IsRequired = false, DefaultValue = 5)]
        public int RateLimit
        {
            get
            {
                return (int)this[ClassroomApiElement.PropRateLimit];
            }
            set
            {
                this[ClassroomApiElement.PropRateLimit] = value;
            }
        }

        [ConfigurationProperty(ClassroomApiElement.PropPoolSize, IsRequired = false, DefaultValue = 30)]
        public int PoolSize
        {
            get
            {
                return (int)this[ClassroomApiElement.PropPoolSize];
            }
            set
            {
                this[ClassroomApiElement.PropPoolSize] = value;
            }
        }

        [ConfigurationProperty(PropImportThreadsCourseMember, IsRequired = false, DefaultValue = 10)]
        public int ImportThreadsCourseMember
        {
            get
            {
                return (int)this[ClassroomApiElement.PropImportThreadsCourseMember];
            }
            set
            {
                this[ClassroomApiElement.PropImportThreadsCourseMember] = value;
            }
        }
    }
}