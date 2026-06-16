using Microsoft.AspNetCore.Mvc;
using TEST_ONE_RETAKE.Repositories;
using TEST_ONE_RETAKE.DTOs;
using TEST_ONE_RETAKE.Services;

namespace TEST1_REVISION.Controllers
{
    [ApiController]
    [Route("api/stores")]
    public class MakersController : ControllerBase
    {
        private readonly IStoreService _storeService;

        public MakersController(IStoreService storeService)
        {
            _storeService = storeService;
        }

        [HttpGet("{name:string}")]
        public async Task<IActionResult> GetStores(string? name)
        {
            var maker = await _storeService.GetAllStoresAsync(name);

            if (maker is null)
            {
                return NotFound($"stores with provided {name} do not exist");
            }

            return Ok(maker);
        }

        [HttpPost]
        public async Task<IActionResult> CreateStore([FromBody] CreateStoreDTO createStoreDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var errorMsg = await _storeService.CreateStoreAsync(createStoreDto);

            
            if (errorMsg is not null)
            {
                return Conflict(errorMsg);
            }

            return Ok("Sucesfully created");
        }
    }
}