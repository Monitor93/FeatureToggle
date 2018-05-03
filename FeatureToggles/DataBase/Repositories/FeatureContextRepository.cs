using FeatureToggle.DataBase.Abstract;
using FeatureToggle.DataBase.Models;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace FeatureToggle.DataBase.Repositories
{
    class FeatureContextRepository : BaseRepository<FeatureToContext, int>
    {
        public List<FeatureToContext> GetContextForFeature(string featureKey)
        {
            return ExecuteQuery(GetFeatureContextsSqlCommand(featureKey));
        }

        public void Delete(string context, string feature)
        {
            ExecuteNonQuery(DeleteByParamsSql(new Dictionary<string, string>
            {
                [nameof(FeatureToContext.Context)] = context,
                [nameof(FeatureToContext.Feature)] = feature
            }));
        }

        public void Delete(string context, string feature, string param)
        {
            ExecuteNonQuery(DeleteByParamsSql(new Dictionary<string, string>
            {
                [nameof(FeatureToContext.Context)] = context,
                [nameof(FeatureToContext.Feature)] = feature,
                [nameof(FeatureToContext.Param)] = param
            }));
        }

        public void DeleteContextForFeature(string featureKey)
        {
            ExecuteNonQuery(DeleteFeatureContextsSql(featureKey));
        }

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

        private SqlCommand GetFeatureContextsSqlCommand(string featureKey)
        {
            var command = new SqlCommand($"SELECT * FROM {TableName} WHERE {nameof(FeatureToContext.Feature)} = @featureKey", SqlConnection);
            command.Parameters.AddWithValue("@featureKey", featureKey);
            return command;
        }

        private SqlCommand DeleteFeatureContextsSql(string featureKey)
        {
            var command = new SqlCommand($"DELETE FROM {TableName} WHERE {nameof(FeatureToContext.Feature)} = @featureKey", SqlConnection);
            command.Parameters.AddWithValue("@featureKey", featureKey);
            return command;
        }

        protected override string MergeCondition()
        {
            var featureEqual = $"Target.{ nameof(FeatureToContext.Feature)} = Source.{ nameof(FeatureToContext.Feature)}";
            var contextEqual = $"Target.{ nameof(FeatureToContext.Context)} = Source.{ nameof(FeatureToContext.Context)}";
            var paramEqual = $"Target.{ nameof(FeatureToContext.Param)} = Source.{ nameof(FeatureToContext.Param)}";
            return $"{contextEqual} AND {featureEqual} AND {paramEqual}";
        }
    }
}
