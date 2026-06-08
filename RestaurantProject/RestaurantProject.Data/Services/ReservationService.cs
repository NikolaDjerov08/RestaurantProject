using Microsoft.EntityFrameworkCore;
using Restaurant.Core.Contracts;
using Restaurant.Core.Entities;
using Restaurant.Core.Models.Reservations;
using Restaurant.Infrastructure.Data;

namespace Restaurant.Infrastructure.Services;

public class ReservationService : IReservationService
{
    private readonly ApplicationDbContext _context;

    public ReservationService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<int> CreateAsync(string userId, ReservationFormModel model)
    {
        var entity = new Reservation
        {
            UserId = userId,
            ReservationDate = model.ReservationDate,
            GuestsCount = model.GuestsCount,
            SpecialRequest = model.SpecialRequest,
            IsConfirmed = false
        };

        _context.Reservations.Add(entity);
        await _context.SaveChangesAsync();
        return entity.Id;
    }

    public async Task<ReservationServiceModel?> GetByIdAsync(int id)
    {
        return await Project().FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<IReadOnlyList<ReservationServiceModel>> GetForUserAsync(string userId)
    {
        return await Project()
            .Where(r => r.UserId == userId)
            .OrderByDescending(r => r.ReservationDate)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<ReservationServiceModel>> GetAllAsync()
    {
        return await Project()
            .OrderByDescending(r => r.ReservationDate)
            .ToListAsync();
    }

    public async Task<bool> UpdateAsync(int id, ReservationFormModel model)
    {
        var entity = await _context.Reservations.FirstOrDefaultAsync(r => r.Id == id);
        if (entity is null)
        {
            return false;
        }

        entity.ReservationDate = model.ReservationDate;
        entity.GuestsCount = model.GuestsCount;
        entity.SpecialRequest = model.SpecialRequest;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await _context.Reservations.FirstOrDefaultAsync(r => r.Id == id);
        if (entity is null)
        {
            return false;
        }

        entity.IsDeleted = true;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ConfirmAsync(int id)
    {
        var entity = await _context.Reservations.FirstOrDefaultAsync(r => r.Id == id);
        if (entity is null)
        {
            return false;
        }

        entity.IsConfirmed = true;
        await _context.SaveChangesAsync();
        return true;
    }

    private IQueryable<ReservationServiceModel> Project() =>
        _context.Reservations
            .AsNoTracking()
            .Select(r => new ReservationServiceModel
            {
                Id = r.Id,
                UserId = r.UserId,
                UserFullName = r.User.FullName,
                UserEmail = r.User.Email!,
                ReservationDate = r.ReservationDate,
                GuestsCount = r.GuestsCount,
                SpecialRequest = r.SpecialRequest,
                IsConfirmed = r.IsConfirmed
            });
}
