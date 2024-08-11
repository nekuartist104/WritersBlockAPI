using Dapper;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using WritersBlockAPI.Controllers.Requests;
using WritersBlockAPI.Models;

namespace WritersBlockAPI.Repositories
{
    public class AreaTypeRepository : IAreaTypeRepository
    {
        private string _connectionString;

        public AreaTypeRepository(IConfiguration configuration)
        {
            _connectionString = configuration["Database:ConnectionString"];

        }

        public void Create(CreateAreaTypeRequest createAreaTypeRequest)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string insertAreaTypeQuery = "INSERT INTO [dbo].[AreaType] (Name) VALUES (@Name);";
                int rowsAffected = connection.Execute(insertAreaTypeQuery, new { Name = createAreaTypeRequest.Name });

                if (rowsAffected != 1)
                {
                    throw new Exception($"Error creating area type {JsonConvert.SerializeObject(createAreaTypeRequest)} :: rowsAffected = {rowsAffected}");
                }
            }
        }

        public List<AreaType> All()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string getAreaTypesQuery = "SELECT * FROM [dbo].[AreaType];";
                List<AreaType> areaTypes = connection.Query<AreaType>(getAreaTypesQuery).ToList();

                return areaTypes;
            }
        }

        public AreaType Find(int areaTypeId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string getAreaTypeQuery = "SELECT TOP 1 * FROM [dbo].[AreaType] WHERE AreaTypeId = @AreaTypeId;";
                AreaType areaType = connection.QuerySingleOrDefault<AreaType>(getAreaTypeQuery, new { AreaTypeId = areaTypeId });

                return areaType;
            }
        }

        public void Destroy(int areaTypeId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string deleteAreaTypeQuery = "DELETE FROM [dbo].[AreaType] WHERE AreaTypeId = @AreaTypeId";
                int rowsAffected = connection.Execute(deleteAreaTypeQuery, new { AreaTypeId = areaTypeId });

                if (rowsAffected != 1)
                {
                    throw new Exception($"Error deleting world with id {areaTypeId} :: rowsAffected = {rowsAffected}");
                }
            }
        }
    }
}
