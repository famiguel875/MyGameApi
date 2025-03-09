using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MyGameApi.Models
{
    public class Player
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        
        public string Username { get; set; } = "";
        public string PasswordHash { get; set; } = "";
        public int HighestScore { get; set; } = 0;
    }
}
