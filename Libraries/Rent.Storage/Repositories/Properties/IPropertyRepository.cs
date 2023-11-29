using Rent.Entities.Properties;
using Rent.Storage.Configuration.BaseRepository;

namespace Rent.Storage.Repositories.Properties
{
    public interface IPropertyRepository : IBaseRepository<Property>
    {
        Task<Property> GetFullPropertyByIdAsync(int propertyId);
        Task<IEnumerable<Property>> GetPropertiesByCityIdAsync(int cityId);
        Task<IEnumerable<Property>> GetPropertiesByLandlordIdAsync(string landlordId);
    }
}
