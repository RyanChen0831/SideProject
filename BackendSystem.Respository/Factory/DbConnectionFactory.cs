using BackendSystem.Respository.Interface;
using Microsoft.Data.SqlClient;
using System.Data;
namespace BackendSystem.Respository.Factory
{
    public class DbConnectionFactory:IDbConnectionFactory
    {
        private readonly string _connectionString;
        public DbConnectionFactory(string connectionString) 
        {
            _connectionString = connectionString;
        }

        public IDbConnection CreateConnection()
        {
            return new SqlConnection(_connectionString);
        }
    }
}
