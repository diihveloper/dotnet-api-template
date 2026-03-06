# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## What This Is

A .NET 10 project implementing clean architecture with DDD patterns.

## Build & Run Commands

```bash
# Build the entire solution
dotnet build DiihTemplate.sln

# Run the API (default: http://localhost:5029, https://localhost:7068)
dotnet run --project src/DiihTemplate.Api

# Run tests
dotnet test

# Docker
docker compose up --build

# Restore dependencies
dotnet restore
```

## Architecture

**Layer dependency flow:** Api -> Application -> Domain <- Data/Infra, with Core shared across all layers.

| Project | Role |
|---|---|
| `Api` | Controllers, middleware pipeline (Program.cs configures everything) |
| `Application` | App services, Mapster mappings, FluentValidation validators |
| `Application.Contracts` | DTOs shared between Api and Application |
| `Core` | Base classes, interfaces, generic implementations for entities, repositories, services, events, exceptions, middlewares |
| `Domain` | Business entities (e.g., `AppUser`) |
| `Domain.Shared` | Enums, constants shared across domain boundaries |
| `Data` | EF Core DbContext, database config |
| `Infra` | External service integrations |
| `Tests` | Integration tests with xUnit + WebApplicationFactory |

## Key Patterns

**Entity hierarchy:** `IEntity<TKey>` -> `Entity<TKey>` -> `Auditable<TKey>` -> `FullAuditable<TKey, TUserKey>`. Audit interfaces (`IHasCreationTime`, `ISoftDelete`, etc.) are granular and composable.

**Repository:** `IReadOnlyRepository<TEntity, TKey>` for queries, `IRepository<TEntity, TKey>` for mutations. Methods use `autoSave` parameter convention. `FindAsync` returns null; `GetAsync` throws `EntityNotFoundException`.

**Services:** `IReadOnlyAppService` for queries, `ICrudAppService` for CRUD. Multiple generic overloads support different DTO combinations (separate create/update DTOs or unified).

**Events:** Domain events split into `IPreSaveDomainEvent` (dispatched before commit, same transaction) and `IDomainEvent` (dispatched after commit). Application events use `System.Threading.Channels` with a `HostedService` consumer. Sync `SaveChanges` is blocked (`NotSupportedException`) — always use `SaveChangesAsync`.

**Validation:** FluentValidation with auto-registration from the Application assembly. `ValidationFilter` runs as a global action filter, returning `ResultDto.BadRequest` with grouped errors.

**Middlewares:** `UnitOfWorkMiddleware` wraps non-GET requests in transactions. `HandleExceptionMiddleware` maps custom exceptions (`BusinessException`, `EntityNotFoundException`, etc.) to HTTP status codes via `ResultDto`, with structured logging via `ILogger`.

**Mapping:** Uses Mapster (not AutoMapper). Mappings registered via `TypeAdapterConfig` scanning in `ApplicationExtensions`.

**Metadata:** Entities implementing `IHasMetadata` get a `Dictionary<string, string>` stored as JSON. Extension methods: `SetMetadata<T>`, `GetMetadata<T>`, `TryGetMetadata`, `HasMetadata`, `RemoveMetadata`.

## Database

<!--#if (POSTGRES) -->
Uses **PostgreSQL** with Npgsql provider. Connection string configured via `ConnectionStrings:Default` in `appsettings.json`.
<!--#endif -->
<!--#if (SQLSERVER) -->
Uses **SQL Server** with Microsoft.EntityFrameworkCore.SqlServer provider. Connection string configured via `ConnectionStrings:Default` in `appsettings.json`.
<!--#endif -->

DbContext auto-handles: UTC DateTime conversion, soft-delete query filters, domain event dispatching on save, lazy loading proxies. Connection resilience with `EnableRetryOnFailure` (3 retries, 5s delay).

<!--#if (SMTP || RABBITMQ || STORAGE) -->
## Infrastructure Services

<!--#if (SMTP) -->
**Email (SMTP):** `IEmailSender` interface in Core, `SmtpEmailSender` implementation in Infra using MailKit. Supports HTML body, CC/BCC, attachments. Configure via `Email` section in `appsettings.json` (`Host`, `Port`, `Username`, `Password`, `FromEmail`, `FromName`, `UseSsl`).

<!--#endif -->
<!--#if (RABBITMQ) -->
**Messaging (RabbitMQ):** `IMessageBroker` interface in Core, `RabbitMqMessageBroker` implementation in Infra using RabbitMQ.Client. Supports publish/subscribe with JSON serialization, persistent delivery, and manual ack/nack. Configure via `RabbitMq` section in `appsettings.json` (`HostName`, `Port`, `Username`, `Password`, `VirtualHost`, `DefaultExchange`). Registered as Singleton.

<!--#endif -->
<!--#if (STORAGE) -->
**File Storage:** `IFileStorage` interface in Core, `LocalFileStorage` implementation in Infra. Supports upload, download, delete, and exists operations on local filesystem. Configure via `Storage:BasePath` in `appsettings.json`.

<!--#endif -->
<!--#endif -->
## API Features

- **CORS:** Configurable via `Cors:Origins` array in `appsettings.json`
- **Health Checks:** `GET /health` with DbContext connectivity check
- **Rate Limiting:** Fixed window (100 req/min) via `AddRateLimiter`, policy name `"fixed"`
- **Auth:** ASP.NET Identity API endpoints with `/register` disabled

## DI Registration

Services are registered via extension methods called in `Program.cs`:
- `AddDiihTemplateCore()` — repositories, UoW, event dispatcher, generic services, ValidationFilter
- `AddDiihTemplateApplicationServices()` — event handlers, HttpClient, Mapster, FluentValidation validators
- `AddDiihTemplateDbContext()` — EF Core with database provider and retry resilience
<!--#if (SMTP || RABBITMQ || STORAGE) -->
- `AddDiihTemplateInfra()` — infrastructure services (email, messaging, storage)
<!--#else -->
- `AddDiihTemplateInfra()` — infrastructure services
<!--#endif -->

## Conventions

- Namespaces follow `DiihTemplate.{Layer}.{Feature}`
- DTOs suffixed with `Dto`, `Request`, `Result`
- `[Searchable]` attribute marks entity properties for query optimization
- Identity uses `AppUser` extending `IdentityUser` with registration endpoint disabled
- Validators go in the Application project, auto-discovered by assembly scanning
<!--#if (SMTP || RABBITMQ || STORAGE) -->
- Infrastructure interfaces in `Core/Services/`, implementations in `Infra/{Feature}/`
- Settings classes use `IOptions<T>` pattern with named config sections
<!--#endif -->
