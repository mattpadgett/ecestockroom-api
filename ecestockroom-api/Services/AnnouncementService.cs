using ecestockroom_api.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ecestockroom_api.Services
{
    public class AnnouncementService
    {
        private readonly IMongoCollection<Announcement> _announcements;

        public AnnouncementService(IDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _announcements = database.GetCollection<Announcement>(settings.AnnouncementsCollectionName);
        }

        public async Task<List<Announcement>> Get() =>
            await _announcements.Find(announcement => true).ToListAsync();

        public async Task<Announcement> Get(string id) =>
            await _announcements.Find<Announcement>(announcement => announcement.Id == id).FirstOrDefaultAsync();

        public async Task Delete(string id) =>
            await _announcements.DeleteOneAsync<Announcement>(announcement => announcement.Id == id);

        public async Task<Announcement> Create(AnnouncementCreate announcementIn)
        {
            var announcement = Announcement.FromCreate(announcementIn);
            await _announcements.InsertOneAsync(announcement);
            return announcement;
        }

        public async void Update(Announcement original, AnnouncementUpdate update) =>
            await _announcements.ReplaceOneAsync(announcement => announcement.Id == original.Id, Announcement.FromUpdate(original, update));
    }
}
