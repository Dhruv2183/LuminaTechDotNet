using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LuminaTech.Data;
using LuminaTech.Data.Entities;

namespace LuminaTech.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class WishlistController : ControllerBase
{
    private readonly LuminaTechDbContext _context;

    public WishlistController(LuminaTechDbContext context)
    {
        _context = context;
    }

    private int GetUserId() =>
        int.Parse(HttpContext.User.FindFirst("userId")?.Value ?? "0");

    [HttpGet]
    public async Task<IActionResult> GetWishlist()
    {
        var userId = GetUserId();
        var items = await _context.Wishlists
            .Where(w => w.UserId == userId)
            .Include(w => w.Product)
            .OrderByDescending(w => w.AddedAt)
            .Select(w => new
            {
                w.ProductId,
                w.AddedAt,
                Product = new
                {
                    w.Product.Id,
                    w.Product.Name,
                    w.Product.Description,
                    w.Product.Price,
                    w.Product.ImageUrl,
                    w.Product.Category
                }
            })
            .ToListAsync();

        return Ok(items);
    }

    [HttpPost]
    public async Task<IActionResult> AddToWishlist([FromBody] WishlistDto dto)
    {
        var userId = GetUserId();
        var exists = await _context.Wishlists
            .AnyAsync(w => w.UserId == userId && w.ProductId == dto.ProductId);

        if (exists)
            return Ok(new { message = "Already in wishlist" });

        _context.Wishlists.Add(new Wishlist
        {
            UserId = userId,
            ProductId = dto.ProductId,
            AddedAt = DateTime.UtcNow
        });
        await _context.SaveChangesAsync();
        return Ok(new { message = "Added to wishlist" });
    }

    [HttpDelete("{productId}")]
    public async Task<IActionResult> RemoveFromWishlist(int productId)
    {
        var userId = GetUserId();
        var item = await _context.Wishlists
            .FirstOrDefaultAsync(w => w.UserId == userId && w.ProductId == productId);

        if (item == null) return NotFound();

        _context.Wishlists.Remove(item);
        await _context.SaveChangesAsync();
        return Ok(new { message = "Removed from wishlist" });
    }

    [HttpPost("move-to-cart/{productId}")]
    public async Task<IActionResult> MoveToCart(int productId)
    {
        var userId = GetUserId();
        var wishItem = await _context.Wishlists
            .FirstOrDefaultAsync(w => w.UserId == userId && w.ProductId == productId);

        if (wishItem == null) return NotFound();

        // Add to cart
        var cartItem = await _context.CartItems
            .FirstOrDefaultAsync(c => c.UserId == userId && c.ProductId == productId);

        if (cartItem != null)
        {
            cartItem.Quantity += 1;
        }
        else
        {
            _context.CartItems.Add(new CartItem
            {
                UserId = userId,
                ProductId = productId,
                Quantity = 1
            });
        }

        // Remove from wishlist
        _context.Wishlists.Remove(wishItem);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Moved to cart" });
    }
}

public class WishlistDto
{
    public int ProductId { get; set; }
}
