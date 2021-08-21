using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ecestockroom_api.Models.Logging
{
    public class Log
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("userId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string UserId { get; set; }

        [BsonElement("timestampUtc")]
        public DateTime TimestampUtc { get; set; }

        [BsonElement("message")]
        public string message { get; set; }

        [BsonElement("collection")]
        public string Collection { get; set; }

        [BsonElement("documentId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string DocumentId { get; set; }

        [BsonElement("document")]
        public string Document { get; set; }

        public Log(string id, string userId, DateTime timestampUtc, string message, string collection, string documentId, string document)
        {
            Id = id;
            UserId = userId;
            TimestampUtc = timestampUtc;
            this.message = message;
            Collection = collection;
            DocumentId = documentId;
            Document = document;
        }
    }
}
