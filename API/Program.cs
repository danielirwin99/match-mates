using API.Extensions;
using API.Middleware;

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
app.UseMiddleware<ExceptionMiddleware>();

app.UseHttpsRedirection();

// Allowing us to connect to our FRONTEND application
app.UseCors(builder => builder.AllowAnyHeader().AllowAnyMethod().WithOrigins("https://localhost:4200"));

// Asks do you have a Valid token?
app.UseAuthentication();

// This one identifies a valid token and asks what are you allowed to do?
app.UseAuthorization();

app.MapControllers();

app.Run();
app.UseAuthorization();
