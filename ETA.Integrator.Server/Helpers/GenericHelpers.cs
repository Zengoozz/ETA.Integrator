using ETA.Integrator.Server.Models.Core;
using System.Text.Json;

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

        public static T JsonDeserialize<T> (string content, JsonSerializerOptions? opt = null) where T : new()
        {
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                PropertyNameCaseInsensitive = true
            };

            if (opt is not null)
                options = opt;

            T? serializedResponse = new();

            try
            {
                serializedResponse = JsonSerializer.Deserialize<T>(content, options);
            }
            catch (Exception)
            {
                throw;
            }

            if(serializedResponse is null)
                throw new ProblemDetailsException(
                    statusCode: StatusCodes.Status500InternalServerError,
                    message: "SERIALIZATION_FAILED",
                    detail: "Could not serialize the response."
                    );

            return serializedResponse;
        }
    }
}