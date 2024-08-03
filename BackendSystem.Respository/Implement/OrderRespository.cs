using BackendSystem.Respository.Interface;
using System;
using System.Collections.Generic;
using System.Data;
using Dapper;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BackendSystem.Respository.Dtos;
using System.Reflection.Metadata;

namespace BackendSystem.Respository.Implement
{
    public class OrderRespository :IOrderRespository
    {
        private readonly IDbConnection _dbConnection;
        public OrderRespository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        //管理者查看所有的OrderList，返回OrderDataModel
        public async Task<IEnumerable<OrderDataModel>> GetOrderList()
        {
            var orderList = new List<OrderDataModel>();
            var str = @"SELECT O.OrderID,OD.ProductID,OD.Quantity,OD.UnitPrice,OD.SubTotal,O.ShippingAddress,O.Payment,O.PaymentStatus ,O.OrderDate
	                        FROM [Order] O
                        INNER JOIN [OrderDetial] OD ON O.OrderID = OD.OrderID";
            var data = await _dbConnection.QueryAsync<OrderDataModel>(str);
            return data.Select(item => new OrderDataModel
            {
                OrderID = item.OrderID,
                ProductID = item.ProductID,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice,
                SubTotal = item.SubTotal,
                ShippingAddress = item.ShippingAddress,
                Payment = item.Payment,
                PaymentStatus = item.PaymentStatus,
                OrderDate = item.OrderDate
            });

        }
        //使用者的OrderList，返回OrderDataModel
        public async Task<IEnumerable<OrderDataModel>> GetOrder(int userId)
        {
            var orderList  = new List<OrderDataModel>();
            var str = @"SELECT O.OrderID,OD.ProductID,OD.Quantity,OD.UnitPrice,OD.SubTotal,O.ShippingAddress,O.Payment,O.PaymentStatus ,O.OrderDate
	                        FROM [Order] O
                        INNER JOIN [OrderDetial] OD ON O.OrderID = OD.OrderID
                        WHERE O.UserID=@UserID";
            var parm = new DynamicParameters();
            parm.Add("UserID", userId, DbType.Int32);
            var data =  await _dbConnection.QueryAsync<OrderDataModel>(str, parm);
            return data.Select(item => new OrderDataModel
            {
                OrderID = item.OrderID,
                ProductID = item.ProductID,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice,
                SubTotal = item.SubTotal,
                ShippingAddress = item.ShippingAddress,
                Payment = item.Payment,
                PaymentStatus = item.PaymentStatus,
                OrderDate = item.OrderDate
            });
        }

        //更改訂單狀態，資料格式OrderCondition
        public async Task<bool> UpdateOrderStatus(OrderStatusCondition orderCondition)
        {
            try
            {
                var str = @"UPDATE [Order] SET Payment = @StatusID WHERE OrderID = @OrderID";
                var parm = new DynamicParameters();
                parm.Add("StatusID", orderCondition.StatusID, DbType.Int32);
                parm.Add("OrderID", orderCondition.OrderID, DbType.String);

                await _dbConnection.ExecuteAsync(str, parm);

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        /// <summary>
        /// 新增訂單
        /// 傳入一個 Order 物件
        /// </summary>
        /// <param name="order"></param>
        /// <returns>bool</returns>
        public async Task<bool> CreateOrder(OrderCondition order)
        {
            try
            {
                //新增訂單
                var createOrder = @"INSERT INTO [Order] 
                                ([OrderID],[UserID],[OrderDate],[TotalAmount],[ShippingAddress],[ShippingStatus],[Payment],[PaymentStatus]) 
                                VALUES
                                (@OrderID,@UserID,GETDATE(),@TotalAmount,@ShippingAddress,@ShippingStatus,@Payment,@PaymentStatus)";
                var parm = new DynamicParameters(order);

                await _dbConnection.ExecuteAsync(createOrder, parm);

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }
        /// <summary>
        /// 新增訂單明細
        /// 傳入一個 OrderDetial的物件
        /// </summary>
        /// <param name="orderDetail"></param>
        /// <returns>bool</returns>
        public async Task<bool> CreateOrderDetail(OrderDetailCondition orderDetail)
        {
            try
            {

                //新增訂單明細
                var createOrderDetail = @"INSERT INTO [OrderDetail] ([OrderID],[ProductID],[Quantity],[UnitPrice],[SubTotal]) 
                                            VALUES
                                          (@OrderID,@ProductID,@Quantity,@UnitPrice,@SubTotal)";
                var orderDetailParameters = new DynamicParameters(orderDetail);

                await _dbConnection.ExecuteAsync(createOrderDetail, orderDetailParameters);

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }


    }
}
