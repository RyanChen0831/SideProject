using BackendSystem.Respository.Interface;
using BackendSystem.Respository.ResultModel;
using Newtonsoft.Json;
using StackExchange.Redis;
using System.Data;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace BackendSystem.Respository.Implement
{
    public class ShoppingCartRespository : IShoppingCartRespository
    {
        private readonly ConnectionMultiplexer _redis;
        private readonly IDbConnection _dbConnection;
        private readonly IDatabase _database;
        public ShoppingCartRespository(IDbConnection dbConnection, ConnectionMultiplexer redis)
        {
            _redis = redis;
            _dbConnection = dbConnection;
            _database = _redis.GetDatabase();
        }
        /// <summary>
        /// 加入購物車，如果商品已存在購物車則修改數量即可。
        /// </summary>
        /// <param name="memberId"></param>
        /// <param name="productId"></param>
        /// <param name="quantity"></param>
        /// <returns></returns>  
        public async Task AddItemToCartAsync(int memberId, ShoppingCartResultModel cart)
        {
            var db = _redis.GetDatabase();
            var cartKey = $"cart:{memberId}";
            var productId = cart.ProductId;

            var existingItem = await db.HashGetAsync(cartKey, productId);
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
            var db = _redis.GetDatabase();
            var cartKey = $"cart:{memberId}";
            foreach (var item in cart)
            {
                var currentItem = JsonConvert.SerializeObject(item);
                var productId = item.ProductId;
                await db.HashSetAsync(cartKey, productId, currentItem);
            }
            return true;
        }


        public async Task<int?> GetCartItemQuantityAsync(int memberId, int productId)
        {
            var db = _redis.GetDatabase();
            var value = await db.HashGetAsync($"cart:{memberId}", productId);
            return (int?)value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="memberId"></param>
        /// <returns></returns>
        public async Task<List<ShoppingCartResultModel>> GetCartItemAsync(int memberId)
        {
            var db = _redis.GetDatabase();
            var cartItems = await db.HashGetAllAsync($"cart:{memberId}");
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
            var db = _redis.GetDatabase();
            await db.HashDeleteAsync($"cart:{memberId}", productId);
        }

        public async Task ClearCartAsync(int memberId)
        {
            var db = _redis.GetDatabase();
            await db.KeyDeleteAsync($"cart:{memberId}");
        }
    }
}
