using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LuminaTech.Data.Entities;

public class Order
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public int UserId { get; set; }
    public User User { get; set; } = null!;

    [MaxLength(50)]
    public string OrderNumber { get; set; } = string.Empty;

    [Column(TypeName = "decimal(10,2)")]
    public decimal TotalAmount { get; set; }

    [MaxLength(20)]
    public string Status { get; set; } = "Pending"; // Pending, Processing, Completed, Cancelled

    [MaxLength(200)]
    public string ShippingAddress { get; set; } = string.Empty;

    [MaxLength(50)]
    public string PaymentMethod { get; set; } = "Credit Card";

    [MaxLength(100)]
    public string PaymentId { get; set; } = string.Empty; // Dummy payment reference

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
}

public class OrderItem
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public int OrderId { get; set; }
    public Order Order { get; set; } = null!;

    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;

    public int Quantity { get; set; } = 1;

    [Column(TypeName = "decimal(10,2)")]
    public decimal UnitPrice { get; set; }
}
