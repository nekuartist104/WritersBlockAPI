using Newtonsoft.Json;
using NuGet.Frameworks;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;
using WritersBlockAPI.Controllers.Requests;
using WritersBlockAPI.Models;

namespace WritersBlockAPI.Tests
{
    public class APITests
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseAddress;

        public APITests()
        {
            _httpClient = new HttpClient();
            _baseAddress = "http://localhost:5137";
        }

        [Fact]
        public async Task World_Success()
        {
            // 1) Create a unique world
            var worldName = $"TestWorld-{Guid.NewGuid()}";

            var createWorldRequest = new CreateWorldRequest()
            {
                Name = worldName
            };
            // Serialize our concrete class into a JSON String
            var jsonBody = JsonConvert.SerializeObject(createWorldRequest);

            // Wrap our JSON inside a StringContent which then can be used by the HttpClient class
            var httpContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");
            var createWorldHttpResponse = await _httpClient.PostAsync($"{_baseAddress}/World", httpContent);

            Assert.True(createWorldHttpResponse.IsSuccessStatusCode);

            // 2) Get all worlds and check our world was created
            var getWorldsHttpResponse = await _httpClient.GetAsync($"{_baseAddress}/World");
            Assert.True(getWorldsHttpResponse.IsSuccessStatusCode);
            var getWorldsHttpResponseBody = await getWorldsHttpResponse.Content.ReadAsStringAsync();
            List<World> worlds = JsonConvert.DeserializeObject<List<World>>(getWorldsHttpResponseBody);

            var myWorld = worlds.FirstOrDefault(world => world.Name == worldName);
            Assert.NotNull(myWorld);
            Assert.True(myWorld.WorldId > 0);

            // 3) Get world by id and check the name
            var getWorldHttpResponse = await _httpClient.GetAsync($"{_baseAddress}/World/{myWorld.WorldId}");
            Assert.True(getWorldHttpResponse.IsSuccessStatusCode);
            var getWorldHttpResponseBody = await getWorldHttpResponse.Content.ReadAsStringAsync();
            World world = JsonConvert.DeserializeObject<World>(getWorldHttpResponseBody);

            Assert.NotNull(world);
            Assert.Equal(worldName, world.Name);

            // 4) Delete world by id 
            var deleteWorldHttpResponse = await _httpClient.DeleteAsync($"{_baseAddress}/World/{myWorld.WorldId}");
            Assert.True(deleteWorldHttpResponse.IsSuccessStatusCode);
        }

        [Fact]
        public async Task World_NotFound()
        {
            var invalidWorldId = -1;
            var getWorldHttpResponse = await _httpClient.GetAsync($"{_baseAddress}/World/{invalidWorldId}");
            Assert.Equal(HttpStatusCode.NotFound, getWorldHttpResponse.StatusCode);
        }

        [Fact]
        public async Task World_NameTooLong()
        {
            var invalidWorldName = new string('a', 256); ;

            var createWorldRequest = new CreateWorldRequest()
            {
                Name = invalidWorldName
            };
            // Serialize our concrete class into a JSON String
            var jsonBody = JsonConvert.SerializeObject(createWorldRequest);

            // Wrap our JSON inside a StringContent which then can be used by the HttpClient class
            var httpContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");
            var createWorldHttpResponse = await _httpClient.PostAsync($"{_baseAddress}/World", httpContent);

            Assert.Equal(HttpStatusCode.UnprocessableEntity, createWorldHttpResponse.StatusCode);
        }

        [Fact]
        public async Task World_Delete()
        {
            // 1) Create a unique world
            var worldName = $"TestWorld-{Guid.NewGuid()}";

            var createWorldRequest = new CreateWorldRequest()
            {
                Name = worldName
            };
            // Serialize our concrete class into a JSON String
            var jsonBody = JsonConvert.SerializeObject(createWorldRequest);

            // Wrap our JSON inside a StringContent which then can be used by the HttpClient class
            var httpContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");
            var createWorldHttpResponse = await _httpClient.PostAsync($"{_baseAddress}/World", httpContent);

            Assert.True(createWorldHttpResponse.IsSuccessStatusCode);

            // 2) Get all worlds and check our world was created
            var getWorldsHttpResponse = await _httpClient.GetAsync($"{_baseAddress}/World");
            Assert.True(getWorldsHttpResponse.IsSuccessStatusCode);
            var getWorldsHttpResponseBody = await getWorldsHttpResponse.Content.ReadAsStringAsync();
            List<World> worlds = JsonConvert.DeserializeObject<List<World>>(getWorldsHttpResponseBody);

            var myWorld = worlds.FirstOrDefault(world => world.Name == worldName);
            Assert.NotNull(myWorld);
            Assert.True(myWorld.WorldId > 0);

            // 3) Delete world by id 
            var deleteWorldHttpResponse = await _httpClient.DeleteAsync($"{_baseAddress}/World/{myWorld.WorldId}");
            Assert.True(deleteWorldHttpResponse.IsSuccessStatusCode);
  
            // 4) Check world is no longer there
            var getWorldHttpResponse = await _httpClient.GetAsync($"{_baseAddress}/World/{myWorld.WorldId}");
            Assert.Equal(HttpStatusCode.NotFound, getWorldHttpResponse.StatusCode);
        }







        [Fact]
        public async Task Location_NotFound()
        {
            int invalidLocationId = -1;
            var getLocationHttpResponse = await _httpClient.GetAsync($"{_baseAddress}/Location/{invalidLocationId}");
            Assert.Equal(HttpStatusCode.NotFound, getLocationHttpResponse.StatusCode);
        }

        [Fact]
        public async Task Location_NameTooLong()
        {
            var worldName = $"Test World - {Guid.NewGuid()}";
            // Create a new world
            var createWorldRequest = new CreateWorldRequest()
            {
                Name = worldName
            };
            var jsonBody = JsonConvert.SerializeObject(createWorldRequest);
            var httpContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");
            var createWorldHttpResponse = await _httpClient.PostAsync($"{_baseAddress}/World", httpContent);

            // Check new world now exists
            var getWorldsHttpResponse = await _httpClient.GetAsync($"{_baseAddress}/World");
            Assert.True(getWorldsHttpResponse.IsSuccessStatusCode);
            var getWorldsHttpResponseBody = await getWorldsHttpResponse.Content.ReadAsStringAsync();
            List<World> worlds = JsonConvert.DeserializeObject<List<World>>(getWorldsHttpResponseBody);

            var myWorld = worlds.FirstOrDefault(world => world.Name == worldName);
            Assert.NotNull(myWorld);
            Assert.True(myWorld.WorldId > 0);

            // Create location with invalid name
            var invalidLocationName = new string('a', 256);
            var myWorldId = myWorld.WorldId;
            var createLocationRequest = new CreateLocationRequest()
            {
                WorldId = myWorldId,
                Name = invalidLocationName,
                Nationality = "nationality",
                Population = 1,
                Climate = "climate",
                Terrain = "terrain"
            };

            var locationJsonBody = JsonConvert.SerializeObject(createLocationRequest);
            var locationHttpContent = new StringContent(locationJsonBody, Encoding.UTF8, "application/json");
            var createLocationHttpResponse = await _httpClient.PostAsync($"{_baseAddress}/Location", locationHttpContent);
            // Check that name of location created is too long
            Assert.Equal(HttpStatusCode.UnprocessableEntity, createLocationHttpResponse.StatusCode);

            // Delete world by id
            var deleteWorldHttpResponse = await _httpClient.DeleteAsync($"{_baseAddress}/World/{myWorld.WorldId}");
            Assert.True(deleteWorldHttpResponse.IsSuccessStatusCode);
        }

