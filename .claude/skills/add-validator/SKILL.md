---
name: add-validator
description: Cria um FluentValidation validator para um DTO existente
argument-hint: [DtoName]
allowed-tools: Read, Write, Edit, Glob, Grep
---

# Criar Validator

Gere um FluentValidation validator para o DTO **$ARGUMENTS**.

## Leia antes de gerar

1. Encontre o DTO: buscar por `$ARGUMENTS.cs` em `src/DiihTemplate.Application.Contracts/`
2. Leia `src/DiihTemplate.Core/Middlewares/ValidationFilter.cs` para entender como validators são executados
3. Leia qualquer validator existente em `src/DiihTemplate.Application/Validators/` como referência

## Arquivo a gerar

### `src/DiihTemplate.Application/Validators/$ARGUMENTSValidator.cs`

```csharp
using DiihTemplate.Application.Contracts;
using FluentValidation;

namespace DiihTemplate.Application.Validators;

public class $ARGUMENTSValidator : AbstractValidator<$ARGUMENTS>
{
    public $ARGUMENTSValidator()
    {
        // Regras baseadas nas propriedades do DTO
    }
}
```

## Convenções

- Validators ficam em `src/DiihTemplate.Application/Validators/`
- São registrados automaticamente pelo `AddValidatorsFromAssembly` em `ApplicationExtensions.cs`
- O `ValidationFilter` executa automaticamente antes dos controllers
- Erros retornam `ResultDto.BadRequest` com erros agrupados por propriedade
- Usar mensagens de erro em português se o projeto usar pt-BR, senão em inglês

## Regras comuns

- `string` → `.NotEmpty().MaximumLength(N)`
- `email` → `.NotEmpty().EmailAddress()`
- `int/decimal` → `.GreaterThan(0)` quando aplicável
- FKs (Guid) → `.NotEmpty()` para campos obrigatórios
- Enums → `.IsInEnum()`

## Perguntar ao usuário

Se as regras de validação não forem óbvias pelas propriedades, pergunte:
1. Tamanho máximo de strings?
2. Campos obrigatórios vs opcionais?
3. Ranges numéricos?
4. Validações customizadas?
