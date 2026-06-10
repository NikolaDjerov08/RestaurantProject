const fs = require("fs");
const {
  Document, Packer, Paragraph, TextRun, Table, TableRow, TableCell,
  AlignmentType, LevelFormat, TableOfContents, HeadingLevel, BorderStyle,
  WidthType, ShadingType, PageBreak, PageNumber, Header, Footer
} = require("docx");

const CONTENT_W = 9360;            // US Letter, 1" margins
const RED = "C0392B";
const YELLOW_BG = "FCEFC7";
const HEAD_BG = "F4D7D2";
const GREY = "CCCCCC";

const border = { style: BorderStyle.SINGLE, size: 1, color: GREY };
const borders = { top: border, bottom: border, left: border, right: border };

// ---------- helpers ----------
const kids = [];
const H1 = (t) => kids.push(new Paragraph({ heading: HeadingLevel.HEADING_1, children: [new TextRun(t)] }));
const H2 = (t) => kids.push(new Paragraph({ heading: HeadingLevel.HEADING_2, children: [new TextRun(t)] }));
const H3 = (t) => kids.push(new Paragraph({ heading: HeadingLevel.HEADING_3, children: [new TextRun(t)] }));
const P = (t, opts = {}) => kids.push(new Paragraph({ spacing: { after: 120 }, children: [new TextRun({ text: t, ...opts })] }));
const RUNS = (runs) => kids.push(new Paragraph({ spacing: { after: 120 }, children: runs }));
const BULLET = (t) => kids.push(new Paragraph({ numbering: { reference: "bul", level: 0 }, spacing: { after: 40 }, children: typeof t === "string" ? [new TextRun(t)] : t }));
const BULLET2 = (t) => kids.push(new Paragraph({ numbering: { reference: "bul", level: 1 }, spacing: { after: 40 }, children: [new TextRun(t)] }));
const SPACER = () => kids.push(new Paragraph({ children: [new TextRun("")] }));
const PB = () => kids.push(new Paragraph({ children: [new PageBreak()] }));

function cell(text, w, { bold = false, fill = null, color = null, align = null } = {}) {
  const runs = Array.isArray(text)
    ? text
    : [new TextRun({ text: String(text), bold, color: color || undefined })];
  return new TableCell({
    borders,
    width: { size: w, type: WidthType.DXA },
    shading: fill ? { fill, type: ShadingType.CLEAR } : undefined,
    margins: { top: 60, bottom: 60, left: 110, right: 110 },
    children: [new Paragraph({ alignment: align || undefined, children: runs })],
  });
}

function table(colWidths, rows, { headerFill = HEAD_BG } = {}) {
  const trs = rows.map((r, ri) =>
    new TableRow({
      tableHeader: ri === 0,
      children: r.map((c, ci) =>
        cell(c, colWidths[ci], ri === 0 ? { bold: true, fill: headerFill } : {})
      ),
    })
  );
  kids.push(new Table({ width: { size: CONTENT_W, type: WidthType.DXA }, columnWidths: colWidths, rows: trs }));
  SPACER();
}

// Criterion callout line (italic, red) used to tie each section to the grading sheet
const CRIT = (t) => kids.push(new Paragraph({ spacing: { after: 120 }, children: [new TextRun({ text: "Изпит/Exam: " + t, italics: true, color: RED, size: 20 })] }));

// ============================================================
// TITLE PAGE
// ============================================================
kids.push(new Paragraph({ spacing: { before: 1800, after: 0 }, alignment: AlignmentType.CENTER,
  children: [new TextRun({ text: "Jerry’s", bold: true, size: 72, color: RED })] }));
kids.push(new Paragraph({ alignment: AlignmentType.CENTER, spacing: { after: 400 },
  children: [new TextRun({ text: "Restaurant Management System", size: 36, color: "555555" })] }));
kids.push(new Paragraph({ alignment: AlignmentType.CENTER, spacing: { after: 120 },
  children: [new TextRun({ text: "Project Overview & Defense Guide", bold: true, size: 30 })] }));
