using Rent.Entities.Properties;
using Rent.Service.Services.Properties.Descriptors;

namespace Rent.Service.Services.Properties
{
    public interface IPropertyService
    {
        Task Add(AddPropertyDescriptor entity);
        Task Edit(EditPropertyDescriptor descriptor, string userId);
        Task Delete(int id, string userId);
    }
}
