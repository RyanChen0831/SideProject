
using BackendSystem.Respository.Dtos;
using BackendSystem.Service.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackendSystem.Service.Interface
{
    public interface IProductService
    {

        public IEnumerable<ProductViewModel?> GetProductList();

        public Task<ProductViewModel?> GetProduct(int id);

        public Task<bool> Create(ProductInfo param);

        public Task<bool> Delete(int id);

        public Task<bool> CheckProductStock(int quantity, int productid);

        public Task<bool> UpdateProduct(ProductInfo product);

        public Task<bool> RemoveProductStock(int quantity, int productid);

    }
}
