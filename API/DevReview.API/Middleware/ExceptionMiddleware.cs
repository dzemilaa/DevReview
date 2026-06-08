using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace DevReview.API.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IHostEnvironment _environment;

        public ExceptionMiddleware(RequestDelegate next, IHostEnvironment environment)
        {
            _next = next;
            _environment = environment;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await WriteProblemDetailsAsync(context, ex);
            }
        }

        private async Task WriteProblemDetailsAsync(HttpContext context, Exception ex)
        {
            var statusCode = MapStatusCode(ex);
            var isServerError = statusCode == (int)System.Net.HttpStatusCode.InternalServerError;

            var problem = new ProblemDetails
            {
                Type = GetProblemType(statusCode),
                Title = GetTitle(statusCode),
                Status = statusCode,
                Detail = isServerError && !_environment.IsDevelopment()
                    ? "An unexpected error occurred. Please try again later."
                    : ex.Message,
                Instance = context.Request.Path
            };

            problem.Extensions["traceId"] = context.TraceIdentifier;

            if (_environment.IsDevelopment())
            {
                problem.Extensions["stackTrace"] = ex.StackTrace;
                if (ex.InnerException != null)
                    problem.Extensions["innerException"] = ex.InnerException.Message;
            }

            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/problem+json";
            await context.Response.WriteAsJsonAsync(problem);
        }

        private static int MapStatusCode(Exception ex) =>
            ex switch
            {
                KeyNotFoundException => (int)HttpStatusCode.NotFound,
                UnauthorizedAccessException => (int)HttpStatusCode.Forbidden,
                InvalidOperationException => (int)HttpStatusCode.BadRequest,
                ArgumentException => (int)HttpStatusCode.BadRequest,
                ApplicationException => (int)HttpStatusCode.BadRequest,
                _ => (int)HttpStatusCode.InternalServerError
            };

        private static string GetTitle(int statusCode) =>
            statusCode switch
            {
                StatusCodes.Status400BadRequest => "Bad Request",
                StatusCodes.Status403Forbidden => "Forbidden",
                StatusCodes.Status404NotFound => "Not Found",
                StatusCodes.Status500InternalServerError => "Internal Server Error",
                _ => "An error occurred"
            };

        private static string GetProblemType(int statusCode) =>
            statusCode switch
            {
                StatusCodes.Status400BadRequest => "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                StatusCodes.Status403Forbidden => "https://tools.ietf.org/html/rfc7231#section-6.5.3",
                StatusCodes.Status404NotFound => "https://tools.ietf.org/html/rfc7231#section-6.5.4",
                StatusCodes.Status500InternalServerError => "https://tools.ietf.org/html/rfc7231#section-6.6.1",
                _ => "about:blank"
            };
    }
}
