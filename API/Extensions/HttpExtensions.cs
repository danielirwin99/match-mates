using System.Text.Json;
using API.Helpers;

namespace API.Extensions
{
    public static class HttpExtensions
    {
        public static void AddPaginationHeader(this HttpResponse response, PaginationHeader header)
        {
            // We need to serialize this into JSON so that it can go back with the header 
            // Needs to be in JSON formats and not C# object format
            var jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            
            // Accessing our response.Headers --> Adding a Header called "Pagination" --> Then we are serializing it with the JsonOptions
            response.Headers.Add("Pagination", JsonSerializer.Serialize(header, jsonOptions));

            // Allowing cors policy (Must be Exact Typing)
            response.Headers.Add("Access-Control-Expose-Headers", "Pagination");
        }
    }
}