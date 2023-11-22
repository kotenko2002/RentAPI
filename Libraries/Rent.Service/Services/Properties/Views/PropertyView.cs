using Rent.Entities.Properties;

namespace Rent.Service.Services.Properties.Views
{
    public class PropertyView
    {
        public int Id { get; set; }
        public int CityId { get; set; }
        public string Address { get; set; }
        public int Price { get; set; }
        public PropertyStatus Status { get; set; }
        //add one photo
    }
}
