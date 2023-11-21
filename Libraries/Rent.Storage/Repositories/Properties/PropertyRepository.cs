using Microsoft.EntityFrameworkCore;
using Rent.Entities.Properties;
using Rent.Storage.Configuration;
using Rent.Storage.Configuration.BaseRepository;

namespace Rent.Storage.Repositories.Properties
{
    public class PropertyRepository : BaseRepository<Property>, IPropertyRepository
    {
        public PropertyRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Property> GetFullPropertyById(int propertyId)
        {
            return await Sourse
                .Include(p => p.Responses)
                    .ThenInclude(r => r.Tenant)
                .FirstOrDefaultAsync(p => p.Id == propertyId);
        }
    }
}
