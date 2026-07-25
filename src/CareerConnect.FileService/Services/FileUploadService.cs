using System.Net;
using CareerConnect.Shared.Exceptions;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;

namespace CareerConnect.FileService.Services;

public class FileUploadService(Cloudinary cloudinary) : IFileUploadService
{
    public async Task<string> UploadFileAsync(IFormFile file)
    {
        try
        {
            await using var stream = file.OpenReadStream();

            // RawUploadParams uploads with resource_type "raw", which accepts any file type -
            // resumes, images, video - without Cloudinary rejecting non-image content.
            var uploadParams = new RawUploadParams
            {
                File = new FileDescription(file.FileName, stream)
            };

            var result = await cloudinary.UploadAsync(uploadParams);

            if (result.Error is not null)
            {
                throw new ApiException(HttpStatusCode.BadRequest, "Error while uploading file");
            }

            return result.SecureUrl.ToString();
        }
        catch (ApiException)
        {
            throw;
        }
        catch (Exception)
        {
            throw new ApiException(HttpStatusCode.BadRequest, "Error while uploading file");
        }
    }

    public async Task DeleteFileByIdAsync(string fileId)
    {
        try
        {
            var result = await cloudinary.DeleteResourcesAsync(new DelResParams
            {
                PublicIds = [fileId]
            });

            if (result.Error is not null)
            {
                throw new ApiException(HttpStatusCode.BadRequest, $"File with id {fileId} was not found");
            }
        }
        catch (ApiException)
        {
            throw;
        }
        catch (Exception)
        {
            throw new ApiException(HttpStatusCode.BadRequest, $"File with id {fileId} was not found");
        }
    }

    public async Task DeleteBatchFilesAsync(string[] fileIds)
    {
        if (fileIds.Length == 0)
        {
            throw new ApiException(HttpStatusCode.BadRequest, "fileIds not found or empty");
        }

        try
        {
            var result = await cloudinary.DeleteResourcesAsync(new DelResParams
            {
                PublicIds = fileIds.ToList()
            });

            if (result.Error is not null)
            {
                throw new ApiException(HttpStatusCode.InternalServerError, "Error batch deleting files");
            }
        }
        catch (ApiException)
        {
            throw;
        }
        catch (Exception)
        {
            throw new ApiException(HttpStatusCode.InternalServerError, "Error batch deleting files");
        }
    }
}
