using FeatureToggle.Attributes;
using FeatureToggle.DataBase.Abstract;

namespace FeatureToggle.DataBase.Models
{
    /// <summary>
    /// Доменная модель связи фичи и контекстов
    /// </summary>
    class FeatureToContext : DbObject<int>
    {
        /// <summary>
        /// Первичный ключ
        /// </summary>
        [Identity]
        public override int Id { get; set; }

        /// <summary>
        /// Фича
        /// </summary>
        [ForeignKey(typeof(Feature))]
        public string Feature { get; set; }

        /// <summary>
        /// Контекст
        /// </summary>
        [ForeignKey(typeof(Context))]
        public string Context { get; set; }

        /// <summary>
        /// Параметр контекста
        /// </summary>
        public string Param { get; set; }

        /// <summary>
        /// Включена ли фича с заданным контекстом в текущем значении параметра контекста
        /// </summary>
        public bool IsEnable { get; set; }
    }
}
