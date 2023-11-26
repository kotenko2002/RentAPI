using Rent.Entities.Properties;

namespace Rent.Entities.Photos
{
    public class Photo 
    {
        public string Id { get; set; }
        public int PropertyId { get; set; }
        public virtual Property Property { get; set; }
    }
}
