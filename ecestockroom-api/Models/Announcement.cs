using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ecestockroom_api.Models
{
    public class Announcement
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("title")]
        public string Title { get; set; }

        [BsonElement("content")]
        public string Content { get; set; }

        [BsonElement("createdUtc")]
        public DateTime CreatedUtc { get; set; }

        [BsonElement("status")]
        public string Status { get; set; }

        public Announcement(string id, string title, string content, DateTime createdUtc, string status)
        {
            Id = id;
            Title = title;
            Content = content;
            CreatedUtc = createdUtc;
            Status = status;
        }

        public static Announcement FromUpdate(Announcement original, AnnouncementUpdate update)
        {
            return new Announcement(
                original.Id,
                update.Title,
                update.Content,
                original.CreatedUtc,
                update.Status
            );
        }

        public static Announcement FromCreate(AnnouncementCreate create)
        {
            return new Announcement(
                null,
                create.Title,
                create.Content,
                create.CreatedUtc,
                create.Status
            );
        }
    }

    public class AnnouncementUpdate
    {
        [BsonElement("title")]
        public string Title { get; set; }

        [BsonElement("content")]
        public string Content { get; set; }

        [BsonElement("status")]
        public string Status { get; set; }
    }

    public class AnnouncementCreate
    {
        [BsonElement("title")]
        public string Title { get; set; }

        [BsonElement("content")]
        public string Content { get; set; }

        [BsonElement("createdUtc")]
        public DateTime CreatedUtc { get; set; }

        [BsonElement("status")]
        public string Status { get; set; }
    }
}
