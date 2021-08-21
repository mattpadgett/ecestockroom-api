using ecestockroom_api.Helpers.Authentication;
using ecestockroom_api.Models;
using ecestockroom_api.Models.Authentication;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ecestockroom_api.Services.Authentication
{
    public class AuthenticationService
    {
        private readonly IMongoCollection<User> _users;
        private readonly IMongoCollection<Role> _roles;

        public AuthenticationService(IDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _users = database.GetCollection<User>(settings.UsersCollectionName);
            _roles = database.GetCollection<Role>(settings.RolesCollectionName);
        }

        public async Task<bool> CheckAccess(string token, string permissionKey)
        {
            var decodedToken = AuthenticationHelpers.ReadToken(token);
            string userId = decodedToken.Claims.First(c => c.Type == "userId").Value;

            User user = await _users.Find<User>(user => user.Id == userId).FirstOrDefaultAsync();

            if (user == null)
            {
                return false;
            }

            List<Role> userRoles = new List<Role>();

            foreach (string roleId in user.Roles)
            {
                userRoles.Add(await _roles.Find(role => role.Id == roleId).FirstOrDefaultAsync());
            }

            string permissionId = Startup.StaticConfiguration.GetSection("PermissionIds")[permissionKey];

            return AuthenticationHelpers.IsPermissionGranted(user, userRoles, permissionId);
        }
    }
}
