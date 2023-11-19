using System.ComponentModel.DataAnnotations;

namespace RentAPI.Models.Auth
{
    public class RefreshTokensModel
    {
        [Required]
        public string AccessToken { get; set; }

        [Required]
        public string RefreshToken { get; set; }
    }
}
