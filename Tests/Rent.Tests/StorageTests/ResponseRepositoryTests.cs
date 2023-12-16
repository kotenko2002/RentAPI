using NUnit.Framework;
using Rent.Entities.Responses;
using Rent.Storage.Configuration;
using Rent.Storage.Repositories.Responses;

namespace Rent.Tests.StorageTests
{
    public class ResponseRepositoryTests
    {
        #region BasicMethods
        [TestCase(1)]
        [TestCase(2)]
        public async Task ResponseRepository_FindAsync_ReturnsSingleValue(int id)
        {
            using var context = new ApplicationDbContext(UnitTestHelper.GetUnitTestDbOptions());
            var responseRepository = new ResponseRepository(context);

            var actual = await responseRepository.FindAsync(id);
            var expected = ExpectedResponses.FirstOrDefault(x => x.Id == id);

            Assert.That(actual,
                Is.EqualTo(expected).Using(new ResponseEqualityComparer()), message: "FindAsync method works incorrect");
        }

        [Test]
        public async Task ResponseRepository_GetAllAsync_ReturnsAllValues()
        {
            using var context = new ApplicationDbContext(UnitTestHelper.GetUnitTestDbOptions());
            var responseRepository = new ResponseRepository(context);

            var actual = await responseRepository.FindAllAsync();

            Assert.That(actual,
                Is.EqualTo(ExpectedResponses).Using(new ResponseEqualityComparer()), message: "GetAllAsync method works incorrect");
        }

        [Test]
        public async Task ResponseRepository_AddAsync_AddsValueToDatabase()
        {
            using var context = new ApplicationDbContext(UnitTestHelper.GetUnitTestDbOptions());
            var responseRepository = new ResponseRepository(context);

            var response = new Response { Id = 4, TenantId = "4", PropertyId = 4, Message = "Message4", Status = ResponseStatus.ApprovedToRent };
            await responseRepository.AddAsync(response);
            await context.SaveChangesAsync();

            Assert.That(context.Responses.Count(),
                Is.EqualTo(4), message: "AddAsync method works incorrect");
        }

        [Test]
        public async Task ResponseRepository_AddRangeAsync_AddsValueToDatabase()
        {
            using var context = new ApplicationDbContext(UnitTestHelper.GetUnitTestDbOptions());
            var responseRepository = new ResponseRepository(context);

            var responses = new[]
            {
            new Response { Id = 4, TenantId = "4", PropertyId = 4, Message = "Message4", Status = ResponseStatus.ApprovedToRent },
            new Response { Id = 5, TenantId = "5", PropertyId = 5, Message = "Message5", Status = ResponseStatus.ApprovedToDialog }
        };
            await responseRepository.AddRangeAsync(responses);
            await context.SaveChangesAsync();

            Assert.That(context.Responses.Count(),
                Is.EqualTo(5), message: "AddRangeAsync method works incorrect");
        }

        [Test]
        public async Task ResponseRepository_RemoveAsync_RemovesValueFromDatabase()
        {
            using var context = new ApplicationDbContext(UnitTestHelper.GetUnitTestDbOptions());
            var responseRepository = new ResponseRepository(context);

            var response = new Response { Id = 1, TenantId = "3", PropertyId = 1, Message = "Message1", Status = ResponseStatus.ApprovedToRent };
            await responseRepository.RemoveAsync(response);
            await context.SaveChangesAsync();

            Assert.That(context.Responses.Count(),
                Is.EqualTo(2), message: "RemoveAsync method works incorrect");
        }

        [Test]
        public async Task ResponseRepository_RemoveRangeAsync_RemovesValuesFromDatabase()
        {
            using var context = new ApplicationDbContext(UnitTestHelper.GetUnitTestDbOptions());
            var responseRepository = new ResponseRepository(context);

            var responses = new[]
            {
            new Response { Id = 1, TenantId = "3", PropertyId = 1, Message = "Message1", Status = ResponseStatus.ApprovedToRent },
            new Response { Id = 2, TenantId = "3", PropertyId = 2, Message = "Message2", Status = ResponseStatus.ApprovedToDialog }
        };
            await responseRepository.RemoveRangeAsync(responses);
            await context.SaveChangesAsync();

            Assert.That(context.Responses.Count(),
                Is.EqualTo(1), message: "RemoveRangeAsync method works incorrect");
        }
        #endregion

        private static IEnumerable<Response> ExpectedResponses =>
            new[]
            {
                new Response { Id = 1, TenantId = "3", PropertyId = 1, Message = "Message1", Status = ResponseStatus.ApprovedToRent },
                new Response { Id = 2, TenantId = "3", PropertyId = 2, Message = "Message2", Status = ResponseStatus.ApprovedToDialog },
                new Response { Id = 3, TenantId = "3", PropertyId = 3, Message = "Message3", Status = ResponseStatus.ApprovedToRent }
            };
    }
}
