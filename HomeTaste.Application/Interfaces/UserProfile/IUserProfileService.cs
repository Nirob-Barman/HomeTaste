using HomeTaste.Application.DTOs.Auth;
using HomeTaste.Application.DTOs.UserProfile;
using HomeTaste.Application.Wrappers;

namespace HomeTaste.Application.Interfaces.UserProfile
{
    public interface IUserProfileService
    {
        Task<Result<UserProfileResponse>> GetProfileAsync();
        Task<Result<UserProfileResponse>> UpdateProfileAsync(UpdateProfileRequest request);
        Task<Result<bool>> ChangePasswordAsync(ChangePasswordRequest request);
        Task<Result<UploadAvatarResponse>> UploadAvatarAsync(Stream content, string fileName, string contentType);
    }
}
