using ecestockroom_api.Models;
using ecestockroom_api.Models.Tools;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ecestockroom_api.Services.Tools
{
    public class ToolService
    {
        private readonly IMongoCollection<ToolGroup> _toolGroups;

        public ToolService(IDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _toolGroups = database.GetCollection<ToolGroup>(settings.ToolGroupsCollectionName);
        }

        public async Task<List<ToolGroup>> Get() =>
            await _toolGroups.Find(toolGroups => true).ToListAsync();
        
        public async Task<ToolGroup> Get(string id) =>
            await _toolGroups.Find<ToolGroup>(toolGroup => toolGroup.Id == id).FirstOrDefaultAsync();

        public async Task Delete(string id) =>
            await _toolGroups.DeleteOneAsync<ToolGroup>(toolGroup => toolGroup.Id == id);

        public async Task<ToolGroup> Create(ToolGroupCreate toolIn)
        {
            var toolGroup = ToolGroup.FromCreate(toolIn);
            await _toolGroups.InsertOneAsync(toolGroup);
            return toolGroup;
        }

        public async void Update(ToolGroup group, ToolGroupUpdate groupIn) =>
            await _toolGroups.ReplaceOneAsync(toolGroup => toolGroup.Id == group.Id, ToolGroup.FromUpdate(group, groupIn));
    }
}
