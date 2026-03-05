using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LuminaTech.Services;

namespace LuminaTech.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class MediaController : ControllerBase
{
    private readonly ICloudinaryService _cloudinaryService;

    public MediaController(ICloudinaryService cloudinaryService)
    {
        _cloudinaryService = cloudinaryService;
    }

    [HttpPost("upload")]
    public async Task<IActionResult> Upload(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest(new { message = "No file provided" });

        if (file.Length > 10 * 1024 * 1024) // 10MB limit
            return BadRequest(new { message = "File size exceeds 10MB limit" });

        var allowedTypes = new[] { "image/jpeg", "image/png", "image/webp", "image/gif" };
        if (!allowedTypes.Contains(file.ContentType))
            return BadRequest(new { message = "Invalid file type. Allowed: JPEG, PNG, WebP, GIF" });

        try
        {
            var (url, publicId) = await _cloudinaryService.UploadImageAsync(file);
            return Ok(new { url, publicId });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = $"Upload failed: {ex.Message}" });
        }
    }

    [HttpDelete("{publicId}")]
    public async Task<IActionResult> Delete(string publicId)
    {
        var success = await _cloudinaryService.DeleteImageAsync(publicId);
        if (!success) return StatusCode(500, new { message = "Failed to delete image" });
        return NoContent();
    }
}
