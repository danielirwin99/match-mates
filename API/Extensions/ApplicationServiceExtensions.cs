using API.Data;
using API.Helpers;
using API.Interfaces;
using API.Services;
using Microsoft.EntityFrameworkCore;

namespace API.Extensions
{
    // When using an Extension method you NEED to make the class STATIC
    public static class ApplicationServiceExtensions
    {
        // We need to return IServiceCollection, AddApplicationServices is just the name we called it
        // To return IService... We need to use "this" 
        // "IServiceCollection services" --> AKA IServiceCollection AS services (Its just an easier name for it)

        // We don't need builder.services --> services is what it is now
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
        {
            // Connecting our Sqlite 
            services.AddDbContext<DataContext>(opt =>
            {
                opt.UseSqlite(config.GetConnectionString("DefaultConnection"));
            });

            // This allows our browser to accept our localhost:5001 as trustworthy for the Angular requests from it
            // Anything before var app is considered our SERVICES CONTAINER
            services.AddCors();

            // Scoped to our HTTP Request --> Our Token Service
            // AddScoped needs two things inside the brackets:
            // 1. Our Interface, 2. Our Implementation Class
            // Using both saves us time to test them
            services.AddScoped<ITokenService, TokenService>();

            // This allows the following files injectable into our User Controller
            services.AddScoped<IUserRepository, UserRepository>();

            // Allows our project to use AutoMapper (Only good for a single project)
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            // Our Cloudinary Service
            services.Configure<CloudinarySettings>(config.GetSection("CloudinarySettings"));

            // Allows us to inject this into Controllers etc
            services.AddScoped<IPhotoService, PhotoService>();

            // Allows us to use our action filter created (See Helpers folder)
            services.AddScoped<LogUserActivity>();

            return services;
        }
    }
}