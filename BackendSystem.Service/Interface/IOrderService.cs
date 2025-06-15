using BackendSystem.Respository.Dtos;
using BackendSystem.Service.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackendSystem.Service.Interface
{
    public interface IOrderService
    {
        public Task<IEnumerable<OrderViewModel>> GetOrderList();
        public Task<IEnumerable<OrderDTO>> GetOrder(int memberId);
        public Task<bool> CreateOrder(OrderInfo order);
        public Task<bool> CreateOrderDetail(OrderDetailInfo orderDetail);
        public Task<bool> UpdateOrderStatus(OrderStatusInfo orderCondition);

    }
}