kids.push(new Paragraph({ alignment: AlignmentType.CENTER, spacing: { after: 600 },
  children: [new TextRun({ text: "ASP.NET Core 8 — Final Project (11th grade)", size: 24, color: "555555" })] }));
kids.push(new Paragraph({ alignment: AlignmentType.CENTER, spacing: { after: 120 },
  children: [new TextRun({ text: "Built with: ASP.NET Core MVC · Razor Views · Web API · Entity Framework Core 8 ·", size: 20, color: "777777" })] }));
kids.push(new Paragraph({ alignment: AlignmentType.CENTER, spacing: { after: 600 },
  children: [new TextRun({ text: "Microsoft SQL Server · ASP.NET Identity · Dependency Injection · Bootstrap 5", size: 20, color: "777777" })] }));
kids.push(new Paragraph({ alignment: AlignmentType.CENTER,
  children: [new TextRun({ text: "This guide explains WHAT each part of the project is, WHY it was chosen, and WHERE it lives in the code — organised to match the exam grading sheet.", italics: true, size: 22, color: "555555" })] }));
PB();

// ============================================================
// TOC
// ============================================================
H1("Table of Contents");
kids.push(new TableOfContents("Table of Contents", { hyperlink: true, headingStyleRange: "1-2" }));
PB();

// ============================================================
// 1. PROJECT OVERVIEW
// ============================================================
H1("1. Project Overview (the goal & the problem)");
CRIT("Criterion 1.2 — formulate the goal, the main problem and the purpose of the app.");
P("Jerry’s is a full restaurant management web application. It lets customers browse a menu, search and filter dishes, add them to a cart, place orders, and book tables — while staff and administrators manage the menu, categories, orders, reservations and users from a protected admin panel.");
RUNS([
  new TextRun({ text: "The problem it solves: ", bold: true }),
  new TextRun("a restaurant needs one system for two very different audiences — the public (ordering & reservations) and the staff (managing the business). The app separates these with authentication and role-based authorization, while keeping all business rules on the server so they cannot be bypassed from the browser."),
]);
RUNS([
  new TextRun({ text: "One-sentence pitch (say this in the defense): ", bold: true }),
  new TextRun("“It’s an ASP.NET Core MVC application with a Web API layer, Entity Framework Core over SQL Server, and ASP.NET Identity for authentication and roles, built on a clean service-based architecture split across three projects.”"),
]);

H2("1.1 At a glance — what the project contains");
table([4680, 1400, 3280], [
  ["Element", "Count", "Where"],
  ["Projects in the solution", "3", "Web · Data · Services"],
  ["Entity models", "6", "RestaurantProject.Services/Entities"],
  ["MVC controllers", "7", "Web/Controllers"],
  ["Web API controllers", "3", "Web/Controllers/Api"],
  ["Admin-area controllers", "6", "Web/Areas/Admin/Controllers"],
  ["Razor views", "30+", "Web/Views & Areas/Admin/Views"],
  ["Service classes (behind interfaces)", "7", "Data/Services + Services/Contracts"],
  ["EF Fluent API configurations", "5", "Data/Data/Configurations"],
  ["EF migrations", "1 (InitialCreate)", "Data/Migrations"],
]);
P("This already satisfies the “minimal structural requirements”: ASP.NET Core 8, 10+ views, 4+ entities, 4+ controllers (one of them an API controller), and the Razor view engine.", { italics: true });

