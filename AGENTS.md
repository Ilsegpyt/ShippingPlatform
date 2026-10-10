# ShippingPlatform — Agent Rules

The existing code is the source of truth. Never invent conventions or introduce new architecture.
When modules differ, document and follow the difference rather than picking one arbitrarily.

---

## Before Implementing Anything

Read the relevant reference files before writing code.

**Module architecture (Domain, Application, Infrastructure):**
- `Src/Modules/Customers/Customers.Domain/Entities/Customer.cs`
- `Src/Modules/Customers/Customers.Application/Commands/RegisterCustomer/Handler.cs`
- `Src/Modules/Customers/Customers.Application/Commands/RegisterCustomer/Validator.cs`
- `Src/Modules/Customers/Infrastructure/Persistence/CustomersDbContext.cs`
- `Src/Modules/Customers/Infrastructure/CustomersModuleServiceCollectionExtensions.cs`

**API endpoint structure:**
- `Src/Api/Modules/Content/ContentEndpoints.cs`
- `Src/Api/Modules/Content/CreateContent/CreateContentEndpoint.cs`
- `Src/Api/Modules/Content/UpdateContent/UpdateContentEndpoint.cs`

**Permissions:**
- `Src/Modules/Identity/Domain/ValueObjects/PermissionKey.cs`
- `Src/Modules/Identity/Infrastructure/Authorization/EndPointAuthorizationExtensions.cs`

**Shared abstractions:**
- `Src/BuildingBlocks/Domain/AggregateRoot.cs`
- `Src/BuildingBlocks/Application/Result.cs`
- `Src/BuildingBlocks/Application/IUnitOfWork.cs`
- `Src/BuildingBlocks/Application/Behaviors/ValidationBehavior.cs`

---

## 1. Module Structure

Every module lives under `Src/Modules/{Module}/`. Add projects only as required.

| Project | Requirement | Purpose |
|---|---|---|
| `{Module}.Domain` | Required for domain logic | Aggregates, domain events, enums, value objects |
| `{Module}.Application` | Required for application use cases | Commands, queries, handlers, validators, repository interfaces |
| `{Module}.Infrastructure` | Required if the module owns data or infrastructure | DbContext, EF configurations, repositories, migrations |
| `{Module}.Contracts` | Conditional: only if other modules need to interact with this module | Public interfaces and DTOs for cross-module access |

The single host is `Src/Api/`. Endpoint registrations live in `Src/Api/Modules/{Module}/`.

---

## 2. Domain Layer

**Primary reference:** `Src/Modules/Customers/Customers.Domain/`

### Aggregates
- Extend `AggregateRoot<Guid>` from `BuildingBlocks.Domain`. Do not implement `IAggregateRoot` manually or duplicate the domain events list. The `Operations` module currently contains a different pattern; do not copy it without checking the actual implementation and its requirements.
- Use a private parameterless constructor when required by EF.
- Use a private full constructor called by a public static factory method, following the reference pattern.
- Properties use `private set` where consistent with the existing domain model.
- State changes go through public domain methods.
- Raise domain events inside domain methods using `RaiseDomainEvent(new {Event}(...))` when domain events are needed.

### Domain Events
- Follow the existing pattern, for example: `public sealed record {Something}Event(..., DateTime OccurredOnUtc) : IDomainEvent`.
- Place events in `{Module}.Domain/Events/`.
- Add domain events only when the module needs to publish events for processing by itself or other modules.

### Soft Delete / Audit
- `ISoftDeletable` and `SoftDeletableEntity<Guid>` are available in `BuildingBlocks.Domain`.
- `SoftDeletableEntity<Guid>` extends `AuditableEntity<Guid>`; use it when both audit and soft-delete behavior are needed.
- `Customer` implements `ISoftDeletable` directly on the aggregate with manual `MarkAsDeleted()`.
- `Operation` extends `SoftDeletableEntity<Guid>` and relies on `OperationsDbContext` to intercept soft deletes and audit fields in `SaveChangesAsync`.
- Both patterns exist. Follow the pattern established in the module being modified unless there is a justified reason to do otherwise.

