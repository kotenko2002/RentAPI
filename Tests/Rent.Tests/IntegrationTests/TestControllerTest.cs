using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using NUnit.Framework;

namespace Rent.Tests.IntegrationTests
{
    public class TestControllerTest
    {
        [Test]
        public async Task Test_test()
        {
            var webAppFactory = new WebApplicationFactory<RentAPI.Program>();
            var httpClient = webAppFactory.CreateDefaultClient();

            var response = await httpClient.GetAsync("/Test");
            var stingResult = await response.Content.ReadAsStringAsync();

            Assert.That(stingResult, Is.EqualTo("ok"));
        }
    }
}
