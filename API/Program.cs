using API.Data;
using API.Entities;
using API.Extensions;
using API.Middleware;
using API.SignalR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// We have moved files from here to ApplicationServices Extension (see file in Extensions Folder)
builder.Services.AddApplicationServices(builder.Configuration);

// We have moved files from here to IdentityServices Extension (see file in Extensions Folder)
builder.Services.AddIdentityServices(builder.Configuration);

var app = builder.Build();

// MIDDLEWARE
// Configure the HTTP request pipeline. --> AS THE REQUEST COMES INTO OUR PIPELINE

// Our Error Middleware
app.UseMiddleware<ExceptionMiddleware>();

app.UseHttpsRedirection();

// Allowing us to connect to our FRONTEND application
app.UseCors(builder => builder
.AllowAnyHeader()
.AllowAnyMethod()
// Need this for SignalR to be Authenticated to the server
.AllowCredentials()
.WithOrigins("https://localhost:4200"));

// Asks do you have a Valid token?
app.UseAuthentication();

// This one identifies a valid token and asks what are you allowed to do?
app.UseAuthorization();

app.MapControllers();

// Endpoint of User Presence (Are they Active Now)
app.MapHub<PresenceHub>("hubs/presence");

// Endpoint for the message call from SignalR
app.MapHub<MessageHub>("hubs/message");

// Gives us access to all of the services we have in this app
using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;
// Since this is not a HTTP Request and does not go through our ExceptionMiddleware --> 
// We need to use a try catch
try
{
    var context = services.GetRequiredService<DataContext>();

    // Populates the Users
    var userManager = services.GetRequiredService<UserManager<AppUser>>();

    // Populates the roles for the users
    var roleManager = services.GetRequiredService<RoleManager<AppRole>>();

    // Applies any pending migrations for the context to the database
    // Will also create the database if it does not already exist
    await context.Database.MigrateAsync();

    // Removes the connections when we wipe the database USING ENTITY FRAMEWORK
    context.Connections.RemoveRange(context.Connections);

    // Now that we have the database we can pass in our SeedUsers data --> The context
    await Seed.SeedUsers(userManager, roleManager);
}
catch (System.Exception ex)
{
    // Functionality that logs any errors 
    var logger = services.GetService<ILogger<Program>>();
    logger.LogError(ex, "An error occurred during migration");
}

app.Run();
