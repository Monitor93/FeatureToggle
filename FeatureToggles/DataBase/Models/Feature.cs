using FeatureToggle.DataBase.Abstract;
using FeatureToggle.TransferObjects;

namespace FeatureToggle.DataBase.Models
{
    /// <summary>
    /// Доменная модель фичи
    /// </summary>
    class Feature : DbObject<string>
    {
        /// <summary>
        /// Значение фичи
        /// </summary>
        public bool Value { get; set; }

        /// <summary>
        /// Базовый конструктор
        /// </summary>
        public Feature() { }

        /// <summary>
        /// Создаёт доменную модуль на основе транспортного объекта
        /// </summary>
        /// <param name="dto">Транспортный объект фичи</param>
        public Feature(FeatureDto dto)
        {
            Id = dto.Key;
            Value = dto.Value;
        }
    }
}