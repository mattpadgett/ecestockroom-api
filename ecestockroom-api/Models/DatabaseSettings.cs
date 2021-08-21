using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ecestockroom_api.Models
{
    public class DatabaseSettings : IDatabaseSettings
    {
        public string LogsCollectionName { get; set; }
        public string UsersCollectionName { get; set; }
        public string PermissionsCollectionName { get; set; }
        public string RolesCollectionName { get; set; }
        public string TokensCollectionName { get; set; }
        public string ToolGroupsCollectionName { get; set; }
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }

    public interface IDatabaseSettings
    {
        string LogsCollectionName { get; set; }
        string UsersCollectionName { get; set; }
        string PermissionsCollectionName { get; set; }
        string RolesCollectionName { get; set; }
        string TokensCollectionName { get; set; }
        string ToolGroupsCollectionName { get; set; }
        string ConnectionString { get; set; }
        string DatabaseName { get; set; }
    }
}