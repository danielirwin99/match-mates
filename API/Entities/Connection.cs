
namespace API.Entities
{
    public class Connection
    {
        public Connection()
        {
            
        }
        // The schema for our database
        public Connection(string connectionId, string username)
        {
            ConnectionId = connectionId;
            Username = username;
        }

        public string ConnectionId { get; set; }
        public string Username { get; set; }
    }
}