using BackendSystem.Respository.Dtos;
using BackendSystem.Respository.ResultModel;
using BackendSystem.Service.Interface;
using Dapper;
using System.Data;

namespace BackendSystem.Respository.Implement
{
    public class ProductRespository : IProductRespository
    {
        private readonly IDbConnection _dbConnection;
        public ProductRespository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }
        public async Task<int> Create(ProductCondition param)
        {
            string str = @"INSERT INTO Product (ProductName, Price, Description,Stock,Rate,EnableTag) VALUES ( @ProductName, @Price, @Description,@Stock,0,'Active')
                           SELECT CAST(SCOPE_IDENTITY() AS INT); ";
            int productId = await _dbConnection.ExecuteScalarAsync<int>(str, param);
            return productId;
        }

        public async Task<bool> Delete(int id)
        {
            string str = @"Update Product SET IsDeleted='Y' WHERE ProductId=@ProductId";
            var param = new DynamicParameters();
            param.Add("ProductId", id, System.Data.DbType.Int32);
            await _dbConnection.ExecuteAsync(str, param);
            return true;
        }

        public async Task DeleteProductImage(int productId, List<string> images)
        {
            if(_dbConnection.State != ConnectionState.Open)
            {
                _dbConnection.Open();
            }           
            using var transaction = _dbConnection.BeginTransaction();
            try
            {
                string delete = @" DELETE FROM ProductImage WHERE ProductId = @ProductId AND Path = @Path ";
                foreach(var image in images)
                {
                    var param = new DynamicParameters();
                    param.Add("@ProdcutId", productId);
                    param.Add("@Path", $"product-image/{productId}/{image}");
                    await _dbConnection.ExecuteAsync(delete, param, transaction: transaction);
                }
                transaction.Commit();
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;
            }
        }
        public async Task<ProductResultModel?> GetProduct(int id)
        {
            string str = @"SELECT * FROM Product WHERE ProductId = @ProductId";
            var param = new DynamicParameters();
            param.Add("ProductId", id, System.Data.DbType.Int32);
            var product = await _dbConnection.QueryFirstOrDefaultAsync<ProductResultModel?>(str, param);

            // 檢查 product 是否為 null，如果是，返回 null
            //Dapper QueryFirstOrDefaultAsync 如果找不到返回值是預設值，這個案例就是 ProductDataModel ，如果要返回null就可以用下面這個方式。
            if (product?.Equals(default(ProductResultModel)) ?? true)
            {
                return null;
            }

            return product;
        }

        public async Task<IEnumerable<ProductResultModel>> GetProductList()
        {
            string str = @" SELECT 
                                Main.ProductId,
                                Main.ProductName,
                                Main.Price,
                                Main.Description,
                                Main.Stock,
                                Main.Rate,
                                Main.EnableTag,
                                Main.CreateDate,
                                Main.UpdateDate,
                                ISNULL((
		                            SELECT STRING_AGG(CONVERT(VARCHAR,C.Category),',')
		                            FROM Category C 
		                            LEFT JOIN  ProductMappingCategory  PMC ON PMC.ProductId = Main.ProductId
		                            WHERE C.CategoryId = PMC.CategoryId
	                            ),'') AS Category,
                                ISNULL((
                                    SELECT STRING_AGG(CONVERT(NVARCHAR(MAX), ProductImage.Path), ',')
                                    FROM ProductImage 
                                    WHERE ProductImage.ProductId = Main.ProductId
                                ), '') AS ProductImage
                            FROM Product Main ";

            var products = await _dbConnection.QueryAsync<ProductResultModel>(str);

            foreach (var product in products)
            {
                product.CategoryList = ConvertStringToList(product.Category);
                product.ProductImageList = ConvertStringToList(product.ProductImage);
            }

            return products;
        }

        public async Task<int?> GetProductStock(int prodcutId)
        {
            string str = @"SELECT Stock FROM Product WHERE ProductId=@ProductId";
            var parm = new DynamicParameters();
            parm.Add("@ProductId", prodcutId, DbType.Int32);
            return await _dbConnection.QueryFirstOrDefaultAsync<int?>(str, parm);
        }

