using Microsoft.AspNetCore.Http;

namespace Rent.Service.Services.FileStorage
{
    public interface IFileStorageService
    {
        Task UploadNewPropertyPhotosAsync(IFormFile[] Photos, int propertyId);
    }
}
