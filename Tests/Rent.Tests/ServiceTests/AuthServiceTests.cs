using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using Rent.Common;
using Rent.Service.Configuration;
using Rent.Service.Services.Authorization.Descriptors;
using Rent.Service.Services.Authorization;
using Rent.Entities.Users;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Rent.Tests.Infrastructure;

namespace Rent.Tests.ServiceTests
{
    public class AuthServiceTests : BaseUnitTest
    {
        private Mock<UserManager<User>> _mockUserManager;
        private IOptions<JwtConfig> _jwtOptions;
        private AuthService _authService;

        [SetUp]
        public void Setup()
        {
            _mockUserManager = new Mock<UserManager<User>>(new Mock<IUserStore<User>>().Object,
                null, null, null, null, null, null, null, null);
            _jwtOptions = Options.Create(new JwtConfig()
            {
                Secret = "xxxxpxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx",
                ValidIssuer = "https://localhost:7023",
                ValidAudience = "https://localhost:7023",
                TokenValidityInMinutes = 15,
                RefreshTokenValidityInDays = 7,
            });
            _authService = new AuthService(_jwtOptions, _mockUserManager.Object);
        }

        [Test]
        public void RegisterAsync_UserExists_ThrowsBusinessException()
        {
            // Arrange
            var descriptor = new RegisterDescriptor { Username = "TestUser" };
            _mockUserManager.Setup(x => x.FindByNameAsync(It.IsAny<string>()))
                .ReturnsAsync(new User());

            // Act & Assert
            Assert.ThrowsAsync<BusinessException>(() => _authService.RegisterAsync(descriptor));
        }

        [Test]
        public void RegisterAsync_CreateFailed_ThrowsBusinessException()
        {
            // Arrange
            var descriptor = new RegisterDescriptor { Username = "TestUser" };
            _mockUserManager.Setup(x => x.FindByNameAsync(It.IsAny<string>()))
                .ReturnsAsync((User)null);
            _mockUserManager.Setup(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Failed());

            // Act & Assert
            Assert.ThrowsAsync<BusinessException>(() => _authService.RegisterAsync(descriptor));
        }

