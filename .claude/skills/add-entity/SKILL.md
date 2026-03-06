---
name: add-entity
description: Cria uma entidade de dominio com sua classe de configuracao EF Core (IEntityTypeConfiguration) e registro no DbContext
argument-hint: [EntityName] [KeyType=Guid]
allowed-tools: Read, Write, Edit, Glob, Grep, Bash
---

# Criar Entidade de Dominio

Gere uma entidade chamada **$0** com chave do tipo **$1** (padrao: `Guid` se nao informado).

## IMPORTANTE: Leia antes de gerar

Antes de gerar qualquer codigo, leia estes arquivos para entender a hierarquia de entidades e os padroes do projeto:

- `src/DiihTemplate.Core/Entities/IEntity.cs` (interface base)
- `src/DiihTemplate.Core/Entities/Entity.cs` (classe base + Entity<TKey>)
- `src/DiihTemplate.Core/Entities/IAuditable.cs` (interface composta: IHasCreationTime + IHasUpdatedTime + IHasDeletedTime)
- `src/DiihTemplate.Core/Entities/Auditable.cs` (CreatedAt, UpdatedAt, IsDeleted, DeletedAt)
- `src/DiihTemplate.Core/Entities/IFullAuditable.cs` (IAuditable + IHasCreator + IHasUpdater + IHasDeleter)
- `src/DiihTemplate.Core/Entities/FullAuditable.cs` (CreatorId, UpdaterId, DeleterId + navegacao opcional)
- `src/DiihTemplate.Core/Entities/ISoftDelete.cs` (IsDeleted)
- `src/DiihTemplate.Core/Entities/IHasCreationTime.cs` (CreatedAt)
- `src/DiihTemplate.Core/Entities/IHasUpdatedTime.cs` (UpdatedAt)
- `src/DiihTemplate.Core/Entities/IHasDeletedTime.cs` (DeletedAt, extends ISoftDelete)
- `src/DiihTemplate.Core/Entities/IHasMetadata.cs` (Dictionary<string, string> serializado como JSON)
- `src/DiihTemplate.Core/Entities/IHasEvents.cs` (domain events)
- `src/DiihTemplate.Core/Entities/IHasCreator.cs` (CreatorId + navegacao)
- `src/DiihTemplate.Core/Entities/ValueObject.cs` (base para value objects)
- `src/DiihTemplate.Core/Attributes/SearchableAttribute.cs` (marca propriedades para busca)
- `src/DiihTemplate.Domain/AppUser.cs` (exemplo de entidade existente)
- `src/DiihTemplate.Data/DiihTemplateDbContext.cs` (DbContext — usa ApplyConfigurationsFromAssembly para auto-descoberta de IEntityTypeConfiguration)

## Hierarquia de entidades disponivel

O projeto oferece classes base e interfaces composiveis. A escolha depende do nivel de auditoria necessario:

### Classes base (heranca)

| Classe | Herda de | Propriedades |
|---|---|---|
| `Entity<TKey>` | — | `Id` |
| `Auditable<TKey>` | `Entity<TKey>` | `Id`, `CreatedAt`, `UpdatedAt`, `IsDeleted`, `DeletedAt` |
| `FullAuditable<TKey, TUserKey>` | `Auditable<TKey>` | Tudo de Auditable + `CreatorId`, `UpdaterId`, `DeleterId` |
| `FullAuditable<TKey, TUserKey, TUser>` | `FullAuditable<TKey, TUserKey>` | Tudo acima + navegacao `Creator`, `Updater`, `Deleter` |

### Interfaces composiveis (podem ser combinadas livremente)

| Interface | Propriedades | Comportamento automatico |
|---|---|---|
| `IHasCreationTime` | `CreatedAt` | Preenchido automaticamente no `Added` |
| `IHasUpdatedTime` | `UpdatedAt` | Preenchido automaticamente no `Modified`/`Added` |
| `ISoftDelete` | `IsDeleted` | `Delete` vira `Modified` com `IsDeleted = true` + query filter global |
| `IHasDeletedTime` | `DeletedAt` (extends `ISoftDelete`) | `DeletedAt` preenchido automaticamente |
| `IAuditable` | Composicao de `IHasCreationTime` + `IHasUpdatedTime` + `IHasDeletedTime` | Todos acima |
| `IHasCreator<TKey>` | `CreatorId` | FK configurada automaticamente pelo CoreModelBuilder |
| `IHasUpdater<TKey>` | `UpdaterId` | FK configurada automaticamente |
| `IHasDeleter<TKey>` | `DeleterId` | FK configurada automaticamente |
| `IHasMetadata` | `Dictionary<string, string>? Metadata` | Serializado como JSON automaticamente |
| `IHasEvents` | `IEnumerable<IDomainEvent> Events` | Dispatch automatico no SaveChangesAsync |

### Outras opcoes

