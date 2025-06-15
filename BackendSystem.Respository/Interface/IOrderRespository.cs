using BackendSystem.Respository.CommandModels;
using BackendSystem.Respository.ResultModel;

namespace BackendSystem.Respository.Interface
{
    public interface IOrderRespository
    {
        public Task<IEnumerable<OrderDataModel>> GetOrderList();
        public Task<IEnumerable<OrderResultModel>> GetOrder(int memberId);
        public Task<bool> CreateOrder(OrderCommandModel order);
        public Task<bool> CreateOrderDetail(OrderDetailCondition orderDetail);
        public Task<bool> UpdateOrderStatus(OrderStatusCommandModel orderCondition);
    }
}
