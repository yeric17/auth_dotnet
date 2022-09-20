namespace ThulloWebAPI.Models
{
    public class UserLoginResponse
    {
        public Guid Id { get; set; }
        public string UniqueName { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Avatar { get; set; } = string.Empty;
        public UserStatus Status { get; set; }
        public string? Phone { get; set; } = string.Empty;
        
        public string? AccessToken { get; set; } = string.Empty;
    }
}
