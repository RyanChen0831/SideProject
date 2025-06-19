using BackendSystem.Respository.CommandModels;
using BackendSystem.Respository.ResultModels;
using System.Data;

namespace BackendSystem.Respository.Interface
{
    public interface IOrderManagementRespository
    {
        public Task<IEnumerable<OrderManagementListItemModel>> GetOrderListItem(IDbConnection conn, SearchOrderQueryModel query);
        public Task<int> DeleteOrder(IDbConnection conn, IDbTransaction tx, DeleteOrderCommandModel command);
        public Task<int> UpdateOrderPaymentStatus(IDbConnection conn, IDbTransaction tx, string orderId, string status, string updateby);
        public Task<int> UpdateOrderShipmentStatus(IDbConnection conn, IDbTransaction tx, string orderId, string status, string updateby);

    }
}
