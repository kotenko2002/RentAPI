using Rent.Service.Authorization.Descriptors;

namespace Rent.Service.Authorization
{
    public class AuthService : IAuthService
    {
        public void Test(LoginDescriptor descriptor)
        {
            Console.WriteLine($"Username: {descriptor.Username}, Password: {descriptor.Password}");
        }
    }
}