        [Fact]
        public async Task Location_NationalityTooLong()
        {
            var worldName = $"Test World - {Guid.NewGuid()}";
            // Create a new world
            var createWorldRequest = new CreateWorldRequest()
            {
                Name = worldName
            };
            var jsonBody = JsonConvert.SerializeObject(createWorldRequest);
            var httpContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");
            var createWorldHttpResponse = await _httpClient.PostAsync($"{_baseAddress}/World", httpContent);

            // Check new world now exists
            var getWorldsHttpResponse = await _httpClient.GetAsync($"{_baseAddress}/World");
            Assert.True(getWorldsHttpResponse.IsSuccessStatusCode);
            var getWorldsHttpResponseBody = await getWorldsHttpResponse.Content.ReadAsStringAsync();
            List<World> worlds = JsonConvert.DeserializeObject<List<World>>(getWorldsHttpResponseBody);

            var myWorld = worlds.FirstOrDefault(world => world.Name == worldName);
            Assert.NotNull(myWorld);
            Assert.True(myWorld.WorldId > 0);

            // Create location with invalid name
            var validLocationName = $"Test Location - {Guid.NewGuid()}";
            var invalidLocationNationality = new string('a', 256);
            var myWorldId = myWorld.WorldId;
            var createLocationRequest = new CreateLocationRequest()
            {
                WorldId = myWorldId,
                Name = validLocationName,
                Nationality = invalidLocationNationality,
                Population = 1,
                Climate = "climate",
                Terrain = "terrain"
            };

            var locationJsonBody = JsonConvert.SerializeObject(createLocationRequest);
            var locationHttpContent = new StringContent(locationJsonBody, Encoding.UTF8, "application/json");
            var createLocationHttpResponse = await _httpClient.PostAsync($"{_baseAddress}/Location", locationHttpContent);
            // Check that name of location created is too long
            Assert.Equal(HttpStatusCode.UnprocessableEntity, createLocationHttpResponse.StatusCode);

            // Delete world by id
            var deleteWorldHttpResponse = await _httpClient.DeleteAsync($"{_baseAddress}/World/{myWorld.WorldId}");
            Assert.True(deleteWorldHttpResponse.IsSuccessStatusCode);
        }

        [Fact]
        public async Task Location_ClimateTooLong()
        {
            var worldName = $"Test World - {Guid.NewGuid()}";
            // Create a new world
            var createWorldRequest = new CreateWorldRequest()
            {
                Name = worldName
            };
            var jsonBody = JsonConvert.SerializeObject(createWorldRequest);
            var httpContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");
            var createWorldHttpResponse = await _httpClient.PostAsync($"{_baseAddress}/World", httpContent);

            // Check new world now exists
            var getWorldsHttpResponse = await _httpClient.GetAsync($"{_baseAddress}/World");
            Assert.True(getWorldsHttpResponse.IsSuccessStatusCode);
            var getWorldsHttpResponseBody = await getWorldsHttpResponse.Content.ReadAsStringAsync();
            List<World> worlds = JsonConvert.DeserializeObject<List<World>>(getWorldsHttpResponseBody);

            var myWorld = worlds.FirstOrDefault(world => world.Name == worldName);
            Assert.NotNull(myWorld);
            Assert.True(myWorld.WorldId > 0);

            // Create location with invalid name
            var validLocationName = $"Test Location - {Guid.NewGuid()}";
            var invalidLocationClimate = new string('a', 256);
            var myWorldId = myWorld.WorldId;
            var createLocationRequest = new CreateLocationRequest()
            {
                WorldId = myWorldId,
                Name = validLocationName,
                Nationality = "nationality",
                Population = 1,
                Climate = invalidLocationClimate,
                Terrain = "terrain"
            };

            var locationJsonBody = JsonConvert.SerializeObject(createLocationRequest);
            var locationHttpContent = new StringContent(locationJsonBody, Encoding.UTF8, "application/json");
            var createLocationHttpResponse = await _httpClient.PostAsync($"{_baseAddress}/Location", locationHttpContent);
            // Check that name of location created is too long
            Assert.Equal(HttpStatusCode.UnprocessableEntity, createLocationHttpResponse.StatusCode);

            // Delete world by id
            var deleteWorldHttpResponse = await _httpClient.DeleteAsync($"{_baseAddress}/World/{myWorld.WorldId}");
            Assert.True(deleteWorldHttpResponse.IsSuccessStatusCode);
        }

        [Fact]
        public async Task Location_TerrainTooLong()
        {
            var worldName = $"Test World - {Guid.NewGuid()}";
            // Create a new world
            var createWorldRequest = new CreateWorldRequest()
            {
                Name = worldName
            };
            var jsonBody = JsonConvert.SerializeObject(createWorldRequest);
            var httpContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");
            var createWorldHttpResponse = await _httpClient.PostAsync($"{_baseAddress}/World", httpContent);

            // Check new world now exists
            var getWorldsHttpResponse = await _httpClient.GetAsync($"{_baseAddress}/World");
            Assert.True(getWorldsHttpResponse.IsSuccessStatusCode);
            var getWorldsHttpResponseBody = await getWorldsHttpResponse.Content.ReadAsStringAsync();
            List<World> worlds = JsonConvert.DeserializeObject<List<World>>(getWorldsHttpResponseBody);

            var myWorld = worlds.FirstOrDefault(world => world.Name == worldName);
            Assert.NotNull(myWorld);
            Assert.True(myWorld.WorldId > 0);

            // Create location with invalid name
            var validLocationName = $"Test Location - {Guid.NewGuid()}";
            var invalidLocationTerrain = new string('a', 256);
            var myWorldId = myWorld.WorldId;
            var createLocationRequest = new CreateLocationRequest()
            {
                WorldId = myWorldId,
                Name = validLocationName,
                Nationality = "nationality",
                Population = 1,
                Climate = "climate",
                Terrain = invalidLocationTerrain
            };

            var locationJsonBody = JsonConvert.SerializeObject(createLocationRequest);
            var locationHttpContent = new StringContent(locationJsonBody, Encoding.UTF8, "application/json");
            var createLocationHttpResponse = await _httpClient.PostAsync($"{_baseAddress}/Location", locationHttpContent);
            // Check that name of location created is too long
            Assert.Equal(HttpStatusCode.UnprocessableEntity, createLocationHttpResponse.StatusCode);

            // Delete world by id
            var deleteWorldHttpResponse = await _httpClient.DeleteAsync($"{_baseAddress}/World/{myWorld.WorldId}");
            Assert.True(deleteWorldHttpResponse.IsSuccessStatusCode);
        }

