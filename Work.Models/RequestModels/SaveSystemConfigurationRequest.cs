using Microsoft.AspNetCore.Http;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Work.Models.RequestModels
{
    public class SaveSystemConfigurationRequest
    {
        public string? Id { get; set; }

        public string CompanyName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;

        public IFormFile? Logo { get; set; }

        // JSON String
        public string SocialMediaJson { get; set; } = string.Empty;

        // Social Media Icons
        public List<IFormFile> SocialMediaIcons { get; set; } = new();
    }

   
}
