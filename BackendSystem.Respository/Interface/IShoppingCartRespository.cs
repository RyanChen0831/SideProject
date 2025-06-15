using BackendSystem.Respository.ResultModel;

namespace BackendSystem.Respository.Interface
{
    public interface IShoppingCartRespository
    {
        public Task AddItemToCartAsync(int memberId, ShoppingCartResultModel cart);
        public Task<int?> GetCartItemQuantityAsync(int memberId, int productId);
        public Task RemoveItemFromCartAsync(int memberId, int productId);
        public Task ClearCartAsync(int memberId);
        public Task<List<ShoppingCartResultModel>> GetCartItemAsync(int memberId);
        public Task<bool> UpdateCartItemAsync(int memberId, List<ShoppingCartResultModel> cart);
    }
}
