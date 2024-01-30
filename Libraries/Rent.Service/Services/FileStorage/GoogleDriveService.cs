using Google.Apis.Drive.v3.Data;
using Google.Apis.Drive.v3;
using Microsoft.AspNetCore.Http;
using Rent.Service.Configuration;
using Microsoft.Extensions.Options;
using Rent.Common;
using System.Net;

namespace Rent.Service.Services.FileStorage
{
    public class GoogleDriveService : IFileStorageService
    {
        private readonly GoogleDriveConfig _config;
        private readonly DriveService _service;

        public GoogleDriveService(IOptions<GoogleDriveConfig> googleDriveOptions, DriveService service)
        {
            _config = googleDriveOptions.Value;
            _service = service;
        }

        public async Task<IEnumerable<string>> UploadFilesAsync(IFormFile[] files)
        {
            var fileIds = new List<string>();

            foreach (var file in files)
            {
                string fileId = await UploadFile(file);
                await SetFilePublic(fileId);

                fileIds.Add(fileId);
            }

            return fileIds;
        }
        
        public async Task DeleteFilesAsync(string[] fileIds)
        {
            foreach (var fileId in fileIds)
            {
                try
                {
                    await _service.Files.Delete(fileId).ExecuteAsync();
                }
                catch (Exception)
                {
                    throw new BusinessException(HttpStatusCode.NotFound, $"File with such Id \"{fileId}\" was not found!");
                }
            }
        }
      
        private async Task<string> UploadFile(IFormFile file)
        {
            var fileMetadata = new Google.Apis.Drive.v3.Data.File()
            {
                Name = file.FileName,
                MimeType = file.ContentType,
                Parents = new List<string> { _config.ParentFolderId }
            };

            FilesResource.CreateMediaUpload request;
            using (var stream = file.OpenReadStream())
            {
                request = _service.Files.Create(
                    fileMetadata, stream, file.ContentType);
                request.Fields = "id";
                await request.UploadAsync();
            }

            return request.ResponseBody.Id;
        }
        private async Task SetFilePublic(string fileId)
        {
            var publicPermission = new Permission()
            {
                Type = "anyone",
                Role = "reader"
            };
            var permissionRequest = _service.Permissions.Create(publicPermission, fileId);

            await permissionRequest.ExecuteAsync();
        }
    }
}
