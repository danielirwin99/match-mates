using API.Extensions;
using Microsoft.AspNetCore.Identity;

namespace API.Entities
{
    // Derived from our IdentityUser class
    public class AppUser : IdentityUser<int>
    {

        // Date of birth for the User
        public DateOnly DateOfBirth { get; set; }

        // Nickname
        public string KnownAs { get; set; }

        // Date created
        public DateTime Created { get; set; } = DateTime.UtcNow;

        // When the user was last active
        public DateTime LastActive { get; set; } = DateTime.UtcNow;

        public string Gender { get; set; }

        // Introduction to the User
        public string Introduction { get; set; }

        // What the User is looking for
        public string LookingFor { get; set; }
        public string Interests { get; set; }
        public string City { get; set; }
        public string Country { get; set; }

        // List of user Photos
        public List<Photo> Photos { get; set; } = new();

        public int GetAge()
        {
            // Imported from our API.Extensions Folder
            return DateOfBirth.CalculateAge();
        }

        public List<UserLike> LikedByUsers { get; set; }
        public List<UserLike> LikedUsers { get; set; }

        // Creates a List for each of these values
        public List<Message> MessagesSent { get; set; }
        public List<Message> MessagesReceived { get; set; }

        // Navigation property to our join table
        public ICollection<AppUserRole> UserRoles { get; set; }

    }
}