// ============================================================
// 2. TECHNOLOGY STACK & WHY
// ============================================================
PB();
H1("2. Technology Stack — and WHY each was chosen");
CRIT("Criteria 2.1 & 4.1 — explain the choice of ASP.NET Core, Razor, MVC/Web API and use the correct professional terminology.");
table([2400, 3480, 3480], [
  ["Technology", "What it is", "Why we use it"],
  ["ASP.NET Core 8 (MVC)", "Microsoft’s cross-platform web framework using the Model–View–Controller pattern.", "Industry standard for C# web apps; gives routing, model binding, DI and security out of the box; required by the assignment."],
  ["Razor View Engine", "Server-side templating that mixes C# (@) with HTML to build pages.", "Generates the user interface on the server; strongly-typed views catch errors at compile time and integrate with tag helpers."],
  ["Web API controllers", "Controllers that return JSON instead of HTML (REST endpoints).", "Let the page update without reloading (AJAX) — used for the live cart and menu filtering."],
  ["Entity Framework Core 8", "An ORM (Object-Relational Mapper) — work with C# objects instead of SQL.", "Code-first model, automatic migrations, LINQ queries that are parameterised (safe from SQL injection)."],
  ["Microsoft SQL Server", "Relational database.", "Required by the assignment; reliable, integrates natively with EF Core."],
  ["ASP.NET Core Identity", "Built-in authentication & user-management system.", "Secure password hashing, login/lockout, roles and authorization — we don’t reinvent security."],
  ["Dependency Injection (DI)", "Built-in container that supplies objects (services) where they are needed.", "Loose coupling and testability — controllers depend on interfaces, not concrete classes."],
  ["Bootstrap 5", "CSS framework.", "Responsive layout quickly; we layer a custom theme on top of it."],
]);

// ============================================================
// 3. ARCHITECTURE
// ============================================================
PB();
H1("3. Architecture — separation of responsibilities");
CRIT("Criteria 3.1, 3.2, 3.3, 3.4 — multiple projects with clear responsibilities, business logic in services, proper DI, low coupling.");
P("The solution is split into three projects so that each layer has one responsibility and the dependencies point in one direction only. This is the single most important thing to explain confidently.");

table([2600, 3380, 3380], [
  ["Project", "Responsibility", "Contains"],
  ["RestaurantProject.Services\n(the Core)", "The “contracts” and shapes — knows nothing about the database or the web.", "Entities, Enums, Contracts (interfaces), DTOs, Service Models, Constants."],
  ["RestaurantProject.Data\n(the Infrastructure)", "Data access & business logic implementation.", "DbContext, Fluent API configurations, Service implementations, Seeders, Migrations."],
  ["RestaurantProject.Web\n(the presentation)", "Everything the user touches.", "MVC + API controllers, Admin area, Razor views, wwwroot (css/js/images), Program.cs."],
]);
RUNS([new TextRun({ text: "Dependency direction: ", bold: true }),
  new TextRun("Web → Services & Data;  Data → Services;  Services → nothing. The Core has no dependencies, so the business contracts never depend on infrastructure. This is the Dependency Inversion idea in practice.")]);

H2("3.1 Thin controllers, fat services");
P("Controllers do almost no work. They receive a request, call a service method, and return a view or JSON. All business logic (price calculation, validation, filtering, status changes) lives in the service classes behind interfaces.");
BULLET("Interfaces (the contract) live in Services/Contracts: IMenuService, IOrderService, ICartService, IReservationService, IUserService, IAdminService, ICategoryService.");
BULLET("Implementations live in Data/Services: MenuService, OrderService, CartService, ReservationService, UserService, AdminService, CategoryService.");
RUNS([new TextRun({ text: "Why: ", bold: true }), new TextRun("if logic is in services, it can be reused by both controllers and the API, and it can be unit-tested without a web server or a real database.")]);

H2("3.2 Dependency Injection — how it is wired");
P("All seven services are registered once in an extension method, AddInfrastructureServices(), in Data/Extensions/ServiceCollectionExtensions.cs, and called from Program.cs:");
P("services.AddScoped<IMenuService, MenuService>();  // …and the other six", { font: "Consolas", size: 20 });
BULLET("AddScoped = one instance per HTTP request — the correct lifetime for services that use the DbContext.");
BULLET("Controllers receive what they need through their constructor (constructor injection); they ask for the interface (IOrderService), and DI supplies the implementation.");
RUNS([new TextRun({ text: "Why it matters for the exam: ", bold: true }), new TextRun("this is the “loose coupling” criterion. The Web project never says new OrderService(); it depends on the abstraction, so the implementation can change (or be mocked in tests) without touching the controllers.")]);

