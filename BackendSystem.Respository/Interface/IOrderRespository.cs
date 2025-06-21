using BackendSystem.Respository.CommandModels;
using BackendSystem.Respository.ResultModels;
using System.Data;

namespace BackendSystem.Respository.Interface
{
    public interface IOrderRespository
    {
        public Task<IEnumerable<OrderResultModel>> GetOrder(IDbConnection conn,int memberId);
        public Task<bool> CreateOrder(IDbConnection conn,IDbTransaction tx, OrderCommandModel command);
        public Task<bool> CreateOrderDetail(IDbConnection conn, IDbTransaction tx, OrderDetailCommandModel command);
        public Task<bool> UpdateOrderStatus(IDbConnection conn, IDbTransaction tx, OrderStatusCommandModel command);
    }
}