        [Test]
        public void RegisterAsync_SuccessfulRegistration_NoExceptionThrown()
        {
            // Arrange
            var descriptor = new RegisterDescriptor { Username = "TestUser" };
            _mockUserManager.Setup(x => x.FindByNameAsync(It.IsAny<string>()))
                .ReturnsAsync((User)null);
            _mockUserManager.Setup(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            // Act & Assert
            Assert.DoesNotThrowAsync(() => _authService.RegisterAsync(descriptor));
        }

        [Test]
        public void LoginAsync_UserNotFound_ThrowsBusinessException()
        {
            // Arrange
            var descriptor = new LoginDescriptor { Username = "TestUser" };
            _mockUserManager.Setup(x => x.FindByNameAsync(It.IsAny<string>()))
                .ReturnsAsync((User)null);

            // Act & Assert
            Assert.ThrowsAsync<BusinessException>(() => _authService.LoginAsync(descriptor));
        }

        [Test]
        public void LoginAsync_WrongPassword_ThrowsBusinessException()
        {
            // Arrange
            var descriptor = new LoginDescriptor { Username = "TestUser", Password = "TestPassword" };
            _mockUserManager.Setup(x => x.FindByNameAsync(It.IsAny<string>()))
                .ReturnsAsync(new User());
            _mockUserManager.Setup(x => x.CheckPasswordAsync(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(false);

            // Act & Assert
            Assert.ThrowsAsync<BusinessException>(() => _authService.LoginAsync(descriptor));
        }

        [Test]
        public async Task LoginAsync_SuccessfulLogin_ReturnsExpectedTokensPairView()
        {
            // Arrange
            var descriptor = new LoginDescriptor { Username = "TestUser", Password = "TestPassword" };
            var user = new User { Id = "TestUserId", UserName = "TestUser" };
            _mockUserManager.Setup(x => x.FindByNameAsync(It.IsAny<string>()))
                .ReturnsAsync(user);
            _mockUserManager.Setup(x => x.CheckPasswordAsync(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(true);
            _mockUserManager.Setup(x => x.GetRolesAsync(It.IsAny<User>()))
                .ReturnsAsync(new List<string> { "TestRole" });
            _mockUserManager.Setup(x => x.UpdateAsync(It.IsAny<User>()))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _authService.LoginAsync(descriptor);

            // Assert
            Assert.IsNotNull(result);
            _mockUserManager.Verify(x => x.FindByNameAsync(descriptor.Username), Times.Once);
            _mockUserManager.Verify(x => x.CheckPasswordAsync(user, descriptor.Password), Times.Once);
            _mockUserManager.Verify(x => x.GetRolesAsync(user), Times.Once);
            _mockUserManager.Verify(x => x.UpdateAsync(user), Times.Once);
        }

        //[Test]
        //public void RefreshTokensAsync_InvalidTokens_ThrowsBusinessException()
        //{
        //    // Arrange
        //    var descriptor = new RefreshTokensDescriptor { AccessToken = "InvalidAccessToken", RefreshToken = "InvalidRefreshToken" };

        //    // Act & Assert
        //    Assert.ThrowsAsync<BusinessException>(() => _authService.RefreshTokensAsync(descriptor));
        //}

        //[Test]
        //public void RefreshTokensAsync_UserNotFound_ThrowsBusinessException()
        //{
        //    // Arrange
        //    var descriptor = new RefreshTokensDescriptor { AccessToken = "ValidAccessToken", RefreshToken = "ValidRefreshToken" };
        //    _mockUserManager.Setup(x => x.FindByNameAsync(It.IsAny<string>()))
        //        .ReturnsAsync((User)null);

        //    // Act & Assert
        //    Assert.ThrowsAsync<BusinessException>(() => _authService.RefreshTokensAsync(descriptor));
        //}

        //[Test]
        //public void RefreshTokensAsync_SuccessfulRefresh_NoExceptionThrown()
        //{
        //    // Arrange
        //    var descriptor = new RefreshTokensDescriptor { AccessToken = "ValidAccessToken", RefreshToken = "ValidRefreshToken" };
        //    var user = new User { Id = "TestUserId", UserName = "TestUser", RefreshToken = "ValidRefreshToken", RefreshTokenExpiryTime = DateTime.Now.AddDays(1) };
        //    _mockUserManager.Setup(x => x.FindByNameAsync(It.IsAny<string>()))
        //        .ReturnsAsync(user);

        //    // Act & Assert
        //    Assert.DoesNotThrowAsync(() => _authService.RefreshTokensAsync(descriptor));
        //}

        [Test]
        public void RefreshTokensAsync_UserNotFound_ThrowsBusinessException()
        {
            // Arrange
            var descriptor = new RefreshTokensDescriptor { AccessToken = GenerateValidJwtToken(), RefreshToken = "ValidRefreshToken" };
            _mockUserManager.Setup(x => x.FindByNameAsync(It.IsAny<string>()))
                .ReturnsAsync((User)null);

            // Act & Assert
            var ex = Assert.ThrowsAsync<BusinessException>(() => _authService.RefreshTokensAsync(descriptor));
            Assert.That(ex.Message, Is.EqualTo("Invalid access token or refresh token"));
        }

        //[Test]
        //public async Task RefreshTokensAsync_SuccessfulRefresh_ReturnsExpectedTokensPairView()
        //{
        //    // Arrange
        //    var descriptor = new RefreshTokensDescriptor { AccessToken = GenerateValidJwtToken(), RefreshToken = "ValidRefreshToken" };
        //    var user = new User { Id = "TestUserId", UserName = "TestUser", RefreshToken = "ValidRefreshToken", RefreshTokenExpiryTime = DateTime.Now.AddDays(1) };
        //    _mockUserManager.Setup(x => x.FindByNameAsync(It.IsAny<string>()))
        //        .ReturnsAsync(user);

        //    // Act
        //    var result = await _authService.RefreshTokensAsync(descriptor);

        //    // Assert
        //    Assert.IsNotNull(result);
        //    //Assert.AreEqual(user.Id, result.User.Id);
        //    _mockUserManager.Verify(x => x.FindByNameAsync(user.UserName), Times.Once);
        //    _mockUserManager.Verify(x => x.UpdateAsync(user), Times.Once);
        //}

        //[Test]
        //public void RefreshTokensAsync_InvalidTokens_ThrowsBusinessException()
        //{
        //    // Arrange
        //    var descriptor = new RefreshTokensDescriptor { AccessToken = "InvalidAccessToken", RefreshToken = "InvalidRefreshToken" };

        //    // Act & Assert
        //    var ex = Assert.ThrowsAsync<BusinessException>(() => _authService.RefreshTokensAsync(descriptor));
        //    Assert.That(ex.Message, Is.EqualTo("Invalid access token or refresh token"));
        //}

        private string GenerateValidJwtToken()
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, "TestUser"),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Value.Secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddMinutes(Convert.ToDouble(_jwtOptions.Value.TokenValidityInMinutes));

            var token = new JwtSecurityToken(
                _jwtOptions.Value.ValidIssuer,
                _jwtOptions.Value.ValidAudience,
                claims,
                expires: expires,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}
