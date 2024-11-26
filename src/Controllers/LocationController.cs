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
    public class LocationController : ControllerBase
    {
        private readonly ILogger<LocationController> _logger;
        private readonly ILocationRepository _locationRepository;

        public LocationController(ILogger<LocationController> logger, ILocationRepository locationRepository)
        {
            _logger = logger;
            _locationRepository = locationRepository;
        }

        [HttpGet]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [AllowAnonymous]
        public IActionResult Index([FromQuery] int? worldId)
        {
            List<Location> locations;

            if (worldId.HasValue)
            {
                locations = _locationRepository.All(worldId.Value);
            }
            else
            {
                locations = _locationRepository.All();
            }

            return new OkObjectResult(locations);
        }

        [HttpGet]
        [Produces("application/json")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("{locationId}")]
        public IActionResult Get(int locationId)
        {
            Location location = _locationRepository.Find(locationId);

            if (location == null)
            {
                return new NotFoundResult();
            }

            return new OkObjectResult(location);
        }

        [HttpDelete]
        [Produces("application/json")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Route("{locationId}")]
        public IActionResult Delete(int locationId)
        {
            _locationRepository.Destroy(locationId);

            return new OkResult();
        }

        [HttpPost]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [AllowAnonymous]
        public IActionResult Create(CreateLocationRequest createLocationRequest)
        {
            try
            {
                _locationRepository.Create(createLocationRequest);
            }
            catch (SqlException ex)
            {
                switch (ex.Number)
                {
                    case 2627:
                        //Hit unique constraint
                        return new ConflictObjectResult($"Location with name {createLocationRequest.Name} already exists");

                    case 2628:
                        //name too long
                        string error = ex.Message;
                        if (error.Contains("Name"))
                        {
                            return new UnprocessableEntityObjectResult($"Name {createLocationRequest.Name} is too long");
                        }
                        if (error.Contains("Nationality"))
                        {
                            return new UnprocessableEntityObjectResult($"Nationality {createLocationRequest.Nationality} is too long");
                        }
                        if (error.Contains("Climate"))
                        {
                            return new UnprocessableEntityObjectResult($"Climate {createLocationRequest.Climate} is too long");
                        }
                        if (error.Contains("Terrain"))
                        {
                            return new UnprocessableEntityObjectResult($"Terrain {createLocationRequest.Terrain} is too long");
                        }
                        else
                        {
                            return new UnprocessableEntityObjectResult($"Something went wrong somewhere");
                        }

                    case 547:
                        //World not found with specified worldId
                        return new NotFoundObjectResult($"World with Id {createLocationRequest.WorldId} does not exist");

                    default:
                        throw;
                }
            }

            return new CreatedResult();
        }
    }
}
