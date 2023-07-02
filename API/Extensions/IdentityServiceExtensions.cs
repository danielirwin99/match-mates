using System.Text;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace API.Extensions
{
    public static class IdentityServiceExtensions
    {
        public static IServiceCollection AddIdentityServices(this IServiceCollection services, IConfiguration config)
        {

            services.AddIdentityCore<AppUser>(options =>
            {
                // Can change your options for username and password here
                options.Password.RequireNonAlphanumeric = false;
            })
            // Adds roles to our server
            .AddRoles<AppRole>()
            // Adds a role manager
            .AddRoleManager<RoleManager<AppRole>>()
            // This creates all of the tables related to identity in our DB
            .AddEntityFrameworkStores<DataContext>();

            
            // BEARER TOKEN PACKAGE
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                // We specify some options that we want for our Bearer Token
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    // Makes sure our Sign in key is valid based upon Signing in and that its not manually made
                    ValidateIssuerSigningKey = true,
                    // Now we decide what our IssuerSigningKey is
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["TokenKey"])),
                    // Our API Server --> Temporary
                    ValidateIssuer = false,
                    // Temporary
                    ValidateAudience = false
                };
            });
            return services;
        }
    }
}