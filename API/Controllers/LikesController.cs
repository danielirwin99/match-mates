
using API.Extensions;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class LikesController : BaseApiController
    {
        // Importing the IUserRepository
        private readonly ILikesRepository _likesRepository;
        private readonly IUserRepository _userRepository;
        public LikesController(IUserRepository userRepository, ILikesRepository likesRepository)
        {
            _userRepository = userRepository;
            _likesRepository = likesRepository;

        }

        // Every time a like is fired we a posting a request, 
        // The username is the root parameter of the person they are about to like
        [HttpPost("username")]
        public async Task<ActionResult> AddLike(string username)
        {
            // Getting the User that will be LIKING
            // This returns a string so we need to convert it into an int
            var sourceUserId = int.Parse(User.GetUserId());

            // Passing in the username we have in our route parameter
            var likedUser = await _userRepository.GetUserByUsernameAsync(username);

            var sourceUser = await _likesRepository.GetUserWithLikes(sourceUserId);
        }
    }
}