using FeatureToggle.DataBase.Abstract;

namespace FeatureToggle.DataBase.Models
{
    /// <summary>
    /// Контекст фичи
    /// </summary>
    class Context : DbObject<string>
    {
        /// <summary>
        /// Базовый конструктор
        /// </summary>
        public Context() { }

        /// <summary>
        /// Создаёт новый контекст с заданным именем
        /// </summary>
        /// <param name="name">Имя контекста</param>
        public Context(string name)
        {
            Id = name;
        }
    }
}