| Tipo | Uso |
|---|---|
| `ValueObject` | Objetos de valor sem identidade (comparacao por propriedades) |
| `IEntity<TKey>` | Interface pura, sem classe base — para quando a entidade ja herda de outra classe (ex: `IdentityUser`) |

## Perguntar ao usuario ANTES de gerar

1. Quais propriedades a entidade deve ter (nome, tipo, nullable)?
2. Qual nivel de auditoria? Apresentar as opcoes:
   - **`Entity<TKey>`** — apenas Id, sem auditoria
   - **`Auditable<TKey>`** — com CreatedAt, UpdatedAt, soft delete (recomendado para a maioria)
   - **`FullAuditable<TKey, string>`** — com tracking de usuario (CreatorId, UpdaterId, DeleterId)
   - **`FullAuditable<TKey, string, AppUser>`** — com tracking + navegacao para AppUser
   - **Interfaces avulsas** — composicao customizada (ex: `Entity<TKey>` + `IHasCreationTime` + `ISoftDelete`)
3. Quais propriedades devem ter `[Searchable]`?
4. A entidade precisa de `IHasMetadata` (dicionario JSON flexivel)?
5. A entidade precisa de `IHasEvents` (domain events)?
6. Alguma propriedade de navegacao (FK para outra entidade)? Se sim, qual o tipo de relacionamento (1:N, N:1, N:N)?

## Arquivos a gerar

### 1. Entity — `src/DiihTemplate.Domain/$0.cs`

```csharp
using DiihTemplate.Core.Entities;

namespace DiihTemplate.Domain;

public class $0 : Auditable<$1> // ou Entity<$1>, FullAuditable<$1, string>, etc.
{
    // Propriedades de negocio definidas pelo usuario
}
```

Regras:
- Usar a classe base ou interfaces conforme resposta do usuario
- Propriedades de navegacao devem ser `virtual` (lazy loading proxies)
- Marcar propriedades de texto pesquisavel com `[Searchable]`
- Collections de navegacao inicializar como `[] ` (ex: `public virtual ICollection<Filho> Filhos { get; set; } = [];`)
- Se usar `IEntity<TKey>` diretamente (sem herdar Entity<TKey>), implementar `GetKeys()` manualmente

### 2. EF Core Configuration — `src/DiihTemplate.Data/Configurations/$0Configuration.cs`

```csharp
using DiihTemplate.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DiihTemplate.Data.Configurations;

public class $0Configuration : IEntityTypeConfiguration<$0>
{
    public void Configure(EntityTypeBuilder<$0> builder)
    {
        builder.ToTable("$0s");

        builder.HasKey(x => x.Id);

        // Configuracoes de propriedades
        // builder.Property(x => x.Nome).IsRequired().HasMaxLength(200);

        // Indices
        // builder.HasIndex(x => x.Email).IsUnique();

        // Relacionamentos
        // builder.HasMany(x => x.Filhos).WithOne(x => x.Pai).HasForeignKey(x => x.PaiId);
    }
}
```

Regras:
- A classe vai em `src/DiihTemplate.Data/Configurations/` (criar a pasta se nao existir)
- O nome da tabela deve ser o nome da entidade no plural (ex: `$0s`). Para nomes que nao pluralizam bem, perguntar ao usuario
- Configurar `IsRequired()` e `HasMaxLength()` para propriedades string obrigatorias
- Configurar indices para campos que serao usados em consultas frequentes ou que precisam ser unicos
- Configurar relacionamentos (HasOne/HasMany/HasMany-WithMany) conforme navegacoes da entidade
- NAO configurar propriedades de auditoria (CreatedAt, UpdatedAt, IsDeleted, etc.) — sao tratadas automaticamente pelo `CoreModelBuilder`
- NAO configurar Metadata — tratado automaticamente se a entidade implementar `IHasMetadata`
- NAO configurar soft delete query filter — tratado automaticamente para entidades com `ISoftDelete`
- NAO configurar FKs de Creator/Updater/Deleter — tratadas automaticamente pelo `CoreModelBuilder`
- A configuracao e auto-descoberta pelo `ApplyConfigurationsFromAssembly` no `OnModelCreating` do DbContext

### 3. Registrar DbSet — Editar `src/DiihTemplate.Data/DiihTemplateDbContext.cs`

Adicionar a propriedade `DbSet` no DbContext:

```csharp
public DbSet<$0> $0s { get; set; }
```

## Checklist final

- [ ] Entity criada em `src/DiihTemplate.Domain/` com a heranca/interfaces corretas
- [ ] Propriedades definidas com tipos, nullability e atributos adequados
- [ ] Navegacoes marcadas como `virtual`
- [ ] Configuration criada em `src/DiihTemplate.Data/Configurations/` com tabela, chave, propriedades e relacionamentos
- [ ] DbSet adicionado no DbContext
- [ ] Build compila sem erros (`dotnet build`)
