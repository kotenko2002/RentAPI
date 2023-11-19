using Rent.Service.Services.Authorization;
using Rent.Service.Services.Cities;
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
        }

        public static void AddRepositories(this IServiceCollection repositories)
        {
            repositories.AddScoped<IUnitOfWork, UnitOfWork>();
        }
    }
}
