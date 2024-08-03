using BackendSystem.Respository.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackendSystem.Service.Interface
{
    public interface IProductRespository
    {
       
        public IEnumerable<ProductDataModel?> GetProductList();

        public Task<ProductDataModel?> GetProduct(int id);

        public Task<bool> Create(ProductCondition param);
        public Task<bool> UpdateProduct(ProductCondition product);
        public Task<bool> Delete(int id);
        public Task<bool> CheckProductStock(int quantity, int productid);
        public Task<bool> RemoveProductStock(int quantity, int productid);
    }
}
