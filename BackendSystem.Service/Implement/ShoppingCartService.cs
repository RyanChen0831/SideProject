using AutoMapper;
using BackendSystem.Respository.Dtos;
using BackendSystem.Respository.Interface;
using BackendSystem.Service.Dtos;
using BackendSystem.Service.Interface;

namespace BackendSystem.Service.Implement
{
    public class ShoppingCartService:IShoppingCartService
    {
        private readonly IShoppingCartRespository _shoppingCartRepository;
        private readonly IMapper _mapper;
        public ShoppingCartService(IShoppingCartRespository shoppingCart,IMapper mapper)
        {
            _shoppingCartRepository = shoppingCart;
            _mapper = mapper;
        }

        public async Task AddItemToCartAsync(string userId, string productId, int quantity)
        {
            await _shoppingCartRepository.AddItemToCartAsync(userId, productId, quantity);
        }

        public async Task<int?> GetCartItemQuantityAsync(string userId, string productId)
        {
            return await _shoppingCartRepository.GetCartItemQuantityAsync(userId, productId);
        }

        public async Task RemoveItemFromCartAsync(string userId, string productId)
        {
            await _shoppingCartRepository.RemoveItemFromCartAsync(userId, productId);
        }

        public async Task ClearCartAsync(int userId)
        {
            await _shoppingCartRepository.ClearCartAsync(userId);
        }

        public async Task<List<ShoppingCartViewModel?>> GetCartItemAsync(int userId)
        {
             var value = await _shoppingCartRepository.GetCartItemAsync(userId);
             var result = _mapper.Map<IEnumerable<ShoppingCartDataModel?>, IEnumerable<ShoppingCartViewModel?>>(value);
            return result.ToList();
        }
    }
}