---

## 3. Application Layer

**Primary reference:** `Src/Modules/Customers/Customers.Application/`

### Abstractions
Place abstractions in `{Module}.Application/Abstractions/`:
- `I{Aggregate}Repository` — read/write operations for the aggregate.
- `I{Module}UnitOfWork : IUnitOfWork` — marker interface implemented by the DbContext when this pattern is used.

### Commands
Use one subfolder per use case under `Commands/{UseCase}/`.

Two file styles coexist in the codebase:
- **Split files** (`Command.cs` + `Handler.cs` + `Validator.cs`) — used in `RegisterCustomer`; preferred for commands with a validator.
- **Single slice file** (`Slice.cs`) — command and handler in one file, used in `UpdateCustomerProfile`.

Follow the style already used in the relevant module or feature.

Typical conventions:
- Command: `public sealed record {UseCase}Command(...) : IRequest<Result>` or `IRequest<Result<TResponse>>`.
- Handler: `public sealed class {UseCase}CommandHandler(...) : IRequestHandler<TCommand, Result>`; use primary-constructor syntax where consistent with the project and return the appropriate `Result` success or failure.
- Validator: `public sealed class {UseCase}CommandValidator : AbstractValidator<TCommand>`; validators are registered and run by `ValidationBehavior<,>` where that pipeline is configured. Follow existing validation behavior rather than throwing manually.

### Queries
Use one subfolder per use case under `Queries/{UseCase}/`:
- Query record and response record in the same file, where that is the established pattern.
- Handler in a separate file, where that is the established pattern.
- Return `Result<TResponse>` or `Result<PagedResult<TResponse>>` as appropriate to the existing conventions.

### Namespace Convention
Follow the namespace pattern used by the relevant module, for example:
- `Content.Application.Content.CreateContent`
- `Customers.Application.Customers.RegisterCustomer`

### Dependency Injection Registration
Follow the existing `{Module}ApplicationServiceCollectionExtensions.cs` pattern when present. Typical registrations include MediatR, `ValidationBehavior<,>`, and FluentValidation validators.

If the module implements a Contracts interface (for example, `ICustomerQueries`), register its implementation and interface according to the existing pattern.

---

## 4. Infrastructure Layer

**Primary reference:** `Src/Modules/Customers/Infrastructure/`

Add an Infrastructure project when the module owns a database or needs infrastructure-specific implementations.

### DbContext
Follow the existing module pattern, for example:

```csharp
public sealed class {Module}DbContext : DbContext, I{Module}UnitOfWork
```

- Add `DbSet<T>` properties for the entities owned by the context, following existing naming conventions.
- `OnModelCreating` should apply configurations from the assembly when that matches the existing pattern.
- If the module uses an Outbox, add `DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();` and configure the table consistently with the existing Outbox implementation.
- If domain events must be persisted for reliable processing, override `SaveChangesAsync` to collect events from aggregate roots and write them as `OutboxMessage` records, following the existing implementation.
- If the module uses `AuditableEntity<T>` or `ISoftDeletable`, handle audit stamping and soft-delete interception according to the established module pattern. `OperationsDbContext` is a reference for that pattern.

### Entity Configurations
Use `IEntityTypeConfiguration<T>` in `Persistence/Configurations/{Entity}Configuration.cs` where this folder structure is established.

- Ignore domain-event collections in EF mapping when required by the aggregate implementation, for example `builder.Ignore(x => x.DomainEvents)`.
- Configure `HasQueryFilter(x => !x.IsDeleted)` for soft-deletable entities.
- Store enums as strings with `.HasConversion<string>()` where this matches the existing model conventions.

### Repositories
Follow the existing repository pattern, for example:

```csharp
public sealed class {Aggregate}Repository({Module}DbContext dbContext)
    : I{Aggregate}Repository
```

