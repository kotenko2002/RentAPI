using Rent.Entities.Comments;
using System.ComponentModel.DataAnnotations;

namespace RentAPI.Models.Comments
{
    public class AddCommentModel
    {
        [Required]
        public string TenantId { get; set; }

        [Required]
        public int? PropertyId { get; set; }

        [Required]
        [StringLength(400)]
        public string Message { get; set; }

        [Required]
        public Rate Rate { get; set; }
    }
}
