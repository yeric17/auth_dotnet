namespace ThulloWebAPI.Models
{
    public class DefaultResponse
    {
        public string Message { get; set; } = string.Empty;
        public DefaultResponse(string message)
        {
            Message = message;
        }

    }

    public interface IJSONData { }
}
