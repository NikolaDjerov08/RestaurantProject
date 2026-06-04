using Microsoft.Extensions.DependencyInjection;
using Restaurant.Core.Contracts;
using Restaurant.Infrastructure.Services;

namespace Restaurant.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        services.AddScoped<IMenuService, MenuService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<IReservationService, ReservationService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IAdminService, AdminService>();
        services.AddScoped<ICartService, CartService>();

        return services;
    }
}
