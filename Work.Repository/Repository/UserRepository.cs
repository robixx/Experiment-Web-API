using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Work.Models.EntityModels;
using Work.Repository.IRepository;
using BCrypt.Net;

namespace Work.Repository.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly IMongoCollection<User> _users;

        public UserRepository(IMongoDatabase database)
        {
           
            _users = database.GetCollection<User>("Users");
        }

        public async Task CreateAsync(User user)
        {
            await _users.InsertOneAsync(user);
        }

       
        public Task<User?> GetByIdAsync(string id) =>
            _users.Find(u => u.Id == id).FirstOrDefaultAsync()!;

        
        public Task<User?> GetByUsernameOrEmailAsync(string credential) =>
            _users.Find(u => u.UserName == credential || u.Email == credential).FirstOrDefaultAsync()!;

       
        public Task UpdateAsync(User user) =>
            _users.ReplaceOneAsync(u => u.Id == user.Id, user);

        public async Task ReplaceRefreshTokensAsync(string userId, DateTime lastLoginAt, string ipAddress, List<RefreshToken> newTokens)
        {
            var objectId = MongoDB.Bson.ObjectId.Parse(userId);
            var filter = Builders<User>.Filter.Eq("_id", objectId);

            // $set অপারেটরের মাধ্যমে সম্পূর্ণ RefreshTokens অ্যারেটিকে নতুন লিস্ট দিয়ে ওভাররাইট করা হলো
            var update = Builders<User>.Update
                .Set(u => u.LastLoginAt, lastLoginAt)
                .Set(u => u.LastLoginIp, ipAddress)
                .Set(u => u.RefreshTokens, newTokens);

            await _users.UpdateOneAsync(filter, update);
        }
        public async Task AddRefreshTokenAsync(string userId, RefreshToken token)
        {
            var filter = Builders<User>.Filter.Eq(u => u.Id, userId);           
            var update = Builders<User>.Update.Push(u => u.RefreshTokens, token);

            await _users.UpdateOneAsync(filter, update);
        }

       
        public async Task UpdateRefreshTokenAsync(string userId, string oldToken, RefreshToken newToken)
        {
            
            var filter = Builders<User>.Filter.And(
                Builders<User>.Filter.Eq(u => u.Id, userId),
                Builders<User>.Filter.ElemMatch(u => u.RefreshTokens, t => t.Token == oldToken)
            );

           
            var update = Builders<User>.Update
                .Set("RefreshTokens.$.Revoked", DateTime.UtcNow)
                .Set("RefreshTokens.$.RevokedByIp", newToken.CreatedByIp)
                .Set("RefreshTokens.$.ReplacedByToken", newToken.Token)
                .Push(u => u.RefreshTokens, newToken); 

            await _users.UpdateOneAsync(filter, update);
        }
 
        public async Task RevokeRefreshTokenAsync(string userId, string token, string ipAddress, string? replacedByToken = null)
        {
            var filter = Builders<User>.Filter.And(
                Builders<User>.Filter.Eq(u => u.Id, userId),
                Builders<User>.Filter.ElemMatch(u => u.RefreshTokens, t => t.Token == token)
            );

            var update = Builders<User>.Update
                .Set("RefreshTokens.$.Revoked", DateTime.UtcNow)
                .Set("RefreshTokens.$.RevokedByIp", ipAddress)
                .Set("RefreshTokens.$.ReplacedByToken", replacedByToken);

            await _users.UpdateOneAsync(filter, update);
        }
    }
}
