using System.ComponentModel.DataAnnotations;
using Restaurant.Core.Constants;

namespace Restaurant.Core.Entities;

public class Reservation
{
    public int Id { get; set; }

    [Required]
    public string UserId { get; set; } = string.Empty;
    public ApplicationUser User { get; set; } = null!;

    public DateTime ReservationDate { get; set; }

    [Range(ValidationConstants.ReservationGuestsMin, ValidationConstants.ReservationGuestsMax)]
    public int GuestsCount { get; set; }

    [MaxLength(ValidationConstants.ReservationSpecialRequestMaxLength)]
    public string? SpecialRequest { get; set; }

    public bool IsConfirmed { get; set; } = false;

    public bool IsDeleted { get; set; } = false;
}
