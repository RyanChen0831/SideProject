using System.Data;
using BackendSystem.Respository.CommandModels;
using BackendSystem.Respository.Interface;
using BackendSystem.Respository.ResultModels;

namespace BackendSystem.Respository.Implement
{
    public class OrderManagementRespository:IOrderManagementRespository
    {
        private readonly IDbConnection _dbConnection ;
        public OrderManagementRespository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection ;
        }

        public Task<int> DeleteOrder(OrderCommandModel order)
        {
            throw new NotImplementedException();
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
