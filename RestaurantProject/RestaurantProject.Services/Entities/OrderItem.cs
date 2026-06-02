using System.ComponentModel.DataAnnotations.Schema;

namespace Restaurant.Core.Entities;

public class OrderItem
{
    public int Id { get; set; }

    public int OrderId { get; set; }
    public Order Order { get; set; } = null!;

    public int MenuItemId { get; set; }
    public MenuItem MenuItem { get; set; } = null!;

    public int Quantity { get; set; }

    // Price snapshot at order time (so historical orders aren't affected by price changes)
    [Column(TypeName = "decimal(18,2)")]
    public decimal Price { get; set; }
}
