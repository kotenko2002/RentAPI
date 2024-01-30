using NUnit.Framework;
using Rent.Entities.Properties;
using Rent.Entities.Responses;
using Rent.Storage.Configuration;
using Rent.Storage.Repositories.Responses;
using Rent.Tests.Infrastructure;

namespace Rent.Tests.StorageTests
{
    public class ResponseRepositoryTests : BaseUnitTest
    {
        #region BasicMethods
        [TestCase(1)]
        [TestCase(2)]
        public async Task ResponseRepository_FindAsync_ShouldReturnSingleValue(int id)
        {
            using var context = new ApplicationDbContext(GetUnitTestDbOptions());
            var responseRepository = new ResponseRepository(context);

            var actual = await responseRepository.FindAsync(id);
            var expected = ExpectedResponses.FirstOrDefault(x => x.Id == id);

            Assert.That(actual,
                Is.EqualTo(expected).Using(new ResponseEqualityComparer()), message: "FindAsync method works incorrect");
        }

        [Test]
        public async Task ResponseRepository_GetAllAsync_ShouldReturnAllValues()
        {
            using var context = new ApplicationDbContext(GetUnitTestDbOptions());
            var responseRepository = new ResponseRepository(context);

            var actual = await responseRepository.FindAllAsync();

            Assert.That(actual,
                Is.EqualTo(ExpectedResponses).Using(new ResponseEqualityComparer()), message: "GetAllAsync method works incorrect");
        }

        [Test]
        public async Task ResponseRepository_AddAsync_ShouldAddValueToDatabase()
        {
            using var context = new ApplicationDbContext(GetUnitTestDbOptions());
            var responseRepository = new ResponseRepository(context);

            var response = new Response { Id = 5, TenantId = "4", PropertyId = 2, Message = "Message5", Status = ResponseStatus.ApprovedToDialog };
            await responseRepository.AddAsync(response);
            await context.SaveChangesAsync();

            Assert.That(context.Responses.Count(),
                Is.EqualTo(5), message: "AddAsync method works incorrect");
        }

        [Test]
        public async Task ResponseRepository_AddRangeAsync_ShouldAddValueToDatabase()
        {
            using var context = new ApplicationDbContext(GetUnitTestDbOptions());
            var responseRepository = new ResponseRepository(context);

            var responses = new[]
            {
                new Response { Id = 5, TenantId = "4", PropertyId = 2, Message = "Message5", Status = ResponseStatus.ApprovedToDialog },
                new Response { Id = 6, TenantId = "4", PropertyId = 3, Message = "Message6", Status = ResponseStatus.NotReviewed }
            };
            await responseRepository.AddRangeAsync(responses);
            await context.SaveChangesAsync();

            Assert.That(context.Responses.Count(),
                Is.EqualTo(6), message: "AddRangeAsync method works incorrect");
        }

        [Test]
        public async Task ResponseRepository_RemoveAsync_ShouldRemoveValueFromDatabase()
        {
            using var context = new ApplicationDbContext(GetUnitTestDbOptions());
            var responseRepository = new ResponseRepository(context);

            var response = new Response { Id = 1, TenantId = "3", PropertyId = 1, Message = "Message1", Status = ResponseStatus.ApprovedToRent };
            await responseRepository.RemoveAsync(response);
            await context.SaveChangesAsync();

            Assert.That(context.Responses.Count(),
                Is.EqualTo(3), message: "RemoveAsync method works incorrect");
        }

        [Test]
        public async Task ResponseRepository_RemoveRangeAsync_ShouldRemoveValuesFromDatabase()
        {
            using var context = new ApplicationDbContext(GetUnitTestDbOptions());
            var responseRepository = new ResponseRepository(context);

            var responses = new[]
            {
                new Response { Id = 1, TenantId = "3", PropertyId = 1, Message = "Message1", Status = ResponseStatus.ApprovedToRent },
                new Response { Id = 2, TenantId = "3", PropertyId = 2, Message = "Message2", Status = ResponseStatus.ApprovedToDialog },
            };
            await responseRepository.RemoveRangeAsync(responses);
            await context.SaveChangesAsync();

            Assert.That(context.Responses.Count(),
                Is.EqualTo(2), message: "RemoveRangeAsync method works incorrect");
        }
        #endregion

        [Test]
        public async Task ResponseRepository_GetFullResponseByIdAsync_ShouldReturnExpectedResponse()
        {
            using var context = new ApplicationDbContext(GetUnitTestDbOptions());
            var responseRepository = new ResponseRepository(context);

            var expected = ExpectedResponses.FirstOrDefault(x => x.Id == 1);
            var actual = await responseRepository.GetFullResponseByIdAsync(1);

            Assert.That(actual, Is.EqualTo(expected).Using(new ResponseEqualityComparer()), message: "GetFullResponseByIdAsync method works incorrect");

            Assert.That(actual.Property,
                Is.EqualTo(ExpectedProperties.FirstOrDefault(x => x.Id == expected.PropertyId)).Using(new PropertyEqualityComparer()), message: "GetFullResponseByIdAsync method doesnt't return included entities");
        }

        [TestCase(1, "3")]
        [TestCase(2, "3")]
        [TestCase(1, "4")]
        public async Task ResponseRepository_GetResponseByPropertyAndTenantIdsAsync_ShouldReturnExpectedResponse(int propertyId, string tenantId)
        {
            using var context = new ApplicationDbContext(GetUnitTestDbOptions());
            var responseRepository = new ResponseRepository(context);

            var expected = ExpectedResponses.FirstOrDefault(x => x.PropertyId == propertyId && x.TenantId == tenantId);
            var actual = await responseRepository.GetResponseByPropertyAndTenantIdsAsync(propertyId, tenantId);

            Assert.That(actual,
                Is.EqualTo(expected).Using(new ResponseEqualityComparer()), message: "GetResponseByPropertyAndTenantIdsAsync method works incorrect");
        }

        private static IEnumerable<Response> ExpectedResponses =>
            new[]
            {
                new Response { Id = 1, TenantId = "3", PropertyId = 1, Message = "Message1", Status = ResponseStatus.ApprovedToRent },
                new Response { Id = 2, TenantId = "3", PropertyId = 2, Message = "Message2", Status = ResponseStatus.ApprovedToDialog },
                new Response { Id = 3, TenantId = "3", PropertyId = 3, Message = "Message3", Status = ResponseStatus.ApprovedToRent },
                new Response { Id = 4, TenantId = "4", PropertyId = 1, Message = "Message4", Status = ResponseStatus.NotReviewed },
            };

        private static IEnumerable<Property> ExpectedProperties =>
            new[]
            {
                new Property { Id = 1, LandlordId = "1", CityId = 1, Address = "Address1", Description = "Description1", Price = 1000, Status = PropertyStatus.Available },
                new Property { Id = 2, LandlordId = "1", CityId = 2, Address = "Address2", Description = "Description2", Price = 2000, Status = PropertyStatus.Occupied },
                new Property { Id = 3, LandlordId = "2", CityId = 2, Address = "Address3", Description = "Description3", Price = 3000, Status = PropertyStatus.Occupied }
            };
    }
}
