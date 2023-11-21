using Rent.Entities.Properties;
using Rent.Entities.Users;

namespace Rent.Entities.Responses
{
    public class Response : BaseEntity
    {
        public string TenantId { get; set; }
        public virtual User Tenant { get; set; }

        public int PropertyId { get; set; }
        public virtual Property Property { get; set; }

        public string Message { get; set; }
        public ResponseStatus Status { get; set; }
    }
}
