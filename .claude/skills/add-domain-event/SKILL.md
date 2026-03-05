---
name: add-domain-event
description: Cria um domain event com seu handler, seguindo o padrão de eventos do template
argument-hint: [EventName] [pre|post]
allowed-tools: Read, Write, Edit, Glob, Grep
---

# Criar Domain Event

Gere um domain event chamado **$0** do tipo **$1** (padrão: `post` se não informado).

## Leia antes de gerar

- `src/DiihTemplate.Core/Events/IDomainEvent.cs`
- `src/DiihTemplate.Core/Events/IPreSaveDomainEvent.cs`
- `src/DiihTemplate.Core/Events/IDomainEventHandler.cs`
- `src/DiihTemplate.Core/Events/DomainEventDispatcher.cs`
- `src/DiihTemplate.Core/Entities/IHasEvents.cs`

## Arquivos a gerar

### 1. Event — `src/DiihTemplate.Domain/Events/$0.cs`

Se o argumento `$1` for `pre`, implementar `IPreSaveDomainEvent` (roda na mesma transaction, antes do commit).
Caso contrário, implementar `IDomainEvent` (roda após o commit).

```csharp
using DiihTemplate.Core.Events;

namespace DiihTemplate.Domain.Events;

public class $0 : IDomainEvent // ou IPreSaveDomainEvent
{
    // Propriedades do evento (perguntar ao usuário)

    public $0(/* params */)
    {
        // Atribuições
    }
}
```

### 2. Handler — `src/DiihTemplate.Application/EventHandlers/$0Handler.cs`

```csharp
using DiihTemplate.Core.Events;
using DiihTemplate.Domain.Events;

namespace DiihTemplate.Application.EventHandlers;

public class $0Handler : IDomainEventHandler<$0>
{
    // Injetar dependências necessárias via construtor

    public async Task HandleAsync($0 domainEvent)
    {
        // Lógica do handler
    }
}
```

## Notas

- Handlers são registrados automaticamente pelo `AddDiihTemplateCoreEvents` via reflection
- Para disparar o evento de uma entidade, ela deve implementar `IHasEvents`
- Use `IPreSaveDomainEvent` quando o handler precisa participar da mesma transaction (ex: validação cross-aggregate)
- Use `IDomainEvent` para side effects que podem falhar independentemente (ex: enviar email, notificar)

## Perguntar ao usuário

1. Quais dados o evento deve carregar?
2. Deve rodar antes ou depois do commit?
3. O que o handler deve fazer?
