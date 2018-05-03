using System.Configuration;

namespace FeatureToggle.Config
{
    public class ValueConfigElement : ConfigurationElement
    {
        [ConfigurationProperty("value", IsRequired = false)]
        public string Value => this["value"] as string;
    }
}
