using System.ComponentModel.DataAnnotations;
using Restaurant.Core.Models.Cart;

namespace Restaurant.Web.Models.Cart;

public class CheckoutViewModel
{
    public CartSummaryModel Cart { get; set; } = new();

    [StringLength(250)]
    [Display(Name = "Delivery address")]
    public string? DeliveryAddress { get; set; }

    [StringLength(300)]
    [Display(Name = "Order notes (optional)")]
    public string? Notes { get; set; }
}
