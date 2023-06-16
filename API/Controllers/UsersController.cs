using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
// ---------------------------------------
// THIS IS OUR CRUD CONTROLLER OF THE USER
// ---------------------------------------
{
    // Base is coming from our APIController file
    public class UsersController : BaseApiController
    {
        private readonly DataContext _context;
        public UsersController(DataContext context)
        {
            _context = context;
        }
        // -----------------------
        // Getting a all users
        // -----------------------
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AppUser>>> GetUsers()
        {
            // Gives us our list of Users
            var users = await _context.Users.ToListAsync();

            return users;
        }
        // -----------------------
        // Getting a specific user
        // -----------------------
        [HttpGet("{id}")]
        // We are specifying that we want to use the id as an argument
        public async Task<ActionResult<AppUser>> GetUser(int id)
        {
            // Finds a primary key value
            var user = await _context.Users.FindAsync(id);

            return user;
        }
    }
}