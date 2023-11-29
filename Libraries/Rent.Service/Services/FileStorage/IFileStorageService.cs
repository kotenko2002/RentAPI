using Microsoft.AspNetCore.Http;

namespace Rent.Service.Services.FileStorage
{
    public interface IFileStorageService
    {
        Task<IEnumerable<string>> UploadFilesAsync(IFormFile[] files);
        Task DeleteFilesAsync(string[] fileIds);
    }
}
