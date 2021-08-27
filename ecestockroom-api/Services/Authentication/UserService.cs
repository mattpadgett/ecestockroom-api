using ecestockroom_api.Models;
using ecestockroom_api.Models.Authentication;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ecestockroom_api.Services.Authentication
{
    public class UserService
    {
        private readonly IMongoCollection<User> _users;

        public UserService(IDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _users = database.GetCollection<User>(settings.UsersCollectionName);
        }

        public async Task<List<User>> Get() =>
            await _users.Find(user => true).ToListAsync();

        public async Task<User> Get(string id) =>
            await _users.Find<User>(user => user.Id == id).FirstOrDefaultAsync();

        public async Task<User> GetByTechId(string techId) =>
            await _users.Find<User>(user => user.TechId == techId).FirstOrDefaultAsync();

        public async Task<User> GetByUsername(string username) =>
            await _users.Find<User>(user => user.Username.ToLower() == username.ToLower()).FirstOrDefaultAsync();

        public async Task<User> Create(UserCreate userIn)
        {
            var user = User.FromCreate(userIn);
            await _users.InsertOneAsync(user);
            return user;
        }

        public async Task<User> Create(User create)
        {
            await _users.InsertOneAsync(create);
            return create;
        }

        public async void Update(User original, UserUpdate userIn) =>
            await _users.ReplaceOneAsync(user => user.Id == original.Id, User.FromUpdate(original, userIn));

        public async void Update(string userId, User update) =>
            await _users.ReplaceOneAsync(user => user.Id == userId, update);
    }
}
