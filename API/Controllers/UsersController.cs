using System.Security.Claims;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
// ---------------------------------------
// THIS IS OUR CRUD CONTROLLER OF THE USER
// ---------------------------------------
{
    // Only allowed to get Users when they are authorised to do so
    // Base is coming from our APIController file
    [Authorize]
    public class UsersController : BaseApiController
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IPhotoService _photoService;
        public UsersController(IUserRepository userRepository, IMapper mapper, IPhotoService photoService)
        {
            _mapper = mapper;
            _photoService = photoService;
            _userRepository = userRepository;
        }
        // -----------------------
        // Getting all users
        // -----------------------
        // Admin can only get all the users
        [Authorize(Roles = "Member")]
        [HttpGet]
        // We need to give our API a hint about where it needs to look to find the UserParams
        public async Task<ActionResult<PagedList<MemberDTO>>> GetUsers([FromQuery] UserParams userParams)
        {

            // Storing our currentUser in a const to use below
            var currentUser = await _userRepository.GetUserByUsernameAsync(User.GetUsername());
            // Links the Username logged in to the CurrentUsername Params
            userParams.CurrentUsername = currentUser.UserName;


            if (string.IsNullOrEmpty(userParams.Gender))
            {
                // If our User is Male --> Return female genders vice versa
                userParams.Gender = currentUser.Gender == "male" ? "female" : "male";
            }


            // Gives us our list of Users from our UserRepository
            var users = await _userRepository.GetMembersAsync(userParams);

            // We get access to our HTTP Response --> We use our extension method that takes a Pagination Header as an argument
            Response.AddPaginationHeader(new PaginationHeader(users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPages));

            // Returning the argument above
            return Ok(users);

        }
        // -----------------------
        // Getting a specific user by username
        // -----------------------
        [Authorize(Roles = "Member")]
        [HttpGet("{username}")]
        public async Task<ActionResult<MemberDTO>> GetUser(string username)
        {
            // Finds the user by searching the Usernames in the database and returns it
            return await _userRepository.GetMemberAsync(username);


        }
        // --------------------------
        // Updating User Information
        // --------------------------
        [HttpPut]
        public async Task<ActionResult> UpdateUser(MemberUpdateDTO memberUpdateDTO)
        {
            // Using NameIdentifier because that's the one we used for our username (See TokenService)
            // Then getting the value of the claim
            // We are using an extension called "GetUsername" so we can use this later for photos
            // See ClaimsPrincipalExtensions.cs in Extensions
            var username = User.GetUsername();

            // Now that we have the username we can go across to our User Repository
            var user = await _userRepository.GetUserByUsernameAsync(username);

            if (user == null) return NotFound();

            // Updating all of the properties we passed into the MemberUpdateDTO into the USER
            _mapper.Map(memberUpdateDTO, user);

            // Saves the changes to the Database
            // NoContent --> 204 Response saying everything is OK and there's nothing to return
            if (await _userRepository.SaveAllAsync()) return NoContent();

            // Return this IF we HAVEN'T made any CHANGES
            return BadRequest("Failed to update user");
        }

        // --------------------------
        // Adding Photo
        // --------------------------
        // For our photos (we need to specify the route parameter since we already have a HttpPost)
        [HttpPost("add-photo")]
        public async Task<ActionResult<PhotoDTO>> AddPhoto(IFormFile file)
        {
            // Pulling the user through from the Repository via Username
            var user = await _userRepository.GetUserByUsernameAsync(User.GetUsername());

            // If there is no user
            if (user == null) return NotFound();

            // Pulls through the photo
            var result = await _photoService.AddPhotoAsync(file);

            // If there was an error with the photo uploading
            if (result.Error != null) return BadRequest(result.Error.Message);

            // Creating new Photo 
            var photo = new Photo
            {
                Url = result.SecureUrl.AbsoluteUri,
                PublicId = result.PublicId
            };

            // Checking to see if its the users first uploaded photo --> IF it is --> Set it to Main
            if (user.Photos.Count == 0) photo.IsMain = true;

            // Entity framework is tracking this add
            user.Photos.Add(photo);

            // If this works (if there are changes saved into our database):
            // We return a map into our PhotoDTO from our photo
            if (await _userRepository.SaveAllAsync())
            {
                // For our 201 Response
                // Takes in three parameters:
                // 1. Specifying the action (Getting the user) 
                // 2. Route Values (username) 
                // 3. Passing the object that we created back (the Photo from our map)
                return CreatedAtAction(nameof(GetUser), new { username = user.UserName }, _mapper.Map<PhotoDTO>(photo));
            }

            // And if the line above is NOT successful
            return BadRequest("Problem adding photo");
        }

        // Changing our main photo request
        [HttpPut("set-main-photo/{photoId}")]
        // photoId must match the one in the parameter above
        public async Task<ActionResult> SetMainPhoto(int photoId)
        {
            // Getting the user from our Repository and Extension
            var user = await _userRepository.GetUserByUsernameAsync(User.GetUsername());

            if (user == null) return NotFound();

            var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);

            if (photo == null) return NotFound();

            // Checking to see if the photo chosen is already the Main Photo
            if (photo.IsMain) return BadRequest("Already Main Photo");

            // Checking to see what the current Main Photo is
            var currentMain = user.Photos.FirstOrDefault(x => x.IsMain);

            // If there is a main photo we set it to equal false
            if (currentMain != null) currentMain.IsMain = false;

            // New photo is equal to true --> New photo is now Main Photo
            photo.IsMain = true;

            // Saves the new changes to the database with a 204 Response
            if (await _userRepository.SaveAllAsync()) return NoContent();

            // Last line of defence if the line above does not work
            return BadRequest("There was an error setting main photo");
        }
        [HttpDelete("delete-photo/{photoId}")]
        public async Task<ActionResult> DeletePhoto(int photoId)
        {
            var user = await _userRepository.GetUserByUsernameAsync(User.GetUsername());

            // Finding the photo in the database / cloud
            var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);

            // If there is no photo that is being deleted --> Return this
            if (photo == null) return NotFound();

            // If the photo being deleted is a Main --> Return this
            if (photo.IsMain) return BadRequest("You cannot delete your main photo");

            // This is for our seeded photos
            if (photo.PublicId != null)
            {
                var result = await _photoService.DeletePhotoAsync(photo.PublicId);
                if (result.Error != null) return BadRequest(result.Error.Message);
            }

            // Removes the photo
            user.Photos.Remove(photo);

            // If the deletion works return a response of 201 Ok
            if (await _userRepository.SaveAllAsync()) return Ok();

            // If the line above does not work --> Return this
            return BadRequest("Problem deleting photo");
        }
    }
}