H2("3.3 The data-shape objects (and why there are several)");
table([2400, 6960], [
  ["Object type", "Purpose"],
  ["Entity", "Maps to a database table (e.g. Order, MenuItem). Used by EF Core only."],
  ["DTO (Data Transfer Object)", "The JSON shape sent to/from the API (e.g. CartDto, MenuItemDto, OrderDto) — keeps the API contract separate from the DB."],
  ["Service Model", "What a service returns to a controller (e.g. MenuItemServiceModel) — decouples services from entities."],
  ["View Model / Form Model", "What a Razor view binds to (e.g. MenuIndexViewModel, CategoryFormModel) — carries exactly the fields a page needs, with validation attributes."],
]);
P("Saying “I don’t expose entities directly to the view or the API — I map them to DTOs / view models” is a strong architecture point.", { italics: true });

// ============================================================
// 4. DOMAIN MODEL
// ============================================================
PB();
H1("4. Domain Model — the 6 entities");
CRIT("Criterion 1.3 (4+ entity models) and 2.5 (CRUD over the main entities).");
table([2200, 4360, 2800], [
  ["Entity", "Key fields", "Relationships"],
  ["ApplicationUser", "FullName, Address, ProfileImageUrl, CreatedOn, IsDeleted (extends IdentityUser)", "1→many Orders, 1→many Reservations"],
  ["Category", "Name (unique), IsDeleted", "1→many MenuItems"],
  ["MenuItem", "Name, Description, Price, ImageUrl, IsAvailable, IsDeleted", "many→1 Category; 1→many OrderItems"],
  ["Order", "OrderedOn, Status (enum), TotalPrice, UserId", "many→1 User; 1→many OrderItems"],
  ["OrderItem", "Quantity, UnitPrice (price snapshot)", "many→1 Order; many→1 MenuItem"],
  ["Reservation", "ReservationDate, PartySize, Status, UserId", "many→1 User"],
]);
RUNS([new TextRun({ text: "OrderStatus enum: ", bold: true }), new TextRun("Pending → Confirmed → Preparing → Ready → Delivered / Cancelled. Stored as a number but serialised as its name in the API.")]);
RUNS([new TextRun({ text: "Important design point — price snapshot: ", bold: true }), new TextRun("OrderItem stores its own UnitPrice at the moment of ordering. If a menu price later changes, past orders keep the price the customer actually paid.")]);
RUNS([new TextRun({ text: "Soft delete: ", bold: true }), new TextRun("instead of really deleting rows, we set IsDeleted = true and hide them with a global query filter. Nothing important is lost, and historical orders still resolve their items.")]);

// ============================================================
// 5. DATABASE / EF CORE
// ============================================================
PB();
H1("5. Database, EF Core, Migrations & Seed Data");
CRIT("Criteria 2.1–2.5 — SQL Server, EF Core, migrations, seed data, CRUD.");
H2("5.1 Code-first with EF Core");
BULLET("We write C# entity classes; EF Core creates the SQL schema from them — this is “code-first”.");
BULLET("ApplicationDbContext (Data/Data) is the gateway: it exposes DbSet<T> for each entity and is queried with LINQ.");
BULLET("LINQ queries are translated to parameterised SQL, which protects against SQL injection automatically.");

H2("5.2 Fluent API configurations (5 files)");
P("Each entity has a configuration class in Data/Data/Configurations (Category, MenuItem, Order, OrderItem, Reservation). They set things the attributes can’t express cleanly:");
BULLET("decimal precision for money (e.g. 18,2), max lengths, and unique indexes (Category.Name).");
BULLET("delete behaviour — e.g. you cannot delete a Category that still has menu items (Restrict).");
BULLET("global soft-delete query filters so deleted rows are hidden everywhere by default.");

H2("5.3 Migrations");
P("A migration is a versioned C# description of a schema change. We created InitialCreate, which builds all tables and indexes. On startup the app calls db.Database.MigrateAsync(), so the database is created/updated automatically — no manual SQL.");
P("Commands used:", { bold: true });
P("dotnet ef migrations add InitialCreate  →  dotnet ef database update", { font: "Consolas", size: 20 });

H2("5.4 Seed data (two kinds)");
table([3000, 6360], [
  ["Seeded via", "What it seeds"],
  ["Migration (DataSeeder)", "The 5 categories and the sample menu items — part of the schema history, so every fresh DB has them."],
  ["Runtime (IdentitySeeder)", "The roles (Admin, Employee, Customer) and the admin account — created on startup, idempotently."],
]);
RUNS([new TextRun({ text: "Default admin login: ", bold: true }), new TextRun({ text: "admin@restaurant.com / Admin@123", font: "Consolas" })]);

