using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using Microsoft.Extensions.Logging;
using TopGear.Application.Interfaces;

namespace TopGear.Infrastructure.Storage;

public class CloudinaryImageUploadService(Cloudinary cloudinary, ILogger<CloudinaryImageUploadService> logger): IImageUploadService
{
    public async Task<string> UploadAsync(Stream file, string fileName, string folder)
    {
        logger.LogInformation("Creating new Image Upload Params");

        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(fileName, file),
            Folder = folder
        };

        logger.LogInformation("Uploading image to cloudinary");

        var result = await cloudinary.UploadAsync(uploadParams);
        return result.SecureUrl.ToString();
    }
}
