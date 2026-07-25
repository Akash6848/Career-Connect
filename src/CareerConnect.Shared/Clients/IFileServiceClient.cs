using Refit;

namespace CareerConnect.Shared.Clients;

/// <summary>
/// Typed Refit contract for calling CareerConnect.FileService, shared by every service that
/// uploads or deletes files so the HTTP surface is defined exactly once.
/// </summary>
public interface IFileServiceClient
{
    [Multipart]
    [Post("/api/files")]
    Task<string> UploadFileAsync([AliasAs("file")] StreamPart file);

    [Post("/api/files/batch-delete")]
    Task BatchDeleteFilesAsync([Body] BatchFileDeleteRequest request);

    [Delete("/api/files/{id}")]
    Task DeleteFileByIdAsync(string id);
}

public record BatchFileDeleteRequest(string[] FileIds);
