using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using NUnit.Framework;
using Rent.Entities.Cities;
using Rent.Entities.Comments;
using Rent.Entities.Photos;
using Rent.Entities.Properties;
using Rent.Entities.Responses;
using Rent.Entities.Users;
using Rent.Storage.Configuration;
using RentAPI.Models.Auth;
using System.Text;

namespace Rent.Tests.IntegrationTests
{
    public class TestControllerTest
    {
        private WebApplicationFactory<RentAPI.Program> _factory;
        private HttpClient _client;

        [SetUp]
        public void Setup()
        {
            _factory = new WebApplicationFactory<RentAPI.Program>().WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    var dbContextDescriptor = services.SingleOrDefault(d =>
                        d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));

                    services.Remove(dbContextDescriptor);

                    services.AddDbContext<ApplicationDbContext>(options =>
                    {
                        options.UseInMemoryDatabase(Guid.NewGuid().ToString());
                    });

                });
            });

            using var scope = _factory.Services.CreateScope(); ;
            var db = scope.ServiceProvider.GetService<ApplicationDbContext>();

            SeedData(db);
            _client = _factory.CreateClient();
        }

        [Test]
        public async Task Test_test()
        {
            var response = await _client.GetAsync("/Test");
            var stingResult = await response.Content.ReadAsStringAsync();

            Assert.That(stingResult, Is.EqualTo("3"));
        }

        [TearDown]
        public void TearDown()
        {
            _client.Dispose();
            _factory.Dispose();
        }

        public void SeedData(ApplicationDbContext context)
        {
            context.Cities.AddRange(
                new City { Id = 1, Name = "City1" },
                new City { Id = 2, Name = "City2" }
            );

            context.SaveChanges();
        }
    }
}
