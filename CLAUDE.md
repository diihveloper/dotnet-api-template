# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## What This Is

A .NET 10 project template (`dotnet new diihtemplate`) implementing clean architecture with DDD patterns. The template supports multi-database selection (Postgres/SqlServer) via `template.json` parameters.

## Build & Run Commands

```bash
# Build the entire solution
dotnet build DiihTemplate.sln

# Run the API (default: http://localhost:5029, https://localhost:7068)
dotnet run --project src/DiihTemplate.Api

# Run tests
dotnet test

# Docker (with Postgres)
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

Multi-database via MSBuild conditional compilation (`POSTGRES`/`SQLSERVER` symbols). DbContext auto-handles: UTC DateTime conversion, soft-delete query filters, domain event dispatching on save, lazy loading proxies. Connection resilience with `EnableRetryOnFailure` (3 retries, 5s delay).

## API Features

- **CORS:** Configurable via `Cors:Origins` array in `appsettings.json`
- **Health Checks:** `GET /health` with DbContext connectivity check
- **Rate Limiting:** Fixed window (100 req/min) via `AddRateLimiter`, policy name `"fixed"`
- **Auth:** ASP.NET Identity API endpoints with `/register` disabled

## DI Registration

Services are registered via extension methods called in `Program.cs`:
- `AddDiihTemplateCore()` — repositories, UoW, event dispatcher, generic services, ValidationFilter
- `AddDiihTemplateApplicationServices()` — event handlers, HttpClient, Mapster, FluentValidation validators
- `AddDiihTemplateDbContext()` — EF Core with database selection and retry resilience
- `AddDiihTemplateInfra()` — infrastructure services

## Conventions

- Namespaces follow `DiihTemplate.{Layer}.{Feature}`
- DTOs suffixed with `Dto`, `Request`, `Result`
- `[Searchable]` attribute marks entity properties for query optimization
- Identity uses `AppUser` extending `IdentityUser` with registration endpoint disabled
- Validators go in the Application project, auto-discovered by assembly scanning
