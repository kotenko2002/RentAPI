using AutoMapper;
using Rent.Service.Authorization.Descriptors;
using RentAPI.Models.Auth;

namespace RentAPI.Infrastructure.Mapper
{
    public class DescriptorMapper : Profile
    {
        public DescriptorMapper()
        {
            CreateMap<LoginModel, LoginDescriptor>();
        }
    }
}
