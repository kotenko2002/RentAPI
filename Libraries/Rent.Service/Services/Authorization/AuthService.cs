using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Rent.Common;
using Rent.Entities.Users;
using Rent.Service.Configuration;
using Rent.Service.Services.Authorization.Descriptors;
using Rent.Service.Services.Authorization.Views;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Rent.Service.Services.Authorization
{
    public class AuthService : IAuthService
    {
        private readonly JwtOptions _jwtOptions;
        private readonly UserManager<User> _userManager;

        public AuthService(
            IOptions<JwtOptions> jwtOptions,
            UserManager<User> userManager)
        {
            _jwtOptions = jwtOptions.Value;
            _userManager = userManager;
        }
        
        public async Task Register(RegisterDescriptor descriptor)
        {
            User existsUser = await _userManager.FindByNameAsync(descriptor.Username);
            if (existsUser != null)
            {
                throw new BusinessException(HttpStatusCode.Conflict, "User already exists!");
            }

            var user = new User()
            {
                UserName = descriptor.Username,
                Email = descriptor.Email,
                PhoneNumber = descriptor.Phone,
                SecurityStamp = Guid.NewGuid().ToString(),
            };

            IdentityResult result = await _userManager.CreateAsync(user, descriptor.Password);
            if (!result.Succeeded)
            {
                string message = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new BusinessException(HttpStatusCode.InternalServerError, message);
            }

            await _userManager.AddToRoleAsync(user, descriptor.Role);
        }

        public async Task<TokensPairView> Login(LoginDescriptor descriptor)
        {
            User user = await _userManager.FindByNameAsync(descriptor.Username);
            if (user == null || !await _userManager.CheckPasswordAsync(user, descriptor.Password))
            {
                throw new BusinessException(HttpStatusCode.Unauthorized, "Wrong username or password");
            }

            IList<string> userRoles = await _userManager.GetRolesAsync(user);

            var authClaims = new List<Claim>
            {
                new Claim("userId", user.Id),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };
            authClaims.AddRange(userRoles.Select(role => new Claim(ClaimTypes.Role, role)));

            JwtSecurityToken accessToken = GenerateAccessToken(authClaims);
            string refreshToken = GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.Now.AddDays(_jwtOptions.RefreshTokenValidityInDays);
            await _userManager.UpdateAsync(user);

            return new TokensPairView(accessToken, user);
        }

        public async Task<TokensPairView> RefreshTokens(RefreshTokensDescriptor descriptor)
        {
            string accessToken = descriptor.AccessToken;
            string refreshToken = descriptor.RefreshToken;

            ClaimsPrincipal principal = GetPrincipalFromExpiredToken(accessToken);
            if (principal == null)
            {
                throw new BusinessException(HttpStatusCode.BadRequest, "Invalid access token or refresh token");
            }

            string username = principal.Identity.Name;
            User user = await _userManager.FindByNameAsync(username);

            if (user == null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
            {
                throw new BusinessException(HttpStatusCode.BadRequest, "Invalid access token or refresh token");
            }

            JwtSecurityToken newAccessToken = GenerateAccessToken(principal.Claims.ToList());
            string newRefreshToken = GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiryTime = DateTime.Now.AddDays(_jwtOptions.RefreshTokenValidityInDays);
            await _userManager.UpdateAsync(user);

            return new TokensPairView(newAccessToken, user);
        }

        public async Task Logout(string username)
        {
            User user = await _userManager.FindByNameAsync(username);
            if (user == null)
            {
                throw new BusinessException(HttpStatusCode.BadRequest, "Invalid access token");
            }

            user.RefreshToken = null;
            await _userManager.UpdateAsync(user);
        }

        private JwtSecurityToken GenerateAccessToken(List<Claim> authClaims)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Secret));

            return new JwtSecurityToken(
                issuer: _jwtOptions.ValidIssuer,
                audience: _jwtOptions.ValidAudience,
                expires: DateTime.Now.AddMinutes(_jwtOptions.TokenValidityInMinutes),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );
        }
        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
        private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Secret)),
                ValidateLifetime = false
            };

            ClaimsPrincipal principal = new JwtSecurityTokenHandler()
                .ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);

            if (securityToken is not JwtSecurityToken jwtSecurityToken ||
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token");
            }

            return principal;
        }
    }
}
