namespace ETA.Integrator.Server.Helpers
{
    public static class GenericHelpers
    {
        public static DateTime GetCurrentUTCTime(int minutes)
        {
            DateTime utcNow = DateTime.UtcNow;

            // Create a new DateTime without milliseconds
            DateTime trimmedUtcNow = new DateTime(
                utcNow.Year,
                utcNow.Month,
                utcNow.Day,
                utcNow.Hour,
                utcNow.Minute,
                utcNow.Second,
                DateTimeKind.Utc
            ).AddMinutes(minutes);
            return trimmedUtcNow;
        }
    }
}