using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LuminaTech.Data;
using LuminaTech.Data.Entities;
using Microsoft.AspNetCore.Authorization;

namespace LuminaTech.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductController : ControllerBase
{
    private readonly LuminaTechDbContext _context;

    public ProductController(LuminaTechDbContext context)
    {
        _context = context;
    }

    private bool IsDemoUser() =>
        HttpContext.User.FindFirst("isDemo")?.Value == "true";

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? category = null)
    {
        var query = _context.Products.Where(p => p.IsActive);

        if (!string.IsNullOrEmpty(category))
            query = query.Where(p => p.Category == category);

        var products = await query.OrderByDescending(p => p.CreatedAt).ToListAsync();
        return Ok(products);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id && p.IsActive);
        if (product == null) return NotFound(new { message = "Product not found" });
        return Ok(product);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] Product product)
    {
        if (IsDemoUser())
            return StatusCode(403, new { message = "Demo accounts cannot create products" });

        product.CreatedAt = DateTime.UtcNow;
        product.IsActive = true;
        _context.Products.Add(product);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, [FromBody] Product updated)
    {
        if (IsDemoUser())
            return StatusCode(403, new { message = "Demo accounts cannot update products" });

        var product = await _context.Products.FindAsync(id);
        if (product == null) return NotFound();

        product.Name = updated.Name;
        product.Description = updated.Description;
        product.Price = updated.Price;
        product.Category = updated.Category;
        product.ImageUrl = updated.ImageUrl;
        product.CloudinaryPublicId = updated.CloudinaryPublicId;

        await _context.SaveChangesAsync();
        return Ok(product);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        if (IsDemoUser())
            return StatusCode(403, new { message = "Demo accounts cannot delete products" });

        var product = await _context.Products.FindAsync(id);
        if (product == null) return NotFound();

        product.IsActive = false;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpGet("categories")]
    public async Task<IActionResult> GetCategories()
    {
        var categories = await _context.Products
            .Where(p => p.IsActive)
            .Select(p => p.Category)
            .Distinct()
            .ToListAsync();
        return Ok(categories);
    }
}
