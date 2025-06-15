using BackendSystem.Respository.CommandModels;
using BackendSystem.Respository.ResultModels;

namespace BackendSystem.Respository.Interface
{
    public interface IOrderManagementRespository
    {
        public Task<IEnumerable<OrderResultModel>> GetAllOrderData();
        public Task<int> DeleteOrder(OrderCommandModel order);
        public Task<int> UpdateOrder(OrderCommandModel order);

    }
}
