using AutoMapper;
using Rent.Entities.Cities;
using Rent.Entities.Properties;
using Rent.Entities.Responses;
using Rent.Service.Services.Authorization.Descriptors;
using Rent.Service.Services.Cities.Views;
using Rent.Service.Services.Properties.Descriptors;
using Rent.Service.Services.Responses.Descriptors;
using Rent.Service.Services.Responses.Views;
using RentAPI.Models.Auth;
using RentAPI.Models.Properties;
using RentAPI.Models.Responses;

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
            CreateMap<AddPropertyModel, AddPropertyDescriptor>();
            CreateMap<ProcessResponseModel, ProcessResponseDescriptor>();

            CreateMap<AddResponseModel, Response>();//del?

            CreateMap<AddPropertyDescriptor, Property>(); 

            CreateMap<City, CityView>();
            CreateMap<Response, ResponseView>();
        }
    }
}
