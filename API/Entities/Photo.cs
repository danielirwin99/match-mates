using System.ComponentModel.DataAnnotations.Schema;

namespace API.Entities
{
    // Creating a table schema and calling it "Photos" for our DB
    [Table("Photos")]
    public class Photo
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public bool IsMain { get; set; }
        public string PublicId { get; set; }

        // Adding the relationship property
        public int AppUserId { get; set; }
        public AppUser AppUser { get; set; }
    }
}