using System.ComponentModel.DataAnnotations;
using NUnit.Framework;
using Restaurant.Core.Models;
using Restaurant.Core.Models.MenuItems;
using Restaurant.Core.Models.Reservations;

namespace Restaurant.Tests.Services;

[TestFixture]
public class ValidationTests
{
    private static IList<ValidationResult> Validate(object model)
    {
        var results = new List<ValidationResult>();
        var context = new ValidationContext(model);
        Validator.TryValidateObject(model, context, results, validateAllProperties: true);
        return results;
    }

    [Test]
    public void MenuItemForm_Valid_PassesValidation()
    {
        var model = new MenuItemFormModel
        {
            Name = "Tiramisu",
            Description = "Coffee dessert",
            Price = 8.00m,
            CategoryId = 1
        };

        Assert.That(Validate(model), Is.Empty);
    }

    [Test]
    public void MenuItemForm_EmptyName_Fails()
    {
        var model = new MenuItemFormModel
        {
            Name = "",
            Description = "Coffee dessert",
            Price = 8.00m,
            CategoryId = 1
        };

        Assert.That(Validate(model), Is.Not.Empty);
    }

    [Test]
    public void MenuItemForm_PriceTooLow_Fails()
    {
        var model = new MenuItemFormModel
        {
            Name = "Free Dish",
            Description = "Should not be allowed",
            Price = 0m,
            CategoryId = 1
        };

        var results = Validate(model);
        Assert.That(results.Any(r => r.MemberNames.Contains(nameof(MenuItemFormModel.Price))), Is.True);
    }

    [Test]
    public void ReservationForm_GuestsOutOfRange_Fails()
    {
        var model = new ReservationFormModel
        {
            ReservationDate = DateTime.Now.AddDays(1),
            GuestsCount = 999
        };

        var results = Validate(model);
        Assert.That(results.Any(r => r.MemberNames.Contains(nameof(ReservationFormModel.GuestsCount))), Is.True);
    }

    [Test]
    public void PagedResult_ComputesTotalPages()
    {
        var page = new PagedResult<int>
        {
            Items = new[] { 1, 2, 3 },
            TotalCount = 10,
            CurrentPage = 1,
            PageSize = 3
        };

        Assert.That(page.TotalPages, Is.EqualTo(4)); // ceil(10/3)
        Assert.That(page.HasNext, Is.True);
        Assert.That(page.HasPrevious, Is.False);
    }

    [Test]
    public void PagedResult_LastPage_HasNoNext()
    {
        var page = new PagedResult<int>
        {
            Items = new[] { 10 },
            TotalCount = 10,
            CurrentPage = 4,
            PageSize = 3
        };

        Assert.That(page.HasNext, Is.False);
        Assert.That(page.HasPrevious, Is.True);
    }
}
