using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Work.Models.CommonData;

namespace Work.Models.EntityModels
{
    public class User : BaseEntity
    {
        [BsonElement("fullName")]
        public string FullName { get; set; } = default!;

        [BsonElement("userName")]
        public string UserName { get; set; } = default!;

        [BsonElement("email")]
        public string Email { get; set; } = default!;

        [BsonElement("phoneNumber")]
        public string PhoneNumber { get; set; } = default!;

        [BsonElement("passwordHash")]
        public string PasswordHash { get; set; } = default!;

        [BsonElement("emailConfirmed")]
        public bool EmailConfirmed { get; set; } = false;

        [BsonElement("phoneConfirmed")]
        public bool PhoneConfirmed { get; set; } = false;

        [BsonElement("status")]
        [BsonRepresentation(BsonType.String)] // ডাটাবেজে ১, ২ না লিখে 'Active', 'Suspended' হিসেবে সেভ হবে
        public UserStatus Status { get; set; } = UserStatus.Active;

        [BsonElement("roleIds")]
        [BsonRepresentation(BsonType.ObjectId)] // রোলের আইডিগুলোকেও মঙ্গো অবজেক্ট আইডি হিসেবে ট্রিক করবে
        public List<string> RoleIds { get; set; } = new();

        [BsonElement("refreshTokens")]
        public List<RefreshToken> RefreshTokens { get; set; } = new();

        [BsonElement("lastLoginAt")]
        public DateTime? LastLoginAt { get; set; }

        [BsonElement("lastLoginIp")]
        public string? LastLoginIp { get; set; }

        [BsonElement("failedLoginAttempt")]
        public int FailedLoginAttempt { get; set; }

        [BsonElement("lockoutEnabled")]
        public bool LockoutEnabled { get; set; } = true;

        [BsonElement("lockoutEnd")]
        public DateTime? LockoutEnd { get; set; }
    }
}
