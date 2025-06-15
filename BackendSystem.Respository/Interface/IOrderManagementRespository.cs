using BackendSystem.Respository.CommandModels;
using BackendSystem.Respository.ResultModels;

namespace BackendSystem.Respository.Interface
{
    public interface IOrderManagementRespository
    {
        public Task<IEnumerable<OrderDataModel>> GetAllOrderData();
        public Task<int> DeleteOrder(OrderCommandModel order);
        public Task<int> UpdateOrder(OrderCommandModel order);

    }
}
