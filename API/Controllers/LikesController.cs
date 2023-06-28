
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class LikesController : BaseApiController
    {
        // Importing the IUserRepository + ILikesRepository
        private readonly ILikesRepository _likesRepository;
        private readonly IUserRepository _userRepository;
        public LikesController(IUserRepository userRepository, ILikesRepository likesRepository)
        {
            _userRepository = userRepository;
            _likesRepository = likesRepository;

        }

        // Every time a like is fired we a posting a request, 
        // The username is the root parameter of the person they are about to like
        // MAKE SURE ITS IN CURLY BRACKETS
        [HttpPost("{username}")]
        public async Task<ActionResult> AddLike(string username)
        {
            // Getting the User that will be LIKING
            var sourceUserId = (User.GetUserId());

            // Passing in the username we have in our route parameter
            var likedUser = await _userRepository.GetUserByUsernameAsync(username);

            // Getting the User from the Id
            var sourceUser = await _likesRepository.GetUserWithLikes(sourceUserId);

            // If there is no liked user return the request NotFound
            if (likedUser == null) return NotFound("Could not find user");

            // If the User tries to like their own profile
            if (sourceUser.UserName == username) return BadRequest("You cannot like yourself");

            // Getting our User and the user they liked to use below
            var userLike = await _likesRepository.GetUserLike(sourceUserId, likedUser.Id);

            // If we have already liked the user --> i.e its not null on the request
            if (userLike != null) return BadRequest("You already liked this user");

            // Syncs the action with the json name in the API
            userLike = new UserLike
            {
                SourceUserId = sourceUserId,
                TargetUserId = likedUser.Id,
            };

            // Adds it to the table when it fires off
            sourceUser.LikedUsers.Add(userLike);

            // Saves it to the table in our 
            if (await _userRepository.SaveAllAsync()) return Ok();

            // If it fails to save in the DB
            return BadRequest("Failed to like user");
        }

        [HttpGet]
        // Predicate is to see whether we want to send back the liked users or liked by users
        public async Task<ActionResult<PagedList<LikeDTO>>> GetUserLikes([FromQuery] LikesParams likesParams) // We have to tell the controller where to find the params
        {
            // We need to get the UserId for our predicate
            likesParams.UserId = User.GetUserId();

            var users = await _likesRepository.GetUserLikes(likesParams);

            // Adding our Pagination Headers to the response
            Response.AddPaginationHeader(new PaginationHeader(users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPages));

            return Ok(users);
        }
    }
}