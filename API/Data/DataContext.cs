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

        // Our Likes Table
        public DbSet<UserLike> Likes { get; set; }

        // Our Messages Table
        public DbSet<Message> Messages { get; set; }


        // We are overriding OnModelCreating
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // We are targeting the entity we are interested in --> UserLike
            builder.Entity<UserLike>()
            // How we are specifying what its primary key is
            .HasKey(k => new { k.SourceUserId, k.TargetUserId });

            builder.Entity<UserLike>()
            // This is the User
            .HasOne(s => s.SourceUser)
            // This lets us as the User like other users
            .WithMany(l => l.LikedUsers)
            .HasForeignKey(s => s.SourceUserId)
            // Deleting likes
            .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<UserLike>()
            // This is the User we are targeting to like
            .HasOne(s => s.TargetUser)
            .WithMany(l => l.LikedByUsers)
            .HasForeignKey(s => s.TargetUserId)
            .OnDelete(DeleteBehavior.NoAction);

            // Message controls for Recipient / Receiver
            builder.Entity<Message>()
            .HasOne(u => u.Recipient)
            .WithMany(m => m.MessagesReceived)
            // Stops the message being deleted from the database
            .OnDelete(DeleteBehavior.Restrict);

            // Message controls for Sender
            builder.Entity<Message>()
            .HasOne(u => u.Sender)
            .WithMany(m => m.MessagesSent)
            // Stops the message being deleted from the database
            .OnDelete(DeleteBehavior.Restrict);
        }
    }
}