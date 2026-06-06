using System.ComponentModel.DataAnnotations;
using Restaurant.Core.Constants;

namespace Restaurant.Core.Models.Users;

public class UserProfileFormModel
{
    [Required]
    [StringLength(
        ValidationConstants.FullNameMaxLength,
        MinimumLength = ValidationConstants.FullNameMinLength,
        ErrorMessage = "{0} must be between {2} and {1} characters.")]
    [Display(Name = "Full name")]
    public string FullName { get; set; } = string.Empty;

    [StringLength(ValidationConstants.AddressMaxLength)]
    public string? Address { get; set; }

    [Url]
    [StringLength(ValidationConstants.ProfileImageUrlMaxLength)]
    [Display(Name = "Profile image URL")]
    public string? ProfileImageUrl { get; set; }
}
