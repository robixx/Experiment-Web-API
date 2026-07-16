using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace WorkExperianceAPI.Models
{
    public class CompanyInfo
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        public string CompanyName { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;

        public string? Image { get; set; }

        public List<SocialMedia> SocialMedias { get; set; } = new();
    }
}
