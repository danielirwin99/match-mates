using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace API.Services
{
    public class TokenService : ITokenService
    {
        // SymmetricKey lets us crypt and decrypt the result, _key is the name of it
        // This key will stay on the server and never go to the client --> i.e public and private key
        private readonly SymmetricSecurityKey _key;
        private readonly UserManager<AppUser> _userManager;

        // Pulling through our User Identity Manager
        public TokenService(IConfiguration config, UserManager<AppUser> userManager)
        {
            _userManager = userManager;
            // Making a new key for each request
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["TokenKey"]));
        }
        public async Task<string> CreateToken(AppUser user)
        {
            // Making a new claim for a token
            var claims = new List<Claim>
            {
                // Function for the UserName that claims the JWT Token
                // We are setting our NameId to Id of the User
                new Claim(JwtRegisteredClaimNames.NameId, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName),
            };

            // Getting the roles for our API Request
            var roles = await _userManager.GetRolesAsync(user);

            // Once we have our roles we want to add it to the list of the request
            // We only want the role they are part of so we need to Select it from a claim
            // Passing in the "role" as the claim type
            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            // Checking to see if the credentials are the same 
            var credentials = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);

            // Describe the token that we are going to return
            var tokenDescription = new SecurityTokenDescriptor
            {
                // Subject includes the claims we want to return
                Subject = new ClaimsIdentity(claims),
                // When the token will expire
                Expires = DateTime.Now.AddDays(30),
                // Setting the sign in credentials to key's credentials
                SigningCredentials = credentials
            };

            // 
            var tokenHandler = new JwtSecurityTokenHandler();

            // Creates our token with our description
            var token = tokenHandler.CreateToken(tokenDescription);

            // 
            return tokenHandler.WriteToken(token);
        }
    }
}