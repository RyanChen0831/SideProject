using BackendSystem.Dtos;
using BackendSystem.Service.Dtos;
using BackendSystem.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;


namespace BackendSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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
        public async Task<IActionResult> AddItemToCart([FromBody] ShoppingCartViewModel cart)
        {
            var memberId = _memberService.GetMemberId();
            if (memberId == null) {
                return Unauthorized("請先登入會員");
            }
            await _shoppingCartService.AddItemToCartAsync(memberId.Value, cart);
            return Ok();
        }

        [HttpGet("itemcount")]
        public async Task<IActionResult> GetCartItemCount(int productId)
        {
            var memberId = _memberService.GetMemberId();
            if (memberId == null)
            {
                return NotFound();
            }
            var itemCount = await _shoppingCartService.GetCartItemQuantityAsync(memberId.Value, productId);
            return Ok(itemCount);
        }

        [HttpGet("GetCart")]
        public async Task<IActionResult> GetCartItem()
        {
            var memberId = _memberService.GetMemberId();
            if (memberId == null)
            {
                return NotFound();
            }
            var itemCount = await _shoppingCartService.GetCartItemAsync(memberId.Value);
            return Ok(itemCount);
        }

        [HttpDelete("RemoveCartItem/{productId}")]
        public async Task<IActionResult> RemoveFromCart(int productId)
        {
            var memberId = _memberService.GetMemberId();
            if (memberId == null)
            {
                return NotFound();
            }
            await _shoppingCartService.RemoveItemFromCartAsync(memberId.Value, productId);
            return Ok();
        }

        [HttpDelete("ClearShoppinCart")]
        public async Task<IActionResult> ClearCart()
        {
            var memberId = _memberService.GetMemberId();
            if (memberId == null)
            {
                return NotFound();
            }
            await _shoppingCartService.ClearCartAsync(memberId.Value);
            return Ok();
        }
        [HttpPut]
        public async Task<IActionResult> UpdateCartItem([FromBody] List<ShoppingCartViewModel> cart) {

            var memberId = _memberService.GetMemberId();
            if (memberId == null)
            {
                return NotFound();
            }
            await _shoppingCartService.UpdateCartItemAsync(memberId.Value, cart);
            return Ok();
        }

    }

}
