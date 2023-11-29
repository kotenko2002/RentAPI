using Rent.Entities.Properties;
using Rent.Service.Services.Properties.Descriptors;
using Rent.Service.Services.Properties.Views;

namespace Rent.Service.Services.Properties
{
    public interface IPropertyService
    {
        Task AddAsync(AddPropertyDescriptor entity);
        Task EditAsync(EditPropertyDescriptor descriptor, string userId);
        Task<IEnumerable<PropertyView>> GetPropertiesByCityIdAsync(int cityId);
        Task<IEnumerable<PropertyView>> GetPropertiesByLandlordId(string landlordId);
        Task<PropertyDetailView> GetFullInfoByIdAsync(int propertyId);
        Task DeleteAsync(int id, string userId);
    }
}
