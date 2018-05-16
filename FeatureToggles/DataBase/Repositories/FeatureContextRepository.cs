using FeatureToggle.DataBase.Abstract;
using FeatureToggle.DataBase.Models;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace FeatureToggle.DataBase.Repositories
{
    /// <summary>
    /// Репозиторий для таблицы связывающей контексты и фичи
    /// </summary>
    class FeatureContextRepository : BaseRepository<FeatureToContext, int>
    {
        /// <summary>
        /// Возвращает контекст для фичи с заданным ключём
        /// </summary>
        /// <param name="featureKey">Ключ фичи</param>
        /// <returns>Список контекстов фичи</returns>
        public List<FeatureToContext> GetContextForFeature(string featureKey)
        {
            return ExecuteQuery(GetFeatureContextsSqlCommand(featureKey));
        }

        /// <summary>
        /// Удаляет связи контекст у фичи
        /// </summary>
        /// <param name="context">Имя контекста</param>
        /// <param name="feature">Ключ фичи</param>
        public void Delete(string context, string feature)
        {
            ExecuteNonQuery(DeleteByParamsSql(new Dictionary<string, string>
            {
                [nameof(FeatureToContext.Context)] = context,
                [nameof(FeatureToContext.Feature)] = feature
            }));
        }

        /// <summary>
        /// Удаляет заданный параметр контекста у фичи
        /// </summary>
        /// <param name="context">Имя контекста</param>
        /// <param name="feature">Ключ фичи</param>
        /// <param name="param">Параметр контекста</param>
        public void Delete(string context, string feature, string param)
        {
            ExecuteNonQuery(DeleteByParamsSql(new Dictionary<string, string>
            {
                [nameof(FeatureToContext.Context)] = context,
                [nameof(FeatureToContext.Feature)] = feature,
                [nameof(FeatureToContext.Param)] = param
            }));
        }

        /// <summary>
        /// Удаляет все контексты у заданной фичи
        /// </summary>
        /// <param name="featureKey">Ключ фичи</param>
        public void DeleteContextForFeature(string featureKey)
        {
            ExecuteNonQuery(DeleteFeatureContextsSql(featureKey));
        }

        /// <summary>
        /// Создаёт sql команду для удаления записей по набору параметров
        /// </summary>
        /// <param name="conditionsParams">Параметры для сравнения</param>
        /// <returns>Созданная Sql команда</returns>
        private SqlCommand DeleteByParamsSql(Dictionary<string, string> conditionsParams)
        {
            var conditions = conditionsParams
                .Select(x => $"{x.Key} = @{x.Key}")
                .ToList();
            var command = new SqlCommand($"DELETE FROM {TableName} WHERE {string.Join(" AND ", conditions)}", SqlConnection);
            foreach (var param in conditionsParams)
            {
                command.Parameters.AddWithValue($"@{param.Key}", param.Value);
            }
            return command;
        }

        /// <summary>
        /// Создаёт sql команду для получения контекст для фичи с заданным ключём
        /// </summary>
        /// <param name="featureKey">Ключ фичи</param>
        /// <returns>Созданная Sql команда</returns>
        private SqlCommand GetFeatureContextsSqlCommand(string featureKey)
        {
            var command = new SqlCommand($"SELECT * FROM {TableName} WHERE {nameof(FeatureToContext.Feature)} = @featureKey", SqlConnection);
            command.Parameters.AddWithValue("@featureKey", featureKey);
            return command;
        }


        /// <summary>
        /// Создайт sql команду для удаления всех контекстов у заданной фичи
        /// </summary>
        /// <param name="featureKey">Ключ фичи</param>
        /// <returns>Созданная Sql команда</returns>
        private SqlCommand DeleteFeatureContextsSql(string featureKey)
        {
            var command = new SqlCommand($"DELETE FROM {TableName} WHERE {nameof(FeatureToContext.Feature)} = @featureKey", SqlConnection);
            command.Parameters.AddWithValue("@featureKey", featureKey);
            return command;
        }

        /// <summary>
        /// Условие сравнения для мёржа при сохранении сущностей в базу
        /// </summary>
        /// <returns>Строка sql условия</returns>
        protected override string MergeCondition()
        {
            var featureEqual = $"Target.{ nameof(FeatureToContext.Feature)} = Source.{ nameof(FeatureToContext.Feature)}";
            var contextEqual = $"Target.{ nameof(FeatureToContext.Context)} = Source.{ nameof(FeatureToContext.Context)}";
            var paramEqual = $"Target.{ nameof(FeatureToContext.Param)} = Source.{ nameof(FeatureToContext.Param)}";
            return $"{contextEqual} AND {featureEqual} AND {paramEqual}";
        }
    }
}