        [Fact]
        public async Task Location_Success()
        {
            //1) Create a valid world
            var worldName = $"Test World - {Guid.NewGuid()}";
            var createWorldRequest = new CreateWorldRequest()
            {
                Name = worldName
            };
            // Serialize word to Json
            var jsonBody = JsonConvert.SerializeObject(createWorldRequest);
            // HttpContent with json
            var httpContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");
            var createWorldHttpResponse = await _httpClient.PostAsync($"{_baseAddress}/World", httpContent);
            // Check world successfully created
            Assert.True(createWorldHttpResponse.IsSuccessStatusCode);

            // Get all worlds
            var getWorldsHttpResponse = await _httpClient.GetAsync($"{_baseAddress}/World");
            Assert.True(getWorldsHttpResponse.IsSuccessStatusCode);
            var getWorldsHttpResponseBody = await getWorldsHttpResponse.Content.ReadAsStringAsync();
            List<World> worlds = JsonConvert.DeserializeObject<List<World>>(getWorldsHttpResponseBody);

            // Get valid world, check it exists and is valid
            World myWorld = worlds.FirstOrDefault(world => world.Name == worldName);
            Assert.NotNull(myWorld);
            Assert.True(myWorld.WorldId > 0);

            // 2) Create valid location
            var locationName = $"Test Location - {Guid.NewGuid()}";
            var createLocationRequest = new CreateLocationRequest()
            {
                WorldId = myWorld.WorldId,
                Name = locationName,
                Nationality = "nationality",
                Population = 1000,
                Climate = "climate",
                Terrain = "terrain"
            };

            // Serialize concrete class into a json for the http client to use
            var locationJsonBody = JsonConvert.SerializeObject(createLocationRequest);
            // Http content with json
            var locationHttpContent = new StringContent(locationJsonBody, Encoding.UTF8, "application/json");
            var createLocationHttpResponse = await _httpClient.PostAsync($"{_baseAddress}/Location", locationHttpContent);
            Assert.True(createLocationHttpResponse.IsSuccessStatusCode);

            // 3) Get all locations
            var getLocationsHttpResponse = await _httpClient.GetAsync($"{_baseAddress}/Location");
            Assert.True(getLocationsHttpResponse.IsSuccessStatusCode);
            // Read string content from response
            var getLocationsHttpResponseBody = await getLocationsHttpResponse.Content.ReadAsStringAsync();
            // Deserialize json into list of Locations
            List<Location> locations = JsonConvert.DeserializeObject<List<Location>>(getLocationsHttpResponseBody);

            // 3) Get valid location, check location exists and is valid
            Location myLocation = locations.FirstOrDefault(location => location.Name == locationName);
            Assert.NotNull(myLocation);
            Assert.True(myLocation.LocationId > 0);

            // 4) Get location by id
            var getLocationHttpResponse = await _httpClient.GetAsync($"{_baseAddress}/Location/{myLocation.LocationId}");
            Assert.True(getLocationHttpResponse.IsSuccessStatusCode);
            var getLocationHttpResponseBody = await getLocationHttpResponse.Content.ReadAsStringAsync();
            Location location = JsonConvert.DeserializeObject<Location>(getLocationHttpResponseBody);

            // 5) Check myLocation name is equal to returned locations name
            Assert.Equal(locationName, location.Name);

            // 6) Delete location and world by id 
            var deleteLocationHttpResponse = await _httpClient.DeleteAsync($"{_baseAddress}/Location/{myLocation.LocationId}");
            Assert.True(deleteLocationHttpResponse.IsSuccessStatusCode);
            var deleteWorldHttpResponse = await _httpClient.DeleteAsync($"{_baseAddress}/World/{myWorld.WorldId}");
            Assert.True(deleteWorldHttpResponse.IsSuccessStatusCode);
        }

        [Fact]
        public async Task Location_Delete()
        {
            // 1) Create a valid world
            var worldName = $"Test World - {Guid.NewGuid()}";
            var createWorldRequest = new CreateWorldRequest()
            {
                Name = worldName
            };
            // Serialize word to Json
            var jsonBody = JsonConvert.SerializeObject(createWorldRequest);
            // HttpContent with json
            var httpContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");
            var createWorldHttpResponse = await _httpClient.PostAsync($"{_baseAddress}/World", httpContent);
            // Check world successfully created
            Assert.True(createWorldHttpResponse.IsSuccessStatusCode);

            // Get all worlds
            var getWorldsHttpResponse = await _httpClient.GetAsync($"{_baseAddress}/World");
            Assert.True(getWorldsHttpResponse.IsSuccessStatusCode);
            var getWorldsHttpResponseBody = await getWorldsHttpResponse.Content.ReadAsStringAsync();
            List<World> worlds = JsonConvert.DeserializeObject<List<World>>(getWorldsHttpResponseBody);

            // Get valid world, check it exists and is valid
            World myWorld = worlds.FirstOrDefault(world => world.Name == worldName);
            Assert.NotNull(myWorld);
            Assert.True(myWorld.WorldId > 0);

            // 2) Create valid location
            var locationName = $"Test Location - {Guid.NewGuid()}";
            var createLocationRequest = new CreateLocationRequest()
            {
                WorldId = myWorld.WorldId,
                Name = locationName,
                Nationality = "nationality",
                Population = 1000,
                Climate = "climate",
                Terrain = "terrain"
            };

            // Serialize concrete class into a json for the http client to use
            var locationJsonBody = JsonConvert.SerializeObject(createLocationRequest);
            // Http content with json
            var locationHttpContent = new StringContent(locationJsonBody, Encoding.UTF8, "application/json");
            var createLocationHttpResponse = await _httpClient.PostAsync($"{_baseAddress}/Location", locationHttpContent);
            Assert.True(createLocationHttpResponse.IsSuccessStatusCode);

            // 3) Get all locations
            var getLocationsHttpResponse = await _httpClient.GetAsync($"{_baseAddress}/Location");
            Assert.True(getLocationsHttpResponse.IsSuccessStatusCode);
            // Read string content from response
            var getLocationsHttpResponseBody = await getLocationsHttpResponse.Content.ReadAsStringAsync();
            // Deserialize json into list of Locations
            List<Location> locations = JsonConvert.DeserializeObject<List<Location>>(getLocationsHttpResponseBody);

            // 3) Get valid location, check location exists and is valid
            Location myLocation = locations.FirstOrDefault(location => location.Name == locationName);
            Assert.NotNull(myLocation);
            Assert.True(myLocation.LocationId > 0);

            // 4) Get location by id
            var getLocationHttpResponse = await _httpClient.GetAsync($"{_baseAddress}/Location/{myLocation.LocationId}");
            Assert.True(getLocationHttpResponse.IsSuccessStatusCode);
            var getLocationHttpResponseBody = await getLocationHttpResponse.Content.ReadAsStringAsync();
            Location location = JsonConvert.DeserializeObject<Location>(getLocationHttpResponseBody);

            // 5) Check myLocation name is equal to returned locations name
            Assert.Equal(locationName, location.Name);

            // 6) Delete myLocation by id
            var deleteLocationHttpResponse = await _httpClient.DeleteAsync($"{_baseAddress}/Location/{myLocation.LocationId}");
            Assert.True(deleteLocationHttpResponse.IsSuccessStatusCode);

            // 7) Check location no longer exists
            var getDeletedLocationHttpResponse = await _httpClient.GetAsync($"{_baseAddress}/Location/{myLocation.LocationId}");
            Assert.Equal(HttpStatusCode.NotFound, getDeletedLocationHttpResponse.StatusCode);

            // 8) Delete world by id
            var deleteWorldHttpResponse = await _httpClient.DeleteAsync($"{_baseAddress}/World/{myWorld.WorldId}");
            Assert.True(deleteWorldHttpResponse.IsSuccessStatusCode);
        }