        public async Task<bool> UpdateProduct(ProductCondition product)
        {
            if (_dbConnection.State != ConnectionState.Open)
            {
                _dbConnection.Open();
            }
            using var transaction = _dbConnection.BeginTransaction();
            string user = "Mike.Chen@gmail.com";
            try
            {
                // 1. 更新 Product 主表
                string updateSql = @"
                UPDATE Product
                SET ProductName = @ProductName,
                    Price = @Price,
                    Description = @Description,
                    Stock = @Stock
                WHERE ProductId = @ProductId";

                var productParams = new DynamicParameters();
                productParams.Add("@ProductId", product.ProductId, DbType.Int32);
                productParams.Add("@ProductName", product.ProductName, DbType.String);
                productParams.Add("@Price", product.Price, DbType.Int32);
                productParams.Add("@Description", product.Description, DbType.String);
                productParams.Add("@Stock", product.Stock, DbType.Int32);

                await _dbConnection.ExecuteAsync(updateSql, productParams, transaction: transaction);

                // 2. 更新 Product 與 Category 的關聯（多對多表）
                if (product.Category?.Count > 0)
                {
                    // 2-1. 先刪除舊資料
                    string deleteSql = @"DELETE FROM ProductMappingCategory WHERE ProductId = @ProductId";
                    await _dbConnection.ExecuteAsync(deleteSql, productParams, transaction: transaction);

                    // 2-2. 再新增新資料
                    string insertSql = @"
                    INSERT INTO ProductMappingCategory
                        (ProductId, CategoryId, UpdateTime, UpdateBy, CreateTime, CreateBy)
                    VALUES
                        (@ProductId, @CategoryId, GETDATE(), @User, GETDATE(), @User)";

                    foreach (var categoryId in product.Category)
                    {
                        var insertParams = new DynamicParameters();
                        insertParams.Add("@ProductId", product.ProductId);
                        insertParams.Add("@CategoryId", categoryId);
                        insertParams.Add("@User", user);

                        await _dbConnection.ExecuteAsync(insertSql, insertParams, transaction: transaction);
                    }
                }

                // 3. 成功就提交
                transaction.Commit();
                return true;
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;
            }
        }

        public async Task<bool> RemoveProductStock(int quantity, int productId)
        {
            string sql = @"UPDATE Product
                           SET Stock = Stock - @Quantity,
                               UpdateDate = GETDATE()
                           WHERE ProductId = @ProductId; ";

            var parameters = new DynamicParameters();
            parameters.Add("@ProductId", productId, DbType.Int32);
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

        public async Task<bool> UpdateProdcutStatus(int productid, string status)
        {
            using (var transaction = _dbConnection.BeginTransaction())
            {
                try
                {
                    var str = @"Update Product SET EnableTag=@Status,UpdateDate=GETDATE() Where ProductId = @ProductId";
                    var parameters = new DynamicParameters();
                    parameters.Add("@ProductId", productid, DbType.Int32);
                    parameters.Add("@Status", status, DbType.String);
                    await _dbConnection.ExecuteAsync(str, parameters, transaction);
                    transaction.Commit();
                    return true;
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public async Task<IEnumerable<ProductCategoryResultModel>> GetProductCategory()
        {
            var str = @"SELECT * FROM Category";
            var res = await _dbConnection.QueryAsync<ProductCategoryResultModel>(str);
            return res;
        }

        public async Task InsertProductImage(ProductImageResultModel image)
        {
            string str = @"INSERT INTO ProductImage(ProductId, Name, Path, CreateTime)
                   VALUES(@ProductId, @Name, @Path, GETDATE())";
            await _dbConnection.ExecuteAsync(str, image);
        }
        //自定義 處理 Dapper 字串陣列
        private List<string> ConvertStringToList(string? value)
        {
            return string.IsNullOrEmpty(value) ? new List<string>() : value.Split(',').ToList();
        }

        public async Task AddCategory(List<int> param)
        {
            string str = @"INSER INTO ProductMappingCategory(ProductId , CategoryId , CreateTime, CreateBy, UpdateTime , UpdateBy)
                            VALUES (@ProductId , @CategoryId,GETDATE(),@Admin ,GETDATE(),@Admin)";
            foreach (var item in param)
            {
                await _dbConnection.ExecuteAsync(str, item);
            }
        }

    }
}
