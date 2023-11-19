using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rent.Entities.Cities;
using Rent.Service.Services.Cities;
using Rent.Service.Services.FileStorage;

namespace RentAPI.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("[controller]")]
    public class CityController : ControllerBase
    {
        private readonly ICityService _cityService;
        
        public CityController(
            ICityService cityService)
        {
            _cityService = cityService;
        }

        [HttpGet("items")]
        public async Task<IActionResult> GetAllCities()
        {
            return Ok(await _cityService.GetAllCities());
        }
    }
}
