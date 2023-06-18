
using System.ComponentModel.DataAnnotations;

namespace API.DTOs
{
    public class LoginDTO
    {
        [Required]
        // These will bind and become lowercase
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
    }
}