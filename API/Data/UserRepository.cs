using API.DTOs;
using API.Entities;
using API.Helpers;
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
        public async Task<PagedList<MemberDTO>> GetMembersAsync(UserParams userParams)
        {
            var query = _context.Users.AsQueryable();

            // Returning back all the users that do not have the CurrentUsername to the server
            query = query.Where(u => u.UserName != userParams.CurrentUsername);

            // Returning back all the users that do not have the same Gender to the server
            query = query.Where(u => u.Gender == userParams.Gender);

            // Working out the minimum date of birth is
            var minDob = DateOnly.FromDateTime(DateTime.Today.AddYears(-userParams.MaxAge - 1));

            // Working out the maximum date of birth is
            var maxDob = DateOnly.FromDateTime(DateTime.Today.AddYears(-userParams.MinAge));

            // Our Query parameters for the age
            query = query.Where(u => u.DateOfBirth >= minDob && u.DateOfBirth <= maxDob);

            // Our Sorting query function on the server
            query = userParams.OrderBy switch
            {
                // This gives us our newest user first
                "created" => query.OrderByDescending(u => u.Created),
                // Our default
                _ => query.OrderByDescending(u => u.LastActive)
            };

            // Pulling CreateAsync from our PagedList.cs
            return await PagedList<MemberDTO>.CreateAsync(
            query.AsNoTracking().ProjectTo<MemberDTO>(_mapper.ConfigurationProvider),
            userParams.PageNumber,
            userParams.PageSize);
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