H2("5.5 CRUD");
P("Create / Read / Update / Delete is implemented over the main entities through the admin panel and the services: Menu items, Categories, Orders (status updates), Reservations and Users all support the relevant operations.");

H2("5.6 Connecting to SQL Server");
P("The connection string lives in appsettings.json under ConnectionStrings:DefaultConnection and points at the SQL Server instance (localhost,1433). EF Core uses UseSqlServer(connectionString) in Program.cs.");

// ============================================================
// 6. CONTROLLERS, VIEWS, API
// ============================================================
PB();
H1("6. Controllers, Views and the Web API");
CRIT("Criteria 1.2–1.5 (views, 4+ controllers incl. an API controller, Razor) and the AJAX bonus.");
H2("6.1 MVC controllers (return HTML views)");
table([2400, 6960], [
  ["Controller", "Responsibility"],
  ["HomeController", "Landing page, About, Contact, error pages."],
  ["MenuController", "Public menu list (search/filter/sort/paging) and dish details."],
  ["CartController", "Session-based cart (add/update/remove)."],
  ["OrderController", "Checkout, order confirmation, order history & details."],
  ["ReservationController", "Create a table reservation, list my reservations."],
  ["ProfileController", "View and edit the user’s profile."],
  ["AccountController", "Register, Login, Logout, Access Denied (uses Identity)."],
]);
H2("6.2 Admin area (role-protected)");
P("A separate Admin area groups the management screens: Dashboard, Menu, Categories, Orders, Reservations, Users. Areas keep admin routes and views isolated from the public site.");

H2("6.3 Web API controllers (return JSON)");
table([2600, 6760], [
  ["API controller", "Endpoints / purpose"],
  ["MenuApiController", "GET /api/menu (search, filter, sort, page), /api/menu/{id}, /api/menu/categories — public."],
  ["CartApiController", "GET/POST/PUT/DELETE /api/cart — live cart updates, protected by the antiforgery token."],
  ["OrderApiController", "GET /api/orders, /api/orders/{id}, PUT status — for authenticated users / staff."],
]);
RUNS([new TextRun({ text: "AJAX: ", bold: true }), new TextRun("the files wwwroot/js/menu-ajax.js and cart-ajax.js call these endpoints with fetch(), so the menu filters and the cart update without a full page reload — this is the “MVC + Web API in one project” point and also covers the paging/sorting/searching bonus.")]);

// ============================================================
// 7. SECURITY
// ============================================================
PB();
H1("7. Security — Identity, Roles & Authorization");
CRIT("Criterion 2.4 — explain Identity, roles, authorization and how pages/functionality are protected.");
BULLET2; // no-op safety
BULLET("Authentication = who you are. ASP.NET Identity handles registration, hashed passwords, login and account lockout (after 5 failed attempts).");
BULLET("Authorization = what you may do. Implemented with roles and policies.");
table([2600, 6760], [
  ["Mechanism", "Detail"],
  ["Roles", "Admin, Employee, Customer (seeded at startup)."],
  ["Policies", "AdminOnly (Admin) and StaffOnly (Admin or Employee), defined in Program.cs."],
  ["Page protection", "[Authorize] / policy attributes on controllers; guests are redirected to /Account/Login."],
  ["CSRF protection", "Antiforgery tokens on every form; AJAX sends the token in the X-CSRF-TOKEN header."],
  ["Ownership checks", "A user can only see/modify their own orders and reservations."],
  ["Server-side price recompute", "At checkout the total is recalculated on the server — client values are never trusted."],
]);
P("Strong sentence for the defense: “Security is enforced on the server, not in the browser — the client can be tampered with, so authorization, CSRF validation and price calculation all happen server-side.”", { italics: true });

