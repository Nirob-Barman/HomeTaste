using HomeTaste.Application.DTOs.Auth;
using HomeTaste.Application.DTOs.UserProfile;
using HomeTaste.Application.Interfaces;
using HomeTaste.Application.Interfaces.Auth;
using HomeTaste.Application.Interfaces.FileStorage;
using HomeTaste.Application.Interfaces.UserProfile;
using HomeTaste.Application.Validators.UserProfile;
using HomeTaste.Application.Wrappers;

namespace HomeTaste.Application.Services.UserProfile
{
    public class UserProfileService : IUserProfileService
    {
        private readonly IUserManager        _userManager;
        private readonly IUserContextService _userContextService;
        private readonly IFileStorage        _fileStorage;

        public UserProfileService(
            IUserManager userManager,
            IUserContextService userContextService,
            IFileStorage fileStorage)
        {
            _userManager        = userManager;
            _userContextService = userContextService;
            _fileStorage        = fileStorage;
        }

        public async Task<Result<UserProfileResponse>> GetProfileAsync()
        {
            var userId = _userContextService.UserId;
            if (string.IsNullOrEmpty(userId))
                return Result<UserProfileResponse>.Fail("Unauthorized", "Unauthorized", ResultType.Unauthorized);

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return Result<UserProfileResponse>.Fail("User not found", "Not found", ResultType.NotFound);

            var roles = await _userManager.GetRolesAsync(user);

            return Result<UserProfileResponse>.Ok(new UserProfileResponse
            {
                Id              = user.Id,
                Email           = user.Email,
                FirstName       = user.FirstName,
                LastName        = user.LastName,
                DateOfBirth     = user.DateOfBirth,
                PhoneNumber     = user.PhoneNumber,
                ProfileImageUrl = user.ProfileImageUrl,
                Roles           = roles.ToList()
            }, "Profile retrieved successfully");
        }

        public async Task<Result<UserProfileResponse>> UpdateProfileAsync(UpdateProfileRequest request)
        {
            var validationErrors = UpdateProfileRequestValidator.Validate(request);
            if (validationErrors.Count > 0)
                return Result<UserProfileResponse>.Fail(string.Join(" ", validationErrors), "Validation failed", ResultType.ValidationError);

            var userId = _userContextService.UserId;
            if (string.IsNullOrEmpty(userId))
                return Result<UserProfileResponse>.Fail("Unauthorized", "Unauthorized", ResultType.Unauthorized);

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return Result<UserProfileResponse>.Fail("User not found", "Not found", ResultType.NotFound);

            if (request.FirstName   != null) user.FirstName   = request.FirstName;
            if (request.LastName    != null) user.LastName     = request.LastName;
            if (request.DateOfBirth.HasValue) user.DateOfBirth = request.DateOfBirth;
            if (request.PhoneNumber != null) user.PhoneNumber  = request.PhoneNumber;

            var (succeeded, errors) = await _userManager.UpdateAsync(user);
            if (!succeeded)
                return Result<UserProfileResponse>.Fail(errors, "Profile update failed", ResultType.Failure);

            var roles = await _userManager.GetRolesAsync(user);

            return Result<UserProfileResponse>.Ok(new UserProfileResponse
            {
                Id              = user.Id,
                Email           = user.Email,
                FirstName       = user.FirstName,
                LastName        = user.LastName,
                DateOfBirth     = user.DateOfBirth,
                PhoneNumber     = user.PhoneNumber,
                ProfileImageUrl = user.ProfileImageUrl,
                Roles           = roles.ToList()
            }, "Profile updated successfully");
        }

        public async Task<Result<bool>> ChangePasswordAsync(ChangePasswordRequest request)
        {
            var validationErrors = ChangePasswordRequestValidator.Validate(request);
            if (validationErrors.Count > 0)
                return Result<bool>.Fail(string.Join(" ", validationErrors), "Validation failed", ResultType.ValidationError);

            var userId = _userContextService.UserId;
            if (string.IsNullOrEmpty(userId))
                return Result<bool>.Fail("Unauthorized", "Unauthorized", ResultType.Unauthorized);

            var (succeeded, errors) = await _userManager.ChangePasswordAsync(userId, request.CurrentPassword, request.NewPassword);
            if (!succeeded)
                return Result<bool>.Fail(errors, "Password change failed", ResultType.Failure);

            return Result<bool>.Ok(true, "Password changed successfully");
        }

        public async Task<Result<UploadAvatarResponse>> UploadAvatarAsync(Stream content, string fileName, string contentType)
        {
            var userId = _userContextService.UserId;
            if (string.IsNullOrEmpty(userId))
                return Result<UploadAvatarResponse>.Fail("Unauthorized", "Unauthorized", ResultType.Unauthorized);

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return Result<UploadAvatarResponse>.Fail("User not found", "Not found", ResultType.NotFound);

            // Delete previous avatar from storage if one exists
            if (!string.IsNullOrEmpty(user.ProfileImagePublicId))
                await _fileStorage.DeleteFileAsync(user.ProfileImagePublicId);

            var upload = await _fileStorage.UploadFileAsync(content, fileName, "avatars");

            user.ProfileImageUrl      = upload.Url;
            user.ProfileImagePublicId = upload.PublicId;

            var (succeeded, errors) = await _userManager.UpdateAsync(user);
            if (!succeeded)
                return Result<UploadAvatarResponse>.Fail(errors, "Avatar update failed", ResultType.Failure);

            return Result<UploadAvatarResponse>.Ok(
                new UploadAvatarResponse { ProfileImageUrl = upload.Url! },
                "Avatar uploaded successfully");
        }
    }
}
