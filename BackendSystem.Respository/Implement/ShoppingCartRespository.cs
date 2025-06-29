using BackendSystem.Respository.Interface;
using BackendSystem.Respository.ResultModels;
using Newtonsoft.Json;
using StackExchange.Redis;
using BackendSystem.Common;

namespace BackendSystem.Respository.Implement
{
    public class ShoppingCartRespository : IShoppingCartRespository
    {
        private readonly IDatabase _database;
        private const string RedisCartKeyPattern = "cart:{0}";
        private string GetRedisKey(int memberId) => string.Format(RedisCartKeyPattern, memberId);
        public ShoppingCartRespository(ConnectionMultiplexer redis)
        {
            _database = redis.GetDatabase();
        }

        public async Task AddItemToCartAsync(int memberId, ShoppingCartResultModel cart)
        {
            string cartKey = GetRedisKey(memberId);
            int productId = cart.ProductId;
            string currentItem = JsonConvert.SerializeObject(cart);
            await _database.HashSetAsync(cartKey, productId, currentItem);
        }

        public async Task<bool> UpdateCartItemAsync(int memberId, List<ShoppingCartResultModel> cart)
        {
            string cartKey = GetRedisKey(memberId);
            var entries = cart.Select(item =>
                new HashEntry(item.ProductId, JsonConvert.SerializeObject(item))
            ).ToArray();
            await _database.HashSetAsync(cartKey, entries);
            return true;
        }
        public async Task<int?> GetCartItemQuantityAsync(int memberId, int productId)
        {
            var value = await _database.HashGetAsync(GetRedisKey(memberId), productId);
            if (!value.IsNullOrEmpty)
            {
                var json = value.ToString();
                if (JsonSafe.TryDeserialize<ShoppingCartResultModel>(json, out var cartItem))
                {
                    return cartItem!.Quantity;
                }
            }
            return null;
        }

        public async Task<List<ShoppingCartResultModel>> GetCartItemAsync(int memberId)
        {
            string cartKey = GetRedisKey(memberId);
            var cartItems = await _database.HashGetAllAsync(cartKey);
            var productlist = new List<ShoppingCartResultModel>();
            foreach (var item in cartItems)
            {
                if (!item.Value.IsNullOrEmpty)
                {
                    string str = item.Value.ToString();
                    if (JsonSafe.TryDeserialize<ShoppingCartResultModel>(str, out var result, out var errorMessage))
                        productlist.Add(result!);
                }
            }
            return productlist;
        }

        public async Task RemoveItemFromCartAsync(int memberId, int productId)
        {
            string cartKey = GetRedisKey(memberId);
            await _database.HashDeleteAsync(cartKey, productId);
        }

        public async Task ClearCartAsync(int memberId)
        {
            string cartKey = GetRedisKey(memberId);
            await _database.KeyDeleteAsync(cartKey);
        }

    }
}
