using Rent.Entities.Properties;
using Rent.Entities.Users;

namespace Rent.Entities.Responses
{
    public class Response : BaseEntity
    {
        public string UserId { get; set; }
        public virtual User Tenant { get; set; }

        public int PropertyId { get; set; }
        public virtual Property City { get; set; }

        public string Message { get; set; }
        public ResponseStatus Status { get; set; }
    }
}
