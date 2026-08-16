using HomeTaste.Application.Common.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Security;
using System.Text.Json;
using DataAnnotationsValidationException = System.ComponentModel.DataAnnotations.ValidationException;
using ApplicationValidationException = HomeTaste.Application.Common.Exceptions.ValidationException;

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
            var statusCode = GetStatusCode(exception);
            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";

            var (message, errors) = GetResponseBody(exception);

            var response = new
            {
                statusCode,
                message,
                success = false,
                data = (object?)null,
                errors
            };

            return context.Response.WriteAsJsonAsync(response);
        }

        // Known Application exceptions carry a client-facing message on purpose (they replace
        // the old ResultType.Fail(errorMessage, ...) path) - surface it as-is. Anything else is
        // an unexpected/unhandled error, so keep the generic message and put the exception's
        // detail in errors instead of leaking internals as the headline message.
        private static (string Message, object? Errors) GetResponseBody(Exception exception)
        {
            if (exception is ApplicationValidationException validationException)
            {
                return (validationException.Message, validationException.Errors);
            }

            if (IsKnownApplicationException(exception))
            {
                return (exception.Message, new List<string> { exception.Message });
            }

            return ("An unexpected error occurred.", new List<string> { exception.Message });
        }

        private static bool IsKnownApplicationException(Exception exception) => exception is
            NotFoundException or
            ConflictException or
            ForbiddenAccessException or
            UnauthorizedException or
            BadRequestException or
            UnprocessableEntityException or
            TooManyRequestsException or
            ServiceUnavailableException;

        private static int GetStatusCode(Exception exception) => exception switch
        {
            // Application exceptions - the replacement for the old ResultType-driven mapping
            NotFoundException => (int)HttpStatusCode.NotFound,
            ConflictException => (int)HttpStatusCode.Conflict,
            ForbiddenAccessException => (int)HttpStatusCode.Forbidden,
            UnauthorizedException => (int)HttpStatusCode.Unauthorized,
            BadRequestException => (int)HttpStatusCode.BadRequest,
            ApplicationValidationException => (int)HttpStatusCode.BadRequest, // matches old ResultType.ValidationError -> 400
            UnprocessableEntityException => 422, // matches old (unused) ResultType.ValidationFailed
            TooManyRequestsException => 429,
            ServiceUnavailableException => 503,

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
            DataAnnotationsValidationException => 422,

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
