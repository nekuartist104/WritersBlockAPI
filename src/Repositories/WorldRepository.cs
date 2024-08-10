using WritersBlockAPI.Models;
using Microsoft.Data.SqlClient;
using Dapper;
using WritersBlockAPI.Controllers.Requests;
using Newtonsoft.Json;

namespace WritersBlockAPI.Repositories
{
    public class WorldRepository : IWorldRepository
    {
        private string _connectionString;

        public WorldRepository(IConfiguration configuration) 
        {
            _connectionString = configuration["Database:ConnectionString"];
        }

        public List<World> All()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string getWorldsQuery = "SELECT * FROM [dbo].[World];";
                List<World> worlds = connection.Query<World>(getWorldsQuery).ToList();

                return worlds;
            }
        }

        public void Create(CreateWorldRequest createWorldRequest)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string insertWorldQuery = "INSERT INTO [dbo].[World] (Name) VALUES (@Name);";
                int rowsAffected = connection.Execute(insertWorldQuery, new { Name = createWorldRequest.Name });

                if (rowsAffected != 1)
                {
                    throw new Exception($"Error creating world {JsonConvert.SerializeObject(createWorldRequest)} :: rowsAffected = {rowsAffected}");
                }
            }
        }

        public World Find(int worldId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string getWorldQuery = "SELECT TOP 1 * FROM [dbo].[World] WHERE WorldId = @WorldId;";
                World world = connection.QuerySingleOrDefault<World>(getWorldQuery, new { WorldId = worldId });

                return world;
            }
        }

        public void Destroy(int worldId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string deleteWorldQuery = "DELETE FROM [dbo].[World] WHERE WorldId = @WorldId;";
                int rowsAffected = connection.Execute(deleteWorldQuery, new { WorldId = worldId });

                if (rowsAffected != 1)
                {
                    throw new Exception($"Error deleting world with id {worldId} :: rowsAffected = {rowsAffected}");
                }
            }
        }
    }
}
