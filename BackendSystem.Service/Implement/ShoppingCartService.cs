using AutoMapper;
using BackendSystem.Respository.Interface;
using BackendSystem.Respository.ResultModel;
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

        public async Task AddItemToCartAsync(int memberId, ShoppingCartViewModel cart)
        {
            var addItem = _mapper.Map<ShoppingCartViewModel, ShoppingCartResultModel>(cart);
            await _shoppingCartRepository.AddItemToCartAsync(memberId, addItem);
        }

        public async Task<int?> GetCartItemQuantityAsync(int memberId, int productId)
        {
            return await _shoppingCartRepository.GetCartItemQuantityAsync(memberId, productId);
        }

        public async Task RemoveItemFromCartAsync(int memberId, int productId)
        {
            await _shoppingCartRepository.RemoveItemFromCartAsync(memberId, productId);
        }

        public async Task ClearCartAsync(int memberId)
        {
            await _shoppingCartRepository.ClearCartAsync(memberId);
        }

        public async Task<List<ShoppingCartViewModel?>> GetCartItemAsync(int memberId)
        {
             var value = await _shoppingCartRepository.GetCartItemAsync(memberId);
             var result = _mapper.Map<IEnumerable<ShoppingCartResultModel?>, IEnumerable<ShoppingCartViewModel?>>(value);
            return result.ToList();
        }

        public async Task<bool> UpdateCartItemAsync(int memberId, List<ShoppingCartViewModel> cart)
        {
            var cartList = _mapper.Map<List<ShoppingCartResultModel>>(cart);
            var value = await _shoppingCartRepository.UpdateCartItemAsync(memberId, cartList);
            return true;
        }
    }
}
