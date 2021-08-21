using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ecestockroom_api.Models.Tools
{
    public class ToolGroup
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("tools")]
        public List<Tool> Tools { get; set; }

        [BsonElement("createdUtc")]
        public DateTime CreatedUtc { get; set; }

        [BsonElement("status")]
        public string Status { get; set; }

        public ToolGroup(string id, string name, List<Tool> tools, DateTime createdUtc, string status)
        {
            Id = id;
            Name = name;
            Tools = tools;
            CreatedUtc = createdUtc;
            Status = status;
        }

        public static ToolGroup FromUpdate(ToolGroup original, ToolGroupUpdate update)
        {
            return new ToolGroup(
                original.Id,
                update.Name,
                update.Tools,
                original.CreatedUtc,
                update.Status
            );
        }

        public static ToolGroup FromCreate(ToolGroupCreate create)
        {
            return new ToolGroup(
                null,
                create.Name,
                create.Tools,
                create.CreatedUtc,
                create.Status
            );
        }

    }

    public class ToolGroupCreate
    {
        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("tools")]
        public List<Tool> Tools { get; set; }

        [BsonElement("createdUtc")]
        public DateTime CreatedUtc { get; set; }

        [BsonElement("status")]
        public string Status { get; set; }
    }

    public class ToolGroupUpdate
    {
        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("tools")]
        public List<Tool> Tools { get; set; }

        [BsonElement("status")]
        public string Status { get; set; }
    }

    public class Tool
    {
        [BsonElement("toolId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string ToolId { get; set; }

        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("description")]
        public string Description { get; set; }

        [BsonElement("url")]
        public string Url { get; set; }

        [BsonElement("createdUtc")]
        public DateTime CreatedUtc { get; set; }

        [BsonElement("status")]
        public string Status { get; set; }
    }
}
