using System.Configuration;

namespace Lithnet.GoogleApps.MA
{
    internal class CalendarApiElement : ConfigurationElement 
    {
        private const string PropRateLimit = "rate-limit";
        private const string PropPoolSize = "pool-size";

        [ConfigurationProperty(CalendarApiElement.PropRateLimit, IsRequired = false, DefaultValue = 1500)]
        public int RateLimit
        {
            get
            {
                return (int)this[CalendarApiElement.PropRateLimit];
            }
            set
            {
                this[CalendarApiElement.PropRateLimit] = value;
            }
        }

        [ConfigurationProperty(CalendarApiElement.PropPoolSize, IsRequired = false, DefaultValue = 30)]
        public int PoolSize
        {
            get
            {
                return (int)this[CalendarApiElement.PropPoolSize];
            }
            set
            {
                this[CalendarApiElement.PropPoolSize] = value;
            }
        }
    }
}