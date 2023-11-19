using AutoMapper;
using Rent.Entities.Cities;
using Rent.Entities.Properties;
using Rent.Service.Services.Authorization.Descriptors;
using Rent.Service.Services.Cities.Views;
using Rent.Service.Services.Properties.Descriptors;
using RentAPI.Models.Auth;
using RentAPI.Models.Properties;

namespace RentAPI.Infrastructure.Mapper
{
    public class Mapper : Profile
    {
        public Mapper()
        {
            CreateMap<LoginModel, LoginDescriptor>();
            CreateMap<RegisterModel, RegisterDescriptor>();
            CreateMap<RefreshTokensModel, RefreshTokensDescriptor>();
            CreateMap<EditPropertyModel, EditPropertyDescriptor>();

            CreateMap<AddPropertyModel, Property>();

            CreateMap<City, CityView>();

        }
    }
}
