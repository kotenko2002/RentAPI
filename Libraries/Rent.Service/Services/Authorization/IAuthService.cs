using Rent.Service.Services.Authorization.Descriptors;
using Rent.Service.Services.Authorization.Views;

namespace Rent.Service.Services.Authorization
{
    public interface IAuthService
    {
        Task RegisterAsync(RegisterDescriptor descriptor);
        Task<TokensPairView> LoginAsync(LoginDescriptor descriptor);
        Task<TokensPairView> RefreshTokensAsync(RefreshTokensDescriptor descriptor);
        Task LogoutAsync(string username);
    }
}
