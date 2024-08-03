using BackendSystem.Respository.Dtos;
using BackendSystem.Service.Interface;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackendSystem.Respository.Implement
{
    public class ProductRespository : IProductRespository
    {
        private readonly IDbConnection _dbConnection;
        public ProductRespository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }
        public async Task<bool> Create(ProductCondition param)
        {
            string str = @"INSERT INTO Product (ProductID, ProductName, Price, Description,Stock) VALUES (@ProductId, @ProductName, @Price, @Description,@Stock);";
            await _dbConnection.ExecuteAsync(str, param);
            return true;
        }

        public async Task<bool> Delete(int id)
        {
            string str = @"DELETE FROM Product WHERE ProductId=@ProductId";
            var param = new DynamicParameters();
            param.Add("ProductId", id, System.Data.DbType.Int32);
            await _dbConnection.ExecuteAsync(str, param);
            return true;
        }

        public async Task<ProductDataModel?>GetProduct(int id)
        {
            string str = @"SELECT * FROM Product WHERE ProductId = @ProductId";
            var param = new DynamicParameters();
            param.Add("ProductId", id, System.Data.DbType.Int32);
            var product = await _dbConnection.QueryFirstOrDefaultAsync <ProductDataModel?>(str, param);

            // 檢查 product 是否為 null，如果是，返回 null
            //Dapper QueryFirstOrDefaultAsync 如果找不到返回值是預設值，這個案例就是 ProductDataModel ，如果要返回null就可以用下面這個方式。
            if (product?.Equals(default(ProductDataModel)) ?? true)
            {
                return null;
            }

            return product;
        }

        public IEnumerable<ProductDataModel?> GetProductList()
        {
            string str = @"SELECT * FROM Product";
            var product = _dbConnection.Query<ProductDataModel?>(str);
            return product;
        }

        public async Task<bool> CheckProductStock(int quantity,int prodcutId)
        {
            string str = @"SELECT Stock FROM Product WHERE ProductID=@ProductID";
            var parm = new DynamicParameters();
            parm.Add("@ProductID", prodcutId, DbType.Int32);
            var stock = await _dbConnection.QueryFirstOrDefaultAsync<int?>(str,parm);
            if(stock==null)
            {
                return false;
            }
            var result = stock.Value;
            if (quantity > result)
            {
                return false;
            }
            return true;
        }

        public async Task<bool> UpdateProduct(ProductCondition product)
        {
            string str = @"Update Product
                            Set  ProductName=@ProductName,
                                 Price=@Price,
                                 Description=@Description,
                                 Stock=@Stock 
                               WHERE ProductID=@ProductID";
            var param = new DynamicParameters();

            param.Add("@ProductID", product.ProductID, DbType.Int32);
            param.Add("@ProductName", product.ProductName, DbType.String);
            param.Add("@Price", product.Price, DbType.Int32);
            param.Add("@Description", product.Description, DbType.String);
            param.Add("@Stock", product.Stock, DbType.Int32);
            try
            {
                await _dbConnection.ExecuteAsync(str, param);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> RemoveProductStock(int quantity, int productId)
        {
            string sql = @"UPDATE Product
                           SET Stock = Stock - @Quantity,
                               UpdateDate = GETDATE()
                           WHERE ProductID = @ProductID; ";

            var parameters = new DynamicParameters();
            parameters.Add("@ProductID", productId, DbType.Int32);
            parameters.Add("@Quantity", quantity, DbType.Int32);

            try
            {
                await _dbConnection.ExecuteAsync(sql, parameters);
                return true;
            }
            catch (Exception ex)
            {
                return false;               
            }
        }
    }
}