        [Fact]
        public async Task Location_WorldDoesNotExist()
        {
            var locationName = $"Test Location - {Guid.NewGuid()}";
            var createLocationRequest = new CreateLocationRequest()
            {
                WorldId = -1,
                Name = locationName,
                Nationality = "nationality",
                Population = 1000,
                Climate = "climate",
                Terrain = "terrain"
            };

            var jsonBody = JsonConvert.SerializeObject(createLocationRequest);
            var httpContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");
            var createLocationHttpResponse = await _httpClient.PostAsync($"{_baseAddress}/Location", httpContent);

            // Check result is not found
            Assert.Equal(HttpStatusCode.NotFound, createLocationHttpResponse.StatusCode);
        }





        [Fact]
        public async Task AreaType_Success()
        {
            // 1) Create valid area type
            var areaTypeName = $"Test Area Type - {Guid.NewGuid()}";
            var createAreaTypeRequest = new CreateAreaTypeRequest()
            {
                Name = areaTypeName
            };

            // Serialise concrete class into json for http client to use
            var jsonBody = JsonConvert.SerializeObject(createAreaTypeRequest);
            // Http content with json body
            var httpContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");
            // Http request to create the new area type
            var createAreaTypeHttpResponse = await _httpClient.PostAsync($"{_baseAddress}/AreaType", httpContent);
            // Check area type was successfully created
            Assert.True(createAreaTypeHttpResponse.IsSuccessStatusCode);

            // 2) Get list of area types
            var getAreaTypesHttpResponse = await _httpClient.GetAsync($"{_baseAddress}/AreaType");
            Assert.True(getAreaTypesHttpResponse.IsSuccessStatusCode);
            // Read content string
            var getAreaTypesHttpResponseBody = await getAreaTypesHttpResponse.Content.ReadAsStringAsync();
            // Deserialise json into list of AreaType objects
            List<AreaType> areaTypes = JsonConvert.DeserializeObject<List<AreaType>>(getAreaTypesHttpResponseBody);

            // Get our areaType from the list of area types
            AreaType myAreaType = areaTypes.FirstOrDefault(areaType => areaType.Name == areaTypeName);
            Assert.NotNull(myAreaType);
            Assert.True(myAreaType.AreaTypeId > 0);

            // 3) Now get our areaType by id by http response
            var getAreaTypeHttpResponse = await _httpClient.GetAsync($"{_baseAddress}/AreaType/{myAreaType.AreaTypeId}");
            Assert.True(getAreaTypeHttpResponse.IsSuccessStatusCode);
            // Read json content from http response
            var getAreaTypeHttpResponseBody = await getAreaTypeHttpResponse.Content.ReadAsStringAsync();
            // Deserialize from json into AreaType object
            AreaType areaType = JsonConvert.DeserializeObject<AreaType>(getAreaTypeHttpResponseBody);

            // 4) Check areaType is not null, and that areaTypeName and areaType name are equal
            Assert.NotNull(areaType);
            Assert.Equal(areaTypeName, areaTypeName);

            // 5) Delete area type by id 
            var deleteAreaTypeHttpResponse = await _httpClient.DeleteAsync($"{_baseAddress}/AreaType/{myAreaType.AreaTypeId}");
            Assert.True(deleteAreaTypeHttpResponse.IsSuccessStatusCode);
        }

        [Fact]
        public async Task AreaType_Delete()
        {
            // 1) Create valid area type
            var areaTypeName = $"Test Area Type - {Guid.NewGuid()}";
            var createAreaTypeRequest = new CreateAreaTypeRequest()
            {
                Name = areaTypeName
            };

            // Serialise concrete class into json for http client to use
            var jsonBody = JsonConvert.SerializeObject(createAreaTypeRequest);
            // Http content with json body
            var httpContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");
            // Http request to create the new area type
            var createAreaTypeHttpResponse = await _httpClient.PostAsync($"{_baseAddress}/AreaType", httpContent);
            // Check area type was successfully created
            Assert.True(createAreaTypeHttpResponse.IsSuccessStatusCode);

            // 2) Get list of area types
            var getAreaTypesHttpResponse = await _httpClient.GetAsync($"{_baseAddress}/AreaType");
            Assert.True(getAreaTypesHttpResponse.IsSuccessStatusCode);
            // Read content string
            var getAreaTypesHttpResponseBody = await getAreaTypesHttpResponse.Content.ReadAsStringAsync();
            // Deserialise json into list of AreaType objects
            List<AreaType> areaTypes = JsonConvert.DeserializeObject<List<AreaType>>(getAreaTypesHttpResponseBody);

            // Get our areaType from the list of area types
            AreaType myAreaType = areaTypes.FirstOrDefault(areaType => areaType.Name == areaTypeName);
            Assert.NotNull(myAreaType);
            Assert.True(myAreaType.AreaTypeId > 0);

            // 3) Now get our areaType by id by http response
            var getAreaTypeHttpResponse = await _httpClient.GetAsync($"{_baseAddress}/AreaType/{myAreaType.AreaTypeId}");
            Assert.True(getAreaTypeHttpResponse.IsSuccessStatusCode);
            // Read json content from http response
            var getAreaTypeHttpResponseBody = await getAreaTypeHttpResponse.Content.ReadAsStringAsync();
            // Deserialize from json into AreaType object
            AreaType areaType = JsonConvert.DeserializeObject<AreaType>(getAreaTypeHttpResponseBody);

            // 4) Check areaType is not null, and that areaTypeName and areaType name are equal
            Assert.NotNull(areaType);
            Assert.Equal(areaTypeName, areaTypeName);

            // 5) Delete areaType by id
            var deleteAreaTypeHttpResponse = await _httpClient.DeleteAsync($"{_baseAddress}/AreaType/{myAreaType.AreaTypeId}");
            Assert.True(deleteAreaTypeHttpResponse.IsSuccessStatusCode);

            // 6) Get areaType by id to check that it is no longer there
            var getDeletedAreaTypeHttpResponse = await _httpClient.GetAsync($"{_baseAddress}/AreaType/{myAreaType.AreaTypeId}");
            Assert.Equal(HttpStatusCode.NotFound, getDeletedAreaTypeHttpResponse.StatusCode);
        }

