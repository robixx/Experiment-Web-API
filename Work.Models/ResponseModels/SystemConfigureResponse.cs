using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Work.Models.ResponseModels
{
    public class SystemConfigureResponse
    {
        public ObjectId Id { get; set; } 

        public string CompanyName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;

        public string Logo { get; set; } = string.Empty;

        public string ImagePath { get; set; } = string.Empty;

        public List<SocialMediaResponse> SocialMedias { get; set; } = new();
    }
}
