using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using API.Entities;
using API.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace API.Services
{
    public class TokenService : ITokenService
    {
        // SymmetricKey lets us crypt and decrypt the result, _key is the name of it
        // This key will stay on the server and never go to the client --> i.e public and private key
        private readonly SymmetricSecurityKey _key;
        public TokenService(IConfiguration config)
        {
            // Making a new key for each request
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["TokenKey"]));
        }
        public string CreateToken(AppUser user)
        {
            // Making a new claim for a token
            var claims = new List<Claim>
            {
                // Function for the UserName that claims the JWT Token
                // We are setting our NameId to Id of the User
                new Claim(JwtRegisteredClaimNames.NameId, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName),
            };

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