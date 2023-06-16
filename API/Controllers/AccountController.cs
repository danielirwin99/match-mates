using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTOs;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
        public async Task<ActionResult<AppUser>> Register(RegisterDTO registerDTO)
        // This is the method to create a RNG key for our SALT PASSWORD
        {
            // If there is a User in the system --> 
            if (await UserExists(registerDTO.Username)) return BadRequest("Username is Taken");

            using var hmac = new HMACSHA512();

            var user = new AppUser
            {
                // We are getting our Username from the registerDTO file
                UserName = registerDTO.Username,

                // Same as Username where we are getting the password
                // Connects the hash to the password we write
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDTO.Password)),

                // Randomly generates a key
                PasswordSalt = hmac.Key
            };
            
            // Telling the database to add the user
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return user;
        }

        // Checking to see if the User Exists
        private async Task<Boolean> UserExists(string username)
        {
            // This asynchronously determines whether a sequence contains any elements 
            // --> i Any Existing Users
            // ToLower checks the user in lowercase
            return await _context.Users.AnyAsync(user => user.UserName == username.ToLower());
        }
    }
}