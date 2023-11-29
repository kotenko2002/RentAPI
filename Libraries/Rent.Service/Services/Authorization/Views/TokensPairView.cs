using Rent.Entities.Users;
using System.IdentityModel.Tokens.Jwt;

namespace Rent.Service.Services.Authorization.Views
{
    public class TokensPairView
    {
        public TokenView AccessToken { get; set; }
        public TokenView RefreshToken { get; set; }

        public TokensPairView(JwtSecurityToken accessToken, User user)
        {
            AccessToken = new TokenView()
            {
                Token = new JwtSecurityTokenHandler().WriteToken(accessToken),
                ExpirationDate = accessToken.ValidTo
            };

            RefreshToken = new TokenView()
            {
                Token = user.RefreshToken,
                ExpirationDate = user.RefreshTokenExpiryTime
            };
        }
    }
}
