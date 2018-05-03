using FeatureToggle.Enums;
using FeatureToggle.TransferObjects;

namespace FeatureToggle
{
    public interface IFeatureToggle
    {
        /// <summary>
        /// Создаёт новую фичу или обновляет информацию о существующей
        /// </summary>
        /// <param name="feature">Фича</param>
        void CreateOrUpdateFeature(FeatureDto feature);

        /// <summary>
        /// Получить полную информацию о фиче по ключу
        /// </summary>
        /// <param name="key">Ключ</param>
        /// <returns>Фича</returns>
        FeatureDto GetFeature(string key);

        /// <summary>
        /// Проверить функциональность фичи
        /// </summary>
        /// <param name="key">Ключ фичи</param>
        /// <param name="throws">Бросить исключение если фича не найдена</param>
        /// <returns>Включена ли фича</returns>
        bool IsEnable(string key, DefaultFeatureValue defaultValue = DefaultFeatureValue.False);

        /// <summary>
        /// Проверить функциональность фичи с учётом контекста
        /// </summary>
        /// <param name="key">ФИчва</param>
        /// <param name="context"></param>
        /// <param name="param"></param>
        /// <param name="defaultValue">Результат в случае если фича не найдена</param>
        /// <returns></returns>
        bool IsEnable(string key, string context, string param, DefaultFeatureValue defaultValue = DefaultFeatureValue.False);

        /// <summary>
        /// Удаляет фичу
        /// </summary>
        /// <param name="key">Ключ фичи</param>
        void DeleteFeature(string key);

        /// <summary>
        /// Удаляет контекст у заданной фичи
        /// </summary>
        /// <param name="context">Удаляемый контекст</param>
        /// <param name="feature">Фича у которой надо удалить контекст</param>
        void DeleteContext(string context, string feature);

        /// <summary>
        /// Удаляет контекст у заданной фичи с заданным параметром
        /// </summary>
        /// <param name="context">Удаляемый контекст</param>
        /// <param name="feature">Фича у которой надо удалить контекст</param>
        /// <param name="param">Параметр который будет удалён</param>
        void DeleteContext(string context, string feature, string param);
    }
}
