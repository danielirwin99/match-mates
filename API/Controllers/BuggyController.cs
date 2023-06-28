using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class BuggyController : BaseApiController
    {
        private readonly DataContext _context;
        public BuggyController(DataContext context)
        {
            _context = context;
        }

        [Authorize]

        // Setting up our Different Error Request Endpoints
        // 401 Unauthorized Error
        [HttpGet("auth")]
        public ActionResult<string> GetSecret()
        {
            return "Secret Text";
        }

        // 404 Server Not Found
        [HttpGet("not-found")]
        public ActionResult<AppUser> GetNotFound()
        {
            var thing = _context.Users.Find(-1);

            if (thing == null) return NotFound("Route could not be found");

            return thing;
        }

        // 500 Internal Server Error
        [HttpGet("server-error")]
        public ActionResult<string> GetServerError()
        {

            var thing = _context.Users.Find(-1);

            var thingToReturn = thing.ToString();

            return thingToReturn;

        }

        // 400 Bad Request Error
        [HttpGet("bad-request")]
        public ActionResult<string> GetBadRequest()
        {
            return BadRequest("Bad Request");
        }
    }
}