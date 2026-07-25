using CareerConnect.FileService.Services;
using CareerConnect.Shared.Clients;
using Microsoft.AspNetCore.Mvc;

namespace CareerConnect.FileService.Controllers;

[ApiController]
[Route("api/files")]
public class FilesController(IFileUploadService fileUploadService) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<string>> UploadFile(IFormFile file) =>
        Ok(await fileUploadService.UploadFileAsync(file));

    [HttpPost("batch-delete")]
    public async Task<IActionResult> BatchDeleteFiles([FromBody] BatchFileDeleteRequest request)
    {
        await fileUploadService.DeleteBatchFilesAsync(request.FileIds);
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteFileById(string id)
    {
        await fileUploadService.DeleteFileByIdAsync(id);
        return Ok();
    }
}
