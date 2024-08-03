using AutoMapper;
using BackendSystem.Respository.Dtos;
using BackendSystem.Service.Dtos;
using BackendSystem.Service.Interface;
using BackendSystem.Service.ServiceMapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public Task<bool> CheckProductStock(int quantity, int productid)
        {
            return _productRespository.CheckProductStock(quantity, productid);
        }

        public async Task<bool> Create(ProductInfo param)
        {
            //先做Model的轉換
            var par = _mapper.Map<ProductInfo, ProductCondition>(param);
            //帶入Creat方法中
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
                var result = _mapper.Map<ProductDataModel?, ProductViewModel?>(data);
                return result;
            }
        }

        public IEnumerable<ProductViewModel?> GetProductList()
        {
            var data = _productRespository.GetProductList();
            var result = _mapper.Map<IEnumerable<ProductDataModel?>,IEnumerable<ProductViewModel?>>(data);

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
    }
}
