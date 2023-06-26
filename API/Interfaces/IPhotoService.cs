using CloudinaryDotNet.Actions;

namespace API.Interfaces
{
    public interface IPhotoService
    {
        // IFormFile --> Represents a file Http Request
        Task<ImageUploadResult> AddPhotoAsync(IFormFile file);
        
        // This is connected to our our Photo.cs publicId
        Task<DeletionResult> DeletePhtoAsync(string publicId);
    }
}