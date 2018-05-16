namespace FeatureToggle.DataBase.Abstract
{
    /// <summary>
    /// Базовый класс для сущностей в базе
    /// </summary>
    /// <typeparam name="T">Тип первичного ключа</typeparam>
    abstract class DbObject<T>
    {
        /// <summary>
        /// Первичный ключ
        /// </summary>
        public virtual T Id { get; set; }

        /// <summary>
        /// Конструктор по умолчанию
        /// </summary>
        protected DbObject() { }
    }
}
