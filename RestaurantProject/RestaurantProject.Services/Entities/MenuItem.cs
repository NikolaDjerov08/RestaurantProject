using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Restaurant.Core.Constants;

namespace Restaurant.Core.Entities;

public class MenuItem
{
    public int Id { get; set; }

    [Required]
    [MaxLength(ValidationConstants.MenuItemNameMaxLength)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(ValidationConstants.MenuItemDescriptionMaxLength)]
    public string Description { get; set; } = string.Empty;

    [Column(TypeName = "decimal(18,2)")]
    public decimal Price { get; set; }

    [MaxLength(ValidationConstants.MenuItemImageUrlMaxLength)]
    public string? ImageUrl { get; set; }

    public bool IsAvailable { get; set; } = true;

    public bool IsDeleted { get; set; } = false;

    // Relationship
    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;

    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}
