using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ecestockroom_api.Models.ProPoints
{
    public class PPUserEntry
    {
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("eventId")]
        public string EventId { get; set; }

        [BsonElement("checkInUtc")]
        public DateTime CheckInUtc { get; set; }

        [BsonElement("checkOutUtc")]
        public DateTime? CheckOutUtc { get; set; }

        [BsonElement("points")]
        public int? Points { get; set; }

        [BsonElement("status")]
        public string Status { get; set; }

        public PPUserEntry(string eventId, DateTime checkInUtc, DateTime? checkOutUtc, int? points, string status)
        {
            EventId = eventId;
            CheckInUtc = checkInUtc;
            CheckOutUtc = checkOutUtc;
            Points = points;
            Status = status;
        }
    }
}
