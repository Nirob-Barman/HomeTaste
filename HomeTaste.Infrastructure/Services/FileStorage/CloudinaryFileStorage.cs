using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using HomeTaste.Application.DTOs.File;
using HomeTaste.Application.Interfaces.FileStorage;

namespace HomeTaste.Infrastructure.Services.FileStorage
{
    public class CloudinaryFileStorage : IFileStorage
    {
        private readonly Cloudinary _cloudinary;

        public CloudinaryFileStorage(Cloudinary cloudinary)
        {
            _cloudinary = cloudinary;
        }

        public async Task<FileUploadResult> UploadFileAsync(Stream content, string fileName, string folder)
        {
            try
            {
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(fileName, content),
                    Folder = folder
                };

                var result = await _cloudinary.UploadAsync(uploadParams);

                if (result.StatusCode != System.Net.HttpStatusCode.OK)
                    throw new Exception("Cloudinary upload failed.");

                return new FileUploadResult
                {
                    Url = result.SecureUrl.ToString(),
                    PublicId = result.PublicId,
                };
            }
            catch
            {
                return null!;
            }
        }

        public async Task<bool> DeleteFileAsync(string publicId)
        {
            try
            {
                if (string.IsNullOrEmpty(publicId))
                    throw new Exception("Invalid file path. Could not extract public ID.");

                var deleteParams = new DeletionParams(publicId);
                var result = await _cloudinary.DestroyAsync(deleteParams);

                if (result.StatusCode != System.Net.HttpStatusCode.OK)
                    throw new Exception("Cloudinary file deletion failed.");

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
