using BackendSystem.Respository.CommandModels;
using BackendSystem.Respository.Interface;
using BackendSystem.Respository.ResultModels;
using Dapper;
using System.Data;

namespace BackendSystem.Respository.Implement
{
    public class OrderRespository : IOrderRespository
    {
        public OrderRespository()
        {
        }
        public async Task<IEnumerable<OrderResultModel>> GetOrder(IDbConnection conn, int memberId)
        {
            Dictionary<string, OrderResultModel> orderDict = new Dictionary<string, OrderResultModel>();
            var str = @"
                    SELECT [Orders].*,Prodcut.ProductName,[OrderDetail].Quantity,[OrderDetail].SubTotal,PImage.Path FROM [Orders] 
                    INNER JOIN [OrderDetail] ON[Orders].OrderId = [OrderDetail].OrderId
                    INNER JOIN [Product] Prodcut ON [OrderDetail].ProductId = Prodcut.ProductId
                    LEFT JOIN ProductImage PImage ON PImage.ProductId = Prodcut.ProductId
                    Where MemberId=@MemberId ";
            var parm = new DynamicParameters();
            parm.Add("MemberId", memberId, DbType.Int32);
            var data = await _dbConnection.QueryAsync<OrderResultModel, OrderDetailResultModel, OrderResultModel>(
                str,
                (order, orderDetail) =>
                {
                    if (!orderDict.TryGetValue(order.OrderId, out var currentOrder))
                    {
                        currentOrder = order;
                        currentOrder.OrderDetail = new List<OrderDetailResultModel>();
                        orderDict.Add(order.OrderId, currentOrder);
                    }
                    currentOrder.OrderDetail.Add(orderDetail);
                    return currentOrder;
                }
                , parm
                , splitOn: "ProductName"
                );
            return orderDict.Values;
        }

        /// <summary>
        /// 更改訂單狀態，資料格式OrderCondition
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="tx"></param>
        /// <param name="orderCondition"></param>
        /// <returns></returns>
        public async Task<bool> UpdateOrderStatus(IDbConnection conn, IDbTransaction tx, OrderStatusCommandModel command)
        {
            try
            {
                var str = @"UPDATE [Orders] SET Payment = @StatusID WHERE OrderId = @OrderId AND ";
                await conn.ExecuteAsync(str, command, tx);
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
        public async Task<bool> CreateOrder(IDbConnection conn, IDbTransaction tx, OrderCommandModel command)
        {
            try
            {
                //新增訂單
                var createOrder = @"INSERT INTO [Orders] 
                                ([OrderId],[MemberId],[OrderDate],[TotalAmount],[ShippingAddress],[ShippingStatus],[Payment],[PaymentStatus]) 
                                VALUES
                                (@OrderId,@MemberId,GETDATE(),@TotalAmount,@ShippingAddress,@ShippingStatus,@Payment,@PaymentStatus)";
                await conn.ExecuteAsync(createOrder, command, tx);
                return true;
            }
            catch (Exception ex)
            {
                throw;
            }

        }
        /// <summary>
        /// 新增訂單明細
        /// 傳入一個 OrderDetial的物件
        /// </summary>
        /// <param name="orderDetail"></param>
        /// <returns>bool</returns>
        public async Task<bool> CreateOrderDetail(IDbConnection conn, IDbTransaction tx, OrderDetailCommandModel command)
        {
            try
            {
                //新增訂單明細
                var createOrderDetail = @"INSERT INTO [OrderDetail] ([OrderId],[ProductId],[Quantity],[UnitPrice],[SubTotal]) 
                                            VALUES
                                          (@OrderId,@ProductId,@Quantity,@UnitPrice,@SubTotal)";
                await conn.ExecuteAsync(createOrderDetail, command, tx);
                return true;
            }
            catch (Exception ex)
            {
                throw;
            }

        }

    }
}
