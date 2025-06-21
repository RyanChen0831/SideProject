using BackendSystem.Respository.CommandModels;
using BackendSystem.Respository.ResultModels;

namespace BackendSystem.Respository.Interface
{
    public interface IOrderRespository
    {
        public Task<IEnumerable<OrderResultModel>> GetOrder(int memberId);
        public Task<bool> CreateOrder(OrderCommandModel order);
        public Task<bool> CreateOrderDetail(OrderDetailCommandModel orderDetail);
        public Task<bool> UpdateOrderStatus(OrderDetailCommandModel orderCondition);
    }
}
