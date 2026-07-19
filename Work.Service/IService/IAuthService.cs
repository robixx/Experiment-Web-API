using Microsoft.AspNetCore.Identity.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Work.Models.EntityModels;
using Work.Models.RequestModels;
using Work.Models.ResponseModels;

namespace Work.Service.IService
{
    public interface IAuthService
    {
        Task<bool> RegisterAsync(RegisterRequestDto request);
        Task<AuthResponseDto> LoginAsync(LoginRequestDto dto, string ipAddress);
        Task<AuthResponseDto> RefreshTokenAsync(string expiredAccessToken, string oldRefreshToken, string ipAddress);
        Task<bool> LogoutAsync(string expiredAccessToken, string refreshToken, string ipAddress);
        Task<AuthResponseDto> RotateTokenAsync(string refreshToken, string ipAddress);
    }
}
