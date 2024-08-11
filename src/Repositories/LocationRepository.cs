using Dapper;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using WritersBlockAPI.Controllers.Requests;
using WritersBlockAPI.Models;

namespace WritersBlockAPI.Repositories
{
    public class LocationRepository : ILocationRepository
    {
        private string _connectionString;
        public LocationRepository(IConfiguration configuration)
        {
            _connectionString = configuration["Database:ConnectionString"];
        }

        public List<Location> All()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string getLocationsQuery = "SELECT * FROM [dbo].[Location];";
                List<Location> Locations = connection.Query<Location>(getLocationsQuery).ToList();

                return Locations;
            }
        }

        public List<Location> All(int worldId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string getLocationsQuery = "SELECT * FROM [dbo].[Location] WHERE WorldId = @WorldId;";
                List<Location> Locations = connection.Query<Location>(getLocationsQuery, new { WorldId = worldId }).ToList();

                return Locations;
            }
        }

        public void Create(CreateLocationRequest createLocationRequest)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string insertLocationQuery = "INSERT INTO [dbo].[Location] (WorldId, Name, Nationality, Population, Climate, Terrain) VALUES (@WorldId, @Name, @Nationality, @Population, @Climate, @Terrain);";
                int rowsAffected = connection.Execute(insertLocationQuery, new {
                    WorldId = createLocationRequest.WorldId,
                    Name = createLocationRequest.Name,
                    Nationality = createLocationRequest.Nationality,
                    Population = createLocationRequest.Population,
                    Climate = createLocationRequest.Climate,
                    Terrain = createLocationRequest.Terrain
                });

                if (rowsAffected != 1)
                {
                    throw new Exception($"Error creating Location {JsonConvert.SerializeObject(createLocationRequest)} :: rowsAffected = {rowsAffected}");
                }
            }
        }

        public Location Find(int locationId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string getLocationQuery = "SELECT TOP 1 * FROM [dbo].[Location] WHERE LocationId = @LocationId;";
                Location Location = connection.QuerySingleOrDefault<Location>(getLocationQuery, new { LocationId = locationId });

                return Location;
            }
        }

        public void Destroy(int locationId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string deleteLocationQuery = "DELETE FROM [dbo].[Location] WHERE LocationId = @LocationId;";
                int rowsAffected = connection.Execute(deleteLocationQuery, new { LocationId = locationId });

                if (rowsAffected != 1)
                {
                    throw new Exception($"Error deleting Location with id {locationId} :: rowsAffected = {rowsAffected}");
                }
            }
        }
    }
}
