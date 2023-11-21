using AutoMapper;
using Rent.Common;
using Rent.Entities.Properties;
using Rent.Service.Services.FileStorage;
using Rent.Service.Services.Properties.Descriptors;
using Rent.Storage.Uow;
using System.Net;

namespace Rent.Service.Services.Properties
{
    public class PropertyService : IPropertyService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _uow;
        private readonly IFileStorageService _fileStorageService;

        public PropertyService(
            IMapper mapper,
            IUnitOfWork uow,
            IFileStorageService fileStorageService)
        {
            _mapper = mapper;
            _uow = uow;
            _fileStorageService = fileStorageService;
        }

        public async Task Add(AddPropertyDescriptor descriptor)
        {
            var entity = _mapper.Map<Property>(descriptor);

            await _uow.PropertyRepository.AddAsync(entity);
            await _uow.CompleteAsync();

            await _fileStorageService.UploadNewPropertyPhotos(descriptor.Photos, entity.Id);
            //add files
        }

        public async Task Edit(EditPropertyDescriptor descriptor, string userId)
        {
            Property entity = await _uow.PropertyRepository.FindAsync(descriptor.Id);

            if(entity == null)
            {
                throw new BusinessException(HttpStatusCode.NotFound, "Property not found.");
            }

            if (entity.LandlordId != userId)
            {
                throw new BusinessException(HttpStatusCode.Forbidden,
                    "Access denied. You do not have permission to edit this property.");
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

            if (entity.LandlordId != userId)
            {
                throw new BusinessException(HttpStatusCode.Forbidden,
                    "Access denied. You do not have permission to delete this property.");
            }

            await _uow.PropertyRepository.RemoveAsync(entity);
            await _uow.CompleteAsync();
        }
    }
}
