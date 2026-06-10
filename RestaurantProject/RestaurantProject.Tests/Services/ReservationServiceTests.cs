using NUnit.Framework;
using Restaurant.Core.Models.Reservations;
using Restaurant.Infrastructure.Data;
using Restaurant.Infrastructure.Services;
using Restaurant.Tests.Helpers;

namespace Restaurant.Tests.Services;

[TestFixture]
public class ReservationServiceTests
{
    private ApplicationDbContext _context = null!;
    private ReservationService _service = null!;

    [SetUp]
    public async Task SetUp()
    {
        _context = await TestDbFactory.CreateSeededContextAsync();
        _service = new ReservationService(_context);
    }

    [TearDown]
    public void TearDown() => _context.Dispose();

    private static ReservationFormModel Form(int guests = 2) => new()
    {
        ReservationDate = DateTime.Now.AddDays(1),
        GuestsCount = guests,
        SpecialRequest = "Window seat"
    };

    [Test]
    public async Task CreateAsync_CreatesUnconfirmedReservation()
    {
        var id = await _service.CreateAsync("user-1", Form());

        var reservation = await _service.GetByIdAsync(id);
        Assert.That(reservation, Is.Not.Null);
        Assert.That(reservation!.IsConfirmed, Is.False);
        Assert.That(reservation.GuestsCount, Is.EqualTo(2));
    }

    [Test]
    public async Task ConfirmAsync_SetsConfirmed()
    {
        var id = await _service.CreateAsync("user-1", Form());

        var ok = await _service.ConfirmAsync(id);

        Assert.That(ok, Is.True);
        var reservation = await _service.GetByIdAsync(id);
        Assert.That(reservation!.IsConfirmed, Is.True);
    }

    [Test]
    public async Task DeleteAsync_SoftDeletes()
    {
        var id = await _service.CreateAsync("user-1", Form());

        var ok = await _service.DeleteAsync(id);

        Assert.That(ok, Is.True);
        var reservation = await _service.GetByIdAsync(id);
        Assert.That(reservation, Is.Null);
    }

    [Test]
    public async Task GetForUserAsync_ReturnsOnlyOwnReservations()
    {
        await _service.CreateAsync("user-1", Form());
        await _service.CreateAsync("user-1", Form(4));

        var list = await _service.GetForUserAsync("user-1");

        Assert.That(list, Has.Count.EqualTo(2));
        Assert.That(list.All(r => r.UserId == "user-1"), Is.True);
    }

    [Test]
    public async Task UpdateAsync_ChangesDetails()
    {
        var id = await _service.CreateAsync("user-1", Form());

        var newForm = Form(8);
        newForm.SpecialRequest = "Birthday";
        var ok = await _service.UpdateAsync(id, newForm);

        Assert.That(ok, Is.True);
        var reservation = await _service.GetByIdAsync(id);
        Assert.That(reservation!.GuestsCount, Is.EqualTo(8));
        Assert.That(reservation.SpecialRequest, Is.EqualTo("Birthday"));
    }

    [Test]
    public async Task ConfirmAsync_UnknownId_ReturnsFalse()
    {
        var ok = await _service.ConfirmAsync(999);
        Assert.That(ok, Is.False);
    }
}
