using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LuminaTech.Data;
using LuminaTech.Data.Entities;

namespace LuminaTech.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrderController : ControllerBase
{
    private readonly LuminaTechDbContext _context;

    public OrderController(LuminaTechDbContext context)
    {
        _context = context;
    }

    private int GetUserId() =>
        int.Parse(HttpContext.User.FindFirst("userId")?.Value ?? "0");

    [HttpPost("checkout")]
    public async Task<IActionResult> Checkout([FromBody] CheckoutDto dto)
    {
        var userId = GetUserId();
        var cartItems = await _context.CartItems
            .Where(c => c.UserId == userId)
            .Include(c => c.Product)
            .ToListAsync();

        if (!cartItems.Any())
            return BadRequest(new { error = "Cart is empty" });

        // Generate order number
        var orderNumber = $"LT-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..8].ToUpper()}";

        // Simulate payment processing
        var paymentId = $"PAY-{Guid.NewGuid().ToString()[..12].ToUpper()}";

        var order = new Order
        {
            UserId = userId,
            OrderNumber = orderNumber,
            TotalAmount = cartItems.Sum(c => c.Product.Price * c.Quantity),
            Status = "Completed",
            ShippingAddress = dto.ShippingAddress ?? "Default Address",
            PaymentMethod = dto.PaymentMethod ?? "Credit Card",
            PaymentId = paymentId,
            Items = cartItems.Select(c => new OrderItem
            {
                ProductId = c.ProductId,
                Quantity = c.Quantity,
                UnitPrice = c.Product.Price
            }).ToList()
        };

        _context.Orders.Add(order);
        _context.CartItems.RemoveRange(cartItems); // Clear cart after checkout
        await _context.SaveChangesAsync();

        return Ok(new
        {
            message = "Order placed successfully!",
            orderNumber,
            paymentId,
            total = order.TotalAmount,
            status = order.Status
        });
    }

    [HttpGet]
    public async Task<IActionResult> GetOrders()
    {
        var userId = GetUserId();
        var orders = await _context.Orders
            .Where(o => o.UserId == userId)
            .Include(o => o.Items)
            .ThenInclude(i => i.Product)
            .OrderByDescending(o => o.CreatedAt)
            .Select(o => new
            {
                o.Id,
                o.OrderNumber,
                o.TotalAmount,
                o.Status,
                o.PaymentMethod,
                o.PaymentId,
                o.CreatedAt,
                Items = o.Items.Select(i => new
                {
                    i.ProductId,
                    ProductName = i.Product.Name,
                    ProductImage = i.Product.ImageUrl,
                    i.Quantity,
                    i.UnitPrice
                })
            })
            .ToListAsync();

        return Ok(orders);
    }
}

public class CheckoutDto
{
    public string? ShippingAddress { get; set; }
    public string? PaymentMethod { get; set; }
}
