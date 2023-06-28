
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class LikesRepository : ILikesRepository
    {
        private readonly DataContext _context;
        public LikesRepository(DataContext context)
        {
            _context = context;

        }

        // Finds the user like entity that matches the primary key
        public async Task<UserLike> GetUserLike(int sourceUserId, int targetUserId)
        {
            return await _context.Likes.FindAsync(sourceUserId, targetUserId);
        }

        // We need to find our User Likes based on the predicate
        public async Task<PagedList<LikeDTO>> GetUserLikes(LikesParams likesParams)
        {
            // Getting users from the database ordered by their Username,
            // AsQueryable doesn't execute it to the database yet
            var users = _context.Users.OrderBy(u => u.UserName).AsQueryable();

            var likes = _context.Likes.AsQueryable();

            // Passing through the predicate from our LikesParams helper file
            if (likesParams.Predicate == "liked")
            {
                // Likes query for the User
                // Passing it through from our LikesParams file
                likes = likes.Where(like => like.SourceUserId == likesParams.UserId);
                // Filters out the users based on what's inside the likes list
                users = likes.Select(like => like.TargetUser);
            }

            // Passing it through from our LikesParams file
            if (likesParams.Predicate == "likedBy")
            {
                // Likes query for the target user
                likes = likes.Where(like => like.TargetUserId == likesParams.UserId);
                // Filters out the users based on what's inside the likes list
                users = likes.Select(like => like.SourceUser);
            }


            // Executing whats going to be inside the query request
            var likedUsers = users.Select(user => new LikeDTO
            {
                UserName = user.UserName,
                KnownAs = user.KnownAs,
                Age = user.DateOfBirth.CalculateAge(),
                PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain).Url,
                City = user.City,
                Id = user.Id
            });

            return await PagedList<LikeDTO>.CreateAsync(likedUsers, likesParams.PageNumber, likesParams.PageSize);
        }

        public async Task<AppUser> GetUserWithLikes(int userId)
        {
            return await _context.Users
            // Checking to see if a user has already been liked or not
            .Include(x => x.LikedUsers)
            .FirstOrDefaultAsync(x => x.Id == userId);
        }
    }
}