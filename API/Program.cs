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

// Anything before var app is considered our SERVICES CONTAINER

var app = builder.Build();

// Configure the HTTP request pipeline. --> AS THE REQUEST COMES INTO OUR PIPELINE

// MIDDLEWARE
app.UseHttpsRedirection();

// app.UseAuthorization();

app.MapControllers();

app.Run();