- Use primary constructors where consistent with the project.
- Follow existing write method conventions, including synchronous `Add(T entity)` where used.
- Use `.AsNoTracking()` for read-only queries where appropriate.
- Use `.IgnoreQueryFilters()` only when intentionally querying records hidden by query filters.

### Module DI Registration
Follow `{Module}ModuleServiceCollectionExtensions.cs` where this pattern is used. Typically, one `Add{Module}Module(this IServiceCollection services, IConfiguration configuration)` method registers the DbContext, Unit of Work, repositories, and Application registrations.

Register the module from `Program.cs` using the established single-entry-point pattern where applicable. Do not split registrations if the existing module pattern consolidates them.

---

## 5. Contracts Layer (Conditional)

**Reference:** `Src/Modules/Customers/Customers.Contracts/ICustomerQueries.cs`

Add a `{Module}.Contracts` project only if other modules need to interact with this module.

- Expose interfaces and DTOs required by external consumers, such as `I{Entity}Queries` with response records.
- Expose action interfaces (for example, `IIdentityUserRegistrar`) when another module needs to trigger an operation in this module.
- Other modules should reference only `{Module}.Contracts`, not `{Module}.Domain` or `{Module}.Application`, for cross-module access.
- Keep contracts minimal and expose only what external consumers need.

---

## 6. Shared BuildingBlocks

Reuse existing abstractions from `BuildingBlocks` instead of reimplementing them.

| Abstraction | Project |
|---|---|
| `AggregateRoot<TId>`, `Entity<TId>`, `ValueObject` | `BuildingBlocks.Domain` |
| `IDomainEvent : INotification` | `BuildingBlocks.Domain` |
| `ISoftDeletable`, `SoftDeletableEntity<T>`, `AuditableEntity<T>` | `BuildingBlocks.Domain` |
| `OutboxMessage` | `BuildingBlocks.Domain` |
| `Result`, `Result<T>` | `BuildingBlocks.Application` |
| `IUnitOfWork` | `BuildingBlocks.Application` |
| `ICurrentUser` | `BuildingBlocks.Application.Abstractions` |
| `PaginationRequest`, `PagedResult<T>` | `BuildingBlocks.Application` |
| `ValidationBehavior<,>` | `BuildingBlocks.Application.Behaviors` |
| Integration event DTOs | `BuildingBlocks.Contracts/IntegrationEvents/{Module}/` |

Use only the abstractions relevant to the module; do not add unnecessary dependencies.

---

## 7. API Endpoints

**Primary reference:** `Src/Api/Modules/Content/`

Follow the endpoint structure and implementation conventions in `Content`, not the endpoint structure of `Customers`.

Each use case gets its own file and folder, following this pattern:

```text
Src/Api/Modules/{Module}/
├── {Module}Endpoints.cs
└── {UseCase}/
    └── {UseCase}Endpoint.cs
```

### Endpoint File

Follow the actual `Content` endpoint pattern. A typical endpoint is a static class with one `Map(IEndpointRouteBuilder app)` method, and the request record may be co-located in the same file.

```csharp
public static class {UseCase}Endpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.Map{Verb}("/api/{module}/...", async (
            {UseCase}Request request,
            ISender sender,
            CancellationToken ct) =>
        {
            var result = await sender.Send(new {UseCase}Command(...), ct);
            return result.IsSuccess
                ? Results.{SuccessResult}(...)
                : Results.{FailureResult}(result.Error);
        });
    }
}

public sealed record {UseCase}Request(...);
```

Apply authorization according to the endpoint's actual access requirements and the existing `Content` and `Identity` conventions.

### Coordinator

The `{Module}Endpoints.cs` file coordinates use-case endpoint mapping, following the existing `Content` pattern:

```csharp
public static class {Module}Endpoints
{
    public static void Map{Module}Endpoints(this IEndpointRouteBuilder app)
    {
        {UseCase}Endpoint.Map(app);
    }
}
```

