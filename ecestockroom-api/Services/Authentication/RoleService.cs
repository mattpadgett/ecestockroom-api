using ecestockroom_api.Models;
using ecestockroom_api.Models.Authentication;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ecestockroom_api.Services.Authentication
{
    public class RoleService
    {
        private readonly IMongoCollection<Role> _roles;

        public RoleService(IDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _roles = database.GetCollection<Role>(settings.RolesCollectionName);
        }

        public async Task<List<Role>> Get() =>
            await _roles.Find(role => true).ToListAsync();

        public async Task<Role> Get(string id) =>
            await _roles.Find<Role>(role => role.Id == id).FirstOrDefaultAsync();

        public async Task<Role> Create(RoleCreate roleIn)
        {
            var role = Role.FromCreate(roleIn);
            await _roles.InsertOneAsync(role);
            return role;
        }

        public async void Update(string id, RoleUpdate roleIn) =>
            await _roles.ReplaceOneAsync(role => role.Id == id, Role.FromUpdate(id, roleIn));
    }
}
