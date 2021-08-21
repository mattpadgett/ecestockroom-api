using ecestockroom_api.Models;
using ecestockroom_api.Models.Authentication;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ecestockroom_api.Services.Authentication
{
    public class TokenService
    {
        private readonly IMongoCollection<Token> _tokens;

        public TokenService(IDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _tokens = database.GetCollection<Token>(settings.TokensCollectionName);
        }

        public async Task<List<Token>> Get() =>
            await _tokens.Find(token => true).ToListAsync();

        public async Task<Token> Get(string id) =>
            await _tokens.Find<Token>(token => token.Id == id).FirstOrDefaultAsync();

        public async Task<Token> GetByToken(string tokenIn) =>
            await _tokens.Find<Token>(token => token.TokenString == tokenIn).FirstOrDefaultAsync();

        public async Task<Token> Create(Token token)
        {
            await _tokens.InsertOneAsync(token);
            return token;
        }

        public async Task InvalidateUserTokens(string userId)
        {
            var filter = Builders<Token>.Filter.Eq("userId", ObjectId.Parse(userId));
            var update = Builders<Token>.Update.Set("validFlag", false);

            await _tokens.UpdateManyAsync(filter, update);
        }

        public async Task InvalidateToken(string id)
        {
            var filter = Builders<Token>.Filter.Eq("_id", ObjectId.Parse(id));
            var update = Builders<Token>.Update.Set("validFlag", false);

            await _tokens.UpdateOneAsync(filter, update);
        }

        public async Task UseToken(string id)
        {
            var filter = Builders<Token>.Filter.Eq("_id", ObjectId.Parse(id));
            var update = Builders<Token>.Update.Set("usedFlag", true);

            await _tokens.UpdateOneAsync(filter, update);
        }

        public async Task AddUsage(string id, TokenAction action)
        {
            var token = await _tokens.Find(token => token.Id == id).FirstOrDefaultAsync();

            token.Usages.Add(action);

            var filter = Builders<Token>.Filter.Eq("_id", ObjectId.Parse(id));
            var update = Builders<Token>.Update.Set("usages", token.Usages);

            await _tokens.UpdateOneAsync(filter, update);
        }
    }
}
