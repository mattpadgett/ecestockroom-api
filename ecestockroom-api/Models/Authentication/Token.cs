using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace ecestockroom_api.Models.Authentication
{
    public class Token
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("userId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string UserId { get; set; }

        [BsonElement("type")]
        public string Type { get; set; }

        [BsonElement("token")]
        public string TokenString { get; set; }

        [BsonElement("createdUtc")]
        public DateTime CreatedUtc { get; set; }

        [BsonElement("usages")]
        public List<TokenAction> Usages { get; set; }

        [BsonElement("usedFlag")]
        public bool UsedFlag { get; set; }

        [BsonElement("validFlag")]
        public bool ValidFlag { get; set; }

        public Token(string id, string userId, string type, string tokenString, DateTime createdUtc, List<TokenAction> usages, bool usedFlag, bool validFlag)
        {
            Id = id;
            UserId = userId;
            Type = type;
            TokenString = tokenString;
            CreatedUtc = createdUtc;
            Usages = usages;
            UsedFlag = usedFlag;
            ValidFlag = validFlag;
        }
    }
    
    public class TokenAction
    {
        [BsonElement("utc")]
        public DateTime Utc { get; set; }

        [BsonElement("action")]
        public string Action { get; set; }

        public TokenAction(DateTime utc, string action)
        {
            Utc = utc;
            Action = action;
        }
    }
}
