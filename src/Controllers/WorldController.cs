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
        [ProducesResponseType(StatusCodes.Status200OK)]
        [AllowAnonymous]
        public IActionResult Index()
        {
            List<World> worlds = _worldRepository.All();

            return new OkObjectResult(worlds);
        }

        [HttpGet]
        [Produces("application/json")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Route("{worldId}")]
        public IActionResult Delete(int worldId)
        {
            _worldRepository.Destroy(worldId);

            return new OkResult();
        }

        [HttpPost]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [AllowAnonymous]
        public IActionResult Create(CreateWorldRequest createWorldRequest)
        {
            try
            {
                _worldRepository.Create(createWorldRequest);
            }
            catch (SqlException ex)
            {
                switch (ex.Number)
                {
                    case 2627:
                        // Hit unique constraint
                        return new ConflictObjectResult($"World with name {createWorldRequest.Name} already exists");

                    case 2628:
                        // name too long
                        return new UnprocessableEntityObjectResult($"Name {createWorldRequest.Name} is too long");

                    default:
                        throw;
                    }
                }

            return new CreatedResult();
        }
    }
}
