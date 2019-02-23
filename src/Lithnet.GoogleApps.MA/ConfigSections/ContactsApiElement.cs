using System.Configuration;

namespace Lithnet.GoogleApps.MA
{
    internal class ContactsApiElement : ConfigurationElement
    {
        private const string PropRateLimit = "rate-limit";
        private const string PropPoolSize = "pool-size";

        [ConfigurationProperty(ContactsApiElement.PropRateLimit, IsRequired = false, DefaultValue = 1500)]
        public int RateLimit
        {
            get
            {
                return (int)this[ContactsApiElement.PropRateLimit];
            }
            set
            {
                this[ContactsApiElement.PropRateLimit] = value;
            }
        }

        [ConfigurationProperty(ContactsApiElement.PropPoolSize, IsRequired = false, DefaultValue = 30)]
        public int PoolSize
        {
            get
            {
                return (int)this[ContactsApiElement.PropPoolSize];
            }
            set
            {
                this[ContactsApiElement.PropPoolSize] = value;
            }
        }
    }
}