        [Fact]
        public async Task AreaType_NotFound()
        {
            var invalidAreaTypeId = -1;
            var getAreaTypeHttpResponse = await _httpClient.GetAsync($"{_baseAddress}/AreaType/{invalidAreaTypeId}");
            Assert.Equal(HttpStatusCode.NotFound, getAreaTypeHttpResponse.StatusCode);
        }

        [Fact]
        public async Task AreaType_NameTooLong()
        {
            var invalidAreaTypeName = new string('a', 256);
            var createAreaType = new CreateAreaTypeRequest()
            {
                Name = invalidAreaTypeName
            };

            var jsonBody = JsonConvert.SerializeObject(createAreaType);
            var httpContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");
            var createAreaTypeHttpResponse = await _httpClient.PostAsync($"{_baseAddress}/AreaType", httpContent);
            Assert.Equal(HttpStatusCode.UnprocessableEntity, createAreaTypeHttpResponse.StatusCode);
        }






        [Fact]
        public async Task Area_Success()
        {
            // 1) Create valid world
            var worldName = $"Test World - {Guid.NewGuid()}";
            var createWorldRequest = new CreateWorldRequest()
            {
                Name = worldName
            };

            var worldJsonBody = JsonConvert.SerializeObject(createWorldRequest);
            var worldHttpContent = new StringContent(worldJsonBody, Encoding.UTF8, "application/json");
            var createWorldHttpResponse = await _httpClient.PostAsync($"{_baseAddress}/World", worldHttpContent);
            Assert.True(createWorldHttpResponse.IsSuccessStatusCode);

            var getWorldsHttpResponse = await _httpClient.GetAsync($"{_baseAddress}/World");
            Assert.True(getWorldsHttpResponse.IsSuccessStatusCode);
            var getWorldsHttpResponseBody = await getWorldsHttpResponse.Content.ReadAsStringAsync();
            List<World> worlds = JsonConvert.DeserializeObject<List<World>>(getWorldsHttpResponseBody);

            World myWorld = worlds.FirstOrDefault(world => world.Name == worldName);
            Assert.NotNull(myWorld);
            Assert.True(myWorld.WorldId > 0);

            // 2) Create valid location
            var locationName = $"Test Location - {Guid.NewGuid()}";
            var createLocationRequest = new CreateLocationRequest()
            {
                WorldId = myWorld.WorldId,
                Name = locationName,
                Nationality =  "nationality",
                Population = 1,
                Climate = "climate",
                Terrain = "terrain"
            };

            var locationJsonBody = JsonConvert.SerializeObject(createLocationRequest);
            var locationHttpContent = new StringContent(locationJsonBody, Encoding.UTF8, "application/json");
            var createLocationHttpResponse = await _httpClient.PostAsync($"{_baseAddress}/Location", locationHttpContent);
            Assert.True(createLocationHttpResponse.IsSuccessStatusCode);

            var getLocationsHttpResponse = await _httpClient.GetAsync($"{_baseAddress}/Location");
            Assert.True(getLocationsHttpResponse.IsSuccessStatusCode);
            var getLocationsHttpResponseBody = await getLocationsHttpResponse.Content.ReadAsStringAsync();
            List<Location> locations = JsonConvert.DeserializeObject<List<Location>>(getLocationsHttpResponseBody);

            Location myLocation = locations.FirstOrDefault(location => location.Name == locationName);
            Assert.NotNull(myLocation);
            Assert.True(myLocation.LocationId > 0);

            // 3) Create valid Area Type
            var areaTypeName = $"Test Area Type - {Guid.NewGuid()}";
            var createAreaTypeRequest = new CreateAreaTypeRequest()
            {
                Name = areaTypeName
            };

            var areaTypeJsonBody = JsonConvert.SerializeObject(createAreaTypeRequest);
            var areaTypeHttpContent = new StringContent(areaTypeJsonBody, Encoding.UTF8, "application/json");
            var createAreaTypeHttpResponse = await _httpClient.PostAsync($"{_baseAddress}/AreaType", areaTypeHttpContent);
            Assert.True(createAreaTypeHttpResponse.IsSuccessStatusCode);

            var getAreaTypesHttpResponse = await _httpClient.GetAsync($"{_baseAddress}/AreaType");
            Assert.True(getAreaTypesHttpResponse.IsSuccessStatusCode);
            var getAreaTypesHttpResponseBody = await getAreaTypesHttpResponse.Content.ReadAsStringAsync();
            List<AreaType> areaTypes = JsonConvert.DeserializeObject<List<AreaType>>(getAreaTypesHttpResponseBody);

            AreaType myAreaType = areaTypes.FirstOrDefault(areaType => areaType.Name == areaTypeName);
            Assert.NotNull(myAreaType);
            Assert.True(myAreaType.AreaTypeId > 0);

            // 4) Create valid area
            var areaName = $"Test Area - {Guid.NewGuid()}";
            var createAreaRequest = new CreateAreaRequest()
            {
                LocationId = myLocation.LocationId,
                AreaTypeId = myAreaType.AreaTypeId,
                Name = areaName,
                Size = 1
            };

            var areaJsonBody = JsonConvert.SerializeObject(createAreaRequest);
            var areaHttpContent = new StringContent(areaJsonBody, Encoding.UTF8, "application/json");
            var createAreaHttpResponse = await _httpClient.PostAsync($"{_baseAddress}/Area", areaHttpContent);
            Assert.True(createAreaHttpResponse.IsSuccessStatusCode);

            var getAreasHttpResponse = await _httpClient.GetAsync($"{_baseAddress}/Area");
            Assert.True(getAreasHttpResponse.IsSuccessStatusCode);
            var getAreasHttpResponseBody = await getAreasHttpResponse.Content.ReadAsStringAsync();
            List<Area> areas = JsonConvert.DeserializeObject<List<Area>>(getAreasHttpResponseBody);

            Area myArea = areas.FirstOrDefault(area => area.Name == areaName);
            Assert.NotNull(myArea);
            Assert.True(myArea.AreaId > 0);

            //5) Get myArea by id
            var getAreaHttpResponse = await _httpClient.GetAsync($"{_baseAddress}/Area/{myArea.AreaId}");
            Assert.True(getAreaHttpResponse.IsSuccessStatusCode);
            var getAreaHttpResponseBody = await getAreaHttpResponse.Content.ReadAsStringAsync();
            Area area = JsonConvert.DeserializeObject<Area>(getAreaHttpResponseBody);

            // 6) Check myArea name is equal to returned areas name
            Assert.NotNull(area);
            Assert.Equal(areaName, area.Name);

            // 7) Delete world, location, area type and area by id 
            var deleteAreaHttpResponse = await _httpClient.DeleteAsync($"{_baseAddress}/Area/{myArea.AreaId}");
            Assert.True(deleteAreaHttpResponse.IsSuccessStatusCode);
            var deleteAreaTypeHttpResponse = await _httpClient.DeleteAsync($"{_baseAddress}/AreaType/{myAreaType.AreaTypeId}");
            Assert.True(deleteAreaTypeHttpResponse.IsSuccessStatusCode);
            var deleteLocationHttpResponse = await _httpClient.DeleteAsync($"{_baseAddress}/Location/{myLocation.LocationId}");
            Assert.True(deleteLocationHttpResponse.IsSuccessStatusCode);
            var deleteWorldHttpResponse = await _httpClient.DeleteAsync($"{_baseAddress}/World/{myWorld.WorldId}");
            Assert.True(deleteWorldHttpResponse.IsSuccessStatusCode);
        }

