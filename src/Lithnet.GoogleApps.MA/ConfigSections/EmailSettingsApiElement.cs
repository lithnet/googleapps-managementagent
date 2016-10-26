using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace Lithnet.GoogleApps.MA
{
    internal class EmailSettingsApiElement : ConfigurationElement 
    {
        private const string PropRateLimit = "rate-limit";
        private const string PropPoolSize = "pool-size";

        [ConfigurationProperty(EmailSettingsApiElement.PropRateLimit, IsRequired = false, DefaultValue = 1500)]
        public int RateLimit
        {
            get
            {
                return (int)this[EmailSettingsApiElement.PropRateLimit];
            }
            set
            {
                this[EmailSettingsApiElement.PropRateLimit] = value;
            }
        }

        [ConfigurationProperty(EmailSettingsApiElement.PropPoolSize, IsRequired = false, DefaultValue = 30)]
        public int PoolSize
        {
            get
            {
                return (int)this[EmailSettingsApiElement.PropPoolSize];
            }
            set
            {
                this[EmailSettingsApiElement.PropPoolSize] = value;
            }
        }
    }
}