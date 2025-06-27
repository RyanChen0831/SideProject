using BackendSystem.Respository.Interface;
using BackendSystem.Respository.ResultModels;
using Newtonsoft.Json;
using StackExchange.Redis;

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
            string cartKey = GetRedisKey(memberId);
            int productId = cart.ProductId;
            await _database.HashSetAsync(cartKey, productId, JsonConvert.SerializeObject(cart));
        }

        public async Task<bool> UpdateCartItemAsync(int memberId, List<ShoppingCartResultModel> cart)
        {
            foreach (var item in cart)
            {
                var currentItem = JsonConvert.SerializeObject(item);
                var productId = item.ProductId;
                string cartKey = GetRedisKey(memberId);
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
            string cartKey = GetRedisKey(memberId);
            var cartItems = await _database.HashGetAllAsync(cartKey);
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
            string cartKey = GetRedisKey(memberId);
            await _database.HashDeleteAsync(cartKey, productId);
        }

        public async Task ClearCartAsync(int memberId)
        {
            string cartKey = GetRedisKey(memberId);
            await _database.KeyDeleteAsync(cartKey);
        }

        private const string RedisCartKeyPattern = "cart:{0}";
        private string GetRedisKey(int memberId) => string.Format(RedisCartKeyPattern, memberId);
    }
}
