using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Work.Models.EntityModels;

namespace Work.Repository.IRepository
{
    public interface IUserRepository
    {
        Task CreateAsync(User user);
        Task<User?> GetByIdAsync(string id);
        Task<User?> GetByUsernameOrEmailAsync(string credential);
        Task UpdateAsync(User user);

        // Refresh Token Management Operations
        Task AddRefreshTokenAsync(string userId, RefreshToken token);
        Task UpdateRefreshTokenAsync(string userId, string oldToken, RefreshToken newToken);
        Task RevokeRefreshTokenAsync(string userId, string token, string ipAddress, string? replacedByToken = null);
        Task ReplaceRefreshTokensAsync(string userId, DateTime lastLoginAt, string ipAddress, List<RefreshToken> newTokens);
    }
}
