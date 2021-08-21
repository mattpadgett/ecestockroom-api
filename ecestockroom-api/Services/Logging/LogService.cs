using ecestockroom_api.Models;
using ecestockroom_api.Models.Logging;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ecestockroom_api.Services.Logging
{
    public class LogService
    {
        private readonly IMongoCollection<Log> _logs;

        public LogService(IDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _logs = database.GetCollection<Log>(settings.LogsCollectionName);
        }

        public async Task<List<Log>> Get() =>
            await _logs.Find(log => true).ToListAsync();

        public async Task<Log> Get(string id) =>
            await _logs.Find<Log>(log => log.Id == id).FirstOrDefaultAsync();

        public async Task<Log> Create(Log logIn)
        {
            await _logs.InsertOneAsync(logIn);
            return logIn;
        }
    }
}
