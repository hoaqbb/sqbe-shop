using CloudinaryDotNet.Actions;

namespace API.Interfaces
{
    public interface IImageService
    {
        Task<ImageUploadResult> AddImageAsync(IFormFile file);
        Task<List<ImageUploadResult>> AddMultipleImagesAsync(IFormFile[] files);
        Task<DeletionResult> DeleteImageAsync(string publicId);
        Task<List<DeletionResult>> DeleteMultipleImagesAsync(List<string> publicIds);
    }
}
