using System.Text;
using API.Data;
using API.Interfaces;
using API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Connecting our Sqlite 
builder.Services.AddDbContext<DataContext>(opt =>
{
    opt.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// This allows our browser to accept our localhost:5001 as trustworthy for the Angular requests from it

// Anything before var app is considered our SERVICES CONTAINER
builder.Services.AddCors();

// Scoped to our HTTP Request --> Our Token Service
// AddScoped needs two things inside the brackets:
// 1. Our Interface, 2. Our Implementation Class
// Using both saves us time to test them
builder.Services.AddScoped<ITokenService, TokenService>();

// BEARER TOKEN PACKAGE
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    // We specify some options that we want for our Bearer Token
    options.TokenValidationParameters = new TokenValidationParameters
    {
        // Makes sure our Sign in key is valid based upon Signing in and that its not manually made
        ValidateIssuerSigningKey = true,
        // Now we decide what our IssuerSigningKey is
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["TokenKey"])),
        // Our API Server --> Temporary
        ValidateIssuer = false,
        // Temporary
        ValidateAudience = false
    };
});

var app = builder.Build();


// MIDDLEWARE
app.UseHttpsRedirection();

// Configure the HTTP request pipeline. --> AS THE REQUEST COMES INTO OUR PIPELINE
// Allowing us to connect to our FRONTEND application
app.UseCors(builder => builder.AllowAnyHeader().AllowAnyMethod().WithOrigins("https://localhost:4200"));

// Asks do you have a Valid token?
app.UseAuthentication();

// This one identifies a valid token and asks what are you allowed to do?
app.UseAuthorization();

app.MapControllers();

app.Run();
// app.UseAuthorization(); --> Not using it ATM
