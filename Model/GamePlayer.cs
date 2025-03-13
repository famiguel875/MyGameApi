using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MyGameApi.Model
{
    public class GamePlayer
    {
        [BsonId]
        [BsonRepresentation(BsonType.Int64)]
        public long Id { get; set; }
        
        [BsonElement("Name")]
        [JsonPropertyName("Name")]
        public string Name { get; set; } = string.Empty;
        
        // Vida máxima y vida actual
        public int MaxHp { get; set; }
        public int Hp { get; set; }
        
        // Posición en el juego (opcional)
        public float[]? Position { get; set; }
        
        // Puntaje máximo alcanzado
        public int MaxScore { get; set; }
    }
}
