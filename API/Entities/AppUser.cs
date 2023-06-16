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
    }
}