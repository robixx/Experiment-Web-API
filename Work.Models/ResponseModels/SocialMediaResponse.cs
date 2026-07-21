using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Work.Models.ResponseModels
{
    public class SocialMediaResponse
    {
        public ObjectId Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Url { get; set; } = string.Empty;

        public string Icon { get; set; } = string.Empty;

        public string IconUrl { get; set; } = string.Empty;

        public bool Status { get; set; }
    }
}
