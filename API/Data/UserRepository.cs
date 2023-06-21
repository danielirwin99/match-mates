using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    // Implementing our Repository created in Interfaces
    public class UserRepository : IUserRepository
    {
        // Pulling through our User Data
        private readonly DataContext _context;
        public readonly IMapper _mapper;
        public UserRepository(DataContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
        }

        // Returns the Single Member --> GET REQUEST
        public async Task<MemberDTO> GetMemberAsync(string username)
        {
            return await _context.Users
            // When UserName equals username
            .Where(x => x.UserName == username)
            // We are projecting to MemberDTO
            // We need to map our user properties into the MemberDTO
            .ProjectTo<MemberDTO>(_mapper.ConfigurationProvider)
            .SingleOrDefaultAsync();
        }

        // Returns our Users --> GET ALL REQUEST
        public async Task<IEnumerable<MemberDTO>> GetMembersAsync()
        {
            // Same as above but a get all request
            return await _context.Users.ProjectTo<MemberDTO>(_mapper.ConfigurationProvider).ToListAsync();
        }

        // -----------------------------------------------------------------------------------------
        // Finding our User by the Id Method
        public async Task<AppUser> GetUserByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }
        // -----------------------------------------------------------------------------------------
        // The same as find User by Id but for the Username (checking to see if they are the same)
        public async Task<AppUser> GetUserByUsernameAsync(string username)
        {
            return await _context.Users
            // This is to tell entity framework to INCLUDE related data --> AKA Eager loading the entity
            // Including the photo in our GET response
            .Include(p => p.Photos)
            .SingleOrDefaultAsync(x => x.UserName == username);
        }
        // -----------------------------------------------------------------------------------------
        // Gets all the Users in a list
        public async Task<IEnumerable<AppUser>> GetUsersAsync()
        {
            return await _context.Users
            // This is to tell entity framework to INCLUDE related data --> AKA Eager loading the entity
            // Including the photo in our GET response
            .Include(p => p.Photos)
            .ToListAsync();

        }
        // -----------------------------------------------------------------------------------------
        // Saving the changes to the database --> We want to make sure that there is a change i.e greater than 0
        public async Task<bool> SaveAllAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
        // -----------------------------------------------------------------------------------------
        // This tells the entity that something has changed with the User state
        public void Update(AppUser user)
        {
            _context.Entry(user).State = EntityState.Modified;
        }
    }
}