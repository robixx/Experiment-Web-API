using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Work.Models.CommonData;
using Work.Models.RequestModels;
using Work.Models.ResponseModels;
using Work.Service.IService;

namespace WorkExperianceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _authService.RegisterAsync(request);

            if (!result)
            {
                return BadRequest(new { message = "Username or Email is already taken." });
            }

            return Ok(new { message = "Registration successful! You can now log in." });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto dto)
        {
            var ipAddress = GetIpAddress();           
            var result = await _authService.LoginAsync(dto, ipAddress);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            var refreshToken = result.Message;
            SetRefreshTokenCookie(refreshToken, DateTime.Now.AddDays(7));

            result.Message = "Login successful";
            return Ok(result);
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDto dto)
        {
            if (!Request.Cookies.TryGetValue("refreshToken", out var oldToken))
            {
                return BadRequest(new AuthResponseDto { Success = false, Message = "Refresh token is missing from cookies" });
            }

            var ipAddress = GetIpAddress();
            var result = await _authService.RefreshTokenAsync(dto.ExpiredAccessToken, oldToken!, ipAddress);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            var newRefreshToken = result.Message;
            SetRefreshTokenCookie(newRefreshToken, DateTime.UtcNow.AddDays(7));

            result.Message = "Token refreshed successfully";
            return Ok(result);
        }

        [HttpPost("auto-refresh-token")]
        public async Task<IActionResult> RefreshToken()
        {
           
            var refreshToken = Request.Cookies["refreshToken"];

            if (string.IsNullOrEmpty(refreshToken))
            {
                return Unauthorized(new { Message = "Refresh token is missing. Please login again." });
            }
            var ipAddress = GetIpAddress(); 
            var result = await _authService.RotateTokenAsync(refreshToken, ipAddress);
            if (!result.Success)
            {
               
                Response.Cookies.Delete("refreshToken");
                return Unauthorized(result);
            }
          
            SetRefreshTokenCookie(result.RefreshToken, result.ExpireAt);
           
            return Ok(new
            {
                result.Success,
                result.AccessToken,
                result.ExpireAt,
                result.Message
            });
        }


        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] RefreshTokenRequestDto dto)
        {
            if (Request.Cookies.TryGetValue("refreshToken", out var token))
            {
                var ipAddress = GetIpAddress();
                await _authService.LogoutAsync(dto.ExpiredAccessToken, token!, ipAddress);
            }

            Response.Cookies.Delete("refreshToken");
            return Ok(new { success = true, message = "Logged out successfully" });
        }

        private void SetRefreshTokenCookie(string token, DateTime expires)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = expires
            };
            Response.Cookies.Append("refreshToken", token, cookieOptions);
        }

        private string GetIpAddress()
        {
            if (Request.Headers.ContainsKey("X-Forwarded-For"))
                return Request.Headers["X-Forwarded-For"]!;

            return HttpContext.Connection.RemoteIpAddress?.MapToIPv4().ToString() ?? "127.0.0.1";
        }
    }
}
