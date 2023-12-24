using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using Rent.Entities.Properties;
using Rent.Service.Services.Properties.Views;
using Rent.Tests.Infrastructure;
using RentAPI.Infrastructure.Middlewares;
using RentAPI.Models.Properties;
using System.Net;
using System.Net.Http.Headers;

namespace Rent.Tests.IntegrationTests
{
    public class PropertyControllerTests : BaseIntegrationTest
    {
        #region AddNewProperty
        [Test]
        public Task PropertyController_PropertyController_AddNewProperty_ShouldReturnOk()
        {
            return PerformTest(async (client) =>
            {
                // Arrange
                string accessToken = await GenerateAccessToken(username: "landlord1");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                var model = GetAddPropertyModel(cityId: 1);
                var content = GetMultipartFormDataContentByAddPropertyModel(model);

                // Act
                var response = await client.PostAsync("property/landlord", content);

                var responseContent = await response.Content.ReadAsStringAsync();

                // Assert
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(responseContent, Is.EqualTo("Added successfully!"));
            }, configureServices: service =>
            {
                FileStorageServiceClientMock
                   .Setup(_ => _.UploadFilesAsync(It.IsAny<IFormFile[]>()))
                       .ReturnsAsync(new List<string> { "6", "7", "8" });

                service.AddSingleton(FileStorageServiceClientMock.Object);
            });
        }

        [Test]
        public Task PropertyController_AddNewProperty_ShouldReturnCityNotFound()
        {
            return PerformTest(async (client) =>
            {
                // Arrange
                string accessToken = await GenerateAccessToken(username: "landlord1");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                var model = GetAddPropertyModel(cityId: 9999);
                var content = GetMultipartFormDataContentByAddPropertyModel(model);

                // Act
                var response = await client.PostAsync("property/landlord", content);

                var responseContent = await response.Content.ReadAsStringAsync();
                var errorResponse = JsonConvert.DeserializeObject<ErrorResponse>(responseContent);

                // Assert
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
                Assert.That(errorResponse.Message, Is.EqualTo("City not found."));
            });
        }

        private AddPropertyModel GetAddPropertyModel(int cityId)
        {
            return new AddPropertyModel
            {
                CityId = cityId,
                Address = "Test Address",
                Description = "Test Description",
                Price = 1000,
                Photos = new IFormFile[]
                {
                    GetPhoto("jpg1.jpg"),
                    GetPhoto("png1.png"),
                    GetPhoto("webp1.webp"),
                }
            };
        }
        private MultipartFormDataContent GetMultipartFormDataContentByAddPropertyModel(AddPropertyModel model)
        {
            var content = new MultipartFormDataContent
            {
                { new StringContent(model.CityId.ToString()), "CityId" },
                { new StringContent(model.Address), "Address" },
                { new StringContent(model.Description), "Description" },
                { new StringContent(model.Price.ToString()), "Price" }
            };
            foreach (var photo in model.Photos)
            {
                var streamContent = new StreamContent(photo.OpenReadStream());
                streamContent.Headers.ContentType = new MediaTypeHeaderValue(photo.ContentType);
                content.Add(streamContent, "Photos", photo.FileName);
            }
            return content;
        }
        #endregion

        #region EditProperty
        [Test]
        public Task PropertyController_EditProperty_ShouldReturnOk()
        {
            return PerformTest(async (client) =>
            {
                // Arrange
                string accessToken = await GenerateAccessToken(username: "landlord1");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                var model = GetEditPropertyModel(propertyId: 1);
                var content = GetMultipartFormDataContentByEditPropertyModel(model);

                // Act
                var response = await client.PatchAsync("property/landlord", content);

                var responseContent = await response.Content.ReadAsStringAsync();

                // Assert
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(responseContent, Is.EqualTo("Edited successfully!"));
            }, configureServices: service =>
            {
                FileStorageServiceClientMock
                   .Setup(_ => _.UploadFilesAsync(It.IsAny<IFormFile[]>()))
                       .ReturnsAsync(new List<string> { "6", "7", "8" });

                FileStorageServiceClientMock
                     .Setup(_ => _.DeleteFilesAsync(new string[] { "1", "2" }));

                service.AddSingleton(FileStorageServiceClientMock.Object);
            });
        }

        [Test]
        public Task PropertyController_EditProperty_ShouldReturnPropertyNotFound()
        {
            return PerformTest(async (client) =>
            {
                // Arrange
                string accessToken = await GenerateAccessToken(username: "landlord1");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                var model = GetEditPropertyModel(propertyId: 9999);
                var content = GetMultipartFormDataContentByEditPropertyModel(model);

                // Act
                var response = await client.PatchAsync("property/landlord", content);

                var responseContent = await response.Content.ReadAsStringAsync();
                var errorResponse = JsonConvert.DeserializeObject<ErrorResponse>(responseContent);

                // Assert
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
                Assert.That(errorResponse.Message, Is.EqualTo("Property not found."));
            });
        }

