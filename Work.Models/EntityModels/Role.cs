using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Work.Models.EntityModels
{
    public class Role : BaseEntity
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public string Name { get; set; } = default!;

        public string Description { get; set; } = default!;

        public bool IsActive { get; set; } = true;

        public List<string> Permissions { get; set; } = new();
    }
}
