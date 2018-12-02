using System.Configuration;

namespace Lithnet.GoogleApps.MA
{
    internal class GmailApiElement : ConfigurationElement 
    {
        private const string PropRateLimit = "rate-limit";

        [ConfigurationProperty(GmailApiElement.PropRateLimit, IsRequired = false, DefaultValue = 250)]
        public int RateLimit
        {
            get => (int)this[GmailApiElement.PropRateLimit];
            set => this[GmailApiElement.PropRateLimit] = value;
        }
    }
}