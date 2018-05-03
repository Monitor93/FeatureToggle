using System.Data.SqlClient;
using FeatureToggle.DataBase.Abstract;
using FeatureToggle.DataBase.Models;

namespace FeatureToggle.DataBase.Repositories
{
    class ContextRepository : BaseRepository<Context, string>
    {

        public void DeleteNotRefContexts()
        {
            ExecuteNonQuery(DeleteNotRefContextsSql());
        }

        private SqlCommand DeleteNotRefContextsSql()
        {
            return new SqlCommand($"DELETE FROM {TableName} WHERE Id NOT IN (SELECT {nameof(FeatureToContext.Context)} FROM {nameof(FeatureToContext)})", SqlConnection);
        }
    }
}
