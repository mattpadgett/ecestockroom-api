using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ecestockroom_api.Models.Authentication
{
    public class Permission
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("description")]
        public string Description { get; set; }

        [BsonElement("key")]
        public string Key { get; set; }
    }
}
