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
        public static async Task SeedUsers(UserManager<AppUser> userManager)
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

            // Looping over the users with a for each loop
            foreach (var user in users)
            {
                // Our randomly generated key format for the users
                using var hmac = new HMACSHA512();

                // Puts the username lowercase
                user.UserName = user.UserName.ToLower();

                // Our Dummy users credentials
                // Identity returns and saves the changes
                await userManager.CreateAsync(user, "Pa$$w0rd");
            }
        }
    }
}