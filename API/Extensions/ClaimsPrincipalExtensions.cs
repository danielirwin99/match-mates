

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
        public static string GetUserId(this ClaimsPrincipal user)
        {
            return user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }
    }
}