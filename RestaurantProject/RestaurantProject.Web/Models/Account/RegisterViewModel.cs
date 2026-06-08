using System.ComponentModel.DataAnnotations;
using Restaurant.Core.Constants;

namespace Restaurant.Web.Models.Account;

public class RegisterViewModel
{
    [Required]
    [StringLength(
        ValidationConstants.FullNameMaxLength,
        MinimumLength = ValidationConstants.FullNameMinLength,
        ErrorMessage = "{0} must be between {2} and {1} characters.")]
    [Display(Name = "Full name")]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "{0} must be at least {2} characters.")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [DataType(DataType.Password)]
    [Display(Name = "Confirm password")]
    [Compare(nameof(Password), ErrorMessage = "The passwords do not match.")]
    public string ConfirmPassword { get; set; } = string.Empty;

    [StringLength(ValidationConstants.AddressMaxLength)]
    public string? Address { get; set; }
}
