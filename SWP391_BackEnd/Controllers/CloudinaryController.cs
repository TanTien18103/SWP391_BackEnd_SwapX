using BusinessObjects.Dtos;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Mvc;

namespace SWP391_BackEnd.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CloudinaryController : ControllerBase
    {
        private readonly Cloudinary _cloudinary;

        public CloudinaryController(Cloudinary cloudinary)
        {
            _cloudinary = cloudinary;
        }

        // POST: api/cloudinary/upload
        [HttpPost("upload")]
        public async Task<IActionResult> Upload([FromForm] CloudinaryUploadRequestDto request)
        {
            if (request.File == null || request.File.Length == 0)
                return BadRequest("No file uploaded.");

            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(request.File.FileName, request.File.OpenReadStream()),
                Folder = request.Folder
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);

            var response = new CloudinaryUploadResponseDto
            {
                PublicId = uploadResult.PublicId,
                Url = uploadResult.Url?.ToString(),
                SecureUrl = uploadResult.SecureUrl?.ToString(),
                Format = uploadResult.Format,
                Bytes = uploadResult.Bytes
            };

            return Ok(response);
        }

        // DELETE: api/cloudinary/delete/{publicId}
        [HttpDelete("delete/{publicId}")]
        public async Task<IActionResult> Delete(string publicId)
        {
            var delParams = new DeletionParams(publicId);
            var delResult = await _cloudinary.DestroyAsync(delParams);

            var response = new CloudinaryDeleteResponseDto
            {
                PublicId = publicId,
                Status = delResult.Result
            };

            return Ok(response);
        }
    }
}