using ETA.Integrator.Server.Models.Core;
using Microsoft.AspNetCore.Mvc;

namespace ETA.Integrator.Server.Middlewares
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (ProblemDetailsException ex)
            {
                _logger.LogError(ex, ex.Message);
                await HandleExceptionAsync(context, ex.StatusCode, ex.Message, ex.Detail);
            }
            catch (Exception ex)
            {
                var exMsg = "An unexpected error occurred";

                if (ex.Source == "System.Text.Json")
                    exMsg = "Serialization exception";

                _logger.LogError(ex, ex.Message);
                await HandleExceptionAsync(context, 500, exMsg, ex.Message);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, int statusCode, string title, string detail)
        {
            var problemDetails = new ProblemDetails
            {
                Status = statusCode,
                Title = title,
                Detail = detail,
                Instance = context.Request.Path
            };

            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";
            return context.Response.WriteAsJsonAsync(problemDetails);
        }
    }

}
