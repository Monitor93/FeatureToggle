using FeatureToggle.DataBase.Models;
using System.Collections.Generic;
using System.Linq;

namespace FeatureToggle.TransferObjects
{
    /// <summary>
    /// Фича
    /// </summary>
    public class FeatureDto
    {
        /// <summary>
        /// Ключ фичи
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Включена ли фича
        /// </summary>
        public bool Value { get; set; }

        /// <summary>
        /// Контексты фичи
        /// </summary>
        public List<FeatureContextDto> Contexts { get; set; }

        /// <summary>
        /// Контекст <c>context</c> фичи
        /// </summary>
        public FeatureContextDto this[string context]
        {
            get
            {
                return Contexts.FirstOrDefault(x => x.ContextName == context);
            }
        }

        /// <summary>
        /// Создаёт новую фитчу без контекста
        /// </summary>
        public FeatureDto()
        {
            Contexts = new List<FeatureContextDto>();
        }

        /// <summary>
        /// Создаёт новую фитчу без контекста
        /// </summary>
        /// <param name="key">Ключ фичи</param>
        /// <param name="value">Значение фичи</param>
        public FeatureDto(string key, bool value)
        {
            Key = key;
            Value = value;
            Contexts = new List<FeatureContextDto>();
        }

        /// <summary>
        /// Создаёт новую фичу с контекстом
        /// </summary>
        /// <param name="key">Ключ фичи</param>
        /// <param name="defaultValue">Значение фичи по умолчанию без учёта контекста</param>
        /// <param name="context">Контекст</param>
        public FeatureDto(string key, bool defaultValue, IEnumerable<FeatureContextDto> context)
        {
            Key = key;
            Value = defaultValue;
            Contexts = context.ToList();
        }

        /// <summary>
        /// Создаёт транспортный объект фичи на основе доменной модели 
        /// </summary>
        /// <param name="feature">Доменная модель фичи</param>
        internal FeatureDto(Feature feature)
        {
            Key = feature.Id;
            Value = feature.Value;
            Contexts = new List<FeatureContextDto>();
        }

        /// <summary>
        /// Создаёт транспортный объект фичи с контекстом на основе доменных моелей
        /// </summary>
        /// <param name="feature">Доменная модель фичи</param>
        /// <param name="context">Доменные модели контекстов</param>
        internal FeatureDto(Feature feature, IEnumerable<FeatureToContext> context)
        {
            Key = feature.Id;
            Value = feature.Value;
            Contexts = context
                .GroupBy(x => x.Context)
                .Select(x => new FeatureContextDto(x.Key, x.ToList()))
                .ToList();
        }
    }
}
