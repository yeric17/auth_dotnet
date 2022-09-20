using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ThulloWebAPI.Models
{
    public class Token
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid TokenId { get; set; }
        [Required]
        public string TokenValue { get; set; } = string.Empty;
        public TokenType TokenType { get; set; }

        public TokenStatus TokenStatus { get; set; }
        public Guid UserId { get; set; }

        public DateTime? Expires { get; set; }
    }

    public enum TokenType
    {
        AccessToken = 1,
        RefreshToken,
        EmailConfim,
    }

    public enum TokenStatus
    {
        Active = 1,
        Inactive,
    }
}
