using System.ComponentModel.DataAnnotations;
using Restaurant.Core.Constants;

namespace Restaurant.Core.Entities;

public class Category
{
    public int Id { get; set; }

    [Required]
    [MaxLength(ValidationConstants.CategoryNameMaxLength)]
    public string Name { get; set; } = string.Empty;

    public bool IsDeleted { get; set; } = false;

    public ICollection<MenuItem> MenuItems { get; set; } = new List<MenuItem>();
}
