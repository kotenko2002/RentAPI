using Rent.Service.Authorization.Descriptors;

namespace Rent.Service.Authorization
{
    public interface IAuthService
    {
        void Test(LoginDescriptor descriptor);
    }
}
