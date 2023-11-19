using AutoMapper;
using Rent.Entities.Cities;
using Rent.Service.Services.Cities.Views;

namespace RentAPI.Infrastructure.Mapper
{
    public class ViewMapper : Profile
    {
        public ViewMapper()
        {
            CreateMap<City, CityView>();
        }
    }
}