        [Test]
        public Task PropertyController_EditProperty_PropertyNotFound_ShouldReturnForbidden()
        {
            return PerformTest(async (client) =>
            {
                // Arrange
                string accessToken = await GenerateAccessToken(username: "landlord1", fakeUserId: "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxaxxxxxx");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                var model = GetEditPropertyModel(propertyId: 1);
                var content = GetMultipartFormDataContentByEditPropertyModel(model);

                // Act
                var response = await client.PatchAsync("property/landlord", content);

                var responseContent = await response.Content.ReadAsStringAsync();
                var errorResponse = JsonConvert.DeserializeObject<ErrorResponse>(responseContent);

                // Assert
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
                Assert.That(errorResponse.Message, Is.EqualTo("Access denied. You do not have permission to edit this property."));
            });
        }

        private EditPropertyModel GetEditPropertyModel(int propertyId)
        {
            return new EditPropertyModel
            {
                Id = propertyId,
                Address = "Edited Test Address",
                Description = "Edited Test Description",
                Price = 5000,
                Status = PropertyStatus.Occupied,
                Photos = new IFormFile[]
                {
                    GetPhoto("jpg2.jpg"),
                    GetPhoto("png2.png"),
                    GetPhoto("webp2.webp"),
                },
                PhotoIdsToDelete = new string[] { "1", "2" }
            };
        }

        private MultipartFormDataContent GetMultipartFormDataContentByEditPropertyModel(EditPropertyModel model)
        {
            var content = new MultipartFormDataContent
            {
                { new StringContent(model.Id.ToString()), "Id" },
                { new StringContent(model.Address), "Address" },
                { new StringContent(model.Description), "Description" },
                { new StringContent(model.Price.ToString()), "Price" },
                { new StringContent(model.Status.ToString()), "Status" }
            };

            foreach (var photo in model.Photos)
            {
                var streamContent = new StreamContent(photo.OpenReadStream());
                streamContent.Headers.ContentType = new MediaTypeHeaderValue(photo.ContentType);
                content.Add(streamContent, "Photos", photo.FileName);
            }

            foreach (var photoId in model.PhotoIdsToDelete)
            {
                content.Add(new StringContent(photoId), "PhotoIdsToDelete");
            }

            return content;
        }

        #endregion

        #region GetPropertiesByCityId
        [Test]
        public Task PropertyController_GetPropertiesByCityId_ShouldReturnOk()
        {
            return PerformTest(async (client) =>
            {
                // Arrange
                string accessToken = await GenerateAccessToken(username: "tenant1");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                int cityId = 1;

                // Act
                var response = await client.GetAsync($"property/tenant/items/{cityId}");

                var responseContent = await response.Content.ReadAsStringAsync();
                var properties = JsonConvert.DeserializeObject<IEnumerable<PropertyView>>(responseContent);

                // Assert
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(properties,
                   Is.EqualTo(PropertyViewsByCity).Using(new PropertyViewEqualityComparer()));
            });
        }

        [Test]
        public Task PropertyController_GetPropertiesByCityId_ShouldReturnCityNotFound()
        {
            return PerformTest(async (client) =>
            {
                // Arrange
                string accessToken = await GenerateAccessToken(username: "tenant1");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                int cityId = 9999;

                // Act
                var response = await client.GetAsync($"property/tenant/items/{cityId}");

                var responseContent = await response.Content.ReadAsStringAsync();
                var errorResponse = JsonConvert.DeserializeObject<ErrorResponse>(responseContent);

                // Assert
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
                Assert.That(errorResponse.Message, Is.EqualTo("City not found."));
            });
        }

        private IEnumerable<PropertyView> PropertyViewsByCity =>
           new[]
           {
                new PropertyView { Id = 1, CityName = "City1", Address = "Address1", PhotoUrl = "https://drive.google.com/uc?id=1", Price = 1000 }
           };
        #endregion

        #region GetPropertiesByLandlordId
        [Test]
        public Task PropertyController_GetPropertiesByLandlordId_ShouldReturnOk()
        {
            return PerformTest(async (client) =>
            {
                // Arrange
                string accessToken = await GenerateAccessToken(username: "landlord1");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                // Act
                var response = await client.GetAsync("property/landlord/items");

                var responseContent = await response.Content.ReadAsStringAsync();
                var properties = JsonConvert.DeserializeObject<IEnumerable<PropertyView>>(responseContent);

                // Assert
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(properties,
                   Is.EqualTo(PropertyViewsByLandlord).Using(new PropertyViewEqualityComparer()));
            });
        }

