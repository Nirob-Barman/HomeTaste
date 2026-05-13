using HomeTaste.Application.DTOs.File;

namespace HomeTaste.Application.Interfaces.FileStorage
{
    public interface IFileStorage
    {
        Task<FileUploadResult> UploadFileAsync(Stream content, string fileName, string folder);
        Task<bool> DeleteFileAsync(string filePath);
    }
}