namespace API.Extensions
{
    // Extending the DateTime Class
    // This has to be static
    public static class DateTimeExtensions
    {
        // Returning an int that is called Calculate Age --> We need to specify what we are extending -->
        // In this case DateOnly (we are calling it dob)
        public static int CalculateAge(this DateOnly dob)
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);

            var age = today.Year - dob.Year;

            // If there birthday has already happened then we want to take off 1 year so its correct
            if (dob > today.AddYears(-age)) age--;

            return age;
        }
    }
}