using Rent.Entities.Properties;

namespace Rent.Entities.Cities
{
    public class City : BaseEntity
    {
        public string Name { get; set; }

        public virtual ICollection<Property> Properties { get; set; }
    }
}
