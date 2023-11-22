using Rent.Entities.Properties;
using Rent.Service.Services.Properties.Descriptors;

namespace Rent.Service.Services.Properties
{
    public interface IPropertyService
    {
        Task AddAsync(AddPropertyDescriptor entity);
        Task EditAsync(EditPropertyDescriptor descriptor, string userId);
        Task DeleteAsync(int id, string userId);
    }
}
