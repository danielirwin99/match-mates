using API.Extensions;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc.Filters;

namespace API.Helpers
{
    // Updating the User through an action filter
    public class LogUserActivity : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // Gives us the Action Executed context
            var resultContext = await next();

            // We want to ensure the user is authenticated
            if (!resultContext.HttpContext.User.Identity.IsAuthenticated) return;

            // Getting access to our user claims principle --> GetUsername()
            var userId = resultContext.HttpContext.User.GetUserId();

            // Getting access to the repository because we need to update something for the user
            var repo = resultContext.HttpContext.RequestServices.GetRequiredService<IUserRepository>();

            // Getting access to our user from repository from the line above
            var user = await repo.GetUserByIdAsync(userId);

            // Setting the users last active to UTC Format to NOW
            user.LastActive = DateTime.UtcNow;

            // Saving changes
            await repo.SaveAllAsync();
        }
    }
}