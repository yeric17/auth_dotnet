using System.ComponentModel.DataAnnotations;

namespace ThulloWebAPI.Models
{
    public class UserRegisterRequest
    {
        [Required, MaxLength(45)]
        public string UniqueName { get; set; } = string.Empty;
        [Required, MaxLength(45)]
        public string Name { get; set; } = string.Empty;
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;
        [Required, MinLength(8)]
        public string Password { get; set; } = string.Empty;
    }
}
