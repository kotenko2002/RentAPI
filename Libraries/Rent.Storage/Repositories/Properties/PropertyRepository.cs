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
    }
}
