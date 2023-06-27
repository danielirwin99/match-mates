

namespace API.Helpers
{
    public class UserParams
    {
        // How many we want to return per page
        private const int MaxPageSize = 50;

        // What page we want to return first
        public int PageNumber { get; set; } = 1;

        private int _pageSize = 10;

        public int PageSize
        {
            // Gets the value of 10
            get => _pageSize;
            // If the value is greater than the MaxPageSize --> Return MaxPageSize, If its smaller --> Return the value
            set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
        }
        
    }
}