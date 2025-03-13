using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MyGameApi.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyGameApi.Service
{
    public class GamePlayerService
    {
        private readonly IMongoCollection<GamePlayer> _players;

        public GamePlayerService(IOptions<GamePlayerDatabaseSettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            var database = client.GetDatabase(settings.Value.DatabaseName);
            _players = database.GetCollection<GamePlayer>(settings.Value.CollectionName);
        }

        public async Task<List<GamePlayer>> GetAsync() =>
            await _players.Find(player => true).ToListAsync();

        public async Task<GamePlayer?> GetAsync(long id) =>
            await _players.Find(player => player.Id == id).FirstOrDefaultAsync();

        public async Task CreateAsync(GamePlayer player) =>
            await _players.InsertOneAsync(player);

        public async Task UpdateAsync(long id, GamePlayer updatedPlayer) =>
            await _players.ReplaceOneAsync(player => player.Id == id, updatedPlayer);

        public async Task RemoveAsync(long id) =>
            await _players.DeleteOneAsync(player => player.Id == id);
    }
}
