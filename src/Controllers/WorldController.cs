using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using WritersBlockAPI.Controllers.Requests;
using WritersBlockAPI.Models;
using WritersBlockAPI.Repositories;

namespace WritersBlockAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WorldController : ControllerBase
    {
        private readonly ILogger<WorldController> _logger;
        private readonly IWorldRepository _worldRepository;

        public WorldController(ILogger<WorldController> logger, IWorldRepository worldRepository)
        {
            _logger = logger;
            _worldRepository = worldRepository;
        }

        [HttpGet]
        [Produces("application/json")]
        [AllowAnonymous]
        public IActionResult Index()
        {
            List<World> worlds = _worldRepository.All();

            return new OkObjectResult(worlds);
        }

        [HttpGet]
        [Produces("application/json")]
        [AllowAnonymous]
        [Route("{worldId}")]
        public IActionResult Get(int worldId)
        {
            World world = _worldRepository.Find(worldId);

            if (world == null)
            {
                return new NotFoundResult();
            }

            return new OkObjectResult(world);
        }

        [HttpDelete]
        [Produces("application/json")]
        [AllowAnonymous]
        [Route("{worldId}")]
        public IActionResult Delete(int worldId)
        {
            _worldRepository.Destroy(worldId);

            return new OkResult();
        }

        [HttpPost]
        [Produces("application/json")]
        [AllowAnonymous]
        public IActionResult Create(CreateWorldRequest createWorldRequest)
        {
            try
            {
                _worldRepository.Create(createWorldRequest);
            }
            catch (SqlException ex)
            {
                if (ex.Number == 2627)
                {
                    // Hit unique constraint
                    return new ConflictObjectResult($"World with name {createWorldRequest.Name} already exists");
                }

                if (ex.Number == 2628)
                {
                    // name too long
                    return new UnprocessableEntityObjectResult($"Name {createWorldRequest.Name} is too long");
                }
            }

            return new OkResult();
        }
    }
}
