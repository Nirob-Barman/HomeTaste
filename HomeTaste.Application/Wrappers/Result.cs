namespace HomeTaste.Application.Wrappers
{
    /// <summary>
    /// Success-shaped return value for CQRS handlers. Failures are signaled by throwing
    /// one of the Application.Common.Exceptions types instead of returning a failed Result -
    /// the API-layer ExceptionMiddleware maps those to the appropriate HTTP status.
    ///
    /// ResultType is DEPRECATED and only kept so the not-yet-converted services still compile
    /// during the migration - converted handlers never set it (it stays at its default). It gets
    /// deleted, along with this property, once every service has moved off it.
    /// </summary>
    public class Result<T>
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public T? Data { get; set; }
        public List<string>? Errors { get; set; }
        public ResultType ResultType { get; set; }

        public static Result<T> Ok(T? data, string? message = null, ResultType resultType = ResultType.Success)
        {
            return new Result<T>
            {
                Success = true,
                Data = data,
                Message = message,
                ResultType = resultType
            };
        }

        public static Result<T> Fail(string error, string? message = null, ResultType resultType = ResultType.Failure)
        {
            return new Result<T>
            {
                Success = false,
                Errors = new List<string> { error },
                Message = message,
                ResultType = resultType
            };
        }

        public static Result<T> Fail(List<string> errors, string? message = null, ResultType resultType = ResultType.Failure)
        {
            return new Result<T>
            {
                Success = false,
                Errors = errors,
                Message = message,
                ResultType = resultType
            };
        }

        public static Result<T> From(bool success, T? data = default, string? message = null, List<string>? errors = null, ResultType resultType = ResultType.Success)
        {
            return new Result<T>
            {
                Success = success,
                Data = data,
                Message = message,
                Errors = errors,
                ResultType = resultType
            };
        }
    }

    public enum ResultType
    {
        Success,
        ValidationError,
        Unauthorized,
        Forbidden,
        NotFound,
        Conflict,
        Failure,
        Created,
        NoContent,
        ValidationFailed,
        TooManyRequests,
        BadRequest,
        ServiceUnavailable
    }
}
