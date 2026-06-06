using System.ComponentModel.DataAnnotations;
using Restaurant.Core.Constants;

namespace Restaurant.Core.Models.Reservations;

public class ReservationFormModel
{
    [Required]
    [Display(Name = "Date & Time")]
    [DataType(DataType.DateTime)]
    public DateTime ReservationDate { get; set; } = DateTime.Now.AddHours(2);

    [Required]
    [Range(
        ValidationConstants.ReservationGuestsMin,
        ValidationConstants.ReservationGuestsMax,
        ErrorMessage = "Number of guests must be between {1} and {2}.")]
    [Display(Name = "Number of guests")]
    public int GuestsCount { get; set; } = 2;

    [StringLength(ValidationConstants.ReservationSpecialRequestMaxLength)]
    [Display(Name = "Special request")]
    public string? SpecialRequest { get; set; }
}
