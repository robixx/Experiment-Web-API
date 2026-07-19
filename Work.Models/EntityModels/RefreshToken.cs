using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Work.Models.EntityModels
{
    public class RefreshToken : BaseEntity
    {
        public string UserId { get; set; } = default!;

        public string Token { get; set; } = default!;

        public DateTime Expires { get; set; }

        public DateTime? Revoked { get; set; }

        public string? ReplacedByToken { get; set; }

        public string CreatedByIp { get; set; } = default!;

        public string? RevokedByIp { get; set; }

        public bool IsExpired => DateTime.UtcNow >= Expires;

        public bool IsRevoked => Revoked != null;

        public bool IsActive => !IsExpired && !IsRevoked;
    }
}
