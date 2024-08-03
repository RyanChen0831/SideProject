using BackendSystem.Respository.Dtos;
using BackendSystem.Service.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackendSystem.Service.Interface
{
    public interface IShoppingCartService
    {
        public Task AddItemToCartAsync(int userId, string productId, int quantity);
        public Task<int?> GetCartItemQuantityAsync(int userId, string productId);
        public Task RemoveItemFromCartAsync(int userId, string productId);
        public Task ClearCartAsync(int userId);
        public Task<List<ShoppingCartViewModel?>> GetCartItemAsync(int userId);
    }
}
