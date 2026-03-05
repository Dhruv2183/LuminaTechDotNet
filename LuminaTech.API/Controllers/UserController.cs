using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LuminaTech.Data;
using System.Security.Claims;

namespace LuminaTech.API.Controllers;

[ApiController]
[Route("api/users")]
[Authorize]
public class UserController : ControllerBase
{
    private readonly LuminaTechDbContext _context;

    public UserController(LuminaTechDbContext context)
    {
        _context = context;
    }

    private int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst("userId")?.Value;
        return int.Parse(userIdClaim ?? "0");
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetProfile()
    {
        var userId = GetCurrentUserId();
        var user = await _context.Users.FindAsync(userId);
        if (user == null) return NotFound();

        return Ok(new
        {
            user.Id,
            user.Email,
            user.FullName,
            user.ProfilePictureUrl,
            user.Role,
            user.CreatedAt
        });
    }

    [HttpGet("me/wishlist")]
    public async Task<IActionResult> GetWishlist()
    {
        var userId = GetCurrentUserId();
        var wishlist = await _context.Wishlists
            .Where(w => w.UserId == userId)
            .Include(w => w.Product)
            .Select(w => new
            {
                w.Product.Id,
                w.Product.Name,
                w.Product.Description,
                w.Product.Price,
                w.Product.Category,
                w.Product.ImageUrl,
                w.AddedAt
            })
            .ToListAsync();

        return Ok(wishlist);
    }

    [HttpPost("me/wishlist/{productId}")]
    public async Task<IActionResult> AddToWishlist(int productId)
    {
        var userId = GetCurrentUserId();
        var exists = await _context.Wishlists
            .AnyAsync(w => w.UserId == userId && w.ProductId == productId);

        if (exists) return BadRequest(new { message = "Product already in wishlist" });

        var product = await _context.Products.FindAsync(productId);
        if (product == null) return NotFound(new { message = "Product not found" });

        _context.Wishlists.Add(new Data.Entities.Wishlist
        {
            UserId = userId,
            ProductId = productId,
            AddedAt = DateTime.UtcNow
        });

        await _context.SaveChangesAsync();
        return Ok(new { message = "Added to wishlist" });
    }

    [HttpDelete("me/wishlist/{productId}")]
    public async Task<IActionResult> RemoveFromWishlist(int productId)
    {
        var userId = GetCurrentUserId();
        var item = await _context.Wishlists
            .FirstOrDefaultAsync(w => w.UserId == userId && w.ProductId == productId);

        if (item == null) return NotFound();

        _context.Wishlists.Remove(item);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
