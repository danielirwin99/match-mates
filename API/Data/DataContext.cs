using API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace API.Data
{
    // IdentityDbContext has a built in Users context
    // We need to tell Identity that we are using int for the id (take in the two entities)
    // This means everything we use need to specify that we are using an integer for the id
    public class DataContext : IdentityDbContext<AppUser,
    AppRole, int,
    IdentityUserClaim<int>,
    AppUserRole, IdentityUserLogin<int>,
    IdentityRoleClaim<int>,
    IdentityUserToken<int>>
    {
        // We want to inject this data context into other classes
        public DataContext(DbContextOptions options) : base(options)
        {
        }

        // Our Likes Table
        public DbSet<UserLike> Likes { get; set; }

        // Our Messages Table
        public DbSet<Message> Messages { get; set; }


        // We are overriding OnModelCreating
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<AppUser>()
            // Many to many relationships
            .HasMany(ur => ur.UserRoles)
            // One to many relationship
            .WithOne(u => u.User)
            .HasForeignKey(ur => ur.UserId)
            // Cannot be null
            .IsRequired();

            builder.Entity<AppRole>()
            // Many to many relationships
            .HasMany(ur => ur.UserRoles)
            // One to many relationship
            .WithOne(u => u.Role)
            .HasForeignKey(ur => ur.RoleId)
            // Cannot be null
            .IsRequired();

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