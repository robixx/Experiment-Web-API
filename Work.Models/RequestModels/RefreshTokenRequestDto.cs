using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Work.Models.RequestModels
{
    public class RefreshTokenRequestDto
    {
        // মেয়াদোত্তীর্ণ এক্সেস টোকেন যা থেকে ইউজার আইডি বের করা হবে
        public string ExpiredAccessToken { get; set; } = string.Empty;
    }
}
