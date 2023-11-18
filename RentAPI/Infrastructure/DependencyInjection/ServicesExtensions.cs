using Rent.Service.Authorization;

namespace RentAPI.Infrastructure.DependencyInjection
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
        }

        public static void AddRepositories(this IServiceCollection repositories)
        {
        }
    }
}
