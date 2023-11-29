using Rent.Entities.Cities;
using Rent.Entities.Comments;
using Rent.Entities.Photos;
using Rent.Entities.Responses;
using Rent.Entities.Users;

namespace Rent.Entities.Properties
{
    public class Property : BaseEntity
    {
        public string LandlordId { get; set; }
        public virtual User Landlord { get; set; }

        public int? CityId { get; set; }
        public virtual City City { get; set; }

        public string Address { get; set; }
        public string Description { get; set; }
        public int Price { get; set; }
        public PropertyStatus Status { get; set; }

        public virtual ICollection<Photo> Photos { get; set; }
        public virtual ICollection<Response> Responses { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
    }
}
