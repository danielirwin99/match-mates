using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AdminController : BaseApiController
    {
        private readonly UserManager<AppUser> _userManager;
        public AdminController(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        // Configuring polices from IdentityService Extensions
        [Authorize(Policy = "RequireAdminRole")]
        [HttpGet("users-with-roles")]
        public async Task<ActionResult> GetUserWithRoles()
        {
            // Making a list with the related tables included
            var listOfUsers = await _userManager.Users
            .OrderBy(u => u.UserName)
            // We need to access the User Roles Table AND its related table --> Roles table
            .Select(u => new
            {
                u.Id,
                Username = u.UserName,
                Roles = u.UserRoles.Select(r => r.Role.Name).ToList()
            })
            .ToListAsync();

            // Returning it into our API Request
            return Ok(listOfUsers);
        }

        // EDITING THE ROLE
        [Authorize(Policy = "RequireAdminRole")]
        // Its post instead of put because we want to see the updated roles list
        [HttpPost("edit-roles/{username}")]
        // Username is coming from something else than the roles --> Must specify for the roles
        public async Task<ActionResult> EditRoles(string username, [FromQuery] string roles)
        {
            // Checking to see if we have anything side the query string
            if (string.IsNullOrEmpty(roles)) return BadRequest("You must select at least one role");

            // Splits the roles into separate arrays
            var selectedRoles = roles.Split(",").ToArray();

            // Finding the user
            var user = await _userManager.FindByNameAsync(username);

            if (user == null) return NotFound();

            // Getting the existing roles of the user
            var userRoles = await _userManager.GetRolesAsync(user);

            // Adds the roles to the user EXCEPT for the ones they already are
            var result = await _userManager.AddToRolesAsync(user, selectedRoles.Except(userRoles));

            // If you couldn't add the role
            if (!result.Succeeded) return BadRequest("Failed to add roles");

            result = await _userManager.RemoveFromRolesAsync(user, userRoles.Except(selectedRoles));

            if (!result.Succeeded) return BadRequest("Failed to remove from roles");

            // Returning an updated list of roles that the user is in
            return Ok(await _userManager.GetRolesAsync(user));
        }

        [Authorize(Policy = "ModeratePhotoRole")]
        [HttpGet("photos-to-moderate")]
        public ActionResult GetPhotosForModeration()
        {
            return Ok("Admin or moderators can see this");
        }
    }
}