using System.ComponentModel.DataAnnotations;

namespace API.DTOs
{
    public class RegisterDTO
    {
        // Required makes sure you cant use an empty string for your entries --> Sends an error
        [Required]
        // These will bind and become lowercase
        public string Username { get; set; }
        [Required] public string KnownAs { get; set; }
        [Required] public string Gender { get; set; }
        [Required] public DateOnly? DateOfBirth { get; set; } // Optional to make required work
        [Required] public string City { get; set; }
        [Required] public string Country { get; set; }

        [Required]
        [StringLength(8, MinimumLength = 5)]
        public string Password { get; set; }
    }
}