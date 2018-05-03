using System;
using System.Configuration;

namespace FeatureToggle.Config
{
    /// <summary>
    /// Класс конфигурации
    /// </summary>
    public static class FeatureToggleConfiguration
    {
        /// <summary>
        /// Подключение базы данных
        /// </summary>
        public static string ConnectionString
        {
            get
            {
                var connectionStringName = Section.ConnectionStringName.Value;
                return string.IsNullOrWhiteSpace(connectionStringName)
                    ? ConfigurationManager.ConnectionStrings[1].ConnectionString
                    : ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
            }
        }

        /// <summary>
        /// Размер строк в Sql
        /// </summary>
        public static int VarcharSize
        {
            get
            {
                int value;
                if (int.TryParse(Section.VarcharSize.Value, out value))
                {
                    return value;
                }
                return 256;
            }
        }

        /// <summary>
        /// Время жизни значения фичи в кэше
        /// </summary>
        public static TimeSpan CasheLifeTime
        {
            get
            {
                TimeSpan value;
                if (TimeSpan.TryParse(Section.CasheLifeTime.Value, out value))
                {
                    return value;
                }
                return new TimeSpan(1, 0, 0);
            }
        }

        private static FeatureToggleConfigurationSection Section => (FeatureToggleConfigurationSection)ConfigurationManager.GetSection("featureToggle");
    }
}
