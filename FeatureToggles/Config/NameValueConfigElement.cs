using System.Configuration;

namespace FeatureToggle.Config
{
    /// <summary>
    /// Элемент конфигурации со значением
    /// </summary>
    public class ValueConfigElement : ConfigurationElement
    {
        /// <summary>
        /// Значение элемента конфигурации
        /// </summary>
        [ConfigurationProperty("value", IsRequired = false)]
        public string Value => this["value"] as string;
    }
}