        [Fact]
        public async Task Area_Delete()
        {
            // 1) Create valid world
            var worldName = $"Test World - {Guid.NewGuid()}";
            var createWorldRequest = new CreateWorldRequest()
            {
                Name = worldName
            };

            var worldJsonBody = JsonConvert.SerializeObject(createWorldRequest);
            var worldHttpContent = new StringContent(worldJsonBody, Encoding.UTF8, "application/json");
            var createWorldHttpResponse = await _httpClient.PostAsync($"{_baseAddress}/World", worldHttpContent);
            Assert.True(createWorldHttpResponse.IsSuccessStatusCode);

            var getWorldsHttpResponse = await _httpClient.GetAsync($"{_baseAddress}/World");
            Assert.True(getWorldsHttpResponse.IsSuccessStatusCode);
            var getWorldsHttpResponseBody = await getWorldsHttpResponse.Content.ReadAsStringAsync();
            List<World> worlds = JsonConvert.DeserializeObject<List<World>>(getWorldsHttpResponseBody);

            World myWorld = worlds.FirstOrDefault(world => world.Name == worldName);
            Assert.NotNull(myWorld);
            Assert.True(myWorld.WorldId > 0);

            // 2) Create valid location
            var locationName = $"Test Location - {Guid.NewGuid()}";
            var createLocationRequest = new CreateLocationRequest()
            {
                WorldId = myWorld.WorldId,
                Name = locationName,
                Nationality = "nationality",
                Population = 1,
                Climate = "climate",
                Terrain = "terrain"
            };

            var locationJsonBody = JsonConvert.SerializeObject(createLocationRequest);
            var locationHttpContent = new StringContent(locationJsonBody, Encoding.UTF8, "application/json");
            var createLocationHttpResponse = await _httpClient.PostAsync($"{_baseAddress}/Location", locationHttpContent);
            Assert.True(createLocationHttpResponse.IsSuccessStatusCode);

            var getLocationsHttpResponse = await _httpClient.GetAsync($"{_baseAddress}/Location");
            Assert.True(getLocationsHttpResponse.IsSuccessStatusCode);
            var getLocationsHttpResponseBody = await getLocationsHttpResponse.Content.ReadAsStringAsync();
            List<Location> locations = JsonConvert.DeserializeObject<List<Location>>(getLocationsHttpResponseBody);

            Location myLocation = locations.FirstOrDefault(location => location.Name == locationName);
            Assert.NotNull(myLocation);
            Assert.True(myLocation.LocationId > 0);

            // 3) Create valid Area Type
            var areaTypeName = $"Test Area Type - {Guid.NewGuid()}";
            var createAreaTypeRequest = new CreateAreaTypeRequest()
            {
                Name = areaTypeName
            };

            var areaTypeJsonBody = JsonConvert.SerializeObject(createAreaTypeRequest);
            var areaTypeHttpContent = new StringContent(areaTypeJsonBody, Encoding.UTF8, "application/json");
            var createAreaTypeHttpResponse = await _httpClient.PostAsync($"{_baseAddress}/AreaType", areaTypeHttpContent);
            Assert.True(createAreaTypeHttpResponse.IsSuccessStatusCode);

            var getAreaTypesHttpResponse = await _httpClient.GetAsync($"{_baseAddress}/AreaType");
            Assert.True(getAreaTypesHttpResponse.IsSuccessStatusCode);
            var getAreaTypesHttpResponseBody = await getAreaTypesHttpResponse.Content.ReadAsStringAsync();
            List<AreaType> areaTypes = JsonConvert.DeserializeObject<List<AreaType>>(getAreaTypesHttpResponseBody);

            AreaType myAreaType = areaTypes.FirstOrDefault(areaType => areaType.Name == areaTypeName);
            Assert.NotNull(myAreaType);
            Assert.True(myAreaType.AreaTypeId > 0);

            // 4) Create valid area
            var areaName = $"Test Area - {Guid.NewGuid()}";
            var createAreaRequest = new CreateAreaRequest()
            {
                LocationId = myLocation.LocationId,
                AreaTypeId = myAreaType.AreaTypeId,
                Name = areaName,
                Size = 1
            };

            var areaJsonBody = JsonConvert.SerializeObject(createAreaRequest);
            var areaHttpContent = new StringContent(areaJsonBody, Encoding.UTF8, "application/json");
            var createAreaHttpResponse = await _httpClient.PostAsync($"{_baseAddress}/Area", areaHttpContent);
            Assert.True(createAreaHttpResponse.IsSuccessStatusCode);

            var getAreasHttpResponse = await _httpClient.GetAsync($"{_baseAddress}/Area");
            Assert.True(getAreasHttpResponse.IsSuccessStatusCode);
            var getAreasHttpResponseBody = await getAreasHttpResponse.Content.ReadAsStringAsync();
            List<Area> areas = JsonConvert.DeserializeObject<List<Area>>(getAreasHttpResponseBody);

            Area myArea = areas.FirstOrDefault(area => area.Name == areaName);
            Assert.NotNull(myArea);
            Assert.True(myArea.AreaId > 0);

            // 5) Get myArea by id
            var getAreaHttpResponse = await _httpClient.GetAsync($"{_baseAddress}/Area/{myArea.AreaId}");
            Assert.True(getAreaHttpResponse.IsSuccessStatusCode);
            var getAreaHttpResponseBody = await getAreaHttpResponse.Content.ReadAsStringAsync();
            Area area = JsonConvert.DeserializeObject<Area>(getAreaHttpResponseBody);

            Assert.NotNull(area);
            Assert.Equal(areaName, area.Name);

            // 6) delete area by id
            var deleteAreaHttpResponse = await _httpClient.DeleteAsync($"{_baseAddress}/Area/{myArea.AreaId}");
            Assert.True(deleteAreaHttpResponse.IsSuccessStatusCode);

            // 7) Get area by id and check that it no longer exists
            var getDeletedAreaHttpResponse = await _httpClient.GetAsync($"{_baseAddress}/Area/{myArea.AreaId}");
            Assert.Equal(HttpStatusCode.NotFound, getDeletedAreaHttpResponse.StatusCode);

            // 8) Delete world, location and area type by id
            var deleteAreaTypeHttpResponse = await _httpClient.DeleteAsync($"{_baseAddress}/AreaType/{myAreaType.AreaTypeId}");
            Assert.True(deleteAreaTypeHttpResponse.IsSuccessStatusCode);
            var deleteLocationHttpResponse = await _httpClient.DeleteAsync($"{_baseAddress}/Location/{myLocation.LocationId}");
            Assert.True(deleteLocationHttpResponse.IsSuccessStatusCode);
            var deleteWorldHttpResponse = await _httpClient.DeleteAsync($"{_baseAddress}/World/{myWorld.WorldId}");
            Assert.True(deleteWorldHttpResponse.IsSuccessStatusCode);
        }


