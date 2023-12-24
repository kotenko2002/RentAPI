using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using NUnit.Framework;
using System.Net.Http.Headers;
using System.Net;
using Rent.Entities.Comments;
using RentAPI.Infrastructure.Middlewares;
using RentAPI.Models.Comments;
using System.Text;
using Rent.Service.Services.Comments.Views;
using Rent.Tests.Infrastructure;

namespace Rent.Tests.IntegrationTests
{
    public class CommentControllerTests : BaseIntegrationTest
    {
        #region AddNewProperty
        [Test]
        public Task CommentController_AddNewProperty_ShouldReturnOk()
        {
            return PerformTest(async (client) =>
            {
                // Arrange
                string accessToken = await GenerateAccessToken(username: "tenant1");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                var model = new AddCommentModel
                {
                    PropertyId = 1,
                    Message = "Test message",
                    Rate = Rate.Average
                };

                var content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");

                // Act
                var response = await client.PostAsync("comment/tenant/add", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                // Assert
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(responseContent, Is.EqualTo("Added successfully!"));
            });
        }

        [Test]
        public Task CommentController_AddNewProperty_ShouldReturnForbidden()
        {
            return PerformTest(async (client) =>
            {
                // Arrange
                string accessToken = await GenerateAccessToken(username: "tenant2");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                var model = new AddCommentModel
                {
                    PropertyId = 1,
                    Message = "Test message",
                    Rate = Rate.Average
                };

                var content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");

                // Act
                var response = await client.PostAsync("comment/tenant/add", content);
                var responseContent = await response.Content.ReadAsStringAsync();
                var errorResponse = JsonConvert.DeserializeObject<ErrorResponse>(responseContent);

                // Assert
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
                Assert.That(errorResponse.Message, Is.EqualTo("Access denied. You do not have permission to comment this property."));
            });
        }

        #endregion

        #region GetCommentsByPropertyId
        [Test]
        public Task CommentController_GetCommentsByPropertyId_ShouldReturnOk()
        {
            return PerformTest(async (client) =>
            {
                // Arrange
                string accessToken = await GenerateAccessToken(username: "tenant1");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                int propertyId = 1;

                // Act
                var response = await client.GetAsync($"comment/items/{propertyId}");

                var responseContent = await response.Content.ReadAsStringAsync();
                var comments = JsonConvert.DeserializeObject<IEnumerable<CommentView>>(responseContent);

                // Assert
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(comments,
                   Is.EqualTo(CommentViews).Using(new CommentViewEqualityComparer()));
            });
        }

        [Test]
        public Task CommentController_GetCommentsByPropertyId_ShouldReturnPropertyNotFound()
        {
            return PerformTest(async (client) =>
            {
                // Arrange
                string accessToken = await GenerateAccessToken(username: "tenant1");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                int propertyId = 9999;

                // Act
                var response = await client.GetAsync($"comment/items/{propertyId}");
                var responseContent = await response.Content.ReadAsStringAsync();
                var errorResponse = JsonConvert.DeserializeObject<ErrorResponse>(responseContent);

                // Assert
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
                Assert.That(errorResponse.Message, Is.EqualTo("Property not found."));
            });
        }

        private IEnumerable<CommentView> CommentViews =>
            new[]
            {
                new CommentView {  Id = 1, UserName = "tenant1", Message = "Message1", Rate = Rate.Average  }
            };
        #endregion

        #region MyRDeletePropertyegion
        [Test]
        public Task CommentController_DeleteProperty_ShouldReturnOk()
        {
            return PerformTest(async (client) =>
            {
                // Arrange
                string accessToken = await GenerateAccessToken(username: "tenant1");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                int commentId = 1; // Replace with appropriate test data

                // Act
                var response = await client.DeleteAsync($"comment/tenant/{commentId}");

                // Assert
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

                var responseContent = await response.Content.ReadAsStringAsync();
                Assert.That(responseContent, Is.EqualTo("Deleted successfully!"));
            });
        }

        [Test]
        public Task CommentController_DeleteProperty_ShouldReturnCommentNotFound()
        {
            return PerformTest(async (client) =>
            {
                // Arrange
                string accessToken = await GenerateAccessToken(username: "tenant1");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                int commentId = 9999; // Replace with appropriate test data that does not exist in the database

                // Act
                var response = await client.DeleteAsync($"comment/tenant/{commentId}");
                var responseContent = await response.Content.ReadAsStringAsync();
                var errorResponse = JsonConvert.DeserializeObject<ErrorResponse>(responseContent);

                // Assert
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
                Assert.That(errorResponse.Message, Is.EqualTo("Comment not found."));
            });
        }

        [Test]
        public Task CommentController_DeleteProperty_ShouldReturnForbidden()
        {
            return PerformTest(async (client) =>
            {
                // Arrange
                string accessToken = await GenerateAccessToken(username: "tenant2");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                int commentId = 1; // Replace with appropriate test data that tenant2 does not have access to

                // Act
                var response = await client.DeleteAsync($"comment/tenant/{commentId}");
                var responseContent = await response.Content.ReadAsStringAsync();
                var errorResponse = JsonConvert.DeserializeObject<ErrorResponse>(responseContent);

                // Assert
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
                Assert.That(errorResponse.Message, Is.EqualTo("Access denied. You do not have permission to delete this comment."));
            });
        }
        #endregion
    }
}
