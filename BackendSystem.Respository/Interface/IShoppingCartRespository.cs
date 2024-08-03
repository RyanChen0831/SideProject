using BackendSystem.Respository.Dtos;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackendSystem.Respository.Interface
{
    public interface IShoppingCartRespository
    {
        public Task AddItemToCartAsync(string userId, string productId, int quantity);
        public Task<int?> GetCartItemQuantityAsync(string userId, string productId);
        public Task RemoveItemFromCartAsync(string userId, string productId);
        public Task ClearCartAsync(int userId);
        public Task<List<ShoppingCartDataModel>> GetCartItemAsync(int userId);
    }
}
