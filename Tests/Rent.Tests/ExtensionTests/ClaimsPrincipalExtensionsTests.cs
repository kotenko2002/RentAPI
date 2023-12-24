using NUnit.Framework;
using Rent.Common;
using RentAPI.Infrastructure.Extensions;
using System.Security.Claims;

namespace Rent.Tests.ExtensionTests
{
    public class ClaimsPrincipalExtensionsTests
    {
        [Test]
        public void ClaimsPrincipalExtensions_GetUserId_ShouldReturnExpectedValue_WhenUserIdExists()
        {
            // Arrange
            var claimType = "userId";
            var claimValue = "123";
            var claims = new List<Claim> { new Claim(claimType, claimValue) };
            var identity = new ClaimsIdentity(claims);
            var principal = new ClaimsPrincipal(identity);

            // Act
            var result = principal.GetUserId();

            // Assert
            Assert.That(result, Is.EqualTo(claimValue));
        }

        [Test]
        public void ClaimsPrincipalExtensions_GetUsername_ShouldReturnExpectedValue_WhenUsernameExists()
        {
            // Arrange
            var claimType = ClaimTypes.Name;
            var claimValue = "username";
            var claims = new List<Claim> { new Claim(claimType, claimValue) };
            var identity = new ClaimsIdentity(claims);
            var principal = new ClaimsPrincipal(identity);

            // Act
            var result = principal.GetUsername();

            // Assert
            Assert.That(result, Is.EqualTo(claimValue));
        }

        [TestCase("userId")]
        [TestCase(ClaimTypes.Name)]
        public void ClaimsPrincipalExtensions_GetInfoByDataName_ShouldThrowBusinessException_WhenDataDoesNotExist(string claimType)
        {
            // Arrange
            var claims = new List<Claim>();
            var identity = new ClaimsIdentity(claims);
            var principal = new ClaimsPrincipal(identity);

            // Act & Assert
            Assert.Throws<BusinessException>(() => principal.GetUserId());
        }
    }
}
