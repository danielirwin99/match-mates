namespace API.DTOs
{
    public class RegisterDTO
    {
        // These will bind and become lowercase
        public string Username { get; set; }
        public string Password { get; set; }
    }
}