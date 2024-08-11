using Newtonsoft.Json;
using NuGet.Frameworks;
using System.Net;
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
    }
}