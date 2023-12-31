﻿using Rent.Service.Services.Authorization;
using Rent.Service.Services.Cities;
using Rent.Service.Services.Comments;
using Rent.Service.Services.FileStorage;
using Rent.Service.Services.Properties;
using Rent.Service.Services.Responses;
using Rent.Storage.Uow;

namespace RentAPI.Infrastructure.Extensions
{
    public static class ServicesExtensions
    {
        public static void AddCustomServices(this IServiceCollection services)
        {
            services.AddServices();
            services.AddRepositories();
        }

        public static void AddServices(this IServiceCollection services)
        {
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ICityService, CityService>();
            services.AddScoped<IPropertyService, PropertyService>();
            services.AddScoped<IResponseService, ResponseService>();
            services.AddScoped<ICommentService, CommentService>();

            services.AddScoped<IFileStorageService, GoogleDrive>();
        }

        public static void AddRepositories(this IServiceCollection repositories)
        {
            repositories.AddScoped<IUnitOfWork, UnitOfWork>();
        }
    }
}
