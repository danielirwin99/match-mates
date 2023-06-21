
using API.Extensions;

namespace API.Entities
{
    public class AppUser
    {
        // Id is our Primary Key
        public int Id { get; set; }

        // Our Username for each of the users
        public string UserName { get; set; }

        // Our Salt and Hashed password
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }

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

        // List of user Photos
        public List<Photo> Photos { get; set; } = new();

        public int GetAge()
        {
            // Imported from our API.Extensions Folder
            return DateOfBirth.CalculateAge();
        }



    }
}