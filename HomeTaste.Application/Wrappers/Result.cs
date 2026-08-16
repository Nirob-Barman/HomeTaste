namespace HomeTaste.Application.Wrappers
{
    /// <summary>
    /// Success-shaped return value for CQRS handlers. Failures are signaled by throwing
    /// one of the Application.Common.Exceptions types instead of returning a failed Result -
    /// the API-layer ExceptionMiddleware maps those to the appropriate HTTP status.
    /// </summary>
    public class Result<T>
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public T? Data { get; set; }
        public List<string>? Errors { get; set; }

        public static Result<T> Ok(T? data, string? message = null)
        {
            return new Result<T>
            {
                Success = true,
                Data = data,
                Message = message
            };
        }

        public static Result<T> Fail(string error, string? message = null)
        {
            return new Result<T>
            {
                Success = false,
                Errors = new List<string> { error },
                Message = message
            };
        }

        public static Result<T> Fail(List<string> errors, string? message = null)
        {
            return new Result<T>
            {
                Success = false,
                Errors = errors,
                Message = message
            };
        }

        public static Result<T> From(bool success, T? data = default, string? message = null, List<string>? errors = null)
        {
            return new Result<T>
            {
                Success = success,
                Data = data,
                Message = message,
                Errors = errors
            };
        }
    }
}
