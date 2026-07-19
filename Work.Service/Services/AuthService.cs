using Microsoft.AspNetCore.Identity.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Work.Models.CommonData;
using Work.Models.EntityModels;
using Work.Models.RequestModels;
using Work.Models.ResponseModels;
using Work.Repository.IRepository;
using Work.Repository.Repository;
using Work.Service.IService;

namespace Work.Service.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _config;
        private readonly JwtSettings _jwtSettings;

        // 🎯 আপনার নিজস্ব কনস্ট্রাক্টর স্ট্রাকচার (যা এখন ১০০% সচল)
        public AuthService(IUserRepository userRepository, IConfiguration config, JwtSettings jwtSettings)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _jwtSettings = jwtSettings ?? throw new ArgumentNullException(nameof(jwtSettings));
        }
        public async Task<bool> RegisterAsync(RegisterRequestDto request)
        {
            
            var existingUser = await _userRepository.GetByUsernameOrEmailAsync(request.Email);
            if (existingUser != null) return false;

            var existingUsername = await _userRepository.GetByUsernameOrEmailAsync(request.Username);
            if (existingUsername != null) return false;           
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);            
            var newUser = new User
            {
                FullName = request.FullName,
                UserName = request.Username,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                PasswordHash = passwordHash,                
                EmailConfirmed = false,
                PhoneConfirmed = false,
                Status = UserStatus.Active,
                RoleIds = new List<string> { ObjectId.GenerateNewId().ToString() }, 
                RefreshTokens = new List<RefreshToken>(),
                FailedLoginAttempt = 0,
                LockoutEnabled = true,
                LockoutEnd = null
            };
            
            await _userRepository.CreateAsync(newUser);

            return true;
        }

        public async Task<AuthResponseDto> LoginAsync(LoginRequestDto dto, string ipAddress)
        {
           
            var user = await _userRepository.GetByUsernameOrEmailAsync(dto.Email);           
            if (user == null)
            {
                return new AuthResponseDto { Success = false, Message = "Invalid email or password" };
            }         
            if (user.LockoutEnabled && user.LockoutEnd.HasValue && user.LockoutEnd.Value > DateTime.Now)
            {
                var remainingMinutes = Math.Ceiling((user.LockoutEnd.Value - DateTime.Now).TotalMinutes);
                return new AuthResponseDto
                {
                    Success = false,
                    Message = $"Your account is locked due to multiple failed attempts. Try again after {remainingMinutes} minutes."
                };
            }

            bool isPasswordMatch = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);

            if (!isPasswordMatch)
            {
                
                user.FailedLoginAttempt += 1;
                if (user.FailedLoginAttempt >= 5) 
                {
                    user.LockoutEnd = DateTime.Now.AddMinutes(15);
                }

                await _userRepository.UpdateAsync(user);

                return new AuthResponseDto { Success = false, Message = "Invalid email or password" };
            }
           
            if (user.Status != UserStatus.Active)
            {
                return new AuthResponseDto { Success = false, Message = "Your account is inactive." };
            }

            var accessToken = GenerateAccessToken(user);
            var refreshToken = GenerateRefreshToken(user.Id!, ipAddress);
            await _userRepository.AddRefreshTokenAsync(user.Id!, refreshToken);
            user.LastLoginAt = DateTime.Now;
            user.LastLoginIp = ipAddress;
            user.RefreshTokens = new List<RefreshToken> { refreshToken };
            await _userRepository.ReplaceRefreshTokensAsync(user.Id!, user.LastLoginAt.Value, user.LastLoginIp, user.RefreshTokens);
            await _userRepository.UpdateAsync(user);
            return new AuthResponseDto
            {
                Success = true,
                AccessToken = accessToken,
                ExpireAt = DateTime.UtcNow.AddMinutes(15),
                Message = refreshToken.Token
            };
        }

        public async Task<AuthResponseDto> RefreshTokenAsync(string expiredAccessToken, string oldRefreshToken, string ipAddress)
        {
            var principal = GetPrincipalFromExpiredToken(expiredAccessToken);
            if (principal == null)
            {
                return new AuthResponseDto { Success = false, Message = "Invalid token session" };
            }

            var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return new AuthResponseDto { Success = false, Message = "User identity not found" };
            }

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return new AuthResponseDto { Success = false, Message = "User does not exist" };
            }

            var currentRefreshToken = user.RefreshTokens.FirstOrDefault(t => t.Token == oldRefreshToken);
            if (currentRefreshToken == null || !currentRefreshToken.IsActive)
            {
                return new AuthResponseDto { Success = false, Message = "Invalid or expired refresh token" };
            }

            var newAccessToken = GenerateAccessToken(user);
            var newRefreshToken = GenerateRefreshToken(user.Id!, ipAddress);

            await _userRepository.UpdateRefreshTokenAsync(user.Id!, oldRefreshToken, newRefreshToken);

            return new AuthResponseDto
            {
                Success = true,
                AccessToken = newAccessToken,
                ExpireAt = DateTime.Now.AddMinutes(15),
                Message = newRefreshToken.Token
            };
        }

        public async Task<bool> LogoutAsync(string expiredAccessToken, string refreshToken, string ipAddress)
        {
            var principal = GetPrincipalFromExpiredToken(expiredAccessToken);
            var userId = principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId)) return false;

            await _userRepository.RevokeRefreshTokenAsync(userId, refreshToken, ipAddress);
            return true;
        }
        public async Task<AuthResponseDto> RotateTokenAsync(string refreshToken, string ipAddress)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSettings.Secret);

            try
            {
                
                tokenHandler.ValidateToken(refreshToken, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false, 
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero 
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;              
                var userId = jwtToken.Claims.First(x => x.Type == "userId").Value;
                var tokenType = jwtToken.Claims.First(x => x.Type == "tokenType").Value;               
                if (tokenType != "refresh")
                {
                    return new AuthResponseDto { Success = false, Message = "Invalid token token type." };
                }
                               
                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null || user.Status != UserStatus.Active)
                {
                    return new AuthResponseDto { Success = false, Message = "User is inactive or no longer exists." };
                }
               
                var newAccessToken = GenerateAccessToken(user);
                var newRefreshToken = GenerateRefreshToken(user.Id, ipAddress); 

                return new AuthResponseDto
                {
                    Success = true,
                    AccessToken = newAccessToken,
                    RefreshToken = newRefreshToken.Token, 
                    ExpireAt = DateTime.Now.AddMinutes(15),
                    Message = "Token rotated successfully."
                };
            }
            catch (Exception)
            {
               
                return new AuthResponseDto { Success = false, Message = "Invalid or expired refresh token." };
            }
        }
        #region Private Token Helper Methods
        private string GenerateAccessToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id!),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email)
            };

            foreach (var roleId in user.RoleIds)
            {
                claims.Add(new Claim(ClaimTypes.Role, roleId));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JwtSettings:Secret"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["JwtSettings:Issuer"],
                audience: _config["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(15),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private RefreshToken GenerateRefreshToken(string userId, string ipAddress)
        {
            return new RefreshToken
            {
                Id = ObjectId.GenerateNewId().ToString(),
                UserId = userId,
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                Expires = DateTime.Now.AddDays(7),
                CreatedByIp = ipAddress
            };
        }

        private ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JwtSettings:Secret"]!)),
                ValidateLifetime = false
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
                if (securityToken is not JwtSecurityToken jwtSecurityToken ||
                    !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                    return null;

                return principal;
            }
            catch
            {
                return null;
            }
        }
        #endregion
    }
}

