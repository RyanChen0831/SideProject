using BackendSystem.Service.Dtos;

namespace BackendSystem.Service.Interface
{
    public interface IShoppingCartService
    {
        public Task AddItemToCartAsync(int memberId, ShoppingCartViewModel cart);
        public Task<int?> GetCartItemQuantityAsync(int memberId, int productId);
        public Task RemoveItemFromCartAsync(int memberId, int productId);
        public Task ClearCartAsync(int memberId);
        public Task<List<ShoppingCartViewModel?>> GetCartItemAsync(int memberId);
        public Task<bool> UpdateCartItemAsync(int memberId, List<ShoppingCartViewModel> cart);
    }
}
