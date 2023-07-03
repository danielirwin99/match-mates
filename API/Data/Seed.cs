using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class Seed
    {
        // Identity build in Manager Service
        public static async Task SeedUsers(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
        {
            // If there are any Users in our database --> It will stop the execution
            if (await userManager.Users.AnyAsync()) return;

            // Looking inside our json mock data
            var userData = await File.ReadAllTextAsync("Data/UserSeedData.json");

            // Just in case we have made a mistake with casing --> We want to create some options
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            // Deserialize --> go from Json to a C# object
            // Then we need to specify the type of thing we want to deserialize --> List of AppUser
            var users = JsonSerializer.Deserialize<List<AppUser>>(userData);

            // Our roles generated
            var roles = new List<AppRole>
            {
                new AppRole{Name ="Member"},
                new AppRole{Name ="Admin"},
                new AppRole{Name ="Moderator"}
            };

            foreach (var role in roles)
            {
                await roleManager.CreateAsync(role);
            }

            // Looping over the users with a for each loop
            foreach (var user in users)
            {
                // Puts the username lowercase
                user.UserName = user.UserName.ToLower();

                // Our Dummy users credentials
                // Identity returns and saves the changes
                await userManager.CreateAsync(user, "Pa$$w0rd");

                // Adding Member roles to the dummy seeded data
                await userManager.AddToRoleAsync(user, "Member");
            }

            // Admin user
            var admin = new AppUser
            {
                UserName = "admin"
            };

            await userManager.CreateAsync(admin, "Pa$$w0rd");

            // Returns an IEnumerable for our roles
            await userManager.AddToRolesAsync(admin, new[] { "Admin", "Moderator" });
        }
    }
}