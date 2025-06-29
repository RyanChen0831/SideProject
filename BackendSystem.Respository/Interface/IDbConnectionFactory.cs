using System.Data;

namespace BackendSystem.Respository.Interface
{
    public interface IDbConnectionFactory
    {
        public IDbConnection CreateConnection();
    }
}
