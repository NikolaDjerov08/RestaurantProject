namespace Restaurant.Core.Models.Reservations;

public class ReservationServiceModel
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string UserFullName { get; set; } = string.Empty;
    public string UserEmail { get; set; } = string.Empty;
    public DateTime ReservationDate { get; set; }
    public int GuestsCount { get; set; }
    public string? SpecialRequest { get; set; }
    public bool IsConfirmed { get; set; }
}
