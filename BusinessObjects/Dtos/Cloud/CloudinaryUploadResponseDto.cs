namespace BusinessObjects.Dtos;

public class CloudinaryUploadResponseDto
{
    public string PublicId { get; set; }
    public string Url { get; set; }
    public string SecureUrl { get; set; }
    public string Format { get; set; }
    public long Bytes { get; set; }
}   