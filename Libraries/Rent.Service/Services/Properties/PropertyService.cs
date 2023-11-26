using AutoMapper;
using Rent.Common;
using Rent.Entities.Cities;
using Rent.Entities.Photos;
using Rent.Entities.Properties;
using Rent.Service.Services.FileStorage;
using Rent.Service.Services.Properties.Descriptors;
using Rent.Service.Services.Properties.Views;
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

        public async Task AddAsync(AddPropertyDescriptor descriptor)
        {
            City city = await _uow.CityRepository.FindAsync(descriptor.CityId);
            if(city == null)
            {
                throw new BusinessException(HttpStatusCode.NotFound, "City not found.");
            }

            var property = _mapper.Map<Property>(descriptor);
            property.Status = PropertyStatus.Available;

            await _uow.PropertyRepository.AddAsync(property);
            await _uow.CompleteAsync();

            IEnumerable<string> fileIds = await _fileStorageService.UploadFilesAsync(descriptor.Photos);

            var files = fileIds.Select(fileId => new Photo()
            {
                Id = fileId,
                PropertyId = property.Id
            });
            await _uow.PhotoRepository.AddRangeAsync(files);
            await _uow.CompleteAsync();
        }

        public async Task EditAsync(EditPropertyDescriptor descriptor, string userId)
        {
            Property property = await _uow.PropertyRepository.FindAsync(descriptor.Id);

            if(property == null)
            {
                throw new BusinessException(HttpStatusCode.NotFound, "Property not found.");
            }

            if (property.LandlordId != userId)
            {
                throw new BusinessException(HttpStatusCode.Forbidden,
                    "Access denied. You do not have permission to edit this property.");
            }

            if (descriptor.CityId.HasValue)
                property.CityId = descriptor.CityId;

            if (!string.IsNullOrEmpty(descriptor.Address))
                property.Address = descriptor.Address;

            if (!string.IsNullOrEmpty(descriptor.Description))
                property.Description = descriptor.Description;

            if (descriptor.Price.HasValue)
                property.Price = (int)descriptor.Price;

            if (descriptor.Status.HasValue)
                property.Status = (PropertyStatus)descriptor.Status;

            if (descriptor.PhotoIdsToDelete.Any())
            {
                IEnumerable<Photo> photos = await _uow.PhotoRepository.GetPhotosByIds(descriptor.PhotoIdsToDelete);
                if(descriptor.PhotoIdsToDelete.Length != photos.Count())
                {
                    throw new BusinessException(HttpStatusCode.NotFound, "Photo not found.");
                }

                await _uow.PhotoRepository.RemoveRangeAsync(photos);

                await _fileStorageService.DeleteFilesAsync(descriptor.PhotoIdsToDelete);
            }

            if (descriptor.Photos.Any())
            {
                IEnumerable<string> fileIds = await _fileStorageService.UploadFilesAsync(descriptor.Photos);

                var files = fileIds.Select(fileId => new Photo()
                {
                    Id = fileId,
                    PropertyId = property.Id
                });
                await _uow.PhotoRepository.AddRangeAsync(files);
            }

            await _uow.CompleteAsync();
        }

        public async Task<IEnumerable<PropertyView>> GetPropertiesByCityIdAsync(int cityId)
        {
            IEnumerable<Property> properties = await _uow.PropertyRepository.GetPropertiesByCityIdAsync(cityId);

            return _mapper.Map<IEnumerable<PropertyView>>(properties);
        }

        public async Task<IEnumerable<PropertyView>> GetPropertiesByLandlordId(string landlordId)
        {
            IEnumerable<Property> properties = await _uow.PropertyRepository.GetPropertiesByLandlordIdAsync(landlordId);

            return _mapper.Map<IEnumerable<PropertyView>>(properties);
        }

        public Task<PropertyDetailView> GetFullInfoByIdAsync(int propertyId)
        {
            throw new NotImplementedException();
        }

        public async Task DeleteAsync(int id, string userId)
        {
            Property property = await _uow.PropertyRepository.GetFullPropertyByIdAsync(id);

            if (property == null)
            {
                throw new BusinessException(HttpStatusCode.NotFound, "Property not found.");
            }

            if (property.LandlordId != userId)
            {
                throw new BusinessException(HttpStatusCode.Forbidden,
                    "Access denied. You do not have permission to delete this property.");
            }

            var fileIdsToDelete = property.Photos.Select(p => p.Id).ToArray();

            await _uow.PropertyRepository.RemoveAsync(property);
            await _uow.CompleteAsync();

            await _fileStorageService.DeleteFilesAsync(fileIdsToDelete);
        }
    }
}
