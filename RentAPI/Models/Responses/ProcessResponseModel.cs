using Rent.Entities.Responses;
using System.ComponentModel.DataAnnotations;

namespace RentAPI.Models.Responses
{
    public class ProcessResponseModel
    {
        [Required]
        public int? ResponseId { get; set; }

        [Required]
        public ResponseStatus? Status { get; set; }
    }
}
