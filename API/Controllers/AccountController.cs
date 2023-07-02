using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly ITokenService _tokenService;
        public IMapper _mapper;
        private readonly UserManager<AppUser> _userManager;

        // Using UserManager Identity
        public AccountController(UserManager<AppUser> userManager, ITokenService tokenService, IMapper mapper)
        {
            _userManager = userManager;
            _mapper = mapper;
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

            var user = _mapper.Map<AppUser>(registerDTO);

            // We are getting our Username from the registerDTO file
            user.UserName = registerDTO.Username.ToLower();

            // Saves the user into our database
            var result = await _userManager.CreateAsync(user, registerDTO.Password);

            // Checking to see if the result was successful
            if (!result.Succeeded) return BadRequest(result.Errors);

            // If register is successful return this
            return new UserDTO
            {
                Username = user.UserName,
                Token = _tokenService.CreateToken(user),
                KnownAs = user.KnownAs,
            };
        }
        // ---------------------
        // LOGIN POST METHOD
        // ---------------------
        [HttpPost("login")]
        public async Task<ActionResult<UserDTO>> Login(LoginDTO loginDTO)
        {
            // Looking for our user from Identity Manager (User Manager)
            // SingleorDefault --> If we don't find the user in our DB --> We get NULL back (Default Value)
            var user = await _userManager.Users
            .Include(p => p.Photos)
            .SingleOrDefaultAsync(x => x.UserName == loginDTO.Username);

            // If there is no user return this
            if (user == null) return Unauthorized("Invalid Username");

            // Checking to see if the password is the correct one from Identity to the LoginDTO
            var result = await _userManager.CheckPasswordAsync(user, loginDTO.Password);

            // If the password is wrong
            if (!result) return Unauthorized("Invalid password");

            // If the hash entered for the login and the database hash are equal --> return the User --> valid entry
            return new UserDTO
            {
                Username = user.UserName,
                Token = _tokenService.CreateToken(user),
                PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain)?.Url,
                KnownAs = user.KnownAs,
                Gender = user.Gender
            };
        }

        // Checking to see if the User Exists
        private async Task<Boolean> UserExists(string username)
        {
            // This asynchronously determines whether a sequence contains any elements 
            // --> i Any Existing Users
            // ToLower checks the user in lowercase
            return await _userManager.Users.AnyAsync(user => user.UserName == username.ToLower());
        }
    }
}