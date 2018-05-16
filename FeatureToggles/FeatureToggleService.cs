using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using FeatureToggle.Config;
using FeatureToggle.DataBase.Models;
using FeatureToggle.DataBase.Repositories;
using FeatureToggle.Enums;
using FeatureToggle.Exceptions;
using FeatureToggle.TransferObjects;

namespace FeatureToggle
{
    /// <summary>
    /// Сервис управления функциональнастями
    /// </summary>
    public class FeatureToggleService : IFeatureToggle, IDisposable
    {
        /// <summary>
        /// Репозитории
        /// </summary>
        private readonly FeatureRepository _featureRepository;
        private readonly ContextRepository _contextRepository;
        private readonly FeatureContextRepository _featureContextToggleRepository;

        /// <summary>
        /// Кэш
        /// </summary>
        private static readonly MemoryCache Cash;
        private static readonly object Locker = new object();

        /// <summary>
        /// Создаёт политику кэша с заданным временем жизни записи в кэше
        /// </summary>
        private static CacheItemPolicy CachePolicy => new CacheItemPolicy
        {
            AbsoluteExpiration = new DateTimeOffset(DateTime.UtcNow.Add(FeatureToggleConfiguration.CasheLifeTime))
        };

        /// <summary>
        /// Создаёт экземпляр класса <see cref="FeatureToggleService"/>
        /// </summary>
        public FeatureToggleService()
        {
            _featureRepository = new FeatureRepository();
            _contextRepository = new ContextRepository();
            _featureContextToggleRepository = new FeatureContextRepository();
        }

        /// <summary>
        /// Статический конструктор для инициализации кэша
        /// </summary>
        static FeatureToggleService()
        {
            lock (Locker)
            {
                Cash = new MemoryCache("FeatureToggleMemoryCache");
            }
        }

        /// <summary>
        /// Создаёт новую фичу или обновляет информацию о существующей
        /// </summary>
        /// <param name="feature">Фича</param>
        public void CreateOrUpdateFeature(FeatureDto feature)
        {
            var featureToDb = new Feature(feature);
            var contextsToDb = feature.Contexts.Select(x => new Context(x.ContextName));
            var contextParamsToDb = new List<FeatureToContext>();
            foreach(var context in feature.Contexts)
            {
                contextParamsToDb.AddRange(context.Params.Select(values => new FeatureToContext
                {
                    Context = context.ContextName,
                    Param = values.Key,
                    IsEnable = values.Value,
                    Feature = feature.Key
                }));
            }
            _featureRepository.Save(featureToDb);
            _contextRepository.Save(contextsToDb.ToArray());
            _featureContextToggleRepository.Save(contextParamsToDb.ToArray());
            RefreshFeatureInCashe(feature.Key, feature);
        }

        /// <summary>
        /// Получить полную информацию о фиче по ключу
        /// </summary>
        /// <param name="key">Ключ</param>
        /// <returns>Фича</returns>
        public FeatureDto GetFeature(string key)
        {
            lock (key)
            {
                if (Cash.Contains(key))
                {
                    return (FeatureDto)Cash.Get(key);
                }
                else
                {
                    var feature = _featureRepository.Get(key);
                    if (feature != null)
                    {
                        var contexts = _featureContextToggleRepository.GetContextForFeature(key);
                        var dto = (contexts == null || !contexts.Any())
                            ? new FeatureDto(feature)
                            : new FeatureDto(feature, contexts);
                        Cash.Add(key, dto, CachePolicy);
                        return dto;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }

        /// <summary>
        /// Проверить функциональность фичи
        /// </summary>
        /// <param name="key">Ключ фичи</param>
        /// <param name="defaultValue">Результат в случае если фича не найдена</param>
        /// <returns>Включена ли фича</returns>
        public bool IsEnable(string key, DefaultFeatureValue defaultValue = DefaultFeatureValue.False)
        {
            var feature = GetFeature(key);
            if (feature != null)
            {
                return feature.Value;
            }
            else
            {
                switch (defaultValue)
                {
                    case DefaultFeatureValue.True:
                        return true;
                    case DefaultFeatureValue.False:
                        return false;
                    default:
                    case DefaultFeatureValue.Exception:
                        throw new FeatureNotFounException(key);
                }
            }
        }

        /// <summary>
        /// Проверить функциональность фичи с учётом контекста
        /// </summary>
        /// <param name="key">Фича</param>
        /// <param name="context">Контекст</param>
        /// <param name="param">Параметр</param>
        /// <param name="defaultValue">Результат в случае если фича не найдена</param>
        /// <returns>Включена ли фича с заданным контекстом и параметром</returns>
        public bool IsEnable(string key, string context, string param, DefaultFeatureValue defaultValue = DefaultFeatureValue.False)
        {
            var feature = GetFeature(key);
            if (feature != null)
            {
                return feature[context]?[param] ?? feature.Value;
            }
            else
            {
                switch (defaultValue)
                {
                    case DefaultFeatureValue.True:
                        return true;
                    case DefaultFeatureValue.False:
                        return false;
                    default:
                    case DefaultFeatureValue.Exception:
                        throw new FeatureNotFounException(key);
                }
            }
        }

        /// <summary>
        /// Удаляет фичу
        /// </summary>
        /// <param name="key">Ключ фичи</param>
        public void DeleteFeature(string key)
        {
            _featureContextToggleRepository.DeleteContextForFeature(key);
            _contextRepository.DeleteNotRefContexts();
            _featureRepository.Delete(key);
            lock (key)
            {
                Cash.Remove(key);
            }
        }

        /// <summary>
        /// Удаляет контекст у заданной фичи
        /// </summary>
        /// <param name="context">Удаляемый контекст</param>
        /// <param name="feature">Фича у которой надо удалить контекст</param>
        public void DeleteContext(string context, string feature)
        {
            _featureContextToggleRepository.Delete(context, feature);
            RefreshFeatureInCashe(feature, GetFeatureDtoFromDb(feature));
        }

        /// <summary>
        /// Удаляет контекст у заданной фичи с заданным параметром
        /// </summary>
        /// <param name="context">Удаляемый контекст</param>
        /// <param name="feature">Фича у которой надо удалить контекст</param>
        /// <param name="param">Параметр который будет удалён</param>
        public void DeleteContext(string context, string feature, string param)
        {
            _featureContextToggleRepository.Delete(context, feature, param);
            RefreshFeatureInCashe(feature, GetFeatureDtoFromDb(feature));
        }

        /// <summary>
        /// Обновляет информацию о фиче в кэше
        /// </summary>
        /// <param name="key">Ключ фичи</param>
        /// <param name="feature">фича</param>
        private void RefreshFeatureInCashe(string key, FeatureDto feature)
        {
            lock (key)
            {
                if (Cash.Contains(key))
                {
                    Cash.Remove(key);
                }
                if (feature != null)
                {
                    Cash.Add(key, feature, CachePolicy);
                }
            }
        }

        /// <summary>
        /// Получает фичу из базы данных
        /// </summary>
        /// <param name="key">Ключ фичи</param>
        /// <returns>Фича</returns>
        private FeatureDto GetFeatureDtoFromDb(string key)
        {
            var feature = _featureRepository.Get(key);
            var contexts = _featureContextToggleRepository.GetContextForFeature(key);
            var dto = (contexts == null || !contexts.Any())
                ? new FeatureDto(feature)
                : new FeatureDto(feature, contexts);
            return dto;
        }

        public void Dispose()
        {
            _featureRepository.Dispose();
            _contextRepository.Dispose();
            _featureContextToggleRepository.Dispose();
        }
    }
}
