---
name: scaffold-crud
description: Scaffold completo de CRUD - Entity, DTO, EF Config, Validator, Controller e registro no DbContext
argument-hint: [EntityName] [KeyType=Guid]
allowed-tools: Read, Write, Edit, Glob, Grep, Bash
---

# Scaffold CRUD Completo

Gere o scaffold completo para a entidade **$0** com chave do tipo **$1** (padrão: `Guid` se não informado).

## IMPORTANTE: Leia antes de gerar

Antes de gerar qualquer código, leia estes arquivos para entender os padrões exatos do projeto:
- `src/DiihTemplate.Core/Entities/Auditable.cs` (herança de entidade)
- `src/DiihTemplate.Core/Entities/FullAuditable.cs` (se existir)
- `src/DiihTemplate.Core/Dtos/EntityDto.cs` (herança de DTO)
- `src/DiihTemplate.Core/Controllers/BasicCrudController.cs` (herança de controller)
- `src/DiihTemplate.Core/Services/CrudAppService.cs` (herança de service)
- `src/DiihTemplate.Domain/AppUser.cs` (exemplo de entidade)
- `src/DiihTemplate.Application.Contracts/AppUserDto.cs` (exemplo de DTO)
- `src/DiihTemplate.Application/Mappers/AppUserMapping.cs` (exemplo de mapping)
- `src/DiihTemplate.Data/DiihTemplateDbContext.cs` (registro de entidade)

## Arquivos a gerar

### 1. Entity — `src/DiihTemplate.Domain/$0.cs`

```csharp
using DiihTemplate.Core.Entities;

namespace DiihTemplate.Domain;

public class $0 : Auditable<$1>
{
    // Propriedades de negócio (perguntar ao usuário quais)
}
```

- Herdar de `Auditable<$1>` (tem CreatedAt, UpdatedAt, IsDeleted, DeletedAt)
- Se precisar de tracking de usuário, usar `FullAuditable<$1, string>`
- Usar `[Searchable]` nas propriedades que devem ser pesquisáveis
- Propriedades de navegação devem ser `virtual` (lazy loading proxies)

### 2. DTOs — `src/DiihTemplate.Application.Contracts/`

Criar 3 arquivos:
- `$0Dto.cs` — herda de `EntityDto<$1>`, DTO de saída
- `Create$0Dto.cs` — DTO de criação (sem Id)
- `Update$0Dto.cs` — DTO de atualização (sem Id)

```csharp
using DiihTemplate.Core.Dtos;

namespace DiihTemplate.Application.Contracts;

public class $0Dto : EntityDto<$1>
{
    // Propriedades mapeadas da entidade
}

public class Create$0Dto
{
    // Propriedades necessárias para criação
}

public class Update$0Dto
{
    // Propriedades editáveis
}
```

### 3. Mapster Mapping — `src/DiihTemplate.Application/Mappers/$0Mapping.cs`

```csharp
using DiihTemplate.Application.Contracts;
using DiihTemplate.Domain;
using Mapster;

namespace DiihTemplate.Application.Mappers;

public class $0Mapping : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<$0, $0Dto>();
        config.NewConfig<Create$0Dto, $0>();
        config.NewConfig<Update$0Dto, $0>();
    }
}
```

### 4. Validator — `src/DiihTemplate.Application/Validators/Create$0Validator.cs`

```csharp
using DiihTemplate.Application.Contracts;
using FluentValidation;

namespace DiihTemplate.Application.Validators;

public class Create$0Validator : AbstractValidator<Create$0Dto>
{
    public Create$0Validator()
    {
        // Regras de validação
    }
}
```

### 5. Controller — `src/DiihTemplate.Api/Controllers/$0Controller.cs`

```csharp
using DiihTemplate.Application.Contracts;
using DiihTemplate.Core.Controllers;
using DiihTemplate.Core.Dtos;
using DiihTemplate.Core.Services;
using DiihTemplate.Domain;
using Microsoft.AspNetCore.Mvc;

namespace DiihTemplate.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class $0Controller : BasicCrudController<$0, $1, $0Dto, $0Dto, IPagedRequest, Create$0Dto, Update$0Dto>
{
    public $0Controller(
        ICrudAppService<$0, $1, $0Dto, $0Dto, IPagedRequest, Create$0Dto, Update$0Dto> service)
        : base(service)
    {
    }
}
```

### 6. Registrar no DbContext — Editar `src/DiihTemplate.Data/DiihTemplateDbContext.cs`

Adicionar `DbSet<$0>`:
```csharp
public DbSet<$0> $0s { get; set; }
```

### 7. Registrar Repository — Editar `src/DiihTemplate.Data/DataServiceCollectionExtension.cs`

Se necessário, registrar repositório específico no método `AddRepositories`.

## Checklist final

- [ ] Entity criada com propriedades definidas pelo usuário
- [ ] DTOs criados (output, create, update)
- [ ] Mapping Mapster registrado
- [ ] Validator criado com regras básicas
- [ ] Controller criado herdando BasicCrudController
- [ ] DbSet adicionado no DbContext
- [ ] Build compila sem erros (`dotnet build`)

## Interação com o usuário

ANTES de gerar os arquivos, pergunte ao usuário:
1. Quais propriedades a entidade deve ter (nome, tipo, nullable)?
2. Deve herdar de `Auditable` ou `FullAuditable` (tracking de usuário)?
3. Quais propriedades são `[Searchable]`?
4. Alguma propriedade de navegação (FK para outra entidade)?
