# HomeTaste — CLAUDE.md

> ⚠️ **Architecture migration in progress.** This codebase is being refactored from the Service-layer + `Result<T>`/`ResultType`/`ApiResponseMapper` architecture documented below into **Clean Architecture with CQRS + MediatR** (feature-based `Application/Features/`, FluentValidation, exception-based error mapping, no `ResultType`, no `ApiResponseMapper`). The full discovery findings, refactoring map, and phased plan live in `plan.md` at the repo root (gitignored, local-only). Until that migration lands feature-by-feature, the conventions below remain accurate for any code that hasn't been converted yet — check `plan.md` for what's already moved before assuming a file still follows the old pattern.

## Project Overview
HomeTaste is an ASP.NET Core 8 Web API platform for a scalable homemade food service. It follows **Clean Architecture** with 4 layers: Domain → Application → Infrastructure → API. The system supports customers, admins, and delivery personnel with JWT-based authentication, meal management, order processing, inventory, reviews, support tickets, and real-time features.

## Solution Structure

```
HomeTaste/
├── HomeTaste.Domain/          # Entities only — no dependencies
├── HomeTaste.Application/     # Business logic, DTOs, interfaces, services, validators
├── HomeTaste.Infrastructure/  # EF Core, Identity, JWT, email, file storage, middleware
├── HomeTaste.API/             # Controllers, Program.cs, appsettings
└── HomeTaste.UnitTests/       # xUnit + Moq unit tests
```

## Build & Run

```bash
# Build entire solution
dotnet build

# Run the API
dotnet run --project HomeTaste.API

# Add EF migration
dotnet ef migrations add <MigrationName> --project HomeTaste.Infrastructure --startup-project HomeTaste.API

# Apply migrations
dotnet ef database update --project HomeTaste.Infrastructure --startup-project HomeTaste.API
```

> **Requires:** .NET 8 SDK, SQL Server (WINDOWS\SQLEXPRESS), dotnet-ef tool

## Architecture Conventions

### Always follow these patterns — no exceptions:

| Concern | Pattern |
|---|---|
| DB access | `_unitOfWork.Repository<T>()` — never inject `ApplicationDbContext` directly |
| Service returns | `Result<T>` from `HomeTaste.Application.Wrappers` |
| Pagination | `PaginatedResponse<T>` with `PaginationMeta` |
| User context | `IUserContextService` inside the **service** — never accept `userId`/`email` as method params when call originates from current HTTP user |
| New entity | Create in `Domain/Entities/` → add `DbSet` in `ApplicationDbContext` → add `IEntityTypeConfiguration<T>` in `Infrastructure/Persistence/Configuration/` → run migration |
| New service | Interface in `Application/Interfaces/` → implementation in `Application/Services/` → register in DI |
| Enums | Always defined in `HomeTaste.Domain/` — never in Application or API |
| Validators | `Application/Validators/` (see note below) |

> **Note:** `Application/Validators/` currently holds hand-rolled `public static class X { public static List<string> Validate(...) }` helpers, not the FluentValidation library — no `FluentValidation` package is referenced anywhere in the solution today. The migration in `plan.md` converts these to real FluentValidation `AbstractValidator<T>` classes run via a MediatR pipeline behavior; until a given feature is converted, follow the existing static-helper convention for consistency with its neighbors.

### Result<T> usage
```csharp
return Result<T>.Ok(data, "Success message");
return Result<T>.Fail("Error message");
return Result<T>.From(resultType, data, "message");
```

Available `ResultType` values: `Success`, `ValidationError`, `Unauthorized`, `Forbidden`, `NotFound`, `Conflict`, `Failure`, `Created`, `NoContent`, `ValidationFailed`, `TooManyRequests`, `BadRequest`, `ServiceUnavailable`

### Layer Dependency Rules
Dependencies flow **inward only** — no exceptions:

```
Domain        ← zero dependencies (no NuGet, no framework references)
Application   ← depends on Domain only
Infrastructure ← depends on Application + Domain
API           ← depends on Application only (never references Infrastructure directly)
```

- API **never** `new`s or imports any Infrastructure concrete type — only resolves via DI
- Infrastructure **never** references API
- Application **never** references EF Core, `HttpContext`, or any framework concern
- Domain entities are **pure POCOs** — no EF attributes, no data annotations, no business logic methods

### EF Core Concurrency — Never use Task.WhenAll
EF Core's scoped `DbContext` is **not thread-safe**. Running multiple service calls concurrently on the same request will throw.

