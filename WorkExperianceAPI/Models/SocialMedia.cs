using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace WorkExperianceAPI.Models
{
    public class SocialMedia
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Url { get; set; } = string.Empty;

        public string? Image { get; set; }

        public bool Status { get; set; }
    }
}
