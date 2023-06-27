using Microsoft.EntityFrameworkCore;

namespace API.Helpers
{
    // Type of List
    public class PagedList<T> : List<T>
    {
        public PagedList(IEnumerable<T> items, int count, int pageNumber, int pageSize)
        {
            CurrentPage = pageNumber;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            PageSize = pageSize;
            TotalCount = count;
            // Returns a list of our items
            AddRange(items);
        }

        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }

        // Passing in our query here (query built in entity framework)
        public static async Task<PagedList<T>> CreateAsync(IQueryable<T> source, int pageNumber, int pageSize)
        {
            // This gives us the count of our items from our query
            // Its going to look at our query and find the total count
            var count = await source.CountAsync();
            // We are getting our items using a Skip and Take method and then executing it to a list
            var items = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

            // Returning the arguments inside our PagedList
            return new PagedList<T>(items, count, pageNumber, pageSize);
        }
    }
}