using Dapper;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using WritersBlockAPI.Controllers.Requests;
using WritersBlockAPI.Models;

namespace WritersBlockAPI.Repositories
{
    public class AreaRepository : IAreaRepository
    {
        private string _connectionString;

        public AreaRepository(IConfiguration configuration)
        {
            _connectionString = configuration["Database:ConnectionString"];
        }

        public List<Area> All()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string getAreasQuery = "SELECT * FROM [dbo].[Area];";
                List<Area> areas = connection.Query<Area>(getAreasQuery).ToList();

                return areas;
            }
        }

        public List<Area> All(int locationId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string getAreasQuery = "SELECT * FROM [dbo].[Area] WHERE LocationId = @LocationId;";
                List<Area> areas = connection.Query<Area>(getAreasQuery, new { LocationId = locationId }).ToList();

                return areas;
            }
        }

        public void Create(CreateAreaRequest createAreaRequest)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string insertAreaQuery = "INSERT INTO [dbo].[Area] (LocationId, AreaTypeId, Name, Size) VALUES (@LocationId, @AreaTypeId, @Name, @Size);";
                int rowsAffected = connection.Execute(insertAreaQuery, new {
                    LocationId = createAreaRequest.LocationId,
                    AreaTypeId = createAreaRequest.AreaTypeId,
                    Name = createAreaRequest.Name,
                    Size = createAreaRequest.Size
                });

                if (rowsAffected != 1)
                {
                    throw new Exception($"Error creating Location {JsonConvert.SerializeObject(createAreaRequest)} :: rowsAffected = {rowsAffected}");
                }
            }
        }

        public Area Find(int areaId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string getAreaQuery = "SELECT TOP 1 * FROM [dbo].[Area] WHERE AreaId = @AreaId";
                Area area = connection.QuerySingleOrDefault<Area>(getAreaQuery, new { AreaId = areaId });

                return area;
            }
        }

        public void Destroy(int areaId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string deleteAreaQuery = "DELETE FROM [dbo].[Area] WHERE AreaId = @AreaId";
                int rowsAffected = connection.Execute(deleteAreaQuery, new { AreaId = areaId });

                if (rowsAffected != 1)
                {
                    throw new Exception($"Error deleting Area with id {areaId} :: rowsAffected = {rowsAffected}");
                }
            }
        }
    }
}
