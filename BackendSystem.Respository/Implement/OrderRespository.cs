using BackendSystem.Respository.CommandModels;
using BackendSystem.Respository.Interface;
using BackendSystem.Respository.ResultModels;
using Dapper;
using System.Data;

namespace BackendSystem.Respository.Implement
{
    public class OrderRespository : IOrderRespository
    {
        private readonly IDbConnection _dbConnection;
        public OrderRespository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<IEnumerable<OrderResultModel>> GetOrder(int memberId)
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

        //更改訂單狀態，資料格式OrderCondition
        public async Task<bool> UpdateOrderStatus(OrderDetailCommandModel orderCondition)
        {
            try
            {
                var str = @"UPDATE [Orders] SET Payment = @StatusID WHERE OrderId = @OrderId";
                var parm = new DynamicParameters();
                parm.Add("StatusID", orderCondition.StatusId, DbType.Int32);
                parm.Add("OrderID", orderCondition.OrderId, DbType.String);

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
        public async Task<bool> CreateOrder(OrderCommandModel order)
        {
            try
            {
                //新增訂單
                var createOrder = @"INSERT INTO [Orders] 
                                ([OrderId],[MemberId],[OrderDate],[TotalAmount],[ShippingAddress],[ShippingStatus],[Payment],[PaymentStatus]) 
                                VALUES
                                (@OrderId,@MemberId,GETDATE(),@TotalAmount,@ShippingAddress,@ShippingStatus,@Payment,@PaymentStatus)";
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
        public async Task<bool> CreateOrderDetail(OrderDetailCommandModel orderDetail)
        {
            try
            {

                //新增訂單明細
                var createOrderDetail = @"INSERT INTO [OrderDetail] ([OrderId],[ProductId],[Quantity],[UnitPrice],[SubTotal]) 
                                            VALUES
                                          (@OrderId,@ProductId,@Quantity,@UnitPrice,@SubTotal)";
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