        [Test]
        public Task PropertyController_GetPropertiesByLandlordId_ShouldReturnUserNotFound()
        {
            return PerformTest(async (client) =>
            {
                // Arrange
                string accessToken = await GenerateAccessToken(username: "landlord1", fakeUserId: "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxaxxxxxx");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                // Act
                var response = await client.GetAsync("property/landlord/items");

                var responseContent = await response.Content.ReadAsStringAsync();
                var errorResponse = JsonConvert.DeserializeObject<ErrorResponse>(responseContent);

                // Assert
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
                Assert.That(errorResponse.Message, Is.EqualTo("User not found."));
            });
        }

        private IEnumerable<PropertyView> PropertyViewsByLandlord =>
            new[]
            {
                new PropertyView { Id = 1, CityName = "City1", Address = "Address1", PhotoUrl = "https://drive.google.com/uc?id=1", Price = 1000 },
                new PropertyView { Id = 2, CityName = "City2", Address = "Address2", PhotoUrl = "https://drive.google.com/uc?id=3", Price = 2000 }
            };
        #endregion

        #region GetPropertyFullInfoById
        [Test]
        public Task PropertyController_GetPropertyFullInfoById_ShouldReturnOk()
        {
            return PerformTest(async (client) =>
            {
                // Arrange
                string accessToken = await GenerateAccessToken(username: "landlord1");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                int propertyId = 1;

                // Act
                var response = await client.GetAsync($"property/item/{propertyId}");

                var responseContent = await response.Content.ReadAsStringAsync();
                var property = JsonConvert.DeserializeObject<PropertyDetailView>(responseContent);

                // Assert
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(property,
                   Is.EqualTo(PropertyDetailViews).Using(new PropertyDetailViewEqualityComparer()));
            });
        }

        [Test]
        public Task PropertyController_GetPropertyFullInfoById_ShouldReturnPropertyNotFound()
        {
            return PerformTest(async (client) =>
            {
                // Arrange
                string accessToken = await GenerateAccessToken(username: "landlord1");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                int propertyId = 9999;

                // Act
                var response = await client.GetAsync($"property/item/{propertyId}");

                var responseContent = await response.Content.ReadAsStringAsync();
                var errorResponse = JsonConvert.DeserializeObject<ErrorResponse>(responseContent);

                // Assert
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
                Assert.That(errorResponse.Message, Is.EqualTo("Property not found."));
            });
        }

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
        #endregion

        #region DeleteProperty
        [Test]
        public Task PropertyController_DeleteProperty_ShouldReturnOk()
        {
            return PerformTest(async (client) =>
            {
                // Arrange
                string accessToken = await GenerateAccessToken(username: "landlord1");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                int propertyId = 1;

                // Act
                var response = await client.DeleteAsync($"property/landlord/{propertyId}");

                var responseContent = await response.Content.ReadAsStringAsync();

                // Assert
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(responseContent, Is.EqualTo("Deleted successfully!"));
            }, configureServices: service =>
            {
                FileStorageServiceClientMock
                     .Setup(_ => _.DeleteFilesAsync(new string[] { "1", "2"}));

                service.AddSingleton(FileStorageServiceClientMock.Object);
            });
        }

        [Test]
        public Task PropertyController_DeleteProperty_ShouldReturnPropertyNotFound()
        {
            return PerformTest(async (client) =>
            {
                // Arrange
                string accessToken = await GenerateAccessToken(username: "landlord1");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                int propertyId = 9999;

                // Act
                var response = await client.DeleteAsync($"property/landlord/{propertyId}");

                var responseContent = await response.Content.ReadAsStringAsync();
                var errorResponse = JsonConvert.DeserializeObject<ErrorResponse>(responseContent);

                // Assert
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
                Assert.That(errorResponse.Message, Is.EqualTo("Property not found."));
            });
        }

        [Test]
        public Task PropertyController_DeleteProperty_ShouldReturnForbidden()
        {
            return PerformTest(async (client) =>
            {
                // Arrange
                string accessToken = await GenerateAccessToken(username: "landlord1", fakeUserId: "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxaxxxxxx");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                int propertyId = 1;

                // Act
                var response = await client.DeleteAsync($"property/landlord/{propertyId}");

                var responseContent = await response.Content.ReadAsStringAsync();
                var errorResponse = JsonConvert.DeserializeObject<ErrorResponse>(responseContent);

                // Assert
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
                Assert.That(errorResponse.Message, Is.EqualTo("Access denied. You do not have permission to delete this property."));
            });
        }
        #endregion
    }
}