```csharp
// ❌ WRONG
await Task.WhenAll(mealsTask, categoriesTask, ingredientsTask);

// ✅ CORRECT
var meals        = await _mealService.GetAllMealsAsync(...);
var categories   = await _mealCategoryService.GetAllMealCategoriesAsync(...);
var ingredients  = await _ingredientService.GetAllIngredientsAsync(...);
```

### Controller Design Rules
Controllers must be **thin** — no business logic:

- Call service → return `Result<T>` mapped to HTTP response
- Never call `_unitOfWork` directly from a controller
- Authorization via `[Authorize(Roles = "Admin")]` etc. at controller or action level
- All responses use standardized format via `Result<T>`

### Transaction Usage
Use `BeginTransaction / CommitAsync / RollbackAsync` **only** when a single operation mutates more than one aggregate root and partial success is unacceptable.

```csharp
await _unitOfWork.BeginTransaction();
try
{
    // ... multiple repository mutations ...
    await _unitOfWork.SaveChangesAsync();
    await _unitOfWork.CommitAsync();
}
catch
{
    await _unitOfWork.RollbackAsync();
    throw;
}
```

> Single-aggregate saves do **not** need explicit transactions.

### Repository & UnitOfWork — READ ONLY ACCESS
`GenericRepository<T>` and `UnitOfWork` are **locked** — do not modify them.

If a new repository method is genuinely required:
1. Add the method signature to `IRepository<T>` or `IUnitOfWork` in `Application/Interfaces/Persistence/`
2. Add the implementation to `GenericRepository<T>` or `UnitOfWork` in `Infrastructure/Persistence/Repositories/`
3. **Always add an XML doc comment** (`/// <summary>`) to any new method — both on the interface and the implementation
4. Keep the new method consistent with the existing naming and signature patterns

### Entity Configuration Pattern
EF Core relationships and precision are defined in **individual configuration classes**.

```
HomeTaste.Infrastructure/Persistence/Configuration/
```

`ApplicationDbContext.OnModelCreating` calls `builder.ApplyConfigurationsFromAssembly(...)` — adding a new entity config is just a new file, no edits to `ApplicationDbContext`.

