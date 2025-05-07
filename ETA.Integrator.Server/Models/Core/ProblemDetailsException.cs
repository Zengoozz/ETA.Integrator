namespace ETA.Integrator.Server.Models.Core
{
    public class ProblemDetailsException : Exception
    {
        public int StatusCode { get; }
        public string Detail { get; }

        public ProblemDetailsException(int statusCode, string message, string detail = "")
            : base(message)
        {
            StatusCode = statusCode;
            Detail = detail;
        }
    }
}
