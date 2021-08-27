using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ecestockroom_api.Models.ProPoints
{
    public class PPEvent
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("type")]
        public string Type { get; set; }

        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("hourlyPointRate")]
        public int HourlyPointRate { get; set; }

        [BsonElement("eventUtc")]
        public DateTime EventUtc { get; set; }

        [BsonElement("status")]
        public string Status { get; set; }

        public PPEvent(string id, string type, string name, int hourlyPointRate, DateTime eventUtc, string status)
        {
            Id = id;
            Type = type;
            Name = name;
            HourlyPointRate = hourlyPointRate;
            EventUtc = eventUtc;
            Status = status;
        }

        public static PPEvent FromCreate(PPEventCreate create)
        {
            return new PPEvent(
                null,
                create.Type,
                create.Name,
                create.HourlyPointRate,
                create.EventUtc,
                create.Status
            );
        }

        public static PPEvent FromUpdate(PPEvent original, PPEventUpdate update)
        {
            return new PPEvent(
                original.Id,
                update.Type,
                update.Name,
                update.HourlyPointRate,
                update.EventUtc,
                update.Status
            );
        }
    }

    public class PPEventCreate
    {
        [BsonElement("type")]
        public string Type { get; set; }

        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("hourlyPointRate")]
        public int HourlyPointRate { get; set; }

        [BsonElement("eventUtc")]
        public DateTime EventUtc { get; set; }

        [BsonElement("status")]
        public string Status { get; set; }
    }

    public class PPEventUpdate
    {
        [BsonElement("type")]
        public string Type { get; set; }

        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("hourlyPointRate")]
        public int HourlyPointRate { get; set; }

        [BsonElement("eventUtc")]
        public DateTime EventUtc { get; set; }

        [BsonElement("status")]
        public string Status { get; set; }
    }
}
