# Jerry's — Restaurant Management System

A full-stack restaurant management web application built with **ASP.NET Core MVC (.NET 8)**, **Entity Framework Core 8**, **Microsoft SQL Server** and **ASP.NET Core Identity**. Customers browse the menu, order and reserve tables; staff and admins manage everything through a dedicated, role-protected admin panel.

Built to satisfy the **ASP.NET MVC final project** requirements.

---

## Tech stack

- **ASP.NET Core 8 MVC** + Razor Views
- **Entity Framework Core 8** (code-first, migrations, Fluent API)
- **Microsoft SQL Server**
- **ASP.NET Core Identity** (roles: Admin, Employee, Customer) with a custom user
- **Web API** controllers + **AJAX** (`fetch`)
- **Bootstrap 5** + a custom responsive theme
- **NUnit + Moq + EF Core InMemory** for unit testing
- **Dependency injection** throughout; clean, service-based architecture

---

## Architecture

The solution is split into **three projects** plus a test project, each with a single responsibility:

```
RestaurantProject.sln
├── RestaurantProject.Services   // Core: Entities, Enums, Contracts (interfaces), DTOs, Models, Constants
├── RestaurantProject.Data       // Infrastructure: DbContext, Fluent API configs, Services (business logic), Seeders, Migrations
├── RestaurantProject.Web        // MVC app: Controllers, API controllers, Admin area, Razor Views, wwwroot
└── RestaurantProject.Tests      // NUnit + Moq unit tests
```

**Dependency direction:** `Web → Services, Data` · `Data → Services` · `Services → nothing`.

Controllers are **thin** — all business logic lives in **services behind interfaces** (`IMenuService`, `IOrderService`, `ICartService`, `IReservationService`, `IUserService`, `IAdminService`, `ICategoryService`), registered with the built-in DI container in `ServiceCollectionExtensions.AddInfrastructureServices()`.

---

## Getting started

### Prerequisites
- .NET 8 SDK
- Microsoft SQL Server (LocalDB, a local instance, or a Docker container)

### Configure the database
Edit the connection string in `RestaurantProject.Web/appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost,1433;Database=Restaurant;User Id=sa;Password=YOUR_PASSWORD;TrustServerCertificate=true;"
}
```
(For LocalDB use: `Server=(localdb)\\MSSQLLocalDB;Database=Restaurant;Trusted_Connection=True;TrustServerCertificate=True`.)

### Run
```bash
dotnet restore
dotnet run --project RestaurantProject.Web
```
Migrations are applied and roles/admin are seeded **automatically on startup**. In Visual Studio 2022: open `RestaurantProject.sln`, set `RestaurantProject.Web` as the startup project and press **F5**.

### Migrations (already created)
```bash
dotnet ef migrations add InitialCreate --project RestaurantProject.Data --startup-project RestaurantProject.Web
dotnet ef database update          --project RestaurantProject.Data --startup-project RestaurantProject.Web
```

---

## Seeded data

| | |
|---|---|
| **Admin login** | `admin@restaurant.com` / `Admin@123` |
| Roles | Admin, Employee, Customer (seeded at runtime, idempotent) |
| Categories & menu items | seeded via migration |

---

## Features

### Customer
- Landing page with hero + featured sections
- Menu with live **search**, **category filter**, **sort** (name/price) and **pagination** (AJAX-enhanced)
- Menu item detail pages
- Session-based **cart** with live add / update / remove and a navbar badge
- **Checkout** → order placement (price recomputed server-side) → confirmation
- Order history & order details
- Table **reservations** with future-date validation, plus "my reservations"
- Profile view & edit

### Admin panel (`/Admin`, role-gated)
- **Dashboard**: KPI cards + revenue and top-sellers charts (Chart.js)
- **Menu items**: full CRUD + availability toggle
- **Categories**: CRUD with name-uniqueness and delete-protection
- **Orders**: list, details, status workflow (Pending → Confirmed → Preparing → Ready → Delivered/Cancelled)
- **Reservations**: confirm / remove
- **Users** (Admin only): list, role management, soft-delete

### Web API (AJAX)
| Method | Route | Auth |
|---|---|---|
| GET | `/api/menu` (search/filter/sort/page) | public |
| GET | `/api/menu/{id}` · `/api/menu/categories` | public |
| GET/POST/PUT/DELETE | `/api/cart` | CSRF token |
| GET/PUT | `/api/orders` · `/api/orders/{id}` · status | auth / staff |

AJAX mutations send the antiforgery token via the `X-CSRF-TOKEN` header.

---

## Security

- ASP.NET Identity authentication (hashed passwords, account lockout after 5 failed attempts)
- Role-based authorization with policies (`AdminOnly`, `StaffOnly`)
- **CSRF**: antiforgery tokens on all forms and AJAX mutations
- **SQL injection**: parameterised queries everywhere via EF Core LINQ
- **XSS**: Razor output-encoding; JS uses `textContent`
- **Parameter tampering**: ownership checks on orders/reservations; server-side price recomputation at checkout
- Custom error pages for 400/401/404/500 via `UseStatusCodePagesWithReExecute`

---

## Data model

- **ApplicationUser** (extends IdentityUser): FullName, Address, ProfileImageUrl, CreatedOn, soft-delete
- **Category** 1—* **MenuItem** (`OnDelete: Restrict`, soft-delete filters)
- **Order** 1—* **OrderItem** *—1 **MenuItem** (OrderItem stores a **price snapshot**)
- **Reservation** *—1 **ApplicationUser**

Fluent API configures indexes, delete behaviours, decimal precision and global soft-delete query filters.

---

## Testing

NUnit + Moq + EF Core InMemory, **58 unit tests** across the service layer:

```bash
dotnet test
# with coverage scoped to the business logic:
dotnet test --settings coverage.runsettings
```

Coverage of the **service layer (business logic) is ~80–90%** — above the 70% target. Covered: order total calculation & price-snapshotting, unavailable-item rejection, cart hydration/totals (Moq'd `IHttpContextAccessor` + fake `ISession`), menu search/filter/sort/paging, category delete-protection & name-uniqueness, reservation lifecycle & ownership checks, admin dashboard statistics, and model validation.

---

## License

Educational project.