// ============================================================
// 8. FEATURES
// ============================================================
PB();
H1("8. Key Features (what to demo)");
CRIT("Criterion 1.3 — demonstrate the main functionalities; bonus for paging/sorting/searching and extras.");
H2("8.1 Customer side");
["Landing page with hero and featured sections",
 "Menu with live search, category filter, sort (name/price) and pagination (AJAX)",
 "Dish detail pages",
 "Session cart with live add/update/remove and a navbar badge",
 "Checkout → order placement (price recomputed server-side) → confirmation",
 "Order history and order details",
 "Table reservations with future-date validation, plus “my reservations”",
 "Profile view & edit"].forEach(BULLET);
H2("8.2 Admin side (/Admin, role-gated)");
["Dashboard with KPI cards and charts (Chart.js)",
 "Menu items — full CRUD + availability toggle",
 "Categories — CRUD with name-uniqueness and delete-protection",
 "Orders — list, details, status workflow (Pending → … → Delivered/Cancelled)",
 "Reservations — confirm / remove",
 "Users — list, role management, soft-delete (Admin only)"].forEach(BULLET);

// ============================================================
// 9. TESTING
// ============================================================
PB();
H1("9. Testing (unit tests, mocking, code coverage)");
CRIT("Theory 2.5 and Practical 8.1–8.5 — a separate NUnit test project, mocking, business-logic coverage ≥ 70%.");
H2("9.1 What you must be able to explain (theory)");
table([2500, 6860], [
  ["Term", "Meaning"],
  ["Unit test", "A small automated test that checks one piece of logic in isolation."],
  ["NUnit", "The testing framework — [Test], [TestFixture], Assert.That(...)."],
  ["Mocking (Moq)", "Replacing a real dependency (e.g. the DbContext or IHttpContextAccessor) with a fake controlled object so you test only your logic."],
  ["EF Core InMemory", "A fake in-memory database provider used so tests run fast without SQL Server."],
  ["Code coverage", "The % of your code executed by tests. The target here is ≥ 70%, focused on the service layer."],
]);
P("What should be tested: the business logic in the services — order total & price-snapshotting, rejecting unavailable items, cart totals, menu search/filter/sort/paging, category delete-protection & name-uniqueness, reservation rules and ownership checks — NOT trivial getters or the framework.");

H2("9.2 Honest status & action item");
RUNS([new TextRun({ text: "⚠ Current status: ", bold: true, color: RED }),
  new TextRun("this RestaurantProject solution does not yet contain a separate test project. A complete 55-test NUnit + Moq suite already exists for the sibling copy of this codebase and can be ported in. ")]);
RUNS([new TextRun({ text: "Recommended before the defense: ", bold: true }),
  new TextRun("add a RestaurantProject.Tests project (NUnit + Moq + EF InMemory) so criteria 8.1–8.5 (5 points) and theory 2.5 are covered. I can do this for you on request.")]);

// ============================================================
// 10. SOURCE CONTROL & DOCS
// ============================================================
PB();
H1("10. Source Control & Documentation");
CRIT("Criteria 9.1–9.5 and the mandatory conditions — public GitHub repo, README, ≥ 25 commits over ≥ 7 days, no commits after the deadline.");
table([4680, 4680], [
  ["Requirement", "Status / action"],
  ["Public GitHub repository from the start", "Action: ensure the project is pushed to a public repo."],
  ["README.md (description, tech, run instructions, features)", "⚠ Not present in this copy — should be added (a full version exists in the sibling copy)."],
  ["≥ 25 commits across ≥ 7 different days", "Action: verify the commit history meets this."],
  ["No commits after 23:59 on the final date", "Action: stop committing after the deadline."],
]);
P("These are “cheap” points and are partly mandatory conditions — make sure they are done. I can generate a README.md and help with the repository setup if you want.", { italics: true });

