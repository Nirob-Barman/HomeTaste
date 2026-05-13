using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Security;
using System.Text.Json;

namespace HomeTaste.Infrastructure.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception occurred.");
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
           
            var response = new
            {
                //StatusCode = context.Response.StatusCode,
                statusCode = GetStatusCode(exception),
                Message = "An unexpected error occurred.",
                success = false,
                data = (object)null!,
                errors = exception.Message // Optional: hide in production                
            };

            return context.Response.WriteAsJsonAsync(response);
        }


        private static int GetStatusCode(Exception exception) => exception switch
        {
            // 400 Bad Request - Client-side input problems
            ArgumentNullException => (int)HttpStatusCode.BadRequest,
            ArgumentException => (int)HttpStatusCode.BadRequest,
            FormatException => (int)HttpStatusCode.BadRequest,
            InvalidCastException => (int)HttpStatusCode.BadRequest,
            JsonException => (int)HttpStatusCode.BadRequest,
            IndexOutOfRangeException => (int)HttpStatusCode.BadRequest,
            OverflowException => (int)HttpStatusCode.BadRequest, // Too large/small numeric values
            DivideByZeroException => (int)HttpStatusCode.BadRequest,

            // Malformed input or query expressions
            InvalidDataException => (int)HttpStatusCode.BadRequest,
            InvalidProgramException => (int)HttpStatusCode.BadRequest,
            NullReferenceException => (int)HttpStatusCode.BadRequest,

            // 401 Unauthorized - Missing or invalid authentication
            UnauthorizedAccessException => (int)HttpStatusCode.Unauthorized,

            // 403 Forbidden - Authenticated but not allowed
            SecurityException => (int)HttpStatusCode.Forbidden,

            // HTTP Request not allowed by policy/middleware
            HttpRequestException => (int)HttpStatusCode.Forbidden,

            // 404 Not Found - Resource doesn't exist
            KeyNotFoundException => (int)HttpStatusCode.NotFound,
            FileNotFoundException => (int)HttpStatusCode.NotFound,
            DirectoryNotFoundException => (int)HttpStatusCode.NotFound,

            // 405 Method Not Allowed (optional) - Unsupported HTTP method
            NotSupportedException => (int)HttpStatusCode.MethodNotAllowed,

            // 408 Request Timeout - Client waited too long
            TimeoutException => (int)HttpStatusCode.RequestTimeout,

            // 409 Conflict - State conflict (e.g., duplicate, invalid update)
            InvalidOperationException => (int)HttpStatusCode.Conflict,

            // Concurrency conflict (e.g., EF Core)
            DbUpdateConcurrencyException => (int)HttpStatusCode.Conflict,


            // 422 Unprocessable Entity (common for validation) - Validation errors
            ValidationException => 422, // FluentValidation, for example

            // 429 Too Many Requests - Rate limiting or cancellation
            OperationCanceledException => 429, // Optional, e.g. cancellation token use

            // 500 Internal Server Error - Catch-all, unexpected server issues
            StackOverflowException => (int)HttpStatusCode.InternalServerError,
            OutOfMemoryException => (int)HttpStatusCode.InternalServerError,
            AccessViolationException => (int)HttpStatusCode.InternalServerError,
            AppDomainUnloadedException => (int)HttpStatusCode.InternalServerError,

            // IO exception (e.g., file access issues)
            IOException => (int)HttpStatusCode.InternalServerError,

            // 501 Not Implemented - Feature not supported
            NotImplementedException => (int)HttpStatusCode.NotImplemented,

            // Fallback
            _ => (int)HttpStatusCode.InternalServerError
        };
    }
}
