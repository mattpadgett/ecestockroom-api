using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace ecestockroom_api.Models.Authentication
{
    public class Role
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("description")]
        public string Description { get; set; }

        [BsonElement("permissions")]
        [BsonRepresentation(BsonType.ObjectId)]
        public List<string> Permissions { get; set; }

        [BsonElement("defaultFlag")]
        public bool DefaultFlag { get; set; }

        public Role(string id, string name, string description, List<string> permissions, bool defaultFlag)
        {
            Id = id;
            Name = name;
            Description = description;
            Permissions = permissions;
            DefaultFlag = defaultFlag;
        }

        public static Role FromUpdate(string id, RoleUpdate update)
        {
            return new Role(
                id,
                update.Name,
                update.Description,
                update.Permissions,
                update.DefaultFlag
            );
        }

        public static Role FromCreate(RoleCreate create)
        {
            return new Role(
                null,
                create.Name,
                create.Description,
                create.Permissions,
                create.DefaultFlag
            );
        }
    }

    public class RoleUpdate
    {
        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("description")]
        public string Description { get; set; }

        [BsonElement("permissions")]
        [BsonRepresentation(BsonType.ObjectId)]
        public List<string> Permissions { get; set; }

        [BsonElement("defaultFlag")]
        public bool DefaultFlag { get; set; }
    }

    public class RoleCreate
    {
        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("description")]
        public string Description { get; set; }

        [BsonElement("permissions")]
        [BsonRepresentation(BsonType.ObjectId)]
        public List<string> Permissions { get; set; }

        [BsonElement("defaultFlag")]
        public bool DefaultFlag { get; set; }
    }
}
