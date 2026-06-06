namespace Restaurant.Core.Constants;

public static class ValidationConstants
{
    // MenuItem
    public const int MenuItemNameMinLength = 2;
    public const int MenuItemNameMaxLength = 80;
    public const int MenuItemDescriptionMinLength = 5;
    public const int MenuItemDescriptionMaxLength = 500;
    public const double MenuItemPriceMin = 0.01;
    public const double MenuItemPriceMax = 9999.99;
    public const int MenuItemImageUrlMaxLength = 2048;

    // Category
    public const int CategoryNameMinLength = 2;
    public const int CategoryNameMaxLength = 50;

    // ApplicationUser
    public const int FullNameMinLength = 2;
    public const int FullNameMaxLength = 100;
    public const int AddressMaxLength = 250;
    public const int ProfileImageUrlMaxLength = 2048;

    // Reservation
    public const int ReservationGuestsMin = 1;
    public const int ReservationGuestsMax = 30;
    public const int ReservationSpecialRequestMaxLength = 500;

    // Order
    public const int OrderItemQuantityMin = 1;
    public const int OrderItemQuantityMax = 100;
}
