using Microsoft.AspNetCore.Http;

namespace CareerConnect.FileService.Services;

public interface IFileUploadService
{
    Task<string> UploadFileAsync(IFormFile file);
    Task DeleteFileByIdAsync(string fileId);
    Task DeleteBatchFilesAsync(string[] fileIds);
}
