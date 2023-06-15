using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class DataContext : DbContext
    {
        // We want to inject this data context into other classes
        public DataContext(DbContextOptions options) : base(options)
        {
        }
        // Users = Our Table Name
        public DbSet<AppUser> Users { get; set; }
    }
}