using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using NUnit.Framework;
using Rent.Entities.Cities;
using Rent.Service.Services.Cities.Views;
using System.Net;
using System.Net.Http.Headers;

namespace Rent.Tests.IntegrationTests
{
    public class CityControllerTests
    {
        private IntegrationTestHelper _helper;
        private WebApplicationFactory<RentAPI.Program> _factory;
        private HttpClient _client;

        [SetUp]
        public void Setup()
        {
            _helper = new IntegrationTestHelper();
            _factory = _helper.GetWebApplicationFactory(Guid.NewGuid().ToString());
            _client = _factory.CreateClient();
        }

        [Test]
        public async Task GetAllCities_ReturnsOk()
        {
            string accessToken = await _helper.GenerateAccessToken(_factory, "landlord1");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await _client.GetAsync("/city/items");

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var responseContent = await response.Content.ReadAsStringAsync();
            var cities = JsonConvert.DeserializeObject<IEnumerable<CityView>>(responseContent);

            Assert.That(cities,
               Is.EqualTo(CityViews).Using(new CityViewEqualityComparer()), message: "GetAllCities endpoint works incorrect");
        }

        private IEnumerable<CityView> CityViews =>
            new[]
            {
                new CityView { Id = 1, Name = "City1" },
                new CityView { Id = 2, Name = "City2" }
            };

        [TearDown]
        public void TearDown()
        {
            _client.Dispose();
            _factory.Dispose();
        }
    }
}
