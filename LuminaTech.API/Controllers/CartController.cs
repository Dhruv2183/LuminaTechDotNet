using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LuminaTech.Data;
using LuminaTech.Data.Entities;

namespace LuminaTech.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CartController : ControllerBase
{
    private readonly LuminaTechDbContext _context;

    public CartController(LuminaTechDbContext context)
    {
        _context = context;
    }

    private int GetUserId() =>
        int.Parse(HttpContext.User.FindFirst("userId")?.Value ?? "0");

    [HttpGet]
    public async Task<IActionResult> GetCart()
    {
        var userId = GetUserId();
        var items = await _context.CartItems
            .Where(c => c.UserId == userId)
            .Include(c => c.Product)
            .OrderByDescending(c => c.AddedAt)
            .Select(c => new
            {
                c.Id,
                c.ProductId,
                c.Quantity,
                c.AddedAt,
                Product = new
                {
                    c.Product.Id,
                    c.Product.Name,
                    c.Product.Price,
                    c.Product.ImageUrl,
                    c.Product.Category
                }
            })
            .ToListAsync();

        var total = items.Sum(i => i.Product.Price * i.Quantity);
        return Ok(new { items, total });
    }

    [HttpPost]
    public async Task<IActionResult> AddToCart([FromBody] AddToCartDto dto)
    {
        var userId = GetUserId();
        var existing = await _context.CartItems
            .FirstOrDefaultAsync(c => c.UserId == userId && c.ProductId == dto.ProductId);

        if (existing != null)
        {
            existing.Quantity += dto.Quantity > 0 ? dto.Quantity : 1;
            await _context.SaveChangesAsync();
            return Ok(new { message = "Cart updated", quantity = existing.Quantity });
        }

        var item = new CartItem
        {
            UserId = userId,
            ProductId = dto.ProductId,
            Quantity = dto.Quantity > 0 ? dto.Quantity : 1
        };
        _context.CartItems.Add(item);
        await _context.SaveChangesAsync();
        return Ok(new { message = "Added to cart" });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateQuantity(int id, [FromBody] UpdateQuantityDto dto)
    {
        var userId = GetUserId();
        var item = await _context.CartItems
            .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);
        if (item == null) return NotFound();

        if (dto.Quantity <= 0)
        {
            _context.CartItems.Remove(item);
        }
        else
        {
            item.Quantity = dto.Quantity;
        }
        await _context.SaveChangesAsync();
        return Ok(new { message = "Cart updated" });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> RemoveFromCart(int id)
    {
        var userId = GetUserId();
        var item = await _context.CartItems
            .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);
        if (item == null) return NotFound();

        _context.CartItems.Remove(item);
        await _context.SaveChangesAsync();
        return Ok(new { message = "Removed from cart" });
    }

    [HttpGet("count")]
    public async Task<IActionResult> GetCartCount()
    {
        var userId = GetUserId();
        var count = await _context.CartItems
            .Where(c => c.UserId == userId)
            .SumAsync(c => c.Quantity);
        return Ok(new { count });
    }
}

public class AddToCartDto
{
    public int ProductId { get; set; }
    public int Quantity { get; set; } = 1;
}

public class UpdateQuantityDto
{
    public int Quantity { get; set; }
}
