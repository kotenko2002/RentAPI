using Rent.Entities.Users;
using System.ComponentModel.DataAnnotations;

namespace RentAPI.Models.Auth
{
    public class RegisterModel
    {
        [Required(ErrorMessage = "User Name is required")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Role is required")]
        [RegularExpression($"({Roles.Tenant}|{Roles.Landlord})", ErrorMessage = $"Role must be either '{Roles.Tenant}' or '{Roles.Landlord}'")]
        public string Role { get; set; }

        [EmailAddress]
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; }

        [Phone]
        [Required(ErrorMessage = "Phone is required")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
    }
}
