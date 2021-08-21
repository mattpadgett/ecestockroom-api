using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace ecestockroom_api.Models.Authentication
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("lastName")]
        public string LastName { get; set; }

        [BsonElement("firstName")]
        public string FirstName { get; set; }

        [BsonElement("username")]
        public string Username { get; set; }

        [BsonElement("password")]
        public string Password { get; set; }

        [BsonElement("permissions")]
        [BsonRepresentation(BsonType.ObjectId)]
        public List<string> Permissions { get; set; }

        [BsonElement("roles")]
        [BsonRepresentation(BsonType.ObjectId)]
        public List<string> Roles { get; set; }

        public User(string id, string lastName, string firstName, string username, string password, List<string> permissions, List<string> roles)
        {
            Id = id;
            LastName = lastName;
            FirstName = firstName;
            Username = username;
            Password = password;
            Permissions = permissions;
            Roles = roles;
        }

        public static User FromUpdate(User original, UserUpdate update)
        {
            return new User(
                original.Id,
                update.LastName,
                update.FirstName,
                original.Username,
                update.Password,
                update.Permissions,
                update.Roles
            );
        }

        public static User FromCreate(UserCreate create)
        {
            return new User(
                null,
                create.LastName,
                create.FirstName,
                create.Username,
                create.Password,
                create.Permissions,
                create.Roles
            );
        }
    }

    public class UserUpdate
    {
        [BsonElement("lastName")]
        public string LastName { get; set; }

        [BsonElement("firstName")]
        public string FirstName { get; set; }

        [BsonElement("password")]
        public string Password { get; set; }

        [BsonElement("permissions")]
        [BsonRepresentation(BsonType.ObjectId)]
        public List<string> Permissions { get; set; }

        [BsonElement("roles")]
        [BsonRepresentation(BsonType.ObjectId)]
        public List<string> Roles { get; set; }
    }

    public class UserCreate
    {
        [BsonElement("lastName")]
        public string LastName { get; set; }

        [BsonElement("firstName")]
        public string FirstName { get; set; }

        [BsonElement("username")]
        public string Username { get; set; }

        [BsonElement("password")]
        public string Password { get; set; }

        [BsonElement("permissions")]
        [BsonRepresentation(BsonType.ObjectId)]
        public List<string> Permissions { get; set; }

        [BsonElement("roles")]
        [BsonRepresentation(BsonType.ObjectId)]
        public List<string> Roles { get; set; }
    }
}
