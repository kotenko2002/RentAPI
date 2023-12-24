using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Rent.Service.Configuration;
using Rent.Service.Services.Authorization;
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
        public static void AddCustomServices(this IServiceCollection services, GoogleDriveConfig googleDriveConfigs)
        {
            services.AddServices(googleDriveConfigs);
            services.AddRepositories();
        }

        public static void AddServices(this IServiceCollection services, GoogleDriveConfig googleDriveConfigs)
        {
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ICityService, CityService>();
            services.AddScoped<IPropertyService, PropertyService>();
            services.AddScoped<IResponseService, ResponseService>();
            services.AddScoped<ICommentService, CommentService>();

            services.AddSingleton(provider =>
            {
                return CreateGoogleDriveService(googleDriveConfigs);
            });
            services.AddScoped<IFileStorageService, GoogleDriveService>();
        }

        public static void AddRepositories(this IServiceCollection repositories)
        {
            repositories.AddScoped<IUnitOfWork, UnitOfWork>();
        }

        private static DriveService CreateGoogleDriveService(GoogleDriveConfig config)
        {
            ServiceAccountCredential credential;

            using (var stream = new FileStream("google_drive_credentials.json", FileMode.Open, FileAccess.Read))
                credential = new ServiceAccountCredential(new ServiceAccountCredential.Initializer(config.ClientEmail)
                {
                    Scopes = new string[] { DriveService.Scope.Drive, DriveService.Scope.DriveFile }
                }.FromPrivateKey(config.PrivateKey));

            return new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = config.ApplicationName
            });
        }
    }
}
