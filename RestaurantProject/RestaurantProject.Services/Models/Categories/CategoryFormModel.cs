using System.ComponentModel.DataAnnotations;
using Restaurant.Core.Constants;

namespace Restaurant.Core.Models.Categories;

public class CategoryFormModel
{
    [Required]
    [StringLength(
        ValidationConstants.CategoryNameMaxLength,
        MinimumLength = ValidationConstants.CategoryNameMinLength,
        ErrorMessage = "{0} must be between {2} and {1} characters.")]
    public string Name { get; set; } = string.Empty;
}
