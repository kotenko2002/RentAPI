using Microsoft.EntityFrameworkCore;
using Rent.Entities.Properties;
using Rent.Storage.Configuration;
using Rent.Storage.Configuration.BaseRepository;
using System.Diagnostics;
using System.Net;

namespace Rent.Storage.Repositories.Properties
{
    public class PropertyRepository : BaseRepository<Property>, IPropertyRepository
    {
        public PropertyRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Property> GetFullPropertyByIdAsync(int propertyId)
        {
            return await Sourse
                .Include(p => p.Responses)
                    .ThenInclude(r => r.Tenant)
                .Include(p => p.Photos)
                .Include(p => p.City)
                .FirstOrDefaultAsync(p => p.Id == propertyId);
        }

        public async Task<IEnumerable<Property>> GetPropertiesByCityIdAsync(int cityId)
        {
            return await Sourse
                .Where(p => p.CityId == cityId)
                .Include(p => p.Photos)
                .ToListAsync();
        }

        public async Task<IEnumerable<Property>> GetPropertiesByLandlordIdAsync(string landlordId)
        {
            return await Sourse
                .Where(p => p.LandlordId == landlordId)
                .Include(p => p.City)
                .Include(p => p.Photos)
                .ToListAsync();
        }
    }
}
