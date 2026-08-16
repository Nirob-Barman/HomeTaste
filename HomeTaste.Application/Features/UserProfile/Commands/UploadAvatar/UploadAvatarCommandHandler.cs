using HomeTaste.Application.Common.Exceptions;
using HomeTaste.Application.Interfaces;
using HomeTaste.Application.Interfaces.Auth;
using HomeTaste.Application.Interfaces.FileStorage;
using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.UserProfile.Commands.UploadAvatar
{
    public class UploadAvatarCommandHandler : IRequestHandler<UploadAvatarCommand, Result<UploadAvatarResponse>>
    {
        private readonly IUserManager _userManager;
        private readonly IUserContextService _userContextService;
        private readonly IFileStorage _fileStorage;

        public UploadAvatarCommandHandler(IUserManager userManager, IUserContextService userContextService, IFileStorage fileStorage)
        {
            _userManager = userManager;
            _userContextService = userContextService;
            _fileStorage = fileStorage;
        }

        public async Task<Result<UploadAvatarResponse>> Handle(UploadAvatarCommand command, CancellationToken cancellationToken)
        {
            var userId = _userContextService.UserId;
            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedException("Unauthorized");

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                throw new NotFoundException("User not found");

            // Delete previous avatar from storage if one exists
            if (!string.IsNullOrEmpty(user.ProfileImagePublicId))
                await _fileStorage.DeleteFileAsync(user.ProfileImagePublicId);

            var upload = await _fileStorage.UploadFileAsync(command.Content, command.FileName, "avatars");

            user.ProfileImageUrl = upload.Url;
            user.ProfileImagePublicId = upload.PublicId;

            var (succeeded, errors) = await _userManager.UpdateAsync(user);
            if (!succeeded)
                throw new ServerErrorException(string.Join(" ", errors));

            return Result<UploadAvatarResponse>.Ok(
                new UploadAvatarResponse { ProfileImageUrl = upload.Url! },
                "Avatar uploaded successfully");
        }
    }
}
