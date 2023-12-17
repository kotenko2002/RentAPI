using Microsoft.AspNetCore.Mvc;
using Rent.Entities.Cities;
using Rent.Storage.Uow;

namespace RentAPI.Controllers
{
    [ApiController, Route("[controller]")]
    public class TestController : ControllerBase
    {
        private readonly IUnitOfWork _uow;

        public TestController(IUnitOfWork uow) => _uow = uow;

        [HttpGet]
        public async Task<ActionResult<string>> Test()
        {
            await _uow.CityRepository.AddAsync(new City() { Id = 99, Name = "Poko" });
            await _uow.CompleteAsync();

            var cities = await _uow.CityRepository.FindAllAsync();
            return cities.Count().ToString();
        }
    }
}
