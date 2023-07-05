using System.ComponentModel.DataAnnotations;

namespace API.Entities
{
    public class Group
    {
        public Group()
        {
            
        }
        public Group(string name)
        {
            Name = name;
        }

        // This name is our Primary key (its unique)
        [Key]
        // Group Name
        public string Name { get; set; }

        public ICollection<Connection> Connections { get; set; } = new List<Connection>();
    }
}