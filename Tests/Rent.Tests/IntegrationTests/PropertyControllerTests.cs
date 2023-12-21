using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using NUnit.Framework;
using Rent.Service.Services.Properties.Views;
using RentAPI.Infrastructure.Middlewares;
using System.Net.Http.Headers;
using System.Net;
using Rent.Tests.Helpers;
using Microsoft.AspNetCore.Http;
using RentAPI.Models.Properties;
using Microsoft.AspNetCore.TestHost;
using Rent.Service.Services.FileStorage;
using Moq;
using Microsoft.Extensions.DependencyInjection;

namespace Rent.Tests.IntegrationTests
{
    public class PropertyControllerTests
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

        #region AddNewProperty
        //[Test]
        //public async Task AddNewProperty_ReturnsOk()
        //{
        //    // Arrange
        //    _factory.WithWebHostBuilder(builder =>
        //    {
        //        builder.ConfigureTestServices(services =>
        //        {
        //            var fileStorageService = services.SingleOrDefault(d => d.ServiceType == typeof(IFileStorageService));
        //            services.Remove(fileStorageService);

        //            var mockedService = new Mock<IFileStorageService>();
        //            mockedService.Setup(_ => _.UploadFilesAsync(It.IsAny<IFormFile[]>()))
        //                .ReturnsAsync(new List<string> { "fileId1", "fileId2" });
        //            services.AddScoped(_ => mockedService.Object);
        //        });
        //    });
        //    _client = _factory.CreateClient();

        //    string accessToken = await _helper.GenerateAccessToken(_factory, "landlord1");
        //    _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        //    var model = new AddPropertyModel
        //    {
        //        CityId = 1,
        //        Address = "Test Address",
        //        Description = "Test Description",
        //        Price = 1000,
        //        Photos = new IFormFile[]
        //        {
        //            _helper.GetPhoto("jpg1.jpg"),
        //            _helper.GetPhoto("jpg2.jpg"),
        //            _helper.GetPhoto("jpg3.jpg"),
        //        }
        //    };

        //    var content = new MultipartFormDataContent
        //    {
        //        { new StringContent(model.CityId.ToString()), "CityId" },
        //        { new StringContent(model.Address), "Address" },
        //        { new StringContent(model.Description), "Description" },
        //        { new StringContent(model.Price.ToString()), "Price" }
        //    };
        //    foreach (var photo in model.Photos)
        //    {
        //        var streamContent = new StreamContent(photo.OpenReadStream());
        //        streamContent.Headers.ContentType = new MediaTypeHeaderValue(photo.ContentType);
        //        content.Add(streamContent, "Photos", photo.FileName);
        //    }
        //    // Act
        //    var response = await _client.PostAsync("property/landlord/add", content);

        //    // Assert
        //    Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        //}

        //[Test]
        //public async Task AddNewProperty_UnsupportedFileType_ReturnsBadRequest()
        //{
        //    // Arrange
        //    string accessToken = await _helper.GenerateAccessToken(_factory, "landlord1");
        //    _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        //    var model = new AddPropertyModel
        //    {
        //        CityId = 1,
        //        Address = "Test Address",
        //        Description = "Test Description",
        //        Price = 1000,
        //        Photos = new IFormFile[] { /* Mocked IFormFile objects with unsupported file types */ }
        //    };

        //    var content = new MultipartFormDataContent();
        //    // Add model properties to the content

        //    // Act
        //    var response = await _client.PostAsync("property/landlord/add", content);

        //    // Assert
        //    Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        //}

        //[Test]
        //public async Task AddNewProperty_CityNotFound_ReturnsNotFound()
        //{
        //    // Arrange
        //    string accessToken = await _helper.GenerateAccessToken(_factory, "landlord1");
        //    _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        //    var model = new AddPropertyModel
        //    {
        //        CityId = 9999, // Non-existing city ID
        //        Address = "Test Address",
        //        Description = "Test Description",
        //        Price = 1000,
        //        Photos = new IFormFile[] { /* Mocked IFormFile objects with supported file types */ }
        //    };

        //    var content = new MultipartFormDataContent();
        //    // Add model properties to the content

        //    // Act
        //    var response = await _client.PostAsync("property/landlord/add", content);

        //    // Assert
        //    Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        //}
        #endregion


        #region EditProperty

        #endregion

        #region GetPropertiesByCityId
        [Test]
        public async Task GetPropertiesByCityId_ReturnsOk()
        {
            // Arrange
            string accessToken = await _helper.GenerateAccessToken(_factory, "tenant1");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            int cityId = 1;

            // Act
            var response = await _client.GetAsync($"property/tenant/items/{cityId}");

            var responseContent = await response.Content.ReadAsStringAsync();
            var properties = JsonConvert.DeserializeObject<IEnumerable<PropertyView>>(responseContent);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(properties,
               Is.EqualTo(PropertyViewsByCity).Using(new PropertyViewEqualityComparer()));
        }

