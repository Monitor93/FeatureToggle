using System.Configuration;

namespace FeatureToggle.Config
{
    /// <summary>
    /// Секция конфигурации FeatureToggle
    /// </summary>
    public class FeatureToggleConfigurationSection : ConfigurationSection
    {
        /// <summary>
        /// Соединение с базой
        /// </summary>
        [ConfigurationProperty("ConnectionStringName")]
        public ValueConfigElement ConnectionStringName => (ValueConfigElement)this["ConnectionStringName"];

        /// <summary>
        /// Размер строковых полей в базе
        /// </summary>
        [ConfigurationProperty("VarcharSize")]
        public ValueConfigElement VarcharSize => (ValueConfigElement)this["VarcharSize"];

        /// <summary>
        /// Время жизни записей в кэше
        /// </summary>
        [ConfigurationProperty("CasheLifeTime")]
        public ValueConfigElement CasheLifeTime => (ValueConfigElement)this["CasheLifeTime"];
    }
}
