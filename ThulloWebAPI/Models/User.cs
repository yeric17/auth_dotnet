using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ThulloWebAPI.Models
{
    
    public class User: IJSONData
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        
        public Guid Id { get; set; }

        [Index(IsUnique = true)]
        public string UniqueName { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        [Index(IsUnique = true)]
        public string Email { get; set; } = string.Empty;
        public byte[] PasswordHash { get; set; } = new byte[32];
        public byte[] PasswordSalt { get; set; } = new byte[32];
        public string? Avatar { get; set; } = string.Empty;
        public UserStatus Status { get; set; }
        public string? Phone { get; set; } = string.Empty;

        public List<Token> Tokens { get; set; } = new List<Token>();

    }

    public enum UserStatus
    {
        Active = 1,
        Inactive,
        NotConfirmEmail,
    }
}
