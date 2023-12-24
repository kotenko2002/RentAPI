using Microsoft.AspNetCore.Http;
using Moq;
using NUnit.Framework;
using Rent.Common;
using Rent.Entities.Cities;
using Rent.Entities.Properties;
using Rent.Service.Services.FileStorage;
using Rent.Service.Services.Properties.Descriptors;
using Rent.Service.Services.Properties;
using Rent.Storage.Uow;
using Rent.Tests.Infrastructure;
using Rent.Entities.Photos;
using Rent.Service.Services.Properties.Views;

namespace Rent.Tests.ServiceTests
{
    public class PropertyServiceTests : BaseUnitTest
    {
        #region AddNewProperty
        [Test]
        public async Task PropertyService_AddAsync_ShouldAddProperty()
        {
            // Arrange
            var mockFile1 = new Mock<IFormFile>();
            var mockFile2 = new Mock<IFormFile>();
            var descriptor = new AddPropertyDescriptor
            {
                LandlordId = "1",
                CityId = 1,
                Address = "Address1",
                Description = "Description1",
                Price = 1000,
                Status = PropertyStatus.Available,
                Photos = new IFormFile[] { mockFile1.Object, mockFile2.Object }
            };
            var photoIds = new string[] { "1", "2" };
            var city = new City { Id = 1, Name = "City1" };
            var property = new Property
            {
                Id = 1,
                LandlordId = "1",
                CityId = 1,
                Address = "Address1",
                Description = "Description1",
                Price = 1000,
                Status = PropertyStatus.Available
            };

            var mockUnitOfWork = new Mock<IUnitOfWork>();
            mockUnitOfWork.Setup(uow => uow.CityRepository.FindAsync(descriptor.CityId)).ReturnsAsync(city);
            mockUnitOfWork.Setup(uow => uow.PropertyRepository.AddAsync(It.IsAny<Property>())).Returns(Task.CompletedTask);
            mockUnitOfWork.Setup(uow => uow.PhotoRepository.AddRangeAsync(It.IsAny<Photo[]>())).Returns(Task.CompletedTask);
            mockUnitOfWork.Setup(uow => uow.CompleteAsync()).Returns(Task.CompletedTask);

            var mockFileStorageService = new Mock<IFileStorageService>();
            mockFileStorageService.Setup(service => service.UploadFilesAsync(descriptor.Photos)).ReturnsAsync(photoIds);

            var propertyService = new PropertyService(CreateMapperProfile(), mockUnitOfWork.Object, mockFileStorageService.Object, null);

            // Act
            await propertyService.AddAsync(descriptor);

            // Assert
            mockUnitOfWork.Verify(uow => uow.CityRepository.FindAsync(descriptor.CityId), Times.Once);
            mockUnitOfWork.Verify(uow => uow.PropertyRepository.AddAsync(It.IsAny<Property>()), Times.Once);
            mockFileStorageService.Verify(service => service.UploadFilesAsync(descriptor.Photos), Times.Once);
            mockUnitOfWork.Verify(uow => uow.CompleteAsync(), Times.Exactly(2));
        }

        [Test]
        public void PropertyService_AddAsync_ShouldThrowBusinessException_WhenCityNotFound()
        {
            // Arrange
            var descriptor = new AddPropertyDescriptor
            {
                LandlordId = "1",
                CityId = 1,
                Address = "Address1",
                Description = "Description1",
                Price = 1000,
                Status = PropertyStatus.Available,
                Photos = new IFormFile[0]
            };

            var mockUnitOfWork = new Mock<IUnitOfWork>();
            mockUnitOfWork.Setup(uow => uow.CityRepository.FindAsync(descriptor.CityId)).ReturnsAsync((City)null);

            var propertyService = new PropertyService(null, mockUnitOfWork.Object, null, null);

            // Act & Assert
            Assert.ThrowsAsync<BusinessException>(() => propertyService.AddAsync(descriptor));
        }
        #endregion