        [Fact]
        public async Task Area_NameTooLong()
        {
            // 1) Create valid world
            var worldName = $"Test World - {Guid.NewGuid()}";
            var createWorldRequest = new CreateWorldRequest()
            {
                Name = worldName
            };

            var worldJsonBody = JsonConvert.SerializeObject(createWorldRequest);
            var worldHttpContent = new StringContent(worldJsonBody, Encoding.UTF8, "application/json");
            var createWorldHttpResponse = await _httpClient.PostAsync($"{_baseAddress}/World", worldHttpContent);
            Assert.True(createWorldHttpResponse.IsSuccessStatusCode);

            var getWorldsHttpResponse = await _httpClient.GetAsync($"{_baseAddress}/World");
            Assert.True(getWorldsHttpResponse.IsSuccessStatusCode);
            var getWorldsHttpResponseBody = await getWorldsHttpResponse.Content.ReadAsStringAsync();
            List<World> worlds = JsonConvert.DeserializeObject<List<World>>(getWorldsHttpResponseBody);

            World myWorld = worlds.FirstOrDefault(world => world.Name == worldName);
            Assert.NotNull(myWorld);
            Assert.True(myWorld.WorldId > 0);

            // 2) Create valid location
            var locationName = $"Test Location - {Guid.NewGuid()}";
            var createLocationRequest = new CreateLocationRequest()
            {
                WorldId = myWorld.WorldId,
                Name = locationName,
                Nationality = "nationality",
                Population = 1,
                Climate = "climate",
                Terrain = "terrain"
            };

            var locationJsonBody = JsonConvert.SerializeObject(createLocationRequest);
            var locationHttpContent = new StringContent(locationJsonBody, Encoding.UTF8, "application/json");
            var createLocationHttpResponse = await _httpClient.PostAsync($"{_baseAddress}/Location", locationHttpContent);
            Assert.True(createLocationHttpResponse.IsSuccessStatusCode);

            var getLocationsHttpResponse = await _httpClient.GetAsync($"{_baseAddress}/Location");
            Assert.True(getLocationsHttpResponse.IsSuccessStatusCode);
            var getLocationsHttpResponseBody = await getLocationsHttpResponse.Content.ReadAsStringAsync();
            List<Location> locations = JsonConvert.DeserializeObject<List<Location>>(getLocationsHttpResponseBody);

            Location myLocation = locations.FirstOrDefault(location => location.Name == locationName);
            Assert.NotNull(myLocation);
            Assert.True(myLocation.LocationId > 0);

            // 3) Create valid Area Type
            var areaTypeName = $"Test Area Type - {Guid.NewGuid()}";
            var createAreaTypeRequest = new CreateAreaTypeRequest()
            {
                Name = areaTypeName
            };

            var areaTypeJsonBody = JsonConvert.SerializeObject(createAreaTypeRequest);
            var areaTypeHttpContent = new StringContent(areaTypeJsonBody, Encoding.UTF8, "application/json");
            var createAreaTypeHttpResponse = await _httpClient.PostAsync($"{_baseAddress}/AreaType", areaTypeHttpContent);
            Assert.True(createAreaTypeHttpResponse.IsSuccessStatusCode);

            var getAreaTypesHttpResponse = await _httpClient.GetAsync($"{_baseAddress}/AreaType");
            Assert.True(getAreaTypesHttpResponse.IsSuccessStatusCode);
            var getAreaTypesHttpResponseBody = await getAreaTypesHttpResponse.Content.ReadAsStringAsync();
            List<AreaType> areaTypes = JsonConvert.DeserializeObject<List<AreaType>>(getAreaTypesHttpResponseBody);

            AreaType myAreaType = areaTypes.FirstOrDefault(areaType => areaType.Name == areaTypeName);
            Assert.NotNull(myAreaType);
            Assert.True(myAreaType.AreaTypeId > 0);

            // 4) Create area with invalid AreaName
            var areaName = new string('a', 256);
            var createAreaRequest = new CreateAreaRequest()
            {
                LocationId = myLocation.LocationId,
                AreaTypeId = myAreaType.AreaTypeId,
                Name = areaName,
                Size = 1
            };

            var areaJsonBody = JsonConvert.SerializeObject(createAreaRequest);
            var areaHttpContent = new StringContent(areaJsonBody, Encoding.UTF8, "application/json");
            var createAreaHttpResponse = await _httpClient.PostAsync($"{_baseAddress}/Area", areaHttpContent);
            Assert.Equal(HttpStatusCode.UnprocessableEntity, createAreaHttpResponse.StatusCode);

            // 5) Delete world, location and area type by id
            var deleteAreaTypeHttpResponse = await _httpClient.DeleteAsync($"{_baseAddress}/AreaType/{myAreaType.AreaTypeId}");
            Assert.True(deleteAreaTypeHttpResponse.IsSuccessStatusCode);
            var deleteLocationHttpResponse = await _httpClient.DeleteAsync($"{_baseAddress}/Location/{myLocation.LocationId}");
            Assert.True(deleteLocationHttpResponse.IsSuccessStatusCode);
            var deleteWorldHttpResponse = await _httpClient.DeleteAsync($"{_baseAddress}/World/{myWorld.WorldId}");
            Assert.True(deleteWorldHttpResponse.IsSuccessStatusCode);
        }

