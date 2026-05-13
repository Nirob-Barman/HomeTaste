
namespace HomeTaste.API.Wrappers
{
    public class ApiResponse<T>
    {
        public int StatusCode { get; set; }
        public string? Message { get; set; }
        public bool Success { get; set; }
        public T? Data { get; set; }
        public object? Errors { get; set; }

        public ApiResponse() { }

        public ApiResponse(T data, string message = "", int statusCode = 200)
        {
            Data = data;
            Message = message;
            Success = statusCode is >= 200 and < 300;
            StatusCode = statusCode;
        }

        public static ApiResponse<T> FailResponse(string message, object? errors = null, int statusCode = 400)
        {
            return new ApiResponse<T>
            {
                Message = message,
                Success = false,
                StatusCode = statusCode,
                Errors = errors
            };
        }

        public static ApiResponse<T> SuccessResponse(T data, string message = "", int statusCode = 200)
        {
            if (typeof(T) == typeof(string) && string.IsNullOrWhiteSpace(message) && data is string strData)
            {
                message = strData;
            }
            return new ApiResponse<T>(data, message, statusCode);
        }

        public static ApiResponse<T> CreatedResponse(T data, string message = "Resource created")
        {
            return new ApiResponse<T>(data, message, 201);
        }

        public static ApiResponse<T> NoContentResponse(string message = "No Content")
        {
            return new ApiResponse<T>(default!, message, 204)
            {
                Success = true
            };
        }
    }
}
