using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Work.Models.EntityModels;

namespace Work.Models.ResponseModels
{
    public class AuthResponseDto
    {
        public bool Success { get; set; }

        public string AccessToken { get; set; } = string.Empty;

        public string RefreshToken { get; set; }= string.Empty!;

        public DateTime ExpireAt { get; set; }

        public string Message { get; set; } = string.Empty;
    }
}
