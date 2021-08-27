using ecestockroom_api.Models;
using ecestockroom_api.Models.ProPoints;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ecestockroom_api.Services.ProPoints
{
    public class PPEventService
    {
        private readonly IMongoCollection<PPEvent> _ppEvents;

        public PPEventService(IDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _ppEvents = database.GetCollection<PPEvent>(settings.ProPointsEventsCollectionName);
        }

        public async Task<List<PPEvent>> Get() =>
            await _ppEvents.Find(ppEvents => true).ToListAsync();

        public async Task<PPEvent> Get(string id) =>
            await _ppEvents.Find<PPEvent>(ppEvent => ppEvent.Id == id).FirstOrDefaultAsync();

        public async Task Delete(string id) =>
            await _ppEvents.DeleteOneAsync<PPEvent>(ppEvent => ppEvent.Id == id);

        public async Task<PPEvent> Create(PPEventCreate create)
        {
            var ppEvent = PPEvent.FromCreate(create);
            await _ppEvents.InsertOneAsync(ppEvent);
            return ppEvent;
        }

        public async void Update(PPEvent original, PPEventUpdate update) =>
            await _ppEvents.ReplaceOneAsync(ppEvent => ppEvent.Id == original.Id, PPEvent.FromUpdate(original, update));
    }
}
