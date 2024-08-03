using BackendSystem.Service.Dtos;
using BackendSystem.Service.Interface;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BackendSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]   
    public class MemberAPIController : ControllerBase
    {
        private readonly IMemberService _memberService;
        public MemberAPIController(IMemberService memberService)
        {
            _memberService = memberService;
        }

        [HttpPost("Register")]
        [EnableCors("MyAllowSpecificOrigins")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody]MemberInfo info )
        {
            var result = await _memberService.RegisterMember(info);
            if (result.IsSucceed == true)
            {
                return Ok();
            }
            else
            {
                return BadRequest(result.Message);
            }
        }

        [HttpPost("VerifyEmail")]
        [EnableCors("MyAllowSpecificOrigins")]
        [AllowAnonymous]
        public async Task<IActionResult> VerifyEmail([FromBody] string token)
        {
            var result = await _memberService.VerifyEmail(token);
            if (result.IsSucceed && result.Object!=null) {

                var claims = new List<Claim>()
                {
                    new Claim(ClaimTypes.Name, result.Object.Name),
                    new Claim(ClaimTypes.Role, result.Object.Role),
                    new Claim(ClaimTypes.NameIdentifier, result.Object.Id.ToString()) // 使用ClaimTypes.NameIdentifier存儲使用者ID
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = false, // 是否持久化存儲Token
                    AllowRefresh = false // 是否允許Token刷新
                };

                // 使用ClaimsPrincipal和AuthenticationProperties創建一個身份驗證Cookie
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

                //加入一個標記使用者是否登入的Cookie(不放敏感資訊)
                HttpContext.Response.Cookies.Append("LoginFlag", "true", new CookieOptions
                {
                    HttpOnly = false,
                    Secure = false,
                    SameSite = SameSiteMode.Lax
                });

                return Ok();
            }
            return BadRequest(result.Message);
        }

    }
}
