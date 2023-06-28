
using API.DTOs;
using API.Entities;

namespace API.Interfaces
{
    public interface ILikesRepository
    {
        // Two properties that make up the primary key of the entity
        Task<UserLike> GetUserLike(int sourceUserId, int targetUserId);
        Task<AppUser> GetUserWithLikes(int userId);

        // Do they want to get the user they have liked or the user they have been liked by
        Task<IEnumerable<LikeDTO>> GetUserLikes(string predicate, int userId);
    }
}