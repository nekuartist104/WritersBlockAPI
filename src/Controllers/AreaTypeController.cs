using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using WritersBlockAPI.Controllers.Requests;
using WritersBlockAPI.Models;
using WritersBlockAPI.Repositories;

namespace WritersBlockAPI.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class AreaTypeController : ControllerBase
    {

        private readonly ILogger<AreaTypeController> _logger;
        private readonly IAreaTypeRepository _areaTypeRepository;

        public AreaTypeController(ILogger<AreaTypeController> logger, IAreaTypeRepository areaTypeRepository)
        {
            _logger = logger;
            _areaTypeRepository = areaTypeRepository;
        }

        [HttpGet]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [AllowAnonymous]
        public IActionResult Index()
        {
            List<AreaType> areaTypes = _areaTypeRepository.All();

            return new OkObjectResult(areaTypes);
        }

        [HttpGet]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [AllowAnonymous]
        [Route("{areaTypeId}")]
        public IActionResult Get(int areaTypeId)
        {
            AreaType areaType = _areaTypeRepository.Find(areaTypeId);
            if (areaType == null)
            {
                return new NotFoundResult();
            }

            return new OkObjectResult(areaType);
        }

        [HttpPost]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [AllowAnonymous]
        public IActionResult Create(CreateAreaTypeRequest createAreaTypeRequest)
        {
            try
            {
                _areaTypeRepository.Create(createAreaTypeRequest);
            }
            catch (SqlException ex)
            {
                if (ex.Number == 2627)
                {
                    // Hit unique constraint
                    return new ConflictObjectResult($"Area type with name {createAreaTypeRequest.Name} already exists");
                }
                if (ex.Number == 2628)
                {
                    // name too long
                    return new UnprocessableEntityObjectResult($"Name {createAreaTypeRequest.Name} is too long");
                }

                throw;
            }

            return new CreatedResult();
        }

        [HttpDelete]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [AllowAnonymous]
        [Route("{areaTypeId}")]

        public IActionResult Delete(int areaTypeId)
        {
            _areaTypeRepository.Destroy(areaTypeId);
            return new OkResult();
        }


    }
}
