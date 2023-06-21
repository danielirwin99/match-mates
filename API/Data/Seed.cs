using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class Seed
    {
        // Data context lets us pull through the Users
        public static async Task SeedUsers(DataContext context)
        {
            // If there are any Users in our database --> It will stop the execution
            if (await context.Users.AnyAsync()) return;

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
                // Generating the password for the user --> Inside the Quotes is our PASSWORD (same password for each user)
                user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes("Pa$$w0rd"));
                // The PasswordSalt = the hmac key
                user.PasswordSalt = hmac.Key;

                // Adding the user to our context (DataContext)
                context.Users.Add(user);
            }

            // Saves the changes to use in our Database
            await context.SaveChangesAsync();
        }
    }
}