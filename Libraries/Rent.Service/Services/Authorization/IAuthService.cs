using Rent.Service.Services.Authorization.Descriptors;
using Rent.Service.Services.Authorization.Views;

namespace Rent.Service.Services.Authorization
{
    public interface IAuthService
    {
        Task Register(RegisterDescriptor descriptor);
        Task<TokensPairView> Login(LoginDescriptor descriptor);
        Task<TokensPairView> RefreshTokens(RefreshTokensDescriptor descriptor);
        Task Logout(string username);
    }
}
