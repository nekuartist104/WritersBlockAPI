using Microsoft.AspNetCore.Mvc;
using WritersBlockAPI.Repositories;
using WritersBlockAPI.Models;
using Microsoft.AspNetCore.Authorization;
using WritersBlockAPI.Controllers.Requests;
using System.Reflection.Metadata.Ecma335;
using Microsoft.Data.SqlClient;

namespace WritersBlockAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AreaController : ControllerBase
    {

        private readonly ILogger<AreaController> _logger;
        private readonly IAreaRepository _areaRepository;

        public AreaController(ILogger<AreaController> logger, IAreaRepository areaRepository)
        {
            _logger = logger;
            _areaRepository = areaRepository;
        }

        [HttpGet]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [AllowAnonymous]
        public IActionResult Index([FromQuery] int? locationId)
        {
            List<Area> areas;

            if (locationId.HasValue)
            {
                areas = _areaRepository.All(locationId.Value);
            }
            else
            {
                areas = _areaRepository.All();
            }

            return new OkObjectResult(areas);
        }

        [HttpPost]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult Create(CreateAreaRequest createAreaRequest)
        {
            try
            {
                _areaRepository.Create(createAreaRequest);
            }
            catch (SqlException ex)
            {
                switch (ex.Number)
                {
                    case 2627:
                        //Hit unique constraint
                        return new ConflictObjectResult($"Area with name {createAreaRequest.Name} already exists");

                    case 2628:
                        //name too long
                        return new UnprocessableEntityObjectResult($"Name {createAreaRequest.Name} is too long");

                    case 547:
                        //Location or AreaType not found with specified locationId or areaTypeId
                        if (ex.Message.Contains("LocationId"))
                        {
                            return new NotFoundObjectResult($"Location with Id {createAreaRequest.LocationId} doesn't exist");
                        }

                        else if (ex.Message.Contains("AreaTypeId"))
                        {
                            return new NotFoundObjectResult($"Area type with Id {createAreaRequest.AreaTypeId} doesn't exist");

                        }

                        else
                        {
                            return new NotFoundObjectResult("Something went wrong somewhere");
                        }

                    default:
                        throw;
                }
            }

            return new CreatedResult();
        }

        [HttpGet]
        [Produces("application/json")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("{areaId}")]
        public IActionResult Get(int areaId)
        {
            Area area = _areaRepository.Find(areaId);

            if (area == null)
            {
                return new NotFoundResult();
            }

            return new OkObjectResult(area);
        }

        [HttpDelete]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Route("{areaId}")]
        public IActionResult delete(int areaId)
        {
            _areaRepository.Destroy(areaId);

            return new OkResult();
        }
    }
}
