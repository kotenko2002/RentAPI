using Microsoft.AspNetCore.Http;

namespace Rent.Service.Services.FileStorage
{
    public interface IFileStorageService
    {
        Task UploadNewPropertyPhotos(IFormFile[] Photos, int propertyId);
    }
}
