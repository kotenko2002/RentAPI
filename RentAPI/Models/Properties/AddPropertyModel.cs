using Rent.Entities.Properties;
using System.ComponentModel.DataAnnotations;

namespace RentAPI.Models.Properties
{
    public class AddPropertyModel
    {
        [Required]
        public int? CityId { get; set; }

        [Required]
        [StringLength(100)]
        public string Address { get; set; }

        [Required]
        [StringLength(500)]
        public string Description { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int? Price { get; set; }

        [Required]
        public PropertyStatus Status { get; set; }
    }
}
