using Microsoft.AspNetCore.Http;

namespace Rent.Service.Services.FileStorage
{
    public class ServerStorage : IFileStorageService
    {
        private readonly string basePath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");

        public async Task UploadNewPropertyPhotosAsync(IFormFile[] photos, int propertyId)
        {
            string propertyDirectoryPath = Path.Combine(basePath, propertyId.ToString());

            Directory.CreateDirectory(propertyDirectoryPath);

            foreach (var photo in photos)
            {
                var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(photo.FileName);
                var filePath = Path.Combine(propertyDirectoryPath, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await photo.CopyToAsync(fileStream);
                }
            }
        }
    }
}
