using Rent.Common;
using System.Net;

namespace RentAPI.Infrastructure.Extensions
{
    public static class FileExtensions
    {
        public static bool IsPhoto(this IFormFile file)
        {
            if (file == null)
            {
                throw new BusinessException(HttpStatusCode.BadRequest, $"File is required.");
            }

            var supportedTypes = new HashSet<string> { "image/jpeg", "image/png", "image/gif", "image/webp" };
            var contentType = file.ContentType.ToLower();

            return supportedTypes.Contains(contentType);
        }
    }
}
