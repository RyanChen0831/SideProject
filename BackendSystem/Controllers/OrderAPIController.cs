using BackendSystem.Service.Interface;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace BackendSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("MyAllowSpecificOrigins")]
    public class OrderAPIController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IMemberService _memberService;
        public OrderAPIController(IOrderService orderService, IMemberService memberService)
        {
            _orderService = orderService;
            _memberService = memberService;

        }
        [HttpGet]
        public async Task<IActionResult> GetOrder()
        {
            int? memberId = _memberService.GetMemberId();

            if (memberId == null)
            {
                return BadRequest();
            }
            var orderList = await _orderService.GetOrder(memberId.Value);

            return Ok(orderList);

        }



    }
}
