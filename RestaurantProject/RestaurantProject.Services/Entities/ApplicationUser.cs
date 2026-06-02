using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Restaurant.Core.Constants;

namespace Restaurant.Core.Entities;

public class ApplicationUser : IdentityUser
{
    [Required]
    [MaxLength(ValidationConstants.FullNameMaxLength)]
    public string FullName { get; set; } = string.Empty;

    [MaxLength(ValidationConstants.AddressMaxLength)]
    public string? Address { get; set; }

    [MaxLength(ValidationConstants.ProfileImageUrlMaxLength)]
    public string? ProfileImageUrl { get; set; }

    public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

    public bool IsDeleted { get; set; } = false;

    // Navigation
    public ICollection<Order> Orders { get; set; } = new List<Order>();
    public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}
