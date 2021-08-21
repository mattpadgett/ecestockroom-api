using ecestockroom_api.Models;
using ecestockroom_api.Models.Authentication;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ecestockroom_api.Services.Authentication
{
    public class PermissionService
    {
        private readonly IMongoCollection<Permission> _permissions;

        public PermissionService(IDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _permissions = database.GetCollection<Permission>(settings.PermissionsCollectionName);
        }

        public async Task<List<Permission>> Get() =>
            await _permissions.Find(permission => true).ToListAsync();

        public async Task<Permission> Get(string id) =>
            await _permissions.Find<Permission>(permission => permission.Id == id).FirstOrDefaultAsync();
    }
}
