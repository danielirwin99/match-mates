using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly DataContext _context;
        private readonly ITokenService _tokenService;
        public AccountController(DataContext context, ITokenService tokenService)
        {
            // This is what we write inside our API --> i.e typing up the username and password
            _context = context;
            _tokenService = tokenService;
        }
        // --------------------
        // REGISTER POST METHOD
        // --------------------
        [HttpPost("register")] // api/account/register
        public async Task<ActionResult<UserDTO>> Register(RegisterDTO registerDTO)
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

            // If register is successful return this
            return new UserDTO
            {
                Username = user.UserName,
                Token = _tokenService.CreateToken(user),
                // Our Main Photo IF there is one
                PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain)?.Url
            };
        }
        // ---------------------
        // LOGIN POST METHOD
        // ---------------------
        [HttpPost("login")]
        public async Task<ActionResult<UserDTO>> Login(LoginDTO loginDTO)
        {
            // Looking for our user
            // SingleorDefault --> If we don't find the user in our DB --> We get NULL back (Default Value)
            var user = await _context.Users
            .Include(p => p.Photos)
            .SingleOrDefaultAsync(x => x.UserName == loginDTO.Username);

            // If there is no user return this
            if (user == null) return Unauthorized("Invalid Username");

            // We need to pass in the key we get back i.e the password
            using var hmac = new HMACSHA512(user.PasswordSalt);

            // What our user enters for the password --> Converted into a hash
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDTO.Password));

            // Need to loop over the hashes looking for the correct one
            for (int i = 0; i < computedHash.Length; i++)
            {
                // If the index of computedHash is not equal to the index we are looking for
                if (computedHash[i] != user.PasswordHash[i])
                {
                    // If it does not equal return this
                    return Unauthorized("Invalid Password");
                }

            }
            // If the hash entered for the login and the database hash are equal --> return the User --> valid entry
            return new UserDTO
            {
                Username = user.UserName,
                Token = _tokenService.CreateToken(user),
                PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain)?.Url
            };
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