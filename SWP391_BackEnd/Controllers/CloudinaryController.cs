using BusinessObjects.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Services.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CloudinaryController : ControllerBase
{
    private readonly ICloudinaryService _cloudinaryService;

    public CloudinaryController(ICloudinaryService cloudinaryService)
    {
        _cloudinaryService = cloudinaryService;
    }

    [HttpPost("upload")]
    public async Task<IActionResult> Upload([FromForm] CloudinaryUploadRequestDto request)
    {
        var result = await _cloudinaryService.UploadAsync(request);
        return StatusCode(result.StatusCode, result);
    }

    [HttpDelete("delete/{publicId}")]
    public async Task<IActionResult> Delete(string publicId)
    {
        var result = await _cloudinaryService.DeleteAsync(publicId);
        return StatusCode(result.StatusCode, result);
    }
}