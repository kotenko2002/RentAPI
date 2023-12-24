using Newtonsoft.Json;
using NUnit.Framework;
using Rent.Service.Services.Authorization.Views;
using Rent.Tests.Infrastructure;
using RentAPI.Infrastructure.Middlewares;
using RentAPI.Models.Auth;
using System.Net;
using System.Net.Http.Headers;
using System.Text;

namespace Rent.Tests.IntegrationTests
{
    public class AuthControllerTests : BaseIntegrationTest
    {
        #region Register
        [Test]
        public Task  AuthController_Register_ShouldReturnOk()
        {
            return PerformTest(async (client) =>
            {
                // Arrange
                var model = new RegisterModel
                {
                    Username = "TestUsername",
                    Role = "Tenant",
                    Email = "testuser1@gmail.com",
                    Phone = "0988987776",
                    Password = "Qwerty123!"
                };
                var content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");

                // Act
                var response = await client.PostAsync("auth/register", content);

                var responseContent = await response.Content.ReadAsStringAsync();

                // Assert
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(responseContent, Is.EqualTo("User created successfully!"));
            });
        }

        [Test]
        public Task  AuthController_Register_ShouldReturnUserAlreadyExists()
        {
            return PerformTest(async (client) =>
            {
                // Arrange
                var model = new RegisterModel
                {
                    Username = "landlord1",
                    Role = "Tenant",
                    Email = "testuser1@gmail.com",
                    Phone = "0988987776",
                    Password = "Qwerty123!"
                };
                var content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");

                // Act
                var response = await client.PostAsync("auth/register", content);

                var responseContent = await response.Content.ReadAsStringAsync();
                var errorResponse = JsonConvert.DeserializeObject<ErrorResponse>(responseContent);

                // Assert
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Conflict));
                Assert.That(errorResponse.Message, Is.EqualTo("User already exists!"));
            });
        }
        #endregion

        #region Login
        [Test]
        public Task  AuthController_Login_ShouldReturnOk()
        {
            return PerformTest(async (client) =>
            {
                // Arrange
                var model = new LoginModel
                {
                    Username = "Landlord1",
                    Password = "Qwerty123!"
                };
                var content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");

                // Act
                var response = await client.PostAsync("auth/login", content);

                var responseContent = await response.Content.ReadAsStringAsync();
                var tokens = JsonConvert.DeserializeObject<TokensPairView>(responseContent);

                // Assert
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(tokens.AccessToken.Token, Is.Not.Null.Or.Empty);
                Assert.That(tokens.RefreshToken.Token, Is.Not.Null.Or.Empty);
            });
        }

        [TestCase("Landlord1", "wrongPassword")]
        [TestCase("wrongUsername", "Qwerty123!")]
        public Task  AuthController_Login_ShouldReturnWrongUsernameOrPassword(string username, string password)
        {
            return PerformTest(async (client) =>
            {
                // Arrange
                var model = new LoginModel
                {
                    Username = username,
                    Password = password
                };
                var content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");

                // Act
                var response = await client.PostAsync("auth/login", content);

                var responseContent = await response.Content.ReadAsStringAsync();
                var errorResponse = JsonConvert.DeserializeObject<ErrorResponse>(responseContent);

                // Assert
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
                Assert.That(errorResponse.Message, Is.EqualTo("Wrong username or password"));
            });
        }
        #endregion

        #region RefreshTokens
        [Test]
        public Task  AuthController_RefreshTokens_ShouldReturnOk()
        {
            return PerformTest(async (client) =>
            {
                // Arrange
                var model = new RefreshTokensModel
                {
                    AccessToken = await GenerateAccessToken(username: "landlord1"),
                    RefreshToken = "refresh_token"
                };
                var content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");

                // Act
                var response = await client.PostAsync("auth/refresh-tokens", content);

                var responseContent = await response.Content.ReadAsStringAsync();
                var tokens = JsonConvert.DeserializeObject<TokensPairView>(responseContent);

                // Assert
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(tokens.AccessToken.Token, Is.Not.Null.Or.Empty);
                Assert.That(tokens.RefreshToken.Token, Is.Not.Null.Or.Empty);
            });
        }

        [Test]
        public Task  AuthController_RefreshTokens_ShouldReturnInvalidAccessTokenOrRefreshToken()
        {
            return PerformTest(async (client) =>
            {
                // Arrange
                var model = new RefreshTokensModel
                {
                    AccessToken = await GenerateAccessToken(username: "landlord1"),
                    RefreshToken = "wrongRefreshToken"
                };
                var content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");

                // Act
                var response = await client.PostAsync("auth/refresh-tokens", content);

                var responseContent = await response.Content.ReadAsStringAsync();
                var errorResponse = JsonConvert.DeserializeObject<ErrorResponse>(responseContent);

                // Assert
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                Assert.That(errorResponse.Message, Is.EqualTo("Invalid access token or refresh token"));
            });
        }
        #endregion

        #region Logout
        [Test]
        public Task  AuthController_Logout_ShouldReturnOk()
        {
            return PerformTest(async (client) =>
            {
                string accessToken = await GenerateAccessToken(username: "landlord1");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                // Act
                var response = await client.DeleteAsync($"auth/logout");

                var responseContent = await response.Content.ReadAsStringAsync();

                // Assert
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            });
        }

        [Test]
        public Task  AuthController_Logout_ShouldReturnInvalidAccessToken()
        {
            return PerformTest(async (client) =>
            {
                string accessToken = await GenerateAccessToken(username: "landlord1", fakeUsername: "fakeUsername");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                // Act
                var response = await client.DeleteAsync($"auth/logout");

                var responseContent = await response.Content.ReadAsStringAsync();
                var errorResponse = JsonConvert.DeserializeObject<ErrorResponse>(responseContent);

                // Assert
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                Assert.That(errorResponse.Message, Is.EqualTo("Invalid access token"));
            });
        }
        #endregion
    }
}