// ============================================================
// 11. LIKELY QUESTIONS
// ============================================================
PB();
H1("11. Likely Commission Questions — with answers");
CRIT("Criteria 3.1–3.4 — understand and answer questions, explain your own code, argue your choices.");
const QA = [
  ["Why did you split the project into three parts?", "So each layer has one responsibility and dependencies flow one way (Web → Data → Services). The Core (Services project) has no dependencies, which keeps business contracts independent of the database and the web, and makes the services unit-testable."],
  ["What is the difference between an MVC controller and an API controller?", "An MVC controller returns a Razor view (HTML); an API controller returns data (JSON). I use API controllers for AJAX features like the live cart and menu filtering so the page updates without reloading."],
  ["What is Entity Framework Core and why use it?", "It’s an ORM: I work with C# objects and LINQ instead of writing SQL. It gives me code-first models, migrations, and parameterised queries that are safe from SQL injection."],
  ["What is a migration?", "A versioned C# file describing a schema change. dotnet ef migrations add creates it; database update (or MigrateAsync on startup) applies it. It keeps the database in sync with my models."],
  ["How does dependency injection work here?", "I register interface→implementation pairs as Scoped in AddInfrastructureServices(). Controllers ask for the interface in their constructor and the DI container supplies the concrete service. This gives loose coupling and testability."],
  ["How is the app secured?", "ASP.NET Identity for authentication (hashed passwords, lockout), roles (Admin/Employee/Customer) and policies (AdminOnly/StaffOnly) for authorization, antiforgery tokens for CSRF, ownership checks on orders/reservations, and server-side price recomputation."],
  ["Where is the business logic?", "In the service classes (Data/Services) behind interfaces (Services/Contracts). Controllers are thin — they just call services and return a result."],
  ["Why store UnitPrice on OrderItem?", "It’s a price snapshot. If a dish price changes later, historical orders must keep the price the customer actually paid."],
  ["What is a DTO and why use one?", "A Data Transfer Object — the shape I send over the API. It separates the public contract from my database entities so I don’t leak internal fields and can change the DB without breaking the API."],
  ["How do you do paging/searching/sorting?", "In MenuService I apply the search term, category filter and sort to the IQueryable, then Skip/Take for the current page, and return a PagedResult. The Menu page can call this via the API (AJAX) or a normal request."],
  ["What is mocking and why is it needed in tests?", "Replacing a real dependency with a fake so I test only my logic. For the cart I mock IHttpContextAccessor and the session; for data I use EF Core InMemory."],
  ["What would you improve / what are the alternatives?", "Alternatives I considered: Razor Pages instead of MVC, or a full SPA (React) talking only to the API. I chose MVC + a small API because it matches the assignment and keeps server-side rendering with progressive AJAX enhancement."],
];
QA.forEach(([q, a], i) => {
  kids.push(new Paragraph({ spacing: { before: 120, after: 40 }, children: [new TextRun({ text: `Q${i + 1}. ${q}`, bold: true })] }));
  kids.push(new Paragraph({ spacing: { after: 80 }, children: [new TextRun({ text: "A. ", bold: true, color: RED }), new TextRun(a)] }));
});

// ============================================================
// 12. GLOSSARY
// ============================================================
PB();
H1("12. Glossary of professional terms");
CRIT("Criterion 4.1 — correct use of MVC, Web API, EF Core, Identity, migration, DI, service layer, unit testing, etc.");
const G = [
  ["MVC", "Model–View–Controller — separates data, presentation and request handling."],
  ["Web API", "Controllers that expose data as JSON over HTTP (REST)."],
  ["REST", "A style of HTTP API using verbs (GET/POST/PUT/DELETE) on resources."],
  ["ORM", "Object-Relational Mapper — maps C# classes to database tables (EF Core)."],
  ["Entity Framework Core", "Microsoft’s ORM; code-first, LINQ, migrations."],
  ["DbContext", "The EF class that represents a session with the database."],
  ["Migration", "Versioned schema change generated from the model."],
  ["Seed data", "Initial data inserted into the database (categories, roles, admin)."],
  ["ASP.NET Identity", "Built-in authentication & user/role management."],
  ["Authentication / Authorization", "Who you are / what you’re allowed to do."],
  ["Role / Policy", "A named group (Admin) / a rule built from roles or claims."],
  ["Dependency Injection (DI)", "Supplying objects to a class instead of it creating them."],
  ["Service layer", "Classes holding business logic, used by controllers."],
  ["DTO", "Data Transfer Object — the shape sent over the API."],
  ["View Model", "An object a Razor view binds to, with validation attributes."],
  ["Fluent API", "Configuring EF model rules in code (precision, indexes, relations)."],
  ["CSRF / antiforgery", "Protection against forged cross-site form submissions."],
  ["Unit test", "Automated test of one piece of logic in isolation (NUnit)."],
  ["Mocking", "Replacing a dependency with a controlled fake (Moq)."],
  ["Code coverage", "Percentage of code exercised by tests."],
];
table([2600, 6760], [["Term", "Meaning"], ...G]);

