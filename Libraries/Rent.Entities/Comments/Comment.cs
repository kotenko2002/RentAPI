using Rent.Entities.Properties;
using Rent.Entities.Users;

namespace Rent.Entities.Comments
{
    public class Comment : BaseEntity
    {
        public int UserId { get; set; }
        public virtual User Tenant { get; set; }

        public int PropertyId { get; set; }
        public virtual Property City { get; set; }

        public string Message { get; set; }
        public Rate Rate { get; set; }
    }
}
