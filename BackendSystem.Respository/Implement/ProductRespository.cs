using BackendSystem.Respository.CommandModels;
using BackendSystem.Respository.ResultModels;
using BackendSystem.Service.Interface;
using Dapper;
using System.Data;

namespace BackendSystem.Respository.Implement
{
    public class ProductRespository : IProductRespository
    {
        public ProductRespository()
        {
        }
        public async Task<ProductResultModel?> GetProduct(IDbConnection conn, int id)
        {
            string str = @"SELECT * FROM Product WHERE ProductId = @ProductId";
            var param = new DynamicParameters();
            param.Add("ProductId", id, System.Data.DbType.Int32);
            var product = await conn.QueryFirstOrDefaultAsync<ProductResultModel?>(str, param);
            return product;
        }
        public async Task<IEnumerable<ProductResultModel>> GetProductList(IDbConnection conn)
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
                            FROM Product Main "
            ;

            var products = await conn.QueryAsync<ProductResultModel>(str);

            foreach (var product in products)
            {
                product.CategoryList = ConvertStringToList(product.Category);
                product.ProductImageList = ConvertStringToList(product.ProductImage);
            }

            return products;
        }
        public async Task<int> CreateProduct(IDbConnection conn, IDbTransaction tx, ProductCommandModel param)
        {
            string str = @"INSERT INTO Product (ProductName, Price, Description,Stock,Rate,EnableTag) VALUES ( @ProductName, @Price, @Description,@Stock,0,'Active')
                           SELECT CAST(SCOPE_IDENTITY() AS INT); ";
            int productId = await conn.ExecuteScalarAsync<int>(str, param, tx);
            return productId;
        }
        public async Task<bool> SoftDeleteProduct(IDbConnection conn, IDbTransaction tx, int id)
        {

            string str = @"UPDATE Product SET IsDeleted = 1 WHERE ProductId = @ProductId";
            var param = new DynamicParameters();
            param.Add("ProductId", id, System.Data.DbType.Int32);
            await conn.ExecuteAsync(str, param, tx);
            return true;
        }
        public async Task<bool> DeleteProductImage(IDbConnection conn, IDbTransaction tx, int productId, List<string> images)
        {

            string delete = @" DELETE FROM ProductImage WHERE ProductId = @ProductId AND Path = @Path ";
            foreach (var image in images)
            {
                var param = new DynamicParameters();
                param.Add("@ProdcutId", productId);
                param.Add("@Path", $"product-image/{productId}/{image}");
                await conn.ExecuteAsync(delete, param, tx);
            }
            return true;
        }
        public async Task<int> GetProductStock(IDbConnection conn, int prodcutId)
        {
            string str = @"SELECT Stock FROM Product WHERE ProductId=@ProductId";
            var parm = new DynamicParameters();
            parm.Add("@ProductId", prodcutId, DbType.Int32);
            return await conn.QueryFirstOrDefaultAsync<int>(str, parm);
        }
        public async Task<bool> UpdateProductBasicInfo(IDbConnection conn, IDbTransaction tx, ProductCommandModel product)
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

            await conn.ExecuteAsync(updateSql, productParams, tx);
            return true;
        }
        public async Task<bool> UpdateProductCategories(IDbConnection conn, IDbTransaction tx, int productId, List<int>? categoryIds, string updatedBy)
        {
            // 沒有要更新就不動作
            if (categoryIds == null)
                return true;

            // 先刪除所有舊的資料
            const string deleteSql = @"DELETE FROM ProductMappingCategory WHERE ProductId = @ProductId";
            await conn.ExecuteAsync(deleteSql, new { ProductId = productId }, tx);

            // 如果是空清單清空完就結束
            if (categoryIds.Count == 0)
                return true;

            const string insertSql = @"
            INSERT INTO ProductMappingCategory
                (ProductId, CategoryId, UpdateTime, UpdateBy, CreateTime, CreateBy)
            VALUES
            (@ProductId, @CategoryId, GETDATE(), @User, GETDATE(), @User)";

            foreach (var categoryId in categoryIds)
            {
                var insertParams = new DynamicParameters();
                insertParams.Add("@ProductId", productId);
                insertParams.Add("@CategoryId", categoryId);
                insertParams.Add("@User", updatedBy);

                await conn.ExecuteAsync(insertSql, insertParams, tx);
            }

            return true;
        }
        public async Task<bool> RemoveProductStock(IDbConnection conn, IDbTransaction tx, int quantity, int productId)
        {
            const string sql = @"UPDATE Product
                           SET Stock = Stock - @Quantity,
                               UpdateDate = GETDATE()
                           WHERE ProductId = @ProductId; ";
            var parameters = new DynamicParameters();
            parameters.Add("@ProductId", productId, DbType.Int32);
            parameters.Add("@Quantity", quantity, DbType.Int32);
            await conn.ExecuteAsync(sql, parameters,tx);
            return true;
        }
        public async Task<bool> UpdateProdcutStatus(IDbConnection conn, IDbTransaction tx, int productid, string status)
        {
            const string str = @"Update Product SET EnableTag=@Status,UpdateDate=GETDATE() Where ProductId = @ProductId";
            var parameters = new DynamicParameters();
            parameters.Add("@ProductId", productid, DbType.Int32);
            parameters.Add("@Status", status, DbType.String);
            await conn.ExecuteAsync(str, parameters, tx);
            return true;
        }
        public async Task<int> AddProductCategories(IDbConnection conn, IDbTransaction tx, int productId, List<int> categoryIds, string current)
        {
            if (categoryIds == null || categoryIds.Count == 0)
                return 0;

            const string sql = @"
            INSERT INTO ProductMappingCategory
                (ProductId, CategoryId, CreateTime, CreateBy, UpdateTime, UpdateBy)
            VALUES
            (@ProductId, @CategoryId, GETDATE(), @Admin, GETDATE(), @Admin)";

            int total = 0;
            foreach (var categoryId in categoryIds)
            {
                var parameters = new DynamicParameters();
                parameters.Add("@ProductId", productId);
                parameters.Add("@CategoryId", categoryId);
                parameters.Add("@Admin", current);

                total += await conn.ExecuteAsync(sql, parameters, tx);
            }

            return total;
        }
        public async Task<IEnumerable<ProductCategoryResultModel>> GetProductCategories(IDbConnection conn)
        {
            var str = @"SELECT * FROM Category";
            var res = await conn.QueryAsync<ProductCategoryResultModel>(str);
            return res;
        }
        public async Task<int> InsertProductImage(IDbConnection conn, IDbTransaction tx, ProductImageResultModel image)
        {
            string str = @"INSERT INTO ProductImage(ProductId, Name, Path, CreateTime)
                   VALUES(@ProductId, @Name, @Path, GETDATE())";
            var res = await conn.ExecuteAsync(str, image,tx);
            return res;
        }
        private List<string> ConvertStringToList(string? value)
        {
            return string.IsNullOrEmpty(value) ? new List<string>() : value.Split(',').ToList();
        }

    }
}
