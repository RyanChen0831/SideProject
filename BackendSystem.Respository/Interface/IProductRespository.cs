using BackendSystem.Respository.Dtos;
using BackendSystem.Respository.ResultModel;

namespace BackendSystem.Service.Interface
{
    public interface IProductRespository
    {
        public Task<IEnumerable<ProductResultModel>> GetProductList();
        public Task<ProductResultModel?> GetProduct(int id);
        public Task<int> Create(ProductCondition param);
        public Task AddCategory(List<int> param);
        public Task<bool> UpdateProduct(ProductCondition product);
        public Task<bool> Delete(int id);
        public Task DeleteProductImage(int productId, List<string> images);
        public Task<int?> GetProductStock(int productid);
        public Task<bool> RemoveProductStock(int quantity, int productid);
        public Task<bool> UpdateProdcutStatus(int productid, string status);
        public Task<IEnumerable<ProductCategoryResultModel>> GetProductCategory();

        public Task InsertProductImage(ProductImageResultModel image);
    }
}
