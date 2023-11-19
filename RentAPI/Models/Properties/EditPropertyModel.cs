using Rent.Entities.Properties;
using System.ComponentModel.DataAnnotations;

namespace RentAPI.Models.Properties
{
    public class EditPropertyModel
    {
        [Required]
        public int? Id { get; set; }

        public int? CityId { get; set; }

        [StringLength(100)]
        public string Address { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        [Range(1, int.MaxValue)]
        public int? Price { get; set; }

        public PropertyStatus? Status { get; set; }
    }
}
