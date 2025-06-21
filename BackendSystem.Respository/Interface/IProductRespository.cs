using BackendSystem.Respository.CommandModels;
using BackendSystem.Respository.ResultModels;
using System.Data;

namespace BackendSystem.Service.Interface
{
    public interface IProductRespository
    {
        public Task<ProductResultModel?> GetProduct(IDbConnection conn, int id);
        public Task<IEnumerable<ProductResultModel>> GetProductList(IDbConnection conn);
        public Task<int> CreateProduct(IDbConnection conn, IDbTransaction tx, ProductCommandModel param);
        public Task<bool> SoftDeleteProduct(IDbConnection conn, IDbTransaction tx, int id);
        public Task<int> GetProductStock(IDbConnection conn, int productid);
        public Task<int> AddProductCategories(IDbConnection conn, IDbTransaction tx, int productId, List<int> categoryIds, string current);
        public Task<bool> UpdateProductBasicInfo(IDbConnection conn, IDbTransaction tx, ProductCommandModel product);
        public Task<bool> UpdateProductCategories(IDbConnection conn, IDbTransaction tx, int productId, List<int> categoryIds, string updatedBy);
        public Task<bool> DeleteProductImage(IDbConnection conn, IDbTransaction tx, int productId, List<string> images);
        public Task<bool> RemoveProductStock(IDbConnection conn, IDbTransaction tx, int quantity, int productid);
        public Task<bool> UpdateProdcutStatus(IDbConnection conn, IDbTransaction tx, int productid, string status);
        public Task<IEnumerable<ProductCategoryResultModel>> GetProductCategories(IDbConnection conn);
        public Task<int> InsertProductImage(IDbConnection conn, IDbTransaction tx, ProductImageResultModel image);
    }
}
