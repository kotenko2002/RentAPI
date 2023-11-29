using Microsoft.AspNetCore.Identity;
using Rent.Entities.Comments;
using Rent.Entities.Properties;
using Rent.Entities.Responses;

namespace Rent.Entities.Users
{
    public class User : IdentityUser
    {
        public string RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }

        public virtual ICollection<Property> Properties { get; set; }
        public virtual ICollection<Response> Responses { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
    }
}
