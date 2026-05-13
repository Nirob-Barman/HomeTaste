

namespace HomeTaste.Application.Common
{
    public static class MessageConstants
    {
        public static class Registration
        {
            public const string ValidationFailed = "Validation failed";
            public const string UserAlreadyExists = "A user with this email already exists.";
            public const string AdminRoleNotAllowed = "You are not allowed to assign the 'Admin' role.";
            public const string UserCreationFailed = "User creation failed: {0}";
        }

        public static class Login
        {
            public const string ValidationFailed = "Validation failed";
            public const string InvalidUsername = "Invalid username";
            public const string InvalidPassword = "Invalid password";
            public const string LoginSuccessful = "Login successful";
        }

        public static class RefreshToken
        {
            public const string RefreshTokenNotFound = "Refresh token not found";
            public const string RefreshTokenRevoked = "Refresh token revoked";
            public const string RefreshTokenExpired = "Refresh token expired";
            public const string TokenRefreshedSuccessfully = "Token refreshed successfully";
        }

        public static class Logout
        {
            public const string LogoutSuccessful = "Logout successful";
            public const string InvalidRefreshToken = "Invalid refresh token";
        }

        public static class UserProfile
        {
            public const string UserNotFound = "User not found";
            public const string UserProfileRetrievedSuccessfully = "User profile retrieved successfully";
        }
    }
}
