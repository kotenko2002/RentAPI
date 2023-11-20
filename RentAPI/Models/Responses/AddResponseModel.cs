using Rent.Entities.Responses;
using System.ComponentModel.DataAnnotations;

namespace RentAPI.Models.Responses
{
    public class AddResponseModel
    {
        [Required]
        public int? PropertyId { get; set; }

        [Required]
        [StringLength(400)]
        public string Message { get; set; }

        [Required]
        public ResponseStatus? Status { get; set; }
    }
}
