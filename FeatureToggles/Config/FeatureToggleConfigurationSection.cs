using System.Configuration;

namespace FeatureToggle.Config
{
    public class FeatureToggleConfigurationSection : ConfigurationSection
    {
        [ConfigurationProperty("ConnectionStringName")]
        public ValueConfigElement ConnectionStringName => (ValueConfigElement)this["ConnectionStringName"];
        [ConfigurationProperty("VarcharSize")]
        public ValueConfigElement VarcharSize => (ValueConfigElement)this["VarcharSize"];
        [ConfigurationProperty("CasheLifeTime")]
        public ValueConfigElement CasheLifeTime => (ValueConfigElement)this["CasheLifeTime"];
    }
}