        #region EditProperty
        [Test]
        public async Task PropertyService_EditAsync_ShouldEditProperty()
        {
            // Arrange
            var mockFile1 = new Mock<IFormFile>();
            var mockFile2 = new Mock<IFormFile>();
            var descriptor = new EditPropertyDescriptor
            {
                Id = 1,
                CityId = 2,
                Address = "New Address",
                Description = "New Description",
                Price = 2000,
                Status = PropertyStatus.Occupied,
                Photos = new IFormFile[] { mockFile1.Object, mockFile2.Object },
                PhotoIdsToDelete = new string[0]
            };
            var property = new Property
            {
                Id = 1,
                LandlordId = "1",
                CityId = 1,
                Address = "Address1",
                Description = "Description1",
                Price = 1000,
                Status = PropertyStatus.Available
            };
            var photoIds = new string[] { "1", "2" };

            var mockUnitOfWork = new Mock<IUnitOfWork>();
            mockUnitOfWork.Setup(uow => uow.PropertyRepository.FindAsync(descriptor.Id)).ReturnsAsync(property);
            mockUnitOfWork.Setup(uow => uow.CompleteAsync()).Returns(Task.CompletedTask);
            mockUnitOfWork.Setup(uow => uow.PhotoRepository.AddRangeAsync(It.IsAny<Photo[]>())).Returns(Task.CompletedTask);

            var mockFileStorageService = new Mock<IFileStorageService>();
            mockFileStorageService.Setup(service => service.UploadFilesAsync(descriptor.Photos)).ReturnsAsync(photoIds);

            var propertyService = new PropertyService(CreateMapperProfile(), mockUnitOfWork.Object, mockFileStorageService.Object, null);

            // Act
            await propertyService.EditAsync(descriptor, "1");

            // Assert
            mockUnitOfWork.Verify(uow => uow.PropertyRepository.FindAsync(descriptor.Id), Times.Once);
            mockFileStorageService.Verify(service => service.UploadFilesAsync(descriptor.Photos), Times.Once);
            mockUnitOfWork.Verify(uow => uow.CompleteAsync(), Times.Once);
        }

        [Test]
        public void PropertyService_EditAsync_ShouldThrowBusinessException_WhenPropertyNotFound()
        {
            // Arrange
            var descriptor = new EditPropertyDescriptor { Id = 1 };

            var mockUnitOfWork = new Mock<IUnitOfWork>();
            mockUnitOfWork.Setup(uow => uow.PropertyRepository.FindAsync(descriptor.Id)).ReturnsAsync((Property)null);

            var propertyService = new PropertyService(CreateMapperProfile(), mockUnitOfWork.Object, null, null);

            // Act & Assert
            Assert.ThrowsAsync<BusinessException>(() => propertyService.EditAsync(descriptor, "1"));
        }

        [Test]
        public void PropertyService_EditAsync_ShouldThrowBusinessException_WhenUserIsNotTheLandlord()
        {
            // Arrange
            var descriptor = new EditPropertyDescriptor { Id = 1 };
            var property = new Property { Id = 1, LandlordId = "2" };

            var mockUnitOfWork = new Mock<IUnitOfWork>();
            mockUnitOfWork.Setup(uow => uow.PropertyRepository.FindAsync(descriptor.Id)).ReturnsAsync(property);

            var propertyService = new PropertyService(CreateMapperProfile(), mockUnitOfWork.Object, null, null);

            // Act & Assert
            Assert.ThrowsAsync<BusinessException>(() => propertyService.EditAsync(descriptor, "1"));
        }
        #endregion

        #region GetPropertiesByCityId
        [Test]
        public async Task PropertyService_GetPropertiesByCityIdAsync_ShouldReturnProperties()
        {
            // Arrange
            var cityId = 1;
            var city = new City { Id = cityId, Name = "City1" };
            var properties = new List<Property>
            {
                new Property { Id = 1, CityId = cityId, Address = "Address1", Price = 1000, Photos = new List<Photo>() },
                new Property { Id = 2, CityId = cityId, Address = "Address2", Price = 2000, Photos = new List<Photo>() }
            };

            var mockUnitOfWork = new Mock<IUnitOfWork>();
            mockUnitOfWork.Setup(uow => uow.CityRepository.FindAsync(cityId)).ReturnsAsync(city);
            mockUnitOfWork.Setup(uow => uow.PropertyRepository.GetPropertiesByCityIdAsync(cityId)).ReturnsAsync(properties);

            var propertyService = new PropertyService(CreateMapperProfile(), mockUnitOfWork.Object, null, null);

            // Act
            IEnumerable<PropertyView> result = await propertyService.GetPropertiesByCityIdAsync(cityId);

            // Assert
            Assert.That(result.Count(), Is.EqualTo(properties.Count));
            mockUnitOfWork.Verify(uow => uow.CityRepository.FindAsync(cityId), Times.Once);
            mockUnitOfWork.Verify(uow => uow.PropertyRepository.GetPropertiesByCityIdAsync(cityId), Times.Once);
        }

        [Test]
        public void PropertyService_GetPropertiesByCityIdAsync_ShouldThrowBusinessException_WhenCityNotFound()
        {
            // Arrange
            var cityId = 1;

            var mockUnitOfWork = new Mock<IUnitOfWork>();
            mockUnitOfWork.Setup(uow => uow.CityRepository.FindAsync(cityId)).ReturnsAsync((City)null);

            var propertyService = new PropertyService(CreateMapperProfile(), mockUnitOfWork.Object, null, null);

            // Act & Assert
            Assert.ThrowsAsync<BusinessException>(() => propertyService.GetPropertiesByCityIdAsync(cityId));
        }
        #endregion

