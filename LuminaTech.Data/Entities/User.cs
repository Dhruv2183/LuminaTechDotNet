using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LuminaTech.Data.Entities;

public class User
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [MaxLength(100)]
    public string GoogleId { get; set; } = string.Empty;

    [Required]
    [MaxLength(255)]
    public string Email { get; set; } = string.Empty;

    [MaxLength(200)]
    public string FullName { get; set; } = string.Empty;

    [MaxLength(500)]
    public string ProfilePictureUrl { get; set; } = string.Empty;

    [MaxLength(20)]
    public string Role { get; set; } = "User";

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public bool IsActive { get; set; } = true;

    // Navigation
    public ICollection<Wishlist> Wishlists { get; set; } = new List<Wishlist>();
}
