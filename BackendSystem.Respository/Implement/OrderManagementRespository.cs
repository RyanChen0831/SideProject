using System.Data;
using BackendSystem.Respository.CommandModels;
using BackendSystem.Respository.Interface;
using BackendSystem.Respository.ResultModels;
using Dapper;

namespace BackendSystem.Respository.Implement
{
    public class OrderManagementRespository:IOrderManagementRespository
    {
        private readonly IDbConnection _dbConnection ;
        public OrderManagementRespository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<int> DeleteOrder(OrderCommandModel order)
        {
            if (_dbConnection.State != ConnectionState.Open)
                _dbConnection.Open();
            using var transaction = _dbConnection.BeginTransaction();
            try
            {
                string delete = @" UPDATE Orders SET IsDeleted = 1,DeletedBy = @DeletedBy,DeletedDate = GETDATE() WHERE OrderId = @OrderId AND IsDeleted = 0 ";
                var res = await _dbConnection.ExecuteAsync(delete, order, transaction);
                transaction.Commit();
                return res;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public Task<IEnumerable<OrderResultModel>> GetAllOrderData()
        {
            throw new NotImplementedException();
        }

        public Task<int> UpdateOrder(OrderCommandModel order)
        {
            throw new NotImplementedException();
        }
    }
}
