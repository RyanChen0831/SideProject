using AutoMapper;
using BackendSystem.Respository.Dtos;
using BackendSystem.Respository.Implement;
using BackendSystem.Respository.ResultModel;
using BackendSystem.Service.Dtos;
using BackendSystem.Service.Interface;

namespace BackendSystem.Service.Implement
{
    public class ProductService : IProductService
    {
        private readonly IProductRespository _productRespository;
        private readonly IMapper _mapper;
        public ProductService(IProductRespository productRespository, IMapper mapper)
        {
            _productRespository = productRespository;
            _mapper = mapper;
        }

        public async Task<bool> CheckProductStock(int quantity, int productid)
        {
            var stock = await _productRespository.GetProductStock(productid);

            if (!stock.HasValue || quantity > stock.Value)
            {
                return false;
            }
            return true;

        }

        public async Task<int> Create(ProductInfo param)
        {
            var par = _mapper.Map<ProductCondition>(param);
            return await _productRespository.Create(par);
        }

        public async Task<bool> Delete(int id)
        {
            return await _productRespository.Delete(id);
        }

        public async Task<ProductViewModel?> GetProduct(int id)
        {
            var data = await _productRespository.GetProduct(id);

            // 檢查商品是否存在
            if (data == null)
            {
                // 如果商品不存在，返回null
                return null;
            }
            else
            {
                var result = _mapper.Map<ProductResultModel?, ProductViewModel?>(data);
                return result;
            }
        }

        public async Task<IEnumerable<ProductViewModel?>> GetProductList()
        {
            var data = await _productRespository.GetProductList();
            var result = _mapper.Map<IEnumerable<ProductResultModel?>, IEnumerable<ProductViewModel?>>(data);

            return result;
        }

        public async Task<bool> UpdateProduct(ProductInfo product)
        {
            var param = _mapper.Map<ProductInfo, ProductCondition>(product);

            return await _productRespository.UpdateProduct(param);
        }

        public async Task<bool> RemoveProductStock(int quantity, int productid)
        {
            try
            {
                await _productRespository.RemoveProductStock(quantity, productid);
                return true;
            }
            catch (Exception)
            {
                return false;
                throw;
            }
        }

        public async Task<bool> UpdateProdcutStatus(int productid, string status)
        {
            var res = await _productRespository.UpdateProdcutStatus(productid, status);

            if (res)
            {
                return true;
            }
            else { 
                return false; 
            }

        }

        public async Task<IEnumerable<ProductCategoryViewModel>> GetProductCategory()
        {
            var res = await _productRespository.GetProductCategory();

            return _mapper.Map<IEnumerable<ProductCategoryViewModel>>(res);

        }

        public async Task DeleteProductImage(int productId, List<string> images)
        {
            try
            {
                await _productRespository.DeleteProductImage(productId, images);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