        #region GetPropertyFullInfoById
        [Test]
        public async Task PropertyService_GetFullInfoByIdAsync_ShouldReturnPropertyDetail()
        {
            // Arrange
            var propertyId = 1;
            var property = new Property
            {
                Id = 1,
                LandlordId = "1",
                CityId = 1,
                Address = "Address1",
                Description = "Description1",
                Price = 1000,
                Status = PropertyStatus.Available,
                Photos = new List<Photo>(),
                City = new City { Id = 1, Name = "City1"}
            };

            var mockUnitOfWork = new Mock<IUnitOfWork>();
            mockUnitOfWork.Setup(uow => uow.PropertyRepository.GetFullPropertyByIdAsync(propertyId)).ReturnsAsync(property);

            var propertyService = new PropertyService(null, mockUnitOfWork.Object, null, null);

            // Act
            var result = await propertyService.GetFullInfoByIdAsync(propertyId);

            // Assert
            Assert.That(result.Id, Is.EqualTo(property.Id));
        }

        [Test]
        public void PropertyService_GetFullInfoByIdAsync_ShouldThrowBusinessException_WhenPropertyNotFound()
        {
            // Arrange
            var propertyId = 1;

            var mockUnitOfWork = new Mock<IUnitOfWork>();
            mockUnitOfWork.Setup(uow => uow.PropertyRepository.GetFullPropertyByIdAsync(propertyId)).ReturnsAsync((Property)null);

            var propertyService = new PropertyService(null, mockUnitOfWork.Object, null, null);

            // Act & Assert
            Assert.ThrowsAsync<BusinessException>(() => propertyService.GetFullInfoByIdAsync(propertyId));
        }
        #endregion

        #region DeleteProperty
        [Test]
        public async Task PropertyService_DeleteAsync_ShouldDeleteProperty()
        {
            // Arrange
            var propertyId = 1;
            var userId = "1";
            var property = new Property
            {
                Id = propertyId,
                LandlordId = userId,
                Photos = new List<Photo>
                {
                    new Photo { Id = "1" },
                    new Photo { Id = "2" }
                }
            };

            var mockUnitOfWork = new Mock<IUnitOfWork>();
            mockUnitOfWork.Setup(uow => uow.PropertyRepository.GetFullPropertyByIdAsync(propertyId)).ReturnsAsync(property);
            mockUnitOfWork.Setup(uow => uow.PropertyRepository.RemoveAsync(property)).Returns(Task.CompletedTask);
            mockUnitOfWork.Setup(uow => uow.CompleteAsync()).Returns(Task.CompletedTask);

            var mockFileStorageService = new Mock<IFileStorageService>();
            mockFileStorageService.Setup(service => service.DeleteFilesAsync(It.IsAny<string[]>())).Returns(Task.CompletedTask);

            var propertyService = new PropertyService(null, mockUnitOfWork.Object, mockFileStorageService.Object, null);

            // Act
            await propertyService.DeleteAsync(propertyId, userId);

            // Assert
            mockUnitOfWork.Verify(uow => uow.PropertyRepository.GetFullPropertyByIdAsync(propertyId), Times.Once);
            mockUnitOfWork.Verify(uow => uow.PropertyRepository.RemoveAsync(property), Times.Once);
            mockFileStorageService.Verify(service => service.DeleteFilesAsync(It.IsAny<string[]>()), Times.Once);
            mockUnitOfWork.Verify(uow => uow.CompleteAsync(), Times.Once);
        }

        [Test]
        public void PropertyService_DeleteAsync_ShouldThrowBusinessException_WhenPropertyNotFound()
        {
            // Arrange
            var propertyId = 1;
            var userId = "1";

            var mockUnitOfWork = new Mock<IUnitOfWork>();
            mockUnitOfWork.Setup(uow => uow.PropertyRepository.GetFullPropertyByIdAsync(propertyId)).ReturnsAsync((Property)null);

            var propertyService = new PropertyService(null, mockUnitOfWork.Object, null, null);

            // Act & Assert
            Assert.ThrowsAsync<BusinessException>(() => propertyService.DeleteAsync(propertyId, userId));
        }

        [Test]
        public void PropertyService_DeleteAsync_ShouldThrowBusinessException_WhenUserIsNotLandlord()
        {
            // Arrange
            var propertyId = 1;
            var userId = "1";

            var property = new Property { Id = propertyId, LandlordId = "2" };

            var mockUnitOfWork = new Mock<IUnitOfWork>();
            mockUnitOfWork.Setup(uow => uow.PropertyRepository.GetFullPropertyByIdAsync(propertyId)).ReturnsAsync(property);

            var propertyService = new PropertyService(null, mockUnitOfWork.Object, null, null);

            // Act & Assert
            Assert.ThrowsAsync<BusinessException>(() => propertyService.DeleteAsync(propertyId, userId));
        }
        #endregion
    }
}
