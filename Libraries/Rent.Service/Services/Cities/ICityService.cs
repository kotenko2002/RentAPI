using Rent.Entities.Cities;
using Rent.Service.Services.Cities.Views;

namespace Rent.Service.Services.Cities
{
    public interface ICityService
    {
        Task<IEnumerable<CityView>> GetAllCitiesAsync();
    }
}
