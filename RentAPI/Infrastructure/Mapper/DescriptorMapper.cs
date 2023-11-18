using AutoMapper;
using Rent.Service.Services.Authorization.Descriptors;
using RentAPI.Models.Auth;

namespace RentAPI.Infrastructure.Mapper
{
    public class DescriptorMapper : Profile
    {
        public DescriptorMapper()
        {
            CreateMap<LoginModel, LoginDescriptor>();
            CreateMap<RegisterModel, RegisterDescriptor>();
            CreateMap<RefreshTokensModel, RefreshTokensDescriptor>();
        }
    }
}
