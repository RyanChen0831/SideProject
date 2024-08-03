using BackendSystem.Respository.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackendSystem.Respository.Interface
{
    public interface IOrderRespository
    {
        public Task<IEnumerable<OrderDataModel>> GetOrderList();
        public Task<IEnumerable<OrderDataModel>> GetOrder(int userId);
        public Task<bool> CreateOrder(OrderCondition order);
        public Task<bool> CreateOrderDetail(OrderDetailCondition orderDetail);
        public Task<bool> UpdateOrderStatus(OrderStatusCondition orderCondition);
    }
}