        [Test]
        public async Task GetPropertiesByCityId_CityNotFound_ReturnsNotFound()
        {
            // Arrange
            string accessToken = await _helper.GenerateAccessToken(_factory, "tenant1");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            int cityId = 9999;

            // Act
            var response = await _client.GetAsync($"property/tenant/items/{cityId}");
            var responseContent = await response.Content.ReadAsStringAsync();
            var errorResponse = JsonConvert.DeserializeObject<ErrorResponse>(responseContent);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
            Assert.That(errorResponse.Message, Is.EqualTo("City not found."));
        }
        #endregion

        #region GetPropertiesByLandlordId
        [Test]
        public async Task GetPropertiesByLandlordId_ReturnsOk()
        {
            // Arrange
            string accessToken = await _helper.GenerateAccessToken(_factory, "landlord1");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            // Act
            var response = await _client.GetAsync("property/landlord/items");

            var responseContent = await response.Content.ReadAsStringAsync();
            var properties = JsonConvert.DeserializeObject<IEnumerable<PropertyView>>(responseContent);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(properties,
               Is.EqualTo(PropertyViewsByLandlord).Using(new PropertyViewEqualityComparer()));
        }

        [Test]
        public async Task GetPropertiesByLandlordId_UserNotFound_ReturnsNotFound()
        {
            // Arrange
            string accessToken = await _helper.GenerateAccessToken(_factory, "landlord1", "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxaxxxxxx");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            // Act
            var response = await _client.GetAsync("property/landlord/items");
            var responseContent = await response.Content.ReadAsStringAsync();
            var errorResponse = JsonConvert.DeserializeObject<ErrorResponse>(responseContent);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
            Assert.That(errorResponse.Message, Is.EqualTo("User not found."));
        }
        #endregion

        #region GetPropertyFullInfoById
        [Test]
        public async Task GetPropertyFullInfoById_ReturnsOk()
        {
            // Arrange
            string accessToken = await _helper.GenerateAccessToken(_factory, "landlord1");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            int propertyId = 1;

            // Act
            var response = await _client.GetAsync($"property/item/{propertyId}");

            var responseContent = await response.Content.ReadAsStringAsync();
            var property = JsonConvert.DeserializeObject<PropertyDetailView>(responseContent);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(property,
               Is.EqualTo(PropertyDetailViews).Using(new PropertyDetailViewEqualityComparer()));
        }

        [Test]
        public async Task GetPropertyFullInfoById_PropertyNotFound_ReturnsNotFound()
        {
            // Arrange
            string accessToken = await _helper.GenerateAccessToken(_factory, "landlord1");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            int propertyId = 9999;

            // Act
            var response = await _client.GetAsync($"property/item/{propertyId}");
            var responseContent = await response.Content.ReadAsStringAsync();
            var errorResponse = JsonConvert.DeserializeObject<ErrorResponse>(responseContent);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
            Assert.That(errorResponse.Message, Is.EqualTo("Property not found."));
        }
        #endregion

        #region DeleteProperty

        #endregion

        private IEnumerable<PropertyView> PropertyViewsByCity =>
            new[]
            {
                new PropertyView { Id = 1, CityName = "City1", Address = "Address1", PhotoUrl = "https://drive.google.com/uc?id=1", Price = 1000 }
            };

        private IEnumerable<PropertyView> PropertyViewsByLandlord =>
            new[]
            {
                new PropertyView { Id = 1, CityName = "City1", Address = "Address1", PhotoUrl = "https://drive.google.com/uc?id=1", Price = 1000 },
                new PropertyView { Id = 2, CityName = "City2", Address = "Address2", PhotoUrl = "https://drive.google.com/uc?id=3", Price = 2000 }
            };

        private PropertyDetailView PropertyDetailViews =>
            new PropertyDetailView
            {
                Id = 1,
                CityId = 1,
                CityName = "City1",
                Address = "Address1",
                Description = "Description1",
                Price = 1000,
                Photos = new PhotoView[]
                {
                    new PhotoView { Id = "1", Url = "https://drive.google.com/uc?id=1" },
                    new PhotoView { Id = "2", Url = "https://drive.google.com/uc?id=2" }
                }
            };

        [TearDown]
        public void TearDown()
        {
            _client.Dispose();
            _factory.Dispose();
        }
    }
}
