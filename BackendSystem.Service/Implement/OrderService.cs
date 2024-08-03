using AutoMapper;
using BackendSystem.Respository.Dtos;
using BackendSystem.Respository.Interface;
using BackendSystem.Service.Dtos;
using BackendSystem.Service.Interface;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackendSystem.Service.Implement
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRespository _orderRespository;
        private readonly IMapper _mapper;
        public OrderService(IOrderRespository orderRespository,IMapper mapper)
        {
            _orderRespository = orderRespository;
            _mapper = mapper;
        }

        public async Task<bool> CreateOrder(OrderInfo order)
        {
            try
            {
                var par = _mapper.Map<OrderInfo, OrderCondition>(order);
                return await _orderRespository.CreateOrder(par);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> CreateOrderDetail(OrderDetailInfo orderDetail)
        {
            try
            {
                var par = _mapper.Map<OrderDetailInfo, OrderDetailCondition>(orderDetail);
                return await _orderRespository.CreateOrderDetail(par);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<OrderViewModel>> GetOrder(int userId)
        {
            var order = await _orderRespository.GetOrder(userId);
            var result = _mapper.Map<IEnumerable<OrderDataModel>, IEnumerable<OrderViewModel> >(order);
            return result;
        }

        public async Task<IEnumerable<OrderViewModel>> GetOrderList()
        {
            var orderList = await _orderRespository.GetOrderList();
            var result = _mapper.Map<IEnumerable<OrderDataModel>, IEnumerable<OrderViewModel>>(orderList);
            return result;
        }

        public Task<bool> UpdateOrderStatus(OrderStatusInfo orderCondition)
        {
            try
            {
                var par = _mapper.Map<OrderStatusInfo,OrderStatusCondition>(orderCondition);
                return _orderRespository.UpdateOrderStatus(par);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
