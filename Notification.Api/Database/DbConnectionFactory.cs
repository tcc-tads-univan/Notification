using Notification.Api.Database.Interfaces;
using System.Data;
using System.Data.SqlClient;

namespace Notification.Api.Database
{
    public class DbConnectionFactory : IDbConnectionFactory
    {
        private readonly string _connectionString;
        public DbConnectionFactory(string connectionString)
        {
            _connectionString = connectionString;
        }
        public async Task<IDbConnection> CreateConnection()
        {
            var dbConnection = new SqlConnection(_connectionString);
            await dbConnection.OpenAsync();
            return dbConnection;
        }
    }
}
