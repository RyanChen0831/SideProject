using BackendSystem.Service.Implement;
using BackendSystem.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;


namespace BackendSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    [EnableCors("MyAllowSpecificOrigins")]
    public class ShoppingCartAPIController : ControllerBase
    {
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IMemberService _memberService;
        public ShoppingCartAPIController(IShoppingCartService shoppingCartService, IMemberService memberService)
        {
            _shoppingCartService = shoppingCartService;
            _memberService = memberService;
        }

        [HttpPost]
        public async Task<IActionResult> AddItemToCart( string productId, int quantity)
        {
            var userId = _memberService.GetUserId();
            if (userId == null) {
                return Unauthorized();
            }
            await _shoppingCartService.AddItemToCartAsync(userId.Value, productId, quantity);
            return Ok();
        }

        [HttpGet("itemcount")]
        public async Task<IActionResult> GetCartItemCount(int userId, string productId)
        {
            var itemCount = await _shoppingCartService.GetCartItemQuantityAsync(userId, productId);
            return Ok(itemCount);
        }

        [HttpGet("item")]
        public async Task<IActionResult> GetCartItem(int userId)
        {
            var itemCount = await _shoppingCartService.GetCartItemAsync(userId);
            return Ok(itemCount);
        }

        [HttpDelete]
        public async Task<IActionResult> RemoveFromCart(int userId, string productId)
        {
            await _shoppingCartService.RemoveItemFromCartAsync(userId, productId);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> ClearCart(int userId)
        {
            await _shoppingCartService.ClearCartAsync(userId);
            return Ok();
        }


    }
}
