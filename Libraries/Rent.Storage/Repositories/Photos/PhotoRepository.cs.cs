using Rent.Storage.Configuration.BaseRepository;
using Rent.Storage.Configuration;
using Rent.Entities.Photos;

namespace Rent.Storage.Repositories.Photos
{
    public class PhotoRepository : BaseRepository<Photo>, IPhotoRepository
    {
        public PhotoRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
