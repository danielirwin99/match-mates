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
            // Authentication FIRST --> THEN AUTHORISATION
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

                // Authenticating inside SignalR
                options.Events = new JwtBearerEvents
                {
                    // This gives us access to the token
                    OnMessageReceived = context =>
                    {
                        // Our Bearer Token --> Need to access the query
                        var accessToken = context.Request.Query["access_token"];

                        // Accessing the path
                        var path = context.HttpContext.Request.Path;

                        // If there is a token and the path starts with "/hubs"
                        if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs")) // Path from PresenceHub.cs
                        {
                            // GIVES ACCESS TO THE BEARER TOKEN
                            context.Token = accessToken;
                        }

                        // Returns the task as complete
                        return Task.CompletedTask;
                    }
                };
            });

            services.AddAuthorization(options =>
            {
                // Options from our Admin Controller
                options.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"));
                options.AddPolicy("ModeratePhotoRole", policy => policy.RequireRole("Admin", "Moderator"));
            });
            return services;
        }
    }
}