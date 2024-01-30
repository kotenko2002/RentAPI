using Moq;
using NUnit.Framework;
using Rent.Entities.Cities;
using Rent.Service.Services.Cities;
using Rent.Service.Services.Cities.Views;
using Rent.Storage.Uow;
using Rent.Tests.Infrastructure;

namespace Rent.Tests.ServiceTests
{
    public class CityServiceTests : BaseUnitTest
    {
        #region GetAllCitiesAsync
        [Test]
        public async Task CityService_GetAllCitiesAsync_ShouldReturnAllCities()
        {
            // Arrange
            var cities = new[]
            {
                new City { Id = 1, Name = "City1" },
                new City { Id = 2, Name = "City2" }
            };

            var mockUnitOfWork = new Mock<IUnitOfWork>();
            mockUnitOfWork.Setup(uow => uow.CityRepository.FindAllAsync())
                .ReturnsAsync(cities);

            var cityService = new CityService(CreateMapperProfile(), mockUnitOfWork.Object);

            // Act
            var actual = await cityService.GetAllCitiesAsync();

            // Assert
            Assert.IsNotNull(actual);
            Assert.That(actual,
                Is.EqualTo(ExpectedCitiesViews).Using(new CityViewEqualityComparer()), message: "GetAllCitiesAsync method works incorrect");
        }

        private IEnumerable<CityView> ExpectedCitiesViews => new[]
        {
            new CityView { Id = 1, Name = "City1" },
            new CityView { Id = 2, Name = "City2" }
        };
        #endregion
    }
}
