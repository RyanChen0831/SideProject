using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Cors;
using BackendSystem.Dtos;
using BackendSystem.Service.Interface;
using System.Runtime.CompilerServices;

namespace BackendSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class LoginAPIController : ControllerBase
    {
        private readonly IMemberService _memberService;

        public LoginAPIController(IMemberService memberService)
        {
            _memberService = memberService;
        }

        [HttpPost]
        [EnableCors("MyAllowSpecificOrigins")]
        public async Task<IActionResult> Login(UserParameter user)
        {
            var userFromDb = await _memberService.GetMember(user.Account, user.Password);

            if (userFromDb != null)
            {
                var claims = new List<Claim>()
                {
                    new Claim(ClaimTypes.Name, userFromDb.Name),
                    new Claim(ClaimTypes.Role, userFromDb.Role),
                    new Claim(ClaimTypes.NameIdentifier, userFromDb.MemberId.ToString()) // 使用ClaimTypes.NameIdentifier存儲使用者ID
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true, // 是否持久化存儲Token
                    AllowRefresh = true, // 是否允許Cookie刷新
                };

                // 使用ClaimsPrincipal和AuthenticationProperties創建一個身份驗證Cookie
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

                ////加入一個標記使用者是否登入的Cookie(不放敏感資訊)
                //HttpContext.Response.Cookies.Append("LoginFlag", "true", new CookieOptions
                //{
                //    HttpOnly = false,
                //    Secure = false,
                //    SameSite = SameSiteMode.Lax
                //});

                return Ok();
            }

            return BadRequest();
        }

        [HttpDelete("Logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            //HttpContext.Response.Cookies.Delete("LoginFlag");//刪除標記
            return Ok();
        }

        [HttpPost("CheckLogin")]
        [EnableCors("MyAllowSpecificOrigins")]
        public IActionResult CheckLoginStatus()
        {           
            if (HttpContext.User.Identity?.IsAuthenticated == true)
            {
                return Ok(new { isLogin = true});
            }
            else
            {
                return Unauthorized(new { isLogin = false });
            }
        }

    }
}
