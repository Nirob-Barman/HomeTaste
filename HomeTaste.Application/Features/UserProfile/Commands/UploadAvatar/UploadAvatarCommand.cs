using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.UserProfile.Commands.UploadAvatar
{
    public class UploadAvatarCommand : IRequest<Result<UploadAvatarResponse>>
    {
        public Stream Content { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }

        public UploadAvatarCommand(Stream content, string fileName, string contentType)
        {
            Content = content;
            FileName = fileName;
            ContentType = contentType;
        }
    }
}
