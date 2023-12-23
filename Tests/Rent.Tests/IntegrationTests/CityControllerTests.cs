using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using NUnit.Framework;
using Rent.Service.Services.Cities.Views;
using Rent.Tests.Helpers;
using System.Net;
using System.Net.Http.Headers;

namespace Rent.Tests.IntegrationTests
{
    public class CityControllerTests : BaseIntegrationTest
    {
        [Test]
        public Task GetAllCities_ReturnsOk()
        {
            return PerformTest(async (client) =>
            {
                // Arrange
                string accessToken = await GenerateAccessToken(username: "landlord1");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                // Act
                var response = await client.GetAsync("city/items");

                var responseContent = await response.Content.ReadAsStringAsync();
                var cities = JsonConvert.DeserializeObject<IEnumerable<CityView>>(responseContent);

                // Assert
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(cities,
                   Is.EqualTo(CityViews).Using(new CityViewEqualityComparer()));
            });
        }

        private IEnumerable<CityView> CityViews =>
            new[]
            {
                new CityView { Id = 1, Name = "City1" },
                new CityView { Id = 2, Name = "City2" }
            };
    }
}
