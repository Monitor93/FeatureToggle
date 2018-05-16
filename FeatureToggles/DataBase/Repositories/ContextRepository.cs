using System.Data.SqlClient;
using FeatureToggle.DataBase.Abstract;
using FeatureToggle.DataBase.Models;

namespace FeatureToggle.DataBase.Repositories
{
    /// <summary>
    /// Репозиторий для контекстов
    /// </summary>
    class ContextRepository : BaseRepository<Context, string>
    {
        /// <summary>
        /// Удаляет контексты на которые нет ссылок
        /// </summary>
        public void DeleteNotRefContexts()
        {
            ExecuteNonQuery(DeleteNotRefContextsSql());
        }

        /// <summary>
        /// Создаёт sql команду для удаления контекстов на которые нет ссылок
        /// </summary>
        /// <returns>Созданная Sql команда</returns>
        private SqlCommand DeleteNotRefContextsSql()
        {
            return new SqlCommand($"DELETE FROM {TableName} WHERE Id NOT IN (SELECT {nameof(FeatureToContext.Context)} FROM {nameof(FeatureToContext)})", SqlConnection);
        }
    }
}
