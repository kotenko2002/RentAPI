using NUnit.Framework;
using Rent.Entities.Cities;
using Rent.Storage.Configuration;
using Rent.Storage.Repositories.Cities;
using Rent.Tests.Helpers;

namespace Rent.Tests.StorageTests
{
    public class CityRepositoryTests
    {
        #region BasicMethods
        [TestCase(1)]
        [TestCase(2)]
        public async Task CityRepository_FindAsync_ReturnsSingleValue(int id)
        {
            using var context = new ApplicationDbContext(UnitTestHelper.GetUnitTestDbOptions());
            var cityRepository = new CityRepository(context);

            var actual = await cityRepository.FindAsync(id);
            var expected = ExpectedCities.FirstOrDefault(x => x.Id == id);

            Assert.That(actual,
                Is.EqualTo(expected).Using(new CityEqualityComparer()), message: "FindAsync method works incorrect");
        }

        [Test]
        public async Task CityRepository_GetAllAsync_ReturnsAllValues()
        {
            using var context = new ApplicationDbContext(UnitTestHelper.GetUnitTestDbOptions());
            var cityRepository = new CityRepository(context);

            var actual = await cityRepository.FindAllAsync();

            Assert.That(actual,
                Is.EqualTo(ExpectedCities).Using(new CityEqualityComparer()), message: "GetAllAsync method works incorrect");
        }

        [Test]
        public async Task CityRepository_AddAsync_AddsValueToDatabase()
        {
            using var context = new ApplicationDbContext(UnitTestHelper.GetUnitTestDbOptions());
            var cityRepository = new CityRepository(context);

            var city = new City { Id = 3, Name = "City3" };
            await cityRepository.AddAsync(city);
            await context.SaveChangesAsync();

            Assert.That(context.Cities.Count(),
                Is.EqualTo(3), message: "AddAsync method works incorrect");
        }

        [Test]
        public async Task CityRepository_AddRangeAsync_AddsValueToDatabase()
        {
            using var context = new ApplicationDbContext(UnitTestHelper.GetUnitTestDbOptions());
            var cityRepository = new CityRepository(context);

            var cities = new[]
            {
                new City { Id = 3, Name = "City3" },
                new City { Id = 4, Name = "City4" }
            };
            await cityRepository.AddRangeAsync(cities);
            await context.SaveChangesAsync();

            Assert.That(context.Cities.Count(),
                Is.EqualTo(4), message: "AddRangeAsync method works incorrect");
        }

        [Test]
        public async Task CityRepository_RemoveAsync_RemovesValueFromDatabase()
        {
            using var context = new ApplicationDbContext(UnitTestHelper.GetUnitTestDbOptions());
            var cityRepository = new CityRepository(context);

            var city = new City { Id = 1, Name = "City1" };
            await cityRepository.RemoveAsync(city);
            await context.SaveChangesAsync();

            Assert.That(context.Cities.Count(),
                Is.EqualTo(1), message: "RemoveAsync method works incorrect");
        }

        [Test]
        public async Task CityRepository_RemoveRangeAsync_RemovesValuesFromDatabase()
        {
            using var context = new ApplicationDbContext(UnitTestHelper.GetUnitTestDbOptions());
            var cityRepository = new CityRepository(context);

            var cities = new[]
            {
                new City { Id = 1, Name = "City1" },
                new City { Id = 2, Name = "City2" }
            };
            await cityRepository.RemoveRangeAsync(cities);
            await context.SaveChangesAsync();

            Assert.That(context.Cities.Count(),
                Is.EqualTo(0), message: "RemoveRangeAsync method works incorrect");
        }
        #endregion

        private static IEnumerable<City> ExpectedCities =>
            new[]
            {
                new City { Id = 1, Name = "City1" },
                new City { Id = 2, Name = "City2" }
            };
    }
}
