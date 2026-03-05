---
name: add-service
description: Cria um application service customizado com interface, implementação e registro DI
argument-hint: [ServiceName]
allowed-tools: Read, Write, Edit, Glob, Grep
---

# Criar Application Service

Gere um application service chamado **$ARGUMENTS** com interface e implementação.

Use esta skill quando precisar de um service que NÃO é CRUD genérico — por exemplo, services de integração, relatórios, processamento, ou lógica de negócio complexa.

## Leia antes de gerar

- `src/DiihTemplate.Core/Services/` — entender `IAppService` e padrões de base
- `src/DiihTemplate.Application/ApplicationExtensions.cs` — registro de DI
- `src/DiihTemplate.Infra/InfrastructureExtensions.cs` — se for serviço de infraestrutura

## Arquivos a gerar

### 1. Interface — `src/DiihTemplate.Application.Contracts/Services/I$ARGUMENTSService.cs`

```csharp
using DiihTemplate.Core.Services;

namespace DiihTemplate.Application.Contracts.Services;

public interface I$ARGUMENTSService : IAppService
{
    // Métodos definidos pelo usuário
}
```

### 2. Implementação — `src/DiihTemplate.Application/Services/$ARGUMENTSService.cs`

```csharp
using DiihTemplate.Application.Contracts.Services;

namespace DiihTemplate.Application.Services;

public class $ARGUMENTSService : I$ARGUMENTSService
{
    // Injetar dependências via construtor

    // Implementar métodos da interface
}
```

### 3. Registro DI — Editar `src/DiihTemplate.Application/ApplicationExtensions.cs`

Adicionar no método `AddDiihTemplateApplicationServices`:
```csharp
services.AddScoped<I$ARGUMENTSService, $ARGUMENTSService>();
```

## Convenções

- Interface em `Application.Contracts` (acessível por outras camadas)
- Implementação em `Application` (dependências internas)
- Se for serviço de infraestrutura (HTTP client, storage, etc.), colocar implementação em `Infra` e registrar em `InfrastructureExtensions`
- Sempre herdar `IAppService` como marker interface
- Scoped por padrão (use Singleton apenas se stateless e thread-safe)

## Perguntar ao usuário

1. Quais métodos o service deve ter (assinatura)?
2. É um service de aplicação ou infraestrutura?
3. Quais dependências precisa injetar?
