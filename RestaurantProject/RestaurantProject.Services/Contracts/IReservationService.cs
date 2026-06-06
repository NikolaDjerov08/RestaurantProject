using Restaurant.Core.Models.Reservations;

namespace Restaurant.Core.Contracts;

public interface IReservationService
{
    Task<int> CreateAsync(string userId, ReservationFormModel model);

    Task<ReservationServiceModel?> GetByIdAsync(int id);

    Task<IReadOnlyList<ReservationServiceModel>> GetForUserAsync(string userId);

    Task<IReadOnlyList<ReservationServiceModel>> GetAllAsync();

    Task<bool> UpdateAsync(int id, ReservationFormModel model);

    Task<bool> DeleteAsync(int id);

    Task<bool> ConfirmAsync(int id);
}
