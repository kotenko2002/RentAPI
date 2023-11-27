using Rent.Storage.Configuration.BaseRepository;
using Rent.Storage.Configuration;
using Rent.Entities.Photos;
using Microsoft.EntityFrameworkCore;

namespace Rent.Storage.Repositories.Photos
{
    public class PhotoRepository : BaseRepository<Photo>, IPhotoRepository
    {
        public PhotoRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Photo>> GetPhotosByIds(string[] ids)
        {
            return await Sourse
                .Where(p => ids.Contains(p.Id))
                .ToListAsync();
        }

        public async Task<IEnumerable<Photo>> GetFirstPhotoByPropertyIds(int[] propertyIds)
        {
            return await Sourse
                .Where(p => propertyIds.Contains(p.PropertyId))
                .GroupBy(p => p.PropertyId)
                .Select(g => g.FirstOrDefault())
                .ToListAsync();
        }
    }
}
