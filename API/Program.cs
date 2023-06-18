using System.Text;
using API.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// We have moved files from here to ApplicationServices Extension (see file in Extensions Folder)
builder.Services.AddApplicationServices(builder.Configuration);

// We have moved files from here to IdentityServices Extension (see file in Extensions Folder)
builder.Services.AddIdentityServices(builder.Configuration);

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