        [Fact]
        public async Task Area_LocationNotFound()
        {
            // 1) Create valid area type
            var areaTypeName = $"Test Area Type - {Guid.NewGuid()}";
            var createAreaTypeRequest = new CreateAreaTypeRequest()
            {
                Name = areaTypeName
            };

            var areaTypeJsonBody = JsonConvert.SerializeObject(createAreaTypeRequest);
            var areaTypeHttpContent = new StringContent(areaTypeJsonBody, Encoding.UTF8, "application/json");
            var createAreaTypeHttpResponse = await _httpClient.PostAsync($"{_baseAddress}/AreaType", areaTypeHttpContent);
            Assert.True(createAreaTypeHttpResponse.IsSuccessStatusCode);

            var getAreaTypesHttpResponse = await _httpClient.GetAsync($"{_baseAddress}/AreaType");
            Assert.True(getAreaTypesHttpResponse.IsSuccessStatusCode);
            var getAreaTypesHttpResponseBody = await getAreaTypesHttpResponse.Content.ReadAsStringAsync();
            List<AreaType> areaTypes = JsonConvert.DeserializeObject<List<AreaType>>(getAreaTypesHttpResponseBody);

            AreaType myAreaType = areaTypes.FirstOrDefault(areaType => areaType.Name == areaTypeName);
            Assert.NotNull(myAreaType);
            Assert.True(myAreaType.AreaTypeId > 0);

            // 2) Create area with invalid LocationId
            var areaName = $"Test Area - {Guid.NewGuid()}";
            var createAreaRequest = new CreateAreaRequest()
            {
                LocationId = -1,
                AreaTypeId = myAreaType.AreaTypeId,
                Name = areaName,
                Size = 1
            };

            var areaJsonBody = JsonConvert.SerializeObject(createAreaRequest);
            var areaHttpContent = new StringContent(areaJsonBody, Encoding.UTF8, "application/json");
            var createAreaHttpResponse = await _httpClient.PostAsync($"{_baseAddress}/Area", areaHttpContent);
            Assert.Equal(HttpStatusCode.NotFound, createAreaHttpResponse.StatusCode);

            // 3) Delete area type by id
            var deleteAreaTypeHttpResponse = await _httpClient.DeleteAsync($"{_baseAddress}/AreaType/{myAreaType.AreaTypeId}");
            Assert.True(deleteAreaTypeHttpResponse.IsSuccessStatusCode);
        }

        [Fact]
        public async Task Area_AreaTypeNotFound()
        {
            // 1) Create valid world
            var worldName = $"Test World - {Guid.NewGuid()}";
            var createWorldRequest = new CreateWorldRequest()
            {
                Name = worldName
            };

            var worldJsonBody = JsonConvert.SerializeObject(createWorldRequest);
            var worldHttpContent = new StringContent(worldJsonBody, Encoding.UTF8, "application/json");
            var createWorldHttpResponse = await _httpClient.PostAsync($"{_baseAddress}/World", worldHttpContent);
            Assert.True(createWorldHttpResponse.IsSuccessStatusCode);

            var getWorldsHttpResponse = await _httpClient.GetAsync($"{_baseAddress}/World");
            Assert.True(getWorldsHttpResponse.IsSuccessStatusCode);
            var getWorldsHttpResponseBody = await getWorldsHttpResponse.Content.ReadAsStringAsync();
            List<World> worlds = JsonConvert.DeserializeObject<List<World>>(getWorldsHttpResponseBody);

            World myWorld = worlds.FirstOrDefault(world => world.Name == worldName);
            Assert.NotNull(myWorld);
            Assert.True(myWorld.WorldId > 0);

            // 2) Create valid location
            var locationName = $"Test Location - {Guid.NewGuid()}";
            var createLocationRequest = new CreateLocationRequest()
            {
                WorldId = myWorld.WorldId,
                Name = locationName,
                Nationality = "nationality",
                Population = 1,
                Climate = "climate",
                Terrain = "terrain"
            };

            var locationJsonBody = JsonConvert.SerializeObject(createLocationRequest);
            var locationHttpContent = new StringContent(locationJsonBody, Encoding.UTF8, "application/json");
            var createLocationHttpResponse = await _httpClient.PostAsync($"{_baseAddress}/Location", locationHttpContent);
            Assert.True(createLocationHttpResponse.IsSuccessStatusCode);

            var getLocationsHttpResponse = await _httpClient.GetAsync($"{_baseAddress}/Location");
            Assert.True(getLocationsHttpResponse.IsSuccessStatusCode);
            var getLocationsHttpResponseBody = await getLocationsHttpResponse.Content.ReadAsStringAsync();
            List<Location> locations = JsonConvert.DeserializeObject<List<Location>>(getLocationsHttpResponseBody);

            Location myLocation = locations.FirstOrDefault(location => location.Name == locationName);
            Assert.NotNull(myLocation);
            Assert.True(myLocation.LocationId > 0);

            // 3) Create area with invalid AreaTypeId
            var areaName = $"Test Area - {Guid.NewGuid()}";
            var createAreaRequest = new CreateAreaRequest()
            {
                LocationId = myLocation.LocationId,
                AreaTypeId = -1,
                Name = areaName,
                Size = 1
            };

            var areaJsonBody = JsonConvert.SerializeObject(createAreaRequest);
            var areaHttpContent = new StringContent(areaJsonBody, Encoding.UTF8, "application/json");
            var createAreaHttpResponse = await _httpClient.PostAsync($"{_baseAddress}/Area", areaHttpContent);
            Assert.Equal(HttpStatusCode.NotFound, createAreaHttpResponse.StatusCode);

            // 5) Delete world and location by id
            var deleteLocationHttpResponse = await _httpClient.DeleteAsync($"{_baseAddress}/Location/{myLocation.LocationId}");
            Assert.True(deleteLocationHttpResponse.IsSuccessStatusCode);
            var deleteWorldHttpResponse = await _httpClient.DeleteAsync($"{_baseAddress}/World/{myWorld.WorldId}");
            Assert.True(deleteWorldHttpResponse.IsSuccessStatusCode);
        }

        [Fact]
        public async Task Area_NotFound()
        {
            var invalidAreaId = -1;
            var getAreaHttpResponse = await _httpClient.GetAsync($"{_baseAddress}/Area/{invalidAreaId}");
            Assert.Equal(HttpStatusCode.NotFound, getAreaHttpResponse.StatusCode);
        }
    }
}