// ============================================================
// 13. ACTION ITEMS
// ============================================================
PB();
H1("13. Before the defense — checklist");
P("Already done / present:", { bold: true });
["ASP.NET Core 8, 3-project architecture, 6 entities, 16 controllers (3 of them API), 30+ Razor views",
 "EF Core code-first, Fluent API, InitialCreate migration, seed data, SQL Server connection",
 "Identity with roles & policies, CSRF, ownership checks, server-side price recompute",
 "Service layer behind interfaces + dependency injection",
 "Paging / sorting / searching (bonus), AJAX cart & menu",
 "Runs and is demonstrable end-to-end"].forEach(BULLET);
SPACER();
P("To add for full marks:", { bold: true, });
[ "A separate NUnit + Moq test project with ≥ 70% service coverage (criteria 8.x, theory 2.5)",
  "A README.md (criterion 9.2)",
  "A public GitHub repo with ≥ 25 commits over ≥ 7 days, none after the deadline (criteria 9.x + mandatory)",
  "Optional bonus: deploy publicly (Azure or similar) for up to 5 bonus points"].forEach((t)=>BULLET([new TextRun({text:t})]));
SPACER();
P("Tip: if the app fails to start during the defense the functional criteria are heavily penalised — make sure the SQL Server connection string is correct and the database migrates on first run.", { italics: true, color: RED });

// ============================================================
// BUILD
// ============================================================
const doc = new Document({
  styles: {
    default: { document: { run: { font: "Calibri", size: 22 } } },
    paragraphStyles: [
      { id: "Heading1", name: "Heading 1", basedOn: "Normal", next: "Normal", quickFormat: true,
        run: { size: 34, bold: true, font: "Calibri", color: RED },
        paragraph: { spacing: { before: 280, after: 160 }, outlineLevel: 0 } },
      { id: "Heading2", name: "Heading 2", basedOn: "Normal", next: "Normal", quickFormat: true,
        run: { size: 27, bold: true, font: "Calibri", color: "1F1F1F" },
        paragraph: { spacing: { before: 200, after: 100 }, outlineLevel: 1 } },
      { id: "Heading3", name: "Heading 3", basedOn: "Normal", next: "Normal", quickFormat: true,
        run: { size: 24, bold: true, font: "Calibri", color: "404040" },
        paragraph: { spacing: { before: 140, after: 80 }, outlineLevel: 2 } },
    ],
  },
  numbering: {
    config: [
      { reference: "bul", levels: [
        { level: 0, format: LevelFormat.BULLET, text: "•", alignment: AlignmentType.LEFT, style: { paragraph: { indent: { left: 540, hanging: 280 } } } },
        { level: 1, format: LevelFormat.BULLET, text: "◦", alignment: AlignmentType.LEFT, style: { paragraph: { indent: { left: 1080, hanging: 280 } } } },
      ] },
    ],
  },
  sections: [{
    properties: { page: { size: { width: 12240, height: 15840 }, margin: { top: 1440, right: 1440, bottom: 1440, left: 1440 } } },
    footers: { default: new Footer({ children: [new Paragraph({ alignment: AlignmentType.CENTER,
      children: [ new TextRun({ text: "Jerry’s — Project Overview & Defense Guide   ·   page ", size: 18, color: "888888" }),
        new TextRun({ children: [PageNumber.CURRENT], size: 18, color: "888888" }) ] })] }) },
    children: kids,
  }],
});

Packer.toBuffer(doc).then((buf) => {
  const out = process.argv[2] || "Jerrys-Project-Overview.docx";
  fs.writeFileSync(out, buf);
  console.log("WROTE " + out + " (" + buf.length + " bytes)");
});
