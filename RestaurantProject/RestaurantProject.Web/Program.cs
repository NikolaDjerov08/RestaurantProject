using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Restaurant.Core.Constants;
using Restaurant.Core.Contracts;
using Restaurant.Core.Entities;
using Restaurant.Infrastructure.Data;
using Restaurant.Infrastructure.Data.SeedData;
using Restaurant.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

// --- Database
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// --- Identity with roles
builder.Services
    .AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        options.SignIn.RequireConfirmedAccount = false;
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireNonAlphanumeric = true;
        options.Password.RequiredLength = 6;
        options.User.RequireUniqueEmail = true;
        options.Lockout.MaxFailedAccessAttempts = 5;
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// --- Cookie paths point at our MVC AccountController (not scaffolded Razor Pages)
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromDays(7);
    options.SlidingExpiration = true;
});

// --- Authorization policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(PolicyConstants.AdminOnly, policy =>
        policy.RequireRole(RoleConstants.Admin));

    options.AddPolicy(PolicyConstants.StaffOnly, policy =>
        policy.RequireRole(RoleConstants.Admin, RoleConstants.Employee));
});

// --- Application services
builder.Services.AddInfrastructureServices();

// --- HttpContext access + Session (cart needs both)
builder.Services.AddHttpContextAccessor();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(1);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// --- MVC + Razor Pages
builder.Services.AddControllersWithViews()
    .AddJsonOptions(options =>
    {
        // Serialize/deserialize enums (e.g. OrderStatus) as their string names
        options.JsonSerializerOptions.Converters.Add(
            new System.Text.Json.Serialization.JsonStringEnumConverter());
    });
builder.Services.AddRazorPages();

// --- Antiforgery — custom header for AJAX (Stage 5+)
builder.Services.AddAntiforgery(options => options.HeaderName = "X-CSRF-TOKEN");

var app = builder.Build();

// --- Apply migrations + seed roles/admin at startup
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var db = services.GetRequiredService<ApplicationDbContext>();
    await db.Database.MigrateAsync();
    await IdentitySeeder.SeedAsync(services);
}

if (app.Environment.IsDevelopment())
{
    // Full diagnostics for unhandled exceptions (500) while developing
    app.UseDeveloperExceptionPage();
    app.UseMigrationsEndPoint();
}
else
{
    // Production: route unhandled exceptions to the custom error page
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// Custom error pages for status codes (400, 401, 403, 404, ...) in ALL environments
app.UseStatusCodePagesWithReExecute("/Home/StatusCode", "?code={0}");

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseSession();          // before auth & endpoints
app.UseAuthentication();
app.UseAuthorization();

// --- Expose cart count to the layout on every request
app.Use(async (context, next) =>
{
    var path = context.Request.Path;
    var skip = path.StartsWithSegments("/api")
            || path.StartsWithSegments("/lib")
            || path.StartsWithSegments("/css")
            || path.StartsWithSegments("/js")
            || path.StartsWithSegments("/images");

    if (!skip)
    {
        var cart = context.RequestServices.GetRequiredService<ICartService>();
        context.Items["CartCount"] = await cart.GetCountAsync();
    }
    await next();
});

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Attribute-routed API controllers (/api/...)
app.MapControllers();

app.MapRazorPages();

app.Run();

// Make Program visible to the test project
public partial class Program { }
