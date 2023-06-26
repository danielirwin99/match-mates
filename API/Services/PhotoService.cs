using API.Helpers;
using API.Interfaces;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Options;

namespace API.Services
{
    // Implement interface
    public class PhotoService : IPhotoService
    {
        private readonly Cloudinary _cloudinary;
        // Accessing our class
        // We are calling it config at the end
        public PhotoService(IOptions<CloudinarySettings> config)
        {
            // Syncs our Account to C#
            var acc = new Account(
                config.Value.CloudName, config.Value.ApiKey, config.Value.ApiSecret
            );

            // Setting our private field above to the account
            _cloudinary = new Cloudinary(acc);
        }

        public async Task<ImageUploadResult> AddPhotoAsync(IFormFile file)
        {
            var uploadResult = new ImageUploadResult();

            // If the file is greater than 0 then we know we are working with a file
            if (file.Length > 0)
            {
                // "using" disposes of the function after we use it so it doesn't use up memory
                using var stream = file.OpenReadStream();
                // Our Parameters of the Image
                var UploadParams = new ImageUploadParams
                {
                    // Name of files --> Will be set dynamically to what they are
                    File = new FileDescription(file.FileName, stream),
                    // Our Styling Parameters
                    Transformation = new Transformation().Height(500).Width(500).Crop("fill").Gravity("face"),
                    // Our folder name
                    Folder = "da--net7"
                };
                // Saves the upload data
                uploadResult = await _cloudinary.UploadAsync(UploadParams);
            }
            // Uploads the data / image
            return uploadResult;
        }

        public async Task<DeletionResult> DeletePhtoAsync(string publicId)
        {
            // Deleting via the params of the public id of the photo
            var deleteParams = new DeletionParams(publicId);

            // Deletes it
            return await _cloudinary.DestroyAsync(deleteParams);
        }
    }
}