## Base Entity
Most domain entities inherit `BaseEntity` (a few root-level ones — `RefreshToken`, `Department`, `CategoryType`, `MealPreference`, `LoginAudit` — currently don't and declare their own `Id`). Enums do **not**.

```csharp
public abstract class BaseEntity
{
    public Guid Id { get; set; }
    public DateTime? CreatedAt { get; set; }
    public Guid? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public Guid? UpdatedBy { get; set; }
    public DateTime? DeletedAt { get; set; }
    public Guid? DeletedBy { get; set; }
}
```

> **Note:** every audit field except `Id` is nullable, including `CreatedAt` — this corrects an earlier version of this document that described a non-nullable `DateTime CreatedAt`. `Id` was consolidated into `BaseEntity` from the 24 entities that previously each redeclared it individually (zero schema/behavior change — confirmed via an EF Core migration dry-run producing an empty `Up()`/`Down()`).

## ApplicationUser
Extends `IdentityUser` with app-specific fields. Lives in `HomeTaste.Infrastructure/Identity/`.  
Application services must **not** reference `IdentityApplicationUser` directly — use `IUserManager`.

```csharp
public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime? DateOfBirth { get; set; }
}
```

## Roles

| Role | Access |
|---|---|
| `Admin` | Full access — users, meals, inventory, analytics, support |
| `Customer` | Register, browse meals, order, pay, review |
| `DeliveryPersonnel` | View and update assigned delivery orders |

Roles are seeded via `IdentitySeeder` on startup.

## Domain Entities

| Entity | Location | Purpose |
|---|---|---|
| `ApplicationUser` | Root | Identity user with profile fields |
| `RefreshToken` | Root | JWT refresh token storage |
| `LoginAudit` | Root | Login history, device info, IP |
| `Units` | Root | Measurement units (kg, g, l) |
| `MealCategory` | MealManagement/ | Category grouping (Vegan, Non-Veg) |
| `Meal` | MealManagement/ | Menu item with price and category |
| `Ingredient` | MealManagement/ | Raw ingredient with allergen flag |
| `MealIngredient` | MealManagement/ | Meal ↔ Ingredient with quantity + unit |
| `MealReview` | MealManagement/ | Rating + feedback per meal per user |
| `MealPreference` | MealManagement/ | User dietary preferences |
| `InventoryItem` | MealManagement/ | Stock item with quantity and price |
| `InventoryTransaction` | MealManagement/ | Stock movement (Restock, OrderUse, Adjustment, Deletion) |
| `SupportTicket` | Support/ | Customer support ticket with priority and status |
| `CategoryType` | Support/ | Support ticket category (Food Quality, Delivery Issue) |
| `Department` | OrganizationDepartment/ | Internal departments (Kitchen, Delivery) |
| `Tasks` | TaskManagement/ | Internal task with priority and status |

## Enums

| Enum | Location | Values |
|---|---|---|
| `TicketStatus` | Domain | `Open=1`, `Resolved=2`, `InProgress=3`, `Closed=4` |
| `TicketPriority` | Domain | `Low=1`, `Medium=2`, `High=3`, `Urgent=4` |
| `TaskPriority` | Domain | `Low=1`, `Medium=2`, `High=3` |
| `TasksStatus` | Domain | `Pending=1`, `InProgress=2`, `Completed=3`, `Cancelled=4` |
| `TransactionType` | Domain | `Restock=1`, `OrderUse=2`, `Adjustment=3`, `Deletion=4` |
| `ResultType` | Application/Wrappers | (see Result<T> section) |

## Tech Stack

| Layer | Technology |
|---|---|
| Framework | ASP.NET Core 8 Web API |
| ORM | Entity Framework Core 8 |
| Raw SQL | Dapper (via UnitOfWork) |
| Database | SQL Server (WINDOWS\SQLEXPRESS) |
| Auth | ASP.NET Identity + JWT Bearer + Refresh Tokens |
| File Storage | Cloudinary + Supabase S3 |
| Email | Gmail SMTP (smtp.gmail.com:587) |
| OAuth | Google OAuth 2.0 |
| Docs | Swagger / OpenAPI (XML comments) |
| Tests | xUnit + Moq |

## Key Files

| File | Purpose |
|---|---|
| `HomeTaste.Infrastructure/Persistence/ApplicationDbContext.cs` | All DbSets; calls `ApplyConfigurationsFromAssembly` |
| `HomeTaste.Infrastructure/Persistence/Repositories/GenericRepository.cs` | Full IRepository<T> implementation — **do not modify** |
| `HomeTaste.Infrastructure/Persistence/Repositories/UnitOfWork.cs` | Transaction + Dapper integration — **do not modify** |
| `HomeTaste.Infrastructure/Identity/IdentitySeeder.cs` | Seed: roles, default admin user |
| `HomeTaste.Infrastructure/Identity/IdentityUserManager.cs` | IUserManager adapter over ASP.NET Identity |
| `HomeTaste.Infrastructure/Services/JwtTokenGenerator.cs` | Access + refresh token generation/validation |
| `HomeTaste.Infrastructure/Services/UserContextService.cs` | IUserContextService — reads claims from HttpContext |
| `HomeTaste.Infrastructure/Services/EmailService.cs` | Gmail SMTP implementation |
| `HomeTaste.Infrastructure/Services/FileStorage/CloudinaryFileStorage.cs` | Image upload/delete via Cloudinary |
| `HomeTaste.Infrastructure/Middleware/ExceptionMiddleware.cs` | Global exception handler |
| `HomeTaste.Application/Wrappers/Result.cs` | Generic Result<T> — used for all service returns |
| `HomeTaste.Application/Wrappers/PaginatedResponse.cs` | Pagination wrapper with metadata |
| `HomeTaste.Application/Common/MessageConstants.cs` | Shared string constants for messages |
| `HomeTaste.API/Program.cs` | DI setup, middleware pipeline, seed on startup |
| `HomeTaste.API/appsettings.json` | Connection strings, JWT, email, Cloudinary config |

## Configuration (appsettings.json)

```json
{
  "ConnectionStrings": { "DefaultConnection": "Server=WINDOWS\\SQLEXPRESS;Database=HomeTaste;..." },
  "JwtSettings": {
    "AccessToken":  { "ExpiryMinutes": 60,   "SigningAlgorithm": "HS256" },
    "RefreshToken": { "ExpiryMinutes": 1440 }
  },
  "EmailSettings": { "SmtpServer": "smtp.gmail.com", "Port": 587 },
  "CloudinarySettings": { "ApiKey": "..." }
}
```

> Never commit real credentials. Use environment variables or user secrets in development.

---

## Unit Tests

### Testing Framework
- **xUnit** — test runner
- **Moq** — mocking library

### What to Test
Focus unit tests on **Application layer services** — this is where all business logic lives.

```
HomeTaste.UnitTests/
└── Services/
    ├── MealServiceTests.cs
    ├── IngredientServiceTests.cs
    ├── MealCategoryServiceTests.cs
    └── ...
```

### What to Mock in Unit Tests

| Dependency | Mock? | Reason |
|---|---|---|
| `IUnitOfWork` | **Always mock** | Abstracts EF Core — never hit real DB in unit tests |
| `IRepository<T>` (via `_unitOfWork.Repository<T>()`) | **Always mock** | Same reason |
| `IUserContextService` | **Always mock** | Reads from HttpContext — not available in unit tests |
| `IEmailService` | **Always mock** | External SMTP — side-effect, slow, unreliable in tests |
| `IFileStorage` | **Always mock** | External Cloudinary/Supabase — network call |
| `IJwtTokenGenerator` | **Always mock** | Crypto operation — not relevant to business logic |
| `IDateTimeService` | **Always mock** | Enables deterministic time-based assertions |
| `ILogger<T>` | **Always mock** (or use `NullLogger<T>`) | No assertions needed on logs in unit tests |

### What NOT to Test in Unit Tests (ignore / skip)

| Component | Why to Skip |
|---|---|
| `GenericRepository<T>` | EF Core internals — requires real DbContext; use integration tests |
| `UnitOfWork` | Transaction management tied to `DbContext` — integration concern |
| `ApplicationDbContext` | Infrastructure — belongs in integration tests |
| `ExceptionMiddleware` | Middleware pipeline — test via integration/functional tests |
| `JwtTokenGenerator` | Crypto — if tested, test as integration with real config |
| `IdentityUserManager` / `IdentitySignInManager` | Thin adapters over ASP.NET Identity — not business logic |
| `IdentitySeeder` | One-time startup seeding — integration concern |
| `CloudinaryFileStorage` | External API — mock `IFileStorage` instead |
| `EmailService` | External SMTP — mock `IEmailService` instead |
| `UserContextService` | HttpContext-bound — mock `IUserContextService` instead |
| `Program.cs` / DI registration | Infrastructure wiring — not unit testable |
| Controllers | Thin by design; test via integration tests or skip if service-tested |
| `CookieService` | HttpContext-bound — integration concern |

### Standard Test Structure

```csharp
public class MealServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<IUserContextService> _userContextMock = new();
    private readonly Mock<IFileStorage> _fileStorageMock = new();
    private readonly MealService _sut;

    public MealServiceTests()
    {
        _sut = new MealService(
            _unitOfWorkMock.Object,
            _userContextMock.Object,
            _fileStorageMock.Object);
    }

    [Fact]
    public async Task GetMealByIdAsync_WhenMealExists_ReturnsOkResult()
    {
        // Arrange
        var mealId = Guid.NewGuid();
        _unitOfWorkMock
            .Setup(u => u.Repository<Meal>().GetByIdAsync(mealId))
            .ReturnsAsync(new Meal { Id = mealId, Name = "Biryani" });

        // Act
        var result = await _sut.GetMealByIdAsync(mealId);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Data);
    }
}
```

### Naming Convention for Tests

```
MethodName_WhenCondition_ExpectedOutcome
```

Examples:
- `GetMealByIdAsync_WhenMealNotFound_ReturnsNotFoundResult`
- `CreateMealAsync_WhenNameIsDuplicate_ReturnsConflictResult`
- `DeleteMealAsync_WhenMealExists_RemovesAndReturnsOk`

---

## Adding a New Feature (Checklist)

1. **Entity** → `HomeTaste.Domain/Entities/<folder>/NewEntity.cs` (extend `BaseEntity`)
2. **Enum** (if needed) → `HomeTaste.Domain/Enums/` or inline in entity file
3. **DbSet** → Add to `ApplicationDbContext.cs`
4. **Configuration** → `HomeTaste.Infrastructure/Persistence/Configuration/NewEntityConfiguration.cs`
5. **Migration** → `dotnet ef migrations add <Name> ...`
6. **DTO** → `HomeTaste.Application/DTOs/<feature>/NewEntityDto.cs`
7. **Interface** → `HomeTaste.Application/Interfaces/<feature>/INewFeatureService.cs`
8. **Service** → `HomeTaste.Application/Services/<feature>/NewFeatureService.cs`
9. **Register** → Add to DI in `ApplicationServiceRegistration.cs`
10. **Validator** → `HomeTaste.Application/Validators/<feature>/NewEntityValidator.cs` (FluentValidation)
11. **Controller** → `HomeTaste.API/Controllers/NewFeatureController.cs`
12. **Unit Tests** → `HomeTaste.UnitTests/Services/NewFeatureServiceTests.cs`
