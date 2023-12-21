using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using NUnit.Framework;
using Rent.Service.Services.Authorization.Views;
using Rent.Tests.Helpers;
using RentAPI.Models.Auth;
using System.Net;
using System.Text;

namespace Rent.Tests.IntegrationTests
{
    public class AuthControllerTests
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
        public async Task Register_ReturnsOk()
        {
            var model = new RegisterModel
            {
                Username = "TestUsername",
                Role = "Tenant",
                Email = "testuser1@gmail.com",
                Phone = "0988987776",
                Password = "Qwerty123!"
            };
            var content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("auth/register", content);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(await response.Content.ReadAsStringAsync(), Is.EqualTo("User created successfully!"));
        }

        [Test]
        public async Task Login_ReturnsOk()
        {
            var model = new LoginModel
            {
                Username = "Landlord1",
                Password = "Qwerty123!"
            };
            var content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("auth/login", content);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var responseContent = await response.Content.ReadAsStringAsync();
            var tokens = JsonConvert.DeserializeObject<TokensPairView>(responseContent);

            Assert.That(tokens.AccessToken.Token, Is.Not.Null.Or.Empty);
            Assert.That(tokens.RefreshToken.Token, Is.Not.Null.Or.Empty);
        }

        [Test]
        public async Task RefreshTokens_ReturnsOkAndValidTokens()
        {
            var model = new RefreshTokensModel
            {
                AccessToken = await _helper.GenerateAccessToken(_factory, "landlord1"),
                RefreshToken = "refresh_token"
            };
            var content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("auth/refresh-tokens", content);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var responseContent = await response.Content.ReadAsStringAsync();
            var tokens = JsonConvert.DeserializeObject<TokensPairView>(responseContent);

            Assert.That(tokens.AccessToken.Token, Is.Not.Null.Or.Empty);
            Assert.That(tokens.RefreshToken.Token, Is.Not.Null.Or.Empty);
        }

        [TearDown]
        public void TearDown()
        {
            _client.Dispose();
            _factory.Dispose();
        }
    }
}
