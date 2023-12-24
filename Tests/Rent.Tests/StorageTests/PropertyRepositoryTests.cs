using NUnit.Framework;
using Rent.Entities.Cities;
using Rent.Entities.Comments;
using Rent.Entities.Photos;
using Rent.Entities.Properties;
using Rent.Entities.Responses;
using Rent.Entities.Users;
using Rent.Storage.Configuration;
using Rent.Storage.Repositories.Properties;
using Rent.Tests.Infrastructure;

namespace Rent.Tests.StorageTests
{
    public class PropertyRepositoryTests : BaseUnitTest
    {
        #region BasicMethods
        [TestCase(1)]
        [TestCase(2)]
        public async Task PropertyRepository_FindAsync_ShouldReturnSingleValue(int id)
        {
            using var context = new ApplicationDbContext(GetUnitTestDbOptions());
            var propertyRepository = new PropertyRepository(context);

            var actual = await propertyRepository.FindAsync(id);
            var expected = ExpectedProperties.FirstOrDefault(x => x.Id == id);

            Assert.That(actual,
                Is.EqualTo(expected).Using(new PropertyEqualityComparer()), message: "FindAsync method works incorrect");
        }

        [Test]
        public async Task PropertyRepository_GetAllAsync_ShouldReturnAllValues()
        {
            using var context = new ApplicationDbContext(GetUnitTestDbOptions());
            var propertyRepository = new PropertyRepository(context);

            var actual = await propertyRepository.FindAllAsync();

            Assert.That(actual,
                Is.EqualTo(ExpectedProperties).Using(new PropertyEqualityComparer()), message: "GetAllAsync method works incorrect");
        }

        [Test]
        public async Task PropertyRepository_AddAsync_ShouldAddValueToDatabase()
        {
            using var context = new ApplicationDbContext(GetUnitTestDbOptions());
            var propertyRepository = new PropertyRepository(context);

            var property = new Property { Id = 4, LandlordId = "3", CityId = 3, Address = "Address4", Description = "Description4", Price = 4000, Status = PropertyStatus.Available };
            await propertyRepository.AddAsync(property);
            await context.SaveChangesAsync();

            Assert.That(context.Properties.Count(),
                Is.EqualTo(4), message: "AddAsync method works incorrect");
        }

