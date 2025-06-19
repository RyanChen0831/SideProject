using System.Data;
using BackendSystem.Respository.CommandModels;
using BackendSystem.Respository.Interface;
using BackendSystem.Respository.ResultModels;
using Dapper;

namespace BackendSystem.Respository.Implement
{
    public class OrderManagementRespository:IOrderManagementRespository
    {
        public OrderManagementRespository()
        {
        }

        public async Task<int> DeleteOrder(IDbConnection conn, IDbTransaction tx, DeleteOrderCommandModel command)
        {
            const string delete = @" UPDATE Orders SET IsDeleted = 1,DeletedBy = @DeletedBy,DeletedDate = GETDATE() WHERE OrderId = @OrderId AND IsDeleted = 0 ";
            var res = await conn.ExecuteAsync(delete, command, tx);
            return res;
        }

        public async Task<IEnumerable<OrderManagementListItemModel>> GetOrderListItem(IDbConnection conn, SearchOrderQueryModel query)
        {
            var builder = new SqlBuilder();

            var selector = builder.AddTemplate(@"
                SELECT 
                    Orders.*,
                    M.Name,
                    Product.ProductName,
                    OD.Quantity,
                    OD.SubTotal
                FROM Orders
                LEFT JOIN Member M ON M.MemberId = Orders.MemberId
                INNER JOIN OrderDetail OD ON Orders.OrderId = OD.OrderId
                INNER JOIN Product ON OD.ProductId = Product.ProductId
                /**where**/
            ");

            if (!string.IsNullOrWhiteSpace(query.OrderId))
                builder.Where("Orders.OrderId = @OrderId", new { query.OrderId });

            if (!string.IsNullOrWhiteSpace(query.MemberId))
                builder.Where("Orders.MemberId = @MemberId", new { query.MemberId });

            if (!string.IsNullOrWhiteSpace(query.MemberName))
                builder.Where("M.Name LIKE @MemberName", new { MemberName = "%" + query.MemberName + "%" });

            if (!string.IsNullOrWhiteSpace(query.ShippingStatus))
                builder.Where("Orders.ShippingStatus = @ShippingStatus", new { query.ShippingStatus });

            if (!string.IsNullOrWhiteSpace(query.ShippingAddress))
                builder.Where("Orders.ShippingAddress LIKE @ShippingAddress", new { ShippingAddress = "%" + query.ShippingAddress + "%" });

            if (!string.IsNullOrWhiteSpace(query.OrderStatus))
                builder.Where("Orders.OrderStatus = @OrderStatus", new { query.OrderStatus });

            if (!string.IsNullOrWhiteSpace(query.Payment))
                builder.Where("Orders.Payment = @Payment", new { query.Payment });

            if (query.IsCancel.HasValue)
                builder.Where("Orders.IsCancel = @IsCancel", new { query.IsCancel });

            if (query.IsDeleted.HasValue)
                builder.Where("Orders.IsDeleted = @IsDeleted", new { query.IsDeleted });

            Dictionary<string, OrderManagementListItemModel> orderDict = new();

            var data = await conn.QueryAsync<OrderManagementListItemModel, OrderManagementOrderDetailModel, OrderManagementListItemModel>(
                selector.RawSql,
                (order, orderDetail) =>
                {
                    if (!orderDict.TryGetValue(order.OrderId, out var currentOrder))
                    {
                        currentOrder = order;
                        currentOrder.OrderDetail = new List<OrderManagementOrderDetailModel>();
                        orderDict.Add(order.OrderId, currentOrder);
                    }
                    currentOrder.OrderDetail.Add(orderDetail);
                    return currentOrder;
                },
                selector.Parameters,
                splitOn: "ProductName"
            );

            return orderDict.Values;
        }

        public async Task<int> UpdateOrderPaymentStatus(IDbConnection conn, IDbTransaction tx, string orderId, string status ,string updateby)
        {
            const string sql = @"UPDATE Orders SET PaymentStatus = 1, UpdateBy = @UpdateBy, UpdateDate = GETDATE()
                                 WHERE OrderId = @OrderId AND PaymentStatus != @Status";

            var parameters = new { OrderId = orderId, Status = status, UpdateBy = updateby };
            return await conn.ExecuteAsync(sql, parameters, tx);
        }

        public async Task<int> UpdateOrderShipmentStatus(IDbConnection conn, IDbTransaction tx, string orderId, string status, string updateby)
        {
            const string sql = @"UPDATE Orders SET ShipmentStatus = 1, UpdateBy = @UpdateBy, UpdateDate = GETDATE()
                                 WHERE OrderId = @OrderId AND ShipmentStatus != @Status";

            var parameters = new { OrderId = orderId, Status = status, UpdateBy = updateby };
            return await conn.ExecuteAsync(sql, parameters, tx);
        }

    }
}
