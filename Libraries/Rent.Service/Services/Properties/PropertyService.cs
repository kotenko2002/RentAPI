using Rent.Entities.Properties;
using Rent.Service.Services.Properties.Descriptors;
using Rent.Storage.Uow;

namespace Rent.Service.Services.Properties
{
    public class PropertyService : IPropertyService
    {
        private readonly IUnitOfWork _uow;

        public PropertyService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task Add(Property entity)
        {
            await _uow.PropertyRepository.AddAsync(entity);
            await _uow.CompleteAsync();
        }

        public async Task Edit(EditPropertyDescriptor descriptor, string userId)
        {
            Property entity = await _uow.PropertyRepository.FindAsync(descriptor.Id);

            if(entity == null)
            {
                //error
            }

            if (entity.UserId != userId)
            {
                //error
            }

            if (descriptor.CityId.HasValue)
                entity.CityId = descriptor.CityId;

            if (!string.IsNullOrEmpty(descriptor.Address))
                entity.Address = descriptor.Address;

            if (!string.IsNullOrEmpty(descriptor.Description))
                entity.Description = descriptor.Description;

            if (descriptor.Price.HasValue)
                entity.Price = (int)descriptor.Price;

            if (descriptor.Status.HasValue)
                entity.Status = (PropertyStatus)descriptor.Status;

            await _uow.CompleteAsync();
        }

        public async Task Delete(int id, string userId)
        {
            Property entity = await _uow.PropertyRepository.FindAsync(id);

            if (entity.UserId != userId)
            {
                //error
            }

            await _uow.PropertyRepository.RemoveAsync(entity);
            await _uow.CompleteAsync();
        }
    }
}
