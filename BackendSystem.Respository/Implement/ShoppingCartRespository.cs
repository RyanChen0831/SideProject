using AutoMapper.Execution;
using BackendSystem.Respository.Interface;
using BackendSystem.Respository.ResultModels;
using Newtonsoft.Json;
using StackExchange.Redis;
using System.Data;

namespace BackendSystem.Respository.Implement
{
    public class ShoppingCartRespository : IShoppingCartRespository
    {
        private readonly IDatabase _database;
        public ShoppingCartRespository(ConnectionMultiplexer redis)
        {
            _database = redis.GetDatabase();
        }

        public async Task AddItemToCartAsync(int memberId, ShoppingCartResultModel cart)
        {
            var cartKey = $"cart:{memberId}";
            var productId = cart.ProductId;

            var existingItem = await _database.HashGetAsync(cartKey, productId);
            if (existingItem.HasValue)
            {
                var currentItem = JsonConvert.DeserializeObject<ShoppingCartResultModel>(existingItem);
                currentItem.Quantity += cart.Quantity;
                currentItem.SubTotal = currentItem.Quantity * currentItem.Price;

                await db.HashSetAsync(cartKey, productId, JsonConvert.SerializeObject(currentItem));
            }
            else
            {
                await db.HashSetAsync(cartKey, productId, JsonConvert.SerializeObject(cart));
            }
        }

        public async Task<bool> UpdateCartItemAsync(int memberId, List<ShoppingCartResultModel> cart)
        {
            var cartKey = $"cart:{memberId}";
            foreach (var item in cart)
            {
                var currentItem = JsonConvert.SerializeObject(item);
                var productId = item.ProductId;
                await _database.HashSetAsync(cartKey, productId, currentItem);
            }
            return true;
        }


        public async Task<int?> GetCartItemQuantityAsync(int memberId, int productId)
        {
            var value = await _database.HashGetAsync($"cart:{memberId}", productId);
            if (value.HasValue && int.TryParse(value.ToString(), out int quantity))
                return quantity;
            return null;
        }

        public async Task<List<ShoppingCartResultModel>> GetCartItemAsync(int memberId)
        {
            var cartItems = await _database.HashGetAllAsync($"cart:{memberId}");
            var productlist = new List<ShoppingCartResultModel>();

            foreach (var item in cartItems)
            {
                if (item.Value.HasValue)
                {
                    var product = JsonConvert.DeserializeObject<ShoppingCartResultModel>(item.Value);

                    if (product != null)
                    {
                        productlist.Add(product);
                    }
                }
            }

            return productlist;
        }

        public async Task RemoveItemFromCartAsync(int memberId, int productId)
        {
            await _database.HashDeleteAsync(GetRedisKey(memberId), productId);
        }

        public async Task ClearCartAsync(int memberId)
        {
            await _database.KeyDeleteAsync(GetRedisKey(memberId));
        }

        private const string RedisCartKeyPattern = "cart:{0}";
        private string GetRedisKey(int memberId) => string.Format(RedisCartKeyPattern, memberId);
    }
}