### HTTP Return Conventions

Follow the existing endpoint and result patterns. Typical mappings include:

| Situation | Return |
|---|---|
| Successful create | `Results.Ok(result.Value)` or `Results.Created(location, result.Value)` |
| Successful mutation | `Results.NoContent()` |
| Successful read | `Results.Ok(result.Value)` |
| Not found | `Results.NotFound(result.Error)` |
| Business or validation failure | `Results.BadRequest(result.Error)` |

Register endpoints from `Program.cs` using `app.Map{Module}Endpoints();`, where this is the established pattern.

---

## 8. Authentication, Authorization, and Permissions

Follow the existing `Content` and `Identity` conventions for public and protected endpoints.

- Use `.RequirePermission(PermissionCatalog.{Key})` for endpoints that require permission-based authorization.
- Do not force public endpoints to require permissions unnecessarily. Follow existing conventions for public endpoints, including the patterns used in `Content` or `Identity`.
- Permission keys follow `"{module}.{resource}.{action}"` (lowercase, dot-separated) where this convention applies.
- Add new keys as `public static readonly PermissionKey` fields in `PermissionCatalog` at `Src/Modules/Identity/Domain/ValueObjects/PermissionKey.cs`.
- Add new keys to `PermissionCatalog.All`.
- Add keys to `CustomerOwnerPermissions` or `SubAccountPermissions` only when those user types should have access.
- Permission checks are handled by the existing `PermissionAuthorizationHandler` and token-type JWT claim conventions (`internal`, `customer`, `subaccount`, `impersonation`). Do not duplicate this logic in individual endpoints.

---

## 9. Database, Migrations, Outbox, and Background Workers

### Connection Strings
Each module that owns a DbContext should use a named connection string in `appsettings.json`, following the existing project convention. Example:

```json
"{Module}Db": "Server=.;Database=ShippingPlatform;Trusted_Connection=True;TrustServerCertificate=True"
```

### Migrations
Migrations are required whenever a module introduces or changes its database schema.

- Follow the migration location and command conventions of the relevant module.
- Do not run `dotnet ef` commands that create or modify migrations without explicit user approval.
- Propose the command and wait for confirmation before executing it.

Typical migration locations include `{Module}.Infrastructure/Persistence/Migrations/` or `Migrations/` at the Infrastructure project root, depending on the existing module pattern.

### Outbox (Conditional)

Add an Outbox only if the module has domain events that other modules or background jobs must react to and reliable processing is required.

If required, follow the existing implementation, including:
- Adding `DbSet<OutboxMessage>` to the DbContext and configuring the Outbox table consistently with the existing project.
- Persisting events to `OutboxMessages` in `SaveChangesAsync`.
- Defining integration event records in `BuildingBlocks.Contracts/IntegrationEvents/{Module}/` when cross-module integration events are needed.
- Creating `Src/Api/BackgroundJobs/{Module}OutboxProcessorWorker.cs : BackgroundService` if the module requires an Outbox processor.
- Following the existing retry, event filtering, deserialization, publishing, and processed/failed marking conventions.
- Registering the worker in `Program.cs` only when it exists and is required.

Reference: `Src/Api/BackgroundJobs/CustomersOutboxProcessorWorker.cs`

Do not add an Outbox or background worker automatically to every module.

---

## 10. Scope Control and Quality

- Change only what the task requires. Do not refactor, rename, or reformat unrelated code.
- Do not introduce new base classes, patterns, or abstractions unless explicitly requested and approved.
- Do not add Outbox processing, background workers, Contracts projects, or other infrastructure components unless the module's responsibilities require them.
- Add or update migrations when the database schema changes, but obtain explicit approval before running migration commands that modify files.
- If modules differ in how they implement a convention, document the difference and follow the relevant existing pattern.
- Verify the affected project builds successfully and run relevant tests when practical. Do not run commands that modify the project without approval.
