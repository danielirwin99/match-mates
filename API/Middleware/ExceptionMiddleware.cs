using System.Net;
using System.Text.Json;
using API.Errors;

namespace API.Middleware
{
    public class ExceptionMiddleware
    {
        // RequestDelegate works as our next --> Hopping for one middleware to the next
        private readonly RequestDelegate _next;
        // ILogger is for logging our response
        private readonly ILogger<ExceptionMiddleware> _logger;
        // IHostEnvironment is to see if we are running development mode or production mode
        private readonly IHostEnvironment _env;
        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment env)
        {
            _env = env;
            _logger = logger;
            _next = next;

        }
        // This method tells the Middleware where to go next
        // Has to be called InvokeAsync
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                // Passing through the Http context
                await _next(context);
            }
            // If the try block is unsuccessful it will hit the catch block
            catch (System.Exception ex)
            {
                // Logging the error to our terminal
                _logger.LogError(ex, ex.Message);

                // This is returning the type to the client
                context.Response.ContentType = "application/json";

                // Our Status Code that client will see
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                // Check our environment to see if its Development or Production
                var response = _env.IsDevelopment()

                // If we ARE in development mode --> This is our Response
                ? new ApiException(context.Response.StatusCode, ex.Message, ex.StackTrace?.ToString())

                // If we ARE NOT in development mode: Same thing but we are returning this message
                : new ApiException(context.Response.StatusCode, ex.Message, "Internal Server Error");

                // Since the response is being returned as JSON --> We are giving it some options
                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };

                // Passing in response and options as its argument
                var json = JsonSerializer.Serialize(response, options);

                // This returns the response
                await context.Response.WriteAsync(json);
            }
        }
    }
}