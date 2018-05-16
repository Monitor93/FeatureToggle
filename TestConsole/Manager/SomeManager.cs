using System;
using FeatureToggle.Enums;
using FeatureToggle.TransferObjects;
using System.Collections.Generic;
using System.Threading;
using FeatureToggle;

namespace TestConsole.Manager
{
    /// <summary>
    /// Некий менеджер для работы с <see cref="IFeatureToggle"/>
    /// </summary>
    class SomeManager : ISomeManager
    {
        private readonly IFeatureToggle _featureToggle;

        public SomeManager(IFeatureToggle featureToggle)
        {
            _featureToggle = featureToggle;
        }

        /// <summary>
        /// Создаёт фичу в <see cref="IFeatureToggle"/>
        /// </summary>
        /// <param name="key">Ключ фичи</param>
        /// <param name="value">Значение фичи</param>
        public void AddFeature(string key, bool value)
        {
            _featureToggle.CreateOrUpdateFeature(new FeatureDto { Key = key, Value = value });
            WriteInConsoleByThread(string.Format("Create {0} is {1}", key, value));
        }

        /// <summary>
        /// Проверяет значение фичи и выводит результат в консоль
        /// </summary>
        /// <param name="key">Ключ фичи</param>
        public void GetFeature(string key)
        {
            WriteInConsoleByThread(string.Format("Get {0}: {1}", key, _featureToggle.IsEnable(key)));
        }

        /// <summary>
        /// Проверяет существует ли фича и возвращает её значение, иначе выбрасывает исключение
        /// </summary>
        /// <param name="key">Ключ фичи</param>
        public void CheakAndGetFeature(string key)
        {
            WriteInConsoleByThread(string.Format("Get {0}: {1}", key, _featureToggle.IsEnable(key, DefaultFeatureValue.Exception)));
        }

        /// <summary>
        /// Создаёт фичу с контекстом в <see cref="IFeatureToggle"/>
        /// </summary>
        /// <param name="key">Ключ фичи</param>
        /// <param name="value">Значение фичи по умолчанию</param>
        /// <param name="contexts">Контексты фичи</param>
        public void AddFeatureWithContext(string key, bool value, List<FeatureContextDto> contexts)
        {
            _featureToggle.CreateOrUpdateFeature(new FeatureDto(key, value, contexts));
            WriteInConsoleByThread(string.Format("Create {0} is {1} withContexts", key, value));
        }

        /// <summary>
        /// Проверяет значение фичи с контекстом и выводит результат в консоль
        /// </summary>
        /// <param name="key">Ключ фичи</param>
        /// <param name="context">Имяконтекста</param>
        /// <param name="param">Параметр контекста</param>
        public void GetFeature(string key, string context, string param)
        {
            WriteInConsoleByThread(string.Format("Get {0} {1} {2}: {3}", key, context, param, _featureToggle.IsEnable(key, context, param)));
        }

        /// <summary>
        /// Удаляет фичу из <see cref="IFeatureToggle"/>
        /// </summary>
        /// <param name="key">Ключ фичи</param>
        public void DeleteFeature(string key)
        {
            _featureToggle.DeleteFeature(key);
            WriteInConsoleByThread(string.Format("Feature {0} is delete", key));
        }

        /// <summary>
        /// Удаляет контекст у фичи
        /// </summary>
        /// <param name="context">Имя контекста</param>
        /// <param name="feature">Ключ фичи</param>
        public void DeleteContext(string context, string feature)
        {
            _featureToggle.DeleteContext(context, feature);
            WriteInConsoleByThread(string.Format("Context {0} in {1} is delete", context, feature));
        }

        /// <summary>
        /// Удаляет параметр контекста у фичи
        /// </summary>
        /// <param name="context">Имя контекста</param>
        /// <param name="feature">Ключ фичи</param>
        /// <param name="param">Имя параметра</param>
        public void DeleteContext(string context, string feature, string param)
        {
            _featureToggle.DeleteContext(context, feature, param);
            WriteInConsoleByThread(string.Format("{0} in {1} for {2} is delete", param, context, feature));
        }

        /// <summary>
        /// Пишет сообщение в консоли с идентификатором потока и окрашивает каждый поток в свой цвет
        /// </summary>
        /// <param name="str">Сообщение</param>
        private void WriteInConsoleByThread(string str)
        {
            var threadId = Thread.CurrentThread.ManagedThreadId;
            Console.ForegroundColor = (ConsoleColor)((threadId % 15) + 1);
            Console.WriteLine("Tread: {0}\t{1}", threadId, str);
        }
    }
}
