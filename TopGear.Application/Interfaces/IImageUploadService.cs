namespace TopGear.Application.Interfaces;

public interface IImageUploadService
{
    Task<string> UploadAsync(Stream file, string fileName, string folder);
}
