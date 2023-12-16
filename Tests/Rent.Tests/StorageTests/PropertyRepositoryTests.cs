using NUnit.Framework;
using Rent.Entities.Properties;
using Rent.Storage.Configuration;
using Rent.Storage.Repositories.Properties;

namespace Rent.Tests.StorageTests
{
    public class PropertyRepositoryTests
    {
        #region BasicMethods
        [TestCase(1)]
        [TestCase(2)]
        public async Task PropertyRepository_FindAsync_ReturnsSingleValue(int id)
        {
            using var context = new ApplicationDbContext(UnitTestHelper.GetUnitTestDbOptions());
            var propertyRepository = new PropertyRepository(context);

            var actual = await propertyRepository.FindAsync(id);
            var expected = ExpectedProperties.FirstOrDefault(x => x.Id == id);

            Assert.That(actual,
                Is.EqualTo(expected).Using(new PropertyEqualityComparer()), message: "FindAsync method works incorrect");
        }

        [Test]
        public async Task PropertyRepository_GetAllAsync_ReturnsAllValues()
        {
            using var context = new ApplicationDbContext(UnitTestHelper.GetUnitTestDbOptions());
            var propertyRepository = new PropertyRepository(context);

            var actual = await propertyRepository.FindAllAsync();

            Assert.That(actual,
                Is.EqualTo(ExpectedProperties).Using(new PropertyEqualityComparer()), message: "GetAllAsync method works incorrect");
        }

        [Test]
        public async Task PropertyRepository_AddAsync_AddsValueToDatabase()
        {
            using var context = new ApplicationDbContext(UnitTestHelper.GetUnitTestDbOptions());
            var propertyRepository = new PropertyRepository(context);

            var property = new Property { Id = 4, LandlordId = "3", CityId = 3, Address = "Address4", Description = "Description4", Price = 4000, Status = PropertyStatus.Available };
            await propertyRepository.AddAsync(property);
            await context.SaveChangesAsync();

            Assert.That(context.Properties.Count(),
                Is.EqualTo(4), message: "AddAsync method works incorrect");
        }

        [Test]
        public async Task PropertyRepository_AddRangeAsync_AddsValueToDatabase()
        {
            using var context = new ApplicationDbContext(UnitTestHelper.GetUnitTestDbOptions());
            var propertyRepository = new PropertyRepository(context);

            var properties = new[]
            {
                new Property { Id = 4, LandlordId = "3", CityId = 3, Address = "Address4", Description = "Description4", Price = 4000, Status = PropertyStatus.Available },
                new Property { Id = 5, LandlordId = "4", CityId = 4, Address = "Address5", Description = "Description5", Price = 5000, Status = PropertyStatus.Occupied }
            };
            await propertyRepository.AddRangeAsync(properties);
            await context.SaveChangesAsync();

            Assert.That(context.Properties.Count(),
                Is.EqualTo(5), message: "AddRangeAsync method works incorrect");
        }

        [Test]
        public async Task PropertyRepository_RemoveAsync_RemovesValueFromDatabase()
        {
            using var context = new ApplicationDbContext(UnitTestHelper.GetUnitTestDbOptions());
            var propertyRepository = new PropertyRepository(context);

            var property = new Property { Id = 1, LandlordId = "1", CityId = 1, Address = "Address1", Description = "Description1", Price = 1000, Status = PropertyStatus.Available };
            await propertyRepository.RemoveAsync(property);
            await context.SaveChangesAsync();

            Assert.That(context.Properties.Count(),
                Is.EqualTo(2), message: "RemoveAsync method works incorrect");
        }

        [Test]
        public async Task PropertyRepository_RemoveRangeAsync_RemovesValuesFromDatabase()
        {
            using var context = new ApplicationDbContext(UnitTestHelper.GetUnitTestDbOptions());
            var propertyRepository = new PropertyRepository(context);

            var properties = new[]
            {
                new Property { Id = 1, LandlordId = "1", CityId = 1, Address = "Address1", Description = "Description1", Price = 1000, Status = PropertyStatus.Available },
                new Property { Id = 2, LandlordId = "1", CityId = 2, Address = "Address2", Description = "Description2", Price = 2000, Status = PropertyStatus.Occupied }
            };
            await propertyRepository.RemoveRangeAsync(properties);
            await context.SaveChangesAsync();

            Assert.That(context.Properties.Count(),
                Is.EqualTo(1), message: "RemoveRangeAsync method works incorrect");
        }
        #endregion

        private static IEnumerable<Property> ExpectedProperties =>
            new[]
            {
                new Property { Id = 1, LandlordId = "1", CityId = 1, Address = "Address1", Description = "Description1", Price = 1000, Status = PropertyStatus.Available },
                new Property { Id = 2, LandlordId = "1", CityId = 2, Address = "Address2", Description = "Description2", Price = 2000, Status = PropertyStatus.Occupied },
                new Property { Id = 3, LandlordId = "2", CityId = 2, Address = "Address3", Description = "Description3", Price = 3000, Status = PropertyStatus.Occupied }
            };
    }
}
