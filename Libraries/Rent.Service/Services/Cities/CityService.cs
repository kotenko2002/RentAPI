using AutoMapper;
using Rent.Entities.Cities;
using Rent.Service.Services.Cities.Views;
using Rent.Storage.Uow;

namespace Rent.Service.Services.Cities
{
    public class CityService : ICityService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _uow;

        public CityService(
            IMapper mapper,
            IUnitOfWork uow)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CityView>> GetAllCitiesAsync()
        {
            IEnumerable<City> cities = await _uow.CityRepository.FindAllAsync();

            return _mapper.Map<IEnumerable<CityView>>(cities);
        }
    }
}
