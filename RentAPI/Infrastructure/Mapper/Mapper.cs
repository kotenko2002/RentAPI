using AutoMapper;
using Rent.Entities.Cities;
using Rent.Entities.Comments;
using Rent.Entities.Properties;
using Rent.Entities.Responses;
using Rent.Service.Services.Authorization.Descriptors;
using Rent.Service.Services.Cities.Views;
using Rent.Service.Services.Comments.Views;
using Rent.Service.Services.Properties.Descriptors;
using Rent.Service.Services.Properties.Views;
using Rent.Service.Services.Responses.Descriptors;
using Rent.Service.Services.Responses.Views;
using RentAPI.Models.Auth;
using RentAPI.Models.Comments;
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

            CreateMap<AddResponseModel, Response>();
            CreateMap<AddCommentModel, Comment>();

            CreateMap<AddPropertyDescriptor, Property>()
                .ForMember(dest => dest.Photos, opt => opt.Ignore());

            CreateMap<City, CityView>();
            CreateMap<Response, ResponseView>();
            CreateMap<Comment, CommentView>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Tenant.UserName));
            CreateMap<Property, PropertyView>();
        }
    }
}
