using API.Data;
using API.DTOs;
using API.Entities;
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
        public UsersController(IUserRepository userRepository, IMapper mapper)
        {
            _mapper = mapper;
            _userRepository = userRepository;
        }
        // -----------------------
        // Getting all users
        // -----------------------
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDTO>>> GetUsers()
        {
            // Gives us our list of Users from our UserRepository
            var users = await _userRepository.GetMembersAsync();

            // Returning the argument above
            return Ok(users);

        }
        // -----------------------
        // Getting a specific user by username
        // -----------------------
        [HttpGet("{username}")]
        public async Task<ActionResult<MemberDTO>> GetUser(string username)
        {
            // Finds the user by searching the Usernames in the database and returns it
            return await _userRepository.GetMemberAsync(username);


        }
    }
}