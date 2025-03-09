using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MyGameApi.Models;
using MyGameApi.MongoDb;

namespace MyGameApi.Services
{
    public class MongoDbService
    {
        private readonly IMongoCollection<Player> _players;

        public MongoDbService(IOptions<MongoDbSettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            var database = client.GetDatabase(settings.Value.DatabaseName);
            _players = database.GetCollection<Player>("Players");

            // Crear índice único para Username
            var indexKeys = Builders<Player>.IndexKeys.Ascending(p => p.Username);
            var indexOptions = new CreateIndexOptions { Unique = true };
            var indexModel = new CreateIndexModel<Player>(indexKeys, indexOptions);
            _players.Indexes.CreateOne(indexModel);
        }

        public async Task<Player?> GetPlayer(string username) =>
            await _players.Find(p => p.Username == username).FirstOrDefaultAsync();

        public async Task<bool> CreatePlayer(Player player)
        {
            try
            {
                await _players.InsertOneAsync(player);
                return true;
            }
            catch (MongoWriteException ex) when (ex.WriteError.Category == ServerErrorCategory.DuplicateKey)
            {
                return false; // El nombre de usuario ya existe
            }
        }

        public async Task<bool> UpdateScore(string username, int newScore)
        {
            var filter = Builders<Player>.Filter.Eq(p => p.Username, username);
            var existingPlayer = await _players.Find(filter).FirstOrDefaultAsync();

            if (existingPlayer == null || newScore <= existingPlayer.HighestScore)
                return false;

            var update = Builders<Player>.Update.Set(p => p.HighestScore, newScore);
            await _players.UpdateOneAsync(filter, update);
            return true;
        }

        public async Task<List<Player>> GetLeaderboard() =>
            await _players.Find(_ => true)
                          .SortByDescending(p => p.HighestScore)
                          .ToListAsync();
    }
}