        [Test]
        public async Task PropertyRepository_AddRangeAsync_ShouldAddValueToDatabase()
        {
            using var context = new ApplicationDbContext(GetUnitTestDbOptions());
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
        public async Task PropertyRepository_RemoveAsync_ShouldRemoveValueFromDatabase()
        {
            using var context = new ApplicationDbContext(GetUnitTestDbOptions());
            var propertyRepository = new PropertyRepository(context);

            var property = new Property { Id = 1, LandlordId = "1", CityId = 1, Address = "Address1", Description = "Description1", Price = 1000, Status = PropertyStatus.Available };
            await propertyRepository.RemoveAsync(property);
            await context.SaveChangesAsync();

            Assert.That(context.Properties.Count(),
                Is.EqualTo(2), message: "RemoveAsync method works incorrect");
        }

        [Test]
        public async Task PropertyRepository_RemoveRangeAsync_ShouldRemoveValuesFromDatabase()
        {
            using var context = new ApplicationDbContext(GetUnitTestDbOptions());
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

        [Test]
        public async Task PropertyRepository_GetFullPropertyByIdAsync_ShouldReturnExpectedProperty()
        {
            using var context = new ApplicationDbContext(GetUnitTestDbOptions());
            var propertyRepository = new PropertyRepository(context);

            var expected = ExpectedProperties.FirstOrDefault(x => x.Id == 1);
            var actual = await propertyRepository.GetFullPropertyByIdAsync(1);

            Assert.That(actual,
                Is.EqualTo(expected).Using(new PropertyEqualityComparer()), message: "GetFullPropertyByIdAsync method works incorrect");
            Assert.That(actual.Responses.Select(r => r.Tenant),
                Is.EqualTo(expected.Responses.Select(r => r.Tenant)).Using(new UserEqualityComparer()), message: "GetFullPropertyByIdAsync method doesn't return included entities");
            Assert.That(actual.Photos,
                Is.EqualTo(expected.Photos).Using(new PhotoEqualityComparer()), message: "GetFullPropertyByIdAsync method doesn't return included entities");
            Assert.That(actual.City,
                Is.EqualTo(expected.City).Using(new CityEqualityComparer()), message: "GetFullPropertyByIdAsync method doesn't return included entities");
        }

        [TestCase(1)]
        [TestCase(2)]
        public async Task PropertyRepository_GetPropertiesByCityIdAsync_ShouldReturnExpectedProperties(int cityId)
        {
            using var context = new ApplicationDbContext(GetUnitTestDbOptions());
            var propertyRepository = new PropertyRepository(context);

            var expected = ExpectedProperties.Where(x => x.CityId == cityId).ToList();
            var actual = await propertyRepository.GetPropertiesByCityIdAsync(cityId);

            Assert.That(actual,
                Is.EqualTo(expected).Using(new PropertyEqualityComparer()), message: "GetPropertiesByCityIdAsync method works incorrect");
            Assert.That(actual.SelectMany(p => p.Photos),
                Is.EqualTo(expected.SelectMany(p => p.Photos)).Using(new PhotoEqualityComparer()), message: "GetPropertiesByCityIdAsync method doesn't return included entities");
        }

        [TestCase("1")]
        [TestCase("2")]
        public async Task PropertyRepository_GetPropertiesByLandlordIdAsync_ShouldReturnExpectedProperties(string landlordId)
        {
            using var context = new ApplicationDbContext(GetUnitTestDbOptions());
            var propertyRepository = new PropertyRepository(context);

            var expected = ExpectedProperties.Where(x => x.LandlordId == landlordId);
            var actual = await propertyRepository.GetPropertiesByLandlordIdAsync(landlordId);

            Assert.That(actual,
                Is.EqualTo(expected).Using(new PropertyEqualityComparer()), message: "GetPropertiesByLandlordIdAsync method works incorrect");
            Assert.That(actual.Select(p => p.City),
                Is.EqualTo(expected.Select(p => p.City)).Using(new CityEqualityComparer()), message: "GetPropertiesByLandlordIdAsync method doesn't return included entities");
        }

        private static IEnumerable<Property> ExpectedProperties =>
            new[]
            {
                new Property {
                    Id = 1,
                    LandlordId = "1",
                    CityId = 1,
                    Address = "Address1",
                    Description = "Description1",
                    Price = 1000,
                    Status = PropertyStatus.Available,
                    City = new City { Id = 1, Name = "City1" },
                    Photos = new List<Photo> {
                        new Photo { Id = "1", PropertyId = 1 },
                        new Photo { Id = "2", PropertyId = 1 }
                    },
                    Responses = new List<Response> {
                        new Response { Id = 1,
                            TenantId = "3",
                            PropertyId = 1,
                            Message = "Message1",
                            Status = ResponseStatus.ApprovedToRent,
                            Tenant = new User { Id = "3", UserName = "Tenant1" }
                        },
                        new Response { Id = 4,
                            TenantId = "4",
                            PropertyId = 1,
                            Message = "Message4",
                            Status = ResponseStatus.NotReviewed,
                            Tenant = new User { Id = "4", UserName = "Tenant2" }
                        }
                    },
                    Comments = new List<Comment> {
                        new Comment { Id = 1, TenantId = "3", PropertyId = 1, Message = "Message1", Rate = Rate.Average }
                    }
                },
                new Property {
                    Id = 2,
                    LandlordId = "1",
                    CityId = 2,
                    Address = "Address2",
                    Description = "Description2",
                    Price = 2000,
                    Status = PropertyStatus.Occupied,
                    City = new City { Id = 2, Name = "City2" },
                    Photos = new List<Photo> {
                        new Photo { Id = "3", PropertyId = 2 },
                        new Photo { Id = "4", PropertyId = 2 }
                    },
                    Responses = new List<Response> {
                        new Response {
                            Id = 2,
                            TenantId = "3",
                            PropertyId = 2,
                            Message = "Message2",
                            Status = ResponseStatus.ApprovedToDialog,
                            Tenant = new User { Id = "3", UserName = "Tenant1" }
                        }
                    },
                    Comments = new List<Comment> { }
                },
                new Property {
                    Id = 3,
                    LandlordId = "2",
                    CityId = 2,
                    Address = "Address3",
                    Description = "Description3",
                    Price = 3000,
                    Status = PropertyStatus.Occupied,
                    City = new City { Id = 2, Name = "City2" },
                    Photos = new List<Photo> {
                        new Photo { Id = "5", PropertyId = 3 }
                    },
                    Responses = new List<Response> {
                        new Response {
                            Id = 3,
                            TenantId = "3",
                            PropertyId = 3,
                            Message = "Message3",
                            Status = ResponseStatus.ApprovedToRent,
                            Tenant = new User { Id = "3", UserName = "Tenant1" }
                        }
                    },
                    Comments = new List<Comment> {
                        new Comment { Id = 2, TenantId = "3", PropertyId = 3, Message = "Message2", Rate = Rate.Average }
                    }
                }
            };
    }
}
