using System.ComponentModel.DataAnnotations;
using Restaurant.Core.Constants;

namespace Restaurant.Core.Models.MenuItems;

public class MenuItemFormModel
{
    [Required]
    [StringLength(
        ValidationConstants.MenuItemNameMaxLength,
        MinimumLength = ValidationConstants.MenuItemNameMinLength,
        ErrorMessage = "{0} must be between {2} and {1} characters.")]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(
        ValidationConstants.MenuItemDescriptionMaxLength,
        MinimumLength = ValidationConstants.MenuItemDescriptionMinLength,
        ErrorMessage = "{0} must be between {2} and {1} characters.")]
    public string Description { get; set; } = string.Empty;

    [Required]
    [Range(
        ValidationConstants.MenuItemPriceMin,
        ValidationConstants.MenuItemPriceMax,
        ErrorMessage = "{0} must be between {1:F2} and {2:F2}.")]
    public decimal Price { get; set; }

    [Url]
    [StringLength(ValidationConstants.MenuItemImageUrlMaxLength)]
    public string? ImageUrl { get; set; }

    [Required]
    [Display(Name = "Category")]
    public int CategoryId { get; set; }

    [Display(Name = "Available")]
    public bool IsAvailable { get; set; } = true;
}
