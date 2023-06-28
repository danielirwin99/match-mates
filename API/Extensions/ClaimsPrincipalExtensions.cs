

using System.Security.Claims;

namespace API.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        // Getting the Username through ClaimsPrincipal extension
        public static string GetUsername(this ClaimsPrincipal user)
        {
            // Name = UniqueName from Token Service
            return user.FindFirst(ClaimTypes.Name)?.Value;
        }
        public static int GetUserId(this ClaimsPrincipal user)
        {
            // This returns a string so we need to convert it into an int
            return int.Parse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        }
    }
}