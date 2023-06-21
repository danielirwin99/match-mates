namespace API.DTOs
{
    public class MemberDTO
    {
        public int Id { get; set; }

        public string UserName { get; set; }

        // Main Photo
        public string PhotoUrl { get; set; }

        // Date of birth for the User
        public int Age { get; set; }

        // Nickname
        public string KnownAs { get; set; }

        // Date created
        public DateTime Created { get; set; }

        // When the user was last active
        public DateTime LastActive { get; set; }

        public string Gender { get; set; }

        // Introduction to the User
        public string Introduction { get; set; }

        // What the User is looking for
        public string LookingFor { get; set; }
        public string Interests { get; set; }
        public string City { get; set; }

        public string Country { get; set; }


        // List of user Photos
        public List<PhotoDTO> Photos { get; set; }
    }
}