using API.Data;
using Microsoft.EntityFrameworkCore;

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

var app = builder.Build();


// MIDDLEWARE
app.UseHttpsRedirection();

// Configure the HTTP request pipeline. --> AS THE REQUEST COMES INTO OUR PIPELINE
// Allowing us to connect to our FRONTEND application
app.UseCors(builder => builder.AllowAnyHeader().AllowAnyMethod().WithOrigins("https://localhost:4200"));

app.MapControllers();

app.Run();
// app.UseAuthorization(); --> Not using it ATM
