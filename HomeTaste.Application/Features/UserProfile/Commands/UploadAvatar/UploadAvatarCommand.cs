using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.UserProfile.Commands.UploadAvatar
{
    public record UploadAvatarCommand(Stream Content, string FileName, string ContentType)
        : IRequest<Result<UploadAvatarResponse>>;
}
