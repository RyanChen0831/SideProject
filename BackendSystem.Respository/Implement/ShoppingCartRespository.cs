using BackendSystem.Respository.Dtos;
using BackendSystem.Respository.Interface;
using BackendSystem.Service.Interface;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace BackendSystem.Respository.Implement
{
    public class ShoppingCartRespository: IShoppingCartRespository
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
        /// <param name="userId"></param>
        /// <param name="productId"></param>
        /// <param name="quantity"></param>
        /// <returns></returns>
        public async Task AddItemToCartAsync(string userId, string productId, int quantity)
        {
            var db = _redis.GetDatabase();
            if (await db.HashExistsAsync($"cart:{userId}", productId))
            {
                var value = await GetCartItemQuantityAsync(userId, productId);
                var amount = value + quantity;
                await db.HashSetAsync($"cart:{userId}", productId, amount);
            }
            else
            {
                await db.HashSetAsync($"cart:{userId}", productId, quantity);
            }
        }

        public async Task<int?> GetCartItemQuantityAsync(string userId, string productId)
        {
            var db = _redis.GetDatabase();
            var value = await db.HashGetAsync($"cart:{userId}", productId);
            return (int?)value;
        }

        /// <summary>
        /// 取User的CartItem
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>產品ID及數量</returns>
        public async Task<List<ShoppingCartDataModel>> GetCartItemAsync(int userId)
        {
            var db = _redis.GetDatabase();
            var value = await db.HashGetAllAsync($"cart:{userId}");
            var productlist = new List<ShoppingCartDataModel>();
            foreach (var item in value) {
                var product = new ShoppingCartDataModel
                {
                    ProductID = (int)item.Name,
                    Quantity = (int)item.Value
                };
                productlist.Add(product);
            }
            return productlist;
        }

        public async Task RemoveItemFromCartAsync(string userId, string productId)
        {
            var db = _redis.GetDatabase();
            await db.HashDeleteAsync($"cart:{userId}", productId);
        }

        public async Task ClearCartAsync(int userId)
        {
            var db = _redis.GetDatabase();
            await db.KeyDeleteAsync($"cart:{userId}");
        }
    }
}
