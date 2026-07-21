using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Work.Models.EntityModels
{
    public class SystemConfigure
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; }

        [BsonElement("companyName")]
        public string CompanyName { get; set; } = string.Empty;

        [BsonElement("email")]
        public string Email { get; set; } = string.Empty;

        [BsonElement("phone")]
        public string Phone { get; set; } = string.Empty;

        [BsonElement("logo")]
        public string Logo { get; set; } = string.Empty;

        [BsonElement("imagePath")]
        public string ImagePath { get; set; } = string.Empty;

        [BsonElement("socialMedias")]
        public List<SocialMedia> SocialMedias { get; set; } = new();
    }
}
