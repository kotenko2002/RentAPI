using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using NUnit.Framework;
using System.Net.Http.Headers;
using System.Net;
using RentAPI.Models.Responses;
using System.Text;
using RentAPI.Infrastructure.Middlewares;
using Rent.Entities.Responses;
using Rent.Service.Services.Responses.Views;
using Rent.Tests.Infrastructure;

namespace Rent.Tests.IntegrationTests
{
    public class ResponseControllerTests : BaseIntegrationTest
    {
        #region AddNewResponse
        [Test]
        public Task ResponseController_AddNewResponse_ShouldReturnOk()
        {
            return PerformTest(async (client) =>
            {
                // Arrange
                string accessToken = await GenerateAccessToken(username: "tenant1");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                var model = new AddResponseModel
                {
                    PropertyId = 1,
                    Message = "Test message"
                };
                var content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");

                // Act
                var response = await client.PostAsync("response/tenant/add", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                // Assert
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(responseContent, Is.EqualTo("Added successfully!"));
            });
        }
        #endregion

        #region GetResponseByPropertyId
        [Test]
        public Task ResponseController_GetResponsesByPropertyId_ShouldReturnOk()
        {
            return PerformTest(async (client) =>
            {
                // Arrange
                string accessToken = await GenerateAccessToken(username: "landlord1");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                int propertyId = 1;

                // Act
                var response = await client.GetAsync($"response/landlord/items/{propertyId}");

                var responseContent = await response.Content.ReadAsStringAsync();
                var responses = JsonConvert.DeserializeObject<IEnumerable<ResponseView>>(responseContent);

                // Assert
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(responses,
                   Is.EqualTo(ResponseViews).Using(new ResponseViewEqualityComparer()));
            });
        }

        [Test]
        public Task ResponseController_GetResponseByPropertyId_ShouldReturnPropertyNotFound()
        {
            return PerformTest(async (client) =>
            {
                // Arrange
                string accessToken = await GenerateAccessToken(username: "landlord1");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                int propertyId = 9999; 

                // Act
                var response = await client.GetAsync($"response/landlord/items/{propertyId}");
                var responseContent = await response.Content.ReadAsStringAsync();
                var errorResponse = JsonConvert.DeserializeObject<ErrorResponse>(responseContent);

                // Assert
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
                Assert.That(errorResponse.Message, Is.EqualTo("Property not found."));
            });
        }

        [Test]
        public Task ResponseController_GetResponseByPropertyId_ShouldReturnForbidden()
        {
            return PerformTest(async (client) =>
            {
                // Arrange
                string accessToken = await GenerateAccessToken(username: "landlord2");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                int propertyId = 1;

                // Act
                var response = await client.GetAsync($"response/landlord/items/{propertyId}");
                var responseContent = await response.Content.ReadAsStringAsync();
                var errorResponse = JsonConvert.DeserializeObject<ErrorResponse>(responseContent);

                // Assert
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
                Assert.That(errorResponse.Message, Is.EqualTo("Access denied. You do not have permission to get responses for this property."));
            });
        }

        private IEnumerable<ResponseView> ResponseViews =>
            new[]
            {
                new ResponseView { Id = 1, Email = "tenant1@example.com", PhoneNumber = "0988888883", Message = "Message1", Status = ResponseStatus.ApprovedToRent },
                new ResponseView { Id = 4, Email = "tenant2@example.com", PhoneNumber = "0988888884", Message = "Message4", Status = ResponseStatus.NotReviewed }
            };
        #endregion

        #region Process
        [Test]
        public Task ResponseController_Process_ShouldReturnOk()
        {
            return PerformTest(async (client) =>
            {
                // Arrange
                string accessToken = await GenerateAccessToken(username: "landlord1");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                var model = new ProcessResponseModel
                {
                    ResponseId = 1, 
                    Status = ResponseStatus.ApprovedToRent 
                };

                var content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");

                // Act
                var response = await client.PatchAsync("response/landlord/process", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                // Assert
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(responseContent, Is.EqualTo("Status updated successfully!"));
            });
        }

        [Test]
        public Task ResponseController_Process_AccessDenied_ShouldReturnForbidden()
        {
            return PerformTest(async (client) =>
            {
                // Arrange
                string accessToken = await GenerateAccessToken(username: "landlord2");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                var model = new ProcessResponseModel
                {
                    ResponseId = 1,
                    Status = ResponseStatus.ApprovedToRent
                };

                var content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");

                // Act
                var response = await client.PatchAsync("response/landlord/process", content);
                var responseContent = await response.Content.ReadAsStringAsync();
                var errorResponse = JsonConvert.DeserializeObject<ErrorResponse>(responseContent);

                // Assert
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
                Assert.That(errorResponse.Message, Is.EqualTo("Access denied. You do not have permission to process this response."));
            });
        }
        #endregion
    }
}
