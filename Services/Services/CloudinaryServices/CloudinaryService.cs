using BusinessObjects.Dtos;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Services.ApiModels;

namespace Services;

public class CloudinaryService : ICloudinaryService
{
    private readonly Cloudinary _cloudinary;

    public CloudinaryService(Cloudinary cloudinary)
    {
        _cloudinary = cloudinary;
    }

    public async Task<ResultModel<CloudinaryUploadResponseDto>> UploadAsync(CloudinaryUploadRequestDto request)
    {
        if (request.File == null || request.File.Length == 0)
        {
            return new ResultModel<CloudinaryUploadResponseDto>
            {
                IsSuccess = false,
                StatusCode = 400,
                Message = "No file uploaded."
            };
        }

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

        return new ResultModel<CloudinaryUploadResponseDto>
        {
            IsSuccess = true,
            StatusCode = 200,
            Message = "Upload successful",
            Data = response
        };
    }

    public async Task<ResultModel<CloudinaryDeleteResponseDto>> DeleteAsync(string publicId)
    {
        var delParams = new DeletionParams(publicId);
        var delResult = await _cloudinary.DestroyAsync(delParams);

        var response = new CloudinaryDeleteResponseDto
        {
            PublicId = publicId,
            Status = delResult.Result
        };

        return new ResultModel<CloudinaryDeleteResponseDto>
        {
            IsSuccess = delResult.Result == "ok",
            StatusCode = delResult.Result == "ok" ? 200 : 400,
            Message = delResult.Result == "ok" ? "Delete successful" : "Delete failed",
            Data = response
        };
    }
}
