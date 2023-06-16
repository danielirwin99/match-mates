using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly DataContext _context;
        public AccountController(DataContext context)
        {
            _context = context;
        }

        // Post method
        [HttpPost("register")] // api/account/register
        public async Task<ActionResult<AppUser>> Register(string username, string password)
        // This is the method to create a RNG key for our SALT PASSWORD
        {
            using var hmac = new HMACSHA512();

            var user = new AppUser
            {
                UserName = username,
                // Connects the has to the password we write
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password)),
                // Randomly generates a key
                PasswordSalt = hmac.Key
            };
            // Telling the database to add the user
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return user;
        }
    }
}