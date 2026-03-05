using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace LuminaTech.Services;

public interface ICloudinaryService
{
    Task<(string Url, string PublicId)> UploadImageAsync(IFormFile file);
    Task<bool> DeleteImageAsync(string publicId);
    string GetOptimizedUrl(string publicId, int width = 400);
}

public class CloudinaryService : ICloudinaryService
{
    private readonly Cloudinary _cloudinary;

    public CloudinaryService(IConfiguration configuration)
    {
        var cloudName = configuration["Cloudinary:CloudName"] ?? "demo";
        var apiKey = configuration["Cloudinary:ApiKey"] ?? "demo";
        var apiSecret = configuration["Cloudinary:ApiSecret"] ?? "demo";

        var account = new Account(cloudName, apiKey, apiSecret);
        _cloudinary = new Cloudinary(account);
    }

    public async Task<(string Url, string PublicId)> UploadImageAsync(IFormFile file)
    {
        using var stream = file.OpenReadStream();
        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(file.FileName, stream),
            Folder = "luminatech/products",
            Transformation = new Transformation()
                .Quality("auto")
                .FetchFormat("auto")
        };

        var result = await _cloudinary.UploadAsync(uploadParams);

        if (result.Error != null)
            throw new Exception($"Cloudinary upload failed: {result.Error.Message}");

        return (result.SecureUrl.ToString(), result.PublicId);
    }

    public async Task<bool> DeleteImageAsync(string publicId)
    {
        var deleteParams = new DeletionParams(publicId);
        var result = await _cloudinary.DestroyAsync(deleteParams);
        return result.Result == "ok";
    }

    public string GetOptimizedUrl(string publicId, int width = 400)
    {
        return _cloudinary.Api.UrlImgUp
            .Transform(new Transformation()
                .Width(width)
                .Crop("scale")
                .Quality("auto")
                .FetchFormat("auto"))
            .BuildUrl(publicId);
    }
}
