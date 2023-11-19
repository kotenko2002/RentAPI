using Microsoft.AspNetCore.Identity;
using Rent.Entities.Properties;

namespace Rent.Entities.Users
{
    public class User : IdentityUser
    {
        public string RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }

        public virtual ICollection<Property> Properties { get; set; }